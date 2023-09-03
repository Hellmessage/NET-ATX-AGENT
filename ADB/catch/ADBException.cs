using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HAtxLib.ADB {
	public class ADBException : Exception {
		public ADBException(string message) : base(message) { }
	}
}
