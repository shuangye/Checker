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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainWin));
            this.btnCheck = new System.Windows.Forms.Button();
            this.folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
            this.lblLocation = new System.Windows.Forms.Label();
            this.tbLocation = new System.Windows.Forms.TextBox();
            this.checkSub = new System.Windows.Forms.CheckBox();
            this.tbOutput = new System.Windows.Forms.TextBox();
            this.btnBrowse = new System.Windows.Forms.Button();
            this.btnClr = new System.Windows.Forms.Button();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.lblDrag = new System.Windows.Forms.Label();
            this.lnkResult = new System.Windows.Forms.LinkLabel();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.cM21ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menuEnableCm21 = new System.Windows.Forms.ToolStripMenuItem();
            this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.knownIssuesStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.chkAppendOutput = new System.Windows.Forms.CheckBox();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.statusTotalProgress = new System.Windows.Forms.ToolStripProgressBar();
            this.statusCurrentObj = new System.Windows.Forms.ToolStripStatusLabel();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.spinTimeout = new System.Windows.Forms.NumericUpDown();
            this.chkSave = new System.Windows.Forms.CheckBox();
            this.groupCM21 = new System.Windows.Forms.GroupBox();
            this.label1 = new System.Windows.Forms.Label();
            this.btnSCRDownloadPath = new System.Windows.Forms.Button();
            this.lblSCRDownloadPath = new System.Windows.Forms.Label();
            this.txtEID = new System.Windows.Forms.TextBox();
            this.lblPWD = new System.Windows.Forms.Label();
            this.txtPWD = new System.Windows.Forms.TextBox();
            this.txtSCRDownloadPath = new System.Windows.Forms.TextBox();
            this.lblEID = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.menuStrip1.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.spinTimeout)).BeginInit();
            this.groupCM21.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnCheck
            // 
            this.btnCheck.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.btnCheck.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnCheck.Location = new System.Drawing.Point(308, 149);
            this.btnCheck.Name = "btnCheck";
            this.btnCheck.Size = new System.Drawing.Size(177, 28);
            this.btnCheck.TabIndex = 0;
            this.btnCheck.Text = "Check";
            this.btnCheck.UseVisualStyleBackColor = true;
            this.btnCheck.Click += new System.EventHandler(this.btnCheck_Click);
            // 
            // lblLocation
            // 
            this.lblLocation.AutoSize = true;
            this.lblLocation.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lblLocation.Location = new System.Drawing.Point(1, 37);
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
            this.tbLocation.Location = new System.Drawing.Point(176, 36);
            this.tbLocation.Name = "tbLocation";
            this.tbLocation.Size = new System.Drawing.Size(354, 26);
            this.tbLocation.TabIndex = 1;
            // 
            // checkSub
            // 
            this.checkSub.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.checkSub.AutoSize = true;
            this.checkSub.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.checkSub.Location = new System.Drawing.Point(537, 36);
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
            this.tbOutput.Location = new System.Drawing.Point(5, 226);
            this.tbOutput.Margin = new System.Windows.Forms.Padding(3, 0, 3, 3);
            this.tbOutput.MaxLength = 655360;
            this.tbOutput.Multiline = true;
            this.tbOutput.Name = "tbOutput";
            this.tbOutput.ReadOnly = true;
            this.tbOutput.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.tbOutput.Size = new System.Drawing.Size(773, 330);
            this.tbOutput.TabIndex = 100;
            // 
            // btnBrowse
            // 
            this.btnBrowse.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnBrowse.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnBrowse.Location = new System.Drawing.Point(703, 36);
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
            this.btnClr.Location = new System.Drawing.Point(703, 195);
            this.btnClr.Name = "btnClr";
            this.btnClr.Size = new System.Drawing.Size(75, 27);
            this.btnClr.TabIndex = 98;
            this.btnClr.Text = "Clear";
            this.btnClr.UseVisualStyleBackColor = true;
            this.btnClr.Click += new System.EventHandler(this.btnClr_Click);
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            // 
            // lblDrag
            // 
            this.lblDrag.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.lblDrag.AutoSize = true;
            this.lblDrag.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblDrag.ForeColor = System.Drawing.SystemColors.Highlight;
            this.lblDrag.Location = new System.Drawing.Point(221, 180);
            this.lblDrag.Name = "lblDrag";
            this.lblDrag.Size = new System.Drawing.Size(375, 20);
            this.lblDrag.TabIndex = 10;
            this.lblDrag.Text = "Try droping one or more files/folders to this window...";
            this.lblDrag.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lnkResult
            // 
            this.lnkResult.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lnkResult.AutoSize = true;
            this.lnkResult.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lnkResult.Location = new System.Drawing.Point(573, 202);
            this.lnkResult.Margin = new System.Windows.Forms.Padding(0);
            this.lnkResult.Name = "lnkResult";
            this.lnkResult.Size = new System.Drawing.Size(127, 20);
            this.lnkResult.TabIndex = 99;
            this.lnkResult.Text = "Open Result File";
            this.lnkResult.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lnkResult_LinkClicked);
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.cM21ToolStripMenuItem,
            this.helpToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(784, 24);
            this.menuStrip1.TabIndex = 12;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // cM21ToolStripMenuItem
            // 
            this.cM21ToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuEnableCm21});
            this.cM21ToolStripMenuItem.Name = "cM21ToolStripMenuItem";
            this.cM21ToolStripMenuItem.Size = new System.Drawing.Size(50, 20);
            this.cM21ToolStripMenuItem.Text = "&CM21";
            // 
            // menuEnableCm21
            // 
            this.menuEnableCm21.CheckOnClick = true;
            this.menuEnableCm21.Name = "menuEnableCm21";
            this.menuEnableCm21.Size = new System.Drawing.Size(248, 22);
            this.menuEnableCm21.Text = "Compare SCR Report from CM21";
            this.menuEnableCm21.Click += new System.EventHandler(this.menuEnableCm21_Click);
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
            this.knownIssuesStripMenuItem.Size = new System.Drawing.Size(145, 22);
            this.knownIssuesStripMenuItem.Text = "Known Issues";
            this.knownIssuesStripMenuItem.Click += new System.EventHandler(this.knownIssuesStripMenuItem_Click);
            // 
            // aboutToolStripMenuItem
            // 
            this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            this.aboutToolStripMenuItem.Size = new System.Drawing.Size(145, 22);
            this.aboutToolStripMenuItem.Text = "About";
            this.aboutToolStripMenuItem.Click += new System.EventHandler(this.aboutToolStripMenuItem_Click);
            // 
            // chkAppendOutput
            // 
            this.chkAppendOutput.AutoSize = true;
            this.chkAppendOutput.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.chkAppendOutput.Location = new System.Drawing.Point(5, 202);
            this.chkAppendOutput.Name = "chkAppendOutput";
            this.chkAppendOutput.Size = new System.Drawing.Size(137, 24);
            this.chkAppendOutput.TabIndex = 96;
            this.chkAppendOutput.Text = "Append Output";
            this.chkAppendOutput.UseVisualStyleBackColor = true;
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.statusTotalProgress,
            this.statusCurrentObj});
            this.statusStrip1.Location = new System.Drawing.Point(0, 539);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(784, 22);
            this.statusStrip1.TabIndex = 101;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // statusTotalProgress
            // 
            this.statusTotalProgress.Name = "statusTotalProgress";
            this.statusTotalProgress.Size = new System.Drawing.Size(240, 16);
            // 
            // statusCurrentObj
            // 
            this.statusCurrentObj.Name = "statusCurrentObj";
            this.statusCurrentObj.Overflow = System.Windows.Forms.ToolStripItemOverflow.Always;
            this.statusCurrentObj.Size = new System.Drawing.Size(527, 17);
            this.statusCurrentObj.Spring = true;
            this.statusCurrentObj.Text = "Ready";
            // 
            // timer1
            // 
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // spinTimeout
            // 
            this.spinTimeout.Increment = new decimal(new int[] {
            2,
            0,
            0,
            0});
            this.spinTimeout.Location = new System.Drawing.Point(191, 52);
            this.spinTimeout.Maximum = new decimal(new int[] {
            600,
            0,
            0,
            0});
            this.spinTimeout.Minimum = new decimal(new int[] {
            2,
            0,
            0,
            0});
            this.spinTimeout.Name = "spinTimeout";
            this.spinTimeout.Size = new System.Drawing.Size(42, 21);
            this.spinTimeout.TabIndex = 9;
            this.spinTimeout.Value = new decimal(new int[] {
            60,
            0,
            0,
            0});
            // 
            // chkSave
            // 
            this.chkSave.AutoSize = true;
            this.chkSave.Location = new System.Drawing.Point(281, 20);
            this.chkSave.Name = "chkSave";
            this.chkSave.Size = new System.Drawing.Size(112, 19);
            this.chkSave.TabIndex = 6;
            this.chkSave.Text = "Save Credential";
            this.chkSave.UseVisualStyleBackColor = true;
            // 
            // groupCM21
            // 
            this.groupCM21.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupCM21.Controls.Add(this.label2);
            this.groupCM21.Controls.Add(this.label1);
            this.groupCM21.Controls.Add(this.btnSCRDownloadPath);
            this.groupCM21.Controls.Add(this.lblSCRDownloadPath);
            this.groupCM21.Controls.Add(this.spinTimeout);
            this.groupCM21.Controls.Add(this.chkSave);
            this.groupCM21.Controls.Add(this.txtEID);
            this.groupCM21.Controls.Add(this.lblPWD);
            this.groupCM21.Controls.Add(this.txtPWD);
            this.groupCM21.Controls.Add(this.txtSCRDownloadPath);
            this.groupCM21.Controls.Add(this.lblEID);
            this.groupCM21.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.groupCM21.Location = new System.Drawing.Point(5, 66);
            this.groupCM21.Name = "groupCM21";
            this.groupCM21.Size = new System.Drawing.Size(773, 80);
            this.groupCM21.TabIndex = 4;
            this.groupCM21.TabStop = false;
            this.groupCM21.Text = "Compare SCR Report from CM21";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 54);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(179, 15);
            this.label1.TabIndex = 105;
            this.label1.Text = "Operation Timeout (in seconds)";
            // 
            // btnSCRDownloadPath
            // 
            this.btnSCRDownloadPath.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSCRDownloadPath.Location = new System.Drawing.Point(698, 16);
            this.btnSCRDownloadPath.Name = "btnSCRDownloadPath";
            this.btnSCRDownloadPath.Size = new System.Drawing.Size(75, 24);
            this.btnSCRDownloadPath.TabIndex = 8;
            this.btnSCRDownloadPath.Text = "Browse";
            this.btnSCRDownloadPath.UseVisualStyleBackColor = true;
            this.btnSCRDownloadPath.Click += new System.EventHandler(this.btnBrowse_Click);
            // 
            // lblSCRDownloadPath
            // 
            this.lblSCRDownloadPath.AutoSize = true;
            this.lblSCRDownloadPath.Location = new System.Drawing.Point(402, 21);
            this.lblSCRDownloadPath.Name = "lblSCRDownloadPath";
            this.lblSCRDownloadPath.Size = new System.Drawing.Size(151, 15);
            this.lblSCRDownloadPath.TabIndex = 21;
            this.lblSCRDownloadPath.Text = "Download SCR Report To:";
            // 
            // txtEID
            // 
            this.txtEID.Location = new System.Drawing.Point(34, 18);
            this.txtEID.MaxLength = 12;
            this.txtEID.Name = "txtEID";
            this.txtEID.Size = new System.Drawing.Size(100, 21);
            this.txtEID.TabIndex = 4;
            // 
            // lblPWD
            // 
            this.lblPWD.AutoSize = true;
            this.lblPWD.Location = new System.Drawing.Point(137, 21);
            this.lblPWD.Name = "lblPWD";
            this.lblPWD.Size = new System.Drawing.Size(38, 15);
            this.lblPWD.TabIndex = 20;
            this.lblPWD.Text = "PWD:";
            // 
            // txtPWD
            // 
            this.txtPWD.Location = new System.Drawing.Point(175, 18);
            this.txtPWD.MaxLength = 20;
            this.txtPWD.Name = "txtPWD";
            this.txtPWD.PasswordChar = '*';
            this.txtPWD.Size = new System.Drawing.Size(100, 21);
            this.txtPWD.TabIndex = 5;
            // 
            // txtSCRDownloadPath
            // 
            this.txtSCRDownloadPath.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtSCRDownloadPath.Location = new System.Drawing.Point(553, 18);
            this.txtSCRDownloadPath.Name = "txtSCRDownloadPath";
            this.txtSCRDownloadPath.Size = new System.Drawing.Size(139, 21);
            this.txtSCRDownloadPath.TabIndex = 7;
            // 
            // lblEID
            // 
            this.lblEID.AutoSize = true;
            this.lblEID.Location = new System.Drawing.Point(6, 21);
            this.lblEID.Name = "lblEID";
            this.lblEID.Size = new System.Drawing.Size(30, 15);
            this.lblEID.TabIndex = 16;
            this.lblEID.Text = "EID:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.ForeColor = System.Drawing.SystemColors.GrayText;
            this.label2.Location = new System.Drawing.Point(237, 54);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(272, 15);
            this.label2.TabIndex = 106;
            this.label2.Text = "(CM21 process will be kill after this time expires.)";
            // 
            // MainWin
            // 
            this.AcceptButton = this.btnCheck;
            this.AllowDrop = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(784, 561);
            this.Controls.Add(this.btnBrowse);
            this.Controls.Add(this.checkSub);
            this.Controls.Add(this.tbLocation);
            this.Controls.Add(this.lblLocation);
            this.Controls.Add(this.btnCheck);
            this.Controls.Add(this.chkAppendOutput);
            this.Controls.Add(this.btnClr);
            this.Controls.Add(this.lblDrag);
            this.Controls.Add(this.lnkResult);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.menuStrip1);
            this.Controls.Add(this.tbOutput);
            this.Controls.Add(this.groupCM21);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "MainWin";
            this.Text = "Review Package Checker";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainWin_FormClosing);
            this.DragDrop += new System.Windows.Forms.DragEventHandler(this.MainWin_DragDrop);
            this.DragEnter += new System.Windows.Forms.DragEventHandler(this.MainWin_DragEnter);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.spinTimeout)).EndInit();
            this.groupCM21.ResumeLayout(false);
            this.groupCM21.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnCheck;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog1;
        private System.Windows.Forms.Label lblLocation;
        private System.Windows.Forms.TextBox tbLocation;
        private System.Windows.Forms.CheckBox checkSub;
        private System.Windows.Forms.TextBox tbOutput;
        private System.Windows.Forms.Button btnBrowse;
        private System.Windows.Forms.Button btnClr;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.Label lblDrag;
        private System.Windows.Forms.LinkLabel lnkResult;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem knownIssuesStripMenuItem;
        private System.Windows.Forms.CheckBox chkAppendOutput;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripProgressBar statusTotalProgress;
        private System.Windows.Forms.ToolStripStatusLabel statusCurrentObj;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.ToolStripMenuItem cM21ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem menuEnableCm21;
        private System.Windows.Forms.NumericUpDown spinTimeout;
        private System.Windows.Forms.CheckBox chkSave;
        private System.Windows.Forms.GroupBox groupCM21;
        private System.Windows.Forms.Button btnSCRDownloadPath;
        private System.Windows.Forms.Label lblSCRDownloadPath;
        private System.Windows.Forms.TextBox txtEID;
        private System.Windows.Forms.Label lblPWD;
        private System.Windows.Forms.TextBox txtPWD;
        private System.Windows.Forms.TextBox txtSCRDownloadPath;
        private System.Windows.Forms.Label lblEID;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
    }
}

