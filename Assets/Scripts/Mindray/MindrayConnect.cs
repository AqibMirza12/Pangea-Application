using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using VSCaptureMRay;

class MindrayConnect : MonoBehaviour
{
    private void Start()
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

    public static string GetLocalIPv4()
    {
        return Dns.GetHostEntry(Dns.GetHostName())
            .AddressList.First(
                f => f.AddressFamily == AddressFamily.InterNetwork)
            .ToString();
    }

    private void OnGUI()
    {
        GUI.Label(new Rect(0, 0, 100, 50), "Connect the Mindray Monitor using an Ethernet cable");
        GUI.Label(new Rect(0, 0, 100, 50), "Data will be outputted in real-time");
        GUI.Label(new Rect(0, 0, 100, 50), "Connecting to Mindray Monitor");
    }

    public static void ConnectviaTCP()
        {
            GetLocalIPv4();

            string sIntervalset = Input.inputString;
            int[] setarray = { 1, 9, 60, 300, 0 };
            short nIntervalset = 2;
            int nInterval = 9;
            if (sIntervalset != "") nIntervalset = Convert.ToInt16(sIntervalset);
            if (nIntervalset > 0 && nIntervalset < 6) nInterval = setarray[nIntervalset - 1];

            // Create a new TCP Client object with default settings.
            MRayTCPclient _MRaytcpclient = MRayTCPclient.getInstance;

            string IPAddressRemote = Input.inputString;

           
           Console.WriteLine("Requesting Transmission set {0} from monitor", nIntervalset);

        //if (nCSVset > 0 && nCSVset < 4) _MRaytcpclient.m_csvexportset = nCSVset;

        if (IPAddressRemote != "")
            {
                //Default MindRay monitor port is 4601
                _MRaytcpclient.m_remoteIPtarget = new IPEndPoint(IPAddress.Parse(IPAddressRemote), 4601);
                Debug.Log(_MRaytcpclient.m_remoteIPtarget);

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

                        //string path = Path.Combine(Directory.GetCurrentDirectory(), "MRayrawoutput.txt");

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
