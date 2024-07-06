using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

struct NTPInformation {


}
namespace NetTimeSyncTools_winform
{
    public static class UserDefinedGlobalData {
        public static List<NTPClass> globalData = new List<NTPClass>();
    }
    
    internal static class Program
    {
        
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
          
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(true);
            Application.Run(new Form1());
        }


    }
}
