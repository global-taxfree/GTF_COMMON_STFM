using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.IO;
using System.Collections;
using System.Xml;
using log4net;

namespace GTF_Config
{
    public class GTF_ConfigManager
    {
        public string KEY_DIV = "/";
        Dictionary<string, string> m_mapConfig = null;
        public string m_AesKey = "GTF_STFM_1122";
        ILog m_logger = null;

        private static GTF_ConfigManager instance;
        private static object sync_Config = new Object();

        Dictionary<string, XmlDocument> m_mapCustomConfig = new Dictionary<string, XmlDocument>();

        Dictionary<string, string> m_mapLoadedValue = new Dictionary<string, string>();

        public static GTF_ConfigManager Instance(ILog logger = null)
        {
            lock (sync_Config)
            {
                if (instance == null)
                {
                    instance = new GTF_ConfigManager(logger);
                }
                return instance;
            }
        }

        //생성자 private 처리
        private GTF_ConfigManager(ILog logger = null)
        {
            m_logger = logger;
            //loadConfig();
        }

        public Boolean loadCustomConfig(string strType, string strFilePath)
        {
            Boolean bRet = false;
            try
            {
                XmlDocument xmldoc = new XmlDocument();
                xmldoc.Load(strFilePath);
                m_mapCustomConfig.Add(strType, xmldoc);
                bRet = true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                bRet = false;
            }
            return bRet;
        }

        public Boolean loadAppConfig(string strFilePath)
        {
            if (m_mapConfig != null)
                m_mapConfig.Clear();
            m_mapConfig = null;
            m_mapConfig = new Dictionary<string,string>();
            Boolean bRet = true;
            try
            {
                XmlDocument xmldoc = new XmlDocument();
                xmldoc.Load(strFilePath);
                XmlElement root = xmldoc.DocumentElement;

                // 노드 요소들
                XmlNodeList nodes = root.ChildNodes;
                string strValue = string.Empty;
                // 노드 요소의 값을 읽어 옵니다. 1단계 node만 검색
                foreach (XmlNode node in nodes)
                {
                    strValue = node.InnerText;
                    m_mapConfig.Add(node.Name, strValue);
                }

            }
            catch (IOException ex)
            {
                Console.WriteLine(ex.ToString());
                bRet = false;
            }
            return bRet;
        }

        public string getAppValue(string strKey)
        {
            if (m_mapConfig == null || !m_mapConfig.ContainsKey(strKey))
                return string.Empty;
            return m_mapConfig[strKey].ToString();
        }

        public string getCustomValue(string strType, string strKey, Boolean bRefresh = false)
        {
            string strRet = string.Empty;

            if (m_mapCustomConfig == null || !m_mapCustomConfig.ContainsKey(strType) )
            {
                return string.Empty;
            }

            if (!bRefresh && m_mapLoadedValue.ContainsKey(KEY_DIV+strType + KEY_DIV + strKey) )
            {
                strRet = m_mapLoadedValue[KEY_DIV + strType + KEY_DIV + strKey];
            }
            else {
                XmlNode node = m_mapCustomConfig[strType].SelectSingleNode(KEY_DIV + strType+ KEY_DIV + strKey);
                strRet = node == null ? string.Empty : node.InnerText;
                m_mapLoadedValue.Add(KEY_DIV + strType + KEY_DIV + strKey, strRet);//한번 읽은 config 는 다시 파싱하는 시간 없도록 map에 저장
            }
            return strRet;
        }
        

        //AES_256 암호화
        public String AESEncrypt256(String Input, String key)
        {
            RijndaelManaged aes = new RijndaelManaged();
            aes.KeySize = 256;
            aes.BlockSize = 128;
            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.PKCS7;
            aes.Key = Encoding.UTF8.GetBytes(key);
            aes.IV = new byte[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };

            ICryptoTransform encrypt = aes.CreateEncryptor(aes.Key, aes.IV);
            byte[] xBuff = null;
            using (MemoryStream ms = new MemoryStream())
            {
                using (CryptoStream cs = new CryptoStream(ms, encrypt, CryptoStreamMode.Write))
                {
                    byte[] arrData = Encoding.UTF8.GetBytes(Input);
                    cs.Write(arrData, 0, arrData.Length);
                }

                xBuff = ms.ToArray();
            }

            String Output = Convert.ToBase64String(xBuff);
            return Output;
        }

        //AES_256 복호화
        public String AESDecrypt256(String Input, String key)
        {
            RijndaelManaged aes = new RijndaelManaged();
            aes.KeySize = 256;
            aes.BlockSize = 128;
            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.PKCS7;
            aes.Key = Encoding.UTF8.GetBytes(key);
            aes.IV = new byte[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };

            ICryptoTransform decrypt = aes.CreateDecryptor();
            byte[] xBuff = null;
            using (MemoryStream ms = new MemoryStream())
            {
                using (CryptoStream cs = new CryptoStream(ms, decrypt, CryptoStreamMode.Write))
                {
                    byte[] arrData = Convert.FromBase64String(Input);
                    cs.Write(arrData, 0, arrData.Length);
                }

                xBuff = ms.ToArray();
            }

            String Output = Encoding.UTF8.GetString(xBuff);
            return Output;
        }

    }
}
