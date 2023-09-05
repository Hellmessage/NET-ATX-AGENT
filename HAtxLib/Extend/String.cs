using System.Linq;

namespace HAtxLib.Extend {
	public static class String {
		internal static string Strip(this string s, bool trim = true) {
			if (string.IsNullOrWhiteSpace(s)) {
				return "";
			}
			while (s.EndsWith("\r\n")) {
				s = s.Substring(0, s.Length - 2);
			}
			while (s.EndsWith("\n")) {
				s = s.Substring(0, s.Length - 1);
			}
			while (s.EndsWith("\r")) {
				s = s.Substring(0, s.Length - 1);
			}
			if (trim) {
				return s.Trim().TrimStart().TrimEnd();
			} else {
				return s;
			}
		}

		internal static string Unicode(this string str) {
			string result = string.Concat(str.Select(c => IsChinese(c) ? "\\u" + ((int)c).ToString("X4") : c.ToString()));
			return result;
		}

		private static bool IsChinese(char c) {
			// 判断字符是否为Unicode中的CJK（中日韩）统一表意文字范围
			return (c >= '\u4E00' && c <= '\u9FFF') || (c >= '\u3400' && c <= '\u4DBF') || (c >= '\uF900' && c <= '\uFAFF');
		}
	}
}
