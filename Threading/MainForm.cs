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
    public partial class MainForm : Form
    {
        const int MaxValue = 100;
        int currentValue = 0;
        Thread workerThread;
        delegate void WorkerThreadDelegate(int i);
        public MainForm()
        {
            InitializeComponent();
            this.progressBar1.Maximum = MaxValue;
            this.progressBar1.Value = 0;
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            workerThread.Abort();
            this.progressBar1.Value = 1;
            this.txtOut.Text = "";
            currentValue = 0;
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            workerThread = new Thread(WorkerThreadMain);
            workerThread.IsBackground = true;
            workerThread.Start();
        }

        private void WorkerThreadMain()
        {
            WorkerThreadDelegate d = new WorkerThreadDelegate(UpdateUI);
            for (; currentValue < MaxValue; ++currentValue)
            {
                this.Invoke(d, currentValue);
                Thread.Sleep(1000);
            }
        }

        void UpdateUI(int progress)
        {
            txtOut.Text = progress.ToString();
            this.progressBar1.Value = progress;
        }

        private void btnBgWorker_Click(object sender, EventArgs e)
        {
            BgWorker bgWorker = new BgWorker();
            bgWorker.Show();
        }

        
    }
}
