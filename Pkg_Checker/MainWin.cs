using System;
using System.Linq;
using System.Windows.Forms;
using System.Collections.Generic;
using System.IO;
using Pkg_Checker.Interfaces;
using Pkg_Checker.Implementations;
using System.Threading;
using System.Drawing;
using System.Text.RegularExpressions;

namespace Pkg_Checker
{
    public partial class MainWin : Form
    {
        public MainWin()
        {
            InitializeComponent();
            this.ActiveControl = this.tbLocation;
        }

        private bool CheckThread(IPdfReader reader, String file)
        {
            reader.Read(file);
            if (!reader.IsValidReviewPackage())
                return false;

            reader.CheckCommonFields();
            reader.CheckWorkProductType();
            reader.CheckCheckList();
            reader.CheckSCRReportAndPrerequisiteFiles();
            reader.WorkWithAnnot();
            return reader.GetDefects().Count() > 0;
        }

        private void btnCheck_Click(object sender, EventArgs e)
        {
            bool errorOccurred = false;
            int CheckedFileCount = 0;

            if (String.IsNullOrWhiteSpace(this.tbLocation.Text))
            {
                this.ActiveControl = this.tbLocation;
                this.tbOutput.Text = @"Please specify a valid path.";
                return;
            }

            List<String> FileNames =
                Pkg_Checker.FSWalker.Walk(this.tbLocation.Text, "*.PDF", this.checkSub.Checked, ref errorOccurred);

            if (errorOccurred)
            {
                this.tbOutput.Text += "[Error] Error occurred while traversing the path." + Environment.NewLine;
            }

            if (FileNames != null && FileNames.Count > 0)
            {
                // consider the Singleton design pattern
                // Checker goodChecker = new Checker();
                // Aug 22, 2013. Create a instance for each file to avoid interaction effect between each files
                foreach (String fileName in FileNames)
                {
                    this.lblProcessStatus.Text = "Checking " + fileName;
                    // Checker goodChecker = new Checker();                    
                    // goodChecker.Check(fileName, this.cbFix.Checked, ref errorOccurred, Pkg_Checker.Program.ResultPath);
                    IPdfReader reader = new iTextPdfReader();
                    Thread thread = new Thread(() => CheckThread(reader, fileName));
                    thread.Start();
                    thread.Join();
                    reader.Close();

                    this.lblProcessStatus.Text = "Ready";
                    this.tbOutput.AppendText("[Info] Checked " + fileName + Environment.NewLine);
                    foreach (var item in reader.GetDefects())
                        this.tbOutput.AppendText(item + Environment.NewLine);
                    this.tbOutput.AppendText(@"------------------------------------------------------------" + Environment.NewLine);
                    ++CheckedFileCount;
                }
                this.tbOutput.AppendText("Checked " + CheckedFileCount + " of " + FileNames.Count + " file(s)." + Environment.NewLine);
            }

            else
            {
                this.tbOutput.Text = "No .pdf file(s) found." + Environment.NewLine;
            }
        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                tbLocation.Text = folderBrowserDialog1.SelectedPath;
            }
        }

        private void btnClr_Click(object sender, EventArgs e)
        {
            this.tbOutput.Text = "";
            System.IO.File.Delete(Pkg_Checker.Program.ResultPath);
            this.tbOutput.Text = "The result file was deleted." + Environment.NewLine;
        }

        private void MainWin_DragDrop(object sender, DragEventArgs e)
        {
            int CheckedFileCount = 0;
            bool errorOccurred = false;
            FileAttributes attr;
            List<String> FileNames = new List<String>();

            // dropped items may contain folders
            String[] s = (String[])e.Data.GetData(DataFormats.FileDrop, false);
            // List<String> FileNames = s.OfType<String>().ToList<String>();            

            for (int i = 0; i < s.Length; ++i)
            {
                attr = File.GetAttributes(s[i]);
                if ((attr & FileAttributes.Directory) == FileAttributes.Directory)
                {
                    foreach (var item in Pkg_Checker.FSWalker.Walk(s[i], "*.PDF", true, ref errorOccurred))
                        FileNames.Add(item);
                }

                // .pdf files are archives
                if (System.IO.Path.GetExtension(s[i]).ToUpper() == ".PDF")
                    FileNames.Add(s[i]);
            }

            // Process normal files
            foreach (var item in FileNames)
            {
                this.lblProcessStatus.Text = "Checking " + item;
                // Checker goodChecker = new Checker();
                // goodChecker.Check(item, this.cbFix.Checked, ref errorOccurred, Pkg_Checker.Program.ResultPath);
                this.lblProcessStatus.Text = "Ready";

                if (errorOccurred)
                    this.tbOutput.Text += "[Error] Error occurred while checking " + item + Environment.NewLine;
                else
                {
                    this.tbOutput.Text += "[Info] Checked " + item + Environment.NewLine;
                    ++CheckedFileCount;
                }
            }
            this.tbOutput.AppendText("Checked " + CheckedFileCount + " of " + FileNames.Count + " file(s)." + Environment.NewLine);
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

        protected override void WndProc(ref Message m)
        {
            FormWindowState previousWindowState = this.WindowState;
            base.WndProc(ref m);
            FormWindowState currentWindowState = this.WindowState;

            if (previousWindowState != currentWindowState && currentWindowState == FormWindowState.Maximized)
            {
                // Update UI here...
            }
        }

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
            this.tbOutput.Text += @"Multiple CTPs, multiple SCRs." + Environment.NewLine
                + @"Approved version vs. CM21 version." + Environment.NewLine
                + @"Defect count" + Environment.NewLine
                + @"Check .TRT existance" + Environment.NewLine
                + @"Justifications (Regular Expression)" + Environment.NewLine;
            //"TO DO:
            //2. Produced by                        
            //5. review status together with defect
            //6. defect number
            //7. approved version
            //8. review stamps
            //9. checklist acm info
            //10. justification box check
            //11. SCR report version info same as coversheet
            //12. TRT existance
            //13. trace checklist justification box"

        }

    }
}
