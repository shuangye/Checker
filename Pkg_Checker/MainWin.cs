using System;
using System.Windows.Forms;
using System.Collections.Generic;
using System.IO;
using Pkg_Checker.Interfaces;
using Pkg_Checker.Implementations;
using Pkg_Checker.Helpers;
using Pkg_Checker.Entities;
using System.Threading;
using System.ComponentModel;
using System.Text;
using System.Text.RegularExpressions;
using Pkg_Checker.Integration;

namespace Pkg_Checker
{
    public partial class MainWin : Form
    {
        private BackgroundWorker worker;
        private String outputFilePath = @"Check_Result.txt";
        public List<String> FilesToCheck { get; set; }
        public int CheckedFileCount { get; set; }

        // CM21
        private bool CheckWithCM21 { get; set; }
        public String EID { get; set; }
        public String CM21Password { get; set; }
        public String SCRReportDownloadPath { get; set; }

        public MainWin()
        {
            InitializeComponent();

            this.ActiveControl = this.tbLocation;            
            statusTotalProgress.Value = 0;
            FilesToCheck = new List<String>();
            CheckWithCM21 = false;

            worker = new BackgroundWorker();
            worker.WorkerReportsProgress = true;
            worker.WorkerSupportsCancellation = true;
            worker.DoWork += new DoWorkEventHandler(DoWork);
            worker.ProgressChanged += new ProgressChangedEventHandler(UpdateUI);
            worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(WorkDone);

            this.timer1.Interval = 1500;
            this.timer1.Start();
        }

        #region Background Task

        private void DoWork(object sender, DoWorkEventArgs e)
        {
            bool appendOutput = (bool)e.Argument;
            BackgroundWorker worker = sender as BackgroundWorker;
            StreamWriter SW = null;

            try { SW = new StreamWriter(outputFilePath, appendOutput, Encoding.Default); }
            catch (Exception ex)
            {
                MessageBox.Show(@"Cannot open result file for writting: " + ex.Message);
                if (null != SW)
                    SW.Close();
            }            

            // do work...
            foreach (var file in FilesToCheck)
            {
                IPdfReader reader = null;

                WorkProgress progress = new WorkProgress();
                progress.WorkName = file;
                progress.Type = WorkType.Start;                                
                worker.ReportProgress(CheckedFileCount, progress);

                #warning creating a new object each time may increase memory footprint
                // consider the Singleton design pattern
                try { reader = new iTextPdfReader(file); }
                catch (Exception ex)
                {
                    progress.Type = WorkType.FatalError;
                    progress.WorkResult = new List<String> { ex.Message + Environment.NewLine + Environment.NewLine
                         + "Please try to put ITEXTSHARP.DLL at the same place with this executable." };
                    worker.ReportProgress(CheckedFileCount, progress);
                    // worker.CancelAsync();

                    // UI related code should be placed in the UI thread.
                    // MessageBox.Show(ex.Message + Environment.NewLine + Environment.NewLine
                    //     + "Please try to put ITEXTSHARP.DLL at the same place with this executable.",
                    //     "Fatal Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    break;
                }

                if (reader.IsValidReviewPackage())
                {
                    reader.ReadWholeFile();
                    reader.CheckCommonFields();
                    reader.CheckReviewStatus();
                    reader.CheckComments();
                    reader.CheckWorkProducts();
                    reader.CheckCheckList();
                    if (this.CheckWithCM21)
                        reader.CheckWithCM21(EID, CM21Password, SCRReportDownloadPath);

                    // ++CheckedFileCount;
                    progress.Type = WorkType.End;
                    progress.WorkResult = reader.GetDefects();

                    if (null != SW)
                    {
                        foreach (var line in progress.WorkResult)
                            SW.WriteLine(CommonResource.DefectPrefix + line);
                        SW.WriteLine(@"Checked " + file);
                        SW.WriteLine(CommonResource.LineSeperator);
                    }
                }
                else
                {
                    progress.Type = WorkType.ErrorOccurred;
                    progress.WorkResult = new List<string> { file + " is not a valid review package." + Environment.NewLine };
                }

                // output check result
                ++CheckedFileCount;
                worker.ReportProgress(CheckedFileCount, progress);
            }

            if (null != SW)
                SW.Close();
        }

