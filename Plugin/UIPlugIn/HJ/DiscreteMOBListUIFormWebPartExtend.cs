using System;
using System.Collections.Generic;
using System.Data;
using UFIDA.U9.Base.SystemDock;
using UFIDA.U9.Base.SystemDock.Proxy;
using UFIDA.U9.MFG.MO.DiscreteMOBListUIModel;
using UFIDA.U9.MFG.MO.DiscreteMOUIModel;
using UFIDA.U9.UI.PDHelper;
using UFSoft.UBF.UI.ControlModel;
using UFSoft.UBF.UI.Custom;
using UFSoft.UBF.UI.IView;
using UFSoft.UBF.UI.MD.Runtime;
using UFSoft.UBF.UI.WebControlAdapter;
using UFSoft.UBF.UI.WebControls;
using UFSoft.UBF.Util.DataAccess;
using System.Text;
using UFSoft.UBF.Business;
using UFSoft.UBF.ExportService;
using UFSoft.UBF.UI.ActionProcess;
using UFIDA.U9.PM.PurchaseOrderUIModel;
using UFSoft.UBF.Util.Log;
using System.IO;
using UFSoft.UBF.Analysis.Interface.MD.Report.Model;
using UFSoft.UBF.UI.Engine.Builder;
using UFIDA.U9.UI.Commands;
namespace YY.U9.Cust.AY.UIPlugIn
{
    class DiscreteMOBListUIFormWebPartExtend : UFSoft.UBF.UI.Custom.ExtendedPartBase
    {
        private DiscreteMOBListUIFormWebPart _part;

        IUFMenu BtnSettle;

        /// <summary>
        /// 初始化后扩展
        /// </summary>
        /// <param name="part"></param>
        /// <param name="e"></param>
        public override void AfterInit(IPart part, System.EventArgs e)
        {
            base.AfterInit(part, e);
            _part = part as DiscreteMOBListUIFormWebPart;
            IUFButton btnPagPrint = new UFWebButtonAdapter();
            IUFToolbar toolbar = (IUFToolbar)_part.GetUFControlByName(_part.TopLevelContainer, "Toolbar1");

            BtnSettle = new UFWebMenuAdapter();
            BtnSettle.Text = "报废入库维护 ";
            BtnSettle.ID = "BtnSettle";
            BtnSettle.AutoPostBack = true;
            BtnSettle.ItemClick += new MenuItemHandle(BtnSettle_Click);

            //加入操作里面
            IUFDropDownButton DdbOperation = (IUFDropDownButton)_part.GetUFControlByName(part.TopLevelContainer, "DDBtnOperation");
            DdbOperation.MenuItems.Add(BtnSettle);
            if (toolbar != null)
            {
                string guid = "C37A128A-7C39-4E99-8820-1E8623E927F7";
                btnPagPrint = UIControlBuilder.BuilderToolbarButton(toolbar, "True", "btnPagPrint", "True", "True", 70, 28, "7", "", true, false, guid, guid, guid);
                UIControlBuilder.SetButtonAccessKey(btnPagPrint);
                btnPagPrint.Text = "kpm导出";
                btnPagPrint.ID = "btnPagPrint";
                btnPagPrint.AutoPostBack = true;
                btnPagPrint.UIModel = _part.Model.ElementID;
                ((UFWebToolbarAdapter)toolbar).Items.Add(btnPagPrint as System.Web.UI.WebControls.WebControl);
                btnPagPrint.Click += BtnPagPrint_Click;

            }
        }



