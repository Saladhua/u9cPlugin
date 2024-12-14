using System;
using System.Data;
using UFIDA.U9.AP.APBill;
using UFSoft.UBF.Business;
using UFSoft.UBF.PL.Engine;
using UFSoft.UBF.Util.DataAccess;
using UFSoft.UBF.Util.Log;

namespace YY.U9.Cust.LI.AppPlugIn
{
    /// <summary>
    /// D:科华ui改be 科华小零件-应付单
    /// </summary>
    [UFSoft.UBF.Eventing.Configuration.Failfast]
    class KHAPBillHeadInsertedSubscriber : UFSoft.UBF.Eventing.IEventSubscriber
    {
        private static readonly ILogger logger = LoggerManager.GetLogger(typeof(KHAPBillHeadInsertedSubscriber));
        public void Notify(params object[] args)
        {
            #region 从事件参数中取得当前业务实体
            //从事件参数中取得当前业务实体
            if (args == null || args.Length == 0 || !(args[0] is UFSoft.UBF.Business.EntityEvent))
                return;

            BusinessEntity.EntityKey key = ((UFSoft.UBF.Business.EntityEvent)args[0]).EntityKey;
            if (key == null)
            {
                return;
            }
            APBillHead aPBillHead = key.GetEntity() as APBillHead;
            if (aPBillHead == null)
            {
                return;
            }
            #endregion
            if (aPBillHead.Org.Code != "10")
            {
                return;
            }



            //if (webButton != null)
            //{
            //    if (webButton.Action == "SaveClick" || webButton.Action == "SubmitClick")//保存时 或者 提交时
            //    {

            //1.原未税单价取厂商价目表，同供应商的价表截止系统日期的最新价格，保存时计算--私有字段5
            //2.原价税合计按原开发执行
            //3.未税单价取厂商价目表，同供应商的价表截止系统日期的最新价格，保存时计算
            //4.未税金额 = 未税单价 * 应付单行数量，保存时计算
            //5.价税合计 = 未税金额 *（1 + 单据税率），保存时计算
            //6.行偏差金额 = 价税合计 - 原价税合计，保存时计算--私有字段6
            //7.含税单价=未税单价*（1+税率）,保存时计算
            //税额=未税金额*行税率  

            if (aPBillHead.SysState != ObjectState.Inserted)
            {
                foreach (var item in aPBillHead.APBillLines)
                {
                    //有来源再计算
                    if (!string.IsNullOrEmpty(item.SrcDataNum))
                    {
                        item.DescFlexField.PrivateDescSeg6 = ((item.APOCMoney.NonTax + item.APOCMoney.GoodsTax) - decimal.Parse(item.DescFlexField.PrivateDescSeg4)).ToString("0.####");//行偏差金额   
                    }
                }
            }

            string DocTypeCode = aPBillHead.DocType.Code;
            if (aPBillHead.SysState == ObjectState.Inserted)
            {
                if (DocTypeCode == "YF10" || DocTypeCode == "YF11")
                {
                    foreach (var item in aPBillHead.APBillLines)
                    {
                        string itemCode = item.Item.ItemCode;

                        string itemID = item.Item.ItemID.ID.ToString();

                        string Supplier = item.APBillHead.AccrueSupp.Supplier.ID.ToString();

                        int itemCodeLength = itemCode.Length;

                        if (itemCodeLength > 5)
                        {
                            if (itemCode.Substring(0, 5) != "D0301")
                            {
                                throw new Exception("此单据类型只允许购买小零件，请使用PO24  标准订单-小零件进行下单！不正确料号：" + itemCode + "");
                            }
                        }

                        string sql = "SELECT  TOP(1) Price,IsIncludeTax FROM PPR_PurPriceList A1 LEFT JOIN PPR_PurPriceLine A2 ON A1.ID=A2.PurPriceList " +
                            " WHERE A1.Supplier = '" + Supplier + "' AND A2.ItemInfo_ItemID = '" + itemID + "' AND A1.Org = '" + aPBillHead.Org.ID + "'" +
                            " AND CONVERT(VARCHAR(10), GETDATE(), 120) < A2.ToDate  " +
                            " AND A2.Active=1" +
                            " ORDER BY A2.FromDate DESC";

                        DataTable dt = GetDataTable(sql);

                        string Price = "0";

                        string IsIncludeTax = "0";

                        if (dt.Rows != null && dt.Rows.Count > 0)
                        {
                            Price = dt.Rows[0]["Price"].ToString();
                            IsIncludeTax = dt.Rows[0]["IsIncludeTax"].ToString();
                        }
                        if (IsIncludeTax == "False")
                        {
                            //赋值
                            decimal TaxSum = item.APOCMoney.NonTax + item.APOCMoney.GoodsTax;

                            item.DescFlexField.PrivateDescSeg5 = decimal.Parse(Price).ToString("0.####");//原未税单价 

                            //item.DescFlexField.PrivateDescSeg4 = decimal.Parse(Price).ToString("0.####");//原税税单价 

                            item.NonTaxPrice = decimal.Parse(Price);//未税单价

                            decimal TaxRate = 0;//税率

                            foreach (var item1 in item.TaxSchedule.TaxScheduleTaxs)
                            {
                                TaxRate = item1.Tax.TaxRate;
                            }

                            decimal Rate = item.APBillHead.ACToFCExRate;





                            #region 核币
                            item.APOCMoney.NonTax = Math.Round(item.NonTaxPrice * item.PUAmount, 2);//未税金额

                            item.APOCMoney.GoodsTax = Math.Round(item.APOCMoney.NonTax * TaxRate, 2);

                            item.APOCMoney.Tax = Math.Round(item.APOCMoney.GoodsTax, 2);

                            item.APOCMoney.TotalMoney = item.APOCMoney.NonTax + item.APOCMoney.GoodsTax;

                            item.TaxPrice = item.NonTaxPrice * (1 + TaxRate);
                            #endregion

                            #region 本币   本币*汇率
                            item.APFCMoney.NonTax = Math.Round(Rate * item.APOCMoney.NonTax, 2);//未税金额

                            item.APFCMoney.GoodsTax = Math.Round(item.APOCMoney.GoodsTax * Rate, 2);

                            item.APFCMoney.Tax = Math.Round(item.APOCMoney.Tax * Rate, 2);

                            item.APFCMoney.TotalMoney = Math.Round(item.APOCMoney.NonTax + item.APOCMoney.GoodsTax, 2);
                            #endregion

                            item.DescFlexField.PrivateDescSeg4 = (item.APOCMoney.NonTax + item.APOCMoney.GoodsTax).ToString("0.####");//原税税单价

                            item.DescFlexField.PrivateDescSeg6 = ((item.APOCMoney.NonTax + item.APOCMoney.GoodsTax) - decimal.Parse(item.DescFlexField.PrivateDescSeg4)).ToString("0.####");//行偏差金额   

                            foreach (var itemTax in item.APBillTaxDetails)
                            {
                                itemTax.TaxMoney.OCMoney = item.APOCMoney.GoodsTax;

                                itemTax.TaxMoney.FCMoney = item.APFCMoney.GoodsTax;
                            }

                        }
                        else
                        {
                            //IsIncludeTax----这个字段判断0是没勾的，1是含税的要按照下面的公式调整
                            //公式说明：增加一个判断条件，如果厂商价目表是含税的
                            //7.原未税单价取（厂商价目表价格 /（1 + 税率）），同供应商的价表截止系统日期的最新价格，保存时计算
                            //8.原价税合计按原开发执行
                            //9.未税单价取厂商价目表（厂商价目表价格 /（1 + 税率）），同供应商的价表截止系统日期的最新价格，保存时计算
                            //10.未税金额 = 价税合计-税额，保存时计算
                            //11.价税合计 = 单价*数量，保存时计算
                            //12.行偏差金额 = 价税合计 - 原价税合计，保存时计算
                            //赋值
                            decimal TaxRate = 0;

                            foreach (var item1 in item.TaxSchedule.TaxScheduleTaxs)
                            {
                                TaxRate = item1.Tax.TaxRate;
                            }

                            decimal Rate = item.APBillHead.ACToFCExRate;

                            item.DescFlexField.PrivateDescSeg5 = (decimal.Parse(Price) / (1 + TaxRate)).ToString("0.####");//原未税单价 


                            item.NonTaxPrice = decimal.Parse(Price) / (1 + TaxRate);//未税单价

                            item.APOCMoney.NonTax = item.APFCMoney.NonTax;//未税金额

                            item.APOCMoney.GoodsTax = item.APOCMoney.NonTax * TaxRate; //税额 

                            //string NonTax = ((decimal.Parse(Price) / (1 + decimal.Parse(TaxRate))) * decimal.Parse(item.PUAmount.ToString())).ToString();

                            //item["APOCMoney_NonTax"] = NonTax;//未税金额

                            //string TaxSum = ((decimal.Parse(Price) / (1 + decimal.Parse(TaxRate))) * (1 + decimal.Parse(TaxRate))).ToString();//价税合计

                            //item.APOCMoney.NonTax = decimal.Parse(TaxSum) - item.APOCMoney.GoodsTax;//未税金额 

                            //item["TaxPrice"] = ((decimal.Parse(Price) / (1 + decimal.Parse(item["TaxRate"].ToString()))) * (1 + decimal.Parse(item["TaxRate"].ToString())));//含税单价

                            //decimal d6 = decimal.Parse(TaxSum) - decimal.Parse(Price);//价税合计

                            item.DescFlexField.PrivateDescSeg6 = (TaxRate - decimal.Parse(Price)).ToString("0.####");//行偏差金额

                            item.APOCMoney.TotalMoney = item.APOCMoney.NonTax + item.APOCMoney.GoodsTax;

                            item.APFCMoney.TotalMoney = item.APOCMoney.TotalMoney;

                            item.APFCMoney.Tax = item.APOCMoney.Tax; //税额 

                            item.APFCMoney.FeeTax = item.APOCMoney.FeeTax; //税额 

                            item.APFCMoney.Tax = item.APOCMoney.NonTax * TaxRate; //税额 

                            #region 核币
                            item.APOCMoney.NonTax = item.APFCMoney.NonTax;//未税金额

                            item.APOCMoney.GoodsTax = Math.Round(item.APFCMoney.NonTax * TaxRate, 2);

                            item.APOCMoney.Tax = item.APOCMoney.GoodsTax;

                            item.APOCMoney.TotalMoney = item.APOCMoney.NonTax + item.APOCMoney.GoodsTax;
                            #endregion

                            #region 本币   本币*汇率
                            item.APFCMoney.NonTax = Math.Round(Rate * item.APOCMoney.NonTax);//未税金额

                            item.APFCMoney.GoodsTax = Math.Round(item.APOCMoney.GoodsTax * Rate, 2);

                            item.APFCMoney.Tax = Math.Round(item.APOCMoney.Tax * Rate, 2);

                            item.APFCMoney.TotalMoney = Math.Round(item.APOCMoney.NonTax + item.APOCMoney.GoodsTax, 2);
                            #endregion
                            decimal TaxSum = item.APOCMoney.NonTax + item.APOCMoney.GoodsTax;


                            foreach (var itemTax in item.APBillTaxDetails)
                            {
                                itemTax.TaxMoney.OCMoney = item.APOCMoney.GoodsTax;

                                itemTax.TaxMoney.FCMoney = item.APFCMoney.GoodsTax;
                            }
                            item.DescFlexField.PrivateDescSeg4 = (item.APOCMoney.NonTax + item.APOCMoney.GoodsTax).ToString("0.####");//原税税单价

                            item.DescFlexField.PrivateDescSeg6 = ((item.APOCMoney.NonTax + item.APOCMoney.GoodsTax) - decimal.Parse(item.DescFlexField.PrivateDescSeg4)).ToString("0.####");//行偏差金额   

                        }
                    }
                }
                else
                {

                    string businessType = aPBillHead.BusinessType.Name;

                    if (businessType == "预付款	")
                    {
                        return;
                    }

                    foreach (var item in aPBillHead.APBillLines)
                    {
                        string itemCode = item.Item.ItemCode;

                        int itemCodeLength = itemCode.Length;

                        if (itemCodeLength > 5)
                        {
                            if (itemCode.Substring(0, 5) == "D0301")
                            {
                                throw new Exception("此单据类型禁止做小零件发票，请使用YF10 发票-小零件单据类型进行制单！");
                            }
                        }
                    }
                }
            }

        }


        public static DataTable GetDataTable(string sql)
        {
            #region 用法
            //string sql = string.Format("select ID from U9CCustNrknor_DeductDoc where BeginDate>='{0}' and EndDate<='{1}' and DocVersion='{2}' and Org={3} and ID!={4}", item.BeginDate.ToString("yyyy-MM-dd"), item.EndDate.ToString("yyyy-MM-dd"), item.DocVersion, PDContext.Current.OrgID, item.ID);
            //DataTable dt = GetDataTable(sql);
            //return dt;
            #endregion
            DataTable dt = new DataTable();
            System.Data.DataSet returnValue = null;
            DataAccessor.RunSQL(DataAccessor.GetConn(), sql, null, out returnValue);
            if (returnValue != null)
            {
                dt = returnValue.Tables[0];
            }
            return dt;
        }
    }
}
