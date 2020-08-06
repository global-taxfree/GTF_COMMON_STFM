using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using System.Data.OleDb;
using System.Data;
using log4net;
using System.Windows;
using Newtonsoft.Json.Linq;
using ADOX;
using System.IO;
namespace GTF_LocalDB
{
    public class GTF_LocalDBManager
    {
        OleDbConnection dbCon = null;
        OleDbConnection m_ObjCon
        {
            get {
                if (dbCon == null && !string.Empty.Equals(m_Provider))
                {
                    dbCon = new OleDbConnection();
                    dbCon.ConnectionString = m_Provider;
                }
                return dbCon;
            }
            set
            {
                dbCon = value;
            }
        }
        Boolean m_dbOpen = false;
        ILog m_logger = null;

        private string m_DBPassword = "gtfstfm12345";
        private static GTF_LocalDBManager instance;

        private string m_Provider = string.Empty;

        private static object sync_LocalDB = new Object();
        public static GTF_LocalDBManager Instance(ILog logger = null)
        {
            lock (sync_LocalDB)
            {
                if (instance == null)
                {
                    instance = new GTF_LocalDBManager(logger);

                }
                return instance;
            }
        }

        //생성자 private 처리
        private GTF_LocalDBManager(ILog logger = null)
        {
            m_logger = logger;
        }
        
        public Boolean dbOpen(string strFilePath)
        {
            Boolean bCreate = false;

            m_Provider = "Provider=Microsoft.ACE.OLEDB.12.0; Data Source=" + strFilePath + ";Jet OLEDB:Database Password =" + m_DBPassword + "; ";
            try
            {
                System.IO.FileInfo dbFile = new System.IO.FileInfo(strFilePath);
                if (!dbFile.Exists)
                {
                    //// DB 파일이름으로 .accdb 만들기. 
                    Catalog myCatalog = new Catalog();
                    //string strProvider = "Provider=Microsoft.ACE.OLEDB.12.0; Data Source=" + strFilePath + ";Jet OLEDB:Database Password =" + m_DBPassword + "; ";
                    //string strProvider = "Provider=Microsoft.Jet.OLEDB.4.0; Data Source=" + strFilePath + ";Jet OLEDB:Database Password =" + m_DBPassword + "; ";
                    try
                    {
                        myCatalog.Create(m_Provider);
                        bCreate = true;

                    }
                    catch (Exception ex)
                    {
                        //MessageBox.Show(ex.Message, "Make AccessDB Error");
                        Console.WriteLine(ex.Message);
                        return false;
                    }
                }
                else
                {
                    //로컬 DB 데이터 압축. 데이터 압축 해주지 않으면 로컬 DB 파일의 사이즈가 계속 커지게 된다.
                    //Table 을 삭제해도 로컬 DB 내에 복구를 위한 데이터는 남아있다. 데이터 압축을 해주면 복구용 데이터를 모두 삭제한다.
                    try {
                        //임시파일
                        string strTempFilePath = "C:/temp.accdb";
                        if (System.IO.File.Exists(strTempFilePath))
                        {
                            System.IO.File.Delete(strTempFilePath);
                        }

                        //MDB 파일 데이터 압축. JRO 라이브러리 로드가 필요하다.
                        //JRO.JetEngine ddd = new JRO.JetEngine();
                        //string temp1 = "Provider=Microsoft.Jet.OLEDB.4.0; Data Source=" + strFilePath + ";Jet OLEDB:Database Password =" + m_DBPassword + "; ";
                        //string temp2 = "Provider=Microsoft.Jet.OLEDB.4.0; Data Source=" + strTempFilePath + ";Jet OLEDB:Database Password =" + m_DBPassword + "; ";
                        //ddd.CompactDatabase(temp1, temp2);

                        //accdb 파일 데이터 압축
                        Microsoft.Office.Interop.Access.Dao.DBEngine objDbEngine = new Microsoft.Office.Interop.Access.Dao.DBEngine();
                        objDbEngine.CompactDatabase(strFilePath, strTempFilePath, null, null, ";pwd="+ m_DBPassword);//데이터 압축

                        if (System.IO.File.Exists(strTempFilePath))
                        {
                            System.IO.File.Delete(strFilePath);
                            System.IO.File.Move(strTempFilePath, strFilePath);
                        }
                    }
                    catch(Exception e)
                    {
                        Console.WriteLine(e.Message);
                    }
                }
                ;

                //mdb 파일
                //m_ObjCon.ConnectionString = @"Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + strFilePath + ";Jet OLEDB:Database Password =" + m_DBPassword + "; ";
                //accdb 파일
                //m_ObjCon.ConnectionString = @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source="+ strFilePath+ ";Jet OLEDB:Database Password =" + m_DBPassword + ";";
                


                //+ "Persist Security Info=False;"
                //+ "Database Password=gtf_stfm;";
                m_ObjCon.Open();
                m_dbOpen = true;
                if (bCreate)
                {
                    if(!createBasicTable())
                    {
                        m_ObjCon.Close();
                        m_ObjCon = null;
                        if (System.IO.File.Exists(strFilePath))
                        {
                            System.IO.File.Delete(strFilePath);
                        }
                        return false;
                    }
                }
            }
            catch(Exception e)
            {
                m_dbOpen = false;
                e.ToString();
            }
            return m_dbOpen;
        }

