namespace Pkg_Checker
{
    partial class MainWin
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainWin));
            this.btnCheck = new System.Windows.Forms.Button();
            this.lblProcessStatus = new System.Windows.Forms.Label();
            this.folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
            this.lblLocation = new System.Windows.Forms.Label();
            this.tbLocation = new System.Windows.Forms.TextBox();
            this.checkSub = new System.Windows.Forms.CheckBox();
            this.tbOutput = new System.Windows.Forms.TextBox();
            this.btnBrowse = new System.Windows.Forms.Button();
            this.btnClr = new System.Windows.Forms.Button();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.lblOutput = new System.Windows.Forms.Label();
            this.lblDrag = new System.Windows.Forms.Label();
            this.lnkResult = new System.Windows.Forms.LinkLabel();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.knownIssuesStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.totalProgress = new System.Windows.Forms.ProgressBar();
            this.chkAppendOutput = new System.Windows.Forms.CheckBox();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnCheck
            // 
            this.btnCheck.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.btnCheck.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnCheck.Location = new System.Drawing.Point(266, 78);
            this.btnCheck.Name = "btnCheck";
            this.btnCheck.Size = new System.Drawing.Size(177, 28);
            this.btnCheck.TabIndex = 0;
            this.btnCheck.Text = "Check";
            this.btnCheck.UseVisualStyleBackColor = true;
            this.btnCheck.Click += new System.EventHandler(this.btnCheck_Click);
            // 
            // lblProcessStatus
            // 
            this.lblProcessStatus.AutoSize = true;
            this.lblProcessStatus.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lblProcessStatus.Location = new System.Drawing.Point(58, 173);
            this.lblProcessStatus.Name = "lblProcessStatus";
            this.lblProcessStatus.Size = new System.Drawing.Size(42, 15);
            this.lblProcessStatus.TabIndex = 1;
            this.lblProcessStatus.Text = "Ready";
            // 
            // lblLocation
            // 
            this.lblLocation.AutoSize = true;
            this.lblLocation.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lblLocation.Location = new System.Drawing.Point(2, 46);
            this.lblLocation.Name = "lblLocation";
            this.lblLocation.Size = new System.Drawing.Size(167, 20);
            this.lblLocation.TabIndex = 2;
            this.lblLocation.Text = "Review Package Path:";
            // 
            // tbLocation
            // 
            this.tbLocation.AllowDrop = true;
            this.tbLocation.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbLocation.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.tbLocation.Location = new System.Drawing.Point(176, 46);
            this.tbLocation.Name = "tbLocation";
            this.tbLocation.Size = new System.Drawing.Size(354, 26);
            this.tbLocation.TabIndex = 1;
            // 
            // checkSub
            // 
            this.checkSub.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.checkSub.AutoSize = true;
            this.checkSub.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.checkSub.Location = new System.Drawing.Point(536, 47);
            this.checkSub.Name = "checkSub";
            this.checkSub.Size = new System.Drawing.Size(163, 24);
            this.checkSub.TabIndex = 2;
            this.checkSub.Text = "Include sub-folders";
            this.checkSub.UseVisualStyleBackColor = true;
            // 
            // tbOutput
            // 
            this.tbOutput.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbOutput.Location = new System.Drawing.Point(5, 224);
            this.tbOutput.MaxLength = 655360;
            this.tbOutput.Multiline = true;
            this.tbOutput.Name = "tbOutput";
            this.tbOutput.ReadOnly = true;
            this.tbOutput.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.tbOutput.Size = new System.Drawing.Size(776, 332);
            this.tbOutput.TabIndex = 5;
            // 
            // btnBrowse
            // 
            this.btnBrowse.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnBrowse.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnBrowse.Location = new System.Drawing.Point(702, 44);
            this.btnBrowse.Name = "btnBrowse";
            this.btnBrowse.Size = new System.Drawing.Size(75, 27);
            this.btnBrowse.TabIndex = 3;
            this.btnBrowse.Text = "Browse";
            this.btnBrowse.UseVisualStyleBackColor = true;
            this.btnBrowse.Click += new System.EventHandler(this.btnBrowse_Click);
            // 
            // btnClr
            // 
            this.btnClr.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnClr.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnClr.Location = new System.Drawing.Point(672, 197);
            this.btnClr.Name = "btnClr";
            this.btnClr.Size = new System.Drawing.Size(109, 28);
            this.btnClr.TabIndex = 5;
            this.btnClr.Text = "Clear Result";
            this.btnClr.UseVisualStyleBackColor = true;
            this.btnClr.Click += new System.EventHandler(this.btnClr_Click);
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            // 
            // lblOutput
            // 
            this.lblOutput.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.lblOutput.AutoSize = true;
            this.lblOutput.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lblOutput.Location = new System.Drawing.Point(2, 201);
            this.lblOutput.Name = "lblOutput";
            this.lblOutput.Size = new System.Drawing.Size(62, 20);
            this.lblOutput.TabIndex = 9;
            this.lblOutput.Text = "Output:";
            // 
            // lblDrag
            // 
            this.lblDrag.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.lblDrag.AutoSize = true;
            this.lblDrag.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblDrag.ForeColor = System.Drawing.SystemColors.Highlight;
            this.lblDrag.Location = new System.Drawing.Point(114, 117);
            this.lblDrag.Name = "lblDrag";
            this.lblDrag.Size = new System.Drawing.Size(554, 20);
            this.lblDrag.TabIndex = 10;
            this.lblDrag.Text = "You can also simply drop one or more files/folders to this window for checking.";
            this.lblDrag.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lnkResult
            // 
            this.lnkResult.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lnkResult.AutoSize = true;
            this.lnkResult.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lnkResult.Location = new System.Drawing.Point(539, 201);
            this.lnkResult.Name = "lnkResult";
            this.lnkResult.Size = new System.Drawing.Size(127, 20);
            this.lnkResult.TabIndex = 11;
            this.lnkResult.TabStop = true;
            this.lnkResult.Text = "Open Result File";
            this.lnkResult.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lnkResult_LinkClicked);
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.helpToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(784, 24);
            this.menuStrip1.TabIndex = 12;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // helpToolStripMenuItem
            // 
            this.helpToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.knownIssuesStripMenuItem,
            this.aboutToolStripMenuItem});
            this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            this.helpToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
            this.helpToolStripMenuItem.Text = "&Help";
            // 
            // knownIssuesStripMenuItem
            // 
            this.knownIssuesStripMenuItem.Name = "knownIssuesStripMenuItem";
            this.knownIssuesStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.knownIssuesStripMenuItem.Text = "Known Issues";
            this.knownIssuesStripMenuItem.Click += new System.EventHandler(this.knownIssuesStripMenuItem_Click);
            // 
            // aboutToolStripMenuItem
            // 
            this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            this.aboutToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.aboutToolStripMenuItem.Text = "About";
            this.aboutToolStripMenuItem.Click += new System.EventHandler(this.aboutToolStripMenuItem_Click);
            // 
            // totalProgress
            // 
            this.totalProgress.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.totalProgress.Location = new System.Drawing.Point(61, 147);
            this.totalProgress.Name = "totalProgress";
            this.totalProgress.Size = new System.Drawing.Size(661, 23);
            this.totalProgress.TabIndex = 13;
            // 
            // chkAppendOutput
            // 
            this.chkAppendOutput.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.chkAppendOutput.AutoSize = true;
            this.chkAppendOutput.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.chkAppendOutput.Location = new System.Drawing.Point(61, 200);
            this.chkAppendOutput.Name = "chkAppendOutput";
            this.chkAppendOutput.Size = new System.Drawing.Size(137, 24);
            this.chkAppendOutput.TabIndex = 14;
            this.chkAppendOutput.Text = "Append Output";
            this.chkAppendOutput.UseVisualStyleBackColor = true;
            // 
            // MainWin
            // 
            this.AcceptButton = this.btnCheck;
            this.AllowDrop = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(784, 561);
            this.Controls.Add(this.chkAppendOutput);
            this.Controls.Add(this.totalProgress);
            this.Controls.Add(this.lnkResult);
            this.Controls.Add(this.lblDrag);
            this.Controls.Add(this.lblOutput);
            this.Controls.Add(this.btnClr);
            this.Controls.Add(this.btnBrowse);
            this.Controls.Add(this.tbOutput);
            this.Controls.Add(this.checkSub);
            this.Controls.Add(this.tbLocation);
            this.Controls.Add(this.lblLocation);
            this.Controls.Add(this.lblProcessStatus);
            this.Controls.Add(this.btnCheck);
            this.Controls.Add(this.menuStrip1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "MainWin";
            this.Text = "Review Package Checker";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.DragDrop += new System.Windows.Forms.DragEventHandler(this.MainWin_DragDrop);
            this.DragEnter += new System.Windows.Forms.DragEventHandler(this.MainWin_DragEnter);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnCheck;
        private System.Windows.Forms.Label lblProcessStatus;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog1;
        private System.Windows.Forms.Label lblLocation;
        private System.Windows.Forms.TextBox tbLocation;
        private System.Windows.Forms.CheckBox checkSub;
        private System.Windows.Forms.TextBox tbOutput;
        private System.Windows.Forms.Button btnBrowse;
        private System.Windows.Forms.Button btnClr;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.Label lblOutput;
        private System.Windows.Forms.Label lblDrag;
        private System.Windows.Forms.LinkLabel lnkResult;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem knownIssuesStripMenuItem;
        private System.Windows.Forms.ProgressBar totalProgress;
        private System.Windows.Forms.CheckBox chkAppendOutput;
    }
}

