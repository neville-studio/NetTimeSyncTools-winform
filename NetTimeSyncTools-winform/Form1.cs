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
        

        public Form1()
        {
            
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
            for (int i = 0; i < UserDefinedGlobalData.globalData.Count; i++)
            {
                listView1.Items[i].SubItems[4].Text = "" + UserDefinedGlobalData.globalData[i].getStatus();
                listView1.Items[i].SubItems[2].Text = "" + UserDefinedGlobalData.globalData[i].GetIP();
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
            listView1.Items.Clear();
            for (int i = 0; i < UserDefinedGlobalData.globalData.Count; i++)
            {
                ListViewItem item = new ListViewItem();
                item.Text = UserDefinedGlobalData.globalData[i].serverIdentifier;
                item.SubItems.Add(UserDefinedGlobalData.globalData[i].serverName);
                item.SubItems.Add(UserDefinedGlobalData.globalData[i].GetIP());
                item.SubItems.Add("" + UserDefinedGlobalData.globalData[i].getCurrentTime());
                item.SubItems.Add("" + UserDefinedGlobalData.globalData[i].getStatus());
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
    }
    
}
