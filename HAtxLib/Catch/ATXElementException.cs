using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HAtxLib.Catch {
	public class ATXElementException : ATXException {
		public ATXElementException(string message) : base(message) {
		}
	}
}
