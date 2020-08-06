using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GTF_Printer.GTF_JP
{
    public partial class PrintPreviewForm : MetroFramework.Forms.MetroForm
    {
        GTF_JPETRS m_Parent;

        int nPageCount = 0;
        List<int> PublishTypeList;
        int SlipType = 0;

        public PrintPreviewForm(GTF_JPETRS pParent)
        {
            InitializeComponent();
            m_Parent = pParent;
        }

        private void PrintPreviewForm_Load(object sender, EventArgs e)
        {
            List<int> PublistTypeListOri = (List<int>)m_Parent.MapDocid[GTF_JPETRS.DocID.PublishType];
            PublishTypeList = new List<int>(PublistTypeListOri);
            nPageCount = PublishTypeList.Count;

            for (int i = 1; i < nPageCount + 1; i++)
                toolStripComboBox.Items.Add(i);

            toolStripComboBox.SelectedIndex = 0;

            printPreviewControl1.Zoom = 1.0;
            //printPreviewControl1.AutoZoom = true;
            printPreviewControl1.Columns = nPageCount;
            printDocument.DefaultPageSettings.PaperSize = new System.Drawing.Printing.PaperSize("Preview", 284, 3000);
            Activate();
            //toolStrip1.Focus();
            //toolStripButton_Print.Select();
            BTN_PRINT.Focus();
        }

        private void printDocument1_PrintPage(object sender, System.Drawing.Printing.PrintPageEventArgs e)
        {
            SlipType = PublishTypeList[0];
            PublishTypeList.RemoveAt(0);

            float yPos = 0;

            m_Parent.PrintHeader(e, ref yPos, SlipType);

            m_Parent.PrintRetailer(e, ref yPos, SlipType);

            m_Parent.PrintGoodsDetails(e, ref yPos, SlipType);

            m_Parent.PrintRefundDetails(e, ref yPos);//2017.07.11 추가

            m_Parent.PrintTouristDetails(e, ref yPos);

            m_Parent.PrintFooter(e, ref yPos, SlipType);

            //m_Parent.PrintAds(e, ref yPos, SlipType);

            //e.PageSettings.PaperSize = new System.Drawing.Printing.PaperSize("receipt", 284, (int)Math.Round(yPos));

            if (nPageCount != 1)
            {
                e.HasMorePages = true;
                nPageCount--;
            }
            else
                e.HasMorePages = false;
        }

        private void toolStripComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            printPreviewControl1.StartPage = Convert.ToInt32(toolStripComboBox.Text) - 1;
        }

        private void toolStripButton_Print_Click(object sender, EventArgs e)
        {
            m_Parent.JPNPrintTicket(m_Parent.m_docid, m_Parent.m_retailer, m_Parent.m_goods, m_Parent.m_tourist,  m_Parent.m_adsinfo);
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void toolStripButton_Close_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void metroToolTip1_Popup(object sender, PopupEventArgs e)
        {

        }

        private void BTN_PRINT_Click(object sender, EventArgs e)
        {
            m_Parent.JPNPrintTicket(m_Parent.m_docid, m_Parent.m_retailer, m_Parent.m_goods, m_Parent.m_tourist, m_Parent.m_adsinfo);
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void BTN_CLOSE_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}
