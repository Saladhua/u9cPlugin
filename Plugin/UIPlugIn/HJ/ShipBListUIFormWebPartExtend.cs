using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using UFIDA.U9.SCM.SD.ShipBListUIModel;
using UFIDA.U9.UI.PDHelper;
using UFSoft.UBF.UI.ControlModel;
using UFSoft.UBF.UI.Custom;
using UFSoft.UBF.UI.IView;
using UFSoft.UBF.UI.MD.Runtime;
using UFSoft.UBF.UI.WebControlAdapter;
using UFSoft.UBF.Util.DataAccess;

namespace YY.U9.Cust.LI.UIPlugIn
{
    /// <summary>
    /// 开发文档地址D:\setups\porject\宏巨\2023-5-开发文档-宏巨ERP项目U9C-标准出货列表库存量客开 方案1.0
    /// </summary>
    class ShipBListUIFormWebPartExtend : ExtendedPartBase
    {
        private ShipBListUIFormWebPart _part;

        IUFButton BtnSettle;

        public override void AfterInit(IPart part, EventArgs args)
        {
            base.AfterInit(part, args);
            _part = part as ShipBListUIFormWebPart;
            //BtnSettle = new UFWebMenuAdapter();
            //BtnSettle.Text = "可用量查询";
            //BtnSettle.ID = "BtnSettle";
            //BtnSettle.AutoPostBack = true;
            //BtnSettle.ItemClick += new MenuItemHandle(BtnSettle_Click);
            ////加入操作里面
            //IUFDropDownButton DdbOperation = (IUFDropDownButton)_part.GetUFControlByName(part.TopLevelContainer, "DDBtnAferDeal");
            //DdbOperation.MenuItems.Add(BtnSettle);


            //IUFButton BtnSettle = new UFWebButtonAdapter();
            //IUFToolbar toolbar = (IUFToolbar)_part.GetUFControlByName(_part.TopLevelContainer, "Toolbar1");
            //if (toolbar != null)
            //{
            //    string guid = "5c0ad4e7-fdc0-412a-a0c6-681fe595964f";
            //    BtnSettle = UIControlBuilder.BuilderToolbarButton(toolbar, "True", "BtnSettle", "True", "True", 70, 28, "7", "", true, false, guid, guid, guid);
            //    UIControlBuilder.SetButtonAccessKey(BtnSettle);
            //    BtnSettle.Text = "可用量查询";
            //    BtnSettle.ID = "BtnSettle";
            //    BtnSettle.AutoPostBack = true;
            //    BtnSettle.UIModel = _part.Model.ElementID;
            //    ((UFWebToolbarAdapter)toolbar).Items.Add(BtnSettle as System.Web.UI.WebControls.WebControl);
            //    BtnSettle.Click += BtnSettle_Click;
            //}

            //实例化按钮
            BtnSettle = new UFWebButtonAdapter();
            new UFWebButtonAdapter();
            BtnSettle.Text = "可用量查询";
            BtnSettle.ID = "BtnSettle";
            BtnSettle.AutoPostBack = true;
            BtnSettle.Click += new EventHandler(BtnSettle_Click);
            //加入Card容器
            IUFCard card = (IUFCard)_part.GetUFControlByName(part.TopLevelContainer, "Card0");
            card.Controls.Add(BtnSettle);
            CommonFunction.Layout(card, BtnSettle, 11, 0);
        }

