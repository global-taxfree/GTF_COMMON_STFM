using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

using Microsoft.Win32;
using log4net;

namespace GTF_Passport
{

    public class GTF_PassportScanner
    {
        [DllImport("GTF_PASSPORT_COMM.dll")]
        public static extern int OpenPort();
        [DllImport("GTF_PASSPORT_COMM.dll")]
        public static extern int OpenPortByNum(int portNum);
        [DllImport("GTF_PASSPORT_COMM.dll")]
        public static extern int Scan();
        [DllImport("GTF_PASSPORT_COMM.dll")]
        public static extern int ReceiveData(int TimeOut);
        [DllImport("GTF_PASSPORT_COMM.dll")]
        public static extern int ClosePort();
        [DllImport("GTF_PASSPORT_COMM.dll")]
        public static extern int GetPassportInfo(StringBuilder passInfo);

        [DllImport("GTF_PASSPORT_COMM.dll")]
        public static extern int SetPassportScanType(int nType);

        [DllImport("GTF_PASSPORT_COMM.dll")]
        public static extern int GetMRZ1(StringBuilder strMRZ1);
        [DllImport("GTF_PASSPORT_COMM.dll")]
        public static extern int GetMRZ2(StringBuilder strMRZ2);
        [DllImport("GTF_PASSPORT_COMM.dll")]
        public static extern int GetMRZ3(StringBuilder strMRZ3);
        [DllImport("GTF_PASSPORT_COMM.dll")]
        public static extern int Clear();
        [DllImport("GTF_PASSPORT_COMM.dll")]
        public static extern int SetSavePath(StringBuilder strPath);

        private int m_nPort = -1;
        private int m_nType = -1;
        private Boolean m_bOpen = false;

        private string m_strMRZ1 = string.Empty;
        private string m_strMRZ2 = string.Empty;
        private string m_strMRZ3 = string.Empty;
        private ArrayList arrType = new ArrayList();
        private ILog m_logger;

        private string m_strPassPath = string.Empty;

        //Passport_Interface m_passScan = null;

        private static GTF_PassportScanner instance;
        private static object sync_PassScan = new Object();
        public static GTF_PassportScanner Instance(ILog logger = null, string strPassPath = "")
        {
            lock (sync_PassScan)
            {
                if (instance == null)
                {
                    instance = new GTF_PassportScanner(logger, strPassPath);
                }
                return instance;
            }

        }

        //생성자 private 처리

        private GTF_PassportScanner(ILog logger = null, string strPassPath = "")
        {
            //관련 lib 로드
            m_nPort = -1;
            m_nType = -1;
            m_bOpen = false;
            m_logger = logger;
            m_strPassPath = strPassPath;
        }

        //여권스캐너 포트 지정. 반환값 port 번호
        public int open(int nType, int nPort = 0)
        {
            int nRet = 0;
            if (m_bOpen && m_nType == nType) //여권스캔 기 오픈되어 있으면 반환
            {
                nRet = m_nPort;
                return nRet;
            }

            SetPassportScanType(nType);
            if (nPort > 0)//port 지정
            {
                //nRet = passportScanner.open(nPort);
                nRet = OpenPortByNum(nPort);
                
            }
            else //port 자동스캔
            {
                //nRet = passportScanner.open();
                nRet = OpenPort();
            }

            if (nRet > 0)
            {
                //m_passScan = passportScanner;
                m_nPort = nRet;
                m_nType = nType;
                m_bOpen = true;
                if (m_nType == 2 && !string.Empty.Equals(m_strPassPath))
                {
                    SetDawainSavePath(m_strPassPath);
                }
            }
            return nRet;
        }
        //Scan 데이터 수신. Timeout 은 초단위
        public int scan(int nTimeout)
        {
            /*
            if (m_passScan == null)
                return -1;
                */
            if(!m_bOpen)
                return -1;
            int nRet = 0;
            Clear();
            clearMrzData();
            StringBuilder sbMrz1 = new StringBuilder(50);
            StringBuilder sbMrz2 = new StringBuilder(50);
            StringBuilder sbMrz3 = new StringBuilder(50);
            sbMrz1.Clear();
            sbMrz2.Clear();
            sbMrz3.Clear();

            nRet = Scan();
            if (nRet>=0)
            {
                nRet = ReceiveData(nTimeout);
                if (nRet > 0)
                {
                    GetMRZ1(sbMrz1);
                    GetMRZ2(sbMrz2);
                    GetMRZ3(sbMrz3);
                    m_strMRZ1 = sbMrz1.ToString().Trim();
                    if (m_strMRZ1.Length > 44)
                        m_strMRZ1 = m_strMRZ1.Substring(0, 44);
                    m_strMRZ2 = sbMrz2.ToString().Trim();
                    if (m_strMRZ2.Length > 44)
                        m_strMRZ2 = m_strMRZ2.Substring(0, 44);
                    m_strMRZ3 = sbMrz3.ToString().Trim();
                }
            }
            // nRet = m_passScan.recieveData();
            return nRet;
        }

