namespace UsAcRe {
	partial class MainForm {
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing) {
			if(disposing && (components != null)) {
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
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
			this.toolStrip1 = new System.Windows.Forms.ToolStrip();
			this.btnStart = new System.Windows.Forms.ToolStripButton();
			this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
			this.chDetailedSearching = new System.Windows.Forms.ToolStripButton();
			this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.miClearLog = new System.Windows.Forms.ToolStripMenuItem();
			this.miCopyLog = new System.Windows.Forms.ToolStripMenuItem();
			this.txtLog = new System.Windows.Forms.RichTextBox();
			this.toolStrip1.SuspendLayout();
			this.contextMenuStrip1.SuspendLayout();
			this.SuspendLayout();
			// 
			// toolStrip1
			// 
			this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnStart,
            this.toolStripSeparator1,
            this.chDetailedSearching});
			this.toolStrip1.Location = new System.Drawing.Point(0, 0);
			this.toolStrip1.Name = "toolStrip1";
			this.toolStrip1.Size = new System.Drawing.Size(429, 25);
			this.toolStrip1.TabIndex = 0;
			this.toolStrip1.Text = "toolStrip1";
			// 
			// btnStart
			// 
			this.btnStart.CheckOnClick = true;
			this.btnStart.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
			this.btnStart.Image = ((System.Drawing.Image)(resources.GetObject("btnStart.Image")));
			this.btnStart.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.btnStart.Name = "btnStart";
			this.btnStart.Size = new System.Drawing.Size(35, 22);
			this.btnStart.Text = "Start";
			this.btnStart.Click += new System.EventHandler(this.btnStart_Click);
			// 
			// toolStripSeparator1
			// 
			this.toolStripSeparator1.Name = "toolStripSeparator1";
			this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
			// 
			// chDetailedSearching
			// 
			this.chDetailedSearching.CheckOnClick = true;
			this.chDetailedSearching.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
			this.chDetailedSearching.Image = ((System.Drawing.Image)(resources.GetObject("chDetailedSearching.Image")));
			this.chDetailedSearching.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.chDetailedSearching.Name = "chDetailedSearching";
			this.chDetailedSearching.Size = new System.Drawing.Size(106, 22);
			this.chDetailedSearching.Text = "DetailedSearching";
			// 
			// contextMenuStrip1
			// 
			this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.miClearLog,
            this.miCopyLog});
			this.contextMenuStrip1.Name = "contextMenuStrip1";
			this.contextMenuStrip1.Size = new System.Drawing.Size(181, 70);
			// 
			// miClearLog
			// 
			this.miClearLog.Name = "miClearLog";
			this.miClearLog.Size = new System.Drawing.Size(180, 22);
			this.miClearLog.Text = "Clear";
			this.miClearLog.Click += new System.EventHandler(this.miClearLog_Click);
			// 
			// miCopyLog
			// 
			this.miCopyLog.Name = "miCopyLog";
			this.miCopyLog.Size = new System.Drawing.Size(180, 22);
			this.miCopyLog.Text = "Copy";
			// 
			// txtLog
			// 
			this.txtLog.BackColor = System.Drawing.Color.White;
			this.txtLog.ContextMenuStrip = this.contextMenuStrip1;
			this.txtLog.Dock = System.Windows.Forms.DockStyle.Fill;
			this.txtLog.Font = new System.Drawing.Font("Consolas", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this.txtLog.Location = new System.Drawing.Point(0, 25);
			this.txtLog.Name = "txtLog";
			this.txtLog.ReadOnly = true;
			this.txtLog.Size = new System.Drawing.Size(429, 75);
			this.txtLog.TabIndex = 5;
			this.txtLog.Text = "";
			// 
			// MainForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(429, 100);
			this.Controls.Add(this.txtLog);
			this.Controls.Add(this.toolStrip1);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "MainForm";
			this.Text = "User\'s actions repeater";
			this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.MainForm_FormClosed);
			this.Load += new System.EventHandler(this.MainForm_Load);
			this.toolStrip1.ResumeLayout(false);
			this.toolStrip1.PerformLayout();
			this.contextMenuStrip1.ResumeLayout(false);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.ToolStrip toolStrip1;
		private System.Windows.Forms.ToolStripButton btnStart;
		private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
		private System.Windows.Forms.ToolStripMenuItem miClearLog;
		private System.Windows.Forms.ToolStripMenuItem miCopyLog;
		private System.Windows.Forms.RichTextBox txtLog;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
		private System.Windows.Forms.ToolStripButton chDetailedSearching;
	}
}

