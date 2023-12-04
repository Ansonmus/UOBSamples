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
            this.btnDownload = new System.Windows.Forms.Button();
            this.tbnStartClear = new System.Windows.Forms.Button();
            this.btnStartWithLastObject = new System.Windows.Forms.Button();
            this.btnClearLog = new System.Windows.Forms.Button();
            this.btnReset = new System.Windows.Forms.Button();
            this.logBox = new System.Windows.Forms.TextBox();
            this.webView1 = new Microsoft.Toolkit.Forms.UI.Controls.WebViewCompatible();
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
            this.splitContainer1.Margin = new System.Windows.Forms.Padding(5, 6, 5, 6);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
			// 
			// splitContainer1.Panel1
			// 
			this.splitContainer1.Panel1.Controls.Add(this.webView1); 
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
            this.splitContainer1.Size = new System.Drawing.Size(1333, 865);
            this.splitContainer1.SplitterDistance = 511;
            this.splitContainer1.SplitterWidth = 8;
            this.splitContainer1.TabIndex = 0;
            // 
            // browser
            // 
            this.browser.CreationProperties = null;
            this.browser.DefaultBackgroundColor = System.Drawing.Color.White;
            this.browser.Dock = System.Windows.Forms.DockStyle.Fill;
            this.browser.Location = new System.Drawing.Point(0, 0);
            this.browser.Margin = new System.Windows.Forms.Padding(5, 6, 5, 6);
            this.browser.Name = "browser";
            this.browser.Size = new System.Drawing.Size(1333, 511);
            this.browser.TabIndex = 0;
            this.browser.ZoomFactor = 1D;
            // 
            // btnDownload
            // 
            this.btnDownload.Location = new System.Drawing.Point(497, 15);
            this.btnDownload.Margin = new System.Windows.Forms.Padding(5, 6, 5, 6);
            this.btnDownload.Name = "btnDownload";
            this.btnDownload.Size = new System.Drawing.Size(125, 44);
            this.btnDownload.TabIndex = 4;
            this.btnDownload.Text = "Download";
            this.btnDownload.UseVisualStyleBackColor = true;
            this.btnDownload.Click += new System.EventHandler(this.btnDownload_Click);
            // 
            // tbnStartClear
            // 
            this.tbnStartClear.Location = new System.Drawing.Point(157, 15);
            this.tbnStartClear.Margin = new System.Windows.Forms.Padding(5, 6, 5, 6);
            this.tbnStartClear.Name = "tbnStartClear";
            this.tbnStartClear.Size = new System.Drawing.Size(125, 44);
            this.tbnStartClear.TabIndex = 3;
            this.tbnStartClear.Text = "Restart";
            this.tbnStartClear.UseVisualStyleBackColor = true;
            this.tbnStartClear.Click += new System.EventHandler(this.tbnStartClear_Click);
            // 
            // btnStartWithLastObject
            // 
            this.btnStartWithLastObject.Enabled = false;
            this.btnStartWithLastObject.Location = new System.Drawing.Point(292, 15);
            this.btnStartWithLastObject.Margin = new System.Windows.Forms.Padding(5, 6, 5, 6);
            this.btnStartWithLastObject.Name = "btnStartWithLastObject";
            this.btnStartWithLastObject.Size = new System.Drawing.Size(193, 44);
            this.btnStartWithLastObject.TabIndex = 2;
            this.btnStartWithLastObject.Text = "Start with last object";
            this.btnStartWithLastObject.UseVisualStyleBackColor = true;
            this.btnStartWithLastObject.Click += new System.EventHandler(this.btnStartWithLastObject_Click);
            // 
            // btnClearLog
            // 
            this.btnClearLog.Location = new System.Drawing.Point(22, 15);
            this.btnClearLog.Margin = new System.Windows.Forms.Padding(5, 6, 5, 6);
            this.btnClearLog.Name = "btnClearLog";
            this.btnClearLog.Size = new System.Drawing.Size(125, 44);
            this.btnClearLog.TabIndex = 1;
            this.btnClearLog.Text = "Clear log";
            this.btnClearLog.UseVisualStyleBackColor = true;
            this.btnClearLog.Click += new System.EventHandler(this.btnClearLog_Click);
            // 
            // btnReset
            // 
            this.btnReset.Location = new System.Drawing.Point(642, 15);
            this.btnReset.Margin = new System.Windows.Forms.Padding(5, 6, 5, 6);
            this.btnReset.Name = "btnReset";
            this.btnReset.Size = new System.Drawing.Size(193, 44);
            this.btnReset.TabIndex = 4;
            this.btnReset.Text = "Reset";
            this.btnReset.UseVisualStyleBackColor = true;
            this.btnReset.Click += new System.EventHandler(this.btnReset_Click);
            // 
            // logBox
            // 
            this.logBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.logBox.Location = new System.Drawing.Point(5, 71);
            this.logBox.Margin = new System.Windows.Forms.Padding(5, 6, 5, 6);
			this.logBox.Font = new System.Drawing.Font("Consolas", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
			this.logBox.Multiline = true;
            this.logBox.Name = "logBox";
            this.logBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.logBox.Size = new System.Drawing.Size(1321, 262);
            this.logBox.TabIndex = 0;
            // 
            // webView1
            // 
            this.webView1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.webView1.Enabled = false;
            this.webView1.Location = new System.Drawing.Point(0, 0);
            this.webView1.Name = "webView1";
            this.webView1.TabIndex = 0;
			this.webView1.Visible = false;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(10F, 25F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1333, 865);
            this.Controls.Add(this.splitContainer1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(5, 6, 5, 6);
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
		private Microsoft.Toolkit.Forms.UI.Controls.WebViewCompatible webView1;
	}
}

