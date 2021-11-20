﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace Molten.Font
{
    public class FontReader : IDisposable
    {
        class TableEntry
        {
            public Type Type;
            public string[] Dependencies;
        }

        static Dictionary<string, TableEntry> _tableTypes;

        static FontReader()
        {
            _tableTypes = new Dictionary<string, TableEntry>();
            IEnumerable<Type> tableTypes = ReflectionHelper.FindTypesWithAttribute<FontTableTagAttribute>(typeof(FontTableTagAttribute).Assembly);
            foreach (Type t in tableTypes)
            {
                FontTableTagAttribute att = t.GetCustomAttribute<FontTableTagAttribute>();
                _tableTypes.Add(att.Tag, new TableEntry()
                {
                    Type = t,
                    Dependencies = att.Dependencies,
                });
            }
        }

        static FontTable GetTableInstance(TableHeader header)
        {
            if (_tableTypes.TryGetValue(header.Tag, out TableEntry entry))
            {
                FontTable table = Activator.CreateInstance(entry.Type) as FontTable;
                table.Header = header;
                table.Dependencies = entry.Dependencies.Clone() as string[];
                return table;
            }

            return null;
        }

        Stream _stream;
        Logger _log;
        string _filename;
        EnhancedBinaryReader _reader;
        MemoryStream _tableStream;
        byte[] _tableStreamBuffer;

        /// <summary>Creates a new instance of <see cref="FontReader"/>.</summary>
        /// <param name="log">A logger.</param>
        /// <param name="systemFontName">The name of the system font. For example: "Arial", "Times New Roman", "Segoe UI". <para/>
        /// Note that the case-sensitivity of the font name depends on OS pathing rules (e.g. Android/Linux are case-sensitive).</param>
        /// <param name="tableStreamBufferSize">The size of the table stream buffer. By default this is 1MB (1024 bytes (1KB) * 1024KB)</param>
        public FontReader(string systemFontName, Logger log, int tableStreamBufferSize = 1048576)
        {
            _stream = new FileStream(FontFile.GetSystemFontPath(systemFontName), FileMode.Open, FileAccess.Read);
            _stream.Position = 0;
            _filename = systemFontName;
            _log = log;
            _tableStream = new MemoryStream();
            _tableStreamBuffer = new byte[tableStreamBufferSize];
            _reader = GetReader(_stream, false);
        }

        /// <summary>Creates a new instance of <see cref="FontReader"/>.</summary>
        /// <param name="stream">A stream containing from which to read font data.</param>
        /// <param name="log">A logger.</param>
        /// <param name="filename">An optional filename or label to improve log/debug messages.</param>
        /// <param name="leaveOpen">If true, the underlying stream will not be closed or disposed when the <see cref="FontReader"/> is disposed.</param>
        /// <param name="tableStreamBufferSize">The size of the table stream buffer. By default this is 1MB (1024 bytes (1KB) * 1024KB)</param>
        public FontReader(Stream stream, Logger log, string filename = null, bool leaveOpen = false, int tableStreamBufferSize = 1048576)
        {
            _stream = stream;
            _log = log;
            _filename = filename;
            _tableStream = new MemoryStream();
            _tableStreamBuffer = new byte[tableStreamBufferSize];
            _reader = GetReader(_stream, true);
        }

        /// <summary>Parses a .TTF or .OTF font file and returns a new <see cref="FontFile"/> instance containing detailed information about a font.</summary>
        /// <param name="buildFontWhenDone">If true, the font will be built - by calling <see cref="FontFile.Build"/> - when font data has been completely read.</param>
        /// <param name="ignoredTables">One or more tags (of font tables) to be ignored, if any. Ignored tables will not be parsed/loaded.</param>
        public FontFile ReadFont(bool flipYAxis = false, bool buildFontWhenDone = true, params string[] ignoredTables)
        {
            OffsetTable offsetTable;
            FontTableList tables = new FontTableList();

            List<TableHeader> toParse = new List<TableHeader>();
            Dictionary<string, TableHeader> toParseByTag = new Dictionary<string, TableHeader>();

            long fontStartPos = _reader.Position;

            // True-type fonts use big-endian.
            offsetTable = new OffsetTable()
            {
                MajorVersion = _reader.ReadUInt16(),
                MinorVersion = _reader.ReadUInt16(),
                NumTables = _reader.ReadUInt16(),
                SearchRange = _reader.ReadUInt16(),
                EntrySelector = _reader.ReadUInt16(),
                RangeShift = _reader.ReadUInt16(),
            };

            // Read table header entries and calculate where the end of the font file should be.
            long expectedEndPos = fontStartPos;
            for (int i = 0; i < offsetTable.NumTables; i++)
            {
                TableHeader header = ReadTableHeader(_reader);
                expectedEndPos += header.Length;
                bool ignored = false;

                // Check if table is ignored.
                if (ignoredTables != null)
                {
                    for (int j = 0; j < ignoredTables.Length; j++)
                    {
                        if (ignoredTables[j] == header.Tag)
                        {
                            _log.WriteDebugLine($"Ignoring table '{header.Tag}' ({header.Length} bytes)", _filename);
                            ignored = true;
                            break;
                        }
                    }
                }

                if (!ignored)
                {
                    toParse.Add(header);
                    toParseByTag.Add(header.Tag, header);
                }
            }

            // Now parse the tables.
            while (toParse.Count > 0)
            {
                TableHeader header = toParse[toParse.Count - 1];
                LoadTable(fontStartPos, tables, header, toParse, toParseByTag);
            }

            // Spit out warnings for unsupported font tables
            foreach (TableHeader header in tables.UnsupportedTables)
                _log.WriteWarning($"Unsupported table -- {header.ToString()}", _filename);

            /* Jump to the end of the font file data within the stream.
             * Due to table depedency checks, we cannot guarantee the last table to be read is at the end of the font data, so this
             * avoids messing up the stream in a situation where multiple files/fonts/data-sets are held in the same file.*/
            _stream.Position = expectedEndPos;

            FontFile font = new FontFile(tables, flipYAxis);
            if (buildFontWhenDone)
                font.Build();

            return font;
        }

        private void LoadTable(long fontStartPos, FontTableList tables, TableHeader header, List<TableHeader> toParse, Dictionary<string, TableHeader> toParseByTag)
        {
            FontTable table = GetTableInstance(header);
            if (table != null)
            {
                _log.WriteDebugLine($"Supported table '{header.Tag}' found ({header.Length} bytes)", _filename);
                FontTableList dependencies = new FontTableList();
                bool dependenciesValid = true;

                if (table.Dependencies != null && table.Dependencies.Length > 0)
                {
                    _log.WriteDebugLine($"[{header.Tag}] Dependencies: {string.Join(",", table.Dependencies)}");

                    // Attempt to load/retrieve dependency tables before continuing.
                    foreach (string depTag in table.Dependencies)
                    {
                        FontTable dep = tables.Get(depTag);
                        if (dep == null)
                        {
                            if (toParseByTag.TryGetValue(depTag, out TableHeader depHeader))
                            {
                                _log.WriteDebugLine($"[{header.Tag}] Attempting to load missing dependency '{depTag}'");
                                LoadTable(fontStartPos, tables, depHeader, toParse, toParseByTag);
                                dep = tables.Get(depTag);
                                if (dep == null)
                                {
                                    _log.WriteDebugLine($"[{header.Tag}] Dependency '{depTag}' failed to load correctly. Unable to load table.");
                                    dependenciesValid = false;
                                    break;
                                }
                            }
                            else
                            {
                                _log.WriteDebugLine($"[{header.Tag}] Missing dependency '{depTag}'. Unable to load table.");
                                dependenciesValid = false;
                                break;
                            }
                        }

                        _log.WriteDebugLine($"[{header.Tag}] Dependency '{depTag}' found");
                        dependencies.Add(dep);
                    }
                }

                if (dependenciesValid)
                {
                    // Move to the start of the table and parse it.
                    _reader.Position = fontStartPos + header.FileOffset;
                    FillTableStream(header);
                    using (EnhancedBinaryReader tableReader = GetReader(_tableStream, true))
                    {
                        table.Read(tableReader, header, _log, dependencies);
                        tables.Add(table);

                        long expectedEnd = header.StreamOffset + header.Length;
                        long readerPos = tableReader.Position;
                        long posDif = readerPos - expectedEnd;

                        if (expectedEnd != readerPos)
                            _log.WriteDebugLine($"Parsed table '{header.Tag}' -- [MISMATCH] End pos (byte): {readerPos}. Expected: {header.Length}. Dif: {posDif} bytes", _filename);
                        else
                            _log.WriteDebugLine($"Parsed table '{header.Tag}' -- [PASS]", _filename);
                    }
                }
            }
            else
            {
                tables.AddUnsupported(header);
            }

            // Successful or not, we're done with the current table.
            toParse.Remove(header);
            toParseByTag.Remove(header.Tag);
        }

        private EnhancedBinaryReader GetReader(Stream stream, bool leaveOpen)
        {
            if (BitConverter.IsLittleEndian)
                return new FlippedBinaryReader(stream, leaveOpen);
            else
                return new EnhancedBinaryReader(stream, leaveOpen);
        }

        private void FillTableStream(TableHeader header)
        {
            _tableStream.Position = 0;
            long bytesRemaining = header.Length;
            while (bytesRemaining > 0)
            {
                int toCopy = (int)Math.Min(bytesRemaining, _tableStreamBuffer.Length);
                bytesRemaining -= _stream.Read(_tableStreamBuffer, 0, toCopy);
                _tableStream.Write(_tableStreamBuffer, 0, toCopy);
            }
            _tableStream.Position = 0;
        }

        private TableHeader ReadTableHeader(EnhancedBinaryReader reader)
        {
            uint tagCode = reader.ReadUInt32();
            char[] tagChars = new char[4]
            {
                (char)((tagCode & 0xff000000) >> 24),
                (char)((tagCode & 0xff0000) >> 16),
                (char)((tagCode & 0xff00) >> 8),
                (char)(tagCode & 0xff)
            };

            return new TableHeader()
            {
                Tag = new string(tagChars).Trim(),
                CheckSum = reader.ReadUInt32(),
                FileOffset = reader.ReadUInt32(),
                Length = reader.ReadUInt32(),
            };
        }

        public void Dispose()
        {
            _reader.Close();
            _stream.Dispose();
            _tableStream.Dispose();
            _tableStreamBuffer = null;
        }
    }
}
