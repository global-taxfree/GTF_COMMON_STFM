using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using log4net;
using System.Net;
using System.Net.Sockets;
using System.Net.NetworkInformation;
using Newtonsoft.Json.Linq;

namespace GTF_Comm
{
    public class GTF_CommManager
    {
        ILog m_logger = null;
        /*
        private static GTF_Comm instance;
        private static object sync_Comm = new Object();

        public static GTF_Comm Instance(ILog logger = null)
        {
            lock (sync_Comm)
            {
                if (instance == null)
                {
                    instance = new GTF_Comm(logger);

                }
                return instance;
            }
        }
        */
        //생성자 private 처리
        public GTF_CommManager(ILog logger = null)
        {
            m_logger = logger;
            if (m_logger == null)
                m_logger = LogManager.GetLogger("DOC");
        }
      

        //http json 통신
        public string sendHttp_Json(string url, string strJsondata,  Boolean bhttps = false, int nTimeout = 60, Boolean bLog = true, Boolean bEnocode = false)
        {
            //m_logger.Info("Call getHttp_Json");
            string strRet = "";
            string uri = url;
            string requestJson = strJsondata; // "someJsonRequestString";
            try {
                GTF_WebClient webClient = new GTF_WebClient();

                webClient.Timeout = nTimeout * 1000; // 초 단위 timeout

                webClient.Headers[HttpRequestHeader.ContentType] = "application/json";
                webClient.Headers[HttpRequestHeader.Accept] = "application/json";   
                webClient.Encoding = UTF8Encoding.UTF8;
                if (bLog)
                {
                    try {
                        if (strJsondata != null && !"".Equals(strJsondata.Trim()))
                        {
                            JToken reqObj = JToken.Parse(strJsondata);
                            if (bEnocode)
                            {
                                reqObj = converPassportChar(reqObj);
                            }
                            m_logger.Info("sendHttp_Json >> URI:" + uri);
                            m_logger.Info("sendHttp_Json >> REQ:" + reqObj.ToString());
                        }
                        else
                        {
                            m_logger.Info("sendHttp_Json >> RES size:" + strJsondata.Length);
                            m_logger.Info("sendHttp_Json >> RES:" + strJsondata.ToString());
                        }
                    }
                    catch(Exception e2)
                    {
                        m_logger.Error(e2.StackTrace);
                    }
                }
                string responseJSON = webClient.UploadString(new Uri(uri), "POST", requestJson);
                if (bLog)
                {
                    try
                    {
                        if (responseJSON != null && !"".Equals(responseJSON.Trim()))
                        {
                            JToken resObj = JToken.Parse(responseJSON);
                            if (bEnocode)
                            {
                                resObj = converPassportChar(resObj);
                            }
                            m_logger.Info("sendHttp_Json >> RES size:" + responseJSON.Length);
                            m_logger.Info("sendHttp_Json >> RES:" + resObj.ToString());
                        }
                        else
                        {
                            m_logger.Info("sendHttp_Json >> RES size:" + responseJSON.Length);
                            m_logger.Info("sendHttp_Json >> RES:" + responseJSON.ToString());
                        }
                    }catch(Exception e2)
                    {
                        m_logger.Error(e2.StackTrace);
                    }
                }
                strRet = responseJSON;
            }catch(Exception e)
            {
                Console.WriteLine(e.Message);
                m_logger.Error(e.StackTrace);
                strRet = "";
            }

            return strRet;
        }

        public string sendHttp_xml(string url, string strXmlData, Boolean bhttps = false, int nTimeout = 10)
        {
            //m_logger.Info("Call getHttp_xml");
            string strRet = string.Empty;
            string uri = url;
            try
            {
                string requestXml = strXmlData;
                GTF_WebClient webClient = new GTF_WebClient();
                webClient.Timeout = nTimeout * 1000; // 10초 timeout
                webClient.Headers[HttpRequestHeader.ContentType] = "application/xml";
                webClient.Encoding = UTF8Encoding.UTF8;
                string responseXml = webClient.UploadString(uri, requestXml);
                strRet = responseXml;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                strRet = "";
            }
            return strRet;
        }

        public string sendTcpEdi(string ip , int port, string data, int nTimeout=10)
        {
            string strRet = string.Empty;
            try
            {
                // Create a TcpClient.
                // Note, for this client to work you need to have a TcpServer 
                // connected to the same address as specified by the server, port
                // combination.
                //Int32 port = 13000;
                TcpClient client = new TcpClient(ip, port);

                // Translate the passed message into ASCII and store it as a Byte array.
                Byte[] arrSendData = System.Text.Encoding.UTF8.GetBytes(data);
                Byte[] arrRectdata = null;
                // Get a client stream for reading and writing.
                //  Stream stream = client.GetStream();

                client.ReceiveTimeout = nTimeout * 1000;//Timeout 서정
                NetworkStream stream = client.GetStream();

                // Send the message to the connected TcpServer. 
                stream.Write(arrSendData, 0, arrSendData.Length);

                // Receive the TcpServer.response.

                // Buffer to store the response bytes.
                arrRectdata = new Byte[4096];

                // String to store the response ASCII representation.
                String responseData = String.Empty;
                StringBuilder sb = new StringBuilder();
                sb.Clear();

                // Read the first batch of the TcpServer response bytes.
                Int32 bytes = -1;
                while (true)
                {
                    bytes = stream.Read(arrRectdata, 0, arrRectdata.Length);
                    if (bytes == -1)
                        break;
                    sb.Append(System.Text.Encoding.UTF8.GetString(arrRectdata, 0, bytes));
                }

                Console.WriteLine("Received: {0}", sb.ToString());

                // Close everything.
                stream.Close();
                client.Close();
                strRet = sb.ToString();
                sb.Clear();
            }
            catch (ArgumentNullException e)
            {
                Console.WriteLine("ArgumentNullException: {0}", e);
            }
            catch (SocketException e)
            {
                Console.WriteLine("SocketException: {0}", e);
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception: {0}", e);
            }

            return strRet;
        }

        public Boolean sendPing(string host ,int nTimeout = 10000)
        {
            var ping = new Ping();
            var buffer = new byte[32];
            var pingOptions = new PingOptions();

            if(host.IndexOf("//") >=0)
                host = host.Substring(host.IndexOf("//") + 2);
            try
            {
                var reply = ping.Send(host, nTimeout, buffer, pingOptions);
                return (reply != null && reply.Status == IPStatus.Success);
            }
            catch (Exception e)
            {
                //m_logger.Error(e.StackTrace);
                m_logger.Error("Ping Error >> "+ host);
                return false;
            }
        }

        public JToken converPassportChar(JToken jObj)
        {
            if (jObj is JObject)
            {
                Dictionary<string, JToken> dictObj = jObj.ToObject<Dictionary<string, JToken>>();
                IList<string> keys = ((JObject)jObj).Properties().Select(p => p.Name).ToList();
                if (keys != null)
                {
                    for (int i = 0; i < keys.Count; i++)
                    {
                        if (keys[i] != null && keys[i].ToString().ToUpper().IndexOf("PASSPORT") >= 0)
                        {
                            if (jObj[keys[i]] != null && jObj[keys[i]] is JValue)
                            {
                                jObj[keys[i]] = "".PadLeft(jObj[keys[i]].ToString().Length, '*');
                            }
                        }
                    }
                }
            }
            else if (jObj is JArray)
            {
                for(int i=0; ((JArray)jObj).Count > i; i ++)
                {
                    jObj[i] = converPassportChar(jObj[i]);
                }
            }

            return jObj;
        }
    }
}
