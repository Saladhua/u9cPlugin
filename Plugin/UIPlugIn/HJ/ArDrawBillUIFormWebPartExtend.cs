using System;
using System.Data;
using System.Web.UI.WebControls.WebParts;
using UFIDA.U9.FI.AR.ARMaintenanceUIModel;
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
    /// 生成合并生单按钮
    /// </summary>
    ///对应2023-8-开发文档-应收合并生单与拆分
    class ArDrawBillUIFormWebPartExtend : ExtendedPartBase
    {
        private ArDrawBillUIFormWebPart _part;
        /// <summary>
        /// 初始化后扩展
        /// </summary>
        /// <param name="part"></param>
        /// <param name="e"></param>
        public override void AfterInit(IPart part, System.EventArgs e)
        {
            base.AfterInit(part, e);
            _part = part as ArDrawBillUIFormWebPart;

            //（1）、实例化按钮
            IUFButton BtnAR = new UFWebButtonAdapter();
            BtnAR.Text = "合并生单";
            BtnAR.ID = "BtnAR";
            BtnAR.AutoPostBack = true;
            //（2）、加入功能栏Card中
            IUFCard card = (IUFCard)part.GetUFControlByName(part.TopLevelContainer, "CardPlusFunction");
            card.Controls.Add(BtnAR);
            //（3）、设置按钮在容器中的位置
            CommonFunction.Layout(card, BtnAR, 6, 0);//一般为从左往右按钮个数乘以2
            //（4）、绑定按钮事件
            BtnAR.Click += new EventHandler(BtnAR_Click);
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
            //NavigateManager.NavigatePage(_part, "Cust.APUIList");
            NavigateManager.ShowModelWebpart(_part, "de703999-bc65-4bc9-9d0b-0a097a3dce4c", _part.TaskId.ToString(), 992, 504, null, true, true);

            //单据编号（销售出库单号）、源订单号、基地、单据日期、料号、料品、
            //参考料号1、参考料号2、单位、单价、到期日、业务员、税组合、客户。

            #region 数据收集
            DataTable dt = new DataTable();
            dt.Columns.Add("BillDocNo", typeof(string));//单据编号（销售出库单号）
            dt.Columns.Add("SrcDocNo", typeof(string));//源订单号
            dt.Columns.Add("BusinessDate", typeof(string));//单据日期
            dt.Columns.Add("ItemCode1", typeof(string));//参考料号1
            dt.Columns.Add("ItemCode2", typeof(string));//	参考料号2
            dt.Columns.Add("ItemCode", typeof(string));//料号
            dt.Columns.Add("ItemID", typeof(string));//	料品
            dt.Columns.Add("PUom", typeof(string));//	单位
            dt.Columns.Add("ShipLine_DescFlexField_PubDescSeg3", typeof(string));//	出货行备注
            dt.Columns.Add("TaxPrice", typeof(string));//	单价
            dt.Columns.Add("PUAmount", typeof(string));//	可立账数量
            dt.Columns.Add("AROCMoneyNonTax", typeof(string));//可立账未税金额
            dt.Columns.Add("TaxMoneyAC", typeof(string));//可立账税额
            dt.Columns.Add("TotalMoneyTC", typeof(string));///可立账价税合计
            dt.Columns.Add("CurrentPUAmount", typeof(string));//本次立账数量　
            dt.Columns.Add("CurrentNonTaxPrice", typeof(string));//本次立账未税单价　　
            dt.Columns.Add("CurrentTaxPrice", typeof(string));//	本次立账含税单价　　
            dt.Columns.Add("CurrentNonTaxMoney", typeof(string));//本次立账未税金额　　
            dt.Columns.Add("TaxSchedule", typeof(string));//	税组合　
            dt.Columns.Add("CurrentTax", typeof(string));//本次立账税额　　
            dt.Columns.Add("CurrentWithTaxMoney", typeof(string));//本次立账价税合计　　
            dt.Columns.Add("FeeMakeAccountField", typeof(string));//费用立账　　　
            dt.Columns.Add("ContractNo", typeof(string));//	源合同号　　
            dt.Columns.Add("ShipPlanDocNo", typeof(string));//	源出货计划号　　　
            dt.Columns.Add("Transactor", typeof(string));//业务员　　
            dt.Columns.Add("Dept", typeof(string));//部门　　
            dt.Columns.Add("Project", typeof(string));//项目　　
            dt.Columns.Add("Task", typeof(string));//任务　　
            dt.Columns.Add("ItemGrade", typeof(string));//等级　　
            dt.Columns.Add("LotCode", typeof(string));//批号　　
            dt.Columns.Add("AccrueCustSite", typeof(string));//立账客户位置　　
            dt.Columns.Add("PayCustSite", typeof(string));//付款客户位置　　
            dt.Columns.Add("OrderBy", typeof(string));//订货客户　　
            dt.Columns.Add("ShipToCustSite", typeof(string));//收货客户位置　　
            dt.Columns.Add("RecTerm", typeof(string));//收款条件　　
            dt.Columns.Add("ConfirmTerm", typeof(string));//立账条件　　
            dt.Columns.Add("InvoiceNo", typeof(string));//发票号　　
            dt.Columns.Add("AccrueDocType", typeof(string));//单据类型　　　
            dt.Columns.Add("KitShipMode", typeof(string));//	成套收发货　　　
            dt.Columns.Add("AC", typeof(string));//币种　　　
            dt.Columns.Add("ShipLine_Ship_DescFlexField_PrivateDescSeg9", typeof(string));//收货位置　　　
            dt.Columns.Add("ShipLine_SaleCostFC", typeof(string));//出货成本　　　
            #endregion
            try
            {
                foreach (var item in this._part.Model["DrawBillView"].SelectRecords)
                {
                    DataRow dr = dt.NewRow();
                    dr["BillDocNo"] = item["BillDocNo"];//单据编号（销售出库单号）
                    dr["SrcDocNo"] = item["SrcDocNo"];//源订单号
                    dr["BusinessDate"] = item["BusinessDate"];//单据日期
                    dr["ItemCode1"] = item["ItemCode1"];//参考料号1
                    dr["ItemCode2"] = item["ItemCode2"];//	参考料号2
                    dr["ItemCode"] = item["ItemCode"];//料号
                    dr["ItemID"] = item["ItemID"];//料品
                    dr["PUom"] = item["PUom"];//单位
                    dr["ShipLine_DescFlexField_PubDescSeg3"] = item["ShipLine_DescFlexField_PubDescSeg3"];//出货行备注
                    dr["TaxPrice"] = item["TaxPrice"]; //	单价
                    dr["PUAmount"] = item["PUAmount"];//可立账数量
                    dr["AROCMoneyNonTax"] = item["AROCMoneyNonTax"];//可立账未税金额
                    dr["TaxMoneyAC"] = item["TaxMoneyAC"];//可立账税额
                    dr["TotalMoneyTC"] = item["TotalMoneyTC"];///可立账价税合计
                    dr["CurrentPUAmount"] = item["CurrentPUAmount"];//本次立账数量　
                    dr["CurrentNonTaxPrice"] = item["CurrentNonTaxPrice"];//本次立账未税单价　　
                    dr["CurrentTaxPrice"] = item["CurrentTaxPrice"];//本次立账含税单价　　
                    dr["CurrentNonTaxMoney"] = item["CurrentNonTaxMoney"];//本次立账未税金额　　
                    dr["TaxSchedule"] = item["TaxSchedule"];//税组合　
                    dr["CurrentTax"] = item["CurrentTax"];//本次立账税额　　
                    dr["CurrentWithTaxMoney"] = item["CurrentWithTaxMoney"];//本次立账价税合计　　
                    dr["FeeMakeAccountField"] = item["FeeMakeAccountField"];//费用立账　　　
                    dr["ContractNo"] = item["ContractNo"];//源合同号　　
                    dr["ShipPlanDocNo"] = item["ShipPlanDocNo"];//源出货计划号　　　
                    dr["Transactor"] = item["Transactor"];//业务员　　
                    dr["Dept"] = item["Dept"];//部门　　
                    dr["Project"] = item["Project"];//项目　　
                    dr["Task"] = item["Task"];//任务　　
                    dr["ItemGrade"] = item["ItemGrade"];//等级　　
                    dr["LotCode"] = item["LotCode"];//批号　　
                    dr["AccrueCustSite"] = item["AccrueCustSite"];//立账客户位置　　
                    dr["PayCustSite"] = item["PayCustSite"];//付款客户位置　　
                    dr["OrderBy"] = item["OrderBy"];//订货客户　　
                    dr["ShipToCustSite"] = item["ShipToCustSite"];//收货客户位置　　
                    dr["RecTerm"] = item["RecTerm"];//收款条件　　
                    dr["ConfirmTerm"] = item["ConfirmTerm"];//立账条件　　
                    dr["InvoiceNo"] = item["InvoiceNo"];//发票号　　
                    dr["AccrueDocType"] = item["AccrueDocType"];//单据类型　　　
                    dr["KitShipMode"] = item["KitShipMode"];//成套收发货　　　
                    dr["AC"] = item["AC"];//币种　　　
                    dr["ShipLine_Ship_DescFlexField_PrivateDescSeg9"] = item["ShipLine_Ship_DescFlexField_PrivateDescSeg9"];//收货位置　　　
                    dr["ShipLine_SaleCostFC"] = item["ShipLine_SaleCostFC"];//出货成本
                    dt.Rows.Add(dr);
                }
                this._part.CurrentSessionState["ResultMo"] = dt;
            }
            catch (Exception ex)
            {
                IUIModel apModel = this._part.Model;
                this._part.Model.ErrorMessage.SetErrorMessage(ref apModel, ex);
            }
        }


        public override void BeforeRender(IPart Part, EventArgs args)
        {
            DataTable dt = new DataTable();
            if (this._part.CurrentSessionState["ApAdd"] != null)
            {
                ////this.Model.BatchingPlan.AddNewUIRecord();
                //this.Model.ApAddForPart.Clear();//清理
                dt = this._part.CurrentSessionState["ApAdd"] as DataTable;
                this._part.CurrentSessionState["ApAdd"] = null;
                this._part.Model["DrawBillView"].Clear();
                IUIRecord record = this._part.Model["DrawBillView"].FocusedRecord;
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    record = this._part.Model["DrawBillView"].AddNewUIRecord();
                    DataRow dr = dt.Rows[i];
                    record["BillDocNo"] = dr["BillDocNo"];//单据编号（销售出库单号）
                    record["SrcDocNo"] = dr["SrcDocNo"];//源订单号
                }
            }
            base.BeforeRender(Part, args);
        }
    }
}
