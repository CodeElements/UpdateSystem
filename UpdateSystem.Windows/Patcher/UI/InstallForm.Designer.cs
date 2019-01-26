using CodeElements.UpdateSystem.Windows.Patcher.UI.Controls;

namespace CodeElements.UpdateSystem.Windows.Patcher.UI
{
    partial class InstallForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(InstallForm));
            this.applicationImage = new System.Windows.Forms.PictureBox();
            this.headerLabel = new System.Windows.Forms.Label();
            this.progressBar = new System.Windows.Forms.ProgressBar();
            this.statusLabel = new System.Windows.Forms.Label();
            this.progressLabel = new System.Windows.Forms.Label();
            this.bottomPanel1 = new CodeElements.UpdateSystem.Windows.Patcher.UI.Controls.BottomPanel();
            this.cancelButton = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.applicationImage)).BeginInit();
            this.bottomPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // applicationImage
            // 
            this.applicationImage.Image = ((System.Drawing.Image)(resources.GetObject("applicationImage.Image")));
            this.applicationImage.InitialImage = null;
            this.applicationImage.Location = new System.Drawing.Point(12, 6);
            this.applicationImage.Name = "applicationImage";
            this.applicationImage.Size = new System.Drawing.Size(32, 32);
            this.applicationImage.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.applicationImage.TabIndex = 0;
            this.applicationImage.TabStop = false;
            // 
            // headerLabel
            // 
            this.headerLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.headerLabel.BackColor = System.Drawing.Color.White;
            this.headerLabel.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.headerLabel.ForeColor = System.Drawing.SystemColors.HotTrack;
            this.headerLabel.Location = new System.Drawing.Point(50, 6);
            this.headerLabel.Name = "headerLabel";
            this.headerLabel.Size = new System.Drawing.Size(376, 32);
            this.headerLabel.TabIndex = 1;
            this.headerLabel.Text = "CodeElements - UpdateSystem";
            this.headerLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // progressBar
            // 
            this.progressBar.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.progressBar.Location = new System.Drawing.Point(12, 70);
            this.progressBar.Name = "progressBar";
            this.progressBar.Size = new System.Drawing.Size(414, 18);
            this.progressBar.TabIndex = 2;
            // 
            // statusLabel
            // 
            this.statusLabel.AutoEllipsis = true;
            this.statusLabel.Location = new System.Drawing.Point(12, 54);
            this.statusLabel.Name = "statusLabel";
            this.statusLabel.Size = new System.Drawing.Size(372, 13);
            this.statusLabel.TabIndex = 3;
            // 
            // progressLabel
            // 
            this.progressLabel.Location = new System.Drawing.Point(390, 54);
            this.progressLabel.Name = "progressLabel";
            this.progressLabel.Size = new System.Drawing.Size(36, 13);
            this.progressLabel.TabIndex = 5;
            this.progressLabel.Text = "0 %";
            this.progressLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // bottomPanel1
            // 
            this.bottomPanel1.BackColor = System.Drawing.SystemColors.Control;
            this.bottomPanel1.Controls.Add(this.cancelButton);
            this.bottomPanel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.bottomPanel1.Location = new System.Drawing.Point(0, 101);
            this.bottomPanel1.Name = "bottomPanel1";
            this.bottomPanel1.Size = new System.Drawing.Size(438, 43);
            this.bottomPanel1.TabIndex = 4;
            // 
            // cancelButton
            // 
            this.cancelButton.Location = new System.Drawing.Point(332, 10);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(90, 23);
            this.cancelButton.TabIndex = 0;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            this.cancelButton.Click += new System.EventHandler(this.cancelButton_Click);
            // 
            // InstallForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(438, 144);
            this.Controls.Add(this.progressLabel);
            this.Controls.Add(this.bottomPanel1);
            this.Controls.Add(this.statusLabel);
            this.Controls.Add(this.progressBar);
            this.Controls.Add(this.headerLabel);
            this.Controls.Add(this.applicationImage);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "InstallForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Apply updates";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.InstallForm_FormClosing);
            ((System.ComponentModel.ISupportInitialize)(this.applicationImage)).EndInit();
            this.bottomPanel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox applicationImage;
        private System.Windows.Forms.Label headerLabel;
        private System.Windows.Forms.ProgressBar progressBar;
        private System.Windows.Forms.Label statusLabel;
        private Controls.BottomPanel bottomPanel1;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.Label progressLabel;
    }
}