        public JArray getSlipList(string strUserID , string strTableName , string strStartDate , string strEndDate, string strSendFlag, string strStatus, string strSlipNO ="")
        {
            JArray arrRet = new JArray();
            //return arrRet;
            try {
                String Query = string.Empty;
                Query = String.Format("SELECT * FROM " + strTableName + " WHERE USERID= '"+ strUserID+"' ");

                if (!string.Empty.Equals(strSendFlag))
                {
                    Query += " AND SEND_FLAG = '" + strSendFlag + "'";
                }
                if (!string.Empty.Equals(strStatus))
                {
                    Query += " AND SLIP_STATUS_CODE = '" + strStatus + "'";
                }

                if (!string.Empty.Equals(strStartDate))
                {
                    Query += " AND REG_DTM >= '" + strStartDate + "'";
                }

                if (!string.Empty.Equals(strEndDate))
                {
                    Query += " AND REG_DTM <= '" + strEndDate + "'";
                }
                if (!string.Empty.Equals(strSlipNO))
                {
                    Query += " AND SLIP_NO = '" + strSlipNO + "'";
                }

                Query += " ORDER BY SALEDT DESC ";

                OleDbCommand OLECmd = new OleDbCommand(Query, m_ObjCon);
                OLECmd.CommandType = CommandType.Text;
                OleDbDataReader OLEReader = OLECmd.ExecuteReader(CommandBehavior.Default);

                JObject jsonRet = null;

                while (OLEReader.Read())
                {
                    jsonRet = new JObject();
                    for (int i = 0; i < OLEReader.FieldCount; i++)
                    {
                        jsonRet.Add(OLEReader.GetName(i), OLEReader[OLEReader.GetName(i)].ToString());
                    }
                    arrRet.Add(jsonRet);
                }

                OLECmd.Dispose();
                OLEReader.Dispose();
            }catch(Exception ex)
            {
                m_logger.Error(ex.StackTrace);
            }
            return arrRet;
        }

        public Boolean ExistsConfigData(string strKey )
        {
            string strRet = string.Empty;
            string strQuery = string.Empty;
            Boolean bExists = false;
            try {
                strQuery = "SELECT ITEM_VALUE FROM TML_CONFIG WHERE ITEM_KEY=@ITEM_KEY";
                OleDbCommand connCmd = new OleDbCommand(strQuery , m_ObjCon);
                connCmd.Parameters.Add(new OleDbParameter("@ITEM_KEY", strKey));
                OleDbDataReader oleData = connCmd.ExecuteReader(CommandBehavior.CloseConnection);
                while(oleData.Read())
                {
                    bExists = true;
                    strRet = oleData["ITEM_VALUE"].ToString();
                }
            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
            }

            return bExists;
        }

