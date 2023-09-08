using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Dynamic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace HAtxLib.Utils {
	public class HLog {
		public static bool InDebug = false;
		private readonly static object _lock = new object();
		private string _class;
		private string _name;

		[DllImport("kernel32.dll")]
		internal static extern bool AllocConsole();

		[DllImport("kernel32.dll")]
		internal static extern bool FreeConsole();

		public static void OpenConsole() {
			AllocConsole();
		}

		public static void CloseConsole() {
			FreeConsole();
		}

		public static HLog Get<T>(string prefix) {
			return new HLog() {
				_class = typeof(T).Name,
				_name = prefix,
			};
		}

		private HLog() {}


		public void Info(string format, params object[] argv) {
			Log(LogLevel.Info, format, argv);
		}

		public void Debug(string format, params object[] argv) {
			if (InDebug) {
				Log(LogLevel.Debug, format, argv);
			}
		}

		public void Warn(string format, params object[] argv) {
			Log(LogLevel.Warn, format, argv);
		}

        public void Error(string format, params object[] argv) {
            Log(LogLevel.Error, format, argv);
        }

        private void Log(LogLevel level, string format, params object[] argv) {
			StackTrace st = new StackTrace(true);
			StackFrame sf = st.GetFrame(2);
			WriteLine(level, sf.GetMethod().Name, format, argv);
		}

		internal void WriteLine(LogLevel level, string method, string format, params object[] argv) {
			lock(_lock) {
				string str = format;
				if (argv.Length > 0) {
					str = string.Format(str, argv);
				}
				Console.ForegroundColor = GetConsoleColor(level);
				Console.WriteLine($"{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff} [{_class}-{_name}] <{level}> {method}() --- {str}");
				Console.ResetColor();
			}
		}

		private static ConsoleColor GetConsoleColor(LogLevel level) {
			switch (level) {
				case LogLevel.Debug:
					return ConsoleColor.Magenta;
				case LogLevel.Info:
					return ConsoleColor.White;
				case LogLevel.Warn:
					return ConsoleColor.Yellow;
				case LogLevel.Error:
					return ConsoleColor.Red;
				case LogLevel.Remind:
					return ConsoleColor.Green;
			}
			return ConsoleColor.Gray;
		}

		

		public enum LogLevel {
			None = 0,
			Debug = 1,
			Info = 2,
			Error = 3,
			Warn = 4,
			Remind = 5
		}
	}


}
