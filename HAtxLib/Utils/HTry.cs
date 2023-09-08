using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HAtxLib.Utils {
	public class HTry {
		public delegate T TryFunc<T>();

		public static void Run(Action @try) {
			Run(@try, null, null);
		}

		public static T Run<T>(TryFunc<T> @try) {
			return Run(@try, null, null);
		}

		public static void Run(Action @try, Action @catch, Action @finally = null) {
			try {
				@try.Invoke();
			} catch {
				@catch?.Invoke();
			} finally {
				@finally?.Invoke();
			}
		}

		public static T Run<T>(TryFunc<T> @try, TryFunc<T> @catch, Action @finally) {
			try {
				return @try.Invoke();
			} catch {
				if (@catch != null) {
					return @catch.Invoke();
				}
				return default;
			} finally {
				@finally?.Invoke();
			}
		}
	}
}
