using System;
using System.Data;
using System.Web.UI.WebControls;
using UFIDA.U9.Cust.CL.LI.Cust_FindPlanOrderBP.Proxy;
using UFIDA.U9.MFG.MRP.PlanOrderUIModel;
using UFIDA.U9.UI.PDHelper;
using UFSoft.UBF.UI.ControlModel;
using UFSoft.UBF.UI.Custom;
using UFSoft.UBF.UI.IView;
using UFSoft.UBF.UI.WebControls;
using UFSoft.UBF.UI.WebControlAdapter;
using System.Web;
using UFIDA.U9.PPR.PurDiscountPolicyUI;
using System.Collections.Generic;
using UFSoft.UBF.UI.MD.Runtime;
using UFIDA.U9.Cust.YK.LI.Common;
using System.IO;

namespace YY.U9.Cust.LI.UIPlugIn
{
    class PurDiscountPolicyMainUIFormWebPartExtend : ExtendedPartBase
    {
        private PurDiscountPolicyMainUIFormWebPart _part;


        IUFButton BtnImport;


        /// <summary>
        /// 初始化后扩展
        /// </summary>
        /// <param name="part"></param>
        /// <param name="e"></param>        
        public override void AfterInit(IPart part, System.EventArgs e)
        {
            base.AfterInit(part, e);
            _part = part as PurDiscountPolicyMainUIFormWebPart;

            //实例化按钮
            BtnImport = new UFWebButtonAdapter();
            BtnImport.Text = "批量导入";
            BtnImport.ID = "BtnImport";
            BtnImport.AutoPostBack = true;
            BtnImport.Click += new EventHandler(BtnImportAP_Click);
            //加入Card容器
            IUFCard card = (IUFCard)_part.GetUFControlByName(part.TopLevelContainer, "Card0");
            card.Controls.Add(BtnImport);
            CommonFunction.Layout(card, BtnImport, 4, 0);

        }

        public override void AfterRender(IPart Part, EventArgs args)
        {
            //base.AfterRender(Part, args);

        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void BtnImportAP_Click(object sender, EventArgs e)
        {
            //收集界面错误信息
            if (this._part.Model.ErrorMessage.hasErrorMessage)
            {
                this._part.Model.ClearErrorMessage();
            }
            this._part.OnDataCollect(this);
            List<string> listStr = new List<string>();
            try
            {
                //弹出文件导入窗口
                //导入,会在内存里面FileAddress保存这次导入的地址
                this._part.ShowModalDialog("8998a5ce-685e-4783-95a2-bd1a6da99001", "批量导入", "520", "250", this._part.TaskId.ToString());

            }
            catch (Exception ex)
            {
                IUIModel model = this._part.Model;
                this._part.Model.ErrorMessage.SetErrorMessage(ref model, ex);
            }
        }


        public override void AfterLoad(IPart Part, EventArgs args)
        {
            string FileAddress = "";
            if (HttpContext.Current.Session["FileAddress"] != null)
            {
                FileAddress = HttpContext.Current.Session["FileAddress"].ToString();

                HttpContext.Current.Session["FileAddress"] = null;


                UFIDA.U9.Cust.CLLH.CustPurDisPolImportBP.Proxy.PurDisPolImportBPProxy operation = new UFIDA.U9.Cust.CLLH.CustPurDisPolImportBP.Proxy.PurDisPolImportBPProxy();

                //UFIDA.U9.Cust.YK.LI.Common.Proxy.YKServiceProxy operation = new UFIDA.U9.Cust.YK.LI.Common.Proxy.YKServiceProxy();

                operation.ExcelPath = FileAddress;

                //operation.BusinessType = "PurDisPolImport";

                //operation.ActionType = FileAddress;

                bool see = operation.Do();

                string see1 = see.ToString();

                string see2 = "失败";

                if (see1 == "True")
                {
                    see2 = "成功";
                }
                this._part.ShowWindowStatus(see2);
            }
        }
    }

}

