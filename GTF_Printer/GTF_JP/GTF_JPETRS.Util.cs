using System;
using System.Collections.Generic;
using System.Text;
using System.Resources;
using System.Reflection;
using System.Threading;
using System.Globalization;
using System.Windows.Forms;
using System.Drawing;
using System.Net;
using System.IO;

namespace GTF_Printer.GTF_JP
{
    partial class GTF_JPETRS
    {
        public class Item
        {
            public Item(string value, string text) { Value = value; Text = text; }
            public string Value { get; set; }
            public string Text { get; set; }

            public override string ToString() { return Text; }
        }

        public void parsePassportInfo(string mrz1, string mrz2)
        {
            string tmpName = mrz1.Substring(5, mrz1.Length - 5);

            string firstName = tmpName.Substring(0, tmpName.IndexOf("<<")).Replace("<", "");
            string lastName = tmpName.Substring(tmpName.IndexOf("<<"), tmpName.Length - tmpName.IndexOf("<<")).Replace("<", "");
            string passportNo = mrz2.Substring(0, 9).Replace("<", "");
            string country = mrz2.Substring(10, 3).Replace("<", "");
            string birthDate = mrz2.Substring(13, 6).Replace("<", "");
            birthDate = getFullBirthYear(birthDate);			// 생년월일 구하기
            string gender = mrz2.Substring(20, 1).Replace("<", "");
            string expireDate = mrz2.Substring(21, 6).Replace("<", "");
            expireDate = formatDateType("20" + expireDate);	// 만기일은 무조건 2000년

            _buyer_name = "";
            _passport_serial_no = "";
            _nationality_code = "";
            _gender_code = "";
            _buyer_birth = "";
            _pass_expirydt = "";

            _buyer_name = firstName + " " + lastName;
            _passport_serial_no = passportNo;
            _nationality_code = country;
            _gender_code = gender;
            _buyer_birth = birthDate;
            _pass_expirydt = expireDate;

            //txtPassportNm.Text = _buyer_name;
            //txtPassportNo.Text = _passport_serial_no;
            //txtNationality.Text = _nationality_code;
            //txtGender.Text = _gender_code;
            //txtBirthday.Text = _buyer_birth;
            //txtExpireDate.Text = _pass_expirydt;

            string returnParam = firstName + " " + lastName + "|" +
                                 passportNo + "|" +
                                 country + "|" +
                                 birthDate + "|" +
                                 gender + "|" +
                                 expireDate;

        }

        public String getFullBirthYear(String date)
        {
            string fullyear = "";
            int buyer_birth = Convert.ToInt32(date.Substring(0, 2));

            DateTime dtNow = DateTime.Now;   // 현재 날짜, 시간 얻기
            if (buyer_birth > dtNow.Month)
            {
                fullyear = "19" + date;
                fullyear = formatDateType(fullyear);
            }
            else
            {
                fullyear = "20" + date;
                fullyear = formatDateType(fullyear);
            }
            return fullyear;
        }

        public string formatDateType(string date)
        {
            return date.Substring(0, 4) + date.Substring(4, 2) + date.Substring(6, 2);
        }

        public string getLangString(string strResourceId)
        {
            try
            {
                ResourceManager rm = Properties.Resources.ResourceManager;

                switch (Convert.ToString(MapDocid[DocID.LangCode]))
                {
                    case LangCode.zh_CN:      // 중국어
                        {

                            rm = Properties.Resourceszh_CN.ResourceManager; //new ResourceManager("GTFCommActiveX.Properties.Resources.zh-CN", typeof(GTF_COS).Assembly);
                        }
                        break;
                    case LangCode.en_US:      // 영어
                        {
                            rm = Properties.Resourcesen_US.ResourceManager; //new ResourceManager("GTFCommActiveX.Properties.Resources.en-US", typeof(GTF_COS).Assembly);
                        }
                        break;
                    case LangCode.ko_KR:      // 한국어
                        {
                            rm = Properties.Resourcesko_KR.ResourceManager; //new ResourceManager("GTFCommActiveX.Properties.Resources.ko-KR", typeof(GTF_COS).Assembly);
                        }
                        break;
                }
                return rm.GetString(strResourceId);
            }
            catch (Exception ex)
            {
                //MessageBox.Show(this, ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Console.WriteLine(ex.Message);
                return null;
            }
        }

        public string getSlipText(string strResourceid)
        {
            try
            {
                ResourceManager rm = Properties.Resources.ResourceManager;
                return rm.GetString(strResourceid) + "/" + getLangString(strResourceid);
            }
            catch (System.Exception ex)
            {
                //MessageBox.Show(this, ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Console.WriteLine(ex.Message);
                return null;
            }
        }

