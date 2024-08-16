using System;
using System.Data;
using System.Web;
using System.Web.UI.WebControls;
using UFIDA.U9.Cust.CL.LI.Cust_FindPlanOrderBP.Proxy;
using UFIDA.U9.MFG.MRP.PlanOrderUIModel;
using UFIDA.U9.UI.PDHelper;
using UFSoft.UBF.UI.ControlModel;
using UFSoft.UBF.UI.Custom;
using UFSoft.UBF.UI.IView;
using UFSoft.UBF.UI.WebControlAdapter;
using UFSoft.UBF.Util.DataAccess;

namespace YY.U9.Cust.LI.UIPlugIn
{
    class PlanOrderMainUIFormWebPartExtend : ExtendedPartBase
    {
        private PlanOrderMainUIFormWebPart _part;

        IUFButton BtnSettle3;
        /// <summary>
        /// 初始化后扩展
        /// </summary>
        /// <param name="part"></param>
        /// <param name="e"></param>
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
            //GetData7();

            ////实例化按钮
            //BtnSettle4 = new UFWebButtonAdapter();
            //new UFWebButtonAdapter();
            //BtnSettle4.Text = "汇入数据";
            //BtnSettle4.ID = "BtnSettle4";
            //BtnSettle4.AutoPostBack = true;
            //BtnSettle4.Click += new EventHandler(BtnFind_Click);
            ////加入Card容器
            //IUFCard card2 = (IUFCard)part.GetUFControlByName(part.TopLevelContainer, "Card0");
            //card2.Controls.Add(BtnSettle4);
            //CommonFunction.Layout(card2, BtnSettle4, 18, 0);
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
            NavigateManager.ShowModelWebpart(_part, "bd80094b-7b35-4ffe-8c81-964544824b72", _part.TaskId.ToString(), 300, 270, null, true, true);
        }

        /// <summary>
        /// 有
        /// </summary>
        /// <param name="Part"></param>
        /// <param name="executeDefault"></param>
        public override void BeforeDataBinding(IPart Part, out bool executeDefault)
        {
            if (_part.Model.PlanOrder != null)
            {
                int see = this._part.Model.PlanOrder.Records.Count;
            }
            base.BeforeDataBinding(Part, out executeDefault);
            GetData7();
        }

        ///// <summary>
        ///// 有
        ///// </summary>
        ///// <param name="Part"></param>
        ///// <param name="executeDefault"></param>
        //public override void BeforeDataLoad(IPart Part, out bool executeDefault)
        //{
        //    if (_part.Model.PlanOrder != null)
        //    {
        //        int see = this._part.Model.PlanOrder.Records.Count;
        //    }
        //    base.BeforeDataLoad(Part, out executeDefault);  
        //}

        ///// <summary>
        ///// 有
        ///// </summary>
        ///// <param name="Part"></param>
        ///// <param name="args"></param>
        //public override void BeforeLoad(IPart Part, EventArgs args)
        //{
        //    if (_part.Model.PlanOrder != null)
        //    {
        //        int see = this._part.Model.PlanOrder.Records.Count;
        //    }
        //    base.BeforeLoad(Part, args);
        //}

        [Obsolete]
        public void BtnFind_Click(object sender, EventArgs e)
        {

        }
        public void GetData7()
        //[Obsolete]
        //public void BtnFind_Click(object sender, EventArgs e)
        {
            //获取料品
            string Item = "";

            string ItemCode = "";

            string DocNo = "";

            string yjlyl = "0";
            string yjhjl = "0";
            string qgsl = "0";
            string ckkc = "0"; //仓库库存字段默认带出范围内仓库库存
            string dtyl = "0";
            string aqkc = "0";
            string mrkc = "0";

            this._part = (_part as PlanOrderMainUIFormWebPart);
            string whs = "";
            DataTable dt = new DataTable();
            bool set = false;//set 用来判断是否重新选了料品筛选
            //调用模版提供的默认实现.--默认实现可能会调用相应的Action.

            if (HttpContext.Current.Session["Doc_Code"] != null)
            {
                string text = HttpContext.Current.Session["Doc_Code"].ToString();
                ////this.Model.BatchingPlan.AddNewUIRecord();
                //this.Model.ApAddForPart.Clear();//清理
                dt = HttpContext.Current.Session["Doc_Code"] as DataTable;
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    DataRow dr = dt.Rows[i];
                    whs = whs + "'" + dr["ID"].ToString() + "'" + ",";
                }
                set = true;
            }


            //Item = "1002207080050708";
            //UFIDA.U9.Cust.CL.LI.Cust_FindPlanOrderBP.Proxy.FindOperationProxy operationProxy1 = new FindOperationProxy();
            //operationProxy1.Item = Item;
            //operationProxy1.DocNo = DocNo;
            //operationProxy1.Wh = whs;
            //UFIDA.U9.Cust.CL.LI.Cust_FindPlanOrderBP.ResultDtoData see2 = operationProxy1.Do();
            int j = 0;

