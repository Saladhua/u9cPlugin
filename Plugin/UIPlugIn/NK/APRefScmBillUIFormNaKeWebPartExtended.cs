using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using UFIDA.U9.Cust.CustNKFindAPRefScmBillSV;
using UFIDA.U9.FI.AP.PayReqFundUIModel;
using UFIDA.U9.UI.PDHelper;
using UFSoft.UBF.UI.ControlModel;
using UFSoft.UBF.UI.Custom;
using UFSoft.UBF.UI.IView;
using UFSoft.UBF.UI.MD.Runtime;
using UFSoft.UBF.UI.WebControlAdapter;
using UFSoft.UBF.Util.DataAccess;
using UFSoft.UBF.Util.Log;

namespace YY.U9.Cust.LI.UIPlugIn
{
    class APRefScmBillUIFormNaKeWebPartExtended : ExtendedPartBase
    {
        private APRefScmBillUIFormWebPart _part;

        private static readonly ILogger logger = LoggerManager.GetLogger(typeof(APRefScmBillUIFormNaKeWebPartExtended));
        public override void AfterInit(IPart part, System.EventArgs e)
        {
            base.AfterInit(part, e);
            this._part = (part as APRefScmBillUIFormWebPart);
            if (_part == null)
                return;
            //增加列
            IUFDataGrid grid = (IUFDataGrid)_part.GetUFControlByName(_part.TopLevelContainer, "DataGrid0");
            DateTime dt = new DateTime();

            dt = DateTime.Now;

            logger.Error("列表行开始时间：" + dt.ToString("yyyy-MM-dd:hh:mm:ss"));
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
                if (fld.Name.Equals("PayReqFundHead_DescFlexField_PrivateDescSeg23"))
                {
                    IUFDataGridColumn column = new UFWebNumberColumnWrapper(fld.Name);
                    column.ID = "PayReqFundHead_DescFlexField_PrivateDescSeg23";
                    column.Caption = "未入库数量";
                    column.ShowBorderLine = false;
                    column.Point = 3;
                    column.UIField = fld;
                    column.UIFieldID = "PayReqFundHead_DescFlexField_PrivateDescSeg23";
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
            //增加列
            IUFDataGrid grid2 = (IUFDataGrid)_part.GetUFControlByName(_part.TopLevelContainer, "DataGrid1");
            foreach (IUIField fld in _part.Model.RefScmBillLineView.Fields)
            {
                //物料入库数量的SQL语句
                //SELECT PM_POLine.TotalRecievedQtyCU FROM PM_POLine INNER JOIN PM_PurchaseOrder ON PM_POLine.PurchaseOrder=PM_PurchaseOrder.ID WHERE PM_PurchaseOrder.DocNo='单号'
                //SELECT PM_POLine.TotalRecievedQtyCU,PM_POLine.TotalConfirmedQtyCU FROM PM_POLine INNER JOIN PM_PurchaseOrder ON PM_POLine.PurchaseOrder=PM_PurchaseOrder.ID WHERE PM_PurchaseOrder.DocNo='PO2102204010004'
                //TotalRecievedQtyCU 物料入库数量
                //TotalConfirmedQtyCU 开票数量
                if (fld.Name.Equals("PayReqFundHead_DescFlexField_PrivateDescSeg29"))
                {
                    IUFDataGridColumn column = new UFWebNumberColumnWrapper(fld.Name);
                    column.ID = "PayReqFundHead_DescFlexField_PrivateDescSeg29";
                    column.Caption = "物料入库数量";
                    column.ShowBorderLine = false;
                    column.Point = 3;
                    column.UIField = fld;
                    column.UIFieldID = "PayReqFundHead_DescFlexField_PrivateDescSeg29";
                    column.IsSequence = false;
                    column.Visible = true;
                    column.Enabled = false;
                    column.HasSum = false;
                    column.HasEvent = false;
                    column.IsNull = true;
                    column.DataType = 0;
                    column.Width = 80;
                    column.DefaultValue = "";
                    grid2.Columns.Add(column);
                    column.CurrentPart = grid2.CurrentPart;
                }
                if (fld.Name.Equals("PayReqFundHead_DescFlexField_PrivateDescSeg28"))
                {
                    IUFDataGridColumn column = new UFWebNumberColumnWrapper(fld.Name);
                    column.ID = "PayReqFundHead_DescFlexField_PrivateDescSeg28";
                    column.Caption = "开票数量";
                    column.ShowBorderLine = false;
                    column.Point = 3;
                    column.UIField = fld;
                    column.UIFieldID = "PayReqFundHead_DescFlexField_PrivateDescSeg28";
                    column.IsSequence = false;
                    column.Visible = true;
                    column.Enabled = false;
                    column.HasSum = false;
                    column.HasEvent = false;
                    column.IsNull = true;
                    column.DataType = 0;
                    column.Width = 80;
                    column.DefaultValue = "";
                    grid2.Columns.Add(column);
                    column.CurrentPart = grid2.CurrentPart;
                }
                //SELECT InvoiceDate FROM AP_APBillLine INNER JOIN AP_APBillHead ON AP_APBillHead.ID=AP_APBillLine.APBillHead WHERE SrcBillPOID='1002204010079861'
                //最后到票日期
                if (fld.Name.Equals("PayReqFundHead_DescFlexField_PrivateDescSeg27"))
                {
                    IUFDataGridColumn column = new UFWebNumberColumnWrapper(fld.Name);
                    column.ID = "PayReqFundHead_DescFlexField_PrivateDescSeg27";
                    column.Caption = "最后到票日期";
                    column.ShowBorderLine = false;
                    column.Point = 3;
                    column.UIField = fld;
                    column.UIFieldID = "PayReqFundHead_DescFlexField_PrivateDescSeg27";
                    column.IsSequence = false;
                    column.Visible = true;
                    column.Enabled = false;
                    column.HasSum = false;
                    column.HasEvent = false;
                    column.IsNull = true;
                    column.DataType = 0;
                    column.Width = 80;
                    column.DefaultValue = "";
                    grid2.Columns.Add(column);
                    column.CurrentPart = grid2.CurrentPart;
                }
                if (fld.Name.Equals("PayReqFundHead_DescFlexField_PrivateDescSeg26"))
                {
                    IUFDataGridColumn column = new UFWebNumberColumnWrapper(fld.Name);
                    column.ID = "PayReqFundHead_DescFlexField_PrivateDescSeg26";
                    column.Caption = "预付数量";
                    column.ShowBorderLine = false;
                    column.Point = 3;
                    column.UIField = fld;
                    column.UIFieldID = "PayReqFundHead_DescFlexField_PrivateDescSeg26";
                    column.IsSequence = false;
                    column.Visible = true;
                    column.Enabled = false;
                    column.HasSum = false;
                    column.HasEvent = false;
                    column.IsNull = true;
                    column.DataType = 0;
                    column.Width = 80;
                    column.DefaultValue = "";
                    grid2.Columns.Add(column);
                    column.CurrentPart = grid2.CurrentPart;
                }
                //来源行号
                if (fld.Name.Equals("SrcDataNum"))
                {
                    IUFDataGridColumn column = new UFWebNumberColumnWrapper(fld.Name);
                    column.ID = "SrcDataNum";
                    column.Caption = "来源行号";
                    column.ShowBorderLine = false;
                    column.Point = 3;
                    column.UIField = fld;
                    column.UIFieldID = "SrcDataNum";
                    column.IsSequence = false;
                    column.Visible = true;
                    column.Enabled = false;
                    column.HasSum = false;
                    column.HasEvent = false;
                    column.IsNull = true;
                    column.DataType = 0;
                    column.Width = 80;
                    column.DefaultValue = "";
                    grid2.Columns.Add(column);
                    column.CurrentPart = grid2.CurrentPart;
                }

            }

           // DateTime dt2 = new DateTime();

           // dt2 = DateTime.Now;

           // logger.Error("列表行结束时间：" + dt2.ToString("yyyy-MM-dd:hh:mm:ss"));

           // DateTime dt3 = new DateTime();

           // dt3 = DateTime.Now;

           // logger.Error("请款SQL开始时间：" + dt3.ToString("yyyy-MM-dd:hh:mm:ss"));

           // this._part = (part as APRefScmBillUIFormWebPart);

           // if (_part == null)
           //     return;

           // string DocNo = "";

           // string ContractsNumber = "0";//合同数量

           // string RuKuNum = "0";//已入库数量

           // string InvoicesNum = "0";//已开票数量
           // string docno = "'0',";
           // string itemcode = "'0',";
           // string item_id = "'0',";
           // string srcDataNum = "'0',";
           // List<refSDto> refSDtos = new List<refSDto>();
           // foreach (var item in _part.Model.RefScmBillLineView.Records)
           // {
           //     docno = docno + "'" + item["SrcBillNum"].ToString() + "'" + ",";
           //     itemcode = itemcode + "'" + item["Item_Code"].ToString() + "'" + ",";
           //     item_id = item_id + "'" + item["Item"].ToString() + "'" + ",";
           //     srcDataNum = srcDataNum + "'" + item["SrcDataNum"].ToString() + "'" + ",";
           //     docno = string.Join(",", docno.Split(',').Distinct().ToArray());
           //     itemcode = string.Join(",", itemcode.Split(',').Distinct().ToArray());
           //     item_id = string.Join(",", item_id.Split(',').Distinct().ToArray());
           //     srcDataNum = string.Join(",", srcDataNum.Split(',').Distinct().ToArray());
           //     refSDto refSDto = new refSDto();
           //     refSDto.id = item["SrcBill"].ToString();//对应采购订单的id
           //     refSDto.srcbillnum = item["SrcBillNum"].ToString();
           //     refSDto.item_Code = item["Item_Code"].ToString();
           //     refSDto.item = item["Item"].ToString();
           //     refSDto.srcDataNum = item["SrcDataNum"].ToString();
           //     refSDto.canPrePayPUAmount = item["CanPrePayPUAmount"].ToString();
           //     refSDtos.Add(refSDto);
           // }
           // //string sql = string.Format("select ID from U9CCustNrknor_DeductDoc where BeginDate>='{0}' and EndDate<='{1}' and DocVersion='{2}' and Org={3} and ID!={4}", item.BeginDate.ToString("yyyy-MM-dd"), item.EndDate.ToString("yyyy-MM-dd"), item.DocVersion, PDContext.Current.OrgID, item.ID);
           // //DataTable dt = GetDataTable(sql);
           // //return dt;

           // //DataTable dataTable = new DataTable();
           // //DataSet dataSet = new DataSet();

           // string sql_1 = "SELECT   PM_PurchaseOrder.DocNo,PM_PurchaseOrder.ID,PM_POLine.DocLineNo,PM_POLine.TotalRecievedQtyCU,PM_POLine.TotalConfirmedQtyCU,ItemInfo_ItemID,ItemInfo_ItemCode" +
           //"  ,(SELECT TOP(1) AccrueDate FROM AP_APBillLine INNER JOIN AP_APBillHead ON AP_APBillHead.ID = AP_APBillLine.APBillHead WHERE SrcBillPOID" +
           //"  = PM_PurchaseOrder.ID order by  AP_APBillHead.ModifiedOn ASC ) as AccrueDate," +
           //"  (SELECT SUM(PUAmount) AS Sum_PUAmount FROM AP_PayReqFundDetail WHERE SrcBillID = PM_PurchaseOrder.ID " +
           // "   ) as sum_PUAmount" +
           //"  FROM PM_POLine " +
           //"  INNER JOIN PM_PurchaseOrder ON PM_POLine.PurchaseOrder=PM_PurchaseOrder.ID" +
           //"  WHERE PM_PurchaseOrder.DocNo in (" + docno.Substring(0, docno.Length - 1) + ")AND ItemInfo_ItemCode in ( " + itemcode.Substring(0, itemcode.Length - 1) + ")" +
           //"  Order By DocLineNo DESC";
           // //DataAccessor.RunSQL(DataAccessor.GetConn(), sql_1, null, out dataSet);
           // //dataTable = dataSet.Tables[0];


           // DataTable dataTable = U9Common.GetDataTable(sql_1);

           // int n = 0;
           // if (dataTable.Rows != null && dataTable.Rows.Count > 0)
           // {
           //     for (int i = 0; i < dataTable.Rows.Count; i++)
           //     {
           //         n = i;
           //         string ID = dataTable.Rows[n]["ID"].ToString();
           //         string ItemInfo_ItemCode = dataTable.Rows[n]["ItemInfo_ItemCode"].ToString();
           //         string SrcDataNum = dataTable.Rows[n]["DocNo"].ToString();
           //         List<refSDto> NrefSDtos = refSDtos.Where(x => x.item_Code == ItemInfo_ItemCode && x.srcbillnum == SrcDataNum).ToList();
           //         int NCount = NrefSDtos.Count();
           //         if (NCount > 0)
           //         {
           //             foreach (var item in NrefSDtos)
           //             {
           //                 string see = item.item_Code;
           //                 decimal pu = 0;
           //                 bool a = decimal.TryParse(dataTable.Rows[n]["sum_PUAmount"].ToString(), out pu);
           //                 decimal canPrePayPUAmount = 0;
           //                 bool b = decimal.TryParse(item.canPrePayPUAmount, out canPrePayPUAmount);
           //                 foreach (IUIRecord record in _part.Model.RefScmBillLineView.Records)
           //                 {
           //                     if (record["Item_Code"].ToString() == dataTable.Rows[n]["ItemInfo_ItemCode"].ToString())
           //                     {
           //                         string see123123 = record["SrcDataNum"].ToString();

           //                         record["PayReqFundHead_DescFlexField_PrivateDescSeg29"] = decimal.Parse(dataTable.Rows[n]["TotalRecievedQtyCU"].ToString()).ToString("0.####");//入库数量
           //                         record["PayReqFundHead_DescFlexField_PrivateDescSeg28"] = decimal.Parse(dataTable.Rows[n]["TotalConfirmedQtyCU"].ToString()).ToString("0.####");//开票数量
           //                         if (dataTable.Rows[n]["AccrueDate"] == null || dataTable.Rows[n]["AccrueDate"].ToString() == "")
           //                         {
           //                             record["PayReqFundHead_DescFlexField_PrivateDescSeg27"] = "";
           //                         }
           //                         else
           //                         {
           //                             record["PayReqFundHead_DescFlexField_PrivateDescSeg27"] = Convert.ToDateTime(dataTable.Rows[n]["AccrueDate"]).ToString("yyyy-MM-dd");//最后到票日期
           //                         }
           //                         record["PayReqFundHead_DescFlexField_PrivateDescSeg26"] = canPrePayPUAmount - pu;//预付数量
           //                     }
           //                 }
           //             }
           //         }
           //     }
           //     n++;
           // }

           // List<RefScmBillViewDto> refScmBillViewDtos = new List<RefScmBillViewDto>();

           // string SrcBillNum = "";

           // foreach (IUIRecord record in _part.Model.RefScmBillView.Records)
           // {
           //     try
           //     {
           //         if (!string.IsNullOrEmpty(record["SrcBillNum"].ToString()))
           //         {
           //             RefScmBillViewDto refScmBillViewDto = new RefScmBillViewDto();

           //             refScmBillViewDto.SrcBillNum = record["SrcBillNum"].ToString();

           //             DocNo = record["SrcBillNum"].ToString();

           //             SrcBillNum = SrcBillNum + "'" + record["SrcBillNum"].ToString() + "'" + ",";

           //             //refScmBillViewDto.DP24 = decimal.Parse(FindDesPri24(DocNo));

           //             //record["PayReqFundHead_DescFlexField_PrivateDescSeg25"] = decimal.Parse(FindDesPri25(DocNo));

           //             //record["PayReqFundHead_DescFlexField_PrivateDescSeg30"] = decimal.Parse(FindDesPri30(DocNo));

           //             refScmBillViewDtos.Add(refScmBillViewDto);
           //         }
           //     }
           //     catch (Exception ex)
           //     {
           //         throw new Exception("报错单号" + DocNo + "。" + "报错原因" + ex.ToString());
           //     }
           // }

           // SrcBillNum = string.Join(",", SrcBillNum.Split(',').Distinct().ToArray());

           // #region 私有字段24
           // DataTable dataTables24 = new DataTable();
           // DataSet dataSets24 = new DataSet();
           // string sql_s24 = "select  SUM(ReqQtyTU) AS ReqQtyTU,A1.DocNo from PM_PurchaseOrder A1" +
           //     " left join PM_POLine A2 on A1.ID = A2.PurchaseOrder" +
           //     " where A1.DocNo in (" + SrcBillNum.Substring(0, SrcBillNum.Length - 1) + ") GROUP BY ReqQtyTU,A1.DocNo";
           // DataAccessor.RunSQL(DataAccessor.GetConn(), sql_s24, null, out dataSets24);
           // dataTables24 = dataSets24.Tables[0];
           // #region  私有字段24
           // int s24 = 0;
           // if (dataTables24.Rows != null && dataTables24.Rows.Count > 0)
           // {
           //     for (int i = 0; i < dataTables24.Rows.Count; i++)
           //     {
           //         s24 = i;

           //         string SrcDocNo = dataTables24.Rows[s24]["DocNo"].ToString();

           //         //foreach (var item in refScmBillViewDtos)
           //         //{
           //         //    if (item.SrcBillNum == SrcDocNo)
           //         //    {
           //         //        item.DP24 = decimal.Parse(dataTables24.Rows[s24]["ReqQtyTU"].ToString());
           //         //    }
           //         //}
           //         RefScmBillViewDto refScmBillViewDto = new RefScmBillViewDto();

           //         refScmBillViewDto.SrcBillNum = SrcDocNo;

           //         refScmBillViewDto.DP24 = decimal.Parse(dataTables24.Rows[s24]["ReqQtyTU"].ToString());

           //         refScmBillViewDtos.Add(refScmBillViewDto);
           //     }
           //     s24++;
           // }
           // #endregion
           // #endregion

           // #region 私有字段30
           // DataTable dataTables30 = new DataTable();
           // DataSet dataSets30 = new DataSet();
           // string sql_s30 = "SELECT" +
           //     " (SELECT SUM(PUAmount) FROM AP_APBillHead B1 left join AP_APBillLine B2 ON B1.ID = B2.APBillHead " +
           //     " WHERE B2.SrcBillPONum = A2.SrcBillPONum) AS PUAmount," +
           //     " A2.SrcBillPONum FROM AP_APBillHead A1 left join AP_APBillLine A2 ON A1.ID = A2.APBillHead " +
           //     " WHERE A2.SrcBillPONum  in (" + SrcBillNum.Substring(0, SrcBillNum.Length - 1) + ")";
           // DataAccessor.RunSQL(DataAccessor.GetConn(), sql_s30, null, out dataSets30);
           // dataTables30 = dataSets30.Tables[0];
           // int s30 = 0;
           // if (dataTables30.Rows != null && dataTables30.Rows.Count > 0)
           // {
           //     for (int i = 0; i < dataTables30.Rows.Count; i++)
           //     {
           //         s30 = i;

           //         string SrcDocNo = dataTables30.Rows[s30]["SrcBillPONum"].ToString();

           //         foreach (var item in refScmBillViewDtos)
           //         {
           //             if (item.SrcBillNum == SrcDocNo)
           //             {
           //                 item.DP30 = decimal.Parse(dataTables30.Rows[s30]["PUAmount"].ToString());
           //             }
           //         }
           //         //RefScmBillViewDto refScmBillViewDto = new RefScmBillViewDto();

           //         //refScmBillViewDto.SrcBillNum = SrcDocNo;

           //         //refScmBillViewDto.DP30 = decimal.Parse(dataTables30.Rows[s30]["PUAmount"].ToString());

           //         //refScmBillViewDtos.Add(refScmBillViewDto);
           //     }
           //     s30++;
           // }
           // #endregion

           // #region 私有字段25
           // DataTable dataTables25 = new DataTable();
           // DataSet dataSets25 = new DataSet();
           // string sql_s25 = "select" +
           //     " (select  sum(B2.RcvQtyTU) AS RcvQtyTU " +
           //     " from PM_Receivement B1 " +
           //     " left join PM_RcvLine B2 on B1.ID = B2.Receivement where B2.SrcDoc_SrcDocNo = A2.SrcPO_SrcDocNo and B2.Status = 5) AS RcvQtyTU, A2.SrcPO_SrcDocNo " +
           //     " from PM_Receivement A1 left join PM_RcvLine A2 on A1.ID = A2.Receivement where A2.SrcDoc_SrcDocNo in (" + SrcBillNum.Substring(0, SrcBillNum.Length - 1) + ") and A2.Status = 5  ";
           // DataAccessor.RunSQL(DataAccessor.GetConn(), sql_s25, null, out dataSets25);
           // dataTables25 = dataSets25.Tables[0];
           // int s25 = 0;
           // if (dataTables25.Rows != null && dataTables25.Rows.Count > 0)
           // {
           //     for (int i = 0; i < dataTables25.Rows.Count; i++)
           //     {
           //         s25 = i;

           //         string SrcDocNo = dataTables25.Rows[s25]["SrcPO_SrcDocNo"].ToString();

           //         foreach (var item in refScmBillViewDtos)
           //         {
           //             if (item.SrcBillNum == SrcDocNo)
           //             {
           //                 item.DP25 = decimal.Parse(dataTables25.Rows[s25]["RcvQtyTU"].ToString());
           //             }
           //         }
           //         //RefScmBillViewDto refScmBillViewDto = new RefScmBillViewDto();

           //         //refScmBillViewDto.SrcBillNum = SrcDocNo;

           //         //refScmBillViewDto.DP25 = decimal.Parse(dataTables25.Rows[s25]["RcvQtyTU"].ToString());

           //         //refScmBillViewDtos.Add(refScmBillViewDto);
           //     }
           //     s25++;
           // }
           // #endregion

           // logger.Error("sql_1：" + sql_1 + "," + "sql_s24：" + sql_s24 + "sql_s30：" + sql_s30 + "sql_s25：" + sql_s25);
           // foreach (IUIRecord record in _part.Model.RefScmBillView.Records)
           // {
           //     try
           //     {
           //         if (!string.IsNullOrEmpty(record["SrcBillNum"].ToString()))
           //         {
           //             DocNo = record["SrcBillNum"].ToString();

           //             // string se = refScmBillViewDtos.Where(x => x.SrcBillNum == DocNo).Select(x => x.DP24).ToString();

           //             record["PayReqFundHead_DescFlexField_PrivateDescSeg24"] = refScmBillViewDtos.Where(x => x.SrcBillNum == DocNo).Select(x => x.DP24).Sum().ToString("0.####");

           //             record["PayReqFundHead_DescFlexField_PrivateDescSeg25"] = refScmBillViewDtos.Where(x => x.SrcBillNum == DocNo).Select(x => x.DP25).FirstOrDefault().ToString("0.####");

           //             record["PayReqFundHead_DescFlexField_PrivateDescSeg30"] = refScmBillViewDtos.Where(x => x.SrcBillNum == DocNo).Select(x => x.DP30).FirstOrDefault().ToString("0.####");

           //             record["PayReqFundHead_DescFlexField_PrivateDescSeg23"] = (decimal.Parse(record["PayReqFundHead_DescFlexField_PrivateDescSeg24"].ToString()) -
           //                decimal.Parse(record["PayReqFundHead_DescFlexField_PrivateDescSeg25"].ToString())).ToString("0.####");
           //         }
           //     }
           //     catch (Exception ex)
           //     {
           //         throw new Exception("报错单号" + DocNo + "。" + "报错原因" + ex.ToString());
           //     }
           // }

           // DateTime dt24 = new DateTime();

           // dt24 = DateTime.Now;

           // logger.Error("请款SQL结束时间：" + dt24.ToString("yyyy-MM-dd:hh:mm:ss"));


        }

        //public override void BeforeRender(IPart part, EventArgs args)
        //{
        //    DateTime dt = new DateTime();

        //    dt = DateTime.Now;

        //    logger.Error("请款SQL开始时间：" + dt.ToString("yyyy-MM-dd:hh:mm:ss"));

        //    this._part = (part as APRefScmBillUIFormWebPart);

        //    if (_part == null)
        //        return;

        //    string DocNo = "";

        //    string ContractsNumber = "0";//合同数量

        //    string RuKuNum = "0";//已入库数量

        //    string InvoicesNum = "0";//已开票数量
        //    string docno = "'0',";
        //    string itemcode = "'0',";
        //    string item_id = "'0',";
        //    string srcDataNum = "'0',";
        //    List<refSDto> refSDtos = new List<refSDto>();
        //    foreach (var item in _part.Model.RefScmBillLineView.Records)
        //    {
        //        docno = docno + "'" + item["SrcBillNum"].ToString() + "'" + ",";
        //        itemcode = itemcode + "'" + item["Item_Code"].ToString() + "'" + ",";
        //        item_id = item_id + "'" + item["Item"].ToString() + "'" + ",";
        //        srcDataNum = srcDataNum + "'" + item["SrcDataNum"].ToString() + "'" + ",";
        //        docno = string.Join(",", docno.Split(',').Distinct().ToArray());
        //        itemcode = string.Join(",", itemcode.Split(',').Distinct().ToArray());
        //        item_id = string.Join(",", item_id.Split(',').Distinct().ToArray());
        //        srcDataNum = string.Join(",", srcDataNum.Split(',').Distinct().ToArray());
        //        refSDto refSDto = new refSDto();
        //        refSDto.id = item["SrcBill"].ToString();//对应采购订单的id
        //        refSDto.srcbillnum = item["SrcBillNum"].ToString();
        //        refSDto.item_Code = item["Item_Code"].ToString();
        //        refSDto.item = item["Item"].ToString();
        //        refSDto.srcDataNum = item["SrcDataNum"].ToString();
        //        refSDto.canPrePayPUAmount = item["CanPrePayPUAmount"].ToString();
        //        refSDtos.Add(refSDto);
        //    }
        //    //string sql = string.Format("select ID from U9CCustNrknor_DeductDoc where BeginDate>='{0}' and EndDate<='{1}' and DocVersion='{2}' and Org={3} and ID!={4}", item.BeginDate.ToString("yyyy-MM-dd"), item.EndDate.ToString("yyyy-MM-dd"), item.DocVersion, PDContext.Current.OrgID, item.ID);
        //    //DataTable dt = GetDataTable(sql);
        //    //return dt;

        //    //DataTable dataTable = new DataTable();
        //    //DataSet dataSet = new DataSet();

        //    string sql_1 = "SELECT   PM_PurchaseOrder.DocNo,PM_PurchaseOrder.ID,PM_POLine.DocLineNo,PM_POLine.TotalRecievedQtyCU,PM_POLine.TotalConfirmedQtyCU,ItemInfo_ItemID,ItemInfo_ItemCode" +
        //   "  ,(SELECT TOP(1) AccrueDate FROM AP_APBillLine INNER JOIN AP_APBillHead ON AP_APBillHead.ID = AP_APBillLine.APBillHead WHERE SrcBillPOID" +
        //   "  = PM_PurchaseOrder.ID order by  AP_APBillHead.ModifiedOn ASC ) as AccrueDate," +
        //   "  (SELECT SUM(PUAmount) AS Sum_PUAmount FROM AP_PayReqFundDetail WHERE SrcBillID = PM_PurchaseOrder.ID " +
        //    "   ) as sum_PUAmount" +
        //   "  FROM PM_POLine " +
        //   "  INNER JOIN PM_PurchaseOrder ON PM_POLine.PurchaseOrder=PM_PurchaseOrder.ID" +
        //   "  WHERE PM_PurchaseOrder.DocNo in (" + docno.Substring(0, docno.Length - 1) + ")AND ItemInfo_ItemCode in ( " + itemcode.Substring(0, itemcode.Length - 1) + ")" +
        //   "  Order By DocLineNo DESC";
        //    //DataAccessor.RunSQL(DataAccessor.GetConn(), sql_1, null, out dataSet);
        //    //dataTable = dataSet.Tables[0];


        //    DataTable dataTable = U9Common.GetDataTable(sql_1);

        //    int n = 0;
        //    if (dataTable.Rows != null && dataTable.Rows.Count > 0)
        //    {
        //        for (int i = 0; i < dataTable.Rows.Count; i++)
        //        {
        //            n = i;
        //            string ID = dataTable.Rows[n]["ID"].ToString();
        //            string ItemInfo_ItemCode = dataTable.Rows[n]["ItemInfo_ItemCode"].ToString();
        //            string SrcDataNum = dataTable.Rows[n]["DocNo"].ToString();
        //            List<refSDto> NrefSDtos = refSDtos.Where(x => x.item_Code == ItemInfo_ItemCode && x.srcbillnum == SrcDataNum).ToList();
        //            int NCount = NrefSDtos.Count();
        //            if (NCount > 0)
        //            {
        //                foreach (var item in NrefSDtos)
        //                {
        //                    string see = item.item_Code;
        //                    decimal pu = 0;
        //                    bool a = decimal.TryParse(dataTable.Rows[n]["sum_PUAmount"].ToString(), out pu);
        //                    decimal canPrePayPUAmount = 0;
        //                    bool b = decimal.TryParse(item.canPrePayPUAmount, out canPrePayPUAmount);
        //                    foreach (IUIRecord record in _part.Model.RefScmBillLineView.Records)
        //                    {
        //                        if (record["Item_Code"].ToString() == dataTable.Rows[n]["ItemInfo_ItemCode"].ToString())
        //                        {
        //                            string see123123 = record["SrcDataNum"].ToString();

        //                            record["PayReqFundHead_DescFlexField_PrivateDescSeg29"] = decimal.Parse(dataTable.Rows[n]["TotalRecievedQtyCU"].ToString()).ToString("0.####");//入库数量
        //                            record["PayReqFundHead_DescFlexField_PrivateDescSeg28"] = decimal.Parse(dataTable.Rows[n]["TotalConfirmedQtyCU"].ToString()).ToString("0.####");//开票数量
        //                            if (dataTable.Rows[n]["AccrueDate"] == null || dataTable.Rows[n]["AccrueDate"].ToString() == "")
        //                            {
        //                                record["PayReqFundHead_DescFlexField_PrivateDescSeg27"] = "";
        //                            }
        //                            else
        //                            {
        //                                record["PayReqFundHead_DescFlexField_PrivateDescSeg27"] = Convert.ToDateTime(dataTable.Rows[n]["AccrueDate"]).ToString("yyyy-MM-dd");//最后到票日期
        //                            }
        //                            record["PayReqFundHead_DescFlexField_PrivateDescSeg26"] = canPrePayPUAmount - pu;//预付数量
        //                        }
        //                    }
        //                }
        //            }
        //        }
        //        n++;
        //    }

        //    List<RefScmBillViewDto> refScmBillViewDtos = new List<RefScmBillViewDto>();

        //    string SrcBillNum = "";

        //    foreach (IUIRecord record in _part.Model.RefScmBillView.Records)
        //    {
        //        try
        //        {
        //            if (!string.IsNullOrEmpty(record["SrcBillNum"].ToString()))
        //            {
        //                RefScmBillViewDto refScmBillViewDto = new RefScmBillViewDto();

        //                refScmBillViewDto.SrcBillNum = record["SrcBillNum"].ToString();

        //                DocNo = record["SrcBillNum"].ToString();

        //                SrcBillNum = SrcBillNum + "'" + record["SrcBillNum"].ToString() + "'" + ",";

        //                //refScmBillViewDto.DP24 = decimal.Parse(FindDesPri24(DocNo));

        //                //record["PayReqFundHead_DescFlexField_PrivateDescSeg25"] = decimal.Parse(FindDesPri25(DocNo));

        //                //record["PayReqFundHead_DescFlexField_PrivateDescSeg30"] = decimal.Parse(FindDesPri30(DocNo));

        //                refScmBillViewDtos.Add(refScmBillViewDto);
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            throw new Exception("报错单号" + DocNo + "。" + "报错原因" + ex.ToString());
        //        }
        //    }

        //    SrcBillNum = string.Join(",", SrcBillNum.Split(',').Distinct().ToArray());

        //    #region 私有字段24
        //    DataTable dataTables24 = new DataTable();
        //    DataSet dataSets24 = new DataSet();
        //    string sql_s24 = "select  SUM(ReqQtyTU) AS ReqQtyTU,A1.DocNo from PM_PurchaseOrder A1" +
        //        " left join PM_POLine A2 on A1.ID = A2.PurchaseOrder" +
        //        " where A1.DocNo in (" + SrcBillNum.Substring(0, SrcBillNum.Length - 1) + ") GROUP BY ReqQtyTU,A1.DocNo";
        //    DataAccessor.RunSQL(DataAccessor.GetConn(), sql_s24, null, out dataSets24);
        //    dataTables24 = dataSets24.Tables[0];
        //    #region  私有字段24
        //    int s24 = 0;
        //    if (dataTables24.Rows != null && dataTables24.Rows.Count > 0)
        //    {
        //        for (int i = 0; i < dataTables24.Rows.Count; i++)
        //        {
        //            s24 = i;

        //            string SrcDocNo = dataTables24.Rows[s24]["DocNo"].ToString();

        //            //foreach (var item in refScmBillViewDtos)
        //            //{
        //            //    if (item.SrcBillNum == SrcDocNo)
        //            //    {
        //            //        item.DP24 = decimal.Parse(dataTables24.Rows[s24]["ReqQtyTU"].ToString());
        //            //    }
        //            //}
        //            RefScmBillViewDto refScmBillViewDto = new RefScmBillViewDto();

        //            refScmBillViewDto.SrcBillNum = SrcDocNo;

        //            refScmBillViewDto.DP24 = decimal.Parse(dataTables24.Rows[s24]["ReqQtyTU"].ToString());

        //            refScmBillViewDtos.Add(refScmBillViewDto);
        //        }
        //        s24++;
        //    }
        //    #endregion
        //    #endregion

        //    #region 私有字段30
        //    DataTable dataTables30 = new DataTable();
        //    DataSet dataSets30 = new DataSet();
        //    string sql_s30 = "SELECT" +
        //        " (SELECT SUM(PUAmount) FROM AP_APBillHead B1 left join AP_APBillLine B2 ON B1.ID = B2.APBillHead " +
        //        " WHERE B2.SrcBillPONum = A2.SrcBillPONum) AS PUAmount," +
        //        " A2.SrcBillPONum FROM AP_APBillHead A1 left join AP_APBillLine A2 ON A1.ID = A2.APBillHead " +
        //        " WHERE A2.SrcBillPONum  in (" + SrcBillNum.Substring(0, SrcBillNum.Length - 1) + ")";
        //    DataAccessor.RunSQL(DataAccessor.GetConn(), sql_s30, null, out dataSets30);
        //    dataTables30 = dataSets30.Tables[0];
        //    int s30 = 0;
        //    if (dataTables30.Rows != null && dataTables30.Rows.Count > 0)
        //    {
        //        for (int i = 0; i < dataTables30.Rows.Count; i++)
        //        {
        //            s30 = i;

        //            string SrcDocNo = dataTables30.Rows[s30]["SrcBillPONum"].ToString();

        //            foreach (var item in refScmBillViewDtos)
        //            {
        //                if (item.SrcBillNum == SrcDocNo)
        //                {
        //                    item.DP30 = decimal.Parse(dataTables30.Rows[s30]["PUAmount"].ToString());
        //                }
        //            }
        //            //RefScmBillViewDto refScmBillViewDto = new RefScmBillViewDto();

        //            //refScmBillViewDto.SrcBillNum = SrcDocNo;

        //            //refScmBillViewDto.DP30 = decimal.Parse(dataTables30.Rows[s30]["PUAmount"].ToString());

        //            //refScmBillViewDtos.Add(refScmBillViewDto);
        //        }
        //        s30++;
        //    }
        //    #endregion

        //    #region 私有字段25
        //    DataTable dataTables25 = new DataTable();
        //    DataSet dataSets25 = new DataSet();
        //    string sql_s25 = "select" +
        //        " (select  sum(B2.RcvQtyTU) AS RcvQtyTU " +
        //        " from PM_Receivement B1 " +
        //        " left join PM_RcvLine B2 on B1.ID = B2.Receivement where B2.SrcDoc_SrcDocNo = A2.SrcPO_SrcDocNo and B2.Status = 5) AS RcvQtyTU, A2.SrcPO_SrcDocNo " +
        //        " from PM_Receivement A1 left join PM_RcvLine A2 on A1.ID = A2.Receivement where A2.SrcDoc_SrcDocNo in (" + SrcBillNum.Substring(0, SrcBillNum.Length - 1) + ") and A2.Status = 5  ";
        //    DataAccessor.RunSQL(DataAccessor.GetConn(), sql_s25, null, out dataSets25);
        //    dataTables25 = dataSets25.Tables[0];
        //    int s25 = 0;
        //    if (dataTables25.Rows != null && dataTables25.Rows.Count > 0)
        //    {
        //        for (int i = 0; i < dataTables25.Rows.Count; i++)
        //        {
        //            s25 = i;

        //            string SrcDocNo = dataTables25.Rows[s25]["SrcPO_SrcDocNo"].ToString();

        //            foreach (var item in refScmBillViewDtos)
        //            {
        //                if (item.SrcBillNum == SrcDocNo)
        //                {
        //                    item.DP25 = decimal.Parse(dataTables25.Rows[s25]["RcvQtyTU"].ToString());
        //                }
        //            }
        //            //RefScmBillViewDto refScmBillViewDto = new RefScmBillViewDto();

        //            //refScmBillViewDto.SrcBillNum = SrcDocNo;

        //            //refScmBillViewDto.DP25 = decimal.Parse(dataTables25.Rows[s25]["RcvQtyTU"].ToString());

        //            //refScmBillViewDtos.Add(refScmBillViewDto);
        //        }
        //        s25++;
        //    }
        //    #endregion

        //    logger.Error("sql_1：" + sql_1 + "," + "sql_s24：" + sql_s24 + "sql_s30：" + sql_s30 + "sql_s25：" + sql_s25);
        //    foreach (IUIRecord record in _part.Model.RefScmBillView.Records)
        //    {
        //        try
        //        {
        //            if (!string.IsNullOrEmpty(record["SrcBillNum"].ToString()))
        //            {
        //                DocNo = record["SrcBillNum"].ToString();

        //                // string se = refScmBillViewDtos.Where(x => x.SrcBillNum == DocNo).Select(x => x.DP24).ToString();

        //                record["PayReqFundHead_DescFlexField_PrivateDescSeg24"] = refScmBillViewDtos.Where(x => x.SrcBillNum == DocNo).Select(x => x.DP24).Sum().ToString("0.####");

        //                record["PayReqFundHead_DescFlexField_PrivateDescSeg25"] = refScmBillViewDtos.Where(x => x.SrcBillNum == DocNo).Select(x => x.DP25).FirstOrDefault().ToString("0.####");

        //                record["PayReqFundHead_DescFlexField_PrivateDescSeg30"] = refScmBillViewDtos.Where(x => x.SrcBillNum == DocNo).Select(x => x.DP30).FirstOrDefault().ToString("0.####");
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            throw new Exception("报错单号" + DocNo + "。" + "报错原因" + ex.ToString());
        //        }
        //    }

        //    DateTime dt2 = new DateTime();

        //    dt2 = DateTime.Now;

        //    logger.Error("请款SQL结束时间：" + dt2.ToString("yyyy-MM-dd:hh:mm:ss"));

        //}

        public override void BeforeRender(IPart part, EventArgs args)
        {
            DateTime dt2 = new DateTime();

            dt2 = DateTime.Now;

            logger.Error("列表行结束时间：" + dt2.ToString("yyyy-MM-dd:hh:mm:ss"));

            DateTime dt3 = new DateTime();

            dt3 = DateTime.Now;

            logger.Error("请款SQL开始时间：" + dt3.ToString("yyyy-MM-dd:hh:mm:ss"));

            this._part = (part as APRefScmBillUIFormWebPart);

            if (_part == null)
                return;

            string DocNo = "";

            string ContractsNumber = "0";//合同数量

            string RuKuNum = "0";//已入库数量

            string InvoicesNum = "0";//已开票数量
            string docno = "'0',";
            string itemcode = "'0',";
            string item_id = "'0',";
            string srcDataNum = "'0',";
            List<refSDto> refSDtos = new List<refSDto>();
            foreach (var item in _part.Model.RefScmBillLineView.Records)
            {
                docno = docno + "'" + item["SrcBillNum"].ToString() + "'" + ",";
                itemcode = itemcode + "'" + item["Item_Code"].ToString() + "'" + ",";
                item_id = item_id + "'" + item["Item"].ToString() + "'" + ",";
                srcDataNum = srcDataNum + "'" + item["SrcDataNum"].ToString() + "'" + ",";
                docno = string.Join(",", docno.Split(',').Distinct().ToArray());
                itemcode = string.Join(",", itemcode.Split(',').Distinct().ToArray());
                item_id = string.Join(",", item_id.Split(',').Distinct().ToArray());
                srcDataNum = string.Join(",", srcDataNum.Split(',').Distinct().ToArray());
                refSDto refSDto = new refSDto();
                refSDto.id = item["SrcBill"].ToString();//对应采购订单的id
                refSDto.srcbillnum = item["SrcBillNum"].ToString();
                refSDto.item_Code = item["Item_Code"].ToString();
                refSDto.item = item["Item"].ToString();
                refSDto.srcDataNum = item["SrcDataNum"].ToString();
                refSDto.canPrePayPUAmount = item["CanPrePayPUAmount"].ToString();
                refSDtos.Add(refSDto);
            }
            //string sql = string.Format("select ID from U9CCustNrknor_DeductDoc where BeginDate>='{0}' and EndDate<='{1}' and DocVersion='{2}' and Org={3} and ID!={4}", item.BeginDate.ToString("yyyy-MM-dd"), item.EndDate.ToString("yyyy-MM-dd"), item.DocVersion, PDContext.Current.OrgID, item.ID);
            //DataTable dt = GetDataTable(sql);
            //return dt;

            //DataTable dataTable = new DataTable();
            //DataSet dataSet = new DataSet();

            string sql_1 = "SELECT   PM_PurchaseOrder.DocNo,PM_PurchaseOrder.ID,PM_POLine.DocLineNo,PM_POLine.TotalRecievedQtyCU,PM_POLine.TotalConfirmedQtyCU,ItemInfo_ItemID,ItemInfo_ItemCode" +
           "  ,(SELECT TOP(1) AccrueDate FROM AP_APBillLine INNER JOIN AP_APBillHead ON AP_APBillHead.ID = AP_APBillLine.APBillHead WHERE SrcBillPOID" +
           "  = PM_PurchaseOrder.ID order by  AP_APBillHead.ModifiedOn ASC ) as AccrueDate," +
           "  (SELECT SUM(PUAmount) AS Sum_PUAmount FROM AP_PayReqFundDetail WHERE SrcBillID = PM_PurchaseOrder.ID " +
            "   ) as sum_PUAmount" +
           "  FROM PM_POLine " +
           "  INNER JOIN PM_PurchaseOrder ON PM_POLine.PurchaseOrder=PM_PurchaseOrder.ID" +
           "  WHERE PM_PurchaseOrder.DocNo in (" + docno.Substring(0, docno.Length - 1) + ")AND ItemInfo_ItemCode in ( " + itemcode.Substring(0, itemcode.Length - 1) + ")" +
           "  Order By DocLineNo DESC";
            //DataAccessor.RunSQL(DataAccessor.GetConn(), sql_1, null, out dataSet);
            //dataTable = dataSet.Tables[0];


            DataTable dataTable = U9Common.GetDataTable(sql_1);

            int n = 0;
            if (dataTable.Rows != null && dataTable.Rows.Count > 0)
            {
                for (int i = 0; i < dataTable.Rows.Count; i++)
                {
                    n = i;
                    string ID = dataTable.Rows[n]["ID"].ToString();
                    string ItemInfo_ItemCode = dataTable.Rows[n]["ItemInfo_ItemCode"].ToString();
                    string SrcDataNum = dataTable.Rows[n]["DocNo"].ToString();
                    List<refSDto> NrefSDtos = refSDtos.Where(x => x.item_Code == ItemInfo_ItemCode && x.srcbillnum == SrcDataNum).ToList();
                    int NCount = NrefSDtos.Count();
                    if (NCount > 0)
                    {
                        foreach (var item in NrefSDtos)
                        {
                            string see = item.item_Code;
                            decimal pu = 0;
                            bool a = decimal.TryParse(dataTable.Rows[n]["sum_PUAmount"].ToString(), out pu);
                            decimal canPrePayPUAmount = 0;
                            bool b = decimal.TryParse(item.canPrePayPUAmount, out canPrePayPUAmount);
                            foreach (IUIRecord record in _part.Model.RefScmBillLineView.Records)
                            {
                                if (record["Item_Code"].ToString() == dataTable.Rows[n]["ItemInfo_ItemCode"].ToString())
                                {
                                    string see123123 = record["SrcDataNum"].ToString();

                                    record["PayReqFundHead_DescFlexField_PrivateDescSeg29"] = decimal.Parse(dataTable.Rows[n]["TotalRecievedQtyCU"].ToString()).ToString("0.####");//入库数量
                                    record["PayReqFundHead_DescFlexField_PrivateDescSeg28"] = decimal.Parse(dataTable.Rows[n]["TotalConfirmedQtyCU"].ToString()).ToString("0.####");//开票数量
                                    if (dataTable.Rows[n]["AccrueDate"] == null || dataTable.Rows[n]["AccrueDate"].ToString() == "")
                                    {
                                        record["PayReqFundHead_DescFlexField_PrivateDescSeg27"] = "";
                                    }
                                    else
                                    {
                                        record["PayReqFundHead_DescFlexField_PrivateDescSeg27"] = Convert.ToDateTime(dataTable.Rows[n]["AccrueDate"]).ToString("yyyy-MM-dd");//最后到票日期
                                    }
                                    record["PayReqFundHead_DescFlexField_PrivateDescSeg26"] = canPrePayPUAmount - pu;//预付数量
                                }
                            }
                        }
                    }
                }
                n++;
            }

            List<RefScmBillViewDto> refScmBillViewDtos = new List<RefScmBillViewDto>();

            string SrcBillNum = "";

            foreach (IUIRecord record in _part.Model.RefScmBillView.Records)
            {
                try
                {
                    if (!string.IsNullOrEmpty(record["SrcBillNum"].ToString()))
                    {
                        RefScmBillViewDto refScmBillViewDto = new RefScmBillViewDto();

                        refScmBillViewDto.SrcBillNum = record["SrcBillNum"].ToString();

                        DocNo = record["SrcBillNum"].ToString();

                        SrcBillNum = SrcBillNum + "'" + record["SrcBillNum"].ToString() + "'" + ",";

                        //refScmBillViewDto.DP24 = decimal.Parse(FindDesPri24(DocNo));

                        //record["PayReqFundHead_DescFlexField_PrivateDescSeg25"] = decimal.Parse(FindDesPri25(DocNo));

                        //record["PayReqFundHead_DescFlexField_PrivateDescSeg30"] = decimal.Parse(FindDesPri30(DocNo));

                        refScmBillViewDtos.Add(refScmBillViewDto);
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("报错单号" + DocNo + "。" + "报错原因" + ex.ToString());
                }
            }

            SrcBillNum = string.Join(",", SrcBillNum.Split(',').Distinct().ToArray());

            #region 私有字段24
            DataTable dataTables24 = new DataTable();
            DataSet dataSets24 = new DataSet();
            string sql_s24 = "select  SUM(ReqQtyTU) AS ReqQtyTU,A1.DocNo from PM_PurchaseOrder A1" +
                " left join PM_POLine A2 on A1.ID = A2.PurchaseOrder" +
                " where A1.DocNo in (" + SrcBillNum.Substring(0, SrcBillNum.Length - 1) + ") GROUP BY ReqQtyTU,A1.DocNo";
            DataAccessor.RunSQL(DataAccessor.GetConn(), sql_s24, null, out dataSets24);
            dataTables24 = dataSets24.Tables[0];
            #region  私有字段24
            int s24 = 0;
            if (dataTables24.Rows != null && dataTables24.Rows.Count > 0)
            {
                for (int i = 0; i < dataTables24.Rows.Count; i++)
                {
                    s24 = i;

                    string SrcDocNo = dataTables24.Rows[s24]["DocNo"].ToString();

                    //foreach (var item in refScmBillViewDtos)
                    //{
                    //    if (item.SrcBillNum == SrcDocNo)
                    //    {
                    //        item.DP24 = decimal.Parse(dataTables24.Rows[s24]["ReqQtyTU"].ToString());
                    //    }
                    //}
                    RefScmBillViewDto refScmBillViewDto = new RefScmBillViewDto();

                    refScmBillViewDto.SrcBillNum = SrcDocNo;

                    refScmBillViewDto.DP24 = decimal.Parse(dataTables24.Rows[s24]["ReqQtyTU"].ToString());

                    refScmBillViewDtos.Add(refScmBillViewDto);
                }
                s24++;
            }
            #endregion
            #endregion

            #region 私有字段30
            DataTable dataTables30 = new DataTable();
            DataSet dataSets30 = new DataSet();
            string sql_s30 = "SELECT" +
                " (SELECT SUM(PUAmount) FROM AP_APBillHead B1 left join AP_APBillLine B2 ON B1.ID = B2.APBillHead " +
                " WHERE B2.SrcBillPONum = A2.SrcBillPONum) AS PUAmount," +
                " A2.SrcBillPONum FROM AP_APBillHead A1 left join AP_APBillLine A2 ON A1.ID = A2.APBillHead " +
                " WHERE A2.SrcBillPONum  in (" + SrcBillNum.Substring(0, SrcBillNum.Length - 1) + ")";
            DataAccessor.RunSQL(DataAccessor.GetConn(), sql_s30, null, out dataSets30);
            dataTables30 = dataSets30.Tables[0];
            int s30 = 0;
            if (dataTables30.Rows != null && dataTables30.Rows.Count > 0)
            {
                for (int i = 0; i < dataTables30.Rows.Count; i++)
                {
                    s30 = i;

                    string SrcDocNo = dataTables30.Rows[s30]["SrcBillPONum"].ToString();

                    foreach (var item in refScmBillViewDtos)
                    {
                        if (item.SrcBillNum == SrcDocNo)
                        {
                            item.DP30 = decimal.Parse(dataTables30.Rows[s30]["PUAmount"].ToString());
                        }
                    }
                    //RefScmBillViewDto refScmBillViewDto = new RefScmBillViewDto();

                    //refScmBillViewDto.SrcBillNum = SrcDocNo;

                    //refScmBillViewDto.DP30 = decimal.Parse(dataTables30.Rows[s30]["PUAmount"].ToString());

                    //refScmBillViewDtos.Add(refScmBillViewDto);
                }
                s30++;
            }
            #endregion

            #region 私有字段25
            DataTable dataTables25 = new DataTable();
            DataSet dataSets25 = new DataSet();
            string sql_s25 = "select" +
                " (select  sum(B2.RcvQtyTU) AS RcvQtyTU " +
                " from PM_Receivement B1 " +
                " left join PM_RcvLine B2 on B1.ID = B2.Receivement where B2.SrcDoc_SrcDocNo = A2.SrcPO_SrcDocNo and B2.Status = 5) AS RcvQtyTU, A2.SrcPO_SrcDocNo " +
                " from PM_Receivement A1 left join PM_RcvLine A2 on A1.ID = A2.Receivement where A2.SrcDoc_SrcDocNo in (" + SrcBillNum.Substring(0, SrcBillNum.Length - 1) + ") and A2.Status = 5  ";
            DataAccessor.RunSQL(DataAccessor.GetConn(), sql_s25, null, out dataSets25);
            dataTables25 = dataSets25.Tables[0];
            int s25 = 0;
            if (dataTables25.Rows != null && dataTables25.Rows.Count > 0)
            {
                for (int i = 0; i < dataTables25.Rows.Count; i++)
                {
                    s25 = i;

                    string SrcDocNo = dataTables25.Rows[s25]["SrcPO_SrcDocNo"].ToString();

                    foreach (var item in refScmBillViewDtos)
                    {
                        if (item.SrcBillNum == SrcDocNo)
                        {
                            item.DP25 = decimal.Parse(dataTables25.Rows[s25]["RcvQtyTU"].ToString());
                        }
                    }
                    //RefScmBillViewDto refScmBillViewDto = new RefScmBillViewDto();

                    //refScmBillViewDto.SrcBillNum = SrcDocNo;

                    //refScmBillViewDto.DP25 = decimal.Parse(dataTables25.Rows[s25]["RcvQtyTU"].ToString());

                    //refScmBillViewDtos.Add(refScmBillViewDto);
                }
                s25++;
            }
            #endregion

            logger.Error("sql_1：" + sql_1 + "," + "sql_s24：" + sql_s24 + "sql_s30：" + sql_s30 + "sql_s25：" + sql_s25);
            foreach (IUIRecord record in _part.Model.RefScmBillView.Records)
            {
                try
                {
                    if (!string.IsNullOrEmpty(record["SrcBillNum"].ToString()))
                    {
                        DocNo = record["SrcBillNum"].ToString();

                        // string se = refScmBillViewDtos.Where(x => x.SrcBillNum == DocNo).Select(x => x.DP24).ToString();

                        record["PayReqFundHead_DescFlexField_PrivateDescSeg24"] = refScmBillViewDtos.Where(x => x.SrcBillNum == DocNo).Select(x => x.DP24).Sum().ToString("0.####");

                        record["PayReqFundHead_DescFlexField_PrivateDescSeg25"] = refScmBillViewDtos.Where(x => x.SrcBillNum == DocNo).Select(x => x.DP25).FirstOrDefault().ToString("0.####");

                        record["PayReqFundHead_DescFlexField_PrivateDescSeg30"] = refScmBillViewDtos.Where(x => x.SrcBillNum == DocNo).Select(x => x.DP30).FirstOrDefault().ToString("0.####");

                        record["PayReqFundHead_DescFlexField_PrivateDescSeg23"] = (decimal.Parse(record["PayReqFundHead_DescFlexField_PrivateDescSeg24"].ToString()) -
                           decimal.Parse(record["PayReqFundHead_DescFlexField_PrivateDescSeg25"].ToString())).ToString("0.####");
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("报错单号" + DocNo + "。" + "报错原因" + ex.ToString());
                }
            }

            DateTime dt24 = new DateTime();

            dt24 = DateTime.Now;

            logger.Error("请款SQL结束时间：" + dt24.ToString("yyyy-MM-dd:hh:mm:ss"));
        }

        public class RefScmBillViewDto
        {
            public string SrcBillNum { get; set; }
            public decimal DP24 { get; set; }
            public decimal DP25 { get; set; }
            public decimal DP30 { get; set; }
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
            if (dataTable.Rows != null && dataTable.Rows.Count > 0)
            {
                SumNum = dataTable.Rows[0]["ReqQtyTU"] == null ? "0" : dataTable.Rows[0]["ReqQtyTU"].ToString();
            }
            #endregion
            SumNum = decimal.Parse(SumNum).ToString("#0.####");
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

        public class refSDto
        {
            /// <summary>
            /// id
            /// </summary>
            public string id { get; set; }
            /// <summary>
            /// srcbillnum
            /// </summary>
            public string srcbillnum { get; set; }
            /// <summary>
            /// item_Code
            /// </summary>
            public string item_Code { get; set; }
            /// <summary>
            /// item
            /// </summary>
            public string item { get; set; }
            /// <summary>
            /// srcDataNum
            /// </summary>
            public string srcDataNum { get; set; }
            /// <summary>
            /// 预付款需要减的量
            /// </summary>
            public string canPrePayPUAmount { get; set; }

        }
    }
}