        public string selectConfigData(string strKey)
        {
            string strRet = string.Empty;
            string strQuery = string.Empty;
            try
            {
                strQuery = "SELECT ITEM_VALUE FROM TML_CONFIG WHERE ITEM_KEY=@ITEM_KEY";
                OleDbCommand connCmd = new OleDbCommand(strQuery, m_ObjCon);
                connCmd.Parameters.Add(new OleDbParameter("@ITEM_KEY", strKey));
                OleDbDataReader oleData = connCmd.ExecuteReader(CommandBehavior.CloseConnection);
                while (oleData.Read())
                {
                    strRet = oleData["ITEM_VALUE"].ToString();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            return strRet;
        }

        public string updateConfigData(string strKey, string strValue , string strDesc)
        {
            string strRet = string.Empty;
            String strQuery = string.Empty;
            try {
                //if (string.Empty.Equals(ExistsConfigData(strKey)))
                if (ExistsConfigData(strKey))
                {
                    strQuery = "UPDATE TML_CONFIG SET ITEM_DESC= '" + strDesc + "', ITEM_VALUE='" + strValue + "' WHERE ITEM_KEY = '" + strKey + "'";
                }
                else
                {
                    strQuery = "INSERT INTO TML_CONFIG(ITEM_KEY,ITEM_DESC,ITEM_VALUE)VALUES('" + strKey + "','" + strDesc + "','" + strValue + "')";
                }

                OleDbCommand cmd = new OleDbCommand(strQuery, m_ObjCon);
                cmd.ExecuteNonQuery();
                //cmd.Dispose();
                cmd = null;
            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
            }

            return strRet;
        }
        public string deleteConfigData(string strKey)
        {
            string strRet = string.Empty;
            return strRet;
        }

        public Boolean createBasicTable()
        {
            Boolean bRet = true;
            //1. 환경설정 저장 Table 생성
            try
            {
                String strQuery = string.Empty;
                strQuery = "CREATE TABLE TML_CONFIG " +
                "(ITEM_KEY varchar(20), ITEM_DESC varchar(100), ITEM_VALUE varchar(40), " +
                "CONSTRAINT TML_SETUP_PrimaryKey PRIMARY KEY (ITEM_KEY))";
                OleDbCommand connCmd = new OleDbCommand(strQuery, m_ObjCon);
                connCmd.ExecuteNonQuery();
                //connCmd.Dispose();
                connCmd = null;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                bRet = false;
            }
/*
            //2.refund slip Table 생성
            try
            {
                String strQuery = string.Empty;
                strQuery = "CREATE TABLE REFUNDSLIP (" +
                    "	MERCHANT_NO		MEMO , " +
                    "	SLIP_NO		MEMO , " +
                    "	TOTAL_COMM_SALE_AMT		MEMO , " +
                    "	TOTAL_COMM_TAX_AMT		MEMO , " +
                    "	TOTAL_COMM_REFUND_AMT		MEMO , " +
                    "	SALEDT		MEMO , " +
                    "	REFUNDDT		MEMO , " +
                    "	REGDT		MEMO , " +
                    "	MODDT		MEMO , " +
                    "	WORKERID		MEMO , " +
                    "	CUSTOMS_OUT_NO		MEMO , " +
                    "	OUTDT		MEMO , " +
                    "	REFUND_WAY_CODE		MEMO , " +
                    "	REFUND_CARDNO		MEMO , " +
                    "	REFUND_CARD_CODE		MEMO , " +
                    "	REFUND_EXPIRYDT		MEMO , " +
                    "	PUBLISHDT		MEMO , " +
                    "	SLIP_STATUS_CODE		MEMO , " +
                    "	TOTAL_COMM_FEE_AMT		MEMO , " +
                    "	REFUNDSLIPDESC		MEMO , " +
                    "	OUT_DIV_CODE		MEMO , " +
                    "	TML_ID		MEMO , " +
                    "	COMM		MEMO , " +
                    "	EXCOMM		MEMO , " +
                    "	TOTAL_EXCOMM_SALE_AMT		MEMO , " +
                    "	TOTAL_EXCOMM_TAX_AMT		MEMO , " +
                    "	TOTAL_EXCOMM_REFUND_AMT		MEMO , " +
                    "	TOTAL_EXCOMM_FEE_AMT		MEMO , " +
                    "	PASSPORT_SERIAL_NO		MEMO , " +
                    "	REFUND_STATUS_CODE		MEMO , " +
                    "	RETRY_CNT		MEMO , " +
                    "	REFUND_WORKERID		MEMO , " +
                    "	REFUND_MERCHANT_NO		MEMO , " +
                    "	REFUND_DESK_ID		MEMO , " +
                    "	VERIFYYN		MEMO , " +
                    "	VERIFYDT		MEMO , " +
                    "	VERIFY_WORKERID		MEMO , " +
                    "	TOTAL_SLIPSEQ		MEMO , " +
                    "	VOIDDT		MEMO , " +
                    "	VOID_WORKERID		MEMO , " +
                    "	TAX_PROC_TIME_CODE		MEMO , " +
                    "	TOTAL_EXCOMM_IN_TAX_SALE_AMT		MEMO , " +
                    "	TOTAL_COMM_IN_TAX_SALE_AMT		MEMO , " +
                    "	DELETE_YN		MEMO , " +
                    "	UNIKEY		MEMO , " +

                    "	BUYER_BIRTH		MEMO , " +
                    "	PASS_EXPIRYDT		MEMO , " +
                    "	BUYER_NAME		MEMO , " +
                    "	NATIONALITY_CODE		MEMO , " +
                    "	GENDER_CODE		MEMO , " +
                    "	ENTRY_PORT_CODE		MEMO , " +
                    "	RESIDENCE_NO		MEMO , " +
                    "	INPUT_WAY_CODE		MEMO , " +
                    "	ENTRYDT		MEMO , " +
                    "	PASSPORT_TYPE		MEMO , " 

                    +" CONSTRAINT REFUNDSLIP_PrimaryKey PRIMARY KEY (SLIP_NO))";
                OleDbCommand connCmd = new OleDbCommand(strQuery, m_ObjCon);
                connCmd.ExecuteNonQuery();
                //connCmd.Dispose();
                connCmd = null;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                bRet = false;
            }

            //3.Sales Goods Table 생성
            try
            {
                String strQuery = string.Empty;
                strQuery = "CREATE TABLE SALES_GOODS (" +
                "	SLIP_NO		MEMO , " +
                "	SALE_SEQ		MEMO , " +
                "	REC_NO		MEMO , " +
                "	GOODS_NAME		MEMO , " +
                "	GOODS_UNIT_PRICE		MEMO , " +
                "	GOODS_QTY		MEMO , " +
                "	GOODS_AMT		MEMO , " +
                "	TAX_AMT		MEMO , " +
                "	REFUND_AMT		MEMO , " +
                "	REGDT		MEMO , " +
                "	MODDT		MEMO , " +
                "	WORKERID		MEMO , " +
                "	GOODS_TAX_CODE		MEMO , " +
                "	GOODS_ITEMS_CODE		MEMO , " +
                "	FEE_AMT		MEMO , " +
                "	GOODS_GROUP_CODE		MEMO , " +
                "	GOODS_DIVISION		MEMO , " +
                "	TAX_PROC_TIME_CODE		MEMO , " +
                "	GOODS_IN_TAX_AMT		MEMO "+
                " )";
                //" CONSTRAINT SALES_GOODS_PrimaryKey PRIMARY KEY (ITEM_KEY))";
                OleDbCommand connCmd = new OleDbCommand(strQuery, m_ObjCon);
                connCmd.ExecuteNonQuery();
                //connCmd.Dispose();
                connCmd = null;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                bRet = false;
            }
            */
            //4. 채번 Table 생성
            try
            {
                String strQuery = string.Empty;
                strQuery = "CREATE TABLE ID_CREATION_RULE " +
                "( ITEM_KEY varchar(20), ITEM_DESC varchar(100), ITEM_VALUE varchar(40), " +
                "CONSTRAINT TML_SETUP_PrimaryKey PRIMARY KEY (ITEM_KEY))";
                OleDbCommand connCmd = new OleDbCommand(strQuery, m_ObjCon);
                connCmd.ExecuteNonQuery();
                //connCmd.Dispose();
                connCmd = null;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                bRet = false;
            }
            

            return bRet;
        }
        
        public Boolean createSlipTables()
        {
            Catalog cat = new Catalog();

            // Create a new table "Publisher" using ADOX ...
            Table table = new Table();
            table.Name = "REFUNDSLIP";
            cat.Tables.Append(table);
            // Add Column "PublisherID" with Autoincrement
            ADOX.Column col = new Column();
            col.Name = "PublisherID";
            col.ParentCatalog = cat;
            col.Type = ADOX.DataTypeEnum.adInteger;
            col.Properties["Nullable"].Value = false;
            col.Properties["AutoIncrement"].Value = true;
            table.Columns.Append(col);
            return true;
        }


        //table 생성 
        //jsonReq : table 컬럼명. 컬렴 데이터형은 MEMO 로 고정
        //strTableName : table 명
        public Boolean createTable(JObject jsonReq, string strTableName)
        {
            Boolean bRet = true;

            IList<string> keys = jsonReq.Properties().Select(p => p.Name).ToList();

            try
            {
                String strQuery = string.Empty;
                strQuery = "CREATE TABLE " + strTableName + " ( ";
                for (int i = 0; i < keys.Count; i++)
                {
                    //strQuery += keys[i] + " " + jsonReq[keys[i]].ToString() + ", "; //컬럼 데이터형을 가변적으로 주는것도 가능하나, 범용성을 위해 MEMO로 고정
                    strQuery += keys[i] + " MEMO ";

                    if (i != keys.Count -1)
                    {
                        strQuery += " , ";
                    }
                }
                //strQuery += " CONSTRAINT " + strTableName + "_PrimaryKey PRIMARY KEY (" + strKeyName + ") "; //PK 지정도 가능하나, 로컬 DB에서 유의미한 퍼포먼스 차이를 기대할 수 없음
                strQuery += " )";

                OleDbCommand connCmd = new OleDbCommand(strQuery, m_ObjCon);
                connCmd.ExecuteNonQuery();
                //connCmd.Dispose();
                connCmd = null;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                bRet = false;
            }
            return bRet;
        }

        //table 조회
        //strTableName : table 명
        //jsonWhere : where 조건
        public JArray SelectTable(string strTableName, JObject jsonWhere  = null)
        {
            JArray arrRet = new JArray();
            JObject jsonRet = null;

            try
            {
                IList<string> keys = null;
                string strQuery = " SELECT * FROM " + strTableName;
                if (jsonWhere != null && jsonWhere.Count > 0)
                {
                    strQuery += " WHERE " + Environment.NewLine + "";

                    keys = jsonWhere.Properties().Select(p => p.Name).ToList();

                    for (int i = 0; i < keys.Count; i++)
                    {
                        strQuery += " " + keys[i] + " = @" + keys[i];

                        if (i != (keys.Count - 1))
                            strQuery += " and ";
                        strQuery += " " + Environment.NewLine + " ";
                    }
                }

                OleDbCommand cmd = new OleDbCommand(strQuery, m_ObjCon);
                if (keys != null)
                {
                    for (int i = 0; i < keys.Count; i++)
                    {
                        cmd.Parameters.Add(new OleDbParameter("@" + keys[i], jsonWhere[keys[i]].ToString()));

                    }
                }

                OleDbDataReader oleData = cmd.ExecuteReader(CommandBehavior.CloseConnection);
            
                while (oleData.Read())
                {
                    jsonRet = new JObject();
                    for(int i=0; i <  oleData.FieldCount; i ++)
                    {
                        jsonRet.Add(oleData.GetName(i), oleData[oleData.GetName(i)].ToString());
                    }
                    arrRet.Add(jsonRet);
                }
            }

            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            return arrRet;
        }

        //table insert
        //jsonReq : 컬럼명 및 데이터
        //strTableName Data
        public Boolean InsertTable(JObject jsonReq, string strTableName)
        {
            try {
                IList<string> keys = jsonReq.Properties().Select(p => p.Name).ToList();

                string strQuery = "INSERT INTO " + strTableName + " ( ";
                for (int i = 0; i < keys.Count; i++)
                {
                    strQuery += " " + keys[i] + " ";
                    if (i != (keys.Count - 1))
                        strQuery += " , ";
                    strQuery += " " + Environment.NewLine + " ";
                }
                strQuery += " ) ";

                strQuery += " VALUES( ";

                for (int i = 0; i < keys.Count; i++)
                {
                    strQuery += " @" + keys[i] + " ";
                    if (i != (keys.Count - 1))
                        strQuery += " , ";
                    strQuery += " " + Environment.NewLine + " ";

                }
                strQuery += " ) ";

                OleDbCommand cmd = new OleDbCommand(strQuery, m_ObjCon);

                for (int i = 0; i < keys.Count; i++)
                {
                    cmd.Parameters.Add(new OleDbParameter("@" + keys[i], jsonReq[keys[i]].ToString()));
                    //if(System.Text.ASCIIEncoding.Unicode.GetByteCount(jsonReq[keys[i]].ToString()) > 256)
                    //{
                    //    Console.WriteLine("data is too large >> "+ keys[i]+":"+ jsonReq[keys[i]].ToString());
                    //}
                }
                cmd.ExecuteNonQuery();
            }catch(Exception e)
            {
                Console.WriteLine(e.Message);
                return false;
            }

            return true;
        }


        //table update
        //jsonReq : update 컬럼명 및 데이터. where 데이터도 포함
        //jsonWhereKey : where 컬럼명
        //strTableName : table 명 
        public Boolean updateTable(JObject jsonReq, JObject jsonWhereKey, string strTableName)
        {
            Boolean bRet = true;
            try {
                IList<string> keys = jsonReq.Properties().Select(p => p.Name).ToList();

                IList<string> whereKeys = jsonWhereKey.Properties().Select(p => p.Name).ToList();

                string strQuery = "UPDATE " + strTableName + " SET " + Environment.NewLine + "";
                for (int i = 0; i < keys.Count; i++)
                {
                    strQuery += " " + keys[i] + " = @" + keys[i];

                    if (i != (keys.Count - 1))
                        strQuery += " , ";
                    strQuery += " " + Environment.NewLine+" ";
                }

                strQuery += " WHERE " + Environment.NewLine + " ";
                for (int i = 0; i < whereKeys.Count; i++)
                {
                    strQuery += " " + whereKeys[i] + " = @" + whereKeys[i];
                    if (i != (whereKeys.Count - 1))
                        strQuery += " and ";
                    strQuery += " " + Environment.NewLine + " ";
                }

                OleDbCommand cmd = new OleDbCommand(strQuery, m_ObjCon);

                for (int i = 0; i < keys.Count; i++)
                {
                    cmd.Parameters.Add(new OleDbParameter("@" + keys[i], jsonReq[keys[i]].ToString()));
                }
                for (int i = 0; i < whereKeys.Count; i++)
                {
                    cmd.Parameters.Add(new OleDbParameter("@" + whereKeys[i], jsonWhereKey[whereKeys[i]].ToString()));
                }


                int updated_records_count = cmd.ExecuteNonQuery();
                if(updated_records_count>0)
                {
                    bRet = true;
                }
                else
                {
                    bRet = false;
                }
            }catch(Exception e)
            {
                Console.WriteLine(e.Message);
                bRet = false;
            }

            return bRet;
        }

        //table 삭제
        public void DropTable(string strTableName)
        {

            bool tableExists = false;

            DataTable dt = m_ObjCon.GetSchema("tables");

            foreach (DataRow row in dt.Rows)
            {
                if (strTableName.Equals(row["TABLE_NAME"].ToString() ) )
                {
                    tableExists = true;
                    break;
                }
            }

            //table 있으면
            if (tableExists)
            {
                //table 데이터 삭제
                OleDbCommand cmd = new OleDbCommand(string.Format("DELETE FROM {0}", strTableName), m_ObjCon);
                cmd.ExecuteNonQuery();

                //table 삭제
                OleDbCommand cmd2 = new OleDbCommand(string.Format("DROP TABLE {0}", strTableName), m_ObjCon);
                cmd2.ExecuteNonQuery();
                //MessageBox.Show("Table deleted");
                
            }
            else
            {
                //MessageBox.Show(string.Format("Table {0} not exists", tableToDelete));
            }
        }

        public Boolean ExitsTable(string strTableName)
        {

            Boolean tableExists = false;

            DataTable dt = m_ObjCon.GetSchema("tables");

            foreach (DataRow row in dt.Rows)
            {
                if (strTableName.Equals(row["TABLE_NAME"].ToString()))
                {
                    tableExists = true;
                    break;
                }
            }
            return tableExists;
        }
        //UpiBinSelect 
        public JArray SelectUpiBinTable(string strTableName, string strCardNo)
        {
            JArray arrRet = new JArray();
            JObject jsonRet = null;
            try
            {
                string strQuery = " SELECT * FROM BIN WHERE '" + strCardNo + "' LIKE BIN + '%'  ";
                strQuery += " AND PAN_LENGTH ='" + strCardNo.Length + "' ";
                OleDbCommand cmd = new OleDbCommand(strQuery, m_ObjCon);
                OleDbDataReader oleData = cmd.ExecuteReader(CommandBehavior.CloseConnection);

                while (oleData.Read())
                {
                    jsonRet = new JObject();
                    for (int i = 0; i < oleData.FieldCount; i++)
                    {
                        jsonRet.Add(oleData.GetName(i), oleData[oleData.GetName(i)].ToString());
                    }
                    arrRet.Add(jsonRet);
                }
            }

            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            return arrRet;
        }

        public void DBInit()
        {
            DataTable dt = m_ObjCon.GetSchema("tables");

            foreach (DataRow row in dt.Rows)
            {
                //table 데이터 삭제
                OleDbCommand cmd = new OleDbCommand(string.Format("DELETE FROM {0}", row["TABLE_NAME"].ToString()), m_ObjCon);
                cmd.ExecuteNonQuery();

                //table 삭제
                OleDbCommand cmd2 = new OleDbCommand(string.Format("DROP TABLE {0}", row["TABLE_NAME"].ToString()), m_ObjCon);
                cmd2.ExecuteNonQuery();
            }
        }
    }
}
