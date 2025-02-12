using System;
using System.Data;
using System.Web;
using UFIDA.U9.FI.AR.ReceivalUI.ReceivalUIModel;
using UFIDA.U9.UI.PDHelper;
using UFSoft.UBF.UI.ControlModel;
using UFSoft.UBF.UI.Custom;
using UFSoft.UBF.UI.IView;
using UFSoft.UBF.UI.WebControlAdapter;

namespace YY.U9.Cust.LI.UIPlugIn
{
    /// <summary>
    /// 收款单引用销售订单--开发文档名称--纳科补充开发事项711(2)
    /// 位置 D:\setups\porject\纳科\2024-07
    /// </summary>
    class ReceivalMainUIFormWebPartExtended : ExtendedPartBase
    {
        private ReceivalMainUIFormWebPart _part;

        IUFButton BtnSettle3;

        /// <summary>
        /// 初始化后扩展
        /// </summary>
        /// <param name="part"></param>
        /// <param name="e"></param>
        [Obsolete]
        public override void AfterInit(IPart part, System.EventArgs e)
        {
            base.AfterInit(part, e);
            _part = part as ReceivalMainUIFormWebPart;
            //实例化按钮
            BtnSettle3 = new UFWebButtonAdapter();
            new UFWebButtonAdapter();
            BtnSettle3.Text = "销售订单";
            BtnSettle3.ID = "BtnSettle3";
            BtnSettle3.AutoPostBack = true;
            BtnSettle3.Click += new EventHandler(BtnAR_Click);
            //加入Card容器
            IUFCard card = (IUFCard)part.GetUFControlByName(part.TopLevelContainer, "Card0");
            card.Controls.Add(BtnSettle3);
            CommonFunction.Layout(card, BtnSettle3, 16, 0);


            DataTable dt = new DataTable();
            if (this._part.CurrentSessionState["ReTurnDates"] != null)
            {
                dt = this._part.CurrentSessionState["ReTurnDates"] as DataTable;//ReTurnDates--存储的是客开页面点确定保存的一个集合
                this._part.CurrentSessionState["ReTurnDates"] = null;

                int RecBillUseLinesDocLineNo = 1;
                //循环赋值
                //SrcBillNum -- 来源单据号 
                //SrcBillID -- 来源单据ID 
                //SrcDataID -- 来源单据行ID 
                //SrcDataLineNum -- 来源单据行行号

                foreach (var item in this._part.Model.RecBillHead_RecBillLines_RecBillUseLines.Records)
                {
                    RecBillUseLinesDocLineNo = int.Parse(item["LineNum"].ToString()) + 1;
                    if (dt.Rows.Count != 0)
                    {
                        DataRow dr = dt.Rows[0];
                        item["Cust_Customer"] = Convert.ToInt64(dr["Customer"].ToString());
                        item["Cust_Code"] = dr["CustomerCode"].ToString();
                        item["Cust_Name"] = dr["CustomerName"].ToString();
                        item["RecProperty"] = 3;
                        item["EffectiveDate"] = DateTime.Parse(dr["BussinessDate"].ToString());
                        item["TotalMoney_OCMoney"] = decimal.Parse(dr["Money"].ToString());
                        item["PreRecObj"] = 0;
                        item["CusSrcData"] = Convert.ToInt64(dr["SODoc"].ToString());
                        item["CusSrcData_Code"] = dr["SODocCode"].ToString();
                        item["CusSrcData_Name"] = dr["SODocName"].ToString();
                        item["SrcDataID"] = Convert.ToInt64(dr["SrcDataID"].ToString());//来源单据行ID 
                        item["SrcBillNum"] = dr["SODocCode"].ToString(); //来源单据号
                        item["SrcBillType"] = 0;
                        if (dr["Project"] != null)
                        {
                            item["Project"] = Convert.ToInt64(dr["Project"].ToString());
                            item["Project_Code"] = dr["ProjectCode"].ToString();
                            item["Project_Name"] = dr["ProjectName"].ToString();
                        }
                        //item["Project"] = Convert.ToInt64(dr["Project"].ToString());
                        //item["Project_Code"] = dr["ProjectCode"].ToString();
                        //item["Project_Name"] = dr["ProjectName"].ToString();
                        item["SrcBillID"] = Convert.ToInt64(dr["SODoc"].ToString()); //来源单据ID
                        item["SrcBillType"] = 0;
                        item["DescFlexField_PubDescSeg5"] = dr["SOD5"].ToString();
                        item["NonTaxMoney_OCMoney"] = decimal.Parse(dr["Money"].ToString());
                        item["SrcDataLineNum"] = dr["SrcDataLineNum"].ToString();// 来源单据行行号
                    }

                }

                #region 测试
                foreach (var item in this._part.Model.RecBillHead_RecBillLines_RecBillUseLines.Records)
                {
                    string se11 = item["Cust_Customer"].ToString();
                    string se13 = item["Cust_Code"].ToString();
                    string se14 = item["Cust_Name"].ToString();
                    string se15 = item["RecProperty"].ToString();
                    if (item["EffectiveDate"] != null)
                    {
                        string se17 = item["EffectiveDate"].ToString();
                    }
                    string se18 = item["TotalMoney_OCMoney"].ToString();
                    string se123 = item["PreRecObj"].ToString();
                    if (item["CusSrcData"] != null)
                    {
                        string se134 = item["CusSrcData"].ToString();
                        string se1566 = item["CusSrcData_Code"].ToString();
                        string se113 = item["CusSrcData_Name"].ToString();
                        string se11232 = item["SrcDataID"].ToString();
                        string se11233 = item["SrcBillType"].ToString();
                        string se11234 = item["Project"].ToString();
                        string se114 = item["Project_Code"].ToString();
                        string se11442 = item["SrcBillID"].ToString();//标识
                        string se114424 = item["SrcBillType"].ToString();//来源单据类型
                        string se1144 = item["Project_Name"].ToString();
                        string se1144123 = item["DescFlexField_PubDescSeg5"].ToString();

                    }


                }
                #endregion

                //this._part.Model.RecBillHead_RecBillLines_RecBillUseLines.Clear();
                for (int i = 1; i < dt.Rows.Count; i++)
                {

                    DataRow dr = dt.Rows[i];
                    RecBillHead_RecBillLines_RecBillUseLinesRecord rd = this._part.Model.RecBillHead_RecBillLines_RecBillUseLines.AddNewUIRecord();
                    rd.LineNum = RecBillUseLinesDocLineNo;
                    RecBillUseLinesDocLineNo = RecBillUseLinesDocLineNo + 1;
                    rd.Cust_Customer = Convert.ToInt64(dr["Customer"].ToString());
                    rd.Cust_Code = dr["CustomerCode"].ToString();
                    rd.Cust_Name = dr["CustomerName"].ToString();
                    rd.RecProperty = 3;
                    rd.EffectiveDate = DateTime.Parse(dr["BussinessDate"].ToString());
                    rd.TotalMoney_OCMoney = decimal.Parse(dr["Money"].ToString());
                    rd.PreRecObj = 0;
                    rd.CusSrcData = Convert.ToInt64(dr["SODoc"].ToString());
                    rd.CusSrcData_Code = dr["SODocCode"].ToString();
                    rd.CusSrcData_Name = dr["SODocName"].ToString();
                    //rd.SrcDataID = Convert.ToInt64(dr["SODoc"].ToString());
                    //rd.SrcBillNum = dr["SODocCode"].ToString();
                    rd.SrcBillType = 0;
                    rd.Project = Convert.ToInt64(dr["Project"].ToString());
                    rd.Project_Code = dr["ProjectCode"].ToString();
                    rd.Project_Name = dr["ProjectName"].ToString();
                    //rd.SrcBillID = Convert.ToInt64(dr["SODoc"].ToString());
                    rd.SrcBillType = 0;
                    rd.DescFlexField_PubDescSeg5 = dr["SOD5"].ToString();
                    rd.NonTaxMoney_OCMoney = decimal.Parse(dr["Money"].ToString());
                    rd.SrcDataID = Convert.ToInt64(dr["SrcDataID"].ToString());//来源单据行ID 
                    rd.SrcBillNum = dr["SODocCode"].ToString(); //来源单据号
                    rd.SrcBillID = Convert.ToInt64(dr["SODoc"].ToString()); //来源单据ID
                    rd.SrcDataLineNum = dr["SrcDataLineNum"].ToString();// 来源单据行行号
                    rd.SetParentRecord(this._part.Model.RecBillHead_RecBillLines.FocusedRecord);

                }
            }
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

            NavigateManager.ShowModelWebpart(_part, "85d83c53-94c7-4030-ba4a-e3c486a6c2e5", _part.TaskId.ToString(), 1000, 522, null, true, true);

            string Cust_Customer = "";

            foreach (var item in this._part.Model.RecBillHead.Records)
            {
                if (item["Cust_Customer"] != null)
                {
                    Cust_Customer = item["Cust_Customer"].ToString();
                    HttpContext.Current.Session["CustomerID"] = Cust_Customer;
                }
            }

        }
    }
}
