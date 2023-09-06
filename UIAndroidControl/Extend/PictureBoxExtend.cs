using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace UIAndroidControl.Extend {
	internal static class PictureBoxExtend {

		private readonly static ConcurrentDictionary<PictureBox, bool> Dict = new ConcurrentDictionary<PictureBox, bool>();

		internal static void Rotate(this PictureBox view, float angle, int wait = 20) {
			if (Dict.ContainsKey(view)) {
				return;
			}
			if (view.Image == null) {
				return;
			}
			if (Dict.TryAdd(view, true)) {
				Task.Run(async () => {
					Image backup = view.Image;
					float a = 0;
					while (Dict.TryGetValue(view, out bool running) && running) {
						a += angle;
						if (a == 360) {
							a = 0;
						}
						view.Image = RotateImage(backup, a);
						await Task.Delay(wait);
					}
					Dict.TryRemove(view, out _);
				});
			}
		}

		internal static void StopRotate(this PictureBox view) {
			if (Dict.ContainsKey(view)) {
				Dict.TryUpdate(view, false, true);
			}
		}



		private static Image RotateImage(Image image, float angle) {
			Bitmap rotatedBitmap = new Bitmap(image.Width, image.Height);
			rotatedBitmap.SetResolution(image.HorizontalResolution, image.VerticalResolution);
			using (Graphics g = Graphics.FromImage(rotatedBitmap)) {
				g.TranslateTransform(image.Width / 2, image.Height / 2);
				g.RotateTransform(angle);
				g.TranslateTransform(-image.Width / 2, -image.Height / 2);
				g.DrawImage(image, new Point(0, 0));
			}
			return rotatedBitmap;
		}
	}
}
