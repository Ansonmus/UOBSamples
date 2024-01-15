using System;
using Microsoft.Web.WebView2.WinForms;

namespace UOL.UnifeedIEWebBrowserWinForms
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			var resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
			splitContainer1 = new System.Windows.Forms.SplitContainer();
			browser = new WebView2();
			btnDownload = new System.Windows.Forms.Button();
			tbnStartClear = new System.Windows.Forms.Button();
			btnStartWithLastObject = new System.Windows.Forms.Button();
			btnClearLog = new System.Windows.Forms.Button();
			btnReset = new System.Windows.Forms.Button();
			logBox = new System.Windows.Forms.TextBox();
			lblLastObject = new System.Windows.Forms.Label();
			webView1 = new Microsoft.Toolkit.Forms.UI.Controls.WebViewCompatible();
			((System.ComponentModel.ISupportInitialize)splitContainer1).BeginInit();
			splitContainer1.Panel1.SuspendLayout();
			splitContainer1.Panel2.SuspendLayout();
			splitContainer1.SuspendLayout();
			SuspendLayout();
			// 
			// splitContainer1
			// 
			splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
			splitContainer1.Location = new System.Drawing.Point(0, 0);
			splitContainer1.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
			splitContainer1.Name = "splitContainer1";
			splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
			// 
			// splitContainer1.Panel1
			// 
			splitContainer1.Panel1.Controls.Add(browser);
			// 
			// splitContainer1.Panel2
			// 
			splitContainer1.Panel2.Controls.Add(lblLastObject);
			splitContainer1.Panel2.Controls.Add(btnDownload);
			splitContainer1.Panel2.Controls.Add(tbnStartClear);
			splitContainer1.Panel2.Controls.Add(btnStartWithLastObject);
			splitContainer1.Panel2.Controls.Add(btnClearLog);
			splitContainer1.Panel2.Controls.Add(btnReset);
			splitContainer1.Panel2.Controls.Add(logBox);
			splitContainer1.Size = new System.Drawing.Size(1107, 519);
			splitContainer1.SplitterDistance = 306;
			splitContainer1.SplitterWidth = 5;
			splitContainer1.TabIndex = 0;
			// 
			// browser
			// 
			browser.AllowExternalDrop = true;
			browser.CreationProperties = null;
			browser.DefaultBackgroundColor = System.Drawing.Color.White;
			browser.Dock = System.Windows.Forms.DockStyle.Fill;
			browser.Location = new System.Drawing.Point(0, 0);
			browser.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
			browser.Name = "browser";
			browser.Size = new System.Drawing.Size(1107, 306);
			browser.TabIndex = 0;
			browser.ZoomFactor = 1D;
			// 
			// btnDownload
			// 
			btnDownload.Location = new System.Drawing.Point(206, 9);
			btnDownload.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
			btnDownload.Name = "btnDownload";
			btnDownload.Size = new System.Drawing.Size(88, 26);
			btnDownload.TabIndex = 4;
			btnDownload.Text = "Download (Revit Obj)";
			btnDownload.UseVisualStyleBackColor = true;
			btnDownload.Click += btnDownload_Click;
			// 
			// tbnStartClear
			// 
			tbnStartClear.Location = new System.Drawing.Point(110, 9);
			tbnStartClear.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
			tbnStartClear.Name = "tbnStartClear";
			tbnStartClear.Size = new System.Drawing.Size(88, 26);
			tbnStartClear.TabIndex = 3;
			tbnStartClear.Text = "Restart";
			tbnStartClear.UseVisualStyleBackColor = true;
			tbnStartClear.Click += tbnStartClear_Click;
			// 
			// btnStartWithLastObject
			// 
			btnStartWithLastObject.Enabled = false;
			btnStartWithLastObject.Location = new System.Drawing.Point(445, 9);
			btnStartWithLastObject.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
			btnStartWithLastObject.Name = "btnStartWithLastObject";
			btnStartWithLastObject.Size = new System.Drawing.Size(135, 26);
			btnStartWithLastObject.TabIndex = 2;
			btnStartWithLastObject.Text = "Start with last object";
			btnStartWithLastObject.UseVisualStyleBackColor = true;
			btnStartWithLastObject.Click += btnStartWithLastObject_Click;
			// 
			// btnClearLog
			// 
			btnClearLog.Location = new System.Drawing.Point(15, 9);
			btnClearLog.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
			btnClearLog.Name = "btnClearLog";
			btnClearLog.Size = new System.Drawing.Size(88, 26);
			btnClearLog.TabIndex = 1;
			btnClearLog.Text = "Clear log";
			btnClearLog.UseVisualStyleBackColor = true;
			btnClearLog.Click += btnClearLog_Click;
			// 
			// btnReset
			// 
			btnReset.Location = new System.Drawing.Point(302, 9);
			btnReset.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
			btnReset.Name = "btnReset";
			btnReset.Size = new System.Drawing.Size(135, 26);
			btnReset.TabIndex = 4;
			btnReset.Text = "Reset";
			btnReset.UseVisualStyleBackColor = true;
			btnReset.Click += btnReset_Click;
			// 
			// logBox
			// 
			logBox.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
			logBox.Font = new System.Drawing.Font("Consolas", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
			logBox.Location = new System.Drawing.Point(4, 43);
			logBox.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
			logBox.Multiline = true;
			logBox.Name = "logBox";
			logBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
			logBox.Size = new System.Drawing.Size(1100, 156);
			logBox.TabIndex = 0;
			// 
			// lblLastObject
			// 
			lblLastObject.AutoSize = true;
			lblLastObject.Location = new System.Drawing.Point(587, 15);
			lblLastObject.Name = "lblLastObject";
			lblLastObject.Size = new System.Drawing.Size(0, 15);
			lblLastObject.TabIndex = 5;
			// 
			// Form1
			// 
			AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
			AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			ClientSize = new System.Drawing.Size(1107, 519);
			Controls.Add(splitContainer1);
			Icon = (System.Drawing.Icon)resources.GetObject("$this.Icon");
			Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
			Name = "Form1";
			Text = "Unifeed Demo";
			WindowState = System.Windows.Forms.FormWindowState.Maximized;
			Load += Form1_Load;
			splitContainer1.Panel1.ResumeLayout(false);
			splitContainer1.Panel2.ResumeLayout(false);
			splitContainer1.Panel2.PerformLayout();
			((System.ComponentModel.ISupportInitialize)splitContainer1).EndInit();
			splitContainer1.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)browser).EndInit();
			ResumeLayout(false);
		}

		#endregion

		private System.Windows.Forms.SplitContainer splitContainer1;
		private WebView2 browser; // works also in Administrator mode
                private System.Windows.Forms.TextBox logBox;
		private System.Windows.Forms.Button btnStartWithLastObject;
		private System.Windows.Forms.Button btnClearLog;
		private System.Windows.Forms.Button tbnStartClear;
		private System.Windows.Forms.Button btnDownload;
		private System.Windows.Forms.Button btnReset;
		private Microsoft.Toolkit.Forms.UI.Controls.WebViewCompatible webView1;
		private System.Windows.Forms.Label lblLastObject;
	}
}

