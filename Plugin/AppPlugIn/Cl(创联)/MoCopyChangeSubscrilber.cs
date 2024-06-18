using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Web;
using UFIDA.U9.CBO.Enums;
using UFIDA.U9.CBO.MFG.BOM;
using UFIDA.U9.ISV.MO;
using UFIDA.U9.MO.MO;
using UFIDA.U9.SM.ForecastOrder;
using UFIDA.U9.SM.SO;
using UFSoft.UBF.Business;
using UFSoft.UBF.Util.DataAccess;
using UFSoft.UBF.Util.Log;

namespace YY.U9.Cust.LI.AppPlugIn
{
    /// <summary>
    /// 创联-生产订单备料复制
    /// </summary>
    [UFSoft.UBF.Eventing.Configuration.Failfast]
    class MoCopyChangeSubscrilber : UFSoft.UBF.Eventing.IEventSubscriber
    {
        private static readonly UFSoft.UBF.Util.Log.ILogger logger = LoggerManager.GetLogger(typeof(MoCopyChangeSubscrilber));

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
            MO mo = key.GetEntity() as MO;
            if (mo == null)
            {
                return;
            }
            #endregion
            string MOID = "";

            if (HttpContext.Current.Session["MOID"] != null)
            {
                MOID = HttpContext.Current.Session["MOID"].ToString();
                HttpContext.Current.Session["MOID"] = null;
            }
            MO mO = new MO();
            if (!string.IsNullOrEmpty(MOID))
            {
                mO = MO.Finder.FindByID(Convert.ToInt64(MOID));
            }
            else
            {
                return;
            }

            if (mo.DescFlexField.PrivateDescSeg4 == "False")
            {
                return;
            }
            decimal ProductQty = mo.ProductQty;
            #region 取数-赋值

            //mo.CompleteQtyCtlType

            //成功时序为BeforeDefaultValue

            if (!string.IsNullOrEmpty(MOID))
            {
                if (mo.SysState == UFSoft.UBF.PL.Engine.ObjectState.Inserted)
                {
                    if (mo.DocState.Value != 0)
                    {
                        return;
                    }

                    foreach (var item in mo.MOPickLists)
                    {
                        item.Remove();
                    }

                    mo.DescFlexField.PrivateDescSeg30 = "备料赋值";

                    foreach (var item in mO.MOPickLists)
                    {
                        MOPickList mOPickList = MOPickList.Create(mo);
                        mOPickList.DocLineNO = item.DocLineNO;
                        mOPickList.ItemMaster = item.ItemMaster;
                        mOPickList.ActualReqDate = item.ActualReqDate;
                        mOPickList.OperationNum = item.OperationNum;
                        mOPickList.IssueStyle = item.IssueStyle;
                        mOPickList.QtyType = UsageQuantityTypeEnum.GetFromValue(1);
                        mOPickList.QPA = item.QPA;
                        mOPickList.WasteRate = item.WasteRate;
                        mOPickList.IsCalcCost = item.IsCalcCost;
                        mOPickList.SupplyWh = item.SupplyWh;
                        mOPickList.BOMReqQty = item.BOMReqQty == 0 ? 0 : ProductQty * mOPickList.QPA;
                        mOPickList.STDReqQty = item.STDReqQty == 0 ? 0 : ProductQty * mOPickList.QPA;
                        mOPickList.ActualReqQty = item.ActualReqQty == 0 ? 0 : ProductQty * mOPickList.QPA;
                        //mOPickList.ActualReqQty = item.ActualReqQty;
                        mOPickList.ReserveQty = item.ReserveQty;
                        mOPickList.IsSubstitute = item.IsSubstitute;
                        mOPickList.SubstitutedItem = item.SubstitutedItem;
                        mOPickList.SubstitutedPick = item.SubstitutedPick;
                        mOPickList.ReplaceRatio = item.ReplaceRatio;
                        mOPickList.ReplaceSequence = item.ReplaceSequence;
                        mOPickList.SubstituteStyle = item.SubstituteStyle;

                        if (item.ItemMaster.ItemFormAttribute.Value==6)
                        {
                            mOPickList.DescFlexField.PrivateDescSeg30 = "是";

                        }

                        mOPickList.DescFlexField.PrivateDescSeg30 = "是";
                        mOPickList.DescFlexField.PrivateDescSeg1 = item.DescFlexField.PrivateDescSeg1;
                        mOPickList.DescFlexField.PrivateDescSeg2 = item.DescFlexField.PrivateDescSeg2;
                        mOPickList.DescFlexField.PrivateDescSeg3 = item.DescFlexField.PrivateDescSeg3;
                        mo.MOPickLists.Add(mOPickList);
                    }
                    Session.Current.InList(mo);
                }
            }

            #endregion
        }

        //public static MO CreatSCMPickList(MO mo, MOPickList mOPick)
        //{
        //    MO moHead = null;
        //    moHead = MO.Create();

        //    moHead = mo;

        //    mo.MOPickLists.Add(mOPick);

        //    MOPickList.Create(moHead);

        //    return moHead;
        //}
    }
}

