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
        }
    }
    
}
