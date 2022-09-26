using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace VSCaptureMRay
{
    class Program
    {
        static void Main(string[] args)
        {
            ConnectviaTCP();
        }

        public class TcpState
        {
            //Tcp client
            public MRayTCPclient tcpClient;
            //RemoteIP
            public IPEndPoint remoteIP;
            //buffer
            public byte[] buffer = new byte[4096];
        }

        private static ManualResetEvent receiveDone = new ManualResetEvent(false);

        public static void ReceiveCallback(IAsyncResult ar)
        {
            try
            {
                // Retrieve the state object and the client   
                // from the asynchronous state object.  
                TcpState state = (TcpState)ar.AsyncState;

                MRayTCPclient Client = state.tcpClient;

                string path = Path.Combine(Directory.GetCurrentDirectory(), "MRayrawoutput.txt");

                NetworkStream stream = Client.GetStream();

                // Read data from the remote device.  
                byte[] buffer = state.buffer;
                int bytesRead = stream.EndRead(ar);

                byte[] readbuffer = new byte[bytesRead];
                Array.Copy(buffer, readbuffer, bytesRead);

                if (bytesRead > 0)
                {
                    // There might be more data, so store the data received so far.  

                    string readbuffstr = Encoding.GetEncoding("iso-8859-1").GetString(readbuffer);

                    Client.StringDataToFile(path, readbuffstr);

                    Client.ReadData(readbuffer);

                    //  Get the rest of the data.  
                    stream.BeginRead(buffer, 0, buffer.Length, new AsyncCallback(ReceiveCallback), state);

                }
                else
                {
                    // All the data has arrived; put it in response.  


                    // Signal that all bytes have been received.  
                    receiveDone.Set();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        public static void ReadData(object sender, byte[] readbuffer)
        {
            (sender as MRayTCPclient).ReadData(readbuffer);
        }

        public static void WaitForSeconds(int nsec)
        {
            DateTime dt = DateTime.Now;
            DateTime dt2 = dt.AddSeconds(nsec);
            do
            {
                dt = DateTime.Now;
            }
            while (dt2 > dt);

        }

        public static void ConnectviaTCP()
        {
            Console.WriteLine("You may connect an Ethernet cable to the Mindray monitor LAN port");
            Console.WriteLine("Note the IP address from the Network Status menu in the monitor");

            Console.WriteLine();
            Console.WriteLine("Numeric Data Transmission sets:");
            Console.WriteLine("1. 1 second (Real time)");
            Console.WriteLine("2. 9 second (Averaged)");
            Console.WriteLine("3. 1 minute (Averaged)");
            Console.WriteLine("4. 5 minute (Averaged)");
            Console.WriteLine("5. Single poll");
            Console.WriteLine();
            Console.Write("Choose Data Transmission interval (1-5):");

            string sIntervalset = Console.ReadLine();
            int[] setarray = { 1, 9, 60, 300, 0 };
            short nIntervalset = 2;
            int nInterval = 9;
            if (sIntervalset != "") nIntervalset = Convert.ToInt16(sIntervalset);
            if (nIntervalset > 0 && nIntervalset < 6) nInterval = setarray[nIntervalset - 1];

            /*Console.WriteLine();
            Console.WriteLine("CSV Data Export Options:");
            Console.WriteLine("1. Single value list");
            Console.WriteLine("2. Data packet list");
            Console.WriteLine("3. Consolidated data list");
            Console.WriteLine();
            Console.Write("Choose CSV export option (1-3):");

            string sCSVset = Console.ReadLine();
            int nCSVset = 3;
            if (sCSVset != "") nCSVset = Convert.ToInt32(sCSVset);*/

            // Create a new TCP Client object with default settings.
            MRayTCPclient _MRaytcpclient = MRayTCPclient.getInstance;

            Console.WriteLine("Enter the target IP address of the monitor assigned by DHCP:");

            string IPAddressRemote = Console.ReadLine();

            Console.WriteLine("Connecting to {0}...", IPAddressRemote);
            Console.WriteLine();
            Console.WriteLine("Requesting Transmission set {0} from monitor", nIntervalset);
            Console.WriteLine();
            Console.WriteLine("Data will be written to CSV file MRayDataExport.csv in same folder");
            Console.WriteLine();
            Console.WriteLine("Press Escape button to Stop");

            //if (nCSVset > 0 && nCSVset < 4) _MRaytcpclient.m_csvexportset = nCSVset;

            if (IPAddressRemote != "")
            {
                //Default MindRay monitor port is 4601
                _MRaytcpclient.m_remoteIPtarget = new IPEndPoint(IPAddress.Parse(IPAddressRemote), 4601);


                try
                {

                    if (nInterval != 1)
                    {
                        //Intermittent query interface
                        do
                        {
                            MRayTCPclient _MRaytcpclient2 = new MRayTCPclient();
                            _MRaytcpclient2.m_remoteIPtarget = new IPEndPoint(IPAddress.Parse(IPAddressRemote), 4601);

                            _MRaytcpclient2.IntermittentQueryInterfaceRequest();

                            if (nInterval == 0) break;

                            WaitForSeconds(nInterval);

                        }
                        while (true);


                    }
                    else
                    {
                        //Real time results interface

                        _MRaytcpclient.Connect(_MRaytcpclient.m_remoteIPtarget);

                        TcpState state = new TcpState();
                        state.tcpClient = _MRaytcpclient;
                        state.remoteIP = _MRaytcpclient.m_remoteIPtarget;

                        //Send QueryRequest message

                        //Task.Run(() => _MRaytcpclient.SendCycleQueryInterfaceRequest(nInterval));
                        Task.Run(() => _MRaytcpclient.SendCycleQueryInterfaceRequest(9));

                        string path = Path.Combine(Directory.GetCurrentDirectory(), "MRayrawoutput.txt");

                        //Send TCP echo messages
                        //int nEchointerval = 1000;

                        //if (nInterval != 1)
                        //{
                        //Task.Run(() => _MRaytcpclient.SendTCPEchoMessage(nEchointerval));
                        //}

                        //Receive PollDataResponse message
                        NetworkStream netstream = _MRaytcpclient.GetStream();

                        byte[] buffer = state.buffer;

                        netstream.BeginRead(buffer, 0, buffer.Length, new AsyncCallback(ReceiveCallback), state);

                        //Parse PollDataResponses
                    }

                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error opening/writing to TCP port :: " + ex.Message, "Error!");
                }


            }
            else
            {
                Console.WriteLine("Invalid IP Address");
            }



        }
    }
}