        private void UpdateUI(object sender, ProgressChangedEventArgs e)
        {            
            statusTotalProgress.Value = e.ProgressPercentage;

            WorkProgress progress = (WorkProgress)e.UserState;
            switch (progress.Type)
            {
                case WorkType.Start:
                    tbOutput.AppendText(@"Begin to check " + progress.WorkName + Environment.NewLine);
                    statusCurrentObj.Text = String.Format(@"Checking {0} ... {1} of {2}.", 
                        progress.WorkName, e.ProgressPercentage + 1, FilesToCheck.Count);
                    break;

                case WorkType.End:
                    if (null != progress.WorkResult)
                        foreach (var defect in progress.WorkResult)
                            tbOutput.AppendText(CommonResource.DefectPrefix + defect + Environment.NewLine);
                    tbOutput.AppendText(@"Done checking " + progress.WorkName + Environment.NewLine
                        + CommonResource.LineSeperator + Environment.NewLine);
                    break;

                case WorkType.ErrorOccurred:
                    tbOutput.AppendText(progress.WorkResult[0]);
                    break;

                case WorkType.FatalError:
                    MessageBox.Show(progress.WorkResult[0],
                         "Fatal Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    break;

                default:
                    break;
            }
        }

        private void WorkDone(object sender, RunWorkerCompletedEventArgs e)
        {
            CheckedFileCount = 0;
            FilesToCheck.Clear();
            statusCurrentObj.Text = String.Format(@"Done! Checked {0} of {1} file(s).", CheckedFileCount, FilesToCheck.Count);            
            this.btnCheck.Enabled = true;
            // this.chkCM21.Enabled = true;
            this.timer1.Start();
        }

        #endregion Background Task
        
        
        //protected override void WndProc(ref Message m)
        //{
        //    FormWindowState previousWindowState = this.WindowState;
        //    base.WndProc(ref m);
        //    FormWindowState currentWindowState = this.WindowState;

        //    if (previousWindowState != currentWindowState && currentWindowState == FormWindowState.Maximized)
        //    {
        //        // Update UI here...
        //    }
        //}

        #region Menus

        private void knownIssuesStripMenuItem_Click(object sender, EventArgs e)
        {
            tbOutput.AppendText(@"Missing report for SCR XXX." + Environment.NewLine
                + @"XXX.TRT is not printed to the review package." + Environment.NewLine);
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            tbOutput.AppendText(@"This tool checks the potential defects in a review package." + Environment.NewLine
                + @"Build Date: Nov 25, 2014." + Environment.NewLine);
        }

        #endregion Menus

        #region UI Elements Events

        private bool ValidateUserInput()
        {
            if (!this.chkAppendOutput.Checked)
                this.tbOutput.Text = @"";

            if (CheckWithCM21)
            {
                if ((String.IsNullOrWhiteSpace(txtEID.Text) || String.IsNullOrWhiteSpace(txtPWD.Text)))
                {
                    MessageBox.Show("Invalid CM21 credential.", 
                        Program.AppName, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return false;
                }

                try
                {
                    // Directory.Exists(txtSCRDownloadPath.Text);
                    Directory.GetAccessControl(txtSCRDownloadPath.Text);
                }
                catch
                {
                    MessageBox.Show("Cannot write to the SCR report download path.", 
                        Program.AppName, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return false;
                }

                EID = this.txtEID.Text.Trim();
                CM21Password = this.txtPWD.Text;
                SCRReportDownloadPath = this.txtSCRDownloadPath.Text;
            }

            return true;
        }

        private void LaunchTasks(int count)
        {
            if (count > 0)
            {                
                this.timer1.Stop();
                this.btnCheck.Enabled = false;
                // this.chkCM21.Enabled = false;
                this.lblDrag.Visible = false;
                statusTotalProgress.Maximum = count;
                worker.RunWorkerAsync(this.chkAppendOutput.Checked);
            }
            else
                tbOutput.Text = "No .pdf files found." + Environment.NewLine;
        }

        private void MainWin_DragEnter(object sender, DragEventArgs e)
        {
            e.Effect = e.Data.GetDataPresent(DataFormats.FileDrop) ?
                DragDropEffects.Copy : DragDropEffects.None;
        }

        private void MainWin_DragDrop(object sender, DragEventArgs e)
        {
            if (!ValidateUserInput())
                return;

            bool errorOccurred = false;
            // dropped items may contain folders
            String[] s = (String[])e.Data.GetData(DataFormats.FileDrop, false);
            for (int i = 0; i < s.Length; ++i)
            {
                FileAttributes attr = File.GetAttributes(s[i]);
                if ((attr & FileAttributes.Directory) == FileAttributes.Directory)
                    foreach (var item in Pkg_Checker.Helpers.FSWalker.Walk(s[i], "*.PDF", true, ref errorOccurred))
                        FilesToCheck.Add(item);

                // .pdf files are archives
                if (".PDF".Equals(System.IO.Path.GetExtension(s[i]).ToUpper()))
                    FilesToCheck.Add(s[i]);
            }

            if (errorOccurred)
            {
                tbOutput.AppendText("[Error] Cannot traverse the path." + Environment.NewLine);
                return;
            }

            LaunchTasks(FilesToCheck.Count);
        }

        private void btnCheck_Click(object sender, EventArgs e)
        {
            // CM21 cm21 = new CM21(null, "E817739", "Work88888_", "FMS2000", "A350_A380", @"D:\");
            // cm21.FetchSCRReport(new List<SCRReport> { new SCRReport { SCRNumber = 9499.00F } });
            // cm21.Exit();

            if (!ValidateUserInput())
                return;

            if (String.IsNullOrWhiteSpace(this.tbLocation.Text))
            {
                this.ActiveControl = this.tbLocation;
                this.tbOutput.Text = @"Please specify a valid path.";
                return;
            }

            bool errorOccurred = false;
            FilesToCheck.AddRange(Pkg_Checker.Helpers.FSWalker.Walk(this.tbLocation.Text, "*.PDF", this.checkSub.Checked, ref errorOccurred));
            if (errorOccurred)
            {
                tbOutput.AppendText("[Error] Cannot travere the specified path." + Environment.NewLine);
                return;
            }

            LaunchTasks(FilesToCheck.Count);
        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                Button btnSender = (Button)sender;
                if (btnSender.Equals(this.btnBrowse))
                    tbLocation.Text = folderBrowserDialog1.SelectedPath;
                else if (btnSender.Equals(this.btnSCRDownloadPath))
                    txtSCRDownloadPath.Text = folderBrowserDialog1.SelectedPath;
            }
        }

        private void btnClr_Click(object sender, EventArgs e)
        {
            this.tbOutput.Text = "";
            System.IO.File.Delete(outputFilePath);
        }

        private void lnkResult_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            String absoluteResultPath =
                    System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location)
                    + "\\" + outputFilePath;

            if (File.Exists(absoluteResultPath))
            {
                try
                {
                    this.lnkResult.LinkVisited = true;
                    System.Diagnostics.Process.Start(absoluteResultPath);
                }

                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }

            else
            {
                this.tbOutput.Text = "[Warning] Result file " + absoluteResultPath + " has been deleted." + Environment.NewLine;
            }
        }

