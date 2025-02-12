using System;
using System.Web.UI.WebControls;
using UFIDA.U9.MFG.MO.MOChangeItemUIModel;
using UFSoft.UBF.UI.ControlModel;
using UFSoft.UBF.UI.Custom;
using UFSoft.UBF.UI.IView;
using UFSoft.UBF.UI.WebControlAdapter;

namespace YY.U9.Cust.LI.UIPlugIn
{
    class MOChangeItemMainUIFormWebPartExtend : ExtendedPartBase
    {
        private MOChangeItemMainUIFormWebPart _part;
   
        IUFButton BtnSettle;

        /// <summary>
        /// 初始化后扩展
        /// </summary>
        /// <param name="part"></param>
        /// <param name="e"></param>
        [Obsolete]
        public override void AfterInit(IPart part, System.EventArgs e)
        {
            base.AfterInit(part, e);
            _part = part as MOChangeItemMainUIFormWebPart;
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

        [Obsolete]
        public void BtnAR_Click(object sender, EventArgs e)
        {
            //收集界面错误信息
            if (this._part.Model.ErrorMessage.hasErrorMessage)
            {
                this._part.Model.ClearErrorMessage();
            }
            this._part.OnDataCollect(this);


            //判断两个单号均有值

            string MO = "";
            string NewMO = "";

            int i = 0;

            foreach (var item in this._part.Model.TargetMO.Records)
            {
                if (i == 0)
                {
                    MO = item["MO"].ToString();
                }
                if (i == 1)
                {
                    NewMO = item["MO"].ToString();
                }
                i++;
            }


            string ItemID = "";
            //有值就备料复制启动
            if (!string.IsNullOrEmpty(MO) && !string.IsNullOrEmpty(NewMO))
            {
                UFIDA.U9.Cust.LI.CL.MoItemBP.Proxy.MOChangeProxy mOChange = new UFIDA.U9.Cust.LI.CL.MoItemBP.Proxy.MOChangeProxy();
                mOChange.NewMO = NewMO;
                mOChange.MO = MO;
                mOChange.Item = ItemID;
                bool see = mOChange.Do();

                if (see)
                {
                    this._part.Model.ErrorMessage.Message = "成功";
                }
                else
                {
                    this._part.Model.ErrorMessage.Message = "失败";
                }
            }
        }
    }
}
