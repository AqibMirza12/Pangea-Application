using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;
using System.IO;
using NHapi.Base.Parser;
using NHapi.Base.Model;
using NHapi.Base;
using NHapi.Base.Util;
using NHapi.Model.V231;
using NHapi.Model.V231.Message;
using NHapi.Model.V231.Segment;
using NHapiTools.Base.Parser;
using NHapi.Model.V231.Group;
using System.Globalization;
using System.Text.RegularExpressions;
using UnityEngine;

namespace VSCaptureMRay
{
    public class MRayTCPclient : TcpClient
    {
        public IPEndPoint m_remoteIPtarget;
        public int m_csvexportset = 1;
        public static List<string> m_FrameList = new List<string>();

        public static List<NumericValResult> m_NumericValList = new List<NumericValResult>();
        public StringBuilder m_strbuildvalues = new StringBuilder();
        public static StringBuilder m_strbuildheaders = new StringBuilder();
        public static bool m_transmissionstart = true;
        public static List<string> m_NumValHeaders = new List<string>();

        public class NumericValResult
        {
            public string Timestamp;
            public string PhysioID;
            public string Value;
        }

        //Create a singleton TCPclient subclass
        private static volatile MRayTCPclient MRayTClient = null;

        public static MRayTCPclient getInstance
        {

            get
            {
                if (MRayTClient == null)
                {
                    lock (typeof(MRayTCPclient))
                        if (MRayTClient == null)
                        {
                            MRayTClient = new MRayTCPclient();
                        }

                }
                return MRayTClient;
            }

        }

        public MRayTCPclient()
        {
            MRayTClient = this;

            m_remoteIPtarget = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 4601);

            MRayTClient.Client.ReceiveTimeout = 20000;

        }

        public void ReadData(byte[] readbuffer)
        {
            //string HL7Msg1 = RemoveMLLPFrameGetHL7Msg(readbuffer);
            //ProcessPacket(HL7Msg1);

            string readbuffstr = Encoding.GetEncoding("iso-8859-1").GetString(readbuffer);

            ParseFramesFromMLLP(readbuffstr);

            foreach (string HL7Msg in m_FrameList)
            {
                ProcessPacket(HL7Msg);
            }
            m_FrameList.RemoveRange(0, m_FrameList.Count);


        }

        public string RemoveMLLPFrameGetHL7Msg(byte[] readbuffer)
        {
            string readbuffstr = Encoding.GetEncoding("iso-8859-1").GetString(readbuffer);
            StringBuilder sb = new StringBuilder(readbuffstr);

            NHapiTools.Base.Util.MLLP.StripMLLPContainer(sb);
            return sb.ToString();
        }

        public void ParseFramesFromMLLP(string readbufferstr)
        {
            int framestart = readbufferstr.IndexOf((char)0x0B);
            if (framestart >= 0)
            {
                int frameend = readbufferstr.IndexOf((char)0x1C);
                if (frameend != readbufferstr.Count())
                {
                    int frameendchar2 = frameend + 1;
                    if (frameend > framestart && (readbufferstr.ElementAt(frameendchar2) == (char)0x0D))
                    {
                        string HL7substr = readbufferstr.Substring(framestart + 1, (frameend - (framestart + 1)));
                        if (HL7substr != "") m_FrameList.Add(HL7substr);

                        string updatedbufferstr = readbufferstr.Remove(framestart, (frameendchar2 - framestart) + 1);
                        ParseFramesFromMLLP(updatedbufferstr);
                    }

                }

            }


        }