        public void BtnSettle_Click(object sender, EventArgs e)
        {
            object lineRecord;

            //收集界面错误信息
            if (this._part.Model.ErrorMessage.hasErrorMessage)
            {
                this._part.Model.ClearErrorMessage();
            }
            this._part.OnDataCollect(this);

            try
            {
                //d3955bab - 10ef - 408a - aa83 - 061fd8f3861d


                NavigateManager.ShowModelWebpart(_part, "d3955bab-10ef-408a-aa83-061fd8f3861d", _part.TaskId.ToString(), 992, 504, null, true, true);

                GetPLMDocsProxy proxy = new GetPLMDocsProxy();
                List<GetPLMDocInParaDTOData> list = new List<GetPLMDocInParaDTOData>();
                IList<IUIRecord> selectRecords = _part.Model.MO.Cache.GetSelectRecord();

                DataTable dt = new DataTable();
                dt.Columns.Add("ID", typeof(long));
                dt.Columns.Add("SysVersion", typeof(long));
                dt.Columns.Add("DocNo", typeof(string));
                dt.Columns.Add("DepartMentName", typeof(string));
                dt.Columns.Add("Project", typeof(string));
                dt.Columns.Add("ItemMasterCode", typeof(string));
                dt.Columns.Add("ItemMasterName", typeof(string));
                dt.Columns.Add("Version", typeof(string));
                //标准额定损耗
                dt.Columns.Add("StandardRatedLoss", typeof(decimal));
                //标准超额损耗
                dt.Columns.Add("StandardExcessLoss", typeof(decimal));
                //调机损耗
                dt.Columns.Add("ShuntingLoss", typeof(decimal));
                //质量损耗
                dt.Columns.Add("MassLoss", typeof(decimal));
                //制程损耗  
                dt.Columns.Add("ProcessLoss", typeof(decimal));
                //实际损耗率
                dt.Columns.Add("ActualLossRate", typeof(decimal));
                //超额损耗率
                dt.Columns.Add("ExcessLossRate", typeof(decimal));
                //差异
                dt.Columns.Add("Difference", typeof(decimal));
                //入库数量
                dt.Columns.Add("TotalRcvQty", typeof(decimal));
                //已发放数量
                dt.Columns.Add("IssuedQty", typeof(decimal));
                //BOM用量
                dt.Columns.Add("QPA", typeof(decimal));
                //特别发料量
                dt.Columns.Add("SpecialIssuedQty", typeof(decimal));
                foreach (IUIRecord record in selectRecords)
                {
                    DataRow dr = dt.NewRow();
                    DataTable dataTable = new DataTable();
                    string version = "select * from MO_MOPickList where ID='" + record["ID"] + "' ";
                    DataSet dataSet = new DataSet();
                    DataAccessor.RunSQL(DataAccessor.GetConn(), version, null, out dataSet);
                    dataTable = dataSet.Tables[0];
                    try
                    {
                        dr["ShuntingLoss"] = dataTable.Rows[0]["DescFlexField_PrivateDescSeg3"].ToString() == "" ? 0 : dataTable.Rows[0]["DescFlexField_PrivateDescSeg3"];
                        dr["MassLoss"] = dataTable.Rows[0]["DescFlexField_PrivateDescSeg4"].ToString() == "" ? 0 : dataTable.Rows[0]["DescFlexField_PrivateDescSeg4"];
                        dr["ProcessLoss"] = dataTable.Rows[0]["DescFlexField_PrivateDescSeg5"].ToString() == "" ? 0 : dataTable.Rows[0]["DescFlexField_PrivateDescSeg5"];
                        dr["ActualLossRate"] = dataTable.Rows[0]["DescFlexField_PrivateDescSeg6"].ToString() == "" ? 0 : dataTable.Rows[0]["DescFlexField_PrivateDescSeg6"];
                        dr["ExcessLossRate"] = dataTable.Rows[0]["DescFlexField_PrivateDescSeg7"].ToString() == "" ? 0 : dataTable.Rows[0]["DescFlexField_PrivateDescSeg7"];
                        dr["Difference"] = dataTable.Rows[0]["DescFlexField_PrivateDescSeg8"].ToString() == "" ? 0 : dataTable.Rows[0]["DescFlexField_PrivateDescSeg8"];
                    }
                    catch (Exception)
                    {


                    }

                    dr["ID"] = record["ID"];
                    dr["ItemMasterCode"] = record["ItemMaster_Code"];
                    dr["DocNo"] = record["DocNo"];
                    dr["DepartMentName"] = record["DepartMent_Name"];
                    dr["Project"] = record["Project_Name"];
                    dr["ItemMasterCode"] = record["MOPickLists_ItemMaster_Code"];
                    dr["ItemMasterName"] = record["MOPickLists_ItemMaster_Name"];
                    dr["Version"] = record["ItemVersion"];
                    dr["SysVersion"] = record["SysVersion"];
                    //标准额定损耗
                    dr["StandardRatedLoss"] = record["CaculateField2"];
                    //标准超额损耗
                    dr["StandardExcessLoss"] = record["CaculateField1"];
                    dr["TotalRcvQty"] = record["TotalRcvQty"];
                    dr["IssuedQty"] = record["MOPickLists_IssuedQty"];
                    dr["QPA"] = record["MOPickLists_QPA"];
                    dr["SpecialIssuedQty"] = record["MOPickLists_SpecialIssuedQty"];

                    dt.Rows.Add(dr);
                    //UFIDA.U9.MFG.MO.DiscreteMOBListUIModel.MORecord item = (UFIDA.U9.MFG.MO.Discretx`eMOBListUIModel.MORecord)record;
                    //GetPLMDocInParaDTOData data = new GetPLMDocInParaDTOData();
                    //data.PartCode = item.
                    //list.Add(data);
                }
                this._part.CurrentSessionState["ResultMo"] = dt;


                //proxy.GetPLMDocInParaDTOs = list;
                //proxy.Do();




                //DataTable dt = new DataTable();
                //if (this._part.CurrentSessionState["MOListFilterAndOrder_ToWholeSetAnalysis"] !=null)
                //{
                //    dt = this._part.CurrentSessionState["MOListFilterAndOrder_ToWholeSetAnalysis"] as DataTable;
                //    this._part.CurrentSessionState["MOListFilterAndOrder_ToWholeSetAnalysis"] = null;
                // }



            }
            catch (Exception ex)
            {
                IUIModel apModel = this._part.Model;
                this._part.Model.ErrorMessage.SetErrorMessage(ref apModel, ex);
            }

        } 
        public void BtnPagPrint_Click(object sender, EventArgs e)
        {
            _part.DataCollect();
           
            IExportSettings settings = ExportServiceFactory.GetInstance().CreateExportSettingsObject();
            settings.PrintTemplateCatalogType = "U9.MO.MO.MO";//U9.Cust.XR.SiteAssessmentDocPrint
            settings.ExportStyle = enumExportStyle.ExcelNew;
            settings.UserDataCallBack = new DataCallBackHandle(this.GetPrintData);

        
          
            UIActionEventArgs ex = new UIActionEventArgs();
            ex.Tag = settings;
            CommandFactory.DoCommand("OnPrint", (BaseAction)_part.Action, sender, ex);

        }

