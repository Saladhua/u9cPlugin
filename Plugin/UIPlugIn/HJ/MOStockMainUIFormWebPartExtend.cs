using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using UFIDA.U9.Base.SystemDock;
using UFIDA.U9.Base.SystemDock.Proxy;
using UFIDA.U9.MFG.MO.DiscreteMOBListUIModel;
using UFIDA.U9.UI.PDHelper;
using UFSoft.UBF.UI.ControlModel;
using UFSoft.UBF.UI.Custom;
using UFSoft.UBF.UI.Engine.Builder;
using UFSoft.UBF.UI.IView;
using UFSoft.UBF.UI.MD.Runtime;
using UFSoft.UBF.UI.WebControlAdapter;
using UFSoft.UBF.UI.WebControls;
using UFSoft.UBF.Util.DataAccess;

namespace YY.U9.Cust.LI.UIPlugIn
{
    class MOStockMainUIFormWebPartExtend : ExtendedPartBase
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
            BtnSettle = new UFWebMenuAdapter();
            BtnSettle.Text = "配料";
            BtnSettle.ID = "BtnSettle";
            BtnSettle.AutoPostBack = true;
            BtnSettle.ItemClick += new MenuItemHandle(BtnSettle_Click);
            //加入操作里面
            IUFDropDownButton DdbOperation = (IUFDropDownButton)_part.GetUFControlByName(part.TopLevelContainer, "DDBtnOperation");
            DdbOperation.MenuItems.Add(BtnSettle);
        }

        public void BtnSettle_Click(object sender, EventArgs e)
        {
            //收集界面错误信息
            if (this._part.Model.ErrorMessage.hasErrorMessage)
            {
                this._part.Model.ClearErrorMessage();
            }
            this._part.OnDataCollect(this);
            try
            {

                //IList<IUIRecord> selectRecords = _part.Model.MO_MOPickLists.);1498af39-8a9c-4179-9c2f-81716ad8630f
                NavigateManager.NavigatePage(_part, "Cust.BatchingPlan");

                //NavigateManager.ShowModelWebpart(_part, "1498af39-8a9c-4179-9c2f-81716ad8630f", _part.TaskId.ToString(), 992, 504, null, true, true);

                GetPLMDocsProxy proxy = new GetPLMDocsProxy();
                List<GetPLMDocInParaDTOData> list = new List<GetPLMDocInParaDTOData>();
                DataTable dt = new DataTable();
                dt.Columns.Add("MOPickDocNO", typeof(string));//单号
                dt.Columns.Add("ItemInfo_ID", typeof(string));//料号id
                dt.Columns.Add("ItemInfo_Code", typeof(string));//料号code
                dt.Columns.Add("ItemInfo_Name", typeof(string));//料号name

                dt.Columns.Add("Project_ID", typeof(string));//项目ID
                dt.Columns.Add("Project_Name", typeof(string));//项目Name
                dt.Columns.Add("Project_Code", typeof(string));//项目Code

                dt.Columns.Add("BOMReqQty", typeof(string));//BOM需求数量

                string docNo = "";
                foreach (var item in _part.Model.MO.SelectRecords)
                {
                    docNo = item["DocNo"].ToString();

                    DataRow dr = dt.NewRow();
                    dr["MOPickDocNO"] = docNo;
                    ///dr["ItemInfo_ID"] = item["MOPickLists_ItemMaster"];
                    dr["ItemInfo_Code"] = item["MOPickLists_ItemMaster_Code"];
                    dr["ItemInfo_Name"] = item["MOPickLists_ItemMaster_Name"];
                    //dr["Project_ID"] = item["MOPickLists_Projects"];
                    dr["Project_Name"] = item["MOPickLists_Project_Name"];
                    dr["Project_Code"] = item["MOPickLists_Project_Code"];
                    dr["BOMReqQty"] = item["MOPickLists_BOMReqQty"];


                    DataTable dataTable = new DataTable();
                    DataSet dataSet = new DataSet();

                    string sqlForMoDocNoID = "SELECT Project,ItemMaster FROM MO_MOPickList WHERE MO=(SELECT ID FROM MO_MO WHERE DocNo='" + docNo + "')" +
                        "   AND ItemMaster = (SELECT ID FROM CBO_ItemMaster WHERE Code = '" + item["MOPickLists_ItemMaster_Code"].ToString() + "' AND Org = '" + PDContext.Current.OrgID + "')";
                    DataAccessor.RunSQL(DataAccessor.GetConn(), sqlForMoDocNoID, null, out dataSet);
                    dataTable = dataSet.Tables[0];
                    if (dataTable != null && dataTable.Rows.Count > 0)
                    {
                        dr["ItemInfo_ID"] = dataTable.Rows[0]["ItemMaster"].ToString();
                        dr["Project_ID"] = dataTable.Rows[0]["Project"].ToString();
                    }
                    dt.Rows.Add(dr);
                }
                #region 原来的
                #endregion
                this._part.CurrentSessionState["ResultMo"] = dt;
            }
            catch (Exception ex)
            {
                IUIModel apModel = this._part.Model;
                this._part.Model.ErrorMessage.SetErrorMessage(ref apModel, ex);
            }
        }
    }
}
