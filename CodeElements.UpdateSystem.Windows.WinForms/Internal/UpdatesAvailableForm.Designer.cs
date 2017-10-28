namespace CodeElements.UpdateSystem.Windows.WinForms.Internal
{
    partial class UpdatesAvailableForm
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
            this.newUpdatesDescriptionLabel = new System.Windows.Forms.Label();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.panel1 = new System.Windows.Forms.Panel();
            this.panel2 = new System.Windows.Forms.Panel();
            this.downloadProgressBar = new System.Windows.Forms.ProgressBar();
            this.downloadStatsLabel = new System.Windows.Forms.Label();
            this.downloadSpeedLabel = new System.Windows.Forms.Label();
            this.newUpdatesAvailableLabel = new CodeElements.UpdateSystem.Windows.WinForms.Internal.Controls.CaptionLabel();
            this.bottomPanel1 = new CodeElements.UpdateSystem.Windows.WinForms.Internal.Controls.BottomPanel();
            this.cancelButton = new System.Windows.Forms.Button();
            this.installButton = new System.Windows.Forms.Button();
            this.updateActionLabel = new System.Windows.Forms.Label();
            this.tableLayoutPanel1.SuspendLayout();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.bottomPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // newUpdatesDescriptionLabel
            // 
            this.newUpdatesDescriptionLabel.AutoSize = true;
            this.newUpdatesDescriptionLabel.Location = new System.Drawing.Point(80, 29);
            this.newUpdatesDescriptionLabel.Name = "newUpdatesDescriptionLabel";
            this.newUpdatesDescriptionLabel.Size = new System.Drawing.Size(334, 13);
            this.newUpdatesDescriptionLabel.TabIndex = 1;
            this.newUpdatesDescriptionLabel.Text = "New updates can be downloaded. Update from version 0.1.1 to 1.2.3";
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this.panel1, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.bottomPanel1, 0, 3);
            this.tableLayoutPanel1.Controls.Add(this.panel2, 0, 2);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 4;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 50F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 50F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 45F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(575, 403);
            this.tableLayoutPanel1.TabIndex = 3;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.newUpdatesAvailableLabel);
            this.panel1.Controls.Add(this.newUpdatesDescriptionLabel);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(3, 3);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(569, 44);
            this.panel1.TabIndex = 4;
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.updateActionLabel);
            this.panel2.Controls.Add(this.downloadSpeedLabel);
            this.panel2.Controls.Add(this.downloadStatsLabel);
            this.panel2.Controls.Add(this.downloadProgressBar);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel2.Location = new System.Drawing.Point(3, 311);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(569, 44);
            this.panel2.TabIndex = 5;
            // 
            // downloadProgressBar
            // 
            this.downloadProgressBar.Location = new System.Drawing.Point(0, 20);
            this.downloadProgressBar.Name = "downloadProgressBar";
            this.downloadProgressBar.Size = new System.Drawing.Size(569, 23);
            this.downloadProgressBar.TabIndex = 0;
            this.downloadProgressBar.Value = 50;
            // 
            // downloadStatsLabel
            // 
            this.downloadStatsLabel.AutoSize = true;
            this.downloadStatsLabel.Location = new System.Drawing.Point(377, 4);
            this.downloadStatsLabel.Name = "downloadStatsLabel";
            this.downloadStatsLabel.Size = new System.Drawing.Size(114, 13);
            this.downloadStatsLabel.TabIndex = 1;
            this.downloadStatsLabel.Text = "10.56 MiB / 17.19 MiB";
            // 
            // downloadSpeedLabel
            // 
            this.downloadSpeedLabel.AutoSize = true;
            this.downloadSpeedLabel.Location = new System.Drawing.Point(500, 4);
            this.downloadSpeedLabel.Name = "downloadSpeedLabel";
            this.downloadSpeedLabel.Size = new System.Drawing.Size(67, 13);
            this.downloadSpeedLabel.TabIndex = 2;
            this.downloadSpeedLabel.Text = "| 1024 MiB/s";
            // 
            // newUpdatesAvailableLabel
            // 
            this.newUpdatesAvailableLabel.AutoSize = true;
            this.newUpdatesAvailableLabel.Font = new System.Drawing.Font("Segoe UI", 12F);
            this.newUpdatesAvailableLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(51)))), ((int)(((byte)(153)))));
            this.newUpdatesAvailableLabel.Location = new System.Drawing.Point(80, 8);
            this.newUpdatesAvailableLabel.Name = "newUpdatesAvailableLabel";
            this.newUpdatesAvailableLabel.Size = new System.Drawing.Size(179, 21);
            this.newUpdatesAvailableLabel.TabIndex = 0;
            this.newUpdatesAvailableLabel.Text = "3 new updates available.";
            // 
            // bottomPanel1
            // 
            this.bottomPanel1.BackColor = System.Drawing.SystemColors.Control;
            this.bottomPanel1.Controls.Add(this.cancelButton);
            this.bottomPanel1.Controls.Add(this.installButton);
            this.bottomPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.bottomPanel1.Location = new System.Drawing.Point(3, 361);
            this.bottomPanel1.Name = "bottomPanel1";
            this.bottomPanel1.Size = new System.Drawing.Size(569, 39);
            this.bottomPanel1.TabIndex = 2;
            // 
            // cancelButton
            // 
            this.cancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.cancelButton.Location = new System.Drawing.Point(336, 9);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(90, 23);
            this.cancelButton.TabIndex = 1;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            this.cancelButton.Click += new System.EventHandler(this.cancelButton_Click);
            // 
            // installButton
            // 
            this.installButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.installButton.Location = new System.Drawing.Point(432, 9);
            this.installButton.Name = "installButton";
            this.installButton.Size = new System.Drawing.Size(128, 23);
            this.installButton.TabIndex = 0;
            this.installButton.Text = "Install updates";
            this.installButton.UseVisualStyleBackColor = true;
            this.installButton.Click += new System.EventHandler(this.installButton_Click);
            // 
            // updateActionLabel
            // 
            this.updateActionLabel.AutoSize = true;
            this.updateActionLabel.Location = new System.Drawing.Point(3, 4);
            this.updateActionLabel.Name = "updateActionLabel";
            this.updateActionLabel.Size = new System.Drawing.Size(131, 13);
            this.updateActionLabel.TabIndex = 3;
            this.updateActionLabel.Text = "Downloading file \'filename\'";
            // 
            // UpdatesAvailableForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(575, 403);
            this.Controls.Add(this.tableLayoutPanel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "UpdatesAvailableForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "UpdatesAvailableForm";
            this.tableLayoutPanel1.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.bottomPanel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private Controls.CaptionLabel newUpdatesAvailableLabel;
        private System.Windows.Forms.Label newUpdatesDescriptionLabel;
        private Controls.BottomPanel bottomPanel1;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.Button installButton;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Label downloadStatsLabel;
        private System.Windows.Forms.ProgressBar downloadProgressBar;
        private System.Windows.Forms.Label downloadSpeedLabel;
        private System.Windows.Forms.Label updateActionLabel;
    }
}