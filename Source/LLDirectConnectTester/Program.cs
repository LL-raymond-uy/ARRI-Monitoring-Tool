using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using System.Xml;
using System.Web;
using System.Configuration;
namespace LLDirectConnectTester
{
    class Program
    {
        static void Main(string[] args)
        {
            DateTime dateStart = new DateTime(2016,04,17);//this should get a value based on current date so it doesn't get old
            DateTime dateEnd = new DateTime(2016,04,17);
            int unitProfileID = 11667;
            String testInfo = " UnitProfileID: " + unitProfileID + " DateStart: " + dateStart + " DateEnd: " + dateEnd;

            //Get current avail value.  //should also check value on 07 server
            AvailabilityService08.RateAvailability serv08 = new AvailabilityService08.RateAvailability();
            AvailabilityService08.UnitAvail unitAvailPreChange = serv08.GetUnitAvailabilityCache(unitProfileID, dateStart, dateEnd);
            int availOn08PreChange = FindCurrentAvailability(unitAvailPreChange, dateStart);
            LogMessage("Availability on 08 is " + availOn08PreChange + "for" + testInfo);

            //find valaue to change it to
            int numAvail = (availOn08PreChange + 1) % 10;

            testInfo = " ExpectedAvail: " + numAvail + testInfo;


            Uri uri = new Uri("http://connect.leisurelink.com/production/DirectConnectWS/LLWebServices.asmx/LLDirectConnectWS");
            String rdpMessage = RDPMessage1 + dateStart.ToString("yyyy-MM-dd") + RdPMessagePart2 + dateEnd.ToString("yyyy-MM-dd") + RdpMessagePart3 + numAvail + RdpMessagePart4;
            String response = SendRDPUpdate(rdpMessage, uri);

            if (!response.Contains("Success"))
            {
                LogMessage("Invalid RDP Message for " + testInfo);
                return;
            }

            //This may take some time so may need to loop a few times

            //TBD get CAcheValue returned so that last updated timestamp can be retrieved.
            AvailabilityService08.UnitAvail unitAvail = serv08.GetUnitAvailabilityCache(unitProfileID,dateStart, dateEnd);


            CheckAvailabilityValues(unitAvail,numAvail,unitProfileID, dateStart, dateEnd,testInfo );

            //should also check value on 07 server

        }

        //To read mesage via file
        //String defaultFile = @"C:\Build\SOAP_Request_App\SOAP_Request_App\CasadoMsg.txt";
        //String outputFilePath = @"C:\Build\SOAP_Request_App\SOAP_Request_App\CasadoMsgOut.xml";
            //        XmlDocument xdoc = new XmlDocument();

            //if (args.Length != 0)
            //{
            //    xdoc.Load(args[1]);
            //}
            //else
            //{
            //    Console.WriteLine("No paramater found to load file.  Loading default file: " + defaultFile);
            //    xdoc.Load(defaultFile);
            //}
            
            //string xmlstr = xdoc.InnerXml;

        private static void LogMessage(string msg)
        {
            Console.WriteLine(msg);
        }

        private static String SendRDPUpdate(String rdpMessage, Uri uri)
        {

            String response = String.Empty;
            string xmlstr = rdpMessage;
            byte[] byteArray = Encoding.UTF8.GetBytes(xmlstr);
            //String usefulString = HttpUtility.UrlEncodeUnicode(xmlstr);
            //usefulString += "!!";


            HttpWebRequest myWebRQ = (HttpWebRequest)WebRequest.Create(uri);
            myWebRQ.Credentials = CredentialCache.DefaultCredentials;

            myWebRQ.ContentType = "text/xml";
            myWebRQ.Accept = "text/xml";
            myWebRQ.Method = "POST";
            
            myWebRQ.Timeout = 1000000;
            myWebRQ.ContentLength = byteArray.Length;

            Stream stm = myWebRQ.GetRequestStream();
            stm.Write(byteArray, 0, byteArray.Length);
            stm.Close();
        
            WebResponse myWebRS = null;
            StreamReader sr = null;
            TextWriter tw = null;
            try
            {
                myWebRS = myWebRQ.GetResponse();
                sr = new StreamReader(myWebRS.GetResponseStream(), Encoding.UTF8, false);
 
                return sr.ReadToEnd();
                tw.Close();
            }
            catch (WebException e)
            {
                return "WebException Message" + e.Message + "Status: " + e.Status;
            }
            

            
            
            Console.WriteLine("done");
            //Console.ReadLine();

        }