        public void GetPrintData(object sender, DataCallBackEventArgs args)
        {
            IExportSettings settings = ExportServiceFactory.GetInstance().CreateExportSettingsObject();
            settings.PrintTemplateCatalogType = "U9.MO.MO.MO";//U9.Cust.XR.SiteAssessmentDocPrint
            settings.ExportStyle = enumExportStyle.ExcelNewX;
            settings.UserDataCallBack = new DataCallBackHandle(this.GetPrintData);

            string fileName;
            string filePath = string.Empty;
            DataSet returnDataSet = null;
            IReportTemplate report = Common.GetReportTemplateByID("03070b4d-775f-4bb1-a843-4e1be662a036");
            returnDataSet = this.GetOrderDocData();
            fileName = Common.Export(report, GetOrderDocData(), settings, ref filePath);

            //文件重命名
            FileInfo fileInfo = new FileInfo(fileName);
            string dateTime= DateTime.Now.ToString("yyyyMMddHHmmss");
            string newFileName = "MO"+dateTime + fileInfo.Extension;
            fileName = Common.RenameFileName(filePath, fileName, newFileName);
            saveFile(fileName, @"E:\MO");
            //settings.BatchPrint = new BatchPrintHandler(this.GetOrderDocData);
            //switch (args.PrintTemplateID)
            //{
            //    //打印参照模版ID
            //    case "aa7f3383-9cc1-4cc8-aa1c-b21ee15196dd":
            //        {
            //            returnDataSet = this.GetOrderDocData();
            //        }
            //        break;
            //    default:
            //        break;
            //}
            args.ReturnData = returnDataSet;
        }

