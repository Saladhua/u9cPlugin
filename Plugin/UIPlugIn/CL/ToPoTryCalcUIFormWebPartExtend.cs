using System;
using System.Text;
using UFIDA.U9.SCM.PM.PRToPOUIModel;
using UFIDA.U9.UI.PDHelper;
using UFSoft.UBF.UI.ControlModel;
using UFSoft.UBF.UI.Custom;
using UFSoft.UBF.UI.IView;
using UFSoft.UBF.UI.MD.Runtime;
using UFSoft.UBF.UI.MD.Runtime.Implement;
using UFSoft.UBF.UI.WebControlAdapter;

namespace YY.U9.Cust.LI.UIPlugIn
{
    /// <summary>
    /// 请购转PO单价取值
    /// 转PO试算提示
    /// </summary>
    class ToPoTryCalcUIFormWebPartExtend : ExtendedPartBase
    {
        private ToPoTryCalcUIFormWebPart _part;
        IUFButton BtnSettle;

        public override void AfterInit(IPart part, EventArgs args)
        {
            //首先调用原来的事件
            base.AfterInit(_part, args);
            this._part = part as ToPoTryCalcUIFormWebPart;
            //实例化按钮
            BtnSettle = new UFWebButtonAdapter();
            new UFWebButtonAdapter();
            BtnSettle.Text = "供应商单价";
            BtnSettle.ID = "BtnSettle";
            BtnSettle.AutoPostBack = true;
            BtnSettle.Click += new EventHandler(BtnAR_Click);
            //加入Card容器
            IUFCard card = (IUFCard)part.GetUFControlByName(part.TopLevelContainer, "Card1");
            card.Controls.Add(BtnSettle);
            CommonFunction.Layout(card, BtnSettle, 6, 0);

        }

        public void BtnAR_Click(object sender, EventArgs e)
        {
            //收集界面错误信息
            if (this._part.Model.ErrorMessage.hasErrorMessage)
            {
                this._part.Model.ClearErrorMessage();
            }
            this._part.OnDataCollect(this);

            #region 循环表体,给出提示
            string Sup = "";
            string ItemID = "";
            string DocLineNo = "";
            string Message = "";
            foreach (var item in this._part.Model.TryCalcView.Records)
            {
                bool IsOK = false;
                Sup = item["Supplier"] == null ? "" : item["Supplier"].ToString();
                ItemID = item["ItemID"] == null ? "" : item["ItemID"].ToString();
                if (!string.IsNullOrEmpty(Sup))
                {
                    UFIDA.U9.Cust.CLLH.CustSupFindPriceSV.Proxy.CustSupFindPriceSVProxy custSupFindPriceSVProxy = new UFIDA.U9.Cust.CLLH.CustSupFindPriceSV.Proxy.CustSupFindPriceSVProxy();
                    custSupFindPriceSVProxy.Org = PDContext.Current.OrgID;
                    custSupFindPriceSVProxy.SupID = Sup;
                    custSupFindPriceSVProxy.ItemID = ItemID;
                    IsOK = custSupFindPriceSVProxy.Do();
                }
                DocLineNo = item["PRDocLineNo"] == null ? "" : item["PRDocLineNo"].ToString();
                if (IsOK == false)
                {
                    Message = Message + "行号" + DocLineNo + "供应商单价为0" + "，\t\t";
                }
            }
            this._part.Model.ErrorMessage.Message = Message;
            #endregion

        }
    }
}
