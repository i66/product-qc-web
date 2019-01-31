namespace web_utility
{
    partial class Frm_ProductQC
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
            this.webBrowser_qc = new System.Windows.Forms.WebBrowser();
            this.SuspendLayout();
            // 
            // webBrowser_qc
            // 
            this.webBrowser_qc.Dock = System.Windows.Forms.DockStyle.Fill;
            this.webBrowser_qc.Location = new System.Drawing.Point(0, 0);
            this.webBrowser_qc.MinimumSize = new System.Drawing.Size(20, 20);
            this.webBrowser_qc.Name = "webBrowser_qc";
            this.webBrowser_qc.Size = new System.Drawing.Size(800, 450);
            this.webBrowser_qc.TabIndex = 0;
            // 
            // Frm_ProductQC
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 25F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.webBrowser_qc);
            this.Name = "Frm_ProductQC";
            this.Text = "Form1";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.WebBrowser webBrowser_qc;
    }
}