        public void saveFile(string filePathName, string toFilesPath)
        {
            FileInfo file = new FileInfo(filePathName);
            string newFileName = file.Name;
            file.CopyTo(toFilesPath + @"\" + newFileName, true);
        }

        private DataSet GetOrderDocData()
        {
            _part.Model.ClearErrorMessage();
            _part.DataBinding();
            _part.DataCollect();
            _part.DataBinding();
            DataSet returnDataSet = new DataSet();
            //根据自定义数据源给打印模块对应字符赋值，返回dataset
            List<String> moList = new List<string>();
            bool isDesc = true;//是否降序
            int index = 0;
            long fistrID = 0;
            foreach (IUIRecord record in _part.Model.MO.Cache.GetSelectRecord())
            {
                string mo = record["MainId"].ToString();
                if (index == 0)
                {
                    fistrID = Convert.ToInt64(record["MainId"].ToString());
                }
                else if (index == 1)
                {
                    long secendID = Convert.ToInt64(record["MainId"].ToString());
                    if (secendID > fistrID)
                    {
                        if (secendID > fistrID)
                        {
                            isDesc = false;
                        }
                    }
                }
                index++;
                if (!moList.Contains(mo))
                {
                    moList.Add(mo);
                }
            }
            if (moList.Count == 0)
            {
                //throw new Exception("请先选择数据");
                _part.Model.ErrorMessage.Message = "请先选择数据";
                return null;
            }
            DataSet ds = new DataSet();
            StringBuilder sb = new StringBuilder();
            sb.Append("select ROW_NUMBER() OVER(ORDER BY GETDATE()) as xh ,d1.Name as ProductUOM_Name,a.DemandDate, a.CompleteDate, a.DocNo,a.StartDate,a.RcvUOM,b.Code AS ItemMaster_Code,b.Name AS ItemMaster_Name,b.Version,c1.Name,a.ProductQty,b.SPECS  from MO_MO A	");
            sb.Append("	INNER JOIN dbo.CBO_ItemMaster B ON B.ID=A.ItemMaster ");
            sb.Append("	LEFT JOIN dbo.CBO_Project C ON A.Project=C.ID ");
            //sb.Append("	 ,ISNULL(E.DescFlexField_PrivateDescSeg1,'') AS Location ");
            sb.Append("	LEFT JOIN dbo.CBO_Project_Trl C1 ON C1.ID=C.ID AND C1.SysMLFlag='zh-CN'");
            sb.Append("	LEFT JOIN Base_UOM D ON A.ProductUOM=D.ID");
            sb.Append("	LEFT JOIN Base_UOM_Trl D1 ON D1.ID=D.ID AND D1.SysMLFlag='zh-CN'");
            sb.Append(" where 1=1 ");
            sb.Append(" and  A.ID in (" + String.Join(",", moList.ToArray()) + ")");



            UFSoft.UBF.Util.DataAccess.DataAccessor.RunSQL(UFSoft.UBF.Util.DataAccess.DataAccessor.GetConn(), sb.ToString(), null, out ds);
            if (ds == null || ds.Tables.Count == 0 || ds.Tables[0].Rows.Count == 0)
            {
                _part.Model.ErrorMessage.Message = "为查询到打印的数据";
                return null;
            }
             DataTable dt = new DataTable();
            dt.Columns.Add("Sq");
            dt.Columns.Add("DemandDate");
            dt.Columns.Add("MO_DocNo");
            dt.Columns.Add("MO_MOPickLists_ItemMaster_Code");
            dt.Columns.Add("MO_ItemMaster_Version");
            dt.Columns.Add("MO_MOPickLists_ItemMaster_Name");
            dt.Columns.Add("Project");
            dt.Columns.Add("ProductQty");
            dt.Columns.Add("ProductUOM_Name");
            int number = 0;
            foreach (DataRow dr in ds.Tables[0].Rows)
            {

                DataRow drQty = dt.NewRow();
                number++;
                drQty["Sq"] = dr["xh"];
                drQty["DemandDate"] = dr["DemandDate"];
                drQty["MO_DocNo"] = dr["DocNo"];
                drQty["MO_MOPickLists_ItemMaster_Code"] = dr["ItemMaster_Code"];
                drQty["MO_ItemMaster_Version"] = dr["Version"];
                drQty["MO_MOPickLists_ItemMaster_Name"] = dr["ItemMaster_Name"];
                drQty["Project"] = dr["Name"];
                drQty["ProductQty"] = Decimal.Round(Decimal.Parse(dr["ProductQty"].ToString()), 2).ToString();
                drQty["ProductUOM_Name"] = dr["ProductUOM_Name"];
                dt.Rows.Add(drQty);

            }
            returnDataSet.Tables.Add(dt);
            return returnDataSet;
        }

        //public void GenAttachment(PurchaseOrderRecord record, ref string fileName)
        //{
        //    long docTypeID = record.DocumentType;//单据类型ID
        //    string printTemplateID = string.Empty;//模板ID
        //    //获取单据类型打印模板（打印模板存放在单据类型私有字段1）
        //    StringBuilder oqlBuilder = new StringBuilder();
        //    oqlBuilder.Append("select DescFlexField.PrivateDescSeg1 ");
        //    oqlBuilder.Append(" from UFIDA::U9::PM::Pub::PODocType");
        //    oqlBuilder.Append(" where ID=").Append(docTypeID);
        //    EntityViewQuery query = new EntityViewQuery();
        //    DataSet ds = query.ExecuteDataSet(query.CreateQuery(oqlBuilder.ToString()), null);
        //    if (ds != null && ds.Tables.Count > 0)
        //    {
        //        printTemplateID = ds.Tables[0].Rows[0][0].ToString();
        //    }
        //    if (string.IsNullOrEmpty(printTemplateID))
        //    {
        //        IUIModel model = _part.Model;
        //        _part.Model.ErrorMessage.SetErrorMessage(ref model, "请先在单据类型上预置生成附件的打印模板！");
        //        return;
        //    }
        //    string printCatalog = "U9.SCM.PM.POUI";
        //    ds = GetPrintData();
        //    string filePath = string.Empty;

        //    IExportSettings settings = ExportServiceFactory.GetInstance().CreateExportSettingsObject();
        //    settings.ExportStyle = enumExportStyle.PDF;
        //    settings.PrintTemplateCatalogType = printCatalog;
        //    settings.SetPaperSize(2100, 2970);
        //    settings.SetPagerMargin(50, 0, 50, 0);
        //    IReportTemplate report = Common.GetReportTemplateByID(printTemplateID);
        //    fileName = Common.Export(report, ds, settings, ref filePath);

        //    //文件重命名
        //    FileInfo fileInfo = new FileInfo(fileName);
        //    string newFileName = record.DocNo + fileInfo.Extension;
        //    fileName = Common.RenameFileName(filePath, fileName, newFileName);
        //}
        ///// <summary>
        ///// 打印取数
        ///// </summary>
        ///// <returns></returns>
        //private DataSet GetPrintData()
        //{
        //    PurchaseOrderRecord record = _part.Model.PurchaseOrder.FocusedRecord;
        //    string docNo = record["DocNo"].ToString();

        //    StringBuilder oqlBuilder = new StringBuilder();
        //    oqlBuilder.Append("select UFIDA::U9::PM::PO::PurchaseOrder.DocNo as PurchaseOrder_DocNo,UFIDA::U9::PM::PO::PurchaseOrder.BusinessDate as PurchaseOrder_BusinessDate,UFIDA::U9::PM::PO::PurchaseOrder.PurDept.Name as PurchaseOrder_PurDept_Name,UFIDA::U9::PM::PO::PurchaseOrder.Supplier.Name as PurchaseOrder_Supplier_Name,UFIDA::U9::PM::PO::PurchaseOrder.Supplier.Supplier.SupplierContacts.Contact.Name as PurchaseOrder_Supplier_Supplier_SupplierContacts_Contact_Name,UFIDA::U9::PM::PO::PurchaseOrder.Supplier.Supplier.SupplierContacts.Contact.DefaultPhoneNum as PurchaseOrder_Supplier_Supplier_SupplierContacts_Contact_DefaultPhoneNum,UFIDA::U9::PM::PO::PurchaseOrder.TC.Name as PurchaseOrder_TC_Name,UFIDA::U9::PM::PO::PurchaseOrder.POLines.ItemInfo.ItemName as PurchaseOrder_POLines_ItemInfo_ItemName,UFIDA::U9::PM::PO::PurchaseOrder.POLines.PriceUOM.Name as PurchaseOrder_POLines_PriceUOM_Name,UFIDA::U9::PM::PO::PurchaseOrder.POLines.PurQtyPU as PurchaseOrder_POLines_PurQtyPU,UFIDA::U9::PM::PO::PurchaseOrder.POLines.FinallyPriceTC as PurchaseOrder_POLines_FinallyPriceTC,UFIDA::U9::PM::PO::PurchaseOrder.POLines.TotalMnyTC as PurchaseOrder_POLines_TotalMnyTC,UFIDA::U9::PM::PO::PurchaseOrder.POLines.TotalTaxTC as PurchaseOrder_POLines_TotalTaxTC,UFIDA::U9::PM::PO::PurchaseOrder.Supplier.Supplier.SupplierBankAccount.Name as PurchaseOrder_Supplier_Supplier_SupplierBankAccount_Name,UFIDA::U9::PM::PO::PurchaseOrder.Supplier.Supplier.SupplierBankAccount.Bank.Name as PurchaseOrder_Supplier_Supplier_SupplierBankAccount_Bank_Name,UFIDA::U9::PM::PO::PurchaseOrder.POLines.DocLineNo as PurchaseOrder_POLines_DocLineNo,'' as MyBankAddress,'' as MyBank,'' as MyBankNo,'' as MyContract,'' as MyContractTel,'' as TotalTaxJD,'' as PurQtyPUHJD,'' as TotalMnyHJD,'' as TotalTaxHJD,UFIDA::U9::PM::PO::PurchaseOrder.Supplier.Supplier.OfficialLocation.Address1 as PurchaseOrder_Supplier_Supplier_OfficialLocation_Address1,UFIDA::U9::PM::PO::PurchaseOrder.Supplier.Supplier.OfficialLocation.Address2 as PurchaseOrder_Supplier_Supplier_OfficialLocation_Address2,UFIDA::U9::PM::PO::PurchaseOrder.Supplier.Supplier.OfficialLocation.Address3 as PurchaseOrder_Supplier_Supplier_OfficialLocation_Address3,UFIDA::U9::PM::PO::PurchaseOrder.POLines.ItemInfo.ItemCode as PurchaseOrder_POLines_ItemInfo_ItemCode,UFIDA::U9::PM::PO::PurchaseOrder.PurOper.Code as PurchaseOrder_PurOper_Code,UFIDA::U9::PM::PO::PurchaseOrder.TC.PriceRound.Precision as FinallyPriceJD,UFIDA::U9::PM::PO::PurchaseOrder.TC.MoneyRound.Precision as TotalMnyJD,UFIDA::U9::PM::PO::PurchaseOrder.POLines.PriceUOM.Round.Precision as PurQtyPUJD ");
        //    oqlBuilder.Append(" from UFIDA::U9::PM::PO::PurchaseOrder");
        //    oqlBuilder.Append(" where Org='").Append(PDContext.Current.OrgID).Append("'");
        //    oqlBuilder.Append(" and DocNo='").Append(docNo).Append("'");
        //    oqlBuilder.Append(" order by UFIDA::U9::PM::PO::PurchaseOrder.POLines.DocLineNo");

        //    UFSoft.UBF.Business.EntityViewQuery query = new UFSoft.UBF.Business.EntityViewQuery();
        //    DataSet ds = query.ExecuteDataSet(query.CreateQuery(oqlBuilder.ToString()), null);
        //    return ds;
        //}

    }
}
