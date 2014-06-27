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
            this.cbFix = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // btnCheck
            // 
            this.btnCheck.Location = new System.Drawing.Point(176, 72);
            this.btnCheck.Name = "btnCheck";
            this.btnCheck.Size = new System.Drawing.Size(75, 23);
            this.btnCheck.TabIndex = 0;
            this.btnCheck.Text = "Check";
            this.btnCheck.UseVisualStyleBackColor = true;
            this.btnCheck.Click += new System.EventHandler(this.btnCheck_Click);
            // 
            // lblProcessStatus
            // 
            this.lblProcessStatus.AutoSize = true;
            this.lblProcessStatus.Location = new System.Drawing.Point(2, 419);
            this.lblProcessStatus.Name = "lblProcessStatus";
            this.lblProcessStatus.Size = new System.Drawing.Size(38, 13);
            this.lblProcessStatus.TabIndex = 1;
            this.lblProcessStatus.Text = "Ready";
            // 
            // lblLocation
            // 
            this.lblLocation.AutoSize = true;
            this.lblLocation.Location = new System.Drawing.Point(2, 46);
            this.lblLocation.Name = "lblLocation";
            this.lblLocation.Size = new System.Drawing.Size(121, 13);
            this.lblLocation.TabIndex = 2;
            this.lblLocation.Text = "eReview package path:";
            // 
            // tbLocation
            // 
            this.tbLocation.Location = new System.Drawing.Point(122, 46);
            this.tbLocation.Name = "tbLocation";
            this.tbLocation.Size = new System.Drawing.Size(426, 20);
            this.tbLocation.TabIndex = 3;
            // 
            // checkSub
            // 
            this.checkSub.AutoSize = true;
            this.checkSub.Location = new System.Drawing.Point(630, 50);
            this.checkSub.Name = "checkSub";
            this.checkSub.Size = new System.Drawing.Size(115, 17);
            this.checkSub.TabIndex = 4;
            this.checkSub.Text = "Include sub-folders";
            this.checkSub.UseVisualStyleBackColor = true;
            // 
            // tbOutput
            // 
            this.tbOutput.Location = new System.Drawing.Point(5, 168);
            this.tbOutput.Multiline = true;
            this.tbOutput.Name = "tbOutput";
            this.tbOutput.ReadOnly = true;
            this.tbOutput.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.tbOutput.Size = new System.Drawing.Size(746, 248);
            this.tbOutput.TabIndex = 5;
            this.tbOutput.WordWrap = false;
            // 
            // btnBrowse
            // 
            this.btnBrowse.Location = new System.Drawing.Point(555, 46);
            this.btnBrowse.Name = "btnBrowse";
            this.btnBrowse.Size = new System.Drawing.Size(75, 23);
            this.btnBrowse.TabIndex = 6;
            this.btnBrowse.Text = "Browse";
            this.btnBrowse.UseVisualStyleBackColor = true;
            this.btnBrowse.Click += new System.EventHandler(this.btnBrowse_Click);
            // 
            // btnClr
            // 
            this.btnClr.Location = new System.Drawing.Point(422, 72);
            this.btnClr.Name = "btnClr";
            this.btnClr.Size = new System.Drawing.Size(77, 23);
            this.btnClr.TabIndex = 7;
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
            this.lblOutput.AutoSize = true;
            this.lblOutput.Location = new System.Drawing.Point(5, 155);
            this.lblOutput.Name = "lblOutput";
            this.lblOutput.Size = new System.Drawing.Size(42, 13);
            this.lblOutput.TabIndex = 9;
            this.lblOutput.Text = "Output:";
            // 
            // lblDrag
            // 
            this.lblDrag.AutoSize = true;
            this.lblDrag.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblDrag.Location = new System.Drawing.Point(118, 123);
            this.lblDrag.Name = "lblDrag";
            this.lblDrag.Size = new System.Drawing.Size(521, 20);
            this.lblDrag.TabIndex = 10;
            this.lblDrag.Text = "You can simply drag one or more files/folders to this window for checking.";
            // 
            // lnkResult
            // 
            this.lnkResult.AutoSize = true;
            this.lnkResult.Location = new System.Drawing.Point(669, 419);
            this.lnkResult.Name = "lnkResult";
            this.lnkResult.Size = new System.Drawing.Size(85, 13);
            this.lnkResult.TabIndex = 11;
            this.lnkResult.TabStop = true;
            this.lnkResult.Text = "Open Result File";
            this.lnkResult.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lnkResult_LinkClicked);
            // 
            // cbFix
            // 
            this.cbFix.AutoSize = true;
            this.cbFix.Location = new System.Drawing.Point(251, 76);
            this.cbFix.Name = "cbFix";
            this.cbFix.Size = new System.Drawing.Size(104, 17);
            this.cbFix.TabIndex = 12;
            this.cbFix.Text = "Try to fix defects";
            this.cbFix.UseVisualStyleBackColor = true;
            // 
            // MainWin
            // 
            this.AllowDrop = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(754, 432);
            this.Controls.Add(this.cbFix);
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
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "MainWin";
            this.Text = "Review Package Checker";
            this.DragDrop += new System.Windows.Forms.DragEventHandler(this.MainWin_DragDrop);
            this.DragEnter += new System.Windows.Forms.DragEventHandler(this.MainWin_DragEnter);
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
        private System.Windows.Forms.CheckBox cbFix;
    }
}

