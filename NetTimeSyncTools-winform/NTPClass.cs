using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Runtime.CompilerServices;

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
        short Version_Number;                // The maximum Version is 4.
        short Leap_Indicator;
        string serverName;
        short status;             // status = 0: unknown. status = 1 : Sending; status = 2: success. status = 3: error.
        NTPExtensionField[] extensions = new NTPExtensionField[2];

        int error;
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
        long TransmitTimeStampV;
        int KeyIdentifier;
        long CriptoCheckSum;
        
        void sendNTPpacket() {
            Socket s = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            
            IPAddress broadcast = IPAddress.Parse("192.168.1.255");

            
            IPEndPoint ep = new IPEndPoint(broadcast, Constraint.NtpDefaultPort);
            s.Connect(serverName, 123);
            byte[] sendbuf = Encoding.ASCII.GetBytes(args[0]);
            s.SendTo(sendbuf, ep);

            //Console.WriteLine("Message sent to the broadcast address");
        }
        byte[] NTPv4() {
            byte[] res;


            return res;
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
