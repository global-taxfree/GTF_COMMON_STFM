using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing.Printing;
using System.Drawing;
using GTF_Printer;

namespace GTF_Printer.GTF_JP
{
    partial class GTF_JPETRS
    {

        Dictionary<string, string> MapSummeryInfo = new Dictionary<string, string>();

        public int JPNPrintSummaryTicket(string total_slip_seq, string total_sum_amt, string total_tax_amt, string total_fee_amt, string total_refund_amt)
        {
            int retVal = 0;

            MapSummeryInfo.Add(SummaryInfo.SEQ, total_slip_seq);
            MapSummeryInfo.Add(SummaryInfo.SUM_AMT, total_sum_amt);
            MapSummeryInfo.Add(SummaryInfo.TAX_AMT, total_tax_amt);
            MapSummeryInfo.Add(SummaryInfo.FEE_AMT, total_fee_amt);
            MapSummeryInfo.Add(SummaryInfo.REFUND_AMT, total_refund_amt);

            UserPrintDocument printDocObj = new UserPrintDocument();
            if (printer_name != null && !string.Empty.Equals(printer_name.Trim()))
            {
                printDocObj.PrinterSettings.PrinterName = printer_name;
            }
            printDocObj.UserPrintPageEvent += new UserPrintDocument.UserPrintPageEventHandler(eventPrintSummary);
            printDocObj.Print();

            return retVal;
        }

        public void eventPrintSummary(object sender, PrintPageEventArgs e)
        {
            float yPos = 0;

            PrintSummary(e, ref yPos);

            e.HasMorePages = false;
        }

        public void PrintSummary(PrintPageEventArgs e, ref float yPos)
        {
            printFont = new Font("Meiryo", 10, FontStyle.Bold);

            StringFormat strFormat = new StringFormat();
            strFormat.Alignment = StringAlignment.Center;
            e.Graphics.DrawString(Properties.Resources.StringSlipType06, printFont, Brushes.Black, new Rectangle(0, (int)yPos, 284, 24), strFormat);
            strFormat.Alignment = StringAlignment.Near;

            yPos += printFont.GetHeight(e.Graphics) * 2;

            printFont = new Font("Meiryo", 8);
            // 총 Width : 284로 계산.
            // 합산 순번
            e.Graphics.DrawString(String.Format("{0} : {1}", Properties.Resources.StringSlipType06No, MapSummeryInfo[SummaryInfo.SEQ]), printFont, Brushes.Black, new Rectangle(0, (int)yPos, 284, 14), strFormat);
            // 합산 순번 데이터 출력...

            yPos += printFont.GetHeight(e.Graphics) * 3;

            // 총합계금액
            strFormat.Alignment = StringAlignment.Near;
            e.Graphics.DrawString(String.Format("{0} : ", Properties.Resources.StringGoodsDetails11), printFont, Brushes.Black, new Rectangle(0, (int)yPos, 142, 14), strFormat);
            strFormat.Alignment = StringAlignment.Far;
            e.Graphics.DrawString(String.Format("{0,13}", Convert.ToInt32(MapSummeryInfo[SummaryInfo.SUM_AMT]).ToString("#,##0")), printFont, Brushes.Black, new Rectangle(143, (int)yPos, 141, 14), strFormat);
            yPos += printFont.GetHeight(e.Graphics);


            // 총면세액
            strFormat.Alignment = StringAlignment.Near;
            e.Graphics.DrawString(String.Format("{0} : ", Properties.Resources.StringGoodsDetails14), printFont, Brushes.Black, new Rectangle(0, (int)yPos, 142, 14), strFormat);
            strFormat.Alignment = StringAlignment.Far;
            e.Graphics.DrawString(String.Format("{0,13}", Convert.ToInt32(MapSummeryInfo[SummaryInfo.TAX_AMT]).ToString("#,##0")), printFont, Brushes.Black, new Rectangle(143, (int)yPos, 141, 14), strFormat);
            yPos += printFont.GetHeight(e.Graphics);

            // 총수수료
            strFormat.Alignment = StringAlignment.Near;
            e.Graphics.DrawString(String.Format("{0} : ", Properties.Resources.StringGoodsDetails13), printFont, Brushes.Black, new Rectangle(0, (int)yPos, 142, 14), strFormat);
            strFormat.Alignment = StringAlignment.Far;
            e.Graphics.DrawString(String.Format("{0,13}", Convert.ToInt32(MapSummeryInfo[SummaryInfo.FEE_AMT]).ToString("#,##0")), printFont, Brushes.Black, new Rectangle(143, (int)yPos, 141, 14), strFormat);
            yPos += printFont.GetHeight(e.Graphics);


            // 총환급액
            strFormat.Alignment = StringAlignment.Near;
            e.Graphics.DrawString(String.Format("{0} : ", Properties.Resources.StringGoodsDetails12), printFont, Brushes.Black, new Rectangle(0, (int)yPos, 142, 14), strFormat);
            strFormat.Alignment = StringAlignment.Far;
            e.Graphics.DrawString(String.Format("{0,13}", Convert.ToInt32(MapSummeryInfo[SummaryInfo.REFUND_AMT]).ToString("#,##0")), printFont, Brushes.Black, new Rectangle(143, (int)yPos, 141, 14), strFormat);
            yPos += printFont.GetHeight(e.Graphics);

            yPos += printFont.GetHeight(e.Graphics) * 2;

            printFont = new Font("Arial", 6);
            strFormat.Alignment = StringAlignment.Far;
            e.Graphics.DrawString("-- END --", printFont, Brushes.Black, new Rectangle(0, (int)yPos, 284, 14), strFormat);
        }
    }
}
