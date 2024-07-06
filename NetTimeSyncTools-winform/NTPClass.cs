using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Collections;
using System.Threading;
using static System.Net.WebRequestMethods;

namespace NetTimeSyncTools_winform
{
    static class Constraint {
        public const int NtpDefaultPort = 123;
    }
    public struct NTPExtensionField {
        int FieldType;
        uint Length;
        int Value;
        int Padding;
    }
    public class NTPClass
    {
        public ushort Version_Number = 4;                // The maximum Version is 4.
        public ushort Leap_Indicator = 0;
        public string serverName = "";
        public string serverIdentifier = "";
        public short status = 0;             // status = 0: unknown. status = 1 : Sending; status = 2: success. status = 3: error.
        public NTPExtensionField[] extensions = new NTPExtensionField[2];
        public uint Root_Delay = 0;
        public int error = 0;
        public ushort Mode = 3;
        public short Stradium = 0;
        public short Poll = 0;
        public short Precision = 0;
        public int ReferenceIdentifier = 0;
        public long ReferenceTimeStamp = 0;
        public long OriginateTimeStamp = 0;
        public long ReceiveTimeStamp = 0;
        public long TransmitTimeStamp = 0;
        public int KeyIdentifier = 0;
        public long CriptoCheckSum = 0;
        public long EndTime = 0;

        long utcTimeStamp2NTPTimeStamp(long timestamp) {
            DateTime utcTime = DateTime.UtcNow;
            DateTime baseTime = new DateTime(1900, 1, 1, 0, 0, 0, DateTimeKind.Utc);

            long diff = utcTime.Ticks - baseTime.Ticks;
            long result = ((diff / 10000000) << 32) + (diff % 10000000) * 10000 ;
            return result;

        }

        public NTPClass() {
            DateTime currentDate = DateTime.Now;
            TransmitTimeStamp = utcTimeStamp2NTPTimeStamp(currentDate.Ticks);
            
        }

        void handle_NTPThread()
        {
            try
            {
                Socket s = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                s.Connect(serverName, 123);
                status = 1;
                byte[] sendbuf = NTPv4();
                s.Send(sendbuf);
                byte[] receive = new byte[48];
                s.Receive(receive);
                EndTime = Environment.TickCount;
                Transmit(receive);
                s.Close();
                Console.WriteLine("We have sent the message.");
                status = 2;
            }
            catch (Exception e) {
                status = 3;
                error = e.HResult;
                Console.WriteLine(e.Message);
            }
        }

        public void sendNTPpacket() {
            //NTPListener uDPListener = new NTPListener(this);
            //uDPListener.Main();
            Thread thread = new Thread(handle_NTPThread);
            thread.Start();
        }
        byte[] NTPv4() {

            byte[] buf = new byte[48];
            buf[0] = (byte)(Leap_Indicator<< 6 | Version_Number<< 3 | Mode);
            buf[1] = (byte)Stradium;
            buf[2] = (byte)Poll;
            buf[3] = (byte)Precision;
            buf[4] = (byte)(Root_Delay >> 24 &0xff);
            buf[5] = (byte)(Root_Delay >> 16 & 0xff);
            buf[6] = (byte)(Root_Delay >> 8 & 0xff);
            buf[7] = (byte)(Root_Delay & 0xff);
            buf[40] = (byte)(TransmitTimeStamp >> 56 & 0xff);
            buf[41] = (byte)(TransmitTimeStamp >> 48 & 0xff);
            buf[42] = (byte)(TransmitTimeStamp >> 40 & 0xff);
            buf[43] = (byte)(TransmitTimeStamp >> 32 & 0xff);
            buf[44] = (byte)(TransmitTimeStamp >> 24 & 0xff);
            buf[45] = (byte)(TransmitTimeStamp >> 16 & 0xff);
            buf[46] = (byte)(TransmitTimeStamp >> 8 & 0xff);
            buf[47] = (byte)(TransmitTimeStamp & 0xff);
            return buf.ToArray();
        }
        void Transmit(byte[] packet)
        {
            Version_Number = (ushort)(packet[0] >> 3 & 0x07);
            Leap_Indicator = (ushort)(packet[0] >> 6 & 0x03);
            Mode = (ushort)(packet[0] & 0x07);
            Stradium = packet[1];
            Poll = packet[2];
            Precision = packet[3];
            Root_Delay = (uint)(packet[4] << 24 | packet[5] << 16 | packet[6] << 8 | packet[7]);
            ReferenceIdentifier = (int)(packet[12] << 24 | packet[13] << 16 | packet[14] << 8 | packet[15]);
            ReferenceTimeStamp = (long)(packet[16] << 56 | packet[17] << 48 | packet[18] << 40 | packet[19] << 32 | packet[20] << 24 | packet[21] << 16 | packet[22] << 8 | packet[23]);
            OriginateTimeStamp = (long)(packet[24] << 56 | packet[25] << 48 | packet[26] << 40 | packet[27] << 32 | packet[28] << 24 | packet[29] << 16 | packet[30] << 8 | packet[31]);
            ReceiveTimeStamp = (long)(packet[32] << 56 | packet[33] << 48 | packet[34] << 40 | packet[35] << 32 | packet[36] << 24 | packet[37] << 16 | packet[38] << 8 | packet[39]);
            TransmitTimeStamp = packet[40] << 56 | packet[41] << 48 | packet[42] << 40 | packet[43] << 32 | packet[44] << 24 | packet[45] << 16 | packet[46] << 8 | packet[47];

        }
        public string GetIP() {
            int ip = ReferenceIdentifier;
            return $"{ip >> 24 & 0xff}.{ip >> 16 & 0xff}.{ip >> 8 & 0xff}.{ip & 0xff}";
        }
    }
    
}
