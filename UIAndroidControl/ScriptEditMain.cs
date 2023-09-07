using HAtxLib.UIAutomator;
using HAtxLib.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;

namespace UIAndroidControl {
	public partial class ScriptEditMain : Form {

		private readonly UIMain _main;

		public ScriptEditMain(UIMain main) {
			InitializeComponent();
			CheckForIllegalCrossThreadCalls = false;
			_main = main;
			MoveMain();

			AppListRefreshButton_Click(null, null);
		}

		public void MoveMain() {
			Location = new Point(_main.Location.X + _main.Width, _main.Location.Y);
		}




		#region APP

		private List<string> Applist;

		private void AppSearchBox_TextChanged(object sender, EventArgs e) {
			AppListBox.Items.Clear();
			string text = AppSearchBox.Text;
			if (string.IsNullOrWhiteSpace(text)) {
				foreach (string app in Applist) {
					AppListBox.Items.Add(app);
				}
			} else {
				foreach (string app in Applist) {
					if (app.Contains(text)) {
						AppListBox.Items.Add(app);
					}
				}
			}
		}

		private void AppListRefreshButton_Click(object sender, EventArgs e) {
			Applist = _main.Atx.ADB.AppList();
			AppSearchBox_TextChanged(null, null);
		}
		private void AppListBox_DoubleClick(object sender, EventArgs e) {
			if (AppListBox.SelectedItems.Count > 0) {
				string text = AppListBox.SelectedItems[0].ToString();
				AppPackageBox.Text = text;
			}
		}
		private async void AppStartButton_Click(object sender, EventArgs e) {
			string package = AppPackageBox.Text;
			if (string.IsNullOrWhiteSpace(package)) {
				return;
			}
			await Task.Run(() => _main.Atx.AppStart(package, false, true));
		}

		#endregion

		

		private async void HomeButton_Click(object sender, EventArgs e) {
			await Task.Run(() => _main.Atx.Press(HAtxLib.HAtx.PressKey.Home));
		}

		private async void MenuButton_Click(object sender, EventArgs e) {
			await Task.Run(() => _main.Atx.Press(HAtxLib.HAtx.PressKey.Menu));
		}

		private async void BackButton_Click(object sender, EventArgs e) {
			await Task.Run(() => _main.Atx.Press(HAtxLib.HAtx.PressKey.Back));
		}

		private async void PowerButton_Click(object sender, EventArgs e) {
			await Task.Run(() => _main.Atx.Press(HAtxLib.HAtx.PressKey.Power));
		}

		private async void DumpWindowsButton_Click(object sender, EventArgs e) {
			await Task.Run(() => {
				var temp = DumpWindowsButton.Text;
				DumpWindowsButton.Text = "刷新中..."; 
				try {
					XmlDocument doc = new XmlDocument();
					doc.LoadXml(_main.Atx.DumpWindowHierarchy());
					_main.UpdateNode(doc);
				} catch (Exception) {}
				DumpWindowsButton.Text = temp;
			});
		}

		private void DevelopModeCheckBox_CheckedChanged(object sender, EventArgs e) {
			_main.IsDevelop = DevelopModeCheckBox.Checked;
		}
	}
}
