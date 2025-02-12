using System;
using System.Collections.Generic;
using System.Data;
using UFIDA.U9.FI.AP.PayReqFundUIModel;
using UFSoft.UBF.UI.ControlModel;
using UFSoft.UBF.UI.Custom;
using UFSoft.UBF.UI.IView;
using UFSoft.UBF.UI.MD.Runtime;
using UFSoft.UBF.Util.DataAccess;

namespace YY.U9.Cust.LI.UIPlugIn
{
    class APRefScmBillUIFormWebPartExtended : UFSoft.UBF.UI.Custom.ExtendedPartBase
    {
        private APRefScmBillUIFormWebPart _part; 
        /// <summary>
        /// 初始化后扩展
        /// UFIDA.U9.BS.Job.RequestClient.dll
        /// </summary>
        /// <param name="part"></param>
        /// <param name="e"></param>        
        public override void AfterInit(IPart part, System.EventArgs e)
        {
            base.AfterInit(part, e);
            this._part = (part as APRefScmBillUIFormWebPart);
        }

        public override void AfterRender(IPart Part, EventArgs args)
        {
            base.AfterRender(Part, args);
        }

        public override void BeforeEventProcess(IPart Part, string eventName, object sender, EventArgs args, out bool executeDefault)
        {
            base.BeforeEventProcess(Part, eventName, sender, args, out executeDefault);
            if (sender is IUFButton)
            {
                _part = (Part as APRefScmBillUIFormWebPart);
                string controlID = ((IUFButton)sender).ID;
                if (controlID == "BtnOk")
                {
                    if (this._part.Model.ErrorMessage.hasErrorMessage)
                    {
                        this._part.Model.ClearErrorMessage(); ;
                    }
                    string apModel = this._part.Model.PayReqFundHead.FocusedRecord.DocumentType_Code;
                    string text = null;
                    if (apModel.Equals("003"))
                    {
                        text = "材料";
                    }
                    if (apModel.Equals("004"))
                    {
                        text = "资产";
                    }
                    if (apModel.Equals("005"))
                    {
                        text = "工程";
                    }
                    if (apModel.Equals("006"))
                    {
                        text = "费用";
                    }
                    IList<IUIRecord> selectRecords = this._part.Model.RefScmBillView.GetSelectRecords();
                    IList<IUIRecord> selectRecords2 = this._part.Model.RefScmBillLineView.GetSelectRecords();
                    if ((selectRecords != null && selectRecords.Count != 0) || (selectRecords2 != null && selectRecords2.Count != 0))
                    {
                        List<string> list = new List<string>();
                        List<string> list2 = new List<string>();
                        List<string> list3 = new List<string>();
                        if (selectRecords != null && selectRecords.Count > 0)
                        {
                            foreach (IUIRecord current in selectRecords)
                            {
                                RefScmBillViewRecord refScmBillViewRecord = (RefScmBillViewRecord)current;
                                DataTable dataTable = new DataTable();
                                string text2 = "select c.ItemFormAttribute,c.Code from PM_POLine a inner join PM_PurchaseOrder b on b.ID = a.PurchaseOrder  inner join CBO_ItemMaster c on c.ID = a.ItemInfo_ItemID  where b.DocNo = '" + refScmBillViewRecord.SrcBillNum + "'";
                                DataSet dataSet = new DataSet();
                                DataAccessor.RunSQL(DataAccessor.GetConn(), text2, null, out dataSet);
                                dataTable = dataSet.Tables[0];
                                if (dataTable.Rows != null && dataTable.Rows.Count > 0)
                                {
                                    for (int i = 0; i < dataTable.Rows.Count; i++)
                                    {
                                        if (apModel.Equals("003") && (dataTable.Rows[i]["ItemFormAttribute"].ToString().Equals("17") || dataTable.Rows[i]["ItemFormAttribute"].ToString().Equals("18")))
                                        {
                                            list.Add(refScmBillViewRecord.SrcBillNum);
                                            break;
                                        }
                                        if (apModel.Equals("004") && !dataTable.Rows[i]["ItemFormAttribute"].ToString().Equals("18"))
                                        {
                                            list.Add(refScmBillViewRecord.SrcBillNum);
                                            break;
                                        }
                                        if (apModel.Equals("005") && !dataTable.Rows[i]["Code"].ToString().Equals("FY0034"))
                                        {
                                            list.Add(refScmBillViewRecord.SrcBillNum);
                                            break;
                                        }
                                        if (apModel.Equals("006") && dataTable.Rows[i]["Code"].ToString().Equals("FY0034"))
                                        {
                                            list.Add(refScmBillViewRecord.SrcBillNum);
                                            break;
                                        }
                                        if (apModel.Equals("006") && !dataTable.Rows[i]["Code"].ToString().Equals("FY0034") && !dataTable.Rows[i]["ItemFormAttribute"].ToString().Equals("17"))
                                        {
                                            list.Add(refScmBillViewRecord.SrcBillNum);
                                            break;
                                        }
                                    }
                                }
                            }
                            if (text != null && list != null && list.Count > 0)
                            {
                                string message = string.Concat(new string[]
								{
									"请款单据类型为",
									text,
									"请款，所选记录中有不满足条件的采购订单，单号：",
									string.Join(",", list),
									"，请选择满足条件的采购订单行！"
								});
                                throw new Exception(message);
                            }
                        }
                        if (selectRecords2 != null && selectRecords2.Count > 0)
                        {
                            foreach (IUIRecord current in selectRecords2)
                            {
                                RefScmBillLineViewRecord refScmBillLineViewRecord = (RefScmBillLineViewRecord)current;
                                DataTable dataTable = new DataTable();
                                string text2 = string.Concat(new string[]
								{
									"select c.ItemFormAttribute,c.Code from PM_POLine a inner join PM_PurchaseOrder b on b.ID = a.PurchaseOrder  inner join CBO_ItemMaster c on c.ID = a.ItemInfo_ItemID  where b.DocNo = '",
									refScmBillLineViewRecord.SrcBillNum,
									"' and a.ItemInfo_ItemCode =  '",
									refScmBillLineViewRecord.Item_Code,
									"'"
								});
                                DataSet dataSet = new DataSet();
                                DataAccessor.RunSQL(DataAccessor.GetConn(), text2, null, out dataSet);
                                dataTable = dataSet.Tables[0];
                                if (dataTable.Rows != null && dataTable.Rows.Count > 0)
                                {
                                    for (int i = 0; i < dataTable.Rows.Count; i++)
                                    {
                                        if (apModel.Equals("003") && (dataTable.Rows[i]["ItemFormAttribute"].ToString().Equals("17") || dataTable.Rows[i]["ItemFormAttribute"].ToString().Equals("18")))
                                        {
                                            list2.Add(refScmBillLineViewRecord.SrcBillNum);
                                            list3.Add(refScmBillLineViewRecord.Item_Code);
                                        }
                                        if (apModel.Equals("004") && !dataTable.Rows[i]["ItemFormAttribute"].ToString().Equals("18"))
                                        {
                                            list2.Add(refScmBillLineViewRecord.SrcBillNum);
                                            list3.Add(refScmBillLineViewRecord.Item_Code);
                                        }
                                        if (apModel.Equals("005") && !dataTable.Rows[i]["Code"].ToString().Equals("FY0034"))
                                        {
                                            list2.Add(refScmBillLineViewRecord.SrcBillNum);
                                            list3.Add(refScmBillLineViewRecord.Item_Code);
                                        }
                                        if (apModel.Equals("006") && dataTable.Rows[i]["Code"].ToString().Equals("FY0034"))
                                        {
                                            list2.Add(refScmBillLineViewRecord.SrcBillNum);
                                            list3.Add(refScmBillLineViewRecord.Item_Code);
                                        }
                                        if (apModel.Equals("006") && !dataTable.Rows[i]["Code"].ToString().Equals("FY0034") && !dataTable.Rows[i]["ItemFormAttribute"].ToString().Equals("17"))
                                        {
                                            list2.Add(refScmBillLineViewRecord.SrcBillNum);
                                            list3.Add(refScmBillLineViewRecord.Item_Code);
                                        }
                                    }
                                }
                            }
                            if (text != null && list2 != null && list2.Count > 0)
                            {
                                string message = string.Concat(new string[]
								{
									"请款单据类型为",
									text,
									"请款，所选记录中有不满足条件的采购订单行，单号：",
									string.Join(",", list2),
									"，料号：",
									string.Join(",", list3)
								});
                                throw new Exception(message);
                            }
                        }
                    }
                }
            }
        }





    }
}