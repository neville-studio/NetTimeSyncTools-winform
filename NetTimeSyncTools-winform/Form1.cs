using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Net.WebRequestMethods;

namespace NetTimeSyncTools_winform
{
    public partial class Form1 : Form
    {
        static Form1 Forms;
        static bool needUpdate = false;
        public static void updateNotify() {
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
            if(number < 10)
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
                listView1.Items[i].SubItems[3].Text = "" + UserDefinedGlobalData.globalData[i].getCurrentTime();
            };
        }

        private void button1_Click(object sender, EventArgs e)
        {
            MainDialog form = new MainDialog();
            form.ShowDialog(this);
            
        }

        private void Form1_Focus(object sender, EventArgs e)
        {
            Update_List();
            
        }
        private void Update_List() 
        {
            listView1.Items.Clear();
            for (int i = 0; i < UserDefinedGlobalData.globalData.Count; i++)
            {
                ListViewItem item = new ListViewItem();
                item.Text = UserDefinedGlobalData.globalData[i].serverIdentifier;
                item.SubItems.Add(UserDefinedGlobalData.globalData[i].serverName);
                item.SubItems.Add(UserDefinedGlobalData.globalData[i].GetIP());
                item.SubItems.Add("" + UserDefinedGlobalData.globalData[i].getCurrentTime());
                item.SubItems.Add("" + UserDefinedGlobalData.globalData[i].getStatus());
                item.SubItems.Add("" + UserDefinedGlobalData.globalData[i].getOffset() + "ms");
                listView1.Items.Add(item);
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
            
            if (listView1.SelectedItems.Count == 1) {
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
            if(listView1.SelectedItems.Count == 1)
                Index = listView1.SelectedItems[0].Index;
            if (Index > -1 && MessageBox.Show("确认删除此项目吗？","删除时间服务器",MessageBoxButtons.OKCancel) ==DialogResult.OK)
            {
                //int Index = listView1.SelectedItems[0].Index;
                UserDefinedGlobalData.globalData.RemoveAt(Index);
                Update_List();
            }
        }
    }
    
}
