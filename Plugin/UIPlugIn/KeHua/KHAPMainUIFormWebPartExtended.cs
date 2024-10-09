using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UFIDA.U9.FI.AP.APMaintenanceUIModel;
using UFIDA.U9.FI.AR.ARMaintenanceUIModel;
using UFIDA.U9.UI.PDHelper;
using UFSoft.UBF.UI.Custom;
using UFSoft.UBF.UI.IView;

namespace YY.U9.Cust.LI.UIPlugIn
{
    /// <summary>
    /// 科华小零件-应付单
    /// </summary>
    class KHAPMainUIFormWebPartExtended : ExtendedPartBase
    {
        private APMainUIFormWebPart _part;


        public override void BeforeEventProcess(IPart part, string eventName, object sender, EventArgs args, out bool executeDefault)
        {
            base.BeforeEventProcess(part, eventName, sender, args, out executeDefault); 

            UFSoft.UBF.UI.WebControlAdapter.UFWebButton4ToolbarAdapter webButton = sender as UFSoft.UBF.UI.WebControlAdapter.UFWebButton4ToolbarAdapter; 

            this._part = (part as APMainUIFormWebPart);

            if (webButton != null)
            {
                if (webButton.Action == "SaveClick")//保存时 
                {

                    //1.原未税单价取厂商价目表，同供应商的价表截止系统日期的最新价格，保存时计算--私有字段5
                    //2.原价税合计按原开发执行
                    //3.未税单价取厂商价目表，同供应商的价表截止系统日期的最新价格，保存时计算
                    //4.未税金额 = 未税单价 * 应付单行数量，保存时计算
                    //5.价税合计 = 未税金额 *（1 + 单据税率），保存时计算
                    //6.行偏差金额 = 价税合计 - 原价税合计，保存时计算--私有字段6
                    //7.含税单价=未税单价*（1+税率）,保存时计算
                    if (this._part.Model.APBillHead.FocusedRecord["DocumentType_Code"] != null)
                    {
                        string DocTypeCode = this._part.Model.APBillHead.FocusedRecord["DocumentType_Code"].ToString();

                        string Supplier = this._part.Model.APBillHead.FocusedRecord["AccrueSupp_Supplier"].ToString();

                        if (DocTypeCode == "YF10")
                        {
                            foreach (var item in this._part.Model.APBillHead_APBillLines.Records)
                            {
                                string itemCode = item["Item_ItemCode"].ToString();

                                string itemID = item["Item_ItemID"].ToString();

                                int itemCodeLength = itemCode.Length;

                                if (itemCodeLength > 5)
                                {
                                    if (itemCode.Substring(0, 5) != "D0301")
                                    {
                                        throw new Exception("此单据类型只允许购买小零件，请使用PO24  标准订单-小零件进行下单！不正确料号：" + itemCode + "");                                 
                                    }
                                }

                                string sql = "SELECT  TOP(1) Price FROM PPR_PurPriceList A1 LEFT JOIN PPR_PurPriceLine A2 ON A1.ID=A2.PurPriceList " +
                                    " WHERE A1.Supplier = '" + Supplier + "' AND A2.ItemInfo_ItemID = '" + itemID + "' AND A1.Org = '" + PDContext.Current.OrgID + "' AND GETDATE() < A2.ToDate AND A2.FromDate <= GETDATE() " +
                                    " ORDER BY A2.FromDate DESC";

                                DataTable dt = U9Common.GetDataTable(sql);

                                string Price = "0";

                                if (dt.Rows != null && dt.Rows.Count > 0)
                                {
                                    Price = dt.Rows[0]["Price"].ToString();
                                }
                                //赋值
                                item["DescFlexField_PrivateDescSeg5"] = decimal.Parse(Price).ToString("0.####");//原未税单价 
                                item["NonTaxPrice"] = decimal.Parse(Price).ToString("0.####");//未税单价
                                string NonTax = (decimal.Parse(Price) * decimal.Parse(item["PUAmount"].ToString())).ToString();
                                item["APOCMoney_NonTax"] = NonTax;//未税金额
                                string TaxSum = (decimal.Parse(NonTax) * (1 + decimal.Parse(item["TaxRate"].ToString()))).ToString();//价税合计
                                item["APOCMoneyPriceTaxSum"] = decimal.Parse(NonTax) * (1 + decimal.Parse(item["TaxRate"].ToString()));//价税合计
                                item["TaxPrice"] = decimal.Parse(Price) * (1 + decimal.Parse(item["TaxRate"].ToString())); ;//含税单价
                                decimal d6 = decimal.Parse(TaxSum) - decimal.Parse(Price);//价税合计
                                item["DescFlexField_PrivateDescSeg6"] = (decimal.Parse(TaxSum) - decimal.Parse(Price)).ToString("0.####");//行偏差金额
                            }
                        }
                        else
                        {
                            foreach (var item in this._part.Model.APBillHead_APBillLines.Records)
                            {
                                string itemCode = item["Item_ItemCode"].ToString();

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
            }
        }
    }
}

