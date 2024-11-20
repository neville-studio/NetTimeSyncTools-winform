using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace NetTimeSyncTools_winform
{

    public partial class Form1 : Form
    {
        static Form1 Forms;
        static bool needUpdate = false;
        public static void updateNotify(int update = 0)
        {
            if (update == 1)
            {
                foreach (var item in UserDefinedGlobalData.globalData)
                {
                    item.sendNTPpacket();
                }
            }
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
        private void updateStatus(long fc, long avg)
        {
            int lcid = System.Globalization.CultureInfo.CurrentCulture.LCID;
            if (fc > 5000)
            {
                toolStripStatusLabel1.Text = UserDefinedGlobalData.resourceManager.GetString("String_DiffBetweenServersTooLarge");
            }
            else if (avg > 10000)
            {
                toolStripStatusLabel1.Text = UserDefinedGlobalData.resourceManager.GetString("String_NeedUpdate");
            }
            else
            {
                toolStripStatusLabel1.Text = UserDefinedGlobalData.resourceManager.GetString("String_GetReady");
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
                else
                {
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
            string deleteConfirm = UserDefinedGlobalData.resourceManager.GetString("String_DeleteConfirm");
            string deleteConfirmCaption = UserDefinedGlobalData.resourceManager.GetString("String_DeleteConfirmCaption");
            if (Index > -1 && MessageBox.Show(deleteConfirm, deleteConfirmCaption, MessageBoxButtons.OKCancel) == DialogResult.OK)
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
            if (UserDefinedGlobalData.globalData.Count > 0) Update_List();
            UpdateUI();
        }

        private void CopyrightLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            AboutDialog aboutDialog = new AboutDialog();
            aboutDialog.ShowDialog(this);
        }

        private void button5_Click(object sender, EventArgs e)
        {

            if (listView1.SelectedItems.Count == 1 && listView1.SelectedItems[0].Index > -1)
            {
                SYSTEMTIME sYSTEMTIME = new SYSTEMTIME();
                sYSTEMTIME.FromDateTime(UserDefinedGlobalData.globalData[listView1.SelectedItems[0].Index].getCurrentTimeStamp());
                bool successed = Win32API.SetLocalTime(ref sYSTEMTIME);
                if (successed)
                {
                    string message = UserDefinedGlobalData.resourceManager.GetString("String_SetSuccess");
                    MessageBox.Show(message, message, MessageBoxButtons.OK);
                }
                else
                {
                    string message = UserDefinedGlobalData.resourceManager.GetString("String_SetFailed");
                    MessageBox.Show(message, message, MessageBoxButtons.OK);
                }
            }

        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            UserDefinedGlobalData.WriteGlobalNTPGlobalDataToDB();
        }
        void UpdateUI()
        {
            if (listView1.SelectedItems.Count == 1 && listView1.SelectedItems[0].Index > -1)
            {
                button2.Enabled = true;
                button3.Enabled = true;
                button5.Enabled = true;
            }
            else
            {
                button2.Enabled = false;
                button3.Enabled = false;
                button5.Enabled = false;
            }
        }
        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateUI();
        }
    }

    public struct SYSTEMTIME
    {
        public ushort wYear;
        public ushort wMonth;
        public ushort wDayOfWeek;
        public ushort wDay;
        public ushort wHour;
        public ushort wMinute;
        public ushort wSecond;
        public ushort wMilliseconds;

        /// <summary>
        /// 从System.DateTime转换。
        /// </summary>
        /// <param name="time">System.DateTime类型的时间。</param>
        public void FromDateTime(DateTime time)
        {
            wYear = (ushort)time.Year;
            wMonth = (ushort)time.Month;
            wDayOfWeek = (ushort)time.DayOfWeek;
            wDay = (ushort)time.Day;
            wHour = (ushort)time.Hour;
            wMinute = (ushort)time.Minute;
            wSecond = (ushort)time.Second;
            wMilliseconds = (ushort)time.Millisecond;
        }
        /// <summary>
        /// 转换为System.DateTime类型。
        /// </summary>
        /// <returns></returns>
        public DateTime ToDateTime()
        {
            return new DateTime(wYear, wMonth, wDay, wHour, wMinute, wSecond, wMilliseconds);
        }
        /// <summary>
        /// 静态方法。转换为System.DateTime类型。
        /// </summary>
        /// <param name="time">SYSTEMTIME类型的时间。</param>
        /// <returns></returns>
        public static DateTime ToDateTime(SYSTEMTIME time)
        {
            return time.ToDateTime();
        }
    }
    public class Win32API
    {
        [DllImport("Kernel32.dll")]
        public static extern bool SetLocalTime(ref SYSTEMTIME Time);
        [DllImport("Kernel32.dll")]
        public static extern void GetLocalTime(ref SYSTEMTIME Time);
    }
}
