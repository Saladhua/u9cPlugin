using System;
using UFIDA.U9.FI.ER.ReimburseBillUIModel;
using UFIDA.U9.SCM.INV.MiscShipUIModel;
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
    class NKLMiscShipUIMainFormWebPartExtended : ExtendedPartBase
    {
        private MiscShipUIMainFormWebPart _part;


        public override void AfterRender(IPart part, EventArgs args)
        {
            base.AfterRender(part, args);

            this._part = (part as MiscShipUIMainFormWebPart);

            if (this._part.Model.MiscShipment != null)
            {
                #region 更新项目字段
                foreach (var item in this._part.Model.MiscShipment_MiscShipLs.Records)
                {
                    string BenDesPri10 = "";
                    try
                    {
                        BenDesPri10 = item["BenefitProject_DescFlexField_PrivateDescSeg10_ID"] == null ? "" : item["BenefitProject_DescFlexField_PrivateDescSeg10_ID"].ToString();
                    }
                    catch (Exception ex)
                    {
                        BenDesPri10 = "";
                    }
                    if (!string.IsNullOrEmpty(BenDesPri10))
                    {
                        BenDesPri10 = NKMethod.GetDataTable(BenDesPri10);

                        item["DescFlexField_PrivateDescSeg6"] = BenDesPri10;
                    }
                }
                #endregion 
            }
        }
    }
}
