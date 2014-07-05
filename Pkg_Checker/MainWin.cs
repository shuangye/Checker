﻿using System;
using System.Linq;
using System.Windows.Forms;
using System.Collections.Generic;
using System.IO;
using Pkg_Checker.Interfaces;
using Pkg_Checker.Implementations;
using System.Threading;
using System.ComponentModel;
using Pkg_Checker.Entities;

namespace Pkg_Checker
{
    public partial class MainWin : Form
    {
        private BackgroundWorker worker;
        public List<String> FilesToCheck { get; set; }
        public int CheckedFileCount { get; set; }

        public MainWin()
        {
            InitializeComponent();

            this.ActiveControl = this.tbLocation;
            totalProgress.Value = 0;
            FilesToCheck = new List<String>();

            worker = new BackgroundWorker();
            worker.WorkerReportsProgress = true;
            worker.DoWork += new DoWorkEventHandler(DoWork);
            worker.ProgressChanged += new ProgressChangedEventHandler(UpdateUI);
            worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(WorkDone);
        }

        private void DoWork(object sender, DoWorkEventArgs e)
        {            
            WorkProgress progress = new WorkProgress();            
            // int TotalTasks = (int)e.Argument;

            // do work...
            foreach (var file in FilesToCheck)
            {
                progress.WorkName = file;
                progress.Type = WorkType.Start;

                // update UI
                worker.ReportProgress(CheckedFileCount, progress);

                #warning creating a new object each time may increase memory footprint
                // consider the Singleton design pattern
                IPdfReader reader = new iTextPdfReader();
                reader.Read(file);
                if (reader.IsValidReviewPackage())
                {                    
                    reader.TraverseWholeFile();
                    reader.CheckCommonFields();
                    reader.CheckReviewStatus();
                    reader.CheckWorkProducts();
                    reader.CheckCheckList();
                    reader.CheckWholeFileWide();
                    ++CheckedFileCount;
                    progress.Type = WorkType.End;
                    progress.WorkResult = reader.GetDefects();
                }    
                else
                    progress.Type = WorkType.ErrorOccurred;

                // output check result                
                worker.ReportProgress(CheckedFileCount, progress);
            }
        }

        private void UpdateUI(object sender, ProgressChangedEventArgs e)
        {
            totalProgress.Value = e.ProgressPercentage;

            WorkProgress progress = (WorkProgress)e.UserState;
            switch (progress.Type)
            {
                case WorkType.Start:
                    tbOutput.Text += @"Begin to check " + progress.WorkName + Environment.NewLine;
                    lblProcessStatus.Text = String.Format(@"Checking {0} ... {1} of {2}.", 
                        progress.WorkName, e.ProgressPercentage + 1, FilesToCheck.Count);
                    break;
                case WorkType.End:
                    if (null != progress.WorkResult)
                        foreach (var defect in progress.WorkResult)
                            tbOutput.Text += defect + Environment.NewLine;
                    tbOutput.Text += @"Checked " + progress.WorkName + Environment.NewLine
                                     + @"---------------------------------------------------------------------------"
                                     + Environment.NewLine;
                    break;
                case WorkType.ErrorOccurred:
                    tbOutput.Text += String.Format(@"Failed to check" + progress.WorkName +
                        ", or it is not a valid review package." + Environment.NewLine);
                    break;
                default:
                    break;
            }
        }

        private void WorkDone(object sender, RunWorkerCompletedEventArgs e)
        {
            lblProcessStatus.Text = String.Format(@"Done! Checked {0} of {1} files.", CheckedFileCount, FilesToCheck.Count);
            btnCheck.Enabled = true;
            FilesToCheck.Clear();
        }

        private void btnCheck_Click(object sender, EventArgs e)
        {
            btnCheck.Enabled = false;

            if (String.IsNullOrWhiteSpace(this.tbLocation.Text))
            {
                this.ActiveControl = this.tbLocation;
                this.tbOutput.Text = @"Please specify a valid path.";
                return;
            }

            bool errorOccurred = false;
            FilesToCheck.AddRange(Pkg_Checker.FSWalker.Walk(this.tbLocation.Text, "*.PDF", this.checkSub.Checked, ref errorOccurred));
            if (errorOccurred)
            {
                this.tbOutput.Text += "[Error] Error occurred while traversing the path." + Environment.NewLine;
                return;
            }

            if (FilesToCheck.Count > 0)
            {
                totalProgress.Maximum = FilesToCheck.Count;
                worker.RunWorkerAsync(FilesToCheck.Count);
            }
            else
                tbOutput.Text = "No .pdf files found." + Environment.NewLine;
        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)            
                tbLocation.Text = folderBrowserDialog1.SelectedPath;            
        }

        private void btnClr_Click(object sender, EventArgs e)
        {
            this.tbOutput.Text = "";
            System.IO.File.Delete(Pkg_Checker.Program.ResultPath);
            this.tbOutput.Text = "The result file was deleted." + Environment.NewLine;
        }

        private void MainWin_DragDrop(object sender, DragEventArgs e)
        {
            btnCheck.Enabled = false;
            bool errorOccurred = false;

            // dropped items may contain folders
            String[] s = (String[])e.Data.GetData(DataFormats.FileDrop, false);
            for (int i = 0; i < s.Length; ++i)
            {
                FileAttributes attr = File.GetAttributes(s[i]);
                if ((attr & FileAttributes.Directory) == FileAttributes.Directory)                
                    foreach (var item in Pkg_Checker.FSWalker.Walk(s[i], "*.PDF", true, ref errorOccurred))
                        FilesToCheck.Add(item);                

                // .pdf files are archives
                if (System.IO.Path.GetExtension(s[i]).ToUpper() == ".PDF")
                    FilesToCheck.Add(s[i]);
            }

            if (errorOccurred)
            {
                this.tbOutput.Text += "[Error] Error occurred while traversing the path." + Environment.NewLine;
                return;
            }

            if (FilesToCheck.Count > 0)
            {
                totalProgress.Maximum = FilesToCheck.Count;
                worker.RunWorkerAsync(FilesToCheck.Count);
            }
            else
                tbOutput.Text = "No .pdf files found." + Environment.NewLine;
        }

        private void MainWin_DragEnter(object sender, DragEventArgs e)
        {
            e.Effect = e.Data.GetDataPresent(DataFormats.FileDrop) ?
                DragDropEffects.Copy : DragDropEffects.None;
        }

        private void lnkResult_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            String AbsoluteResultPath =
                    System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location)
                    + "\\" + Pkg_Checker.Program.ResultPath;

            if (File.Exists(AbsoluteResultPath))
            {
                try
                {
                    this.lnkResult.LinkVisited = true;
                    System.Diagnostics.Process.Start(AbsoluteResultPath);
                }

                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }

            else
            {
                this.tbOutput.Text = "[Warning] The result file " + AbsoluteResultPath + " has been deleted." + Environment.NewLine;
            }
        }

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

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.tbOutput.Text += @"This tool checks the potential defects in a review package." + Environment.NewLine +
                @"Bug report: mingyang.liu@honeywell.com" + Environment.NewLine +
                @"Powered by iTextSharp." + Environment.NewLine;
        }

        private void supportedChecksToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.tbOutput.Text += @"Currently, this tool can check the following items:" + Environment.NewLine;
            this.tbOutput.Text += @"To be filled..." + Environment.NewLine;
        }

        private void toDoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.tbOutput.Text +=
                @"Approved version vs. CM21 version." + Environment.NewLine;         
            //"TO DO:            
            // How to collect all state models that belong to one sticky note?        
            // Approved ver vs. max ver in CM21                   
        }
    }
}
