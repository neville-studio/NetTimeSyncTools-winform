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
using System.Text.Json;
using static System.Net.WebRequestMethods;
using System.Text.Json.Serialization;
using System.Text.Json.Nodes;

namespace NetTimeSyncTools_winform
{
    static class NTPConstraint {
        public const int NtpDefaultPort = 123;
        public const long NtpEpochMillSeconds = 0x100000000 * 1000;
        public const long NtpUtcBaseDiff = 2208988800;
        const string refidJSONString = "{\"LOCL\": \"Undisciplined Local Clock\",\"GOES\": \"Geosynchronous Orbit Environment Satellite\",\"GPS\": \"Global Position System\",\"GAL\": \"Galileo Positioning System\",\"PPS\": \"Generic pulse-per-second\",\"IRIG\": \"Inter-Range Instrumentation Group\",\"WWVB\": \"LF Radio WWVB Ft. Collins, CO 60 kHz\",\"DCF\": \"LF Radio DCF77 Mainflingen, DE 77.5 kHz\",\"HBG\": \"LF Radio HBG Prangins, HB 75 kHz\",\"MSF\": \"LF Radio MSF Anthorn, UK 60 kHz\",\"JJY\": \"LF Radio JJY Fukushima, JP 40 kHz, Saga, JP 60 kHz\",\"LORC\": \"MF Radio LORAN C station, 100 kHz\",\"TDF\": \"MF Radio Allouis, FR 162 kHz\",\"CHU\": \"HF Radio CHU Ottawa, Ontario\",\"WWV\": \"HF Radio WWV Ft. Collins, CO\",\"WWVH\": \"HF Radio WWVH Kauai, HI\",\"NIST\": \"NIST telephone modem\",\"ACTS\": \"NIST telephone modem\",\"USNO\": \"USNO telephone modem\",\"PTB\": \"European telephone modem\",\"BDS\": \"Beidou Navigation Satellite System\",\"PTP\": \"Precession Time Protocal\",\"MRS\":\"Manual Reference System\",\"DFM\":\"UTC(DFM)\"}";
        public static Dictionary<string,string> refid = JsonDocument.Parse(refidJSONString).RootElement.EnumerateObject().ToDictionary(x => x.Name, x => x.Value.GetString());
        const string kissodeathcodeJSONString = "{\"ACST\":\"The association belongs to a unicast server\",\"AUTH\":\"Server authentication failed\",\"AUTO\":\"Autokey sequence failed\",\"BCST\":\"The association belongs to a broadcast server\",\"CRYP\":\"Cryptographic authentication or identification failed\",\"DENY\":\"Access denied by remote server\",\"DROP\":\"Lost peer in symmetric mode\",\"RSTR\":\"Access denied due to local policy\",\"INIT\":\"The association has not yet synchronized for the first time\",\"MCST\":\"The association belongs to a dynamically discovered server\",\"NKEY\":\"No key found. Either the key was never installed or is not trusted\",\"NTSN\":\"Network Time Security (NTS) negative-acknowledgment (NAK)\",\"RATE\":\"Rate exceeded. The server has temporarily denied access because the client exceeded the rate threshold\",\"RMOT\":\"Alteration of association from a remote host running ntpdc.\",\"STEP\":\"A step change in system time has occurred, but the association has not yet resynchronized\"}";
        public static Dictionary<string,string> kissocode = JsonDocument.Parse(refidJSONString).RootElement.EnumerateObject().ToDictionary(x => x.Name, x => x.Value.GetString());
    }
    public struct NTPExtensionField {
        int FieldType;
        uint Length;
        int Value;
        int Padding;
    }
    class NoTimeFoundException : Exception {
        public NoTimeFoundException() : base("No time found.") { }
    }
    public class NTPClass
    {
        public ushort Version_Number = 4;                // The maximum Version is 4.
        public ushort Leap_Indicator = 0;
        public int epoch = 0;
        public string serverName = "";
        public string serverIdentifier = "";
        public short status = 0;             // status = 0: unknown. status = 1 : Sending; status = 2: success. status = 3: error.
        public NTPExtensionField[] extensions = new NTPExtensionField[2];
        public uint Root_Delay = 0;
        int error = 0;
        string errorInfo = "";
        public ushort Mode = 3;
        public short Stradium = 0;
        public short Poll = 0;
        public short Precision = 0;
        public int ReferenceIdentifier = 0;
        long ReferenceTimeStamp = 0;
        long OriginateTimeStamp = 0;
        long ReceiveTimeStamp = 0;
        long TransmitTimeStamp = 0;
        public int KeyIdentifier = 0;
        public long CriptoCheckSum = 0;
        public long StartTime = 0;
        public long EndTime = 0;
        short IPVersion = 4;
        int offset = 0;
        public string getStatus() {
            switch (status) {
                case 0:
                    return "Unknown";
                case 1:
                    return "Waiting...";
                case 2:
                    return "Normal";
                case 3:
                    return "Failed:0x" + error.ToString("x") + "("+errorInfo+")";
                default:
                    return "Unknown";
            }
        }
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
        public DateTime getCurrentTimeStamp()
        {
            if (status != 2) throw new NoTimeFoundException();
            //1. Calculate the time difference between the current time and the time when the NTP packet was sent.

            ulong t1 = 0;
            t1 = ((ulong)ReceiveTimeStamp >> 32) * 10000000 + (((ulong)ReceiveTimeStamp & 0xffffffff) * 232) / 100000 - NTPConstraint.NtpUtcBaseDiff * 10000000;
            if (ReceiveTimeStamp >> 63 == 0)
            {
                t1 += NTPConstraint.NtpEpochMillSeconds;
            }
            ulong t2 = 0;
            t2 = ((ulong)TransmitTimeStamp >> 32) * 10000000 + (((ulong)TransmitTimeStamp & 0xffffffff) * 232) / 100000 - NTPConstraint.NtpUtcBaseDiff * 10000000;
            if (TransmitTimeStamp >> 63 == 0)
            {
                t2 += NTPConstraint.NtpEpochMillSeconds;
            }
            long curTimeStamp = (long)(((ulong)EndTime + t2 + t1 - (ulong)StartTime) / 2 + (ulong)((Environment.TickCount - EndTime) * 10000));
            DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1)); // 当地时区
            DateTime dt = startTime.AddTicks(curTimeStamp);
            return dt;
        }
        public string getCurrentTime() {

            //DateTime currentDate = new DateTime((long)(((ulong)EndTime + t2 + t1 - (ulong)StartTime)/2+(ulong)((Environment.TickCount-EndTime)*10000)),DateTimeKind.Utc);
            //currentDate = currentDate.AddYears(1970);
            try
            {
                DateTime dt = getCurrentTimeStamp();
                return dt.ToLocalTime().ToString();
            }
            catch (NoTimeFoundException e) {
                return "";
            }
        }
        bool send(Socket s) {
            try
            {

                s.ReceiveTimeout = 5000;
                s.Connect(serverName, NTPConstraint.NtpDefaultPort);
                status = 1;
                byte[] sendbuf = NTPv4();
                StartTime = Environment.TickCount;
                s.Send(sendbuf);
                byte[] receive = new byte[48];
                s.Receive(receive);
                EndTime = Environment.TickCount;
                Transmit(receive);
                s.Close();
                Console.WriteLine("We have sent the message.");
                status = 2;
                IPVersion = s.AddressFamily == AddressFamily.InterNetwork ? (short)4 : (short)6;
                offset = calcOffset();
                return true;
            }
            catch (System.ArgumentException e)
            {
                Console.WriteLine(e.Message);
                s.Close();
                //error = e.HResult;
                //errorInfo = e.Message;
                return false;
            }
            catch (Exception e)
            {
                status = 3;
                error = e.HResult;
                errorInfo = e.Message;
                Console.WriteLine(e.Message);
                s.Close();
                return false;
            }
            finally
            {
                s.Close();
            }
        }
        void handle_NTPThread(object param)
        {
            Socket sV4 = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            Socket sV6 = new Socket(AddressFamily.InterNetworkV6, SocketType.Dgram, ProtocolType.Udp);
            status = 1;
            if (!send(sV4) && !send(sV6))
            { 
                status = 3;
            }
            Form1.updateNotify();

            
        }
        public int getOffset() {
            return offset;
        }
        int calcOffset()
        {
            DateTime dt1 = DateTime.Now;
            try
            {
                 dt1= getCurrentTimeStamp();
            }catch (NoTimeFoundException e)
            {
                return 0;
            }
            DateTime dt2 = new DateTime(DateTime.Now.Ticks,DateTimeKind.Utc);
            return (int)(dt1.Ticks - dt2.Ticks) / 10000;            
        }
        public void sendNTPpacket() {
            //NTPListener uDPListener = new NTPListener(this);
            //uDPListener.Main();
            
            ThreadPool.QueueUserWorkItem(handle_NTPThread);
            
            //Thread thread = new Thread(handle_NTPThread);
            //thread.Start((object)s);
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
            //Mode = (ushort)(packet[0] & 0x07);
            Stradium = packet[1];
            Poll = packet[2];
            Precision = packet[3];
            Root_Delay = (uint)(packet[4] << 24 | packet[5] << 16 | packet[6] << 8 | packet[7]);
            ReferenceIdentifier = (int)(packet[12] << 24 | packet[13] << 16 | packet[14] << 8 | packet[15]);
            ReferenceTimeStamp = (long)(packet[16]) << 56 | (long)(packet[17]) << 48 | (long)(packet[18]) << 40 | (long)(packet[19]) << 32 | (long)(packet[20]) << 24 | (long)(packet[21] << 16) | (long)(packet[22] << 8) | (long)packet[23];
            OriginateTimeStamp = (long)(packet[24]) << 56 | (long)(packet[25]) << 48 | (long)(packet[26]) << 40 | (long)(packet[27]) << 32 | (long)(packet[28]) << 24 | (long)(packet[29]) << 16 | (long)(packet[30]) << 8 | (long)packet[31];
            ReceiveTimeStamp = (long)(packet[32]) << 56 | (long)(packet[33]) << 48 | (long)(packet[34]) << 40 | (long)(packet[35]) << 32 | (long)(packet[36]) << 24 | (long)(packet[37] << 16) | (long)(packet[38] << 8) | (long)packet[39];
            TransmitTimeStamp = (long)(packet[40] ) << 56 | (long)(packet[41] ) << 48 | (long)(packet[42] ) << 40 | (long)(packet[43] ) << 32 | (long)(packet[44] ) << 24 | (long)(packet[45] << 16) | (long)(packet[46] << 8) | (long)packet[47];
        }
        public string GetIP() {
            if (status != 2)return "";
            int ip = ReferenceIdentifier;
            string clockSource = "";
            if (Stradium == 1)
            {
                clockSource = "" + (char)(ip >> 24) + (char)((ip >> 16) & 0xff) + (char)((ip >> 8) & 0xff) + (char)(ip & 0xff);
                clockSource = clockSource.Trim().Replace("\0", "");
                string outStr = "";
                if (NTPConstraint.refid.TryGetValue(clockSource, out outStr))
                {
                    return clockSource + "(" + outStr + ")";
                }
                return clockSource;
            }
            else if (Stradium == 0)
            {
                string kissocode = "";
                kissocode = "" + (char)(ip >> 24) + (char)((ip >> 16) & 0xff) + (char)((ip >> 8) & 0xff) + (char)(ip & 0xff);
                kissocode = clockSource.Trim().Replace("\0", "");
                string outStr = "";
                if (NTPConstraint.kissocode.TryGetValue(kissocode, out outStr))
                {
                    return kissocode + "(" + outStr + ")";
                }
                return kissocode;
            }
            if(IPVersion == 4)
                return $"{ip >> 24 & 0xff}.{ip >> 16 & 0xff}.{ip >> 8 & 0xff}.{ip & 0xff}";
            else
                return $"{(ip >> 16 & 0xffff).ToString("x")}:{(ip & 0xffff).ToString("x")}";
        }
    }
    
}
