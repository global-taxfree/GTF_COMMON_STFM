using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing.Printing;

using System.Drawing;
using System.Resources;
using ZXing;
using ZXing.Common;
using ZXing.Rendering;
using System.Reflection;
using System.Globalization;
using System.Threading;

namespace GTF_Printer.GTF_JP
{
    partial class GTF_JPETRS
    {
        public int SlipType = 0;
        public const int SlipType01 = 1;    // 수출면세물품구입기록표
        public const int SlipType02 = 2;    // 면세물품구매자확약서
        public const int SlipType03 = 3;    // 수출면세물품구입기록표(보관용)
        public const int SlipType04 = 4;    // 구입자보관용
        public const int SlipType05 = 5;    // 수출물품판매자보관용        

        public string m_docid;
        public string m_retailer;
        public string m_tourist;
        public string m_goods;
        public string m_adsinfo;
        public string m_refundwayinfo;

        public Dictionary<string, object> MapDocid = new Dictionary<string, object>();
        public Dictionary<string, string> MapRetailer = new Dictionary<string, string>();
        public Dictionary<string, string> MapTourist = new Dictionary<string, string>();
        public Dictionary<string, object> MapGoods = new Dictionary<string, object>();
        public Dictionary<string, object> MapAdsInfo = new Dictionary<string, object>();
        public Dictionary<string, object> MapRefundInfo = new Dictionary<string, object>();

        StringFormat stringFormat = new StringFormat();
        Font printFont = new Font("Meiryo", 8);
        Font addTextFont = new Font("Meiryo", 9);

