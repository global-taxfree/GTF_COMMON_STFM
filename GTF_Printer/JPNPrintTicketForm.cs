using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using ZXing;
using ZXing.Common;
using ZXing.Rendering;

namespace GTF_Printer
{
    public partial class JPNPrintTicketForm : Form
    {
        #region GTF_JPETRS Properties
        private int _timeout = 20;
        private int _print = 1;

        private string _buyer_no = "";
        private string _passport_serial_no = "";
        private string _buyer_birth = "";
        private string _pass_expirydt = "";
        private string _buyer_name = "";
        private string _nationality_code = "";
        private string _gender_code = "";
        private string _entry_port = "";
        private string _residence_name = "";
        private string _entry_date = "";
        private string _passport_type = "";

        #region Get/Set Properties
        public int time_out
        {
            get { return _timeout; }
            set { _timeout = value; }
        }

        public int print_count
        {
            get { return _print; }
            set { _print = value; }
        }

        public string buyer_name
        {
            get { return _buyer_name; }
            set { _buyer_name = value; }
        }

        public string nationality_code
        {
            get { return _nationality_code; }
            set { _nationality_code = value; }
        }

        public string gender_code
        {
            get { return _gender_code; }
            set { _gender_code = value; }
        }

        public string passport_serial_no
        {
            get { return _passport_serial_no; }
            set { _passport_serial_no = value; }
        }

        public string buyer_birth
        {
            get { return _buyer_birth; }
            set { _buyer_birth = value; }
        }

        public string pass_expirydt
        {
            get { return _pass_expirydt; }
            set { _pass_expirydt = value; }
        }

        public string entry_port
        {
            get
            {
                //Item item = (Item)comboLandingPort.SelectedItem;
                //_entry_port = item.Value;
                return _entry_port;
            }
            set { _entry_port = value; }
        }

        public string residence_name
        {
            get
            {
                //Item item = (Item)comboResidence.SelectedItem;
                //_residence_name = item.Value;
                return _residence_name;
            }
            set { _residence_name = value; }
        }

        public string entry_date
        {
            get
            {
                //_entry_date = txtLandingDate.Text.Replace("-", "");
                return _entry_date;
            }
            set { _entry_date = value; }
        }

        public string buyer_no
        {
            get
            {
                return _buyer_no;
            }
            set { _buyer_no = value; }
        }

        public string passport_type
        {
            get
            {
                //Item item = (Item)comboPassportEtc.SelectedItem;
                //_passport_type = item.Value;
                return _passport_type;
            }
            set { _passport_type = value; }
        }
        #endregion

        private EncodingOptions EncodingOptions { get; set; }
        private Type Renderer { get; set; }
        #endregion

        public delegate void UmcParamClickHandler(string param, int ret);
        //public event UmcParamClickHandler getPassportInfo;

        private delegate void writeMessageDelegate(string mrz1, string mrz2);
        public JPNPrintTicketForm()
        {
            InitializeComponent();
        }
    }
}