        public void ProcessPacket(string readbuffstr)
        {

            int messageidpos = readbuffstr.IndexOf("|103|P|2.3.1|");

            int hlverpos = readbuffstr.IndexOf("2.3.1|\r");

            string demoptdetails = "PID|||0b140f00-fc54-0675-05111f1202007800||mark^henery||19740107|M|\rPV1||I|^^&&2852002658&4601&&1|||||||||||||||A|||\rOBR||||Mindray Monitor|||0|\r";

            string readbuffstr2;

            if (messageidpos == -1)
            {
                readbuffstr2 = readbuffstr.Insert(hlverpos + 7, demoptdetails);
            }
            else readbuffstr2 = readbuffstr;

            PipeParser parser = new PipeParser();

            IMessage imsg = parser.Parse(readbuffstr2);

            NHapi.Model.V231.Message.ORU_R01 oruR1 = imsg as NHapi.Model.V231.Message.ORU_R01;

            int orderobservationRepetitions = oruR1.GetPATIENT_RESULT(0).ORDER_OBSERVATIONRepetitionsUsed;


            for (int i = 0; i < orderobservationRepetitions; i++)
            {
                // ORU_R01_ORDER_OBSERVATION orderObservation = ptResult.GetORDER_OBSERVATION(i);
                ORU_R01_ORDER_OBSERVATION orderObservation = oruR1.GetPATIENT_RESULT(0).GetORDER_OBSERVATION(i);


                int observationRepetitions = orderObservation.OBSERVATIONRepetitionsUsed;

                for (int j = 0; j < observationRepetitions; j++)
                {
                    ORU_R01_OBSERVATION observation = orderObservation.GetOBSERVATION(j);

                    if (observation != null)
                    {
                        OBX obx = observation.OBX;
                        if (obx.ValueType.ToString() == "NM")
                        {
                            DateTime dtDateTime = DateTime.Now;
                            string strDateTime = dtDateTime.ToString("dd-MM-yyyy HH:mm:ss.fff", CultureInfo.InvariantCulture);
                            Console.WriteLine("Time:{0}", strDateTime);

                            string obxvaluename = "ParameterID: " + obx.ObservationIdentifier.Identifier.ToString();
                            string obxname = "Name:" + obx.ObservationIdentifier.Text;
                            //string obxsubidvalue = "SubID: " + obx.ObservationSubID.ToString();

                            Varies val = obx.GetObservationValue(0);
                            string obxvalue = "Value: " + val.Data.ToString();

                            //string aperiodicdate = obx.DateTimeOfTheObservation.TimeOfAnEvent.GetAsDate().ToLongDateString();
                            //string aperiodictime = obx.DateTimeOfTheObservation.TimeOfAnEvent.GetAsDate().ToLongTimeString();

                            /*string dtformat = "yyyyMMddhhmmss";
                            DateTime dt = new DateTime();

                            if (aperiodictime != "")
                            {
                                dt = DateTime.ParseExact(aperiodictime, dtformat, CultureInfo.InvariantCulture);
                            }*/

                            Console.WriteLine(obxvaluename);
                            //Console.WriteLine(obxsubidvalue);
                            Console.WriteLine(obxname);
                            Console.WriteLine(obxvalue);
                            //Console.WriteLine(aperiodicdate);
                            //Console.WriteLine(aperiodictime);

                            NumericValResult NumVal = new NumericValResult();
                            NumVal.Timestamp = strDateTime;
                            NumVal.PhysioID = obx.ObservationIdentifier.Text.ToString();
                            NumVal.Value = val.Data.ToString();

                            m_NumericValList.Add(NumVal);
                            m_NumValHeaders.Add(NumVal.PhysioID);

                        }
                    }
                }


            }
            SaveNumericValueListRows();
        }

        public string BuildMLLPstring(string HL7Message)
        {
            StringBuilder messageString = new StringBuilder();
            messageString.Append((char)0x0B);
            messageString.Append(HL7Message);
            messageString.Append((char)0x1C);
            messageString.Append((char)0x0D);
            return messageString.ToString();
        }

