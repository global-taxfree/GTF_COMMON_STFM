using System;
using System.Collections.Generic;
using System.Text;

namespace GTF_Printer.GTF_JP
{
    partial class GTF_JPETRS
    {
        public class DocID
        {
            public const string LangCode = "LangCode";
            public const string PublishCnt = "PublishCnt";
            public const string PublishType = "PublishType";
            public const string RePublish = "RePublish";
            public const string SlipNo = "SlipNo";
            public const string AllStoreAmtPrint = "AllStoreAmtPrint";
            public const string Unikey = "Unikey";
        }

        public class Retailer
        {
            public const string TaxOffice = "TaxOffice";
            public const string TaxPlace1 = "TaxPlace1";
            public const string TaxPlace2 = "TaxPlace2";
            public const string Seller = "Seller";
            public const string SellerAddr1 = "SellerAddr1";
            public const string SellerAddr2 = "SellerAddr2";
            public const string OptCorpJpnm = "OptCorpJpnm";
        }

        public class Tourist
        {
            public const string PassportType = "PassportType";
            public const string PassportNo = "PassportNo";
            public const string Name = "Name";
            public const string National = "National";
            public const string Birth = "Birth";
            public const string Residence = "Residence";
            public const string LandingPlace = "LandingPlace";
            public const string LandingDate = "LandingDate";
        }

        public class Goods
        {
            public const string SaleDate = "SaleDate";
            public const string ItemTypeCode = "ItemTypeCode";
            public const string ItemTypeSeq = "ItemTypeSeq";
            public const string GoodsSeq = "GoodsSeq";
            public const string Name = "Name";
            public const string UnitPrice = "UnitPrice";
            public const string Qty = "Qty";
            public const string Amt = "Amt";

            public const string SaleAmt = "SaleAmt";
            public const string TaxAmt = "TaxAmt";            
            public const string RefundAmt = "RefundAmt";
            public const string AllStoresTotalAmt = "AllStoresTotalAmt";

            public const string TotalSaleAmt = "TotalSaleAmt";
            public const string TotalTaxAmt = "TotalTaxAmt";
            public const string TotalFeeAmt = "TotalFeeAmt";            
            public const string TotalRefundAmt = "TotalRefundAmt";
        }

        public class AdsInfo
        {
            public const string Count = "Count";
            public const string Type = "Type";
            public const string Target = "Target";
            public const string URL = "URL";
            public const string IMG = "IMG";
            public const string TEXT = "TEXT";
        }

        public class LangCode
        {
            public const string ko_KR = "KR";
            public const string en_US = "EN";
            public const string zh_CN = "CN";
        }

        public class SummaryInfo
        {
            public const string SEQ = "total_slip_seq";
            public const string SUM_AMT = "total_sum_amt";
            public const string TAX_AMT = "total_tax_amt";
            public const string FEE_AMT = "total fee_amt";
            public const string REFUND_AMT = "total_refund_amt";
        }

        public class RefundWayInfo
        {
            public const string REFUND_WAY_CODE = "refund_way_code";
            public const string REFUND_WAY_CODE_DESC = "refund_way_code_desc";
            public const string MASK_REMIT_NO = "mask_remit_no";
        }
    }
}
