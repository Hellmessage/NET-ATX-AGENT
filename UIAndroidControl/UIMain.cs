using HAtxLib;
using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;
using UIAndroidControl.Extend;
using WebSocketSharp;

namespace UIAndroidControl {
	public partial class UIMain : Form {
		private HAtx Atx;
		private int BorderWidth; 
		private int BorderHeight;
		private bool InitSize = false;
		private int FPSCount = 0;

		public UIMain() {
			InitializeComponent();
			CheckForIllegalCrossThreadCalls = false;
			Timer timer = new Timer {
				Interval = 1000
			};
			timer.Tick += FPSTick;
			timer.Start();
		}

		private void FPSTick(object sender, EventArgs e) {
			Text = $"安卓控制器 - FPS:{FPSCount}";
			FPSCount = 0;
		}

		private async void UIMain_Load(object sender, EventArgs e) {
			BorderWidth = Width - UIBox.Width;
			BorderHeight = Height - UIBox.Height;
			UIBox.Rotate(5, 20);
			await Task.Run(() => {
				Atx = new HAtx("304155554f363098", false);
				string url = $"{Atx.AtxAgentWs}/minicap";
				Console.WriteLine(url);
				WebSocket client = new WebSocket(url);
				client.OnOpen += MinicapClientOnOpen;
				client.OnMessage += MinicapClientOnMessage;
				client.Connect();
			});
		}

		private void MinicapClientOnMessage(object sender, MessageEventArgs e) {
			if (e.RawData.Length < 200) {
				return;
			}
			using (MemoryStream stream = new MemoryStream(e.RawData)) {
				FPSCount++;
				Image img = Image.FromStream(stream);
				Size size = new Size(img.Width + BorderWidth, img.Height + BorderHeight);
				if (Width != size.Width || Height != size.Height) {
					Size = size;
					UIBox.Width = img.Width;
					UIBox.Height = img.Height;
					if (!InitSize) {
						InitSize = true;
						Location = new Point((Screen.FromControl(this).Bounds.Width - Width) / 2, (Screen.FromControl(this).Bounds.Height - Height) / 2);
					}
				}
				try {
					UIBox.Image = img;
				} catch {

				}
				
			}
		}

		private async void MinicapClientOnOpen(object sender, EventArgs e) {
			UIBox.StopRotate();
			LoadingLabel.Visible = false;
			await Task.Delay(100);
		}

		private const int WM_NCHITTEST = 0x0084;
		private const int HTBORDER = 18;
		protected override void DefWndProc(ref Message m) {
			if (m.Msg == WM_NCHITTEST) {
				// 拦截窗体大小调整的消息，并返回HTBORDER，以阻止窗体的大小调整
				m.Result = (IntPtr)HTBORDER;
			} else {
				base.DefWndProc(ref m);
			}
		}

		private void UIMain_FormClosing(object sender, FormClosingEventArgs e) {
			Process.GetCurrentProcess().Kill();
		}

		private void UIBox_MouseDown(object sender, MouseEventArgs e) {

		}

		private void UIBox_MouseMove(object sender, MouseEventArgs e) {

		}

		private void UIBox_MouseUp(object sender, MouseEventArgs e) {

		}
	}
}
