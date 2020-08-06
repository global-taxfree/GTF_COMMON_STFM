using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

using System.Drawing.Printing;
using System.Drawing;

using GTF_Printer;

namespace GTF_Printer.GTF_JP
{
    partial class GTF_JPETRS
    {
        public int JPNPrintPreview(string docid, string retailer, string goods, string tourist, string adsinfo)
        {
            buyer_birth    ="19100101";
            buyer_name= "GTFPASS TEST";
            buyer_no  = "";
            entry_date ="20150909";
            entry_port ="";
            gender_code  =  "M";
            nationality_code =  "CHN";
            pass_expirydt  ="20300101";
            passport_serial_no ="E9999999";
            passport_type  ="01";
            print_count = 1;
            residence_name ="00000001";
            time_out = 20;

            int nRet = 0;

            m_docid = docid;
            m_retailer = retailer;
            m_tourist = tourist;
            m_goods = goods;
            m_adsinfo = adsinfo;
            
            setClearParseMap();
            
            try
            {
                if (!ParseParam(docid, retailer, tourist, goods, adsinfo))
                {
                    nRet = -1;
                    return nRet;
                }

                PrintPreviewForm PreviewDlg = new PrintPreviewForm(this);
                if (DialogResult.OK == PreviewDlg.ShowDialog())
                {
                    nRet = 1;
                }
                else
                    nRet = -1;
                    
            }
            catch (Exception e)
            {
                MessageBox.Show(null, e.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                nRet = -1;
            }

            return nRet;
        }
    }
}
