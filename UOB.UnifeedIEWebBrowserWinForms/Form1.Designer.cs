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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
			this.splitContainer1 = new System.Windows.Forms.SplitContainer();
			this.browser = new WebView2();
			this.logBox = new System.Windows.Forms.TextBox();
			this.btnClearLog = new System.Windows.Forms.Button();
			this.btnStartWithLastObject = new System.Windows.Forms.Button();
			this.tbnStartClear = new System.Windows.Forms.Button();
			this.btnDownload = new System.Windows.Forms.Button();
			this.btnReset = new System.Windows.Forms.Button();
			((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
			this.splitContainer1.Panel1.SuspendLayout();
			this.splitContainer1.Panel2.SuspendLayout();
			this.splitContainer1.SuspendLayout();
			this.SuspendLayout();
			// 
			// splitContainer1
			// 
			this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.splitContainer1.Location = new System.Drawing.Point(0, 0);
			this.splitContainer1.Name = "splitContainer1";
			this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
			// 
			// splitContainer1.Panel1
			// 
			this.splitContainer1.Panel1.Controls.Add(this.browser);
			// 
			// splitContainer1.Panel2
			// 
			this.splitContainer1.Panel2.Controls.Add(this.btnDownload);
			this.splitContainer1.Panel2.Controls.Add(this.tbnStartClear);
			this.splitContainer1.Panel2.Controls.Add(this.btnStartWithLastObject);
			this.splitContainer1.Panel2.Controls.Add(this.btnClearLog);
			this.splitContainer1.Panel2.Controls.Add(this.btnReset);
			this.splitContainer1.Panel2.Controls.Add(this.logBox);
			this.splitContainer1.Size = new System.Drawing.Size(800, 450);
			this.splitContainer1.SplitterDistance = 266;
			this.splitContainer1.TabIndex = 0;
			// 
			// webView
			// 
			this.browser.Dock = System.Windows.Forms.DockStyle.Fill;
			this.browser.Location = new System.Drawing.Point(0, 0);
			this.browser.Name = "webView";
			this.browser.TabIndex = 0;
			// 
			// logBox
			// 
			this.logBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.logBox.Location = new System.Drawing.Point(3, 37);
			this.logBox.Multiline = true;
			this.logBox.Name = "logBox";
			this.logBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
			this.logBox.Size = new System.Drawing.Size(794, 140);
			this.logBox.TabIndex = 0;
			// 
			// btnClearLog
			// 
			this.btnClearLog.Location = new System.Drawing.Point(13, 8);
			this.btnClearLog.Name = "btnClearLog";
			this.btnClearLog.Size = new System.Drawing.Size(75, 23);
			this.btnClearLog.TabIndex = 1;
			this.btnClearLog.Text = "Clear log";
			this.btnClearLog.UseVisualStyleBackColor = true;
			this.btnClearLog.Click += new System.EventHandler(this.btnClearLog_Click);
			// 
			// tbnStartClear
			// 
			this.tbnStartClear.Location = new System.Drawing.Point(94, 8);
			this.tbnStartClear.Name = "tbnStartClear";
			this.tbnStartClear.Size = new System.Drawing.Size(75, 23);
			this.tbnStartClear.TabIndex = 3;
			this.tbnStartClear.Text = "Restart";
			this.tbnStartClear.UseVisualStyleBackColor = true;
			this.tbnStartClear.Click += new System.EventHandler(this.tbnStartClear_Click);
			// 
			// btnStartWithLastObject
			// 
			this.btnStartWithLastObject.Enabled = false;
			this.btnStartWithLastObject.Location = new System.Drawing.Point(175, 8);
			this.btnStartWithLastObject.Name = "btnStartWithLastObject";
			this.btnStartWithLastObject.Size = new System.Drawing.Size(116, 23);
			this.btnStartWithLastObject.TabIndex = 2;
			this.btnStartWithLastObject.Text = "Start with last object";
			this.btnStartWithLastObject.UseVisualStyleBackColor = true;
			this.btnStartWithLastObject.Click += new System.EventHandler(this.btnStartWithLastObject_Click);
			// 
			// btnDownload
			// 
			this.btnDownload.Location = new System.Drawing.Point(298, 8);
			this.btnDownload.Name = "btnDownload";
			this.btnDownload.Size = new System.Drawing.Size(75, 23);
			this.btnDownload.TabIndex = 4;
			this.btnDownload.Text = "Download";
			this.btnDownload.UseVisualStyleBackColor = true;
			this.btnDownload.Click += new System.EventHandler(this.btnDownload_Click);
			// 
			// btnReset
			// 
			this.btnReset.Location = new System.Drawing.Point(385, 8);
			this.btnReset.Name = "btnReset";
			this.btnReset.Size = new System.Drawing.Size(116, 23);
			this.btnReset.TabIndex = 4;
			this.btnReset.Text = "Reset";
			this.btnReset.UseVisualStyleBackColor = true;
			this.btnReset.Click += new System.EventHandler(this.btnReset_Click);
			// 
			// Form1
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(800, 450);
			this.Controls.Add(this.splitContainer1);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "Form1";
			this.Text = "Unifeed Demo";
			this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
			this.Load += new System.EventHandler(this.Form1_Load);
			this.splitContainer1.Panel1.ResumeLayout(false);
			this.splitContainer1.Panel2.ResumeLayout(false);
			this.splitContainer1.Panel2.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
			this.splitContainer1.ResumeLayout(false);
			this.ResumeLayout(false);

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
	}
}

