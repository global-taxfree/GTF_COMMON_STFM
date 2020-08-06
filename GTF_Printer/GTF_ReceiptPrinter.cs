using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Printing;
using log4net;
using OposPOSPrinter_CCO;
using System.Windows.Forms;
using GTF_Printer.GTF_JP;

namespace GTF_Printer
{
    public class GTF_ReceiptPrinter
    {
        const long OPOS_SUCCESS = 0;
        const long OPOS_E_CLOSED = 101;
        const long OPOS_E_CLAIMED = 102;
        const long OPOS_E_NOTCLAIMED = 103;
        const long OPOS_E_NOSERVICE = 104;
        const long OPOS_E_DISABLED = 105;
        const long OPOS_E_ILLEGAL = 106;
        const long OPOS_E_NOHARDWARE = 107;
        const long OPOS_E_OFFLINE = 108;
        const long OPOS_E_NOEXIST = 109;
        const long OPOS_E_EXISTS = 110;
        const long OPOS_E_FAILURE = 111;
        const long OPOS_E_TIMEOUT = 112;
        const long OPOS_E_BUSY = 113;
        const long OPOS_E_EXTENDED = 114;
        const int PTR_S_RECEIPT = 2;


        const int PTR_BC_CENTER = -2;
        const int PTR_BCS_Code128 = 110;

        const int PTR_BC_TEXT_NONE = -11;
        const int PTR_BC_TEXT_ABOVE = -12;
        const int PTR_BC_TEXT_BELOW = -13;


        const int PTR_BMT_BMP = 1;
        const int PTR_BM_LEFT = -1;
        const int PTR_BM_CENTER = -2;
        const int PTR_BM_RIGHT = -3;

        const int PTR_BM_ASIS = -11;  // One pixel per printer dot

        OposPOSPrinter_CCO.OPOSPOSPrinter axOPOSPOSPrinter1 = null;
        
        ILog m_logger = null;

        PrintDocument m_printDoc = null;
        Control m_parent = null;
        string m_PrinterName = "";

        //생성자
        public GTF_ReceiptPrinter(ILog logger = null, Control parent =null)
        {
            m_printDoc = new PrintDocument();
            m_logger = logger;
            m_parent = parent;
            if (m_logger == null)
                m_logger = LogManager.GetLogger("");
        }

        public void setPrinter(string strPrinterName)
        {
            m_PrinterName = strPrinterName;
            m_printDoc.PrinterSettings.PrinterName = strPrinterName;
        }

        public void PrintSlip_jp()
        {
            
        }

        public void PrintSlip_sg(string strPrinterName = "SRP-350III")
        {
            string CRLF = "\r\n";
            string ESC = "\x1b";
            string strOutputData;
            strOutputData = ESC + "|cA" + ESC + "|2C" + ESC + "|bC" + "* Gtf Test Slip *" + CRLF + CRLF;
            strOutputData = strOutputData + ESC + "|N" + "   3000 Spring Street, Rancho," + CRLF;
            strOutputData = strOutputData + "   California 10093," + CRLF;
            strOutputData = strOutputData + "   Tel) 858-519-3698 Fax) 3852" + CRLF + CRLF;
            strOutputData = strOutputData + "Orange Juice                   5.00" + CRLF;
            strOutputData = strOutputData + "6 Bufalo Wing                 24.00" + CRLF;
            strOutputData = strOutputData + "Potato Skin                   12.00" + CRLF;
            strOutputData = strOutputData + ESC + "|bC" + ESC + "|2rC" + "Subtotal                      41.00" + CRLF + CRLF;
            strOutputData = strOutputData + ESC + "|N" + "Tax 6%                         2.46" + CRLF;
            strOutputData = strOutputData + ESC + "|bC" + ESC + "|2rC" + "Member Discount                2.30" + CRLF + CRLF;
            strOutputData = strOutputData + ESC + "|N" + ESC + "|bC" + "Cash                         100.00" + CRLF;
            strOutputData = strOutputData + ESC + "|N" + "Amt. Paid                     41.16" + CRLF;
            strOutputData = strOutputData + ESC + "|bC" + ESC + "|2rC" + "Change Due                    58.84" + CRLF + CRLF;
            strOutputData = strOutputData + ESC + "|N" + "Member Number : 452331949" + CRLF + CRLF;
            strOutputData = strOutputData + ESC + "|bC" + ESC + "|cA" + "Have a nice day !" + CRLF + CRLF + CRLF;
            strOutputData = strOutputData + ESC + "|N" + ESC + "|cA" + "Sale Date : 2017/06/02" + CRLF;
            strOutputData = strOutputData + ESC + "|N" + ESC + "|cA" + "Time : 11:23:45" + CRLF + CRLF + CRLF + CRLF + CRLF + CRLF + CRLF + CRLF + CRLF + CRLF;
            PrintOPOS(strPrinterName, strOutputData);
        }