        public async Task SendCycleIntermittentQueryInterfaceRequest(int nInterval)
        {
            int nmillisecond = nInterval * 1000;

            if (nmillisecond != 0)
            {
                do
                {
                    IntermittentQueryInterfaceRequest();
                    await Task.Delay(nmillisecond);

                }
                while (true);
            }
            else IntermittentQueryInterfaceRequest();
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

        public void IntermittentQueryInterfaceRequest()
        {
            try
            {
                MRayTClient.Client.Connect(m_remoteIPtarget);

                SendQueryInterfaceRequest();
                WaitForSeconds(1);
                SendQueryInterfaceRequest();
                WaitForSeconds(1);
                SendQueryInterfaceRequest();


                NetworkStream netstream = MRayTClient.GetStream();

                byte[] readbuffer = new byte[4096];

                StringBuilder completemessage = new StringBuilder();

                int numBytesread = 0;

                do
                {
                    numBytesread = netstream.Read(readbuffer, 0, readbuffer.Length);

                    string readbuffstr = Encoding.GetEncoding("iso-8859-1").GetString(readbuffer, 0, numBytesread);

                    completemessage.Append(readbuffstr);

                }
                while (netstream.DataAvailable);

                string path = Path.Combine(Directory.GetCurrentDirectory(), "MRayrawoutput.txt");

                string readdatastr = completemessage.ToString();

                StringDataToFile(path, readdatastr);

                ParseFramesFromMLLP(readdatastr);

                foreach (string HL7Msg in m_FrameList)
                {
                    ProcessPacket(HL7Msg);
                }
                m_FrameList.RemoveRange(0, m_FrameList.Count);

                completemessage.Clear();

                MRayTClient.Close();

            }
            catch (Exception ex)
            {
                Console.WriteLine("Error opening/writing to TCP port :: " + ex.Message, "Error!");
            }

        }

        public void SendQueryInterfaceRequest()
        {
            //MRayTClient.Client.Send(DataConstants.query_request_bytes1);
            string senddatastring = BuildMLLPstring(DataConstants.query_request_string8);

            //byte[] senddata = Encoding.GetEncoding("iso-8859-1").GetBytes(DataConstants.query_request_string5);
            byte[] senddata = Encoding.GetEncoding("iso-8859-1").GetBytes(senddatastring);

            MRayTClient.Client.Send(senddata);
        }

        public async Task SendCycleQueryInterfaceRequest(int nInterval)
        {
            int nmillisecond = nInterval * 1000;

            //For single data ID 101 uncomment code below
            //string senddatastring = BuildMLLPstring(DataConstants.query_request_string6);

            //For all data every 1 sec uncomment code below
            //string senddatastring = BuildMLLPstring(DataConstants.query_request_string5);

            string senddatastring = BuildMLLPstring(DataConstants.query_request_string8);

            byte[] senddata1 = Encoding.GetEncoding("iso-8859-1").GetBytes(senddatastring);

            if (nmillisecond != 0)
            {
                do
                {
                    MRayTClient.Client.Send(senddata1);
                    await Task.Delay(nmillisecond);

                }
                while (true);
            }
            else MRayTClient.Client.Send(senddata1);
        }

        public async Task SendTCPEchoMessage(int nInterval)
        {
            int nmillisecond = nInterval * 1000;

            string senddatastring = BuildMLLPstring(DataConstants.tcp_echo_msg1);

            byte[] senddata1 = Encoding.GetEncoding("iso-8859-1").GetBytes(senddatastring);

            if (nmillisecond != 0)
            {
                do
                {
                    MRayTClient.Client.Send(senddata1);
                    await Task.Delay(nmillisecond);

                }
                while (true);
            }
            else MRayTClient.Client.Send(senddata1);
        }

        public static void WriteNumericHeadersList()
        {
            if (m_NumericValList.Count != 0 && m_transmissionstart)
            {
                string pathcsv = Path.Combine(Directory.GetCurrentDirectory(), "MRayDataExport.csv");

                m_strbuildheaders.Append("Time");
                m_strbuildheaders.Append(',');


                foreach (NumericValResult NumValResult in m_NumericValList)
                {
                    m_strbuildheaders.Append(NumValResult.PhysioID);
                    m_strbuildheaders.Append(',');

                }

                m_strbuildheaders.Remove(m_strbuildheaders.Length - 1, 1);
                m_strbuildheaders.Replace(",,", ",");
                m_strbuildheaders.AppendLine();
                ExportNumValListToCSVFile(pathcsv, m_strbuildheaders);

                m_strbuildheaders.Clear();
                m_NumValHeaders.RemoveRange(0, m_NumValHeaders.Count);
                m_transmissionstart = false;
            }
        }

        public void SaveNumericValueListRows()
        {
            if (m_NumericValList.Count != 0)
            {
                WriteNumericHeadersList();

                string pathcsv = Path.Combine(Directory.GetCurrentDirectory(), "MRayDataExport.csv");

                m_strbuildvalues.Append(m_NumericValList.ElementAt(0).Timestamp);
                m_strbuildvalues.Append(',');


                foreach (NumericValResult NumValResult in m_NumericValList)
                {
                    m_strbuildvalues.Append(NumValResult.Value);
                    m_strbuildvalues.Append(',');

                }

                m_strbuildvalues.Remove(m_strbuildvalues.Length - 1, 1);
                m_strbuildvalues.Replace(",,", ",");
                m_strbuildvalues.AppendLine();

                ExportNumValListToCSVFile(pathcsv, m_strbuildvalues);
                m_strbuildvalues.Clear();
                m_NumericValList.RemoveRange(0, m_NumericValList.Count);
            }
        }

        public static void ExportNumValListToCSVFile(string _FileName, StringBuilder strbuildNumVal)
        {
            try
            {
                // Open file for reading. 
                StreamWriter wrStream = new StreamWriter(_FileName, true, Encoding.UTF8);

                wrStream.Write(strbuildNumVal);
                strbuildNumVal.Clear();

                // close file stream. 
                wrStream.Close();

            }

            catch (Exception _Exception)
            {
                // Error. 
                Console.WriteLine("Exception caught in process: {0}", _Exception.ToString());
            }

        }

        public void StringDataToFile(string _Filename, string stringData)
        {
            try
            {
                // Open file for reading. 
                StreamWriter wrStream = new StreamWriter(_Filename, true, Encoding.GetEncoding("iso-8859-1"));

                wrStream.WriteLine(stringData);

                // close file stream. 
                wrStream.Close();

            }

            catch (Exception _Exception)
            {
                // Error. 
                Console.WriteLine("Exception caught in process: {0}", _Exception.ToString());
            }

        }

        public bool ByteArrayToFile(string _FileName, byte[] _ByteArray, int nWriteLength)
        {
            try
            {
                /*// Open file for reading. 
                FileStream _FileStream = new FileStream(_FileName, FileMode.Append, FileAccess.Write);

                // Writes a block of bytes to this stream using data from a byte array
                _FileStream.Write(_ByteArray, 0, nWriteLength);
                
                // close file stream. 
                _FileStream.Close();*/

                // Open file for reading. 
                StreamWriter wrStream = new StreamWriter(_FileName, true, Encoding.UTF8);

                String datastr = BitConverter.ToString(_ByteArray);

                wrStream.WriteLine(datastr);

                // close file stream. 
                wrStream.Close();


                return true;
            }

            catch (Exception _Exception)
            {
                // Error. 
                Console.WriteLine("Exception caught in process: {0}", _Exception.ToString());
            }
            // error occured, return false. 
            return false;
        }


    }
}
