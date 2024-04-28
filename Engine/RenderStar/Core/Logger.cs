using RenderStar.Util;
using System.Diagnostics;
using System.Reflection;

namespace RenderStar.Core
{
    public enum LogLevel
    {
        Information,
        Debugging,
        Warning,
        Error,
        FatalError
    }

    public static class Logger
    {
        private static DateTime time = DateTime.Now;

        public static void WriteConsole(string message, LogLevel level)
        {
            time = DateTime.Now;
            string formattedTime = time.ToString("HH':'ss':'mm");

            StackTrace stackTrace = new();

            Type? declaringType = stackTrace.GetFrame(1)!.GetMethod()!.DeclaringType;
            string className = declaringType != null ? declaringType.Name : "Unknown";

            switch (level)
            {
                case LogLevel.Information:
                    Console.WriteLine(ColorFormatter.Format($"&2&l[{formattedTime}&2] [Thread/Information] [{className}&2]: {message}&r"));
                    break;

                case LogLevel.Debugging:
                    Console.WriteLine(ColorFormatter.Format($"&1&l[{formattedTime}] [Thread/Debugging] [{className}]: {message}&r"));
                    break;

                case LogLevel.Warning:
                    Console.WriteLine(ColorFormatter.Format($"&6&l[{formattedTime}] [Thread/Warning] [{className}]: {message}&r"));
                    break;

                case LogLevel.Error:
                    Console.WriteLine(ColorFormatter.Format($"&4&l[{formattedTime}] [Thread/Error] [{className}]: {message}&r"));
                    break;

                case LogLevel.FatalError:
                    Console.WriteLine(ColorFormatter.Format($"&4[{formattedTime}] [Thread/Fatal Error] [{className}]: {message}&r"));
                    Environment.Exit(-1);
                    break;
            }
        }

        public static void ThrowError(string unexpected, string message, bool fatal = false)
        {
            StackTrace stackTrace = new(1, true);

            StackFrame frame = stackTrace.GetFrame(0)!;
            MethodBase method = frame.GetMethod()!;
            Type? declaringType = method.DeclaringType;
            string className = declaringType != null ? declaringType.Name : "Unknown";
            int lineNumber = frame.GetFileLineNumber();

            string location = className != "Unknown" ? $"{className}::{lineNumber}" : "Unknown location";

            WriteConsole($"Unexpected '{unexpected}', at: '{location}' {message}", fatal ? LogLevel.FatalError : LogLevel.Error);
        }
    }
}