        public void PrintSlip_kr()
        {

        }

        public void PrintSlip_ja(string docid, string retailer, string goods, string tourist, string adsinfo, Boolean bPreview = true)
        {
            GTF_JPETRS dd = new GTF_JPETRS();
            dd.printer_name = m_PrinterName;
            if (bPreview)
            {
                dd.JPNPrintPreview(docid, retailer, goods, tourist, adsinfo);
            }
            else
            {
                dd.JPNPrintTicket(docid, retailer, goods, tourist, adsinfo);
            }
            dd = null;
        }

        public void PrintSlip_ja_summary(string total_slip_seq, string total_sum_amt, string total_tax_amt, string total_fee_amt, string total_refund_amt)
        {
            GTF_JPETRS dd = new GTF_JPETRS();
            dd.printer_name = m_PrinterName;
            dd.JPNPrintSummaryTicket(total_slip_seq, total_sum_amt, total_tax_amt, total_fee_amt, total_refund_amt);
            dd = null;
        }

        public void PrintSlip_Test()
        {
            m_printDoc.PrintPage += new PrintPageEventHandler(printDoc_PrintPage);
            m_printDoc.Print();
        }

        private void printDoc_PrintPage(Object sender, PrintPageEventArgs e)
        {
            StringBuilder sbToPrint = new StringBuilder();
            sbToPrint.Append("GTF Slip Test Printing 1\n");
            sbToPrint.Append("GTF Slip Test Printing 2\n");
            sbToPrint.Append("GTF Slip Test Printing 3\n");
            sbToPrint.Append("GTF Slip Test Printing 4\n");
            Font printFont = new Font("Courier New", 12);
            e.Graphics.DrawString(sbToPrint.ToString(), printFont, Brushes.Black, 0, 0);
            printFont.Dispose();
        }

        public Boolean OpenOPOS(string strPrinterName)
        {

            Boolean bRet = true;
            string strErr = "";
            try
            {
                if (axOPOSPOSPrinter1 == null)
                    axOPOSPOSPrinter1 = new OposPOSPrinter_CCO.OPOSPOSPrinter();
                int lRet = axOPOSPOSPrinter1.Open(strPrinterName); // LDN

                if (lRet == OPOS_SUCCESS)
                {
                    lRet = axOPOSPOSPrinter1.ClaimDevice(0);
                    if (lRet == OPOS_SUCCESS)
                    {
                        axOPOSPOSPrinter1.DeviceEnabled = true;
                        axOPOSPOSPrinter1.FlagWhenIdle = true;

                        string strCharLists = axOPOSPOSPrinter1.RecBarCodeRotationList;
                        //StrCharLists is pair (Font A, Font B)//F312 48,64
                        //m_ctlPosPrinter1.SetRecLineChars(48); //select a Font A
                        //axOPOSPOSPrinter1.RecLineChars = 42; //select a Font B
                        axOPOSPOSPrinter1.RecLineChars = 64; //select a Font B
                        axOPOSPOSPrinter1.AsyncMode = false;
                    }
                    else {
                        strErr = "OPOSPrinter Claim Error : " + axOPOSPOSPrinter1.ErrorString;
                        //AfxMessageBox(strErr);
                        // return FALSE;
                    }
                    
                }
                else {
                    strErr = "OPOSPrinter Open Error : " + axOPOSPOSPrinter1.ErrorString;
                }
            }
            catch (Exception e)
            {
                if (axOPOSPOSPrinter1 != null)
                    strErr = "OPOSPrinter Open Error : " + axOPOSPOSPrinter1.ErrorString;
                else
                    strErr = "OPOSPrinter is null Error :" + e.Message;
            }
            return bRet;
        }
        public Boolean PrintOPOS_Text(string strData)
        {
            Boolean bRet = true;
            string strErr = "";
            try
            {
                if (axOPOSPOSPrinter1 != null)
                {
                    axOPOSPOSPrinter1.PrintNormal(PTR_S_RECEIPT, strData);
                }
                
            }catch(Exception e)
            {
                bRet = false;
            }
            return bRet;
        }
        public void font_change_Big()
        {
            if (axOPOSPOSPrinter1 != null)
            {
                axOPOSPOSPrinter1.RecLineChars = 42 ; //select a Font B
            }
        }

