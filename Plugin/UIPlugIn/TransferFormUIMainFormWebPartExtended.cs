using System;
using System.Collections;
using System.Data;
using UFIDA.U9.SCM.INV.TransferFormUIModel;
using UFSoft.UBF.UI.ControlModel;
using UFSoft.UBF.UI.Controls;
using UFSoft.UBF.UI.Custom;
using UFSoft.UBF.UI.IView;
using UFSoft.UBF.UI.WebControls.Association;
using UFSoft.UBF.UI.WebControls.Association.Adapter;
using UFSoft.UBF.UI.WebControls.ClientCallBack;
using UFSoft.UBF.Util.DataAccess;

namespace YY.U9.Cust.LI.UIPlugIn
{
    // Token: 0x02000006 RID: 6
    internal class TransferFormUIMainFormWebPartExtended : ExtendedPartBase
    {
        // Token: 0x04000004 RID: 4
        private TransferFormUIMainFormWebPart _part;
        // Token: 0x06000018 RID: 24 RVA: 0x00003F6C File Offset: 0x0000216C
        public override void AfterInit(IPart part, EventArgs e)
        {
            base.AfterInit(part, e);
            this._part = (part as TransferFormUIMainFormWebPart);
            VersionChangedCallBack();
            ItemChangedCallBack();


        }

        // Token: 0x06000019 RID: 25 RVA: 0x00003F84 File Offset: 0x00002184
        public override void AfterRender(IPart Part, EventArgs args)
        {
            base.AfterRender(Part, args);
            
        }

        ///<summary>
        /// 注册表格单元格内容改变的回调事件
        ///</summary>
        private void VersionChangedCallBack()
        {
            IUFDataGrid datagrid = (IUFDataGrid)_part.GetUFControlByName(_part.TopLevelContainer, "DataGrid8");

            AssociationControl gridCellDataChangedASC = new AssociationControl();       //基本固定代码
            gridCellDataChangedASC.SourceServerControl = datagrid;
            gridCellDataChangedASC.SourceControl.EventName = "OnCellDataChanged";


            //CallBack处理方案
            //版本
            ((IUFClientAssoGrid)gridCellDataChangedASC.SourceControl).FireEventCols.Add("ItemVersion");
            ((IUFClientAssoGrid)gridCellDataChangedASC.SourceControl).FireEventCols.Add("DescFlexSegments_PrivateDescSeg1");

            ClientCallBackFrm gridCellDataChangedCBF = new ClientCallBackFrm();
            gridCellDataChangedCBF.ParameterControls.Add(datagrid);

            gridCellDataChangedCBF.DoCustomerAction += new ClientCallBackFrm.ActionCustomer(VersionChange);
            gridCellDataChangedCBF.Add(gridCellDataChangedASC);
            //this.Controls.Add(gridCellDataChangedCBF);

        }
        ///<summary>
        /// 表格的CallBack处理方式，返回结果
        ///</summary>
        ///<param name="args"></param>
        ///<returns></returns>
        private object VersionChange(CustomerActionEventArgs args)
        {
            this._part.OnDataCollect(this);//收集数据
            this._part.IsDataBinding = true;
            this._part.IsConsuming = false;
            if (this._part.Model.ErrorMessage.hasErrorMessage)
            {
                this._part.Model.ClearErrorMessage();
            }
            //PR_PRLineListRecord record = this._part.Model.PR_PRLineList.FocusedRecord;
            //string itemVersion = record.ItemVersion_Version;

            IUFDataGrid datagrid = (IUFDataGrid)_part.GetUFControlByName(_part.TopLevelContainer, "DataGrid8");

            UFWebClientGridAdapter grid = new UFWebClientGridAdapter(datagrid);

            //取表格数据（当前行）
            ArrayList list = (ArrayList)args.ArgsHash[UFWebClientGridAdapter.ALL_GRIDDATA_SelectedRows]; //基本固定代码
            int curIndex = int.Parse(list[0].ToString());
            Hashtable table = (Hashtable)((ArrayList)args.ArgsHash[datagrid.ClientID])[curIndex];
            string paperVersion = "";

            string itemVersion = table["ItemInfo_ItemVersion"].ToString(); //版本

            //匹配对应的图纸版本
            //正式的ValueSetDef为 1002207040036824，测试的ValueSetDef为1002206131210010

