using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using UFIDA.U9.CBO.Pub.Controller;
using UFIDA.U9.CBO.SCM.Item;
using UFIDA.U9.MFG.MO.StartAnalysisUIModel;
using UFIDA.U9.UI.PDHelper;
using UFSoft.UBF.PL.Engine;
using UFSoft.UBF.UI.ControlModel;
using UFSoft.UBF.UI.Custom;
using UFSoft.UBF.UI.IView;
using UFSoft.UBF.UI.WebControlAdapter;
using UFSoft.UBF.UI.WebControls;
using UFSoft.UBF.Util.DataAccess;

namespace YY.U9.Cust.LI.UIPlugIn
{
    class ItemWIPSimuBListUIFormWebPartExtended : ExtendedPartBase
    {
        private ManufactureSimuResultUIFormWebPart _part;

        IUFMenu BtnSettle;


        public override void AfterInit(IPart part, System.EventArgs e)
        {
            base.AfterInit(part, e);
            _part = part as ManufactureSimuResultUIFormWebPart;
            BtnSettle = new UFWebMenuAdapter();
            BtnSettle.Text = "生成调入单";
            BtnSettle.ID = "BtnSettle";
            BtnSettle.AutoPostBack = true;
            BtnSettle.ItemClick += new MenuItemHandle(BtnSettle_Click);
            //加入操作里面
            IUFDropDownButton DdbOperation = (IUFDropDownButton)_part.GetUFControlByName(part.TopLevelContainer, "DDBQuery");
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
            DataTable dataTable = new DataTable();
            DataSet dataSet = new DataSet();
            List<MoItem> Mmos = new List<MoItem>();

            #region 原始数据
            //        string sqlForMoDocNoID = "SELECT a.ItemMaster,a.ActualReqQty,a.SupplyWh," +
            //" (SELECT Name FROM CBO_Wh_Trl WHERE ID = a.SupplyWh) AS SupplyWhCode," +
            //" (SELECT  InventorySecondUOM FROM CBO_ItemMaster WHERE ID=a.ItemMaster  AND Org = '" + PDContext.Current.OrgID + "' ) AS StoreUOM," +
            //" (SELECT  (SELECT Round_Precision FROM Base_UOM WHERE ID=CBO_ItemMaster.InventorySecondUOM) AS StoreUOM FROM CBO_ItemMaster WHERE ID=a.ItemMaster AND Org = '" + PDContext.Current.OrgID + "' ) AS Round_Precision," +
            //" (a.ActualReqQty-a.IssuedQty) AS IssuedQty," +
            //" (SELECT DescFlexField_PrivateDescSeg15 FROM CBO_ItemMaster WHERE ID = a.ItemMaster) AS PrivateDescSeg15," +
            //" (SELECT  sum((StoreQty - ResvStQty - ResvOccupyStQty))  AS 库存可用量 FROM InvTrans_WhQoh WHERE Wh = a.SupplyWh " +
            //" and ItemInfo_ItemID = a.ItemMaster AND  Org = '" + PDContext.Current.OrgID + "') AS KCKYL," +
            //" (SELECT CostUOM FROM CBO_ItemMaster WHERE ID=a.ItemMaster) AS CostUOM" +
            //" FROM MO_MOPickList a INNER JOIN MO_MO b ON a.MO = b.ID WHERE b.DocNo = '" + docno + "'" +
            //" AND b.ItemMaster = (SELECT ID FROM CBO_ItemMaster WHERE Code = '" + itemcode + "' AND Org = '" + PDContext.Current.OrgID + "')";
            #endregion

            int sererer = _part.Model.ItemWIPSimu.Cache.Count;

            foreach (var item in _part.Model.ItemWIPSimu.SelectRecords)
            {
                //IssuedQty--已发放数量
                string docno = item["MO_DocNo"].ToString();
                string itemcode = item["ItemMaster_Code"].ToString();
                string SetableStatus = item["SetableStatus"].ToString();
                string sqlForMoDocNoID = "select" +
                    " b.ItemMaster," +
                    " b.SupplyWh," +
                    " a.DescFlexField_PrivateDescSeg2," +
                    " (select DescFlexField_PrivateDescSeg15 from CBO_ItemMaster where ID = b.ItemMaster) as PrivateDescSeg15" +
                    " from MO_MO a inner" +
                    " join MO_MOPickList b" +
                    " on a.ID = b.MO" +
                    " where a.DocNo = '" + docno + "'" +
                    " and a.ItemMaster =" +
                    " (select ID from CBO_ItemMaster where Code = '" + itemcode + "' and Org='" + PDContext.Current.OrgID + "')" +
                    " and a.DescFlexField_PrivateDescSeg2 =''";
                DataAccessor.RunSQL(DataAccessor.GetConn(), sqlForMoDocNoID, null, out dataSet);
                dataTable = dataSet.Tables[0];
                if (dataTable != null && dataTable.Rows.Count > 0)
                {
                    for (int i = 0; i < dataTable.Rows.Count; i++)
                    {
                        string p15 = dataTable.Rows[i]["PrivateDescSeg15"].ToString();

                        if (p15 == "1")
                        {
                            MoItem moItem = new MoItem();
                            moItem.MoID = docno;
                            moItem.ItemMasterCode = long.Parse(dataTable.Rows[i]["ItemMaster"].ToString());
                            try
                            {
                                moItem.CompleteWhCode = long.Parse(dataTable.Rows[i]["SupplyWh"].ToString());
                            }
                            catch (Exception ex)
                            {
                                string mes = ex.Message;
                                //this._part.Model.ErrorMessage.Message = "单号" + docno + "下备料" + itemcode + "供应地点为空";
                                continue;
                            }
                            moItem.PrivateDescSeg15 = dataTable.Rows[i]["PrivateDescSeg15"].ToString();//dataTable.Rows[i]["PrivateDescSeg15"].ToString();//"1"
                            moItem.DescFlexField_PrivateDescSeg2 = dataTable.Rows[i]["DescFlexField_PrivateDescSeg2"].ToString();
                            moItem.SetableStatus = SetableStatus;
                            Mmos.Add(moItem);
                        }
                    }
                }
                //string sqlupdate = "UPDATE MO_MO SET DescFlexField_PrivateDescSeg2 = '" + DateTime.Now.ToString("F") + "' WHERE DocNo = '" + docno + "'";
                //DataAccessor.RunSQL(DataAccessor.GetConn(), sqlupdate, null, out dataSet);
            }

            int b = Mmos.Select(x => x.CompleteWhCode).Distinct().Count();

            var whcount = Mmos.Select(x => x.CompleteWhCode).Distinct().ToArray();

            foreach (var itemc in whcount)
            {

                if (itemc.ToString() == "1002302240000602" || itemc.ToString() == "1002302240000591")
                {
                    List<MoItem> mos = Mmos.Where(x => x.CompleteWhCode == long.Parse(itemc.ToString())).ToList();
                    #region 调入单
                    UFIDA.U9.ISV.TransferInISV.Proxy.CommonCreateTransferInSVProxy transferInSVProxy = new UFIDA.U9.ISV.TransferInISV.Proxy.CommonCreateTransferInSVProxy();

                    //UFIDA.U9.ISV.TransferInISV.IC_TransferInDTOData[] Boms;
                    List<UFIDA.U9.ISV.TransferInISV.IC_TransferInDTOData> listBom = new List<UFIDA.U9.ISV.TransferInISV.IC_TransferInDTOData>();
                    List<UFIDA.U9.ISV.TransferInISV.IC_TransInLineDTOData> listBomLine = new List<UFIDA.U9.ISV.TransferInISV.IC_TransInLineDTOData>();

                    //头
                    UFIDA.U9.ISV.TransferInISV.IC_TransferInDTOData Bom = new UFIDA.U9.ISV.TransferInISV.IC_TransferInDTOData();
                    Bom.TransInDocType = new UFIDA.U9.CBO.Pub.Controller.CommonArchiveDataDTOData();
                    Bom.TransInDocType.Code = "TransIn010";//单据类型
                    Bom.TransferType = 0;//调入类型 0为一步式 1为两步式
                    Bom.Org = new UFIDA.U9.CBO.Pub.Controller.CommonArchiveDataDTOData();
                    Bom.Org.ID = long.Parse(PDContext.Current.OrgID);
                    Bom.BusinessDate = new DateTime();
                    //Bom.Memo=//备注
                    Bom.TransInLines = new List<UFIDA.U9.ISV.TransferInISV.IC_TransInLineDTOData>();
                    Bom.SysState = ObjectState.Inserted;

                    //做一次数据筛选
                    List<MoItem> nmos = ForLoopRemove(mos);

                    //设置全局变量Q
                    //decimal quanjuQ = 0;

                    List<MoItem> nnmos = new List<MoItem>();

                    foreach (var item in nmos)
                    {
                        MoItem moItem = new MoItem();
                        string ssee = item.ItemMasterCode.ToString();
                        string ssee2 = item.CompleteWhCode.ToString();
                        string docno = item.MoID.ToString();//生产订单的单号
                        moItem.MoID = item.MoID;//生产订单的单号
                        moItem.ActualReqQty = decimal.Parse(getActualReqQty(item.ItemMasterCode, ssee2, docno));
                        moItem.ItemMasterCode = item.ItemMasterCode;
                        nnmos.Add(moItem);
                    }
                    //行
                    foreach (var item in nmos)
                    {
                        string ssee = item.ItemMasterCode.ToString();

                        string ssee2 = item.CompleteWhCode.ToString();

                        string docno = item.MoID.ToString();//生产订单的单号 

                        //decimal iqty = decimal.Parse(getActualReqQty(item.ItemMasterCode, ssee2, docno));

                        //decimal DrQty = 0;

                        decimal iqty = 0;

                        iqty = nnmos.Where(x => x.ItemMasterCode == item.ItemMasterCode).Sum(x => x.ActualReqQty);

                        string kuc = "0";

                        string kucy = "0";

                        if (!string.IsNullOrEmpty(item.ItemMasterCode.ToString()) && !string.IsNullOrEmpty(item.CompleteWhCode.ToString()))
                        {
                            //kuc = getkc(item.ItemMasterCode.ToString(), item.CompleteWhCode.ToString());
                            //kucy = getkc(item.ItemMasterCode.ToString(), "1002302100001184");
                        }

                        //kuc
                        decimal see = decimal.Parse(kuc);

                        // item.IssuedQty = iqty;

                        decimal DrQty = iqty - decimal.Parse(kuc);

                        #region 原来的
                        //if (decimal.Parse(item.KCKYL) != item.IssuedQty)//生产订单备料对应料品的总数和库存可用量不等
                        //{
                        //    DrQty = item.IssuedQty - decimal.Parse(item.KCKYL);
                        //}
                        //else
                        //{
                        //    DrQty = Math.Round(item.IssuedQty, int.Parse(item.Round_Precision));
                        //    //DrQty = decimal.Parse(item.IssuedQty);
                        //}
                        //quanjuQ = DrQty + quanjuQ;
                        //if (string.IsNullOrEmpty(item.DescFlexField_PrivateDescSeg2) && quanjuQ <= DrQty)
                        //{
                        //    string see = "";
                        //}
                        //if (item.PrivateDescSeg15 == "1" && DrQty > 0 && item.SetableStatus == "2" && DQTY > 0 && quanjuQ <= DrQty && string.IsNullOrEmpty(item.DescFlexField_PrivateDescSeg2))

                        #endregion

                        if (item.PrivateDescSeg15 == "1" && item.SetableStatus == "2")
                        {
                            UFIDA.U9.ISV.TransferInISV.IC_TransInLineDTOData Bom_line = new UFIDA.U9.ISV.TransferInISV.IC_TransInLineDTOData();
                            Bom_line.TransInOwnerOrg = new UFIDA.U9.CBO.Pub.Controller.CommonArchiveDataDTOData();
                            Bom_line.TransInOwnerOrg.ID = long.Parse(PDContext.Current.OrgID);
                            Bom_line.TransInWh = new UFIDA.U9.CBO.Pub.Controller.CommonArchiveDataDTOData();
                            Bom_line.TransInWh.ID = item.CompleteWhCode;//调入存储地点
                            Bom_line.StoreUOMQty = iqty;//调入数量
                            Bom_line.CostUOMQty = iqty;//成本数量l 
                            Bom_line.CostUOM = new CommonArchiveDataDTOData();

                            Bom_line.CostUOM.ID = item.CostUOM;

                            Bom_line.StoreUOM = new CommonArchiveDataDTOData();

                            //Bom_line.StoreUOM.ID = long.Parse(item.StoreUOM);

                            Bom_line.ItemInfo = new ItemInfoData();

                            Bom_line.ItemInfo.ItemID = item.ItemMasterCode;

                            Bom_line.SysState = ObjectState.Inserted;

                            Bom_line.DescFlexSegments = new UFIDA.U9.Base.FlexField.DescFlexField.DescFlexSegmentsData();
                            Bom_line.DescFlexSegments.PrivateDescSeg1 = item.MoID;

                            Bom_line.TransInSubLines = new List<UFIDA.U9.ISV.TransferInISV.IC_TransInSubLineDTOData>();
                            //子行 
                            List<UFIDA.U9.ISV.TransferInISV.IC_TransInSubLineDTOData> listBomSubline = new List<UFIDA.U9.ISV.TransferInISV.IC_TransInSubLineDTOData>();
                            UFIDA.U9.ISV.TransferInISV.IC_TransInSubLineDTOData Bom_subLine = new UFIDA.U9.ISV.TransferInISV.IC_TransInSubLineDTOData();

                            Bom_subLine.TransOutOrg = new UFIDA.U9.CBO.Pub.Controller.CommonArchiveDataDTOData();

                            Bom_subLine.TransOutWh = new UFIDA.U9.CBO.Pub.Controller.CommonArchiveDataDTOData();
                            Bom_subLine.TransOutWh.Code = "01";

                            Bom_subLine.StoreUOMQty = iqty;

                            Bom_subLine.StoreUOM = new CommonArchiveDataDTOData();

                            //Bom_subLine.StoreUOM.ID = long.Parse(item.StoreUOM);

                            kucy = getkc(item.ItemMasterCode.ToString(), "1002302100001184");

                            listBomSubline.Add(Bom_subLine);//加载子行

                            Bom_line.TransInSubLines = listBomSubline;

                            if (decimal.Parse(kucy) > 0 && iqty > 0)
                            {
                                listBomLine.Add(Bom_line);//加载行  
                            }
                        }
                    }
                    Bom.TransInLines = listBomLine;
                    listBom.Add(Bom);
                    transferInSVProxy.TransferInDTOList = listBom;
                    if (Bom.TransInLines.Count == 0)//单据行上面一个都没有
                    {
                        //this._part.Model.ErrorMessage.Message = "检查生产订单是否被使用，或者料品是否能被调用，或者是否齐套状态为齐";
                        continue;
                    }
                    string seee11 = "";
                    try
                    {
                        List<CommonArchiveDataDTOData> see = transferInSVProxy.Do();

                        seee11 = DateTime.Now.ToString("F") + see[0].Code.ToString();

                        this._part.Action.CurrentPart.ShowWindowStatus("成功,调入单号：" + seee11);
                        #endregion
                        foreach (var item in mos)
                        {
                            string sqlupdate = "UPDATE MO_MO SET DescFlexField_PrivateDescSeg2 = '" + seee11 + "' WHERE DocNo = '" + item.MoID + "'";
                            DataAccessor.RunSQL(DataAccessor.GetConn(), sqlupdate, null, out dataSet);
                        }
                    }
                    catch (Exception ex)
                    {
                        this._part.Model.ErrorMessage.Message = "检查生产订单是否被使用，或者料品是否能被调用，或者是否齐套状态为齐" + ex.Message.ToString() + "调入接口报错";
                        return;
                    }
                }
            }
        }

