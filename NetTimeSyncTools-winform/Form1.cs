using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NetTimeSyncTools_winform
{

   

        public partial class Form1 : Form
        {
        

        public Form1()
        {
            
            DoubleBuffered = true;
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
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
            UserDefinedGlobalData.globalData.ForEach((ntp) =>
            {

                listView1.FindItemWithText(ntp.serverIdentifier).SubItems[3].Text = "" + ntp.ReceiveTimeStamp;
            });
        }

        private void button1_Click(object sender, EventArgs e)
        {
            MainDialog form = new MainDialog();
            form.ShowDialog(this);
            
        }

        private void Form1_Focus(object sender, EventArgs e)
        {
            listView1.Items.Clear();
            UserDefinedGlobalData.globalData.ForEach((ntp) =>
            {
                ListViewItem item = new ListViewItem();
                item.Text = ntp.serverIdentifier;
                item.SubItems.Add(ntp.serverName);
                item.SubItems.Add(ntp.GetIP());
                item.SubItems.Add("" + ntp.ReceiveTimeStamp);
                listView1.Items.Add(item);
            });
        }
    }
    
}
