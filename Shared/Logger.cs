using System;

namespace Shared
{
    public static class Logger
    {
		public static void Log(string s, ConsoleColor color = ConsoleColor.White)
		{
			Console.ForegroundColor = color;
			Console.WriteLine(s);
			Console.ResetColor();
		}
    }
}