        public void font_change_Small()
        {
            if (axOPOSPOSPrinter1 != null)
            {
                axOPOSPOSPrinter1.RecLineChars = 64; //select a Font A
            }
        }

        public Boolean PrintOPOS_BMP( string strPath)
        {
            Boolean bRet = true;
            string strErr = "";
            try
            {
                if (axOPOSPOSPrinter1 != null)
                {
                    axOPOSPOSPrinter1.PrintBitmap(PTR_S_RECEIPT, strPath, PTR_BM_ASIS, PTR_BM_CENTER);
                }

            }
            catch (Exception e)
            {
                bRet = false;
            }
            return bRet;
        }

        public Boolean CusPaper()
        {
            Boolean bRet = true;
            string strErr = "";
            try
            {
                if (axOPOSPOSPrinter1 != null)
                {
                    axOPOSPOSPrinter1.CutPaper(95);
                }


            }
            catch (Exception e)
            {
                bRet = false;
            }
            return bRet;
        }

        public Boolean CloseOPOS()
        {
            Boolean bRet = true;
            string strErr = "";
            try
            {
                if (axOPOSPOSPrinter1 != null)
                {
                    axOPOSPOSPrinter1.Close();
                }
                

            }
            catch (Exception e)
            {
                bRet = false;
            }
            return bRet;
        }
        


        public int JPNPrintTicket(string docid, string retailer, string tourist, string goods, string adsinfo)
        {
            return 0;
        }

        public Boolean PrintOPOS(string strPrinterName, string strData)
        {
            Boolean bRet = true;
            string strErr = "";
            try
            {
                if (axOPOSPOSPrinter1 == null)
                    axOPOSPOSPrinter1 = new OposPOSPrinter_CCO.OPOSPOSPrinter();
                int lRet = axOPOSPOSPrinter1.Open(strPrinterName); // LDN

                if (lRet == OPOS_SUCCESS)
                {
                    lRet = axOPOSPOSPrinter1.ClaimDevice(0);
                    if (lRet == OPOS_SUCCESS)
                    {
                        axOPOSPOSPrinter1.DeviceEnabled = true;
                        axOPOSPOSPrinter1.FlagWhenIdle = true;

                        string strCharLists = axOPOSPOSPrinter1.RecBarCodeRotationList;
                        //StrCharLists is pair (Font A, Font B)//F312 48,64
                        //m_ctlPosPrinter1.SetRecLineChars(48); //select a Font A
                        axOPOSPOSPrinter1.RecLineChars = 64; //select a Font B

                        axOPOSPOSPrinter1.AsyncMode = false;
                        axOPOSPOSPrinter1.PrintNormal(PTR_S_RECEIPT, strData);

                        axOPOSPOSPrinter1.CutPaper(95);
                    }
                    else {
                        strErr = "OPOSPrinter Claim Error : " + axOPOSPOSPrinter1.ErrorString;
                        //AfxMessageBox(strErr);
                        // return FALSE;
                    }
                    axOPOSPOSPrinter1.Close();
                }
                else {
                    strErr = "OPOSPrinter Open Error : " + axOPOSPOSPrinter1.ErrorString;
                    // AfxMessageBox(strErr);
                    //return FALSE;
                }
            }
            catch (Exception e)
            {
                if (axOPOSPOSPrinter1 != null)
                    strErr = "OPOSPrinter Open Error : " + axOPOSPOSPrinter1.ErrorString;
                else
                    strErr = "OPOSPrinter is null Error :" + e.Message;
            }
            return bRet;
        }

    }

    
}
