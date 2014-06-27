namespace demo_readpdf
{
    partial class Form1
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.dgvData = new System.Windows.Forms.DataGridView();
            this.tbPath = new System.Windows.Forms.TextBox();
            this.Collect_Defect = new System.Windows.Forms.Button();
            this.Browse = new System.Windows.Forms.Button();
            this.btSave = new System.Windows.Forms.Button();
            this.btClear = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.Series = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Packet = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.FArea = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Status = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.CompleteDate = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Author = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Moderator = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Reviewer = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Page_Number = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Comments = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Migration = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Defect_Type = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.IsDefectState = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ResolutionStatus = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Defect_Severity = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Reply = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.dgvData)).BeginInit();
            this.SuspendLayout();
            // 
            // dgvData
            // 
            this.dgvData.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvData.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Series,
            this.Packet,
            this.FArea,
            this.Status,
            this.CompleteDate,
            this.Author,
            this.Moderator,
            this.Reviewer,
            this.Page_Number,
            this.Comments,
            this.Migration,
            this.Defect_Type,
            this.IsDefectState,
            this.ResolutionStatus,
            this.Defect_Severity,
            this.Reply});
            this.dgvData.Location = new System.Drawing.Point(23, 38);
            this.dgvData.Name = "dgvData";
            this.dgvData.RowHeadersWidth = 25;
            this.dgvData.Size = new System.Drawing.Size(1360, 800);
            this.dgvData.TabIndex = 7;
            // 
            // tbPath
            // 
            this.tbPath.Location = new System.Drawing.Point(114, 12);
            this.tbPath.Name = "tbPath";
            this.tbPath.Size = new System.Drawing.Size(313, 20);
            this.tbPath.TabIndex = 5;
            // 
            // Collect_Defect
            // 
            this.Collect_Defect.Location = new System.Drawing.Point(537, 11);
            this.Collect_Defect.Name = "Collect_Defect";
            this.Collect_Defect.Size = new System.Drawing.Size(72, 21);
            this.Collect_Defect.TabIndex = 6;
            this.Collect_Defect.Text = "Collect";
            this.Collect_Defect.UseVisualStyleBackColor = true;
            this.Collect_Defect.Click += new System.EventHandler(this.Collect_Defect_Click);
            // 
            // Browse
            // 
            this.Browse.Location = new System.Drawing.Point(443, 11);
            this.Browse.Name = "Browse";
            this.Browse.Size = new System.Drawing.Size(72, 21);
            this.Browse.TabIndex = 4;
            this.Browse.Text = "Browse";
            this.Browse.UseVisualStyleBackColor = true;
            this.Browse.Click += new System.EventHandler(this.Browse_Click);
            // 
            // btSave
            // 
            this.btSave.Location = new System.Drawing.Point(631, 11);
            this.btSave.Name = "btSave";
            this.btSave.Size = new System.Drawing.Size(72, 21);
            this.btSave.TabIndex = 8;
            this.btSave.Text = "Export";
            this.btSave.UseVisualStyleBackColor = true;
            this.btSave.Click += new System.EventHandler(this.btSave_Click);
            // 
            // btClear
            // 
            this.btClear.Location = new System.Drawing.Point(728, 11);
            this.btClear.Name = "btClear";
            this.btClear.Size = new System.Drawing.Size(72, 21);
            this.btClear.TabIndex = 9;
            this.btClear.Text = "Clear";
            this.btClear.UseVisualStyleBackColor = true;
            this.btClear.Click += new System.EventHandler(this.btClear_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(29, 15);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(85, 13);
            this.label1.TabIndex = 10;
            this.label1.Text = "PDF File Folder :";
            // 
            // Series
            // 
            this.Series.HeaderText = "Series";
            this.Series.MinimumWidth = 2;
            this.Series.Name = "Series";
            this.Series.Width = 40;
            // 
            // Packet
            // 
            this.Packet.HeaderText = "Packet";
            this.Packet.Name = "Packet";
            this.Packet.Width = 170;
            // 
            // FArea
            // 
            this.FArea.HeaderText = "FArea";
            this.FArea.Name = "FArea";
            this.FArea.Width = 50;
            // 
            // Status
            // 
            this.Status.HeaderText = "Status";
            this.Status.Name = "Status";
            this.Status.Width = 50;
            // 
            // CompleteDate
            // 
            this.CompleteDate.HeaderText = "CompleteDate";
            this.CompleteDate.Name = "CompleteDate";
            // 
            // Author
            // 
            this.Author.HeaderText = "Author";
            this.Author.Name = "Author";
            this.Author.Width = 80;
            // 
            // Moderator
            // 
            this.Moderator.HeaderText = "Moderator";
            this.Moderator.Name = "Moderator";
            this.Moderator.Width = 80;
            // 
            // Reviewer
            // 
            this.Reviewer.HeaderText = "Reviewer";
            this.Reviewer.Name = "Reviewer";
            this.Reviewer.Width = 80;
            // 
            // Page_Number
            // 
            this.Page_Number.HeaderText = "Page Number";
            this.Page_Number.Name = "Page_Number";
            this.Page_Number.Width = 80;
            // 
            // Comments
            // 
            this.Comments.HeaderText = "Comments";
            this.Comments.Name = "Comments";
            this.Comments.Width = 155;
            // 
            // Migration
            // 
            this.Migration.HeaderText = "Migration";
            this.Migration.Name = "Migration";
            this.Migration.Width = 80;
            // 
            // Defect_Type
            // 
            this.Defect_Type.HeaderText = "Defect_Type";
            this.Defect_Type.Name = "Defect_Type";
            this.Defect_Type.Width = 80;
            // 
            // IsDefectState
            // 
            this.IsDefectState.HeaderText = "IsDefectState";
            this.IsDefectState.Name = "IsDefectState";
            this.IsDefectState.Width = 80;
            // 
            // ResolutionStatus
            // 
            this.ResolutionStatus.HeaderText = "ResolutionStatus";
            this.ResolutionStatus.Name = "ResolutionStatus";
            this.ResolutionStatus.Width = 80;
            // 
            // Defect_Severity
            // 
            this.Defect_Severity.HeaderText = "Defect_Severity";
            this.Defect_Severity.Name = "Defect_Severity";
            this.Defect_Severity.Width = 80;
            // 
            // Reply
            // 
            this.Reply.HeaderText = "Reply";
            this.Reply.Name = "Reply";
            this.Reply.Width = 150;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.ClientSize = new System.Drawing.Size(852, 566);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btClear);
            this.Controls.Add(this.btSave);
            this.Controls.Add(this.dgvData);
            this.Controls.Add(this.tbPath);
            this.Controls.Add(this.Collect_Defect);
            this.Controls.Add(this.Browse);
            this.MinimumSize = new System.Drawing.Size(860, 600);
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Defect Collection";
            ((System.ComponentModel.ISupportInitialize)(this.dgvData)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView dgvData;
        private System.Windows.Forms.TextBox tbPath;
        private System.Windows.Forms.Button Collect_Defect;
        private System.Windows.Forms.Button Browse;
        private System.Windows.Forms.Button btSave;
        private System.Windows.Forms.Button btClear;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.DataGridViewTextBoxColumn Series;
        private System.Windows.Forms.DataGridViewTextBoxColumn Packet;
        private System.Windows.Forms.DataGridViewTextBoxColumn FArea;
        private System.Windows.Forms.DataGridViewTextBoxColumn Status;
        private System.Windows.Forms.DataGridViewTextBoxColumn CompleteDate;
        private System.Windows.Forms.DataGridViewTextBoxColumn Author;
        private System.Windows.Forms.DataGridViewTextBoxColumn Moderator;
        private System.Windows.Forms.DataGridViewTextBoxColumn Reviewer;
        private System.Windows.Forms.DataGridViewTextBoxColumn Page_Number;
        private System.Windows.Forms.DataGridViewTextBoxColumn Comments;
        private System.Windows.Forms.DataGridViewTextBoxColumn Migration;
        private System.Windows.Forms.DataGridViewTextBoxColumn Defect_Type;
        private System.Windows.Forms.DataGridViewTextBoxColumn IsDefectState;
        private System.Windows.Forms.DataGridViewTextBoxColumn ResolutionStatus;
        private System.Windows.Forms.DataGridViewTextBoxColumn Defect_Severity;
        private System.Windows.Forms.DataGridViewTextBoxColumn Reply;

    }
}

