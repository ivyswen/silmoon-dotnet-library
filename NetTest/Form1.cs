using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Silmoon.Net;
using System.Net;
using Silmoon.Memory;
using System.Collections;
using Silmoon;
using System.Threading;
using Silmoon.Threading;

namespace NetTest
{
    public partial class Form1 : Silmoon.Windows.Forms.ScrollForm
    {
        ArrayList arraylist = new ArrayList();
        System.Windows.Forms.Timer tmr = new System.Windows.Forms.Timer();
        ActionLimit tl = null;
        public Form1()
        {
            CheckForIllegalCrossThreadCalls = false;
            InitializeComponent();
            tmr.Interval = 1000;
            tmr.Tick += new EventHandler(tmr_Tick);
            tmr.Start();
        }

        void tmr_Tick(object sender, EventArgs e)
        {
            label1.Text = DateTime.Now.ToString();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            checkBox1.Checked = false;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Threads.ExecAsync(t);
        }
        void t()
        {
            listBox1.Items.Add("thread " + Thread.CurrentThread.ManagedThreadId + " start." + DateTime.Now);
            lock (arraylist)
            {
                arraylist.Add(DateTime.Now.ToString());
                Thread.Sleep(3000);
            }
            listBox1.Items.Add("thread " + Thread.CurrentThread.ManagedThreadId + " end." + DateTime.Now);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            string[] items1 = new string[] { "1", "2", "3", "4", "5" };
            string[] items2 = new string[] { "1", "3", "4" };
            string[] missedItems = SmString.MissedItems(items1, items2);

        }

        private void button3_Click(object sender, EventArgs e)
        {
            SmTcp tcp = new SmTcp();
            tcp.AsyncStartListen(IPAddress.Any, 8889);
            tcp.OnReceivedData += new TcpReceiveDataEventHander(tcp_OnReceivedData);
        }

        void tcp_OnReceivedData(TcpStruct localTcpInfo, TcpStruct remoteTcpInfo, byte[] data, ITcpReader tcpReader)
        {
            MessageBox.Show(System.Text.Encoding.Default.GetString(data));
        }

        private void ctlDoButton_Click(object sender, EventArgs e)
        {
            if (tl == null)
            {
                tl = new ActionLimit();
                tl.AddTimeLimit(new TimeLimit(new TimeSpan(0, 0, 0, 1), 1));
                tl.AddTimeLimit(new TimeLimit(new TimeSpan(0, 0, 0, 10), 5));
                tl.AddTimeSection(new Silmoon.Types.TimeSection(new DateTime(1990, 1, 1, 16, 0, 0), new TimeSpan(2, 0, 0)));
            }

            if (tl.Pass)
                listBox1.Items.Insert(0, DateTime.Now.ToString());
            else
                listBox1.Items.Insert(0, "NO");
        }

        private void button5_Click(object sender, EventArgs e)
        {
            SetHeightEx(800, true);
        }

        private void button6_Click(object sender, EventArgs e)
        {
            SetHeightEx(300, true);
        }
    }
}