        // private void chkCM21_CheckedChanged(object sender, EventArgs e)
        // {
        //     panelCM21.Visible = chkCM21.Checked;
        //     CheckWithCM21 = chkCM21.Checked;
        // }

        private void timer1_Tick(object sender, EventArgs e)
        {
            this.lblDrag.Visible = !this.lblDrag.Visible;
        }
        
        private void menuEnableCm21_Click(object sender, EventArgs e)
        {
            // this.menuEnableCm21.Checked;
            this.groupCM21.Visible = this.menuEnableCm21.Checked;
            CheckWithCM21 = this.menuEnableCm21.Checked;
            if (this.menuEnableCm21.Checked)
            {
                this.panelMisc.Location = new System.Drawing.Point
                {
                    X = this.panelMisc.Location.X,
                    Y = this.panelMisc.Location.Y - this.groupCM21.Height
                };
                this.tbOutput.Size = new System.Drawing.Size
                {
                    Height = this.tbOutput.Size.Height - this.groupCM21.Height,
                    Width = this.tbOutput.Size.Width                    
                };
            }
            else
            {
                this.panelMisc.Location = new System.Drawing.Point
                {
                    X = this.panelMisc.Location.X,
                    Y = this.panelMisc.Location.Y + this.groupCM21.Height
                };
            }
        }

        #endregion UI Elements Events
    }
}