        public static List<MoItem> ForLoopRemove(List<MoItem> items)
        {
            List<MoItem> output = new List<MoItem>();
            for (int i = 0; i < items.Count; i++)
            {
                bool flag = false;
                //每个元素都与其他这个元素前面的比较，如果前面没有，则添加，否则不添加
                for (int z = 0; z < i; z++)
                {
                    if (items[z].ItemMasterCode == items[i].ItemMasterCode)
                    {
                        flag = true;
                        break;
                    }
                }
                if (!flag)
                {
                    output.Add(items[i]);
                }
            }
            return output;
        }

        public class MoItem
        {
            public string MoID { get; set; }
            public long ItemMasterCode { get; set; }
            public long CompleteWhCode { get; set; }
            public decimal ActualReqQty { get; set; }

            public string PrivateDescSeg15 { get; set; }
            public long CostUOM { get; set; }
            public string SupplyWhCode { get; set; }
            public string KCKYL { get; set; }
            public decimal IssuedQty { get; set; }
            public string SetableStatus { get; set; }
            public string StoreUOM { get; set; }
            public string Round_Precision { get; set; }
            public string DescFlexField_PrivateDescSeg2 { get; set; }

        }

        public class NMoItem
        {
            public long ItemMasterCode { get; set; }

        }

