using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HAtxLib.Extend {
    internal static class String {
        internal static string Strip(this string s) {
            if (string.IsNullOrWhiteSpace(s)) {
                return "";
            }
            if (s.EndsWith("\r\n")) {
                s = s.Substring(0, s.Length - 2);
            }
            if (s.EndsWith("\n")) {
                s = s.Substring(0, s.Length - 1);
            }
            if (s.EndsWith("\r")) {
                s = s.Substring(0, s.Length - 1);
            }
            return s.Trim().TrimStart().TrimEnd();
        }
    }
}
