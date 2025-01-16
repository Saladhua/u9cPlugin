using System;
using UFIDA.U9.FI.AP.APMaintenanceUIModel;
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
    class NKLAPMainUIFormWebPartExtended : ExtendedPartBase
    {
        private APMainUIFormWebPart _part;

        public override void AfterRender(IPart part, EventArgs args)
        {
            base.AfterRender(part, args);

            this._part = (part as APMainUIFormWebPart);

            if (this._part.Model.APBillHead != null)
            {
                #region 更新项目字段
                foreach (var item in this._part.Model.APBillHead_APBillLines.Records)
                {
                    string PDesPri10 = "";
                    try
                    {
                        PDesPri10 = item["Project"] == null ? "" : item["Project"].ToString();
                    }
                    catch (Exception ex)
                    {
                        PDesPri10 = "";
                    }
                    if (!string.IsNullOrEmpty(PDesPri10))
                    {

                        PDesPri10 = NKMethod.GetDataTable(PDesPri10);

                        item["DescFlexField_PrivateDescSeg6"] = PDesPri10;
                    }
                }
                #endregion 

            }
        }
    }
}
