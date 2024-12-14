using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UFSoft.UBF.Business;
using UFSoft.UBF.Util.DataAccess;
using UFSoft.UBF.Util.Log;
using UFIDA.U9.Base;
using UFIDA.U9.Base.UserRole;
using System.Data;
using System.IO;
using System.Collections;
using UFIDA.U9.PR.PurchaseRequest;
using UFIDA.U9.Base.FlexField.ValueSet;
using UFIDA.U9.PM.PO;
using UFIDA.U9.PM.Enums;
using UFIDA.U9.SM.SO;
using UFIDA.U9.CBO.SCM.Item;
using UFIDA.U9.Complete.RCVRpt;
using UFIDA.U9.Complete.Enums;
using UFIDA.U9.CBO.SCM.Warehouse;
using UFSoft.UBF.PL.Engine.Cache;
using UFIDA.U9.AP.APBill;
using UFIDA.U9.CBO.FI.Enums;
using UFIDA.U9.InvDoc.TransferIn;
using UFIDA.U9.PM.Rcv;
using UFIDA.U9.InvDoc.Enums;
using UFIDA.U9.InvDoc.TransferIn.Proxy;
using UFIDA.U9.InvDoc.InnerBalance;
using UFIDA.U9.CBO.Pub.Controller;
using UFIDA.U9.Base.SOB;
using UFIDA.U9.InvDoc.CostAdjust;
using UFSoft.UBF.PL;
using UFIDA.U9.AR.ARBill;

namespace YY.U9.Cust.KHMM.AppPlugIn
{
    /// <summary>
    /// 应收单价格重算
    /// </summary>
    [UFSoft.UBF.Eventing.Configuration.Failfast]
    class ARBillHeadPriceRecalSubscriber : UFSoft.UBF.Eventing.IEventSubscriber
    {
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
            ARBillHead arBillHead = key.GetEntity() as ARBillHead;
            if (arBillHead == null)
            {
                return;
            }
            //除了本部，其他组织都不重算
            if (Context.LoginOrg.ID != 1002105084816757)
            {
                return;
            }

