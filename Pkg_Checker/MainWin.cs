﻿using System;
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

namespace Pkg_Checker
{
    public partial class MainWin : Form
    {
        private BackgroundWorker worker;
        private String outputFilePath = @"Check_Result.txt";
        private String registryKeyNameEID = "EID";
        private String registryKeyNamePWD = "PWD";
        private List<String> FilesToCheck { get; set; }
        private int CheckedFileCount { get; set; }

        // CM21
        private bool CheckWithCM21 { get; set; }
        private String EID { get; set; }
        private String CM21Password { get; set; }
        private String SCRReportDownloadPath { get; set; }
        private int Timeout { get; set; }

        public MainWin(string[] args)
        {
            InitializeComponent();

            bool fireTask = false;
            bool isCM21InfoPresent = false;

            FilesToCheck = new List<String>();
            worker = new BackgroundWorker();
            worker.WorkerReportsProgress = true;
            worker.WorkerSupportsCancellation = true;
            worker.DoWork += new DoWorkEventHandler(DoWork);
            worker.ProgressChanged += new ProgressChangedEventHandler(UpdateUI);
            worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(WorkDone);
            
            if (null != args)
            {
                switch (args.Length)
                {
                    case 3:
                        // app reviewPackagePath resultPath
                        this.tbLocation.Text = args[1];
                        outputFilePath = args[2];
                        fireTask = true;
                        break;
                    case 6:
                        // app reviewPackagePath resultPath EID CM21Password SCRReportDownloadPath
                        this.tbLocation.Text = args[1];
                        outputFilePath = args[2];
                        this.txtEID.Text = args[3];
                        this.txtPWD.Text = args[4];
                        this.txtSCRDownloadPath.Text = args[5];
                        isCM21InfoPresent = true;
                        fireTask = true;
                        break;
                    case 7:
                        // app reviewPackagePath resultPath EID CM21Password SCRReportDownloadPath timeout
                        this.tbLocation.Text = args[1];
                        outputFilePath = args[2];
                        this.txtEID.Text = args[3];
                        this.txtPWD.Text = args[4];
                        this.txtSCRDownloadPath.Text = args[5];
                        Match match = Regex.Match(args[6], @"\d+");
                        if (match.Success)
                            this.spinTimeout.Value = int.Parse(match.Value);
                        isCM21InfoPresent = true;
                        fireTask = true;
                        break;
                    default:
                        break;
                }
            }

            // UI elements
            CheckWithCM21 = false;
            SlideControls(CheckWithCM21, this.groupCM21.Height, this.btnCheck, this.lblDrag,
                this.chkAppendOutput, this.lnkResult, this.btnClr, this.tbOutput);
            this.statusTotalProgress.Value = 0;
            this.ActiveControl = this.tbLocation;            
            this.timer1.Interval = 1500;
            this.timer1.Start();            

            this.Load += (sender, e) =>
            {
                // simulate UI operations 
                if (isCM21InfoPresent)
                    this.menuEnableCm21.PerformClick();
                if (fireTask)
                    this.btnCheck.PerformClick();
            };
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
                    progress.Warnings = new List<String> { ex.Message + Environment.NewLine + Environment.NewLine
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
                    if (CheckWithCM21)
                        reader.CheckWithCM21(EID, CM21Password, SCRReportDownloadPath, Timeout);

                    // ++CheckedFileCount;
                    progress.Type = WorkType.End;
                    progress.Defects = reader.GetDefects();
                    progress.Warnings = reader.GetWarnings();

                    if (null != SW)
                    {
                        foreach (var line in progress.Defects)
                            SW.WriteLine(CommonResource.DefectPrefix + line);
                        SW.WriteLine(@"Checked " + file);
                        SW.WriteLine(CommonResource.LineSeperator);
                    }
                }
                else
                {
                    progress.Type = WorkType.ErrorOccurred;
                    progress.Warnings = new List<string> { file + " is not a valid review package." + Environment.NewLine };
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
                    if (null != progress.Defects)
                        foreach (var defect in progress.Defects)
                            tbOutput.AppendText(CommonResource.DefectPrefix + defect + Environment.NewLine);
                    if (null != progress.Warnings)
                        foreach (var warning in progress.Warnings)
                            tbOutput.AppendText(CommonResource.WarningPrefix + warning + Environment.NewLine);
                    tbOutput.AppendText(@"Done checking " + progress.WorkName + Environment.NewLine
                        + CommonResource.LineSeperator + Environment.NewLine);
                    break;

                case WorkType.ErrorOccurred:
                    if (null != progress.Defects)
                        tbOutput.AppendText(progress.Defects[0]);
                    else if (null != progress.Warnings)
                        tbOutput.AppendText(progress.Warnings[0]);
                    break;

                case WorkType.FatalError:
                    MessageBox.Show(progress.Defects[0],
                         "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    break;

                default:
                    break;
            }
        }

        private void WorkDone(object sender, RunWorkerCompletedEventArgs e)
        {            
            statusCurrentObj.Text = String.Format(@"Done! Checked {0} of {1} file(s).", CheckedFileCount, FilesToCheck.Count);
            CheckedFileCount = 0;
            FilesToCheck.Clear();
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
        private void menuEnableCm21_Click(object sender, EventArgs e)
        {
            // this.groupCM21.Visible = this.menuEnableCm21.Checked;
            this.instructionsToolStripMenuItem.Enabled = this.menuEnableCm21.Checked;
            CheckWithCM21 = this.menuEnableCm21.Checked;
            if (CheckWithCM21)
            {
                string eid, pwd;
                RegistryOperator.ReadRegistry(Program.AppName, registryKeyNameEID, out eid, registryKeyNamePWD, out pwd);
                if (String.IsNullOrWhiteSpace(this.txtEID.Text))
                    this.txtEID.Text = eid;
                if (String.IsNullOrWhiteSpace(this.txtPWD.Text))
                    this.txtPWD.Text = pwd;
                this.chkSave.Checked = CheckWithCM21;
            }

            SlideControls(CheckWithCM21, this.groupCM21.Height, this.btnCheck,
                this.lblDrag, this.chkAppendOutput, this.lnkResult, this.btnClr, this.tbOutput);
        }

        private void instructionsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            tbOutput.AppendText(@"This program uses the SCR reports cached in the SCR report download path to avoid duplicate fetching from CM21."
                + Environment.NewLine + "CM21 process may become stuck, so it will be killed after timeout." + Environment.NewLine
                );            
        }

        private void howToToolStripMenuItem_Click(object sender, EventArgs e)
        {
            tbOutput.AppendText(@"Ho to start from CLI and execute immediately:" + Environment.NewLine
                + @"Start without CM21 integration: thisApp reviewPackagePath resultPath" + Environment.NewLine
                + @"Start with CM21 integration: thisApp reviewPackagePath resultPath EID CM21Password SCRReportDownloadPath" + Environment.NewLine
                + @"Start with CM21 integration and a timeout value (in seconds): thisApp reviewPackagePath resultPath EID CM21Password SCRReportDownloadPath timeout" + Environment.NewLine
                );
        }

        private void knownIssuesStripMenuItem_Click(object sender, EventArgs e)
        {
            tbOutput.AppendText(@"If some pdf pages cannot be read by the third party lib, this program will report:" + Environment.NewLine
                + @"Missing report for SCR XXX." + Environment.NewLine
                + @"XXX.TRT is not printed to the review package." + Environment.NewLine);
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            tbOutput.AppendText(@"This tool checks the potential defects in CTP/SLTP review packages." + Environment.NewLine
                + @"Build Date: Dec 23, 2014." + Environment.NewLine);
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
                    if (!Directory.Exists(txtSCRDownloadPath.Text))
                        throw new DirectoryNotFoundException(String.Format("Specified directory {0} is not found.", txtSCRDownloadPath.Text));

                    Directory.GetAccessControl(txtSCRDownloadPath.Text);
                }
                catch(Exception ex)
                {
                    MessageBox.Show("Cannot write to the SCR report download path: " + ex.Message,
                        Program.AppName, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return false;
                }

                EID = this.txtEID.Text.Trim().ToUpper();
                CM21Password = this.txtPWD.Text;
                Timeout = Convert.ToInt32(this.spinTimeout.Value);
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
            if (!ValidateUserInput())
                return;

            if (String.IsNullOrWhiteSpace(this.tbLocation.Text))
            {
                this.ActiveControl = this.tbLocation;
                this.tbOutput.Text = @"Please specify a valid review package path." + Environment.NewLine;
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

        private void timer1_Tick(object sender, EventArgs e)
        {
            this.lblDrag.Visible = !this.lblDrag.Visible;
        }
                
        private void SlideControls(bool visible, int offset, params Control[] controls)
        {
            if (null == controls || controls.Length <= 0)
                return;

            const int times = 20;
            if (!visible)
            {
                offset = -offset;
                // hiding goes before animation, thus to alleviate stuck
                this.groupCM21.Visible = visible;
            }
            int delta = offset / times;  // ensure offset / times is an integer
            Thread.Sleep(200);  // delay before the animation starts            
            
            for (int i = 0; i < times; ++i)
            {
                foreach (Control control in controls)                
                    control.Top += delta;
                this.tbOutput.Height += delta;
                this.UpdateBounds();
                Thread.Sleep(3);
            }
            
            this.groupCM21.Visible = visible;            
        }

        // another implementation
        private void _SlideControls(bool visible, int offset, params Control[] controls)
        {
            if (null == controls || controls.Length <= 0)
                return;
            
            int tickCount = 0;
            const int times = 40;  // how many pieces to divide the sliding            
            if (!visible)
            {
                offset = -offset;
                // hiding goes before animation, thus to alleviate stuck
                this.groupCM21.Visible = visible;
            }
            int delta = offset / times;  // ensure offset / times is an integer            
            System.Windows.Forms.Timer timer = new System.Windows.Forms.Timer { Interval = 2 };            
            timer.Tick += (sender, e) =>
            {
                if (tickCount >= times)
                {
                    timer.Stop();
                    timer.Dispose();
                    this.groupCM21.Visible = visible;
                    return;
                }

                ++tickCount;
                foreach (Control control in controls)                                    
                    control.Top += delta;
                UpdateBounds();
            };            
            timer.Start();            
        }
        
        private void MainWin_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (CheckWithCM21)
            {
                if (this.chkSave.Checked)
                    RegistryOperator.WriteRegistry(Program.AppName, registryKeyNameEID, this.txtEID.Text, registryKeyNamePWD, this.txtPWD.Text);
                else
                    RegistryOperator.WriteRegistry(Program.AppName, registryKeyNameEID, String.Empty, registryKeyNamePWD, String.Empty);
            }
        }

        #endregion UI Elements Events                        

        
    }
}
