using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using UFIDA.U9.MFG.MO.MODivideUIModel;
using UFIDA.U9.UI.PDHelper;
using UFSoft.UBF.UI.ControlModel;
using UFSoft.UBF.UI.Custom;
using UFSoft.UBF.UI.IView;
using UFSoft.UBF.UI.WebControlAdapter;


namespace YY.U9.Cust.LI.UIPlugIn
{
    class MODivideMainUIFormWebPartExtend : ExtendedPartBase
    {
        private MODivideMainUIFormWebPart _part;
        //HiddenField wpFindID;
        IUFButton BtnSettle;


        /// <summary>
        /// 初始化后扩展
        /// </summary>
        /// <param name="part"></param>
        /// <param name="e"></param>

        public override void AfterInit(IPart part, System.EventArgs e)
        {
            base.AfterInit(part, e);
            _part = part as MODivideMainUIFormWebPart;
            //实例化按钮
            BtnSettle = new UFWebButtonAdapter();
            new UFWebButtonAdapter();
            BtnSettle.Text = "备料复制";
            BtnSettle.ID = "BtnSettle";
            BtnSettle.AutoPostBack = true;
            BtnSettle.Click += new EventHandler(BtnAR_Click);
            //加入Card容器
            IUFCard card = (IUFCard)part.GetUFControlByName(part.TopLevelContainer, "Card0");
            card.Controls.Add(BtnSettle);
            CommonFunction.Layout(card, BtnSettle, 8, 0);

        }

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

            foreach (var item in this._part.Model.DivideMO.Records)
            {
                string see = item["DocNo"].ToString();
                HttpContext.Current.Session["MO"] = item["DocNo"].ToString();
            }

            foreach (var item in this._part.Model.NewMO.Records)
            {
                string see = item["DocNo"].ToString();
                HttpContext.Current.Session["NewMO"] = item["DocNo"].ToString();
            }

            NavigateManager.ShowModelWebpart(_part, "55499195-262c-4ea8-b986-a24216bcfb59", _part.TaskId.ToString(), 992, 420, null, true, true);
        }
    }
}