        //Scan 명령 취소. 여권스캐너 종류에 따라 지원하지 않는 경우도 있다.
        public int scanCancel()
        {
            int nRet = 1;
            if (!m_bOpen)
                return nRet;

            //nRet = m_passScan.scanCancel();
            //nRet = ScanCancel();
            return nRet;
        }

        //Close
        public void close()
        {
            if (!m_bOpen)
                return;
            ClosePort();
            m_bOpen = false;
        }

        public int getPassportInfo(StringBuilder strPassdata)
        {
            return 1;
        }

        public string getMRZ1()
        {
            return m_strMRZ1;
        }

        public string getMRZ2()
        {
            return m_strMRZ2;
        }

        public string getMRZ3()
        {
            return m_strMRZ3;
        }


        public string GetPassportName()
        {
            string tmpName = m_strMRZ1.Substring(5, m_strMRZ2.Length-5).Replace("<<", "<").Replace("<", " ").Trim();
            return tmpName;
        }

        public string GetPassportFirstName()
        {
            string tempData = m_strMRZ1.Substring(5, m_strMRZ2.Length - 5);

            string tmpName = tempData.IndexOf("<<") >= 0 ? tempData.Substring(0, tempData.IndexOf("<<")) : tempData;
            tmpName = tmpName.Replace("<", " ").Trim();
            return tmpName;
        }
        public string GetPassportLastName()
        {
            string tempData = m_strMRZ1.Substring(5, m_strMRZ2.Length - 5);
            string tmpName = tempData.IndexOf("<<") >= 0 ? tempData.Substring(tempData.IndexOf("<<")) : "";
            tmpName = tmpName.Replace("<", " ").Trim();
            return tmpName;
        }

        public string GetPassportNo()
        {
            string passportNo = m_strMRZ2.Substring(0, 9).Replace("<", "");
            return passportNo;
        }

        public string GetNationality()
        {
            string country = m_strMRZ2.Substring(10, 3).Replace("<", "");
            return country;
        }

        public string GetBirthDate()
        {
            string birth = m_strMRZ2.Substring(13, 6).Replace("<", "");
            return birth;
        }
        public string GetExpireDate()
        {
            string valid = m_strMRZ2.Substring(21, 6).Replace("<", "");
            return valid;
        }
        public string GetSex()
        {
            string gender = m_strMRZ2.Substring(20, 1).Replace("<", "");
            return gender;
        }

        private void clearMrzData()
        {
            m_strMRZ1 = string.Empty;
            m_strMRZ2 = string.Empty;
            m_strMRZ3 = string.Empty;
        }
        public int SetDawainSavePath(string strPath)
        {
            int nRet = 0;
            if (m_bOpen && m_nType == 2) //여권스캔 기 오픈되어 있으면 반환
            {
                StringBuilder sb = new StringBuilder(strPath);
                SetSavePath(sb);
                nRet = 1;
                return nRet;
            }
            
            return nRet;
        }
    }
}
