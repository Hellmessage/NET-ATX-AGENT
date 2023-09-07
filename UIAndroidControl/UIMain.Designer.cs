namespace UIAndroidControl {
	partial class UIMain {
		/// <summary>
		/// 必需的设计器变量。
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// 清理所有正在使用的资源。
		/// </summary>
		/// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
		protected override void Dispose(bool disposing) {
			if (disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows 窗体设计器生成的代码

		/// <summary>
		/// 设计器支持所需的方法 - 不要修改
		/// 使用代码编辑器修改此方法的内容。
		/// </summary>
		private void InitializeComponent() {
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(UIMain));
			this.LoadingLabel = new System.Windows.Forms.Label();
			this.UIBox = new System.Windows.Forms.PictureBox();
			((System.ComponentModel.ISupportInitialize)(this.UIBox)).BeginInit();
			this.SuspendLayout();
			// 
			// LoadingLabel
			// 
			this.LoadingLabel.BackColor = System.Drawing.Color.Transparent;
			this.LoadingLabel.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this.LoadingLabel.Location = new System.Drawing.Point(56, 56);
			this.LoadingLabel.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
			this.LoadingLabel.Name = "LoadingLabel";
			this.LoadingLabel.Size = new System.Drawing.Size(100, 100);
			this.LoadingLabel.TabIndex = 1;
			this.LoadingLabel.Text = "连接中";
			this.LoadingLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// UIBox
			// 
			this.UIBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.UIBox.BackColor = System.Drawing.Color.Transparent;
			this.UIBox.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
			this.UIBox.Image = global::UIAndroidControl.Properties.Resources.loading;
			this.UIBox.Location = new System.Drawing.Point(6, 6);
			this.UIBox.Margin = new System.Windows.Forms.Padding(2);
			this.UIBox.Name = "UIBox";
			this.UIBox.Size = new System.Drawing.Size(200, 200);
			this.UIBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
			this.UIBox.TabIndex = 0;
			this.UIBox.TabStop = false;
			this.UIBox.MouseDown += new System.Windows.Forms.MouseEventHandler(this.UIBox_MouseDown);
			this.UIBox.MouseEnter += new System.EventHandler(this.UIBox_MouseEnter);
			this.UIBox.MouseLeave += new System.EventHandler(this.UIBox_MouseLeave);
			this.UIBox.MouseMove += new System.Windows.Forms.MouseEventHandler(this.UIBox_MouseMove);
			this.UIBox.MouseUp += new System.Windows.Forms.MouseEventHandler(this.UIBox_MouseUp);
			// 
			// UIMain
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.SystemColors.ActiveBorder;
			this.ClientSize = new System.Drawing.Size(212, 212);
			this.Controls.Add(this.LoadingLabel);
			this.Controls.Add(this.UIBox);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.KeyPreview = true;
			this.Margin = new System.Windows.Forms.Padding(2);
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "UIMain";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "安卓控制器";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.UIMain_FormClosing);
			this.Load += new System.EventHandler(this.UIMain_Load);
			this.LocationChanged += new System.EventHandler(this.UIMain_LocationChanged);
			this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.UIMain_KeyDown);
			this.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.UIMain_KeyPress);
			this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.UIMain_KeyUp);
			((System.ComponentModel.ISupportInitialize)(this.UIBox)).EndInit();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.PictureBox UIBox;
		private System.Windows.Forms.Label LoadingLabel;
	}
}

