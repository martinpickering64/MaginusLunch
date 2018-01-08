using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace MaginusLunch.Logging
{
    public class RollingLogger
    {
        public Func<DateTime> GetDate = () => DateTime.Now.Date;
        protected string _currentFilePath;
        private long _currentFileSize;
        private DateTime _lastWriteDate;
        private readonly long _maxFileSize;
        private readonly int _numberOfArchiveFilesToKeep;
        private readonly string _targetDirectory;
        private const long FileLimitInBytes = 10485760;

        public RollingLogger(string targetDirectory, int numberOfArchiveFilesToKeep = 10, long maxFileSize = 10485760)
        {
            _targetDirectory = targetDirectory;
            _numberOfArchiveFilesToKeep = numberOfArchiveFilesToKeep;
            _maxFileSize = maxFileSize;
        }

        public void Write(string message)
        {
            SyncFileSystem();
            InnerWrite(message);
        }

        private void InnerWrite(string message)
        {
            try
            {
                AppendLine(message);
            }
            catch (Exception ex)
            {
                Trace.WriteLine($"MaginusLunch.RollingLogger Could not write to log file '{_currentFilePath}'. Exception: {ex}");
            }
        }

        protected virtual void AppendLine(string message)
        {
            var contents = message + Environment.NewLine;
            File.AppendAllText(_currentFilePath, contents, Encoding.UTF8);
            _currentFileSize = _currentFileSize + contents.Length;
        }

        private void SyncFileSystem()
        {
            if (!HasCurrentDateChanged() && !IsCurrentFileTooLarge()) { return; }
            var today =_lastWriteDate =  GetDate();
            var list = GetRollingLogFiles(_targetDirectory).ToList();
            CalculateNewFileName(list, today);
            PurgeOldFiles(list);
        }

        private bool HasCurrentDateChanged()
        {
            return GetDate() != _lastWriteDate;
        }

        private bool IsCurrentFileTooLarge()
        {
            return _currentFileSize > _maxFileSize;
        }

        private void PurgeOldFiles(List<RollingLogger.LogFile> logFiles)
        {
            foreach (var path in GetFilesToDelete(logFiles))
            {
                try
                {
                    File.Delete(path);
                }
                catch (Exception ex)
                {
                    InnerWrite($"MaginusLunch.RollingLogger Could not purge log file '{path}'. Exception: {ex}");
                }
            }
        }

        private IEnumerable<string> GetFilesToDelete(IEnumerable<LogFile> logFiles) 
            => logFiles.OrderByDescending(x => x.DatePart)
                .ThenByDescending(x => x.SequenceNumber)
                .Select(x => x.Path)
                .Skip(_numberOfArchiveFilesToKeep);

        public static LogFile GetTodaysNewest(IEnumerable<LogFile> logFiles, DateTime today) 
            => logFiles.Where(x => x.DatePart == today)
                .OrderByDescending(x => x.SequenceNumber)
                .FirstOrDefault();

        private static IEnumerable<LogFile> GetRollingLogFiles(string targetDirectory)
        {
            foreach (string enumerateFile in Directory.EnumerateFiles(targetDirectory, searchPattern: "mlLog_*.txt"))
            {
                if (TryDeriveLogInformationFromPath(enumerateFile, out LogFile logFile))
                {
                    yield return logFile;
                }
            }
        }

        private static bool TryDeriveLogInformationFromPath(string file, out LogFile logFile)
        {
            logFile = null;
            var strArray = Path.GetFileNameWithoutExtension(file).Split('_');
            if (strArray.Length != 3 
                || !TryParseDate(strArray[1], out DateTime dateTime) 
                || !int.TryParse(strArray[2], out int result))
            {
                return false;
            }

            logFile = new LogFile()
            {
                DatePart = dateTime,
                SequenceNumber = result,
                Path = file
            };
            return true;
        }

        private static bool TryParseDate(string datePart, out DateTime dateTime) 
            => DateTime.TryParseExact(datePart, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out dateTime);

        private void CalculateNewFileName(List<LogFile> logFiles, DateTime today)
        {
            var todaysNewest = GetTodaysNewest(logFiles, today);
            int num;
            if (todaysNewest == null)
            {
                _currentFileSize = 0L;
                num = 0;
            }
            else
            {
                var length = new FileInfo(todaysNewest.Path).Length;
                if (length > _maxFileSize)
                {
                    num = todaysNewest.SequenceNumber + 1;
                    _currentFileSize = length;
                }
                else
                {
                    num = todaysNewest.SequenceNumber;
                    _currentFileSize = 0L;
                }
            }
            _currentFilePath = Path.Combine(_targetDirectory, $"mlLog_{today.ToString("yyyy-MM-dd")}_{num}.txt");
        }

        public class LogFile
        {
            public DateTime DatePart;
            public string Path;
            public int SequenceNumber;
        }
    }
}
