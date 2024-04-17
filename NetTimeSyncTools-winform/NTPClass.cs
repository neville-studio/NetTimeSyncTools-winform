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

namespace NetTimeSyncTools_winform
{
    static class Constraint {
        public const int NtpDefaultPort = 123;
    }
    struct NTPExtensionField {
        int FieldType;
        uint Length;
        int Value;
        int Padding;
    }
    internal class NTPClass
    {
        public ushort Version_Number;                // The maximum Version is 4.
        ushort Leap_Indicator;
        public string serverName;
        short status;             // status = 0: unknown. status = 1 : Sending; status = 2: success. status = 3: error.
        NTPExtensionField[] extensions = new NTPExtensionField[2];
        uint Root_Delay;
        int error;
        ushort Mode = 3;
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
        long TransmitTimeStampV;
        int KeyIdentifier;
        long CriptoCheckSum;
        
        public void sendNTPpacket() {
            Socket s = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            
            IPAddress broadcast = IPAddress.Parse("192.168.1.255");

            
            IPEndPoint ep = new IPEndPoint(broadcast, Constraint.NtpDefaultPort);
            s.Connect(serverName, 123);
            byte[] sendbuf = NTPv4();
            s.SendTo(sendbuf, ep);

            //Console.WriteLine("Message sent to the broadcast address");
        }
        byte[] NTPv4() {
          List<byte> buf = new List<byte>(640);
            buf.Add((byte)(Leap_Indicator<< 6 | Version_Number<< 3 | Mode));
            buf.Add((byte)Stradium);
            buf.Add((byte)Poll);
            buf.Add((byte)Precision);
            buf.Add((byte)(Root_Delay >> 24 &0xff));
            buf.Add((byte)(Root_Delay >> 16 & 0xff));
            buf.Add((byte)(Root_Delay >> 8 & 0xff));
            buf.Add((byte)(Root_Delay & 0xff));
            return buf.ToArray();
        }
    }
    class UDPListener
    {
        private const int listenPort = 123;

        private static void StartListener()
        {
            UdpClient listener = new UdpClient(listenPort);
            IPEndPoint groupEP = new IPEndPoint(IPAddress.Any, listenPort);

            try
            {
                while (true)
                {
                    Debug.WriteLine("Waiting for broadcast");
                    byte[] bytes = listener.Receive(ref groupEP);

                    Debug.WriteLine($"Received broadcast from {groupEP} :");
                    Debug.WriteLine($" {Encoding.ASCII.GetString(bytes, 0, bytes.Length)}");
                }
            }
            catch (SocketException e)
            {
                Console.WriteLine(e);
            }
            finally
            {
                listener.Close();
            }
        }

        public static void Main()
        {
            StartListener();
        }
    }
}
