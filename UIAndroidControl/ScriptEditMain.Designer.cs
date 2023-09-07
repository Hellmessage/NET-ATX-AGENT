namespace UIAndroidControl {
	partial class ScriptEditMain {
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing) {
			if (disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent() {
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ScriptEditMain));
			this.AppListBox = new System.Windows.Forms.ListBox();
			this.AppSearchBox = new System.Windows.Forms.TextBox();
			this.AppListRefreshButton = new System.Windows.Forms.Button();
			this.AppStartButton = new System.Windows.Forms.Button();
			this.AppPackageBox = new System.Windows.Forms.TextBox();
			this.HomeButton = new System.Windows.Forms.Button();
			this.MenuButton = new System.Windows.Forms.Button();
			this.BackButton = new System.Windows.Forms.Button();
			this.PowerButton = new System.Windows.Forms.Button();
			this.DumpWindowsButton = new System.Windows.Forms.Button();
			this.DevelopModeCheckBox = new System.Windows.Forms.CheckBox();
			this.SuspendLayout();
			// 
			// AppListBox
			// 
			this.AppListBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.AppListBox.FormattingEnabled = true;
			this.AppListBox.ItemHeight = 12;
			this.AppListBox.Location = new System.Drawing.Point(12, 294);
			this.AppListBox.Name = "AppListBox";
			this.AppListBox.Size = new System.Drawing.Size(239, 112);
			this.AppListBox.TabIndex = 0;
			this.AppListBox.DoubleClick += new System.EventHandler(this.AppListBox_DoubleClick);
			// 
			// AppSearchBox
			// 
			this.AppSearchBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.AppSearchBox.Location = new System.Drawing.Point(12, 267);
			this.AppSearchBox.Name = "AppSearchBox";
			this.AppSearchBox.Size = new System.Drawing.Size(194, 21);
			this.AppSearchBox.TabIndex = 1;
			this.AppSearchBox.TextChanged += new System.EventHandler(this.AppSearchBox_TextChanged);
			// 
			// AppListRefreshButton
			// 
			this.AppListRefreshButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.AppListRefreshButton.Location = new System.Drawing.Point(212, 267);
			this.AppListRefreshButton.Name = "AppListRefreshButton";
			this.AppListRefreshButton.Size = new System.Drawing.Size(39, 21);
			this.AppListRefreshButton.TabIndex = 2;
			this.AppListRefreshButton.Text = "刷新";
			this.AppListRefreshButton.UseVisualStyleBackColor = true;
			this.AppListRefreshButton.Click += new System.EventHandler(this.AppListRefreshButton_Click);
			// 
			// AppStartButton
			// 
			this.AppStartButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.AppStartButton.Location = new System.Drawing.Point(212, 240);
			this.AppStartButton.Name = "AppStartButton";
			this.AppStartButton.Size = new System.Drawing.Size(39, 21);
			this.AppStartButton.TabIndex = 4;
			this.AppStartButton.Text = "启动";
			this.AppStartButton.UseVisualStyleBackColor = true;
			this.AppStartButton.Click += new System.EventHandler(this.AppStartButton_Click);
			// 
			// AppPackageBox
			// 
			this.AppPackageBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.AppPackageBox.Location = new System.Drawing.Point(12, 240);
			this.AppPackageBox.Name = "AppPackageBox";
			this.AppPackageBox.Size = new System.Drawing.Size(194, 21);
			this.AppPackageBox.TabIndex = 3;
			// 
			// HomeButton
			// 
			this.HomeButton.Location = new System.Drawing.Point(12, 12);
			this.HomeButton.Name = "HomeButton";
			this.HomeButton.Size = new System.Drawing.Size(75, 23);
			this.HomeButton.TabIndex = 5;
			this.HomeButton.Text = "Home";
			this.HomeButton.UseVisualStyleBackColor = true;
			this.HomeButton.Click += new System.EventHandler(this.HomeButton_Click);
			// 
			// MenuButton
			// 
			this.MenuButton.Location = new System.Drawing.Point(93, 12);
			this.MenuButton.Name = "MenuButton";
			this.MenuButton.Size = new System.Drawing.Size(75, 23);
			this.MenuButton.TabIndex = 6;
			this.MenuButton.Text = "Menu";
			this.MenuButton.UseVisualStyleBackColor = true;
			this.MenuButton.Click += new System.EventHandler(this.MenuButton_Click);
			// 
			// BackButton
			// 
			this.BackButton.Location = new System.Drawing.Point(174, 12);
			this.BackButton.Name = "BackButton";
			this.BackButton.Size = new System.Drawing.Size(75, 23);
			this.BackButton.TabIndex = 7;
			this.BackButton.Text = "Back";
			this.BackButton.UseVisualStyleBackColor = true;
			this.BackButton.Click += new System.EventHandler(this.BackButton_Click);
			// 
			// PowerButton
			// 
			this.PowerButton.Location = new System.Drawing.Point(255, 12);
			this.PowerButton.Name = "PowerButton";
			this.PowerButton.Size = new System.Drawing.Size(75, 23);
			this.PowerButton.TabIndex = 8;
			this.PowerButton.Text = "Power";
			this.PowerButton.UseVisualStyleBackColor = true;
			this.PowerButton.Click += new System.EventHandler(this.PowerButton_Click);
			// 
			// DumpWindowsButton
			// 
			this.DumpWindowsButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.DumpWindowsButton.Location = new System.Drawing.Point(257, 383);
			this.DumpWindowsButton.Name = "DumpWindowsButton";
			this.DumpWindowsButton.Size = new System.Drawing.Size(83, 23);
			this.DumpWindowsButton.TabIndex = 9;
			this.DumpWindowsButton.Text = "获取新DUMP";
			this.DumpWindowsButton.UseVisualStyleBackColor = true;
			this.DumpWindowsButton.Click += new System.EventHandler(this.DumpWindowsButton_Click);
			// 
			// DevelopModeCheckBox
			// 
			this.DevelopModeCheckBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.DevelopModeCheckBox.AutoSize = true;
			this.DevelopModeCheckBox.Location = new System.Drawing.Point(346, 387);
			this.DevelopModeCheckBox.Name = "DevelopModeCheckBox";
			this.DevelopModeCheckBox.Size = new System.Drawing.Size(84, 16);
			this.DevelopModeCheckBox.TabIndex = 10;
			this.DevelopModeCheckBox.Text = "开发者模式";
			this.DevelopModeCheckBox.UseVisualStyleBackColor = true;
			this.DevelopModeCheckBox.CheckedChanged += new System.EventHandler(this.DevelopModeCheckBox_CheckedChanged);
			// 
			// ScriptEditMain
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(644, 418);
			this.Controls.Add(this.DevelopModeCheckBox);
			this.Controls.Add(this.DumpWindowsButton);
			this.Controls.Add(this.PowerButton);
			this.Controls.Add(this.BackButton);
			this.Controls.Add(this.MenuButton);
			this.Controls.Add(this.HomeButton);
			this.Controls.Add(this.AppStartButton);
			this.Controls.Add(this.AppPackageBox);
			this.Controls.Add(this.AppListRefreshButton);
			this.Controls.Add(this.AppSearchBox);
			this.Controls.Add(this.AppListBox);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Margin = new System.Windows.Forms.Padding(2);
			this.Name = "ScriptEditMain";
			this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
			this.Text = "开发辅助";
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.ListBox AppListBox;
		private System.Windows.Forms.TextBox AppSearchBox;
		private System.Windows.Forms.Button AppListRefreshButton;
		private System.Windows.Forms.Button AppStartButton;
		private System.Windows.Forms.TextBox AppPackageBox;
		private System.Windows.Forms.Button HomeButton;
		private System.Windows.Forms.Button MenuButton;
		private System.Windows.Forms.Button BackButton;
		private System.Windows.Forms.Button PowerButton;
		private System.Windows.Forms.Button DumpWindowsButton;
		private System.Windows.Forms.CheckBox DevelopModeCheckBox;
	}
}