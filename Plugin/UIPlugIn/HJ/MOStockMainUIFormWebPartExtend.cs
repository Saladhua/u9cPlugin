using System;
using System.Collections.Generic;
using System.Data;
using UFIDA.U9.Base.SystemDock;
using UFIDA.U9.Base.SystemDock.Proxy;
using UFIDA.U9.MFG.MO.DiscreteMOUIModel;
using UFIDA.U9.UI.PDHelper;
using UFSoft.UBF.UI.ControlModel;
using UFSoft.UBF.UI.Custom;
using UFSoft.UBF.UI.IView;
using UFSoft.UBF.UI.MD.Runtime;
using UFSoft.UBF.UI.WebControlAdapter;
using UFSoft.UBF.UI.WebControls;

namespace YY.U9.Cust.LI.UIPlugIn
{
    class MOStockMainUIFormWebPartExtend : ExtendedPartBase
    {
        private MOStockMainUIFormWebPart _part;
        IUFMenu BtnSettle;
        /// <summary>
        /// 初始化后扩展
        /// </summary>
        /// <param name="part"></param>
        /// <param name="e"></param>
        public override void AfterInit(IPart part, System.EventArgs e)
        {
            base.AfterInit(part, e);
            _part = part as MOStockMainUIFormWebPart;
            BtnSettle = new UFWebMenuAdapter();
            BtnSettle.Text = "配料";
            BtnSettle.ID = "BtnSettle";
            BtnSettle.AutoPostBack = true;
            BtnSettle.ItemClick += new MenuItemHandle(BtnSettle_Click);
            //加入操作里面
            IUFDropDownButton DdbOperation = (IUFDropDownButton)_part.GetUFControlByName(part.TopLevelContainer, "DDBtnActivity");
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
                //IList<IUIRecord> selectRecords = _part.Model.MO_MOPickLists.);
                NavigateManager.ShowModelWebpart(_part, "1498af39-8a9c-4179-9c2f-81716ad8630f", _part.TaskId.ToString(), 992, 504, null, true, true);
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
                foreach (var item in _part.Model.MO.Records)
                {
                    docNo = item["DocNo"].ToString();
                }

                foreach (IUIRecord record in _part.Model.MO_MOPickLists.SelectRecords)
                {
                    DataRow dr = dt.NewRow();
                    dr["MOPickDocNO"] = docNo;
                    dr["ItemInfo_ID"] = record["ItemMaster"];
                    dr["ItemInfo_Code"] = record["ItemMaster_Code"];
                    dr["ItemInfo_Name"] = record["ItemMaster_Name"];

                    dr["Project_ID"] = record["Project"];
                    dr["Project_Name"] = record["Project_Name"];
                    dr["Project_Code"] = record["Project_Code"]; 

                    dr["BOMReqQty"] = record["BOMReqQty"];   

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
    }
}