        public string getkc(string item, string whid)
        {
            string ck = "0";
            //string sqlForCPRK = "select  A5.[Name] as [Wh_Name], A.[ItemInfo_ItemCode] as [Item_ItemCode]," +
            //    "isnull( case  when A2.[ItemFormAttribute] in (16, 22) then A.[ItemInfo_ItemName] else A2.[Name] end ,'') as [ItemSeg_Name]," +
            //    "sum(A.[ToRetStQty]) as [PUToRetQty], A3.[Round_Precision] as [Round1_Precision], convert(decimal(24, 9), 0) as [InOnWayQty], " +
            //    "convert(decimal(24, 9), 0) as [OutOnWayQty], sum( case  when((((A.[IsProdCancel] = 1) or(A.[MO_EntityID] != 0))" +
            //    "or A.[ProductDate] is not null) or(A.[WP_EntityID] != 0)) then A.[StoreQty] else convert(decimal(24, 9), 0) end ) as [NotUseQty]," +
            //    "sum((((A.[StoreQty] - A.[ResvStQty]) - A.[ResvOccupyStQty]) -  case  when((((A.[IsProdCancel] = 1) or(A.[MO_EntityID] != 0)) or A.[ProductDate] is not null)" +
            //    "or(A.[WP_EntityID] != 0)) then A.[StoreQty] else convert(decimal(24, 9), 0) end )) as [CanUseQty], sum(A.[ResvStQty]) as [ReservQty], sum((A.[StoreQty] + A.[ToRetStQty]))" +
            //    "as [BalQty], sum((A.[StoreMainQty] + A.[ToRetStMainQty])) as [BalQty_Main]," +
            //    "sum((((((A.[StoreQty] - A.[ResvStQty]) - A.[ResvOccupyStQty]) -  case  when((((A.[IsProdCancel] = 1) or(A.[MO_EntityID] != 0)) or A.[ProductDate] is not null) or(A.[WP_EntityID] != 0)) then A.[StoreQty] else convert(decimal(24, 9), 0) end ) +A.[SupplyQtySU]) -A.[DemandQtySU])) as [Temp_PAB], convert(bigint, 0) as [Item_ItemID], " +
            //    "convert(bigint, 0) as [W_Uom], convert(bigint, 0) as [MainBaseSU_ID]" +
            //    "from InvTrans_WhQoh as A  left join[CBO_Wh] as A1 on(A.[Wh] = A1.[ID])  left join[CBO_ItemMaster] as A2 on(A.[ItemInfo_ItemID] = A2.[ID])  left join[Base_UOM] as A3 on(A.[StoreUOM] = A3.[ID])  left join[Base_Organization] as A4 on(A.[LogisticOrg] = A4.[ID])  left join[CBO_Wh_Trl] as A5 on(A5.SysMlFlag = 'zh-CN') and(A1.[ID] = A5.[ID])" +
            //    "where(((A2.[Name] is not null and (A2.[Name] != '')) and(A4.[Code] = N'10')) and(A.ItemInfo_ItemID = '" + item.ToString() + "')) and A5.ID = '" + whid.ToString() + "'" +
            //    " group by A5.[Name], A.[ItemInfo_ItemCode]," +
            //    "isnull( case  when A2.[ItemFormAttribute] in (16, 22) then A.[ItemInfo_ItemName] else A2.[Name] end ,''), A3.[Round_Precision]"; 

            string sqlForCPRK = "select ItemInfo_ItemCode as ItemCode,org.Code as 组织编码,sum(WhQoh.StoreQty - WhQoh.ResvStQty - WhQoh.ResvOccupyStQty) as CanUseQty  from InvTrans_WhQoh as WhQoh" +
                " inner join Base_Organization org on org.id = WhQoh.ItemOwnOrg" +
                " where WhQoh.ItemInfo_ItemID = '" + item.ToString() + "' and WhQoh.Wh = '" + whid.ToString() + "'" +
                " group by WhQoh.ItemInfo_ItemCode,org.Code";
            DataTable dataTable = new DataTable();
            DataSet dataSet = new DataSet();
            //sqlForCPRK 成品入库
            DataAccessor.RunSQL(DataAccessor.GetConn(), sqlForCPRK, null, out dataSet);
            dataTable = dataSet.Tables[0];
            if (dataTable != null && dataTable.Rows.Count > 0)
            {
                ck = dataTable.Rows[0]["CanUseQty"] == null ? "0" : dataTable.Rows[0]["CanUseQty"].ToString();
            }
            if (string.IsNullOrEmpty(ck))
            {
                ck = "0";
            }
            return ck;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        /// <param name="moid"></param>
        /// <param name="docno"></param>
        /// <returns></returns>
        /// 原来的例子
        //select sum(ActualReqQty) as ActualReqQty from MO_MO a
        //inner join MO_MOPickList b on a.ID = b.MO where b.ItemMaster = '1002304250315751'and a.TotalRcvQty = '0'
        //and(select DescFlexField_PrivateDescSeg15 from CBO_ItemMaster where ID = b.ItemMaster) = '1'and a.DocState != '3' 
        //and b.SupplyWh= '1002302240000591'and a.DocNo= '256601'
        public string getActualReqQty(long item, string moid, string docno)
        {
            string ck = "0";
            //string sqlForCPRK = "select sum(ActualReqQty) as ActualReqQty" +
            //    " from MO_MO a inner" +
            //    " join MO_MOPickList b" +
            //    " on a.ID = b.MO" +
            //    " where b.ItemMaster = '" + item.ToString() + "'" +
            //    "and(select DescFlexField_PrivateDescSeg15 from CBO_ItemMaster where ID = b.ItemMaster) = '1'" +
            //    "and a.DocState != '3' and b.SupplyWh='" + moid + "'";
            //string sqlForCPRK = "select  top(1) MatchQty,A1.ItemMaster,A1.MOID from " +
            //    " MO_SimuMatchResult A1 " +
            //    " left join MO_SimuDemandPick A2 on A2.ID = A1.DemandPick " +
            //    " where A2.docno > '200000' " +
            //    " and A1.MOID = (select ID from MO_MO where DocNo = '" + docno + "') and A1.ItemMaster = '" + item.ToString() + "' " +
            //    " and A1.Wh = '" + moid + "'";
            string sqlForCPRK = "select  top(1) MatchQty,A1.ItemMaster,A1.MOID from  " +
                " MO_SimuMatchResult A1 " +
                " left join MO_SimuDemandPick A2 on A2.ID = A1.DemandPick " +
                " left join MO_MO B1 ON B1.ID = A1.MOID " +
                " left join CBO_ItemMaster C1 ON C1.ID = A1.ItemMaster  " +
                " where " +
                " B1.DocNo = '" + docno + "'  and A1.ItemMaster = '" + item.ToString() + "' " +
                " and B1.DescFlexField_PrivateDescSeg2 = ''" +
                " and C1.DescFlexField_PrivateDescSeg15 = '1' ";
            DataTable dataTable = new DataTable();
            DataSet dataSet = new DataSet();
            //sqlForCPRK 成品入库
            DataAccessor.RunSQL(DataAccessor.GetConn(), sqlForCPRK, null, out dataSet);
            dataTable = dataSet.Tables[0];
            if (dataTable != null && dataTable.Rows.Count > 0)
            {
                //ck = dataTable.Rows[0]["ActualReqQty"] == null ? "0" : dataTable.Rows[0]["ActualReqQty"].ToString();
                ck = dataTable.Rows[0]["MatchQty"] == null ? "0" : dataTable.Rows[0]["MatchQty"].ToString();
            }

            if (string.IsNullOrEmpty(ck))
            {
                ck = "0";
            }

            return ck;
        }

    }
}