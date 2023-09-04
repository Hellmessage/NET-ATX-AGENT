using System;
using System.Diagnostics;
using System.Xml.Linq;

namespace HAtxLib.Utils {
	public class HRuntime {

		public static void Run(Action action) {
			Run("HRuntime", action);
		}

		public static void Run(string name, Action action) {
			var watch = Stopwatch.StartNew();
			action.Invoke();
			watch.Stop();
			Console.WriteLine($"{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff} {name} 执行时间: {watch.Elapsed.TotalMilliseconds}/ms");
		}

		public static T Run<T>(Func<T> action) {
			return Run("HRuntime", action);
		}

		public static T Run<T>(string name, Func<T> action) {
			var watch = Stopwatch.StartNew();
			T value = action.Invoke();
			watch.Stop();
			Console.WriteLine($"{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff} {name} 执行时间: {watch.Elapsed.TotalMilliseconds}/ms");
			return value;
		}

		public static T Time<T>(Func<T> action, out int usetime) {
			var watch = Stopwatch.StartNew();
			T value = action.Invoke();
			watch.Stop();
			usetime = (int)watch.Elapsed.TotalMilliseconds;
			return value;
		}

	}
}