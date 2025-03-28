using System;

namespace FindingHospitalsAutomation.Utilities.Logger
{
    public static class Log
    {
        public static void Info(string message)
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine($"{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff} | INFO | {message}");
            Console.ResetColor();
        }

        public static void Error(string message)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff} | ERROR | {message}");
            Console.ResetColor();
        }

        public static void Warning(string message)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff} | WARN  | {message}");
            Console.ResetColor();
        }
    }
}
