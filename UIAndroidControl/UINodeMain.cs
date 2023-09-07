using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;

namespace UIAndroidControl {
	public partial class UINodeMain : Form {
		private readonly UIMain _main;
		private UIPanel UISelected = new UIPanel();

		protected override CreateParams CreateParams {
			get {
				CreateParams cp = base.CreateParams;
				cp.ExStyle |= 0x02000000;
				return cp;
			}
		}

		public UINodeMain(UIMain main) {
			InitializeComponent();
			_main = main;
			CheckForIllegalCrossThreadCalls = false;
			UISelected.BackColor = Color.White;
			UISelected.BorderStyle = BorderStyle.FixedSingle;
			UISelected.Size = new Size(10, 10);
			UISelected.Location = new Point(0, 20);
			UISelected.Visible = false;
			Controls.Add(UISelected);
		}

		internal void MoveMain() {
			Size = _main.PicBox.Size;
			Location = new Point(_main.Location.X  + 14, _main.Location.Y + 37);
		}

		public void OnMouseMove(int x, int y) {
			if (_main.DumpWindows == null) {
				return;
			}
			UISelected.Visible = true;
			XmlNode node = _main.DumpWindows;
			Rectangle rect = new Rectangle((int)(x * _main.Bw), (int)(y * _main.Bh), 1, 1);
			foreach (XmlNode item in node.ChildNodes[1].ChildNodes) {
				XmlNode temp = FindInMouse(item, rect);
				if (temp != null) {
					NodeBounds bounds = NodeBounds.Create(temp.Attributes["bounds"].Value);
					UISelected.Size = new Size((int)(bounds.Width / _main.Bw), (int)(bounds.Height / _main.Bh));
					UISelected.Location = new Point((int)(bounds.X / _main.Bw), (int)(bounds.Y / _main.Bh));
					return;
				}
			}
			UISelected.Size = new Size(1, 1);
			UISelected.Location = new Point(1, 1);
		}

		public XmlNode FindInMouse(XmlNode node, Rectangle rect) {
			if (node.Attributes["bounds"] == null) {
				return null;
			}
			NodeBounds bounds = NodeBounds.Create(node.Attributes["bounds"].Value);
			Rectangle nr = new Rectangle(bounds.X, bounds.Y, bounds.Width, bounds.Height);
			if (nr.Contains(rect)) {
				XmlNode result = node;
				if (result.ChildNodes.Count > 0) {
					foreach(XmlNode temp in result.ChildNodes) {
						XmlNode t = FindInMouse(temp, rect);
						if (t != null) {
							result = t;
						}
					}
				}
				return result;
			}
			return null;
		}

		internal void HidePanel() {
			UISelected.Visible = false;
			Console.WriteLine("HidePanel");
		}

		internal void ShowPanel() {
			//UISelected.Visible = true;
			//Console.WriteLine("ShowPanel");
		}

		private void UINodeMain_Load(object sender, EventArgs e) {

		}

		internal class NodeBounds {
			public readonly static NodeBounds Empty = new NodeBounds();

			[JsonProperty("top")]
			public int Top { get; set; } = 0;
			[JsonProperty("left")]
			public int Left { get; set; } = 0;
			[JsonProperty("bottom")]
			public int Bottom { get; set; } = 0;
			[JsonProperty("right")]
			public int Right { get; set; } = 0;

			[JsonIgnore]
			public int X { get => Left; }
			[JsonIgnore]
			public int Y { get => Top; }
			[JsonIgnore]
			public int Width { get => Right - Left; }
			[JsonIgnore]
			public int Height { get => Bottom - Top; }

			[JsonIgnore]
			public Point CenterPos {
				get {
					return new Point((Width / 2) + X, (Height / 2) + Y);
				}
			}

			public static NodeBounds Create(string value) {
				var sp = value.Split(new string[] { "][" }, StringSplitOptions.None);
				var tl = sp[0].Substring(1).Split(',');
				var br = sp[1].Substring(0, sp[1].Length - 1).Split(',');
				int tx = int.Parse(tl[0]);
				int ty = int.Parse(tl[1]);
				int bx = int.Parse(br[0]);
				int by = int.Parse(br[1]);
				return new NodeBounds() {
					Top = ty,
					Left = tx,
					Bottom = by,
					Right = bx
				};
			}
		}
	}
}