            if (arBillHead.OriginalData.DocStatus == BillStatusEnum.Opened  && arBillHead.DocStatus == BillStatusEnum.Approving)
            {
                //币种为人民币，为空
            if ( arBillHead.AC == null || arBillHead.AC.Code == null)
            {
                return;
            }
            if (arBillHead.AC.Code.Equals("C001"))
            {
                return;
            }
            //已重算
            //if (arBillHead.DescFlexField.PrivateDescSeg2.Equals("True"))
            //{
            //    return;
            //}
            string code = arBillHead.AccrueCust.Code.Substring(0, 3);
            //内部结算排除
            if (!code.Equals("NZB"))
            {
                //decimal ACToFCExRate = 0M;
                foreach (ARBillLine arBillLine in arBillHead.ARBillLines)
                {

                    //数据库出货单汇率
                    DataTable tb_Ship = new DataTable();
                    string sql_Ship = "select ACToFCExRate from SM_Ship where DocNo = '" + arBillLine.SrcBillNum + "'";
                    DataSet ds_Ship = new DataSet();
                    DataAccessor.RunSQL(DataAccessor.GetConn(), sql_Ship, null, out ds_Ship);
                    tb_Ship = ds_Ship.Tables[0];

                    //数据库退货处理汇率
                    DataTable tb_RMA = new DataTable();
                    string sql_RMA = "select ACToFCExchRate from SM_RMA where DocNo = '" + arBillLine.SrcBillNum + "'";
                    DataSet ds_RMA = new DataSet();
                    DataAccessor.RunSQL(DataAccessor.GetConn(), sql_RMA, null, out ds_RMA);
                    tb_RMA = ds_RMA.Tables[0];

                    if (tb_Ship.Rows.Count > 0 || tb_RMA.Rows.Count > 0)
                    {
                        decimal acToFCExRate = 0M;
                        if (tb_Ship.Rows.Count > 0)
                        {
                            acToFCExRate = Convert.ToDecimal(tb_Ship.Rows[0]["ACToFCExRate"].ToString());
                        }
                        if (tb_RMA.Rows.Count > 0)
                        {
                            acToFCExRate = Convert.ToDecimal(tb_RMA.Rows[0]["ACToFCExchRate"].ToString());
                        }
                        arBillLine.DescFlexField.PrivateDescSeg2 = Convert.ToString(acToFCExRate);
                        decimal ARFCMoney_NonTax = arBillLine.AROCMoney.NonTax * acToFCExRate;
                        //未税
                        arBillLine.ARFCMoney.NonTax = Math.Round(ARFCMoney_NonTax, 2, MidpointRounding.AwayFromZero);

                        decimal ARFCMoney_GoodsTax = arBillLine.AROCMoney.GoodsTax * acToFCExRate;
                        //税额
                        arBillLine.ARFCMoney.GoodsTax = Math.Round(ARFCMoney_GoodsTax, 2, MidpointRounding.AwayFromZero);

                        decimal ARFCMoney_TotalMoney = arBillLine.AROCMoney.TotalMoney * acToFCExRate;
                        //加税合计
                        arBillLine.ARFCMoney.TotalMoney = Math.Round(ARFCMoney_TotalMoney, 2, MidpointRounding.AwayFromZero);

                        //tax
                        decimal ARFCMoney_Tax = arBillLine.AROCMoney.Tax * acToFCExRate;
                        arBillLine.ARFCMoney.Tax = Math.Round(ARFCMoney_Tax, 2, MidpointRounding.AwayFromZero);
                        //tax
                        decimal ARFCMoney_Fee = arBillLine.AROCMoney.Fee * acToFCExRate;
                        arBillLine.ARFCMoney.Fee = Math.Round(ARFCMoney_Fee, 2, MidpointRounding.AwayFromZero);
                        //tax
                        decimal ARFCMoney_FeeTax = arBillLine.AROCMoney.FeeTax * acToFCExRate;
                        arBillLine.ARFCMoney.FeeTax = Math.Round(ARFCMoney_FeeTax, 2, MidpointRounding.AwayFromZero);


                        //总金额余额
                        decimal ARFCMoneyBalance_TotalMoney = arBillLine.AROCMoneyBalance.TotalMoney * acToFCExRate;
                        arBillLine.ARFCMoneyBalance.TotalMoney = Math.Round(ARFCMoneyBalance_TotalMoney, 2, MidpointRounding.AwayFromZero);
                        decimal ARFCMoneyBalance_Fee = arBillLine.AROCMoneyBalance.Fee * acToFCExRate;
                        arBillLine.ARFCMoneyBalance.Fee = Math.Round(ARFCMoneyBalance_Fee, 2, MidpointRounding.AwayFromZero);
                        decimal ARFCMoneyBalance_FeeTax = arBillLine.AROCMoneyBalance.FeeTax * acToFCExRate;
                        arBillLine.ARFCMoneyBalance.FeeTax = Math.Round(ARFCMoneyBalance_FeeTax, 2, MidpointRounding.AwayFromZero);
                        decimal ARFCMoneyBalance_GoodsTax = arBillLine.AROCMoneyBalance.GoodsTax * acToFCExRate;
                        arBillLine.ARFCMoneyBalance.GoodsTax = Math.Round(ARFCMoneyBalance_GoodsTax, 2, MidpointRounding.AwayFromZero);
                        decimal ARFCMoneyBalance_NonTax = arBillLine.AROCMoneyBalance.NonTax * acToFCExRate;
                        arBillLine.ARFCMoneyBalance.NonTax = Math.Round(ARFCMoneyBalance_NonTax, 2, MidpointRounding.AwayFromZero);
                        decimal ARFCMoneyBalance_Tax = arBillLine.AROCMoneyBalance.Tax * acToFCExRate;
                        arBillLine.ARFCMoneyBalance.Tax = Math.Round(ARFCMoneyBalance_Tax, 2, MidpointRounding.AwayFromZero);


                        decimal CfmedARNonTax_FCMoney = arBillLine.AROCMoney.NonTax * acToFCExRate;
                        arBillLine.CfmedARNonTax.FCMoney = Math.Round(CfmedARNonTax_FCMoney, 2, MidpointRounding.AwayFromZero);
                        foreach (ARBillRCLine rcLine in arBillLine.ARBillRCLines)
                        {
                            if (rcLine.ARBillLine.ID == arBillLine.ID)
                            {
                                //确认行未税金额
                                rcLine.CfmARNonTax.FCMoney = arBillLine.ARFCMoney.NonTax;
                                rcLine.CfmARGoodsTax.FCMoney = arBillLine.ARFCMoney.GoodsTax;
                                rcLine.RevenueMoney.FCMoney = arBillLine.ARFCMoney.TotalMoney;
                                rcLine.RevenueTax.FCMoney = arBillLine.ARFCMoney.GoodsTax;
                            }
                            foreach (ARBillRCMatchLine matchLine in rcLine.ARBillRCMatchLines)
                            {
                                matchLine.RevenueMoney.FCMoney = arBillLine.ARFCMoney.TotalMoney;
                            }
                        }
                            foreach (ARInstalment arInstalment in arBillLine.ARInstalments)
                            {
                                if (arInstalment.ARBillLine.ID == arBillLine.ID)
                                {
                                    //收款行未税金额
                                    arInstalment.ARFCMoneyBalance.NonTax = arBillLine.ARFCMoney.NonTax;
                                    arInstalment.ARFCMoneyBalance.GoodsTax = arBillLine.ARFCMoney.GoodsTax;
                                    arInstalment.ARFCMoneyBalance.TotalMoney = arBillLine.ARFCMoney.TotalMoney;
                                    arInstalment.ARFCMoneyBalance.Fee = arBillLine.ARFCMoney.Fee;
                                    arInstalment.ARFCMoneyBalance.FeeTax = arBillLine.ARFCMoney.FeeTax;
                                    arInstalment.ARFCMoneyBalance.Tax = arBillLine.ARFCMoney.Tax;

                                    arInstalment.ARFCMoney.NonTax = arBillLine.ARFCMoney.NonTax;
                                    arInstalment.ARFCMoney.GoodsTax = arBillLine.ARFCMoney.GoodsTax;
                                    arInstalment.ARFCMoney.TotalMoney = arBillLine.ARFCMoney.TotalMoney;
                                    arInstalment.ARFCMoney.Fee = arBillLine.ARFCMoney.Fee;
                                    arInstalment.ARFCMoney.FeeTax = arBillLine.ARFCMoney.FeeTax;
                                    arInstalment.ARFCMoney.Tax = arBillLine.ARFCMoney.Tax;
                                }
                            }
                            foreach (ARBillMergeAccrueLine mergeAccrueLine in arBillLine.ARBillMergeAccrueLine)
                            {
                                mergeAccrueLine.ARFCMoney.NonTax = arBillLine.ARFCMoney.NonTax;
                                mergeAccrueLine.ARFCMoney.GoodsTax = arBillLine.ARFCMoney.GoodsTax;
                                mergeAccrueLine.ARFCMoney.TotalMoney = arBillLine.ARFCMoney.TotalMoney;
                                mergeAccrueLine.ARFCMoney.Fee = arBillLine.ARFCMoney.Fee;
                                mergeAccrueLine.ARFCMoney.FeeTax = arBillLine.ARFCMoney.FeeTax;
                                mergeAccrueLine.ARFCMoney.Tax = arBillLine.ARFCMoney.Tax;
                            }
                            foreach (ARBillTaxDetail taxDetail in arBillLine.ARBillTaxDetails)
                            {
                                taxDetail.TaxMoney.FCMoney = arBillLine.ARFCMoney.Tax;
                                taxDetail.CfmARTax.FCMoney = arBillLine.ARFCMoney.Tax;
                            }
                        }
                }
                if (arBillHead.AROCMoney.TotalMoney > 0)
                {
                    arBillHead.ACToFCExRate = arBillHead.ARFCMoney.TotalMoney / arBillHead.AROCMoney.TotalMoney;
                }
                arBillHead.DescFlexField.PrivateDescSeg2 = "True";
            }
            }
            
            #endregion
        }
    }
}