        private static string RDPMessage1 = "<?xml version=\"1.0\" encoding=\"UTF-8\" ?>"
+"<soap:Envelope xmlns:soap=\"http://schemas.xmlsoap.org/soap/envelope/\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\">"
+"<soap:Header>"
+"<HTNGHeader xmlns=\"http://htng.org/1.1/Header/\">"
+"<From>"
+"<systemId></systemId>"
+"<Credential>"
+"<userName>5334</userName>"
+"<password>17150912</password>"
+"</Credential>"
+"</From>"
+"<To>"
+"<systemID>LeisureLink</systemID>"
+"</To>"
+"<timeStamp>2016-02-29T15:00:00-05:00</timeStamp>"
+"<echoToken>9720cf34-c32a-c944-9d70-a609cb33f447</echoToken>"
+"<transactionId>785905400011abc</transactionId>"
+"<action>Request</action>"
+"</HTNGHeader>"
+"</soap:Header>"
+"<soap:Body>"
+"<OTA_HotelInvCountNotifRQ EchoToken=\"fa51c533-b568-b95c-cb79-bb42c10eb961abc\" TimeStamp=\"2016-02-29T15:00:00-05:00\" Target=\"Production\" Version=\"1.002\" SequenceNmbr=\"785901500\" xmlns=\"http://www.opentravel.org/OTA/2003/05\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\">"
+"<Inventories HotelCode=\"5334\" HotelName=\"LeisurelinkTSET\">"
+"<Inventory>"
+"<StatusApplicationControl Start=\"";
        //2016-04-17
        private static string RdPMessagePart2 = "\" End=\"";
        //2016-04-17
        private static string RdpMessagePart3 = "\" InvTypeCode=\"11667\" Override=\"true\"/>"
+"<InvCounts>"
+"<InvCount CountType=\"1\" Count=\"";
        // 0
       private static string RdpMessagePart4 = "\"/>"
+"</InvCounts>"
+"</Inventory>"
//<Inventory>
//<StatusApplicationControl Start="2016-04-18" End="2016-04-18" InvTypeCode="11667" Override="true"/>
//<InvCounts>
//<InvCount CountType="1" Count="0"/>
//</InvCounts>
//</Inventory>
+"</Inventories>"
+"</OTA_HotelInvCountNotifRQ>"
+"</soap:Body>"
+"</soap:Envelope>";

        private static int FindCurrentAvailability(AvailabilityService08.UnitAvail unitAvail,DateTime dateStart)
        {
            int numAvail = -1;
            if(unitAvail != null)
            {
                foreach(AvailabilityService08.Availability avail in unitAvail.Availabilities)
                {
                    if (avail.DateAvail == dateStart)
                    {
                        if (avail.NumAvail.HasValue)
                        {
                            if (avail.DateAvail == dateStart)
                            {
                                return avail.NumAvail.Value;
                            }
                        }
                    }
                }
            }
            return numAvail;
        }


        private static void CheckAvailabilityValues(AvailabilityService08.UnitAvail unitAvail,int numAvail,int unitProfileID, DateTime dateStart, DateTime dateEnd, String testInfo )
        {
            if(unitAvail != null)
            {
                foreach(AvailabilityService08.Availability avail in unitAvail.Availabilities)
                {
                    if (avail.DateAvail == dateStart)
                    {
                        if (avail.NumAvail.HasValue)
                        {
                            if (avail.NumAvail.Value == numAvail)
                            {
                                LogMessage("Availabilty Matches AvailabiltyService Cache on 08 for Value: " + numAvail + testInfo);
                            }
                            else
                            {
                                LogMessage("Availabilty Does Not Match in AvailabiltyService Cache on 08 Expected: " + numAvail + " Actual: " + avail.NumAvail.Value + testInfo);
                            }
                        }
                        else
                        {
                            LogMessage("Availabilty Date not in AvailabiltyService Cache on 08" + testInfo);
                        }
                    }
                    else
                    {
                        LogMessage("Availabilty Date not in AvailabiltyService Cache on 08" + testInfo);
                    }
                }
            }
            else
            {
                LogMessage("UnitAvail not in cache on 08 for" + testInfo);
            }
        }
    }
}
