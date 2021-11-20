﻿namespace Molten.Font
{
    /// <summary>
    /// A base class for font sub-tables. Automatically positions the binary reader at the correct byte offset based on the parent table.
    /// </summary>
    public abstract class FontSubTable : IFontTable
    {
        /// <summary>
        /// Gets the parent <see cref="FontTable"/>.
        /// </summary>
        public IFontTable Parent { get; private set; }

        /// <summary>
        /// Gets the sub-table's header. Only the tag and offset are populated by default.
        /// </summary>
        public TableHeader Header { get; internal set; }

        /// <summary>Creates a new instance of a <see cref="FontSubTable"/></summary>
        /// <param name="reader">A binary reader to read the table.</param>
        /// <param name="log">A logger.</param>
        /// <param name="parent">The parent table.</param>
        /// <param name="offset">The offset in bytes from the start of the parent table.</param>
        internal FontSubTable(EnhancedBinaryReader reader, Logger log, IFontTable parent, long offset)
        {
            Parent = parent;
            Header = new TableHeader()
            {
                StreamOffset = parent.Header.StreamOffset + offset,
                FileOffset = parent.Header.FileOffset + offset,
                Tag = GetType().Name,
                TableDepth = parent.Header.TableDepth + 1,
            };

            //log.WriteDebugLine($"{new string('\t', Header.TableDepth)}[{parent.Header.Tag}] Reading sub-table {Header.Tag} at table pos {Header.StreamOffset}");
            reader.Position = Header.StreamOffset;
        }

        protected uint GetLocalOffset(EnhancedBinaryReader reader)
        {
            return (uint)(reader.Position - Header.StreamOffset);
        }

        protected void SetLocalOffset(EnhancedBinaryReader reader, uint offset)
        {
            reader.Position = Header.StreamOffset + offset;
        }
    }
}