        public string getPassPortEtc(string passportInfo)
        {
            string retStr = "";
            switch (Convert.ToInt32(passportInfo))
            {
                case 1:
                    retStr = Properties.Resources.StringPassPortEtc01;
                    break;
                case 2:
                    retStr = Properties.Resources.StringPassPortEtc02;
                    break;
                case 3:
                    retStr = Properties.Resources.StringPassPortEtc03;
                    break;
                case 4:
                    retStr = Properties.Resources.StringPassPortEtc04;
                    break;
                case 5:
                    retStr = Properties.Resources.StringPassPortEtc05;
                    break;
            }
            return retStr;
        }

        public Bitmap getURLImage(string strURL)
        {
            try
            {
                WebClient Downloader = new WebClient();
                Stream ImageStream = Downloader.OpenRead(strURL);
                Bitmap DownloadImage = Bitmap.FromStream(ImageStream) as Bitmap;
                return DownloadImage;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public Bitmap getDataImage(string strData)
        {
            try
            {
                byte[] arr = System.Convert.FromBase64String(strData);
                var ms = new MemoryStream(arr);
                Bitmap DownloadImage = new Bitmap(ms);
                Array.Clear(arr, 0, arr.Length);
                return DownloadImage;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public string getDateString(string strDate)
        {
            return strDate.Substring(0, 4) + "-" + strDate.Substring(4, 2) + "-" + strDate.Substring(6, 2);
        }

        public string SubstringTaxOffice(string strAddr)
        {
            return strAddr.Substring(0, strAddr.IndexOf("("));
        }


        public string getResourceText(string strResourceid)
        {
            try
            {
                ResourceManager rm = Properties.Resources.ResourceManager;
                return rm.GetString(strResourceid);
            }
            catch (System.Exception ex)
            {
                //MessageBox.Show(this, ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Console.WriteLine(ex.Message);
                return null;
            }
        }

        public int getRectHeight(string slipdesc)
        {
            int rectHeight = 12;
            int retVal = 0;

            switch (Convert.ToString(MapDocid[DocID.LangCode]))
            {
                case LangCode.ko_KR:
                    {
                        if (slipdesc.Equals(Properties.ResourceString.SlipType01Desc01))
                            retVal = rectHeight * 3;
                        if (slipdesc.Equals(Properties.ResourceString.SlipType01Desc02))
                            retVal = rectHeight * 2;
                        if (slipdesc.Equals(Properties.ResourceString.SlipType01Desc03))
                            retVal = rectHeight * 3;
                        if (slipdesc.Equals(Properties.ResourceString.SlipType01Desc04))
                            retVal = rectHeight * 5;

                        if (slipdesc.Equals(Properties.ResourceString.SlipType02Desc01))
                            retVal = rectHeight * 2;
                        if (slipdesc.Equals(Properties.ResourceString.SlipType02Desc02))
                            retVal = rectHeight * 2;
                    }
                    break;
                case LangCode.en_US:
                    {
                        if (slipdesc.Equals(Properties.ResourceString.SlipType01Desc01))
                            retVal = rectHeight * 5;
                        if (slipdesc.Equals(Properties.ResourceString.SlipType01Desc02))
                            retVal = rectHeight * 2;
                        if (slipdesc.Equals(Properties.ResourceString.SlipType01Desc03))
                            retVal = rectHeight * 6;
                        if (slipdesc.Equals(Properties.ResourceString.SlipType01Desc04))
                            retVal = rectHeight * 11;

                        if (slipdesc.Equals(Properties.ResourceString.SlipType02Desc01))
                            retVal = rectHeight * 4;
                        if (slipdesc.Equals(Properties.ResourceString.SlipType02Desc02))
                            retVal = rectHeight * 3;
                    }
                    break;
                case LangCode.zh_CN:
                    {
                        if (slipdesc.Equals(Properties.ResourceString.SlipType01Desc01))
                            retVal = rectHeight * 2;
                        if (slipdesc.Equals(Properties.ResourceString.SlipType01Desc02))
                            retVal = rectHeight * 1;
                        if (slipdesc.Equals(Properties.ResourceString.SlipType01Desc03))
                            retVal = rectHeight * 2;
                        if (slipdesc.Equals(Properties.ResourceString.SlipType01Desc04))
                            retVal = rectHeight * 4;

                        if (slipdesc.Equals(Properties.ResourceString.SlipType02Desc01))
                            retVal = rectHeight * 1;
                        if (slipdesc.Equals(Properties.ResourceString.SlipType02Desc02))
                            retVal = rectHeight * 1;
                    }
                    break;
            }
            return retVal;
        }

    }
}
