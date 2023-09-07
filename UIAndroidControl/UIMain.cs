using HAtxLib;
using Newtonsoft.Json.Linq;
using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using UIAndroidControl.Extend;
using WebSocketSharp;
using Timer = System.Windows.Forms.Timer;

namespace UIAndroidControl {
	public partial class UIMain : Form {
		public HAtx Atx { get; private set; }
		public readonly PictureBox PicBox;
		public readonly UINodeMain UINodeWin;
		public XmlDocument DumpWindows { get; set; } = null;
		public float Bw = 0.0f;
		public float Bh = 0.0f;

		private int BorderWidth; 
		private int BorderHeight;
		private bool InitSize = false;
		private int FPSCount = 0;
		private bool IsConnect = false;
		private bool IsSuccess = false;
		private Size AndroidWindowSize = new Size(0,0);
		private ScriptEditMain ScriptEdit = null;
		private readonly ReaderWriterLockSlim _lock = new ReaderWriterLockSlim();
		private WebSocket MinicapClient;
		private WebSocket MinitouchClient;
		

		public UIMain() {
			InitializeComponent();
			CheckForIllegalCrossThreadCalls = false;
			PicBox = UIBox;
			UINodeWin = new UINodeMain(this);
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
				Atx = new HAtx("304155554f363098");
				Console.WriteLine(Atx.AtxAgentUrl);
				string url = $"{Atx.AtxAgentWs}/minicap";
				MinicapClient = new WebSocket(url);
				MinicapClient.OnOpen += MinicapClientOnOpen;
				MinicapClient.OnMessage += MinicapClientOnMessage;
				MinicapClient.Connect();
				Atx.Press(HAtx.PressKey.VolumeDown);
			});
		}

		#region 远程
		private void MinicapClientOnMessage(object sender, MessageEventArgs e) {
			if (e.RawData.Length < 200) {
				return;
			}
			using (MemoryStream stream = new MemoryStream(e.RawData)) {
				FPSCount++;
				try {
					Image img = Image.FromStream(stream);
					Size size = new Size(img.Width + BorderWidth, img.Height + BorderHeight);
					if (Width != size.Width || Height != size.Height) {
						UIBox.StopRotate();
						LoadingLabel.Visible = false;
						UIBox.WaitRotate();
						Size = size;
						UIBox.Width = img.Width;
						UIBox.Height = img.Height;
						if (!InitSize) {
							InitSize = true;
							Location = new Point((Screen.FromControl(this).Bounds.Width - Width) / 2, (Screen.FromControl(this).Bounds.Height - Height) / 2);
							IsSuccess = true;
							AndroidWindowSize = Atx.GetWindowSize();
							Bw = (float)AndroidWindowSize.Width / img.Width;
							Bh = (float)AndroidWindowSize.Height / img.Height;
						}
						UINodeWin.Focus();
					}
					UIBox.Image = img;
				} catch (Exception ex) {
					Console.WriteLine(ex.ToString());
				}
			}
		}
		private void MinicapClientOnOpen(object sender, EventArgs e) {
			IsConnect = true;
			Task.Run(() => { UINodeWin.ShowDialog(); });
		}
		private bool IsMouseLeftDown = false;
		private async void UIBox_MouseDown(object sender, MouseEventArgs e) {
			if (!IsSuccess) {
				return;
			}
			if (e.Button == MouseButtons.Left) {
				IsMouseLeftDown = true;
				//MinitouchOperation("d", e.X, e.Y);
				await Task.Run(() => Atx.TouchDown(e.X * Bw, e.Y * Bh));
			}
		}
		private long MoveStartTime = 0;
		private async void UIBox_MouseMove(object sender, MouseEventArgs e) {
			if (!IsSuccess) {
				return;
			}
			if (IsDevelop) {
				UINodeWin.OnMouseMove(e.X, e.Y);
			}
			long wait = DateTimeOffset.Now.ToUnixTimeMilliseconds() - MoveStartTime;
			if (wait > 20) {
				MoveStartTime = DateTimeOffset.Now.ToUnixTimeMilliseconds();
				if (IsMouseLeftDown) {
					await Task.Run(() => Atx.TouchMove(e.X * Bw, e.Y * Bh));
				}
			}
		}
		private async void UIBox_MouseUp(object sender, MouseEventArgs e) {
			if (!IsSuccess) {
				return;
			}
			if (e.Button == MouseButtons.Left) {
				IsMouseLeftDown = false;
				//MinitouchOperation("u", e.X, e.Y);
				await Task.Run(() => Atx.TouchUp(e.X * Bw, e.Y * Bh));
			}
		}
		private bool IsCtrlDown = false;
		private void UIMain_KeyDown(object sender, KeyEventArgs e) {
			if (e.KeyCode == Keys.Control || e.KeyValue == 17) {
				IsCtrlDown = true;
			}
		}
		private async void UIMain_KeyUp(object sender, KeyEventArgs e) {
			if (e.KeyCode == Keys.Control || e.KeyValue == 17) {
				IsCtrlDown = false;
			} else {
				if (IsCtrlDown) {
					if (e.KeyCode == Keys.Q && IsConnect) {
						if (ScriptEdit == null) {
							ScriptEdit = new ScriptEditMain(this);
							new Thread(new ThreadStart(() => {
								ScriptEdit.ShowDialog();
								ScriptEdit.Dispose();
								ScriptEdit = null;
							})).Start();
						}
					}
				} else {
					if (e.KeyCode == Keys.Enter) {
						await Task.Run(() => Atx.Press(HAtx.PressKey.Enter));
					} else if (e.KeyCode == Keys.Delete || e.KeyCode == Keys.Back) {
						await Task.Run(() => Atx.Press(HAtx.PressKey.Delete));
					}
				}
			}
			
		}
		private async void UIMain_KeyPress(object sender, KeyPressEventArgs e) {
			await Task.Run(() => Atx.ADB.Shell("input", "text", e.KeyChar.ToString()));
		}
		#endregion

		#region 系统
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
		protected override CreateParams CreateParams {
			get {
				CreateParams cp = base.CreateParams;
				cp.ExStyle |= 0x02000000;
				return cp;
			}
		}

		public bool IsDevelop { get; set; } = false;

		private void UIMain_FormClosing(object sender, FormClosingEventArgs e) {
			Process.GetCurrentProcess().Kill();
		}
		private void UIMain_LocationChanged(object sender, EventArgs e) {
			ScriptEdit?.MoveMain();
			UINodeWin?.MoveMain();
		}
		#endregion

		#region NODE
		public void UpdateNode(XmlDocument doc) {
			//WriteLock();
			DumpWindows = doc;
			//UnWriteLock();
		}

		public void WriteLock() {
			_lock.EnterWriteLock();
		}

		public void UnWriteLock() {
			_lock.ExitWriteLock();
		}

		public void ReadLock() {
			_lock.EnterReadLock();
		}

		public void UnReadLock() {
			_lock.ExitReadLock();
		}

		private bool IsInPanel = false;
		private void UIBox_MouseEnter(object sender, EventArgs e) {
			//if (!IsInPanel) {
			//	IsInPanel = true;
			//	
			//}
			UINodeWin.ShowPanel();
		}

		private void UIBox_MouseLeave(object sender, EventArgs e) {
			//IsInPanel = false;
			UINodeWin.HidePanel();
		}
		#endregion
	}
}