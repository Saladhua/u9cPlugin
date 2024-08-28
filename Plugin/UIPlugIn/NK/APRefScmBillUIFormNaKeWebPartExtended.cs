using System;
using System.Data;
using UFIDA.U9.Cust.CustNKFindAPRefScmBillSV;
using UFIDA.U9.FI.AP.PayReqFundUIModel;
using UFIDA.U9.UI.PDHelper;
using UFSoft.UBF.UI.ControlModel;
using UFSoft.UBF.UI.Custom;
using UFSoft.UBF.UI.IView;
using UFSoft.UBF.UI.MD.Runtime;
using UFSoft.UBF.UI.WebControlAdapter;
using UFSoft.UBF.Util.DataAccess;

namespace YY.U9.Cust.LI.UIPlugIn
{
    class APRefScmBillUIFormNaKeWebPartExtended : ExtendedPartBase
    {
        private APRefScmBillUIFormWebPart _part;
        public override void AfterInit(IPart part, System.EventArgs e)
        {
            base.AfterInit(part, e);
            this._part = (part as APRefScmBillUIFormWebPart);
            if (_part == null)
                return;
            //增加列
            IUFDataGrid grid = (IUFDataGrid)_part.GetUFControlByName(_part.TopLevelContainer, "DataGrid0");
            foreach (IUIField fld in _part.Model.RefScmBillView.Fields)
            {
                if (fld.Name.Equals("PayReqFundHead_DescFlexField_PrivateDescSeg25"))
                {
                    IUFDataGridColumn column = new UFWebNumberColumnWrapper(fld.Name);
                    column.ID = "PayReqFundHead_DescFlexField_PrivateDescSeg25";
                    column.Caption = "已入库数量";
                    column.ShowBorderLine = false;
                    column.Point = 3;
                    column.UIField = fld;
                    column.UIFieldID = "PayReqFundHead_DescFlexField_PrivateDescSeg25";
                    column.IsSequence = false;
                    column.Visible = true;
                    column.Enabled = false;
                    column.HasSum = false;
                    column.HasEvent = false;
                    column.IsNull = true;
                    column.DataType = 0;
                    column.Width = 80;
                    column.DefaultValue = "";
                    grid.Columns.Add(column);
                    column.CurrentPart = grid.CurrentPart;
                }
                if (fld.Name.Equals("PayReqFundHead_DescFlexField_PrivateDescSeg30"))
                {
                    IUFDataGridColumn column = new UFWebNumberColumnWrapper(fld.Name);
                    column.ID = "PayReqFundHead_DescFlexField_PrivateDescSeg30";
                    column.Caption = "已开票数量";
                    column.ShowBorderLine = false;
                    column.Point = 3;
                    column.UIField = fld;
                    column.UIFieldID = "PayReqFundHead_DescFlexField_PrivateDescSeg30";
                    column.IsSequence = false;
                    column.Visible = true;
                    column.Enabled = false;
                    column.HasSum = false;
                    column.HasEvent = false;
                    column.IsNull = true;
                    column.DataType = 0;
                    column.Width = 80;
                    column.DefaultValue = "";
                    grid.Columns.Add(column);
                    column.CurrentPart = grid.CurrentPart;
                }
                if (fld.Name.Equals("PayReqFundHead_DescFlexField_PrivateDescSeg24"))
                {
                    IUFDataGridColumn column = new UFWebNumberColumnWrapper(fld.Name);
                    column.ID = "PayReqFundHead_DescFlexField_PrivateDescSeg24";
                    column.Caption = "合同总数量";
                    column.ShowBorderLine = false;
                    column.Point = 3;
                    column.UIField = fld;
                    column.UIFieldID = "PayReqFundHead_DescFlexField_PrivateDescSeg24";
                    column.IsSequence = false;
                    column.Visible = true;
                    column.Enabled = false;
                    column.HasSum = false;
                    column.HasEvent = false;
                    column.IsNull = true;
                    column.DataType = 0;
                    column.Width = 80;
                    column.DefaultValue = "";
                    grid.Columns.Add(column);
                    column.CurrentPart = grid.CurrentPart;
                }
            }
        }

        public override void BeforeRender(IPart part, EventArgs args)
        {
            this._part = (part as APRefScmBillUIFormWebPart);

            if (_part == null)
                return;

            string DocNo = "";

            string ContractsNumber = "";//合同数量

            string RuKuNum = "";//已入库数量

            string InvoicesNum = "";//已开票数量

            foreach (IUIRecord record in _part.Model.RefScmBillView.Records)
            {
                DocNo = record["SrcBillNum"].ToString();
                UFIDA.U9.Cust.CustNKFindAPRefScmBillSV.Proxy.FindAPRefScmBillSVProxy findAPRefScmBillSVProxy = new UFIDA.U9.Cust.CustNKFindAPRefScmBillSV.Proxy.FindAPRefScmBillSVProxy();
                findAPRefScmBillSVProxy.OrgID = PDContext.Current.OrgID;
                findAPRefScmBillSVProxy.DocNo = DocNo;
                RetrunDataData retrunDataData = findAPRefScmBillSVProxy.Do();
                ContractsNumber = retrunDataData.PayReqFundHeadDesPri24;
                record["PayReqFundHead_DescFlexField_PrivateDescSeg24"] = decimal.Parse(ContractsNumber).ToString("#0.####");
                RuKuNum = retrunDataData.PayReqFundHeadDesPri25;
                record["PayReqFundHead_DescFlexField_PrivateDescSeg25"] = decimal.Parse(RuKuNum).ToString("#0.####");
                InvoicesNum = retrunDataData.PayReqFundHeadDesPri30;
                record["PayReqFundHead_DescFlexField_PrivateDescSeg30"] = decimal.Parse(InvoicesNum).ToString("#0.####");
            }
        }



        /// <summary>
        /// 找对应的合同数量
        /// </summary>
        /// <returns></returns>
        public string FindDesPri24(string DocNo)
        {
            string SumNum = "0";
            DataTable dataTable = new DataTable();
            DataSet dataSet = new DataSet();
            string sql_1 = "select SUM(ReqQtyTU) AS ReqQtyTU from PM_PurchaseOrder A1" +
                " left join PM_POLine A2 on A1.ID = A2.PurchaseOrder" +
                " where A1.DocNo = '" + DocNo + "'";
            DataAccessor.RunSQL(DataAccessor.GetConn(), sql_1, null, out dataSet);
            dataTable = dataSet.Tables[0];
            #region 
            int n = 0;
            if (dataTable.Rows != null && dataTable.Rows.Count > 0)
            {
                SumNum = dataTable.Rows[n]["ReqQtyTU"] == null ? "0" : dataTable.Rows[n]["ReqQtyTU"].ToString();
            }
            #endregion
            if (string.IsNullOrEmpty(SumNum))
            {
                SumNum = "0";
            }
            return SumNum;
        }

        /// <summary>
        /// 已入库数量
        /// 会出现一种情况使数据double 拆行
        /// 拆行的我加状态判断
        /// </summary>
        /// <param name="DocNo"></param>
        /// <returns></returns>
        public string FindDesPri25(string DocNo)
        {
            string SumNum = "0";
            DataTable dataTable = new DataTable();
            DataSet dataSet = new DataSet();
            string sql_1 = "select  sum(A2.RcvQtyTU) AS RcvQtyTU from PM_Receivement A1" +
                " left join PM_RcvLine A2 on A1.ID = A2.Receivement " +
                " where  A2.SrcDoc_SrcDocNo = '" + DocNo + "' " +
                " and A2.Status=5 ";
            DataAccessor.RunSQL(DataAccessor.GetConn(), sql_1, null, out dataSet);
            dataTable = dataSet.Tables[0];
            #region 
            int n = 0;
            if (dataTable.Rows != null && dataTable.Rows.Count > 0)
            {
                SumNum = dataTable.Rows[n]["RcvQtyTU"] == null ? "0" : dataTable.Rows[n]["RcvQtyTU"].ToString();
            }
            #endregion
            if (string.IsNullOrEmpty(SumNum))
            {
                SumNum = "0";
            }
            return SumNum;
        }

        /// <summary>
        /// 已开票数量
        /// </summary>
        /// <param name="DocNo"></param>
        /// <returns></returns>
        public string FindDesPri30(string DocNo)
        {
            string SumNum = "0";
            DataTable dataTable = new DataTable();
            DataSet dataSet = new DataSet();
            string sql_1 = "select SUM(PUAmount) AS PUAmount from AP_APBillHead A1 left join AP_APBillLine A2 on A1.ID = A2.APBillHead " +
                " where A2.SrcBillPONum = '" + DocNo + "' ";
            DataAccessor.RunSQL(DataAccessor.GetConn(), sql_1, null, out dataSet);
            dataTable = dataSet.Tables[0];
            #region 
            int n = 0;
            if (dataTable.Rows != null && dataTable.Rows.Count > 0)
            {
                SumNum = dataTable.Rows[n]["PUAmount"] == null ? "0" : dataTable.Rows[n]["PUAmount"].ToString();
            }
            #endregion
            if (string.IsNullOrEmpty(SumNum))
            {
                SumNum = "0";
            }

            return SumNum;
        }
    }
}
