using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace HAtxLib.Catch {
	public class ATXElementException : ATXException {
		public ATXElementException(string info, JObject json) : base($"{info}: {json.ToString(Newtonsoft.Json.Formatting.None)}") { }
		public ATXElementException(string message) : base(message) {}
	}
}
