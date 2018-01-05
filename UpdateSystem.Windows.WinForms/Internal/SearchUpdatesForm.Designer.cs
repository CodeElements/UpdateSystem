namespace CodeElements.UpdateSystem.Windows.WinForms.Internal
{
    partial class SearchUpdatesForm
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
            this.checkingLabel = new CodeElements.UpdateSystem.Windows.WinForms.Internal.Controls.CaptionLabel();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.SuspendLayout();
            // 
            // checkingLabel
            // 
            this.checkingLabel.AutoSize = true;
            this.checkingLabel.Font = new System.Drawing.Font("Segoe UI", 12F);
            this.checkingLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(51)))), ((int)(((byte)(153)))));
            this.checkingLabel.Location = new System.Drawing.Point(12, 5);
            this.checkingLabel.Name = "checkingLabel";
            this.checkingLabel.Size = new System.Drawing.Size(166, 21);
            this.checkingLabel.TabIndex = 0;
            this.checkingLabel.Text = "Checking for updates...";
            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(12, 31);
            this.progressBar1.MarqueeAnimationSpeed = 75;
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(402, 17);
            this.progressBar1.Style = System.Windows.Forms.ProgressBarStyle.Marquee;
            this.progressBar1.TabIndex = 1;
            // 
            // SearchUpdatesForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(426, 58);
            this.Controls.Add(this.progressBar1);
            this.Controls.Add(this.checkingLabel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SearchUpdatesForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "SearchUpdatesForm";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Controls.CaptionLabel checkingLabel;
        private System.Windows.Forms.ProgressBar progressBar1;
    }
}