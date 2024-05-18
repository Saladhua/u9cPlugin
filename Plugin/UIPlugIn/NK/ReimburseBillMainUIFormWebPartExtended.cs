using System;
using UFIDA.U9.FI.ER.ReimburseBillUIModel;
using UFIDA.U9.UI.PDHelper;
using UFSoft.UBF.UI.ControlModel;
using UFSoft.UBF.UI.Custom;
using UFSoft.UBF.UI.IView;
using UFSoft.UBF.UI.WebControlAdapter;


namespace YY.U9.Cust.LI.UIPlugIn
{
    class ReimburseBillMainUIFormWebPartExtended : ExtendedPartBase
    {
        private ReimburseBillMainUIFormWebPart _part;

        IUFButton BtnSettle3;
        /// <summary>
        /// 初始化后扩展
        /// </summary>
        /// <param name="part"></param>
        /// <param name="e"></param>
        public override void AfterInit(IPart part, System.EventArgs e)
        {
            base.AfterInit(part, e);
            _part = part as ReimburseBillMainUIFormWebPart;
            //实例化按钮
            BtnSettle3 = new UFWebButtonAdapter();
            new UFWebButtonAdapter();
            BtnSettle3.Text = "快速分拆";
            BtnSettle3.ID = "BtnSettle3";
            BtnSettle3.AutoPostBack = true;
            BtnSettle3.Click += new EventHandler(BtnAR_Click);
            //加入Card容器
            IUFCard card = (IUFCard)part.GetUFControlByName(part.TopLevelContainer, "Card0");
            card.Controls.Add(BtnSettle3);
            CommonFunction.Layout(card, BtnSettle3, 18, 0);
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

            //this._part.Action.CurrentPart.NavigatePage("7d576ca6-fb91-4c22-9903-43cc99d508bd", null);

            //NavigateManager.ShowModelWebpart(this._part.Action.CurrentPart, "7d576ca6-fb91-4c22-9903-43cc99d508bd", _part.TaskId.ToString());

            NavigateManager.ShowModelWebpart(_part, "7d576ca6-fb91-4c22-9903-43cc99d508bd", _part.TaskId.ToString(), 992, 470, null, true, true);
        }
    }
}