            DataTable dataTable = new DataTable();
            string version = "select  A.[ID], A.[Code], A1.[Name], A.[DependantCode] from  Base_DefineValue as A  left join [Base_DefineValue_Trl] as A1 on (A1.SysMlFlag = 'zh-CN') and (A.[ID] = A1.[ID])where(A.[ValueSetDef] = 1002207040036824)and a.Code = '" + itemVersion + "'";
            DataSet dataSet = new DataSet();
            DataAccessor.RunSQL(DataAccessor.GetConn(), version, null, out dataSet);
            dataTable = dataSet.Tables[0];

            
            if (dataTable != null && dataTable.Rows.Count > 0)
            {
                paperVersion = dataTable.Rows[0]["Code"].ToString();
            }
             
            //图纸版本
            grid.CellValue.Add(new object[] { curIndex, "DescFlexSegments_PrivateDescSeg1_ID", new string[] { paperVersion, paperVersion, paperVersion } });
            args.ArgsResult.Add(grid.ClientInstanceWithValue);
            return args;
        }

        ///<summary>
        /// 注册表格单元格内容改变的回调事件
        ///</summary>
        private void ItemChangedCallBack()
        {
            IUFDataGrid datagrid = (IUFDataGrid)_part.GetUFControlByName(_part.TopLevelContainer, "DataGrid8");

            AssociationControl gridCellDataChangedASC = new AssociationControl();       //基本固定代码
            gridCellDataChangedASC.SourceServerControl = datagrid;
            gridCellDataChangedASC.SourceControl.EventName = "OnCellDataChanged";


            //CallBack处理方案
            //料号
            ((IUFClientAssoGrid)gridCellDataChangedASC.SourceControl).FireEventCols.Add("ItemInfo_ItemID");
            ((IUFClientAssoGrid)gridCellDataChangedASC.SourceControl).FireEventCols.Add("DescFlexSegments_PrivateDescSeg1");

            ClientCallBackFrm gridCellDataChangedCBF = new ClientCallBackFrm();
            gridCellDataChangedCBF.ParameterControls.Add(datagrid);

            gridCellDataChangedCBF.DoCustomerAction += new ClientCallBackFrm.ActionCustomer(ItemChange);
            gridCellDataChangedCBF.Add(gridCellDataChangedASC);
            //this.Controls.Add(gridCellDataChangedCBF);

        }
        ///<summary>
        /// 表格的CallBack处理方式，返回结果
        ///</summary>
        ///<param name="args"></param>
        ///<returns></returns>
        private object ItemChange(CustomerActionEventArgs args)
        {
            this._part.OnDataCollect(this);//收集数据
            this._part.IsDataBinding = true;
            this._part.IsConsuming = false;
            if (this._part.Model.ErrorMessage.hasErrorMessage)
            {
                this._part.Model.ClearErrorMessage();
            }
            //PR_PRLineListRecord record = this._part.Model.PR_PRLineList.FocusedRecord;
            //string itemVersion = record.ItemVersion_Version;

            IUFDataGrid datagrid = (IUFDataGrid)_part.GetUFControlByName(_part.TopLevelContainer, "DataGrid8");

            UFWebClientGridAdapter grid = new UFWebClientGridAdapter(datagrid);

            //取表格数据（当前行）
            ArrayList list = (ArrayList)args.ArgsHash[UFWebClientGridAdapter.ALL_GRIDDATA_SelectedRows]; //基本固定代码
            int curIndex = int.Parse(list[0].ToString());
            Hashtable table = (Hashtable)((ArrayList)args.ArgsHash[datagrid.ClientID])[curIndex];
            string paperVersion = "";

            string itemCode = table["ItemInfo_ItemID_Code"].ToString();

            //料品私有段3，图纸版本值
            DataTable dt = new DataTable();
            string version_ = "select DescFlexField_PrivateDescSeg3 from CBO_ItemMaster where Code = '" + itemCode + "'";
            DataSet ds = new DataSet();
            DataAccessor.RunSQL(DataAccessor.GetConn(), version_, null, out ds);
            dt = ds.Tables[0];

            if (dt != null && dt.Rows.Count > 0)
            {
                paperVersion = dt.Rows[0]["DescFlexField_PrivateDescSeg3"].ToString();
                //paperVersion = dt.Rows[0]["DescFlexField_PrivateDescSeg3"].ToString();
            }
            //图纸版本
            grid.CellValue.Add(new object[] { curIndex, "DescFlexSegments_PrivateDescSeg1_ID", new string[] { paperVersion, paperVersion, paperVersion } });
            args.ArgsResult.Add(grid.ClientInstanceWithValue);
            return args;
        }


    }
}