        [Obsolete]
        public void BtnSettle_Click(object sender, EventArgs e)
        {
            //收集界面错误信息
            if (this._part.Model.ErrorMessage.hasErrorMessage)
            {
                this._part.Model.ClearErrorMessage();
            }
            this._part.OnDataCollect(this);

            //Cache.GetSelectRecord();
            // List<IUIRecord> list = UIRuntimeHelper.Instance.GetSelectRecordFromCache(_part.Model.Ship.Cache.GetSelectRecord()).ToList);

            string ItemCode = "";//料品
            string LotCode = "";//批号
            string DocNo = "";//单号
            try
            {
                _part.DataCollect();//数据收集
                #region SQL语句抓取
                //SELECT ItemInfo_ItemID, LotInfo_LotCode,
                //(SELECT  TOP(1)(StoreQty - ResvStQty - ResvOccupyStQty)  AS KC FROM InvTrans_WhQoh
                //WHERE  ItemInfo_ItemID = ItemInfo_ItemID AND LotInfo_LotCode = LotInfo_LotCode order by CreatedOn desc) AS CPKYL,
                //(SELECT  TOP(1)(StoreQty - ResvStQty - ResvOccupyStQty)  AS KC FROM InvTrans_WhQoh
                //WHERE  ItemInfo_ItemID = ItemInfo_ItemID AND LotInfo_LotCode = LotInfo_LotCode order by CreatedOn desc) AS BLKYL,
                //(SELECT  TOP(1)(StoreQty - ResvStQty - ResvOccupyStQty)  AS KC FROM InvTrans_WhQoh
                //WHERE  ItemInfo_ItemID = ItemInfo_ItemID AND LotInfo_LotCode = LotInfo_LotCode order by CreatedOn desc) AS FPKYL
                //FROM SM_ShipLine
                //where Org = '1002208260110060'
                #endregion
                List<ShipItem> shipItemsFroShipLines = new List<ShipItem>();
                List<ShipItem> shipItems = new List<ShipItem>();
                DataTable dataTable = new DataTable();
                DataSet dataSet = new DataSet();
                #region MyRegion
                string sqlForShipLine = "SELECT ItemInfo_ItemCode," +
                "  LotInfo_LotMaster,(SELECT  sum((StoreQty - ResvStQty - ResvOccupyStQty))  AS KC FROM InvTrans_WhQoh" +
                " WHERE  ItemInfo_ItemID = SM_ShipLine.ItemInfo_ItemID  AND Wh=(SELECT ID FROM CBO_Wh WHERE Code='02' AND Org='" + PDContext.Current.OrgID + "') ) AS CPKYL," +
                " (SELECT  sum((StoreQty - ResvStQty - ResvOccupyStQty))  AS KC FROM InvTrans_WhQoh" +
                " WHERE  ItemInfo_ItemID = SM_ShipLine.ItemInfo_ItemID  AND Wh=(SELECT ID FROM CBO_Wh WHERE Code='0202' AND Org='" + PDContext.Current.OrgID + "') ) AS BLKYL," +
                " (SELECT  sum((StoreQty - ResvStQty - ResvOccupyStQty))  AS KC FROM InvTrans_WhQoh" +
                " WHERE  ItemInfo_ItemID = SM_ShipLine.ItemInfo_ItemID  AND Wh=(SELECT ID FROM CBO_Wh WHERE Code='0201' AND Org='" + PDContext.Current.OrgID + "') ) AS FPKYL, " +
                " (SELECT  sum((StoreQty - ResvStQty - ResvOccupyStQty))  AS KC FROM InvTrans_WhQoh" +
                " WHERE  ItemInfo_ItemID = SM_ShipLine.ItemInfo_ItemID  AND Wh=(SELECT ID FROM CBO_Wh WHERE Code='12' AND Org='" + PDContext.Current.OrgID + "') ) AS YPYL " +
                " FROM SM_ShipLine where Org = '" + PDContext.Current.OrgID + "'";
                DataAccessor.RunSQL(DataAccessor.GetConn(), sqlForShipLine, null, out dataSet);
                dataTable = dataSet.Tables[0];
                if (dataTable != null && dataTable.Rows.Count > 0)
                {
                    for (int i = 0; i < dataTable.Rows.Count; i++)
                    {
                        ShipItem shipItem = new ShipItem();
                        shipItem.ItemCode = dataTable.Rows[i]["ItemInfo_ItemCode"].ToString();
                        shipItem.LotCode = dataTable.Rows[i]["LotInfo_LotMaster"].ToString();
                        shipItem.CPKYL = dataTable.Rows[i]["CPKYL"] == null ? "0" : dataTable.Rows[i]["CPKYL"].ToString();
                        shipItem.BLKYL = dataTable.Rows[i]["BLKYL"] == null ? "0" : dataTable.Rows[i]["BLKYL"].ToString();
                        shipItem.FPKYL = dataTable.Rows[i]["FPKYL"] == null ? "0" : dataTable.Rows[i]["FPKYL"].ToString();
                        shipItem.YPYL = dataTable.Rows[i]["YPYL"] == null ? "0" : dataTable.Rows[i]["YPYL"].ToString();
                        shipItems.Add(shipItem);
                    }
                }
                #endregion
                #region 赋值
                foreach (var item in this._part.Model.Ship.SelectRecords)
                {
                    ShipItem shipItemsFroShipLine = new ShipItem();
                    shipItemsFroShipLine.ItemCode = item["ShipLines_ItemInfo_ItemCode"].ToString();
                    shipItemsFroShipLine.LotCode = item["ShipLines_LotInfo_LotMaster"] == null ? "" : item["ShipLines_LotInfo_LotMaster"].ToString();
                    shipItemsFroShipLine.DocNo = item["DocNo"].ToString();
                    shipItemsFroShipLines.Add(shipItemsFroShipLine);
                }
                #endregion

                #region 更新实例
                //SELECT Org,DocLineNO,Ship,* FROM SM_ShipLine WHERE ItemInfo_ItemCode='5100102-00184' AND LotInfo_LotCode='20220831001' 
                //AND Ship = (select ID FROM SM_Ship WHERE DocNo = '10SM2211150054')
                //UPDATE SM_ShipLine SET DescFlexField_PrivateDescSeg12 = '1', DescFlexField_PrivateDescSeg13 = '2', DescFlexField_PrivateDescSeg14 = '3'
                //WHERE ItemInfo_ItemCode = '5100102-00184' AND LotInfo_LotCode = '20220831001'
                //AND Ship = (select ID FROM SM_Ship WHERE DocNo = '10SM2211150054')

                //exec P_SyncFieldCombineName @FullName = 'UFIDA.U9.SM.Ship.ShipLine',@DescFieldName = 'DescFlexField_PrivateDescSeg12'
                //exec P_SyncFieldCombineName @FullName = 'UFIDA.U9.SM.Ship.ShipLine',@DescFieldName = 'DescFlexField_PrivateDescSeg13'
                //exec P_SyncFieldCombineName @FullName = 'UFIDA.U9.SM.Ship.ShipLine',@DescFieldName = 'DescFlexField_PrivateDescSeg14'
                #endregion
                //shipItemsFroShipLines
                //页面上传过来的值饱含单号，料品，料号
                //
                //私有字段12-库存可用量（成品），私有字段13-不良可用量,私有字段14-复判可用量

                foreach (var item in shipItemsFroShipLines)
                {
                    //new 一个实例               
                    string d12 = "";
                    string d13 = "";
                    string d14 = "";
                    string d19 = "";
                    if (string.IsNullOrEmpty(item.LotCode))
                    {
                        var bd12 = shipItems.Where(x => x.ItemCode == item.ItemCode).Select(x => x.BLKYL).Distinct().ToList();
                        d12 = bd12[0].ToString();
                        var bd13 = shipItems.Where(x => x.ItemCode == item.ItemCode).Select(x => x.CPKYL).Distinct().ToList();
                        d13 = bd13[0].ToString();
                        var bd14 = shipItems.Where(x => x.ItemCode == item.ItemCode).Select(x => x.FPKYL).Distinct().ToList();
                        d14 = bd14[0].ToString();
                        var bd19 = shipItems.Where(x => x.ItemCode == item.ItemCode).Select(x => x.YPYL).Distinct().ToList();
                        d19 = bd19[0].ToString();
                    }
                    else
                    {
                        var bd12 = shipItems.Where(x => x.ItemCode == item.ItemCode && x.LotCode == item.LotCode).Select(x => x.BLKYL).Distinct().ToList();
                        d12 = bd12[0].ToString();
                        var bd13 = shipItems.Where(x => x.ItemCode == item.ItemCode && x.LotCode == item.LotCode).Select(x => x.CPKYL).Distinct().ToList();
                        d13 = bd13[0].ToString();
                        var bd14 = shipItems.Where(x => x.ItemCode == item.ItemCode && x.LotCode == item.LotCode).Select(x => x.FPKYL).Distinct().ToList();
                        d14 = bd14[0].ToString();
                        var bd19 = shipItems.Where(x => x.ItemCode == item.ItemCode && x.LotCode == item.LotCode).Select(x => x.YPYL).Distinct().ToList();
                        d19 = bd19[0].ToString();
                    }

                    #region 原始数据
                    //var bd12 = shipItems.Where(x => x.ItemCode == item.ItemCode && x.LotCode == item.LotCode).Select(x => x.BLKYL).Distinct().ToList();
                    //string d12 = bd12[0].ToString();
                    //var bd13 = shipItems.Where(x => x.ItemCode == item.ItemCode && x.LotCode == item.LotCode).Select(x => x.CPKYL).Distinct().ToList();
                    //string d13 = bd13[0].ToString();
                    //var bd14 = shipItems.Where(x => x.ItemCode == item.ItemCode && x.LotCode == item.LotCode).Select(x => x.FPKYL).Distinct().ToList();
                    //string d14 = bd14[0].ToString();
                    #endregion

                    string updated = "";
                    if (!string.IsNullOrEmpty(item.LotCode))
                    {
                        updated = "UPDATE SM_ShipLine" +
                            " SET DescFlexField_PrivateDescSeg12 = '" + d13 + "', " +
                            " DescFlexField_PrivateDescSeg13 = '" + d12 + "'," +
                            " DescFlexField_PrivateDescSeg19 = '" + d19 + "'," +
                            " DescFlexField_PrivateDescSeg14 = '" + d14 + "'" +
                            " WHERE ItemInfo_ItemCode = '" + item.ItemCode + "' AND LotInfo_LotMaster = '" + item.LotCode + "' " +
                            " AND Ship = (select ID FROM SM_Ship WHERE DocNo = '" + item.DocNo + "')";
                    }
                    else
                    {
                        updated = "UPDATE SM_ShipLine SET DescFlexField_PrivateDescSeg12 = '" + d13 + "'," +
                            " DescFlexField_PrivateDescSeg13 = '" + d12 + "'," +
                            " DescFlexField_PrivateDescSeg19 = '" + d19 + "'," +
                            " DescFlexField_PrivateDescSeg14 = '" + d14 + "'" +
                            " WHERE ItemInfo_ItemCode = '" + item.ItemCode + "'" +
                            " AND Ship = (select ID FROM SM_Ship WHERE DocNo = '" + item.DocNo + "')";
                    }
                    DataAccessor.RunSQL(DataAccessor.GetConn(), updated, null, out dataSet);
                }
                #region 执行多语表
                string updated_P_d12 = "exec P_SyncFieldCombineName @FullName = 'UFIDA.U9.SM.Ship.ShipLine',@DescFieldName = 'DescFlexField_PrivateDescSeg12'";
                DataAccessor.RunSQL(DataAccessor.GetConn(), updated_P_d12, null, out dataSet);
                string updated_P_d13 = "exec P_SyncFieldCombineName @FullName = 'UFIDA.U9.SM.Ship.ShipLine',@DescFieldName = 'DescFlexField_PrivateDescSeg13'";
                DataAccessor.RunSQL(DataAccessor.GetConn(), updated_P_d13, null, out dataSet);
                string updated_P_d14 = "exec P_SyncFieldCombineName @FullName = 'UFIDA.U9.SM.Ship.ShipLine',@DescFieldName = 'DescFlexField_PrivateDescSeg14'";
                DataAccessor.RunSQL(DataAccessor.GetConn(), updated_P_d14, null, out dataSet);
                string updated_P_d19 = "exec P_SyncFieldCombineName @FullName = 'UFIDA.U9.SM.Ship.ShipLine',@DescFieldName = 'DescFlexField_PrivateDescSeg19'";
                DataAccessor.RunSQL(DataAccessor.GetConn(), updated_P_d19, null, out dataSet);
                #endregion

                this._part.Action.CurrentPart.ShowWindowStatus("成功");

                IUFDataGrid uiGrid = this._part.GetUFControlByName(this._part.TopLevelContainer, "DataGrid1") as IUFDataGrid;
                this._part.Action.NavigateAction.Refresh(uiGrid);

            }
            catch (Exception ex)
            {
                IUIModel apModel = this._part.Model;
                this._part.Model.ErrorMessage.SetErrorMessage(ref apModel, ex);
            }
        }

        /// <summary>
        /// Dto
        /// </summary>
        public class ShipItem
        {
            public string ItemCode { get; set; }

            public string LotCode { get; set; }

            public string DocNo { get; set; }
            public string CPKYL { get; set; }
            public string BLKYL { get; set; }
            public string FPKYL { get; set; }
            public string YPYL { get; set; }
        }
    }
}
