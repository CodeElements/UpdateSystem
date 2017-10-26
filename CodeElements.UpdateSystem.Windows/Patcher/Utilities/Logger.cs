using System;
using System.IO;

namespace CodeElements.UpdateSystem.Windows.Patcher.Utilities
{
    /// <summary>
    ///     A very simple logger class. Not thread-safe!
    /// </summary>
    public class Logger
    {
        private readonly string _dateTimeFormat;
        private readonly StreamWriter _streamWriter;

        /// <summary>
        ///     Initialize a new instance of <see cref="Logger" />
        /// </summary>
        /// <param name="path">the file path to the log file</param>
        public Logger(string path)
        {
            _dateTimeFormat = "yyyy-MM-dd HH:mm:ss.fff";
            var exists = File.Exists(path);
            _streamWriter = new StreamWriter(path, true);

            if (exists)
                Info("Appending to existing log file");
        }

        /// <summary>
        ///     Log a debug message
        /// </summary>
        /// <param name="text">Message</param>
        public void Debug(string text)
        {
            WriteFormattedLog(LogLevel.Debug, text);
        }

        /// <summary>
        ///     Log an error message
        /// </summary>
        /// <param name="text">Message</param>
        public void Error(string text)
        {
            WriteFormattedLog(LogLevel.Error, text);
        }

        /// <summary>
        ///     Log a fatal error message
        /// </summary>
        /// <param name="text">Message</param>
        public void Fatal(string text)
        {
            WriteFormattedLog(LogLevel.Fatal, text);
        }

        /// <summary>
        ///     Log an info message
        /// </summary>
        /// <param name="text">Message</param>
        public void Info(string text)
        {
            WriteFormattedLog(LogLevel.Info, text);
        }

        /// <summary>
        ///     Log a waning message
        /// </summary>
        /// <param name="text">Message</param>
        public void Warning(string text)
        {
            WriteFormattedLog(LogLevel.Warning, text);
        }

        /// <summary>
        ///     Format a log message based on log level
        /// </summary>
        /// <param name="level">Log level</param>
        /// <param name="text">Log message</param>
        private void WriteFormattedLog(LogLevel level, string text)
        {
            string pretext;
            switch (level)
            {
                case LogLevel.Info:
                    pretext = DateTime.Now.ToString(_dateTimeFormat) + " [INFO]    ";
                    break;
                case LogLevel.Debug:
                    pretext = DateTime.Now.ToString(_dateTimeFormat) + " [DEBUG]   ";
                    break;
                case LogLevel.Warning:
                    pretext = DateTime.Now.ToString(_dateTimeFormat) + " [WARNING] ";
                    break;
                case LogLevel.Error:
                    pretext = DateTime.Now.ToString(_dateTimeFormat) + " [ERROR]   ";
                    break;
                case LogLevel.Fatal:
                    pretext = DateTime.Now.ToString(_dateTimeFormat) + " [FATAL]   ";
                    break;
                default:
                    pretext = "";
                    break;
            }

            _streamWriter.WriteLine(pretext + text);
        }

        private enum LogLevel
        {
            Info,
            Debug,
            Warning,
            Error,
            Fatal
        }
    }
}