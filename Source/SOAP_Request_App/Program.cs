using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using System.Xml;
using System.Web;
using System.Configuration;


namespace SOAP_Request_App
{
    class Program
    {
        static void Main(string[] args)
        {

           

            string resposne = CheckAvailabilityCache(5334, 11667, new DateTime(2016, 04, 17));
            //String defaultFile = (ConfigurationManager.AppSettings["XmlInputLocation"]) == null ? "C:\booking.xml" : ConfigurationManager.AppSettings["XmlInputLocation"].ToString();
            //String outputFilePath = (ConfigurationManager.AppSettings["XmlOutputLocation"]) == null ? "C:\bookingVefiry.xml" : ConfigurationManager.AppSettings["XmlOutputLocation"].ToString();
            //String defaultFile = @"C:\Users\ccasado\Documents\Visual Studio 2010\Projects\SOAP_Request_App\SOAP_Request_App\TEST.xml";
            //String defaultFile = @"C:\Users\ccasado\Documents\RoomMaster Documents\OTA_HotelInvNotifRQ Sellable Products\OTA_Inventory_SellableProducts CID 5276.xml";
            //String defaultFile = @"C:\Users\ccasado\Desktop\BookIt\multiunit booking3.xml";
            //String defaultFile = @"C:\Users\ccasado\Documents\BookIt Xml\cancelrq.xml";
/*            String defaultFile = @"C:\Build\SOAP_Request_App\SOAP_Request_App\CasadoMsg.txt";
            String outputFilePath = @"C:\Build\SOAP_Request_App\SOAP_Request_App\CasadoMsgOut.xml";
            XmlDocument xdoc = new XmlDocument();

            if (args.Length != 0)
            {
                xdoc.Load(args[1]);
            }
            else
            {
                Console.WriteLine("No paramater found to load file.  Loading default file: " + defaultFile);
                xdoc.Load(defaultFile);
            }
            
            string xmlstr = xdoc.InnerXml;
            byte[] byteArray = Encoding.UTF8.GetBytes(xmlstr);
            String usefulString = HttpUtility.UrlEncodeUnicode(xmlstr);
            usefulString += "!!";

            //Uri xmlValid = new Uri("http://connect.leisurelink.com/DirectConnectWSTest/LLWebServices.asmx/LLDirectConnectWS");
            //Uri xmlValid = new Uri("http://localhost:3573/LLWebServices.asmx/LLDirectConnectWS");
            //Uri xmlValid = new Uri("http://localhost:3573/LLWebServices.asmx/RoomMasterService");
            Uri xmlValid = new Uri("http://connect.leisurelink.com/production/DirectConnectWS/LLWebServices.asmx/LLDirectConnectWS");
            //Uri xmlValid = new Uri("http://localhost:4314/RGWService.asmx/PublicRetailGateway");
            //Uri xmlValid = new Uri("http://localhost:4314/RGWService.asmx/PublicRetailGateway");

            //https://connect.leisurelink.com/production/directconnectws/llwebservices.asmx/roommasterservice

            //this is prod 
            //Uri xmlValid = new Uri("http://bur-lweb02:2200/RGWservice.asmx/PublicRetailGateway");
            HttpWebRequest myWebRQ = (HttpWebRequest)WebRequest.Create(xmlValid);
            myWebRQ.Credentials = CredentialCache.DefaultCredentials;
            //myWebRQ.Headers.Add("SOAPAction: ");
            //wont error during runtime "can not set using this value
            //myWebRQ.Connection = "Close";
            myWebRQ.ContentType = "text/xml";
            myWebRQ.Accept = "text/xml";
            myWebRQ.Method = "POST";
            
            myWebRQ.Timeout = 1000000;
            myWebRQ.ContentLength = byteArray.Length;

            Stream stm = myWebRQ.GetRequestStream();
            stm.Write(byteArray, 0, byteArray.Length);
            stm.Close();
        
            //MemoryStream ms = (MemoryStream)stm.;

            //xdoc.LoadXml(Encoding.UTF8.GetString(ms.ToArray()).Substring(1));
            //xdoc.Save(stm);
            
            //myWebRQ.ProtocolVersion = HttpVersion.Version11;
            //myWebRQ.

            WebResponse myWebRS = null;
            StreamReader sr = null;
            TextWriter tw = null;
            try
            {
                myWebRS = myWebRQ.GetResponse();
                sr = new StreamReader(myWebRS.GetResponseStream(), Encoding.UTF8, false);
                tw = new StreamWriter(outputFilePath, false, Encoding.UTF8);
                tw.Write(sr.ReadToEnd());
                sr.Close();
                tw.Close();
            }
            catch (WebException e)
            {
                Console.WriteLine("WebException");
                Console.WriteLine(e.Message);
                Console.WriteLine(e.Status);
            }
            

            
            
            Console.WriteLine("done");
            //Console.ReadLine();

            */
        }

        //public static string ParseForSettings(DateTime checkAvailDate)
        //{

        //    //ServicePersistent.Instance.AvailabilityCache.GetUnitProfileRateAvail(UnitProfileID);
        //}

        public static String CheckAvailabilityCache(int communityId, int UnitProfileID, DateTime availDate)
        {
            String response= String.Empty;
            //String availCacheUrl = "http://bur-lapp07.leisurelink.local:580/RateAvailReport.aspx?CommunityID="+communityId+"&UnitProfileID=" + UnitProfileID;
            String availCacheUrl = "http://bur-lapp07.leisurelink.local:580/RateAvailReport.aspx?CommunityID=" + communityId + "&UnitProfileID=" + UnitProfileID;
            HttpWebRequest myWebRQ = (HttpWebRequest)WebRequest.Create(availCacheUrl);
            myWebRQ.Credentials = CredentialCache.DefaultCredentials;
            //myWebRQ.Headers.Add("SOAPAction: ");
            //wont error during runtime "can not set using this value
            //myWebRQ.Connection = "Close";
            myWebRQ.ContentType = "text/xml";
            myWebRQ.Accept = "text/xml";
            myWebRQ.Method = "GET";

            //myWebRQ.Timeout = 1000000;
            //myWebRQ.ContentLength = byteArray.Length;

            //Stream stm = myWebRQ.GetRequestStream();
            //stm.Write(byteArray, 0, byteArray.Length);
            //stm.Close();

            WebResponse myWebRS = null;
            StreamReader sr = null;
            try
            {
                myWebRS = myWebRQ.GetResponse();
                sr = new StreamReader(myWebRS.GetResponseStream(), Encoding.UTF8, false);
                response = sr.ReadToEnd();
                sr.Close();
            }
            catch (WebException e)
            {
                Console.WriteLine("WebException");
                Console.WriteLine(e.Message);
                Console.WriteLine(e.Status);
                return response;
            }
            return response;
        }
    }
}