            foreach (var item in this._part.Model.PlanOrder.Records)
            {
                string descFlexField_PrivateDescSeg2 = item["DescFlexField_PrivateDescSeg2"] == null ? "" : item["DescFlexField_PrivateDescSeg2"].ToString();

                if (!string.IsNullOrEmpty(descFlexField_PrivateDescSeg2) && j == 0)
                {
                    //HttpContext.Current.Session["Doc_Code"] = null;
                }
                j = 1;
                try
                {
                    Item = item["Item"] == null ? "" : item["Item"].ToString();
                    ItemCode = item["ItemCode"] == null ? "" : item["ItemCode"].ToString();
                    DocNo = item["DocNo"] == null ? "" : item["DocNo"].ToString();
                }
                catch (Exception ex)
                {
                    //string message = "料号或单号为空";
                    //throw new Exception(message);
                    continue;
                }

                //计划区域维护的仓库
                //会用变动他加我加，他减我减
                string DefWh = "'1002310200118356','1002310200118359','1002310200118360','1002310200118434','1002310200118437'," +
                    "'1002310200118438','1002310200118452','1002310200118521','1002310200118523','1002310200118527','1002310200118568'," +
                    "'1002310200118577','1002310200118591','1002310200118603','1002310200118607','1002310200118648','1002310200118649','1002310200118704','1002310200118605','1002310200118667',";//默认存储地点

                if (!string.IsNullOrEmpty(Item))
                {
                    UFIDA.U9.Cust.CL.LI.Cust_FindPlanOrderBP.Proxy.FindOperationProxy operationProxy = new FindOperationProxy();
                    operationProxy.Item = Item;
                    operationProxy.DocNo = DocNo;
                    operationProxy.Wh = whs;
                    operationProxy.DefWh = DefWh;
                    UFIDA.U9.Cust.CL.LI.Cust_FindPlanOrderBP.ResultDtoData see1 = operationProxy.Do();

                    yjlyl = see1.Yjlyl == "" ? "0" : see1.Yjlyl;
                    yjhjl = see1.Yjjhl == "" ? "0" : see1.Yjjhl;
                    qgsl = see1.Qgsl == "" ? "0" : see1.Qgsl;
                    ckkc = see1.CKKC;
                    dtyl = see1.Dtyl == "" ? "0" : see1.Dtyl;
                    aqkc = see1.Aqkc == "" ? "0" : see1.Aqkc;
                    mrkc = see1.Mrkc;

                    if (string.IsNullOrEmpty(ckkc) || ckkc == null)
                    {
                        ckkc = "0";
                    }
                    if (string.IsNullOrEmpty(mrkc) || mrkc == null)
                    {
                        mrkc = "0";
                    }
                    try
                    {
                        string yjkll = "0";
                        DataTable dataTable = new DataTable();
                        DataSet dataSet = new DataSet();
                        string sqlForMoDocNoID = "exec Cust_GongXuMingxi @SPECS=NULL,@ItemName=NULL,@Org=N'10',@ItemCode=N' (ItemCode = N''" + ItemCode + "'') '";
                        DataAccessor.RunSQL(DataAccessor.GetConn(), sqlForMoDocNoID, null, out dataSet);
                        dataTable = dataSet.Tables[0];
                        if (dataTable != null && dataTable.Rows.Count > 0)
                        {

                            yjlyl = (decimal.Parse(dataTable.Rows[0]["yjgdlll"].ToString()) + decimal.Parse(dataTable.Rows[0]["yjwxll"].ToString())).ToString();
                            yjkll = (decimal.Parse(dataTable.Rows[0]["kykcl"].ToString())).ToString();
                            mrkc = (decimal.Parse(dataTable.Rows[0]["xykcl"].ToString())).ToString(); 
                        }
                        //字段有小数位按计量单位小数位保留，没有小数去掉多余的0
                        item["DescFlexField_PrivateDescSeg3"] = Math.Round(decimal.Parse(yjlyl), 4).ToString("0.####");
                        item["DescFlexField_PrivateDescSeg2"] = Math.Round(decimal.Parse(mrkc), 4).ToString("0.####"); //mrkc;
                        item["DescFlexField_PrivateDescSeg4"] = Math.Round(decimal.Parse(yjhjl), 4).ToString("0.####"); //yjhjl;
                        item["DescFlexField_PrivateDescSeg5"] = Math.Round(decimal.Parse(qgsl), 4).ToString("0.####"); //qgsl;
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
                        //预计可用量 = 仓库库存（私有字段2） + 请购数量（私有字段5） + 预计进货量（私有字段4） - 安全库存 - 预计领用量（私有字段3）
                        #region 旧的
                        //(废弃)仓库库存 + (废弃)预计进货量 - (废弃)安全库存 - (废弃)预计领用量 + 请购数量(废弃)
                        #endregion 
                        //string yjkll = (decimal.Parse(yjlyl) + decimal.Parse(yjhjl) + decimal.Parse(qgsl) + decimal.Parse(D2) + decimal.Parse(D8)).ToString();

                        //string newaqkc = item["Item_InventoryInfo_SafetyStockQty"] == null ? "0" : item["Item_InventoryInfo_SafetyStockQty"].ToString();
                        //yjkll = (decimal.Parse(mrkc) + decimal.Parse(qgsl) + decimal.Parse(yjhjl) - decimal.Parse(newaqkc) - decimal.Parse(yjlyl)).ToString();
                        item["DescFlexField_PrivateDescSeg7"] = Math.Round(decimal.Parse(yjkll), 4).ToString("0.####");// yjkll;
                        item["DescFlexField_PrivateDescSeg1"] = see1.Dtjz;
                        item["DescFlexField_PrivateDescSeg6"] = Math.Round(decimal.Parse(dtyl), 4).ToString("0.####");

                        item["DescFlexField_PrivateDescSeg8"] = Math.Round(decimal.Parse(ckkc), 4).ToString("0.####");
                    }
                    catch (Exception ex)
                    {
                        continue;
                    }
                }
            }
        }

    }
}