        public int JPNPrintTicket(string docid, string retailer, string goods, string tourist, string adsinfo)
        {
            m_docid = docid;
            m_retailer = retailer;
            m_tourist = tourist;
            m_goods = goods;
            m_adsinfo = adsinfo;
            setClearParseMap();

            int nRet = 0;

            try
            {
                if (!ParseParam(docid, retailer, tourist, goods, adsinfo))
                {
                    nRet = -1;
                    return nRet;
                }

                List<int> PublishTypeList = (List<int>)MapDocid[DocID.PublishType];
                foreach (int Type in PublishTypeList)
                {
                    SlipType = Type;
                    UserPrintDocument printDocObj = new UserPrintDocument();
                    printDocObj.UserPrintPageEvent += new UserPrintDocument.UserPrintPageEventHandler(eventPrintDoc);
                    if (printer_name != null && !string.Empty.Equals(printer_name.Trim()))
                    {
                        printDocObj.PrinterSettings.PrinterName = printer_name;
                    }
                    printDocObj.Print();
                }

            }
            catch (Exception e)
            {
                MessageBox.Show(null, e.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                nRet = -1;
            }

            return nRet;
        }

        public void eventPrintDoc(object sender, PrintPageEventArgs e)
        {
            float yPos = 0;

            PrintHeader(e, ref yPos, SlipType);

            PrintRetailer(e, ref yPos, SlipType);

            PrintGoodsDetails(e, ref yPos, SlipType);

            PrintRefundDetails(e, ref yPos);

            PrintTouristDetails(e, ref yPos);

            PrintRefundSerialInfo(e, ref yPos, SlipType);

            PrintFooter(e, ref yPos, SlipType);

            e.HasMorePages = false;
        }

        public void PrintHeader(PrintPageEventArgs e, ref float yPos, int type)
        {
            try
            {
                // 1. Jpn Logo
                PrintJPNLogo(e, ref yPos);

                // 2. Slip Title (Republish)
                PrintSlipTitle(e, ref yPos, type);

                // print Republish
                if (Convert.ToInt32(MapDocid[DocID.RePublish]) > 0)
                    PrintRePublish(e, ref yPos, type);

                // 3. Corp Logo
                PrintCorpLogo(e, ref yPos);

                // 4. Barcode            
                PrintBarcode(e, ref yPos);

                // Add Margin
                yPos += 70;

                // 5. Slip No.
                printFont = new Font("Meiryo", 7);
                e.Graphics.DrawString(String.Format("{0} : {1}", getSlipText(Properties.ResourceString.SlipNo), MapDocid[DocID.SlipNo]), printFont, Brushes.Black, 0, yPos, stringFormat);
                yPos += printFont.GetHeight(e.Graphics);
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(null, ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
        }

        public void PrintFooter(PrintPageEventArgs e, ref float yPos, int type)
        {
            try
            {
                int printLineHeight = 10;

                printFont = new Font("Meiryo", 6);
                switch (type)
                {
                    case SlipType01: // 수출면세물품구입기록표                    
                        {
                            PrintLine(e, ref yPos);
                            yPos += printLineHeight;

                            printFont = new Font("Meiryo", 6);

                            Rectangle rect = new Rectangle(0, (int)yPos, 284, 36);
                            e.Graphics.DrawString(Properties.Resources.StringSlipType01Desc01, printFont, Brushes.Black, rect);
                            e.Graphics.DrawRectangle(Pens.White, Rectangle.Round(rect));
                            yPos += printFont.GetHeight(e.Graphics) + 26;

                            rect = new Rectangle(0, (int)yPos, 284, 24);
                            e.Graphics.DrawString(Properties.Resources.StringSlipType01Desc02, printFont, Brushes.Black, rect);
                            e.Graphics.DrawRectangle(Pens.White, Rectangle.Round(rect));
                            yPos += printFont.GetHeight(e.Graphics) + 14;

                            rect = new Rectangle(0, (int)yPos, 284, 36);
                            e.Graphics.DrawString(Properties.Resources.StringSlipType01Desc03, printFont, Brushes.Black, rect);
                            e.Graphics.DrawRectangle(Pens.White, Rectangle.Round(rect));
                            yPos += printFont.GetHeight(e.Graphics) + 26;

                            rect = new Rectangle(0, (int)yPos, 284, 60);
                            e.Graphics.DrawString(Properties.Resources.StringSlipType01Desc04, printFont, Brushes.Black, rect);
                            e.Graphics.DrawRectangle(Pens.White, Rectangle.Round(rect));
                            yPos += printFont.GetHeight(e.Graphics) + 50;

                            yPos += printLineHeight;

                            // 국가별 출력 구분
                            rect = new Rectangle(0, (int)yPos, 284, getRectHeight(Properties.ResourceString.SlipType01Desc01));
                            e.Graphics.DrawString(getLangString(Properties.ResourceString.SlipType01Desc01), printFont, Brushes.Black, rect);
                            e.Graphics.DrawRectangle(Pens.White, Rectangle.Round(rect));
                            yPos += printFont.GetHeight(e.Graphics) + getRectHeight(Properties.ResourceString.SlipType01Desc01) - printLineHeight;

                            rect = new Rectangle(0, (int)yPos, 284, getRectHeight(Properties.ResourceString.SlipType01Desc02));
                            e.Graphics.DrawString(getLangString(Properties.ResourceString.SlipType01Desc02), printFont, Brushes.Black, rect);
                            e.Graphics.DrawRectangle(Pens.White, Rectangle.Round(rect));
                            yPos += printFont.GetHeight(e.Graphics) + getRectHeight(Properties.ResourceString.SlipType01Desc02) - printLineHeight;

                            rect = new Rectangle(0, (int)yPos, 284, getRectHeight(Properties.ResourceString.SlipType01Desc03));
                            e.Graphics.DrawString(getLangString(Properties.ResourceString.SlipType01Desc03), printFont, Brushes.Black, rect);
                            e.Graphics.DrawRectangle(Pens.White, Rectangle.Round(rect));
                            yPos += printFont.GetHeight(e.Graphics) + getRectHeight(Properties.ResourceString.SlipType01Desc03) - printLineHeight;

                            rect = new Rectangle(0, (int)yPos, 284, getRectHeight(Properties.ResourceString.SlipType01Desc04) + 1);
                            e.Graphics.DrawString(getLangString(Properties.ResourceString.SlipType01Desc04), printFont, Brushes.Black, rect);
                            e.Graphics.DrawRectangle(Pens.White, Rectangle.Round(rect));
                            yPos += printFont.GetHeight(e.Graphics) + getRectHeight(Properties.ResourceString.SlipType01Desc04) - printLineHeight;

                            yPos += printLineHeight;
                        }
                        break;
                    case SlipType02: // 면세물품구매자확약서
                        {
                            PrintLine(e, ref yPos);

                            printFont = new Font("Meiryo", 6);

                            Rectangle rect = new Rectangle(0, (int)yPos, 284, 24);
                            e.Graphics.DrawString(Properties.Resources.StringSlipType02Desc01, printFont, Brushes.Black, rect);
                            e.Graphics.DrawRectangle(Pens.White, Rectangle.Round(rect));
                            yPos += printFont.GetHeight(e.Graphics) + 14;

                            // 국가별 출력 구분
                            rect = new Rectangle(0, (int)yPos, 284, getRectHeight(Properties.ResourceString.SlipType02Desc01));
                            e.Graphics.DrawString(getLangString(Properties.ResourceString.SlipType02Desc01), printFont, Brushes.Black, rect);
                            e.Graphics.DrawRectangle(Pens.White, Rectangle.Round(rect));
                            yPos += printFont.GetHeight(e.Graphics) + getRectHeight(Properties.ResourceString.SlipType02Desc01) - printLineHeight;

                            yPos += printLineHeight * 2;

                            rect = new Rectangle(0, (int)yPos, 284, 24);
                            e.Graphics.DrawString(Properties.Resources.StringSlipType02Desc02, printFont, Brushes.Black, rect);
                            e.Graphics.DrawRectangle(Pens.White, Rectangle.Round(rect));
                            yPos += printFont.GetHeight(e.Graphics) + 14;

                            // 국가별 출력 구분
                            rect = new Rectangle(0, (int)yPos, 284, getRectHeight(Properties.ResourceString.SlipType02Desc02));
                            e.Graphics.DrawString(getLangString(Properties.ResourceString.SlipType02Desc02), printFont, Brushes.Black, rect);
                            e.Graphics.DrawRectangle(Pens.White, Rectangle.Round(rect));
                            yPos += printFont.GetHeight(e.Graphics) + getRectHeight(Properties.ResourceString.SlipType02Desc02) - printLineHeight;

                            yPos += printLineHeight * 5;

                            StringFormat strFormat = new StringFormat();
                            strFormat.Alignment = StringAlignment.Far;

                            e.Graphics.DrawString(getLangString(Properties.ResourceString.Signature), new Font("Consolas", 8), Brushes.Black, new Rectangle(0, (int)yPos, 284, 16), strFormat);
                            yPos += printFont.GetHeight(e.Graphics);
                        }
                        break;
                    case SlipType03: // 수출면세물품구입기록표(보관용)
                        {
                            PrintLine(e, ref yPos);
                            yPos += printLineHeight;

                            printFont = new Font("Meiryo", 6);

                            Rectangle rect = new Rectangle(0, (int)yPos, 284, 36);
                            e.Graphics.DrawString(Properties.Resources.StringSlipType01Desc01, printFont, Brushes.Black, rect);
                            e.Graphics.DrawRectangle(Pens.White, Rectangle.Round(rect));
                            yPos += printFont.GetHeight(e.Graphics) + 26;

                            rect = new Rectangle(0, (int)yPos, 284, 24);
                            e.Graphics.DrawString(Properties.Resources.StringSlipType01Desc02, printFont, Brushes.Black, rect);
                            e.Graphics.DrawRectangle(Pens.White, Rectangle.Round(rect));
                            yPos += printFont.GetHeight(e.Graphics) + 14;

                            rect = new Rectangle(0, (int)yPos, 284, 36);
                            e.Graphics.DrawString(Properties.Resources.StringSlipType01Desc03, printFont, Brushes.Black, rect);
                            e.Graphics.DrawRectangle(Pens.White, Rectangle.Round(rect));
                            yPos += printFont.GetHeight(e.Graphics) + 26;

                            rect = new Rectangle(0, (int)yPos, 284, 60);
                            e.Graphics.DrawString(Properties.Resources.StringSlipType01Desc04, printFont, Brushes.Black, rect);
                            e.Graphics.DrawRectangle(Pens.White, Rectangle.Round(rect));
                            yPos += printFont.GetHeight(e.Graphics) + 50;

                            yPos += printLineHeight;

                            // 국가별 출력 구분
                            rect = new Rectangle(0, (int)yPos, 284, getRectHeight(Properties.ResourceString.SlipType01Desc01));
                            e.Graphics.DrawString(getLangString(Properties.ResourceString.SlipType01Desc01), printFont, Brushes.Black, rect);
                            e.Graphics.DrawRectangle(Pens.White, Rectangle.Round(rect));
                            yPos += printFont.GetHeight(e.Graphics) + getRectHeight(Properties.ResourceString.SlipType01Desc01) - printLineHeight;

                            rect = new Rectangle(0, (int)yPos, 284, getRectHeight(Properties.ResourceString.SlipType01Desc02));
                            e.Graphics.DrawString(getLangString(Properties.ResourceString.SlipType01Desc02), printFont, Brushes.Black, rect);
                            e.Graphics.DrawRectangle(Pens.White, Rectangle.Round(rect));
                            yPos += printFont.GetHeight(e.Graphics) + getRectHeight(Properties.ResourceString.SlipType01Desc02) - printLineHeight;

                            rect = new Rectangle(0, (int)yPos, 284, getRectHeight(Properties.ResourceString.SlipType01Desc03));
                            e.Graphics.DrawString(getLangString(Properties.ResourceString.SlipType01Desc03), printFont, Brushes.Black, rect);
                            e.Graphics.DrawRectangle(Pens.White, Rectangle.Round(rect));
                            yPos += printFont.GetHeight(e.Graphics) + getRectHeight(Properties.ResourceString.SlipType01Desc03) - printLineHeight;

                            rect = new Rectangle(0, (int)yPos, 284, getRectHeight(Properties.ResourceString.SlipType01Desc04) + 1);
                            e.Graphics.DrawString(getLangString(Properties.ResourceString.SlipType01Desc04), printFont, Brushes.Black, rect);
                            e.Graphics.DrawRectangle(Pens.White, Rectangle.Round(rect));
                            yPos += printFont.GetHeight(e.Graphics) + getRectHeight(Properties.ResourceString.SlipType01Desc04) - printLineHeight;

                            yPos += printLineHeight;
                        }
                        break;
                    case SlipType04: // 구입자보관용
                        {
                            yPos += printLineHeight * 5;
                        }
                        break;
                    case SlipType05: // 수출물품판매자보관용
                        {
                            yPos += printLineHeight * 5;
                        }
                        break;
                }

                PrintAds(e, ref yPos, type);

                PrintEndMark(e, ref yPos);
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(null, ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
        }



        public void PrintSlipTitle(PrintPageEventArgs e, ref float yPos, int nType)
        {
            printFont = new Font("Meiryo", 8);
            string strText = "";
            switch (nType)
            {
                case SlipType01:    // 수출면세물품구입기록표
                    {
                        strText = getResourceText(Properties.ResourceString.SlipType01AddText01);

                        e.Graphics.DrawRectangle(new Pen(Brushes.Black), 0, yPos, (strText.Length * 13) + 3, 20);

                        e.Graphics.DrawString(strText, addTextFont, Brushes.Black, 0, yPos, stringFormat);

                        yPos += printFont.GetHeight(e.Graphics) + 10;




                        strText = getSlipText(Properties.ResourceString.SlipType01);

                        e.Graphics.DrawString(strText, printFont, Brushes.Black, 0, yPos, stringFormat);


                        if (MapDocid[DocID.LangCode].Equals(LangCode.en_US))
                            yPos += 8 * 2 * 3;  // font emSize * 2 * linecount
                        else
                            yPos += printFont.GetHeight(e.Graphics) + 10;
                    }
                    break;
                case SlipType02:    // 면세물품구매자확약서
                    {
                        strText = getResourceText(Properties.ResourceString.SlipType02AddText01);

                        e.Graphics.DrawRectangle(new Pen(Brushes.Black), 0, yPos, (strText.Length * 13) + 3, 20);

                        e.Graphics.DrawString(strText, addTextFont, Brushes.Black, 0, yPos, stringFormat);

                        yPos += printFont.GetHeight(e.Graphics) + 10;



                        strText = getSlipText(Properties.ResourceString.SlipType02);

                        if (MapDocid[DocID.LangCode].Equals(LangCode.en_US))
                        {
                            Rectangle rect = new Rectangle(0, (int)yPos, 284, 62);
                            e.Graphics.DrawString(strText, printFont, Brushes.Black, rect);
                            e.Graphics.DrawRectangle(Pens.White, Rectangle.Round(rect));
                            yPos += printFont.GetHeight(e.Graphics) + 48;
                        }
                        else
                        {
                            Rectangle rect = new Rectangle(0, (int)yPos, 284, 48);
                            e.Graphics.DrawString(strText, printFont, Brushes.Black, rect);
                            e.Graphics.DrawRectangle(Pens.White, Rectangle.Round(rect));
                            yPos += printFont.GetHeight(e.Graphics) + 40;
                        }
                    }
                    break;
                case SlipType03:    // 수출면세물품구입기록표(보관용)
                    {
                        strText = getSlipText(Properties.ResourceString.SlipType03);

                        e.Graphics.DrawString(strText, printFont, Brushes.Black, 0, yPos, stringFormat);

                        if (MapDocid[DocID.LangCode].Equals(LangCode.en_US))
                            yPos += 8 * 2 * 4;  // font emSize * 2 * linecount
                        else
                            yPos += printFont.GetHeight(e.Graphics) * 2 + 10;
                    }
                    break;
                case SlipType04:    // 구입자 보관용
                    {
                        strText = getSlipText(Properties.ResourceString.SlipType04);

                        e.Graphics.DrawString(strText, printFont, Brushes.Black, 0, yPos, stringFormat);

                        yPos += printFont.GetHeight(e.Graphics) + 10;
                    }
                    break;
                case SlipType05:    // 수출물품판매자보관용
                    {
                        strText = getSlipText(Properties.ResourceString.SlipType05);

                        e.Graphics.DrawString(strText, printFont, Brushes.Black, 0, yPos, stringFormat);

                        if (MapDocid[DocID.LangCode].Equals(LangCode.en_US))
                            yPos += 8 * 2 * 3;  // font emSize * 2 * linecount
                        else
                            yPos += printFont.GetHeight(e.Graphics) + 10;
                    }
                    break;
            }
        }

        public void PrintRePublish(PrintPageEventArgs e, ref float yPos, int nType)
        {
            try
            {
                stringFormat.LineAlignment = StringAlignment.Center;
                if (MapDocid[DocID.LangCode].Equals("EN"))
                {
                    if (nType != SlipType03)
                        yPos += 10;
                }

                string strRePublish = String.Format("[{0}]", getSlipText(Properties.ResourceString.RePublish));
                e.Graphics.DrawString(strRePublish, printFont, Brushes.Black, 80, yPos, stringFormat);

                yPos += printFont.GetHeight(e.Graphics);
                stringFormat.LineAlignment = StringAlignment.Near;
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(null, ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
        }

        public void PrintJPNLogo(PrintPageEventArgs e, ref float yPos)
        {
            try
            {
                Bitmap jpn_logo = (Bitmap)Properties.Resources.jpn_logo;
                e.Graphics.DrawImage((Image)jpn_logo, new Rectangle(0, (int)yPos, jpn_logo.Width, jpn_logo.Height));
                yPos = jpn_logo.Height;
            }
            catch (Exception ex)
            {
                MessageBox.Show(null, ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public void PrintCorpLogo(PrintPageEventArgs e, ref float yPos)
        {
            try
            {
                ResourceManager rm = Properties.Resources.ResourceManager;

                Bitmap corp_logo = (Bitmap)rm.GetObject("gtf_logo");
                e.Graphics.DrawImage((Image)corp_logo, new Rectangle(0, (int)yPos, corp_logo.Width, corp_logo.Height));
                yPos += corp_logo.Height;
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(null, ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
        }

        public void PrintBarcode(PrintPageEventArgs e, ref float yPos)
        {
            try
            {
                var writer = new BarcodeWriter
                {
                    Format = (BarcodeFormat)BarcodeFormat.CODE_128,
                    Options = EncodingOptions ?? new EncodingOptions
                    {
                        Height = 58,
                        Width = 250
                    },
                    Renderer = (IBarcodeRenderer<Bitmap>)Activator.CreateInstance(Renderer)
                };

                Bitmap qrbmp = writer.Write(Convert.ToString(MapDocid[DocID.SlipNo]));

                Image img = (Image)qrbmp;
                Point p = new Point(0, (int)Math.Round(yPos));
                e.Graphics.DrawImage(img, p);
                yPos += printFont.GetHeight(e.Graphics);
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(null, ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
        }

        public void PrintRetailer(PrintPageEventArgs e, ref float yPos, int type)
        {
            try
            {
                PrintLine(e, ref yPos);

                printFont = new Font("Meiryo", 7, FontStyle.Bold);
                e.Graphics.DrawString(Properties.Resources.StringRetailer, printFont, Brushes.Black, 0, yPos, stringFormat);
                yPos += printFont.GetHeight(e.Graphics);

                PrintLine(e, ref yPos);

                // 1. 소속 세무서
                printFont = new Font("Meiryo", 7);
                e.Graphics.DrawString(String.Format("{0} : {1}", getSlipText(Properties.ResourceString.TaxOffice), SubstringTaxOffice(MapRetailer[Retailer.TaxOffice])), printFont, Brushes.Black, 0, yPos, stringFormat);
                yPos += printFont.GetHeight(e.Graphics);

                // 2. 납세지            
                //e.Graphics.DrawString(String.Format("{0} : {1}", getSlipText(Properties.ResourceString.TaxPlace), MapRetailer[Retailer.TaxPlace]), printFont, Brushes.Black, 0, yPos, stringFormat);
                e.Graphics.DrawString(getSlipText(Properties.ResourceString.TaxPlace), printFont, Brushes.Black, 0, yPos, stringFormat);
                yPos += printFont.GetHeight(e.Graphics);

                StringFormat strFormat = new StringFormat();
                strFormat.Alignment = StringAlignment.Far;

                SizeF stringSize = new SizeF();

                String strTaxPlace = MapRetailer[Retailer.TaxPlace1] + ' ' + MapRetailer[Retailer.TaxPlace2];
                stringSize = e.Graphics.MeasureString(strTaxPlace, printFont);

                if (stringSize.Width < 284)
                {
                    e.Graphics.DrawString(strTaxPlace, printFont, Brushes.Black, new Rectangle(0, (int)yPos, 284, 12), strFormat);
                    yPos += printFont.GetHeight(e.Graphics);
                }
                else
                {
                    e.Graphics.DrawString(MapRetailer[Retailer.TaxPlace1], printFont, Brushes.Black, new Rectangle(0, (int)yPos, 284, 12), strFormat);
                    yPos += printFont.GetHeight(e.Graphics);

                    e.Graphics.DrawString(MapRetailer[Retailer.TaxPlace2], printFont, Brushes.Black, new Rectangle(0, (int)yPos, 284, 12), strFormat);
                    yPos += printFont.GetHeight(e.Graphics);
                }
                /* 사이즈 조절...
                while (stringSize.Width > 284)
                {
                    float size = printFont.Size;
                    size -= 1;
                    printFont = new Font("Meiryo", size);
                    stringSize = e.Graphics.MeasureString(MapRetailer[Retailer.TaxPlace], printFont);
                }
                */

                printFont = new Font("Meiryo", 7);

                // 3. 판매자성명 혹은 상호
                e.Graphics.DrawString(getSlipText(Properties.ResourceString.Seller), printFont, Brushes.Black, 0, yPos, stringFormat);
                yPos += printFont.GetHeight(e.Graphics);

                //Rectangle rect = new Rectangle(0, (int)yPos, 284, 26);
                //e.Graphics.DrawString(MapRetailer[Retailer.Seller], printFont, Brushes.Black, rect, strFormat);
                //e.Graphics.DrawRectangle(Pens.White, Rectangle.Round(rect));
                //yPos += printFont.GetHeight(e.Graphics) + 26;
                e.Graphics.DrawString(MapRetailer[Retailer.Seller], printFont, Brushes.Black, new Rectangle(0, (int)yPos, 284, 12), strFormat);
                yPos += printFont.GetHeight(e.Graphics);

                // 운영회사
                e.Graphics.DrawString(Properties.Resources.StringOperationName, printFont, Brushes.Black, 0, yPos, stringFormat);
                yPos += printFont.GetHeight(e.Graphics);
                e.Graphics.DrawString(MapRetailer[Retailer.OptCorpJpnm], printFont, Brushes.Black, new Rectangle(0, (int)yPos, 284, 12), strFormat);
                yPos += printFont.GetHeight(e.Graphics);

                // 4. 판매장 주소
                e.Graphics.DrawString(getSlipText(Properties.ResourceString.SellerAddr), printFont, Brushes.Black, 0, yPos, stringFormat);
                yPos += printFont.GetHeight(e.Graphics);

                String strSellerAddr = MapRetailer[Retailer.SellerAddr1] + ' ' + MapRetailer[Retailer.SellerAddr2];
                stringSize = e.Graphics.MeasureString(strSellerAddr, printFont);

                if (stringSize.Width < 284)
                {
                    e.Graphics.DrawString(strSellerAddr, printFont, Brushes.Black, new Rectangle(0, (int)yPos, 284, 12), strFormat);
                    yPos += printFont.GetHeight(e.Graphics);
                }
                else
                {
                    e.Graphics.DrawString(MapRetailer[Retailer.SellerAddr1], printFont, Brushes.Black, new Rectangle(0, (int)yPos, 284, 12), strFormat);
                    yPos += printFont.GetHeight(e.Graphics);

                    e.Graphics.DrawString(MapRetailer[Retailer.SellerAddr2], printFont, Brushes.Black, new Rectangle(0, (int)yPos, 284, 12), strFormat);
                    yPos += printFont.GetHeight(e.Graphics);
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(null, ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
        }

        public void PrintTouristDetails(PrintPageEventArgs e, ref float yPos)
        {
            try
            {
                PrintLine(e, ref yPos);
                printFont = new Font("Meiryo", 7, FontStyle.Bold);
                e.Graphics.DrawString(Properties.Resources.StringTouristDetails, printFont, Brushes.Black, 0, yPos, stringFormat);
                yPos += printFont.GetHeight(e.Graphics);
                PrintLine(e, ref yPos);

                printFont = new Font("Meiryo", 7);

                // 1. 여권 종류
                e.Graphics.DrawString(String.Format("{0} : {1}", getSlipText(Properties.ResourceString.PassPortType), MapTourist[Tourist.PassportType]), printFont, Brushes.Black, 0, yPos, stringFormat);
                yPos += printFont.GetHeight(e.Graphics);

                // 2. 여권 번호
                e.Graphics.DrawString(String.Format("{0} : {1}", getSlipText(Properties.ResourceString.PassPortNo), MapTourist[Tourist.PassportNo]), printFont, Brushes.Black, 0, yPos, stringFormat);
                yPos += printFont.GetHeight(e.Graphics);

                // 3. 구매자 성명
                //e.Graphics.DrawString(String.Format("{0} : {1}", getSlipText(Properties.ResourceString.BuyerName), MapTourist[Tourist.Name]), printFont, Brushes.Black, 0, yPos, stringFormat);
                e.Graphics.DrawString(getSlipText(Properties.ResourceString.BuyerName), printFont, Brushes.Black, 0, yPos, stringFormat);
                yPos += printFont.GetHeight(e.Graphics);

                StringFormat strFormat = new StringFormat();
                strFormat.Alignment = StringAlignment.Far;

                e.Graphics.DrawString(MapTourist[Tourist.Name], printFont, Brushes.Black, new Rectangle(0, (int)yPos, 284, 12), strFormat);
                yPos += printFont.GetHeight(e.Graphics);

                // 4. 국적
                e.Graphics.DrawString(String.Format("{0} : {1}", getSlipText(Properties.ResourceString.National), MapTourist[Tourist.National]), printFont, Brushes.Black, 0, yPos, stringFormat);
                yPos += printFont.GetHeight(e.Graphics);

                // 5. 생년월일
                e.Graphics.DrawString(String.Format("{0} : {1}", getSlipText(Properties.ResourceString.Birth), MapTourist[Tourist.Birth]), printFont, Brushes.Black, 0, yPos, stringFormat);
                yPos += printFont.GetHeight(e.Graphics);

                // 6. 체류자격
                e.Graphics.DrawString(String.Format("{0} : {1}", getSlipText(Properties.ResourceString.Residence), MapTourist[Tourist.Residence]), printFont, Brushes.Black, 0, yPos, stringFormat);
                yPos += printFont.GetHeight(e.Graphics);

                // 7. 상륙지 
                // Modified by AsCarion [2015.06.10]
                //e.Graphics.DrawString(String.Format("{0} : {1}", getSlipText(Properties.ResourceString.LandingPlace), MapTourist[Tourist.LandingPlace]), printFont, Brushes.Black, 0, yPos, stringFormat);
                //yPos += printFont.GetHeight(e.Graphics);

                // 8. 상륙년월일
                e.Graphics.DrawString(String.Format("{0} : {1}", getSlipText(Properties.ResourceString.LandingDate), MapTourist[Tourist.LandingDate]), printFont, Brushes.Black, 0, yPos, stringFormat);
                yPos += printFont.GetHeight(e.Graphics);

                yPos += 10;
            }
            catch (Exception ex)
            {
                MessageBox.Show(null, ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
        }

        public void PrintGoodsDetails(PrintPageEventArgs e, ref float yPos, int type)
        {
            try
            {
                PrintLine(e, ref yPos);
                printFont = new Font("Meiryo", 7, FontStyle.Bold);
                e.Graphics.DrawString(Properties.Resources.StringGoodsDetails, printFont, Brushes.Black, 0, yPos, stringFormat);
                yPos += printFont.GetHeight(e.Graphics);
                PrintLine(e, ref yPos);

                // 1. 구입년월일
                e.Graphics.DrawString(String.Format("{0} : {1}", getSlipText(Properties.ResourceString.BuyDate), MapGoods[Goods.SaleDate]), printFont, Brushes.Black, 0, yPos, stringFormat);
                yPos += printFont.GetHeight(e.Graphics);

                for (int i = 0; i < Convert.ToInt32(MapGoods[Goods.ItemTypeSeq]); i++)
                {
                    string ItemMapKey = String.Format("ItemsMap_{0}", i);
                    Dictionary<string, object> ItemMap = (Dictionary<string, object>)MapGoods[ItemMapKey];

                    printFont = new Font("Meiryo", 7, FontStyle.Bold);
                    if (ItemMap[Goods.ItemTypeCode].Equals("01"))       // 소모품
                        e.Graphics.DrawString(getSlipText(Properties.ResourceString.COMM), printFont, Brushes.Black, 0, yPos, stringFormat);
                    else if (ItemMap[Goods.ItemTypeCode].Equals("02"))    // 일반품목
                        e.Graphics.DrawString(getSlipText(Properties.ResourceString.EXCOMM), printFont, Brushes.Black, 0, yPos, stringFormat);
                    yPos += printFont.GetHeight(e.Graphics);

                    printFont = new Font("Meiryo", 7);
                    // Layout 형식으로 출력 변경
                    StringFormat strFormat = new StringFormat();
                    strFormat.Alignment = StringAlignment.Center;

                    // 총 Width : 284로 계산.
                    // 품명
                    e.Graphics.DrawString(Properties.Resources.StringGoodsDetails04, printFont, Brushes.Black, new Rectangle(0, (int)yPos, 100, 12), strFormat);
                    // 단가
                    e.Graphics.DrawString(Properties.Resources.StringGoodsDetails05, printFont, Brushes.Black, new Rectangle(101, (int)yPos, 54, 12), strFormat);
                    // 수량
                    e.Graphics.DrawString(Properties.Resources.StringGoodsDetails06, printFont, Brushes.Black, new Rectangle(155, (int)yPos, 54, 12), strFormat);
                    // 판매가격
                    e.Graphics.DrawString(Properties.Resources.StringGoodsDetails07, printFont, Brushes.Black, new Rectangle(209, (int)yPos, 75, 12), strFormat);

                    yPos += printFont.GetHeight(e.Graphics);

                    e.Graphics.DrawString(getLangString(Properties.ResourceString.GoodsName), new Font("arial", 6), Brushes.Black, new Rectangle(0, (int)yPos, 100, 12), strFormat);
                    e.Graphics.DrawString(getLangString(Properties.ResourceString.GoodsPrice), new Font("arial", 6), Brushes.Black, new Rectangle(101, (int)yPos, 54, 12), strFormat);
                    e.Graphics.DrawString(getLangString(Properties.ResourceString.GoodsAmt), new Font("arial", 6), Brushes.Black, new Rectangle(155, (int)yPos, 54, 12), strFormat);
                    e.Graphics.DrawString(getLangString(Properties.ResourceString.SaleAmt), new Font("arial", 6), Brushes.Black, new Rectangle(209, (int)yPos, 75, 12), strFormat);

                    yPos += printFont.GetHeight(e.Graphics);

                    for (int j = 0; j < Convert.ToInt32(ItemMap[Goods.GoodsSeq]); j++)
                    {
                        string GoodsMapKey = String.Format("GoodsMap_{0}", j);
                        Dictionary<string, object> GoodsMap = (Dictionary<string, object>)ItemMap[GoodsMapKey];

                        // 품명
                        strFormat.Alignment = StringAlignment.Near;
                        e.Graphics.DrawString((string)GoodsMap[Goods.Name], printFont, Brushes.Black, new Rectangle(0, (int)yPos, 100, 12), strFormat);
                        // 단가
                        strFormat.Alignment = StringAlignment.Far;
                        e.Graphics.DrawString(String.Format("{0}", Convert.ToInt32(GoodsMap[Goods.UnitPrice]).ToString("#,##0")), printFont, Brushes.Black, new Rectangle(101, (int)yPos, 54, 12), strFormat);
                        // 수량
                        e.Graphics.DrawString(String.Format("{0}", Convert.ToInt32(GoodsMap[Goods.Qty]).ToString("#,##0")), printFont, Brushes.Black, new Rectangle(155, (int)yPos, 54, 12), strFormat);
                        // 판매가격
                        e.Graphics.DrawString(String.Format("{0}", Convert.ToInt32(GoodsMap[Goods.Amt]).ToString("#,##0")), printFont, Brushes.Black, new Rectangle(209, (int)yPos, 75, 12), strFormat);

                        yPos += printFont.GetHeight(e.Graphics);
                    }

                    yPos += 10;

                    strFormat.Alignment = StringAlignment.Far;

                    // 합계금액
                    e.Graphics.DrawString(getSlipText(Properties.ResourceString.TotalAmt), printFont, Brushes.Black, new Rectangle(0, (int)yPos, 210, 12), strFormat);
                    e.Graphics.DrawString(String.Format("{0,13}", Convert.ToInt32(ItemMap[Goods.SaleAmt]).ToString("#,##0")), printFont, Brushes.Black, new Rectangle(0, (int)yPos, 284, 12), strFormat);
                    yPos += printFont.GetHeight(e.Graphics);

                    // 면세금액                    
                    e.Graphics.DrawString(getSlipText(Properties.ResourceString.TaxAmt), printFont, Brushes.Black, new Rectangle(0, (int)yPos, 210, 12), strFormat);
                    e.Graphics.DrawString(String.Format("{0,13}", Convert.ToInt32(ItemMap[Goods.TaxAmt]).ToString("#,##0")), printFont, Brushes.Black, new Rectangle(0, (int)yPos, 284, 12), strFormat);
                    yPos += printFont.GetHeight(e.Graphics);

                    // 반환금액
                    e.Graphics.DrawString(getSlipText(Properties.ResourceString.RefundAmt), printFont, Brushes.Black, new Rectangle(0, (int)yPos, 210, 12), strFormat);
                    e.Graphics.DrawString(String.Format("{0,13}", Convert.ToInt32(ItemMap[Goods.RefundAmt]).ToString("#,##0")), printFont, Brushes.Black, new Rectangle(0, (int)yPos, 284, 12), strFormat);
                    yPos += printFont.GetHeight(e.Graphics);

                    // 전 점포별 합산금액
                    if (MapDocid[DocID.AllStoreAmtPrint].Equals("Y") && ItemMap.ContainsKey(Goods.AllStoresTotalAmt))
                    {
                        if (ItemMap[Goods.ItemTypeCode].Equals("01"))       // 소모품
                        {
                            //e.Graphics.DrawString(getSlipText(Properties.ResourceString.AllStoresTotalAmount01), printFont, Brushes.Black, new Rectangle(0, (int)yPos, 210, 12), strFormat);
                            e.Graphics.DrawString(Properties.Resources.StringGoodsDetails15, printFont, Brushes.Black, new Rectangle(0, (int)yPos, 210, 12), strFormat);
                        }
                        else if (ItemMap[Goods.ItemTypeCode].Equals("02"))    // 일반품목
                        {
                            //e.Graphics.DrawString(getSlipText(Properties.ResourceString.AllStoresTotalAmount02), printFont, Brushes.Black, new Rectangle(0, (int)yPos, 210, 12), strFormat);
                            e.Graphics.DrawString(Properties.Resources.StringGoodsDetails16, printFont, Brushes.Black, new Rectangle(0, (int)yPos, 210, 12), strFormat);
                        }
                        e.Graphics.DrawString(String.Format("{0,13}", Convert.ToInt32(ItemMap[Goods.AllStoresTotalAmt]).ToString("#,##0")), printFont, Brushes.Black, new Rectangle(0, (int)yPos, 284, 12), strFormat);
                        yPos += printFont.GetHeight(e.Graphics);
                    }
                }

                if (type == SlipType04 || type == SlipType05)
                {
                    PrintLine(e, ref yPos);

                    StringFormat strFormat = new StringFormat();
                    strFormat.Alignment = StringAlignment.Far;

                    printFont = new Font("Meiryo", 7, FontStyle.Bold);

                    // 총 합계금액
                    e.Graphics.DrawString(getSlipText(Properties.ResourceString.TotalSaleAmt), printFont, Brushes.Black, new Rectangle(0, (int)yPos, 210, 12), strFormat);
                    e.Graphics.DrawString(String.Format("{0,13}", Convert.ToInt32(MapGoods[Goods.TotalSaleAmt]).ToString("#,##0")), printFont, Brushes.Black, new Rectangle(0, (int)yPos, 284, 12), strFormat);
                    yPos += printFont.GetHeight(e.Graphics);

                    // 총 면세금액
                    e.Graphics.DrawString(getSlipText(Properties.ResourceString.TotalTaxAmt), printFont, Brushes.Black, new Rectangle(0, (int)yPos, 210, 12), strFormat);
                    e.Graphics.DrawString(String.Format("{0,13}", Convert.ToInt32(MapGoods[Goods.TotalTaxAmt]).ToString("#,##0")), printFont, Brushes.Black, new Rectangle(0, (int)yPos, 284, 12), strFormat);
                    yPos += printFont.GetHeight(e.Graphics);

                    // 총 수수료
                    e.Graphics.DrawString(getSlipText(Properties.ResourceString.TotalFeeAmt), printFont, Brushes.Black, new Rectangle(0, (int)yPos, 210, 12), strFormat);
                    e.Graphics.DrawString(String.Format("{0,13}", Convert.ToInt32(MapGoods[Goods.TotalFeeAmt]).ToString("#,##0")), printFont, Brushes.Black, new Rectangle(0, (int)yPos, 284, 12), strFormat);
                    yPos += printFont.GetHeight(e.Graphics);

                    // 총 반환액
                    e.Graphics.DrawString(getSlipText(Properties.ResourceString.TotalRefundAmt), printFont, Brushes.Black, new Rectangle(0, (int)yPos, 210, 12), strFormat);
                    e.Graphics.DrawString(String.Format("{0,13}", Convert.ToInt32(MapGoods[Goods.TotalRefundAmt]).ToString("#,##0")), printFont, Brushes.Black, new Rectangle(0, (int)yPos, 284, 12), strFormat);
                    yPos += printFont.GetHeight(e.Graphics);

                    printFont = new Font("Meiryo", 7);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(null, ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
        }

        public void PrintAds(PrintPageEventArgs e, ref float yPos, int type)
        {
            try
            {
                if (MapAdsInfo.Count == 0)
                    return;

                for (int i = 0; i < Convert.ToInt32(MapAdsInfo[AdsInfo.Count]); i++)
                {
                    Dictionary<string, object> AdsItemMap = (Dictionary<string, object>)MapAdsInfo[String.Format("{0}", i)];
                    List<int> targetList = (List<int>)AdsItemMap[AdsInfo.Target];

                    for (int j = 0; j < targetList.Count; j++)
                    {
                        if (type != targetList[j])
                            continue;

                        PrintLine(e, ref yPos);

                        if (AdsItemMap[AdsInfo.Type].Equals("01"))       // Image
                        {
                            //Bitmap imgAds = getURLImage(Convert.ToString(AdsItemMap[AdsInfo.URL]));
                            Bitmap imgAds = getDataImage(Convert.ToString(AdsItemMap[AdsInfo.IMG]));
                            
                            if (imgAds != null)
                            {
                                e.Graphics.DrawImage((Image)imgAds, new Rectangle(0, (int)yPos, imgAds.Width, imgAds.Height));
                                yPos += imgAds.Height;
                            }
                        }
                        if (AdsItemMap[AdsInfo.Type].Equals("02"))       // Text
                        {
                            List<string> AdsText = (List<string>)AdsItemMap[AdsInfo.TEXT];
                            for (int k = 0; k < AdsText.Count; k++)
                            {
                                Font adsFont = new Font("Meiryo", 6);
                                e.Graphics.DrawString(AdsText[k], adsFont, Brushes.Black, 0, yPos, stringFormat);
                                yPos += adsFont.GetHeight(e.Graphics);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(null, ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
        }

        public void PrintLine(PrintPageEventArgs e, ref float yPos)
        {
            try
            {
                printFont = new Font("Meiryo", 7);
                e.Graphics.DrawString(Properties.Resources.StringLine, printFont, Brushes.Black, 0, yPos, stringFormat);
                yPos += printFont.GetHeight(e.Graphics);
            }
            catch (Exception ex)
            {
                MessageBox.Show(null, ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
        }

        public void PrintEndMark(PrintPageEventArgs e, ref float yPos)
        {
            try
            {
                printFont = new Font("Meiryo", 4);
                e.Graphics.DrawString(Properties.Resources.StringEndMark, printFont, Brushes.Black, 0, yPos, stringFormat);
                yPos += printFont.GetHeight(e.Graphics);
            }
            catch (Exception ex)
            {
                MessageBox.Show(null, ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
        }

        public void PrintRefundDetails(PrintPageEventArgs e, ref float yPos)
        {
            try
            {
                PrintLine(e, ref yPos);
                printFont = new Font("Meiryo", 7, FontStyle.Bold);
                e.Graphics.DrawString(Properties.Resources.StringRefundway, printFont, Brushes.Black, 0, yPos, stringFormat);
                yPos += printFont.GetHeight(e.Graphics);
                PrintLine(e, ref yPos);

                printFont = new Font("Meiryo", 7);

                // 1. 환급 방법
                e.Graphics.DrawString(String.Format("{0} : {1}", Properties.Resources.StringRefundWayDesc, MapRefundInfo[RefundWayInfo.REFUND_WAY_CODE_DESC]), printFont, Brushes.Black, 0, yPos, stringFormat);
                //yPos += printFont.GetHeight(e.Graphics);

                // 2. 환급 정보
                if (MapRefundInfo[RefundWayInfo.REFUND_WAY_CODE].Equals("06"))
                {
                    e.Graphics.DrawString(String.Format("{0} : {1}", Properties.Resources.StringRefundWayQQ, MapRefundInfo[RefundWayInfo.MASK_REMIT_NO]), printFont, Brushes.Black, 70, yPos, stringFormat);
                    yPos += printFont.GetHeight(e.Graphics);
                }
                else if (MapRefundInfo[RefundWayInfo.REFUND_WAY_CODE].Equals("04"))
                {
                    e.Graphics.DrawString(String.Format("{0} : {1}", Properties.Resources.StringRefundWayUPI, MapRefundInfo[RefundWayInfo.MASK_REMIT_NO]), printFont, Brushes.Black, 70, yPos, stringFormat);
                    yPos += printFont.GetHeight(e.Graphics);
                }
                else
                {
                    yPos += printFont.GetHeight(e.Graphics);
                }


                yPos += 3;
            }
            catch (Exception ex)
            {
                MessageBox.Show(null, ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
        }

        public void PrintRefundSerialInfo(PrintPageEventArgs e, ref float yPos, int type)
        {
            try
            {
                switch (type)
                {
                    case SlipType01:    // 수출면세물품구입기록표
                        {
                            PrintLine(e, ref yPos);
                            printFont = new Font("Meiryo", 7, FontStyle.Bold);
                            e.Graphics.DrawString(Properties.Resources.StringRefundSerial, printFont, Brushes.Black, 0, yPos, stringFormat);
                            yPos += printFont.GetHeight(e.Graphics);
                            PrintLine(e, ref yPos);

                            printFont = new Font("Meiryo", 15);

                            // 1. 시리얼번호
                            e.Graphics.DrawString(String.Format("{0} : {1}", Properties.Resources.StringRefundSerial, MapDocid[DocID.Unikey]), printFont, Brushes.Black, 0, yPos, stringFormat);
                            yPos += printFont.GetHeight(e.Graphics);

                            yPos += 3;
                        }
                        break;
                    case SlipType02:    // 면세물품구매자확약서
                        {


                            yPos += 3;
                        }
                        break;
                    case SlipType03:    // 수출면세물품구입기록표(보관용)
                        {


                            yPos += 3;
                        }
                        break;
                    case SlipType04:    // 구입자 보관용
                        {


                            yPos += 3;
                        }
                        break;
                    case SlipType05:    // 수출물품판매자보관용
                        {
                            yPos += 3;
                        }
                        break;
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(null, ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
        }
    }
}
