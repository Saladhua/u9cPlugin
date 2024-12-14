using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UFIDA.U9.SCM.PM.RFQUIModel;
using UFIDA.U9.UI.PDHelper;
using UFSoft.UBF.UI.ControlModel;
using UFSoft.UBF.UI.Custom;
using UFSoft.UBF.UI.IView;
using UFSoft.UBF.UI.WebControlAdapter;

namespace YY.U9.Cust.LI.UIPlugIn
{
    /// <summary>
    /// 恺之供应商报价记录功能
    /// D:\setups\porject\恺之\2024-08
    /// </summary>
    class RFQMainUIFormWebPartExtended : ExtendedPartBase
    {
        private RFQMainUIFormWebPart _part;

        IUFButton BtnSettle3;

        IUFButton BtnSettle4;
        /// <summary>
        /// 初始化后扩展
        /// </summary>
        /// <param name="part"></param>
        /// <param name="e"></param>
        public override void AfterInit(IPart part, System.EventArgs e)
        {
            base.AfterInit(part, e);
            _part = part as RFQMainUIFormWebPart;
            //实例化按钮
            BtnSettle3 = new UFWebButtonAdapter();
            new UFWebButtonAdapter();
            BtnSettle3.Text = "报价撤销";
            BtnSettle3.ID = "BtnSettle3";
            BtnSettle3.AutoPostBack = true;
            BtnSettle3.Click += new EventHandler(BtnAR_Click);
            //加入Card容器
            IUFCard card = (IUFCard)part.GetUFControlByName(part.TopLevelContainer, "Card0");
            card.Controls.Add(BtnSettle3);
            CommonFunction.Layout(card, BtnSettle3, 12, 0);

            base.AfterInit(part, e);
            _part = part as RFQMainUIFormWebPart;
            //实例化按钮
            BtnSettle4 = new UFWebButtonAdapter();
            new UFWebButtonAdapter();
            BtnSettle4.Text = "报价记录查询";
            BtnSettle4.ID = "BtnSettle4";
            BtnSettle4.AutoPostBack = true;
            BtnSettle4.Click += new EventHandler(BtnAC_Click);
            //加入Card容器
            IUFCard card2 = (IUFCard)part.GetUFControlByName(part.TopLevelContainer, "Card0");
            card2.Controls.Add(BtnSettle4);
            CommonFunction.Layout(card2, BtnSettle4, 14, 0);
        }

        [Obsolete]
        public void BtnAR_Click(object sender, EventArgs e)
        {
            //收集界面错误信息
            if (this._part.Model.ErrorMessage.hasErrorMessage)
            {
                this._part.Model.ClearErrorMessage();
            }
            this._part.OnDataCollect(this);

            string DocLineNo = "";

            string LineID = "";

            string ID = "";

            string Status = "";

            foreach (var item in this._part.Model.RequestForQuotation.Records)
            {
                ID = item["ID"].ToString();
            }

            //foreach (var item in this._part.Model.RequestForQuotation_RFQLines.Records)
            //{
            //    DocLineNo = item["DocLineNo"].ToString();

            //    LineID = item["ID"].ToString();

            //    Status = item["Status"].ToString();
            //}
            if (double.Parse(ID) > 0)
            {
                this._part.CurrentState["RFQID"] = ID;
            }


            #region 操作发布--好使
            //if (double.Parse(ID) > 0)
            //{
            //    List<long> longs = new List<long>();

            //    long id = long.Parse(ID);

            //    longs.Add(id);

            //    UFIDA.U9.PPR.RFQ.Proxy.RFQApprovedProxy rFQApprovedProxy = new UFIDA.U9.PPR.RFQ.Proxy.RFQApprovedProxy();
            //    rFQApprovedProxy.RFQDocKey = long.Parse(ID);
            //    rFQApprovedProxy.ActivityType = 12;
            //    rFQApprovedProxy.IsApproved = false;
            //    rFQApprovedProxy.Do();
            //}
            #endregion

            NavigateManager.ShowModelWebpart(_part, "ea003f06-b0c8-4f6e-9428-124c1d425613", _part.TaskId.ToString(), 300, 300, null, true, true);
        }
        [Obsolete]
        public void BtnAC_Click(object sender, EventArgs e)
        {
            //收集界面错误信息
            if (this._part.Model.ErrorMessage.hasErrorMessage)
            {
                this._part.Model.ClearErrorMessage();
            }
            this._part.OnDataCollect(this);

            NavigateManager.ShowModelWebpart(_part, "ea003f06-b0c8-4f6e-9428-124c1d425613", _part.TaskId.ToString(), 992, 470, null, true, true);
        }
    }
}
