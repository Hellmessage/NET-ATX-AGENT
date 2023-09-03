using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HAtxLib.Utils {
	internal class HProcess {

		public static string Run(params object[] argv) {
			string command = string.Join(" ", argv);
			return Run(command);
		}

		public static string Run(string cmd) {
			ProcessStartInfo info = new ProcessStartInfo() {
				FileName = "cmd.exe",
				Arguments = $"/c \"{cmd}\"",
				RedirectStandardOutput = true,
				UseShellExecute = false,
				CreateNoWindow = true,
			};
			using (Process process = Process.Start(info)) {
				string output = process.StandardOutput.ReadToEnd();
				process.WaitForExit();
				if (!string.IsNullOrWhiteSpace(output)) {
					return output;
				}
				return $"{output}";
			}
		}
	}
}
