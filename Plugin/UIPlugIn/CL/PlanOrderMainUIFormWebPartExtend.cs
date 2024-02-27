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

namespace YY.U9.Cust.LI.UIPlugIn
{
    class PlanOrderMainUIFormWebPartExtend : ExtendedPartBase
    {
        private PlanOrderMainUIFormWebPart _part;

        IUFButton BtnSettle3;

        HiddenField wpFindID;

        public override void AfterInit(IPart part, System.EventArgs e)
        {
            base.AfterInit(part, e);
            _part = part as PlanOrderMainUIFormWebPart;
            //实例化按钮
            BtnSettle3 = new UFWebButtonAdapter();
            new UFWebButtonAdapter();
            BtnSettle3.Text = "仓库确认";
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

            //NaviteParamter param = new NaviteParamter();
            //param.NameValues.Add("CtrlId", "816c34a1-8766-4ff3-8e60-c615e325a0fd");
            //NavigateManager.ShowModelWebpart(this._part.Action.CurrentPart, "bd80094b-7b35-4ffe-8c81-964544824b72", _part.TaskId.ToString());
            NavigateManager.ShowModelWebpart(_part, "bd80094b-7b35-4ffe-8c81-964544824b72", _part.TaskId.ToString(), 300, 250, null, true, true);
        }

        public override void BeforeRender(IPart _part, EventArgs args)
        {
            //获取料品
            string Item = "";

            string DocNo = "";

            string yjlyl = "";
            string yjhjl = "";
            string qgsl = "";

            this._part = (_part as PlanOrderMainUIFormWebPart);



            string items = "";
            DataTable dt = new DataTable();
            bool set = false;//set 用来判断是否重新选了料品筛选
            //调用模版提供的默认实现.--默认实现可能会调用相应的Action.


            if (this._part.CurrentSessionState["ResultItemMaster"] != null)
            {
                ////this.Model.BatchingPlan.AddNewUIRecord();
                //this.Model.ApAddForPart.Clear();//清理
                dt = this._part.CurrentSessionState["ResultItemMaster"] as DataTable;
                this._part.CurrentSessionState["ResultItemMaster"] = null;
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    DataRow dr = dt.Rows[i];
                    items = items + "'" + dr["ID"].ToString() + "'" + ",";
                }
                set = true;
            }

            foreach (var item in this._part.Model.PlanOrder.Records)
            {
                try
                {
                    Item = item["Item"] == null ? "" : item["Item"].ToString();
                    DocNo = item["DocNo"] == null ? "" : item["DocNo"].ToString();
                }
                catch (Exception ex)
                {
                    //string message = "料号或单号为空";
                    //throw new Exception(message);
                    continue;
                }
                if (!string.IsNullOrEmpty(Item))
                {
                    UFIDA.U9.Cust.CL.LI.Cust_FindPlanOrderBP.Proxy.FindOperationProxy operationProxy = new FindOperationProxy();
                    operationProxy.Item = Item;
                    operationProxy.DocNo = DocNo;
                    UFIDA.U9.Cust.CL.LI.Cust_FindPlanOrderBP.ResultDtoData see1 = operationProxy.Do();

                    yjlyl = see1.Yjlyl;
                    yjhjl = see1.Yjlyl;
                    qgsl = see1.Qgsl;
                    try
                    {
                        item["DescFlexField_PrivateDescSeg3"] = yjlyl;
                        item["DescFlexField_PrivateDescSeg4"] = yjhjl;
                        item["DescFlexField_PrivateDescSeg5"] = qgsl;
                        string D2 = item["DescFlexField_PrivateDescSeg2"] == null ? "0" : item["DescFlexField_PrivateDescSeg2"].ToString();
                        if (string.IsNullOrEmpty(D2))
                        {
                            D2 = "0";
                        }
                        string D8 = item["DescFlexField_PrivateDescSeg8"] == null ? "0" : item["DescFlexField_PrivateDescSeg8"].ToString();
                        if (string.IsNullOrEmpty(D8))
                        {
                            D8 = "0";
                        }
                        string yjkll = (decimal.Parse(yjlyl) + decimal.Parse(yjhjl) + decimal.Parse(qgsl) + decimal.Parse(D2) + decimal.Parse(D8)).ToString();
                        item["DescFlexField_PrivateDescSeg7"] = yjkll;
                        item["DescFlexField_PrivateDescSeg1"] = see1.Dtjz;
                    }
                    catch (Exception ex)
                    {
                        continue;
                    }
                }
            }

            base.BeforeRender(_part, args);
        }

    }
}
