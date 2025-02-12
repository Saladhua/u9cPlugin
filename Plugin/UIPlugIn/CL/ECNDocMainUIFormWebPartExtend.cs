using System;
using UFIDA.U9.MFG.ECN.ECNDocUIModel;
using UFSoft.UBF.UI.ControlModel;
using UFSoft.UBF.UI.Custom;
using UFSoft.UBF.UI.IView;
using UFSoft.UBF.UI.WebControlAdapter;


namespace YY.U9.Cust.LI.UIPlugIn
{
    /// <summary>
    /// 系统会报错
    /// </summary>
    class ECNDocMainUIFormWebPartExtend : ExtendedPartBase
    {
        private ECNDocMainUIFormWebPart _part;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="part"></param>
        /// <param name="args"></param>

        public override void AfterInit(IPart part, EventArgs args)
        {
            base.AfterInit(part, args);
            this._part = part as ECNDocMainUIFormWebPart;
            //（1）、实例化按钮
            IUFButton BtnAR = new UFWebButtonAdapter();
            BtnAR.Text = "发行失败";
            BtnAR.ID = "BtnECNDoc";
            BtnAR.AutoPostBack = true;
            //（2）、加入功能栏Card中
            IUFCard card = (IUFCard)part.GetUFControlByName(part.TopLevelContainer, "Card0");
            card.Controls.Add(BtnAR);
            //（3）、设置按钮在容器中的位置
            CommonFunction.Layout(card, BtnAR, 20, 0);//一般为从左往右按钮个数乘以2
            //（4）、绑定按钮事件
            BtnAR.Click += new EventHandler(BtnECNDoc_Click);
        } 

        public void BtnECNDoc_Click(object sender, EventArgs e)
        {
            //收集界面错误信息
            if (this._part.Model.ErrorMessage.hasErrorMessage)
            {
                this._part.Model.ClearErrorMessage();
            }
            this._part.OnDataCollect(this);

            string IsBatchChange = "";//批量变更

            IsBatchChange = this._part.Model.ECNDoc.FocusedRecord["IsBatchChange"].ToString();

            //批量变更，勾选或者不勾选，不勾选直接调用接口，勾选之后需要调用自己做的接口

            if (IsBatchChange == "False")
            {
                #region 循环调用接口

                #endregion
            }

        }
    }
}
