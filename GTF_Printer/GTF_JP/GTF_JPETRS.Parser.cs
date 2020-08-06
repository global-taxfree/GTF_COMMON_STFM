using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace GTF_Printer.GTF_JP
{
    partial class GTF_JPETRS
    {
        public void setClearParseMap()
        {
            MapDocid.Clear();
            MapRetailer.Clear();
            MapGoods.Clear();
            MapTourist.Clear();
            MapAdsInfo.Clear();
            MapRefundInfo.Clear();
        }

        public bool ParseParam(string docid, string retailer, string tourist, string goods, string adsinfo)
        {
            try
            {                
                if (!getParseDocid(docid))
                    return false;

                if (!getParseRetailer(retailer))
                    return false;
                
                if (!getParseTourist(tourist))
                    return false;

                if (!getParseGoods(goods))
                    return false;

                if (!getParseAdsInfo(adsinfo))
                    return false;
            }
            catch
            {
                return false;
            }

            return true;
        }

        public bool getParseDocid(string docid)
        {
            try
            {
                string[] arrDocid = docid.Split('|');
                
                MapDocid.Add(DocID.LangCode, arrDocid[0]);
                MapDocid.Add(DocID.PublishCnt, arrDocid[1]);

                string[] arrPublishType = arrDocid[2].Split('/');
                                
                List<int> PublishTypeList = new List<int>();
                for (int i = 0; i < arrPublishType.Length; i++)
                {
                    PublishTypeList.Add(Convert.ToInt32(arrPublishType[i]));
                }
                MapDocid.Add(DocID.PublishType, PublishTypeList);
                MapDocid.Add(DocID.RePublish, arrDocid[3]);
                MapDocid.Add(DocID.SlipNo, arrDocid[4]);
                MapDocid.Add(DocID.AllStoreAmtPrint, arrDocid[5]);
                MapRefundInfo.Add(RefundWayInfo.REFUND_WAY_CODE, arrDocid[6]);
                MapRefundInfo.Add(RefundWayInfo.REFUND_WAY_CODE_DESC, arrDocid[7]);
                MapRefundInfo.Add(RefundWayInfo.MASK_REMIT_NO, arrDocid[8]);
                //MapDocid.Add(DocID.AllStoreAmtPrint, arrDocid[5]);
                MapDocid.Add(DocID.Unikey, arrDocid[9]);
            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
                return false;
            }
            return true;
        }

        public bool getParseRetailer(string retailer)
        {
            try
            {
                string[] arrRatiler = retailer.Split('|');
                MapRetailer.Add(Retailer.TaxOffice, arrRatiler[0]);
                MapRetailer.Add(Retailer.TaxPlace1, arrRatiler[1]);
                MapRetailer.Add(Retailer.TaxPlace2, arrRatiler[2]);
                MapRetailer.Add(Retailer.Seller, arrRatiler[3]);
                MapRetailer.Add(Retailer.SellerAddr1, arrRatiler[4]);
                MapRetailer.Add(Retailer.SellerAddr2, arrRatiler[5]);
                MapRetailer.Add(Retailer.OptCorpJpnm, arrRatiler[6]);
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }

            return true;
        }

        public bool getParseTourist(string tourist)
        {
            try
            {
                string[] arrTourist = tourist.Split('|');
                //MapTourist.Add(Tourist.PassportType, getPassPortEtc(arrTourist[0]));      // ver.1.1.1.4 여권종류 Text로 수정
                MapTourist.Add(Tourist.PassportType, arrTourist[0]);
                MapTourist.Add(Tourist.PassportNo, arrTourist[1]);
                MapTourist.Add(Tourist.Name, arrTourist[2].ToUpper());
                MapTourist.Add(Tourist.National, arrTourist[3]);
                // Modified by AsCarion [2015.04.14]
                // 날짜 양식 수정                
                MapTourist.Add(Tourist.Birth, getDateString(arrTourist[4]));
                MapTourist.Add(Tourist.Residence, arrTourist[5]);
                //MapTourist.Add(Tourist.LandingPlace, arrTourist[6]);                
                MapTourist.Add(Tourist.LandingDate, getDateString(arrTourist[6]));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
            return true;
        }

        public bool getParseGoods(string goods)
        {
            try
            {
                /*//////////////////////////////////////////////////////////////////////////
                 * Format
                 * [0] : Sale Date          (8)
                 * [1] : Item Type Seq      (3)
                 * 
                 * { [2] ~ [7] X [1] }
                 * [2] : Item Type Code     (2)
                 * [3] : Goods Seq         (3)  
                 * { [4]~[7] x [3] }
                 * [4] : Goods Name
                 * [5] : Goods Unit Price
                 * [6] : Goods Qty
                 * [7] : Goods Amount
                 * 
                 * [8] : Tax Amt
                 * [9] : Sale Amt
                 * [10] : Refund Amt
                 * 
                 * [11] : Item Type Code
                 * [12] : Goods Seq
                 * { [13]~[16] x [12] }
                 * [13] : Goods Name
                 * [14] : Goods Unit Price
                 * [15] : Goods Qty
                 * [16] : Goods Amount
                 * 
                 * [17] : Tax Amt
                 * [18] : Sale Amt
                 * [19] : Refund Amt
                 * 
                 * [20] : Total Sale Amt
                 * [21] : Total Tax Amt
                 * [22] : Total Fee Amt
                 * [23] : Total Refund Amt
                 * 
                 * ex)
                 * 20150310
                 * 2
                 * 
                 * 01
                 * 2
                 * a01 | 10000 | 1 | 10000
                 * a02 | 10000 | 1 | 10000
                 * 1600 | 20000 | 1280
                 * 
                 * 02
                 * 2
                 * b01 | 10000 | 1 | 10000
                 * b02 | 10000 | 1 | 10000
                 * 1600 | 20000 | 1280
                 * 
                 * 40000 | 2560 | 640 | 1920                
                //////////////////////////////////////////////////////////////////////////*/

                string[] arrGoods = goods.Split('|');
                // Modified by AsCarion [2015.04.14]
                // 날짜 양식 수정
                //MapGoods.Add(Goods.SaleDate, arrGoods[0]);
                MapGoods.Add(Goods.SaleDate, getDateString(arrGoods[0]));
                MapGoods.Add(Goods.ItemTypeSeq, arrGoods[1]);

                int nOffSet = 2;

                for (int i = 0; i < Convert.ToInt32(MapGoods[Goods.ItemTypeSeq]); i++)
                {
                    Dictionary<string, object> ItemMap = new Dictionary<string, object>();
                    ItemMap.Add(Goods.ItemTypeCode, arrGoods[nOffSet++]);
                    ItemMap.Add(Goods.GoodsSeq, arrGoods[nOffSet++]);

                    for(int j = 0; j < Convert.ToInt32(ItemMap[Goods.GoodsSeq]); j++)
                    {
                        Dictionary<string, object> GoodsMap = new Dictionary<string, object>();
                        GoodsMap.Add(Goods.Name, arrGoods[nOffSet++]);
                        GoodsMap.Add(Goods.UnitPrice, arrGoods[nOffSet++]);
                        GoodsMap.Add(Goods.Qty, arrGoods[nOffSet++]);
                        GoodsMap.Add(Goods.Amt, arrGoods[nOffSet++]);
                        string ItemMapKey = String.Format("GoodsMap_{0}", j);
                        ItemMap.Add(ItemMapKey, (object)GoodsMap);
                    }
                    ItemMap.Add(Goods.SaleAmt, arrGoods[nOffSet++]);
                    ItemMap.Add(Goods.TaxAmt, arrGoods[nOffSet++]);                    
                    ItemMap.Add(Goods.RefundAmt, arrGoods[nOffSet++]);
                    if (arrGoods[nOffSet].Equals(""))
                        nOffSet++;
                    else
                        ItemMap.Add(Goods.AllStoresTotalAmt, arrGoods[nOffSet++]);
                    

                    string MapGoodsKey = String.Format("ItemsMap_{0}", i);
                    MapGoods.Add(MapGoodsKey, (object)ItemMap);
                }
                MapGoods.Add(Goods.TotalSaleAmt, arrGoods[nOffSet++]);
                MapGoods.Add(Goods.TotalTaxAmt, arrGoods[nOffSet++]);
                MapGoods.Add(Goods.TotalFeeAmt, arrGoods[nOffSet++]);
                MapGoods.Add(Goods.TotalRefundAmt, arrGoods[nOffSet++]);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
            return true;
        }

        public bool getParseAdsInfo(string adsinfo)
        {
            try
            {
                if(adsinfo != null && adsinfo.Length > 0)
                {
                    // AdsCount | ( Type | TargetCnt | Target * N | URL | TEXT ) * N
                    string[] arrAdsinfo = adsinfo.Split('|');
                    int idx = 0;
                    MapAdsInfo.Add(AdsInfo.Count, arrAdsinfo[idx++]);

                    for(int i = 0; i < Convert.ToInt32(MapAdsInfo[AdsInfo.Count]); i++)
                    {
                        Dictionary<string, object> AdsItemMap = new Dictionary<string, object>();

                        AdsItemMap.Add(AdsInfo.Type, arrAdsinfo[idx++]);
                        List<int> targetList = new List<int>();
                        int targetCount = Convert.ToInt32(arrAdsinfo[idx++]);
                        for (int j = 0; j < targetCount; j++)
                            targetList.Add(Convert.ToInt32(arrAdsinfo[idx++]));
                        AdsItemMap.Add(AdsInfo.Target, targetList);
                        if (AdsItemMap[AdsInfo.Type].Equals("01"))  // IMAGE
                        {
                            //AdsItemMap.Add(AdsInfo.URL, arrAdsinfo[idx++]);
                            AdsItemMap.Add(AdsInfo.IMG, arrAdsinfo[idx++]);//URL 에서 이미지 데이터 로드하는것으로 변경
                        }
                        else if (AdsItemMap[AdsInfo.Type].Equals("02"))  // TEXT
                        {
                            List<string> adsTextList = new List<string>();
                            for (int j = 0; j < 5; j++)
                            {
                                if(!arrAdsinfo[idx].Equals(""))
                                    adsTextList.Add(arrAdsinfo[idx++]);
                            }
                            AdsItemMap.Add(AdsInfo.TEXT, adsTextList);
                        }
                        MapAdsInfo.Add(Convert.ToString(i), AdsItemMap);
                    }                    
                }                
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }

            return true;
        }

        public bool getParseRefundWayInfo(string refundwayinfo)
        {
            try
            {
                string[] arrRefundwayinfo = refundwayinfo.Split('|');
                MapRefundInfo.Add(RefundWayInfo.REFUND_WAY_CODE, arrRefundwayinfo[0]);
                MapRefundInfo.Add(RefundWayInfo.REFUND_WAY_CODE_DESC, arrRefundwayinfo[1]);
                MapRefundInfo.Add(RefundWayInfo.MASK_REMIT_NO, arrRefundwayinfo[2]);

            }
            catch
            {
                return false;
            }

            return true;
        }
    }
}
