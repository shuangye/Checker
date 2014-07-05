using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Threading
{
    public partial class BgWorker : Form
    {
        private BackgroundWorker bgWorker;
        const int MaxValue = 100;
        int currentValue = 0;

        public BgWorker()
        {
            InitializeComponent();
            lblStatus.Visible = false;

            bgWorker = new BackgroundWorker();
            bgWorker.WorkerReportsProgress = true;
            bgWorker.DoWork += new DoWorkEventHandler(DoWork);
            bgWorker.ProgressChanged += new ProgressChangedEventHandler(UpdateUI);
            bgWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(WorkDone);
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            // trigger background worker
            bgWorker.RunWorkerAsync();
            btnStart.Enabled = false;
        }

        private void DoWork(object sender, DoWorkEventArgs e)
        {
            // do work...
            // for (; currentValue < MaxValue; ++currentValue)
            // {                
            //     // update UI
            //     bgWorker.ReportProgress(currentValue);
            //     Thread.Sleep(100);
            // }

            // calc PI
            // PI = 4/1 - 4/3 + 4/5 - 4/7 + 4/9 ... Leibniz infinite series
            double PI = 4.0;
            for (long i = 3; i <= 9999999999; i += 2)
            {
                // PI += Math.Pow(-1.0, i) / (2 * i + 1.0);
                if (0 == ((i - 1) & 3))
                    PI += 4.0 / i;
                else
                    PI -= 4.0 / i;
            }
            e.Result = PI;
        }

        private void UpdateUI(object sender, ProgressChangedEventArgs e)
        {            
            progressBar1.Value = e.ProgressPercentage;
            txtOut.Text = e.ProgressPercentage.ToString();
        }

        private void WorkDone(object sender, RunWorkerCompletedEventArgs e)
        {
            txtOut.Text = e.Result.ToString();
            currentValue = 0;
            lblStatus.Text = @"Done!";
            lblStatus.Visible = true;
            btnStart.Enabled = true;            
        }

        
    }
}
