using System;
using System.Reflection;
using System.Windows.Forms;

namespace NetTimeSyncTools_winform
{
    public partial class AboutDialog : Form
    {
        public AboutDialog()
        {
            InitializeComponent();
        }

        private void AboutDialog_Load(object sender, EventArgs e)
        {
            Assembly asm = Assembly.GetExecutingAssembly();//如果是当前程序集

            AssemblyCopyrightAttribute asmcpr = (AssemblyCopyrightAttribute)Attribute.GetCustomAttribute(asm, typeof(AssemblyCopyrightAttribute));


            CopyRightLabel.Text = asmcpr.Copyright;
            VersionLabel.Text = "" + Application.ProductName + ", V" + Application.ProductVersion;
        }

        private void ViewProject_Click(object sender, EventArgs e)
        {
            string target = "https://github.com/neville-studio/NetTimeSyncTools-winform";
            //Use no more than one assignment when you test this code.
            //string target = "ftp://ftp.microsoft.com";
            //string target = "C:\\Program Files\\Microsoft Visual Studio\\INSTALL.HTM";
            try
            {
                System.Diagnostics.Process.Start(target);
            }
            catch (System.ComponentModel.Win32Exception noBrowser)
            {
                if (noBrowser.ErrorCode == -2147467259)
                    MessageBox.Show(noBrowser.Message);
            }
            catch (System.Exception other)
            {
                MessageBox.Show(other.Message);
            }
        }
    }
}
