using System;
using System.Collections.Generic;
using System.Data;
using System.Web;
using System.Web.UI.WebControls;
using UFIDA.U9.FI.AR.ReceivalUI.ReceivalUIModel;
using UFIDA.U9.FI.TI.InvoiceUI.InvoiceBListUIModel;
using UFIDA.U9.PM.PO.POMainListUI;
using UFIDA.U9.UI.PDHelper;
using UFSoft.UBF.UI.ControlModel;
using UFSoft.UBF.UI.Custom;
using UFSoft.UBF.UI.Engine.Builder;
using UFSoft.UBF.UI.IView;
using UFSoft.UBF.UI.MD.Runtime;
using UFSoft.UBF.UI.WebControlAdapter;
using UFSoft.UBF.Util.DataAccess;

namespace YY.U9.Cust.LI.UIPlugIn
{
    /// <summary>
    /// 在采购订单列表，同软硬件收入按钮新增一个按钮“间隔天数”
    /// D:\setups\porject\创联\2024-09 创联工单需求及门户相关需求
    /// </summary>
    class POMainListUIFormWebPartExtended : ExtendedPartBase
    {
        private POMainListUIFormWebPart _part;

        private IUFButton BtnCalcMoney2;

        public override void AfterInit(IPart part, System.EventArgs e)
        {
            base.AfterInit(part, e);
            this._part = (part as POMainListUIFormWebPart);
            IUFToolbar iuftoolbar = (IUFToolbar)this._part.GetUFControlByName(part.TopLevelContainer, "Toolbar1");
            bool flag = iuftoolbar != null;
            if (flag)
            {
                string text = "9884A728-4551-4966-BBC7-82E247603C9D";
                this.BtnCalcMoney2 = UIControlBuilder.BuilderToolbarButton(iuftoolbar, "True", "BtnCalcMoney", "True", "True", 70, 28, "8", "", true, false, text, text, text);
                UIControlBuilder.SetButtonAccessKey(this.BtnCalcMoney2);
                this.BtnCalcMoney2.Text = "间隔天数";
                this.BtnCalcMoney2.ID = "BtnCalcMoney2";
                this.BtnCalcMoney2.AutoPostBack = true;
                this.BtnCalcMoney2.UIModel = this._part.Model.ElementID;
                ((UFWebToolbarAdapter)iuftoolbar).Items.Add(this.BtnCalcMoney2 as WebControl);
                this.BtnCalcMoney2.Click += this.BtnCalcMoney_Click2;
            }
        }

        public void BtnCalcMoney_Click2(object sender, EventArgs e)
        {
            bool hasErrorMessage = this._part.Model.ErrorMessage.hasErrorMessage;
            if (hasErrorMessage)
            {
                this._part.Model.ClearErrorMessage();
            }
            this._part.OnDataCollect(this);
            try
            {
                IList<IUIRecord> selectRecord = this._part.Model.PurchaseOrder.Cache.GetSelectRecord();
                bool flag = selectRecord == null || selectRecord.Count == 0;
                if (flag)
                {
                    throw new Exception("未选择任何采购行！");
                }
                List<string> list = new List<string>();
                //采购订单增加自定义字段4--间隔天数，取供应商确认日期与审核日期的天数
                foreach (IUIRecord iuirecord in selectRecord)
                {
                    if (iuirecord["ApprovedOn"] != null && iuirecord["POLines_POShiplines_VPConfirmTradeDate"] != null)
                    {

                        string ApprovedOn = iuirecord["ApprovedOn"].ToString();

                        string VPConfirmTradeDate = iuirecord["POLines_POShiplines_VPConfirmTradeDate"].ToString();

                        if (!string.IsNullOrEmpty(ApprovedOn) && !string.IsNullOrEmpty(VPConfirmTradeDate))
                        {
                            DateTime dtApprovedOn1 = Convert.ToDateTime(ApprovedOn);

                            DateTime dtApprovedOn = Convert.ToDateTime(dtApprovedOn1.ToString("yyyy-MM-dd"));

                            DateTime dtVPConfirmTradeDate1 = Convert.ToDateTime(VPConfirmTradeDate);

                            DateTime dtVPConfirmTradeDate = Convert.ToDateTime(dtVPConfirmTradeDate1.ToString("yyyy-MM-dd"));

                            System.TimeSpan ts = dtVPConfirmTradeDate - dtApprovedOn;

                            int days = ts.Days;

                            iuirecord["POLines_DescFlexSegments_PrivateDescSeg4"] = days;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                IUIModel model = this._part.Model;
                this._part.Model.ErrorMessage.SetErrorMessage(ref model, ex);
            }
        }
    }
}
