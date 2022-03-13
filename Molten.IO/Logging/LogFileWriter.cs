﻿using System;
using System.IO;
using System.Text;

namespace Molten
{
    public class LogFileWriter : ILogOutput
    {
        /// <summary>
        /// Represents the various naming modes available when creating a log file.
        /// </summary>
        public enum NamingMode
        {
            /// <summary>
            /// The log filename will be populated with an counter value which is incremented until a file is successfully opened for writing.
            /// </summary>
            Incremental = 0,

            /// <summary>
            /// 
            /// </summary>
            DateTime = 1,
        }

        Stream _stream;
        StreamWriter _writer;
        string _strFormat = "[{0}] {1}";
        bool _disposed;

        /// <summary>
        /// Initializes a new instance of the <see cref="LogFileWriter"/> class.
        /// </summary>
        /// <param name="path">The file name format. Must contain string.format syntax for providing an entry for incremental value or date.</param>
        /// <param name="bufferSize">The file stream buffer size.</param>
        public LogFileWriter(string path = "Logs/log.txt", NamingMode nameMode = NamingMode.Incremental, int bufferSize = 1024)
        {
            bool success = false;
            int appendVal = 1;

            LogFileInfo = new FileInfo(path);
            if (LogFileInfo.Directory != null && !LogFileInfo.Directory.Exists)
                LogFileInfo.Directory.Create();

            string fnShort = LogFileInfo.Name.Replace(LogFileInfo.Extension, "");
            string fileName = $"{LogFileInfo.Directory}/{fnShort}{LogFileInfo.Extension}";

            // Open a usable log file. Keep trying until one successfully opens. 
            // Generally this only fails if a log with the same name is already open for writing elsewhere in the engine or OS.
            switch (nameMode)
            {
                case NamingMode.Incremental:
                    while (success == false)
                    {

                        try
                        {
                            _stream = new FileStream(fileName, FileMode.Create, FileAccess.Write, FileShare.Read, bufferSize);
                            success = true;
                        }
                        catch
                        {
                            fileName = $"{LogFileInfo.Directory}/{fnShort}_{appendVal}{LogFileInfo.Extension}";
                            appendVal++;
                        }
                    }
                    break;

                case NamingMode.DateTime:
                    string formattedDate = DateTime.Now.ToString("yyyy-mm-dd_HH-mm-ss");
                    fileName = $"{LogFileInfo.Directory}/{fnShort}_{formattedDate}{LogFileInfo.Extension}";
                    break;
            }

            // Store the successfully opened log writer.
            _writer = new StreamWriter(_stream, Encoding.UTF8);
        }

        /// <summary>
        /// Saves to file.
        /// </summary>
        private void SaveToFile()
        {
            _writer.Flush();
            _writer.Close();
        }

        /// <summary>Closes the underlying file stream and disposes of the <see cref="LogFileWriter"/>.</summary>
        public void Dispose()
        {
            if (_disposed)
                return;

            _disposed = true;
            SaveToFile();
            _stream.Dispose();
        }

        /// <summary>
        /// Clears the log output.
        /// </summary>
        public void Clear() { }

        /// <summary>
        /// Writes the specified text to the log output and terminates it with a new line..
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="color">The color.</param>
        public void WriteLine(string text, Color color)
        {
            string line = string.Format(_strFormat, DateTime.Now.ToLongTimeString(), text);
            _writer.WriteLine(line);
        }

        /// <summary>
        /// Writes the specified text to the log output.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="color">The color.</param>
        public void Write(string text, Color color)
        {
            string line = string.Format(_strFormat, DateTime.Now.ToLongTimeString(), text);
            _writer.Write(line);
        }

        public FileInfo LogFileInfo { get; }
    }
}