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
        public MainDialog()
        {
            InitializeComponent();
         
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
            NTPClass ntp1 = new NTPClass();
            ntp1.serverIdentifier = serverIdentifier.Text;
            ntp1.serverName = serverNameBox.Text;
            ntp1.Version_Number = (ushort)(comboBox1.SelectedIndex + 3);
            ntp1.sendNTPpacket();
            UserDefinedGlobalData.globalData.Add(ntp1);
            this.Dispose();
        }

    
    }
}
