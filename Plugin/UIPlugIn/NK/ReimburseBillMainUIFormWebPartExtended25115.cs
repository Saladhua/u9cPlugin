using System;
using UFIDA.U9.FI.ER.ReimburseBillUIModel;
using UFIDA.U9.UI.PDHelper;
using UFSoft.UBF.UI.ControlModel;
using UFSoft.UBF.UI.Custom;
using UFSoft.UBF.UI.IView;
using UFSoft.UBF.UI.WebControlAdapter;

namespace YY.U9.Cust.LI.UIPlugIn
{
    /// <summary>
    /// 同步项目进度字段
    /// </summary>
    class ReimburseBillMainUIFormWebPartExtended25115 : ExtendedPartBase
    {
        private ReimburseBillMainUIFormWebPart _part;
        public override void AfterRender(IPart part, EventArgs args)
        {
            base.AfterRender(part, args);

            this._part = (part as ReimburseBillMainUIFormWebPart);

            if (this._part.Model.ReimburseBillHead != null)
            {
                #region 计算报销总金额和报销本币总金额
                decimal BxMoney = 0;
                decimal BxTolMoney = 0;
                foreach (var item in this._part.Model.ReimburseBillHead_ReimbuurseBillDetails.Records)
                {
                    BxMoney = decimal.Parse(item["ReimburseMoney"] == null ? "0" : item["ReimburseMoney"].ToString()) + BxMoney;
                    BxTolMoney = decimal.Parse(item["ReimburseMoneyFC"] == null ? "0" : item["ReimburseMoney"].ToString()) + BxTolMoney;
                }
                //取出头上的值
                decimal HeadBxMoney = decimal.Parse(this._part.Model.ReimburseBillHead.FocusedRecord["SumReimburseMoney"].ToString());
                decimal HeadBxTolMoney = decimal.Parse(this._part.Model.ReimburseBillHead.FocusedRecord["SumReimburseFCMoney"].ToString());
                if (BxMoney != HeadBxMoney)
                {
                    this._part.Model.ReimburseBillHead.FocusedRecord["SumReimburseMoney"] = HeadBxMoney;
                }
                if (BxTolMoney != HeadBxTolMoney)
                {
                    this._part.Model.ReimburseBillHead.FocusedRecord["SumReimburseFCMoney"] = HeadBxMoney;
                }
                #endregion

                #region 更新项目字段
                foreach (var item in this._part.Model.ReimburseBillHead_ReimbuurseBillDetails.Records)
                {
                    string ExDesPri10 = "";
                    try
                    {
                        ExDesPri10 = item["ExpensePayProject_DescFlexField_PrivateDescSeg10_ID"] == null ? "" : item["ExpensePayProject_DescFlexField_PrivateDescSeg10_ID"].ToString();
                    }
                    catch (Exception ex)
                    {
                        ExDesPri10 = "";
                    }
                    if (!string.IsNullOrEmpty(ExDesPri10))
                    {
                        ExDesPri10 = NKMethod.GetDataTable(ExDesPri10);


                        item["DescFlexField_PrivateDescSeg8"] = ExDesPri10;
                    }
                }
                #endregion 

            }
        }
    }
}
