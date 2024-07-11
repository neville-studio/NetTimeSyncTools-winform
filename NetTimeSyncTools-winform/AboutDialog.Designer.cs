namespace NetTimeSyncTools_winform
{
    partial class AboutDialog
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AboutDialog));
            this.CopyRightLabel = new System.Windows.Forms.Label();
            this.VersionLabel = new System.Windows.Forms.Label();
            this.ViewProject = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // CopyRightLabel
            // 
            resources.ApplyResources(this.CopyRightLabel, "CopyRightLabel");
            this.CopyRightLabel.Name = "CopyRightLabel";
            // 
            // VersionLabel
            // 
            resources.ApplyResources(this.VersionLabel, "VersionLabel");
            this.VersionLabel.Name = "VersionLabel";
            // 
            // ViewProject
            // 
            resources.ApplyResources(this.ViewProject, "ViewProject");
            this.ViewProject.Name = "ViewProject";
            this.ViewProject.UseVisualStyleBackColor = true;
            this.ViewProject.Click += new System.EventHandler(this.ViewProject_Click);
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // AboutDialog
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.label1);
            this.Controls.Add(this.ViewProject);
            this.Controls.Add(this.VersionLabel);
            this.Controls.Add(this.CopyRightLabel);
            this.ForeColor = System.Drawing.SystemColors.ControlText;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "AboutDialog";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.Load += new System.EventHandler(this.AboutDialog_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label CopyRightLabel;
        private System.Windows.Forms.Label VersionLabel;
        private System.Windows.Forms.Button ViewProject;
        private System.Windows.Forms.Label label1;
    }
}