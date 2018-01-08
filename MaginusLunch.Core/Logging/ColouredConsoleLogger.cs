using System;
using System.Runtime.InteropServices;

namespace MaginusLunch.Logging
{
    internal static class ColouredConsoleLogger
    {
        private static bool LogToConsole = GetStdHandle(-11) != IntPtr.Zero;

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern IntPtr GetStdHandle(int nStdHandle);

        public static void Write(string message, LogLevel logLevel)
        {
            if (!LogToConsole)
            {
                return;
            }
            try
            {
                Console.ForegroundColor = GetColor(logLevel);
                Console.WriteLine(message);
            }
            finally
            {
                Console.ResetColor();
            }
        }

        private static ConsoleColor GetColor(LogLevel logLevel)
        {
            switch (logLevel)
            {
                case LogLevel.Error:
                case LogLevel.Fatal:
                    return ConsoleColor.Red;
                case LogLevel.Warn:
                    return ConsoleColor.DarkYellow;
                case LogLevel.Verbose:
                    return ConsoleColor.Blue;
                default:
                    return ConsoleColor.White;
            }
        }
    }
}
