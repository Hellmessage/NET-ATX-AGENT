using System;
using System.Threading;

namespace HAtxLib.Utils {
	public class HThread {

		public static Thread Run(Action work) {
			Thread thread = new Thread(new ThreadStart(work));
			thread.Start();
			return thread;
		}

		public static Thread RunAndTimeout(Action work, int timeout) {
			Thread thread = Run(work);
			if (timeout > 0) {
				Run(() => {
					Thread.Sleep(timeout);
					HTry.Run(() => thread.Abort());
				});
			}
			return thread;
		}

	}
}
