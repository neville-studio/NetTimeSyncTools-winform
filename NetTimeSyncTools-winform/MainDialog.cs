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
    public partial class MainDialog : Form
    {
        int index = 0;
        string Method = "Add";
        NTPClass ntpClass = null;
        public MainDialog()
        {
            InitializeComponent();
         
        }
        public MainDialog(int Index,NTPClass ntpClass)
        { 
            this.index = Index;
            this.ntpClass = ntpClass;
            
            Method = "Edit";
            InitializeComponent();
            ImeMode = ImeMode.OnHalf;
        }
        private void MainDialog_Load(object sender, EventArgs e)
        {
            serverIdentifier.ImeMode = ImeMode.OnHalf;
            serverNameBox.ImeMode = ImeMode.OnHalf;
            if (Method == "Edit") {
                serverIdentifier.Text = ntpClass.serverIdentifier;
               
                serverNameBox.Text = ntpClass.serverName;
                comboBox1.SelectedIndex = ntpClass.Version_Number - 3;
            }
        }
        private void button2_Click(object sender, EventArgs e)
        {
            //this.Close();
            this.Dispose();
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (Method == "Add")
            {
                NTPClass ntp1 = new NTPClass();
                ntp1.serverIdentifier = serverIdentifier.Text;
                ntp1.serverName = serverNameBox.Text;
                ntp1.Version_Number = (ushort)(comboBox1.SelectedIndex + 3);
                ntp1.sendNTPpacket();
                UserDefinedGlobalData.globalData.Add(ntp1);
                this.Dispose();
            }
            else
            {
                ntpClass.serverIdentifier = serverIdentifier.Text; 
                ntpClass.serverName = serverNameBox.Text;
                ntpClass.Version_Number = (ushort)(comboBox1.SelectedIndex + 3);
                ntpClass.sendNTPpacket();
                UserDefinedGlobalData.globalData[index] = ntpClass;
                this.Dispose();
            }
           
        }
    }
}
