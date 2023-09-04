using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HAtxLib.Utils {
	internal class HLog {
		private string _class;
		private string _name;

		internal static HLog Get<T>(string prefix) {
			return new HLog() {
				_class = typeof(T).Name,
				_name = prefix,
			};
		}

		private HLog() {}



		public void WriteLine(string type, string method, string format, params object[] argv) {
			string str = string.Format(format, argv);
			Console.WriteLine($"{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff} [{_class} - {_name}] <{type}> {method}() --- {str}");
		}
	}
}
