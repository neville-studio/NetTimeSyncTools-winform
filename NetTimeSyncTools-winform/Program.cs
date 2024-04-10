using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

struct NTPInformation {
    short Version_Number;
    short Leap_Indicator;
    short Mode;
    short Stradium;
    short Poll;
    short Precision;
    int SynchronizingDistance;
    int SynchronizingDispersion;
    int ReferenceIdentifier;
    int ReferenceClockIdentifier;
    long ReferenceTimeStamp;
    long OriginateTimeStamp;
    long ReceiveTimeStamp;
    long TransmitTimeStamp;
    int KeyIdentifier;
    long CriptoCheckSum;

}
namespace NetTimeSyncTools_winform
{
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
