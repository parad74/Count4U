namespace Count4U.Modules.ContextCBI.Views.Report
{
    partial class ReportsWinForm
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
			this.reportViewer1 = new Microsoft.Reporting.WinForms.ReportViewer();
			this.SuspendLayout();
			// 
			// reportViewer1
			// 
			this.reportViewer1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.reportViewer1.Location = new System.Drawing.Point(0, 0);
			this.reportViewer1.Name = "reportViewer1";
			this.reportViewer1.Size = new System.Drawing.Size(984, 562);
			this.reportViewer1.TabIndex = 0;
			this.reportViewer1.VisibleChanged += new System.EventHandler(this.reportViewer1_VisibleChanged);
			// 
			// ReportsWinForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(984, 562);
			this.Controls.Add(this.reportViewer1);
			this.Name = "ReportsWinForm";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "ReportsWinForm";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ReportsWinForm_FormClosing);
			this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.ReportsWinForm_FormClosed);
			this.Load += new System.EventHandler(this.ReportsWinForm_Load);
			this.ResumeLayout(false);

        }

        #endregion

		private Microsoft.Reporting.WinForms.ReportViewer reportViewer1;
    }
}