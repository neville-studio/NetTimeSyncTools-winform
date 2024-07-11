using System;
using System.Collections.Generic;
using System.Reflection;
using System.Windows.Forms;

namespace NetTimeSyncTools_winform
{

    public partial class Form1 : Form
    {
        static Form1 Forms;
        static bool needUpdate = false;
        public static void updateNotify()
        {
            needUpdate = true;
        }
        public Form1()
        {
            Forms = this;
            DoubleBuffered = true;
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            InitializeComponent();
        }

        private string plusZero(int number)
        {
            if (number < 10)
            {
                return $"0{number}";
            }
            return $"{number}";
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            DateTime dateTime = DateTime.Now;
            localtimes.Text = dateTime.ToString();
            int startedTime = Environment.TickCount;
            startTime.Text = $"{startedTime / 3600000}:{plusZero(startedTime / 60000 % 60)}:{plusZero(startedTime / 1000 % 60)}";
            if (needUpdate)
            {
                Update_List();
                needUpdate = false;
            }
            for (int i = 0; i < UserDefinedGlobalData.globalData.Count; i++)
            {
                if (UserDefinedGlobalData.globalData[i].status == 2)
                listView1.Items[i].SubItems[3].Text = "" + UserDefinedGlobalData.globalData[i].getCurrentTime();
            };
        }

        private void button1_Click(object sender, EventArgs e)
        {
            MainDialog form = new MainDialog();
            form.ShowDialog(this);

        }
        private void updateStatus(long fc, long avg) {
            int lcid = System.Globalization.CultureInfo.CurrentCulture.LCID;
            if (fc > 5000)
            {
                toolStripStatusLabel1.Text = FormConstraintDate.STATUS_TOO_LARGE_DIFFRENCE_BETWEEN_SERVERS;
                if (lcid == 2052)
                {
                    toolStripStatusLabel1.Text = FormConstraintDate.STATUS_TOO_LARGE_DIFFRENCE_BETWEEN_SERVERS_2052;
                }
                else if (lcid == 1033)
                {
                    toolStripStatusLabel1.Text = FormConstraintDate.STATUS_TOO_LARGE_DIFFRENCE_BETWEEN_SERVERS_1033;
                }
            }
            else if (avg > 10000)
            {
                toolStripStatusLabel1.Text = FormConstraintDate.STATUS_NEED_UPDATE;
                if (lcid == 2052)
                {
                    toolStripStatusLabel1.Text = FormConstraintDate.STATUS_NEED_UPDATE_2052;
                }
                else if (lcid == 1033)
                {
                    toolStripStatusLabel1.Text = FormConstraintDate.STATUS_NEED_UPDATE_1033;
                }
            }
            else
            {
                toolStripStatusLabel1.Text = FormConstraintDate.STATUS_READY;
                if (lcid == 2052)
                {
                    toolStripStatusLabel1.Text = FormConstraintDate.STATUS_READY_2052;
                }
                else if (lcid == 1033)
                {
                    toolStripStatusLabel1.Text = FormConstraintDate.STATUS_READY_1033;
                }
            }
        }
        private void Form1_Focus(object sender, EventArgs e)
        {
            Update_List();

        }
        private void Update_List()
        {
            listView1.Items.Clear();
            long sum = 0;
            List<long> offsetList = new List<long>();
            for (int i = 0; i < UserDefinedGlobalData.globalData.Count; i++)
            {
               
                ListViewItem item = new ListViewItem();
                item.Text = UserDefinedGlobalData.globalData[i].serverIdentifier;
                item.SubItems.Add(UserDefinedGlobalData.globalData[i].serverName);
                item.SubItems.Add(UserDefinedGlobalData.globalData[i].GetIP());
                item.SubItems.Add("" + UserDefinedGlobalData.globalData[i].getCurrentTime());
                item.SubItems.Add("" + UserDefinedGlobalData.globalData[i].getStatus());
                if (UserDefinedGlobalData.globalData[i].status == 2)
                {
                    int offset = UserDefinedGlobalData.globalData[i].getOffset();
                    offsetList.Add(offset);
                    item.SubItems.Add("" + offset + "ms");
                    sum += offset;
                }
                else {
                    item.SubItems.Add("");
                }
                listView1.Items.Add(item);
            }
            if (offsetList.Count > 0)
            {
                long avg = sum / offsetList.Count;
                long fc = 0;
                foreach (int i in offsetList)
                {
                    fc += (i - avg) * (i - avg);
                }
                fc = fc / offsetList.Count;
                updateStatus(fc, avg);
                

            }
            

            
        }
        private void button4_Click(object sender, EventArgs e)
        {
            foreach (var item in UserDefinedGlobalData.globalData)
            {
                item.sendNTPpacket();
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {

            if (listView1.SelectedItems.Count == 1)
            {
                int Index = listView1.SelectedItems[0].Index;
                MainDialog mainDialog = new MainDialog(Index, UserDefinedGlobalData.globalData[Index]);
                mainDialog.ShowDialog(this);
            }
        }
        private void item_dblCLcked(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count == 1)
            {
                int Index = listView1.SelectedItems[0].Index;
                MainDialog mainDialog = new MainDialog(Index, UserDefinedGlobalData.globalData[Index]);
                mainDialog.ShowDialog(this);
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            int Index = -1;
            if (listView1.SelectedItems.Count == 1)
                Index = listView1.SelectedItems[0].Index;
            if (Index > -1 && MessageBox.Show("确认删除此项目吗？", "删除时间服务器", MessageBoxButtons.OKCancel) == DialogResult.OK)
            {
                //int Index = listView1.SelectedItems[0].Index;
                UserDefinedGlobalData.globalData.RemoveAt(Index);
                Update_List();
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Assembly asm = Assembly.GetExecutingAssembly();//如果是当前程序集
            
            AssemblyCopyrightAttribute asmcpr = (AssemblyCopyrightAttribute)Attribute.GetCustomAttribute(asm, typeof(AssemblyCopyrightAttribute));
           

            CopyrightLabel.Text = asmcpr.Copyright;
        }

        private void CopyrightLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            AboutDialog aboutDialog = new AboutDialog();
            aboutDialog.ShowDialog(this);
        }
    }
    class FormConstraintDate
    {
        public const string STATUS_TOO_LARGE_DIFFRENCE_BETWEEN_SERVERS_1033 = "The time differences between servers is too large";
        public const string STATUS_TOO_LARGE_DIFFRENCE_BETWEEN_SERVERS_2052 = "服务器上的时钟差过大！";
        public const string STATUS_TOO_LARGE_DIFFRENCE_BETWEEN_SERVERS = STATUS_TOO_LARGE_DIFFRENCE_BETWEEN_SERVERS_2052;
        public const string STATUS_NEED_UPDATE_1033 = "The time in your machine need to update";
        public const string STATUS_NEED_UPDATE_2052 = "您的机器时间需要更新";
        public const string STATUS_NEED_UPDATE = STATUS_NEED_UPDATE_2052;
        public const string STATUS_READY_2052 = "准备就绪";
        public const string STATUS_READY_1033 = "Ready";
        public const string STATUS_READY = STATUS_READY_2052;
    }
}
