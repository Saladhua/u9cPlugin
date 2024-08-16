using System.Collections.Generic;
using System.Data;
using System.Linq;
using UFIDA.U9.CBO.MFG.BOM;
using UFIDA.U9.CBO.SCM.Item;
using UFIDA.U9.ISV.MO;
using UFIDA.U9.MO.MO;
using UFSoft.UBF.Business;
using UFSoft.UBF.PL.Engine;
using UFSoft.UBF.Util.DataAccess;
using UFSoft.UBF.Util.Log;
namespace YY.U9.Cust.LI.AppPlugIn
{
    /// <summary>
    /// 生产订单排序
    /// </summary>
    /// 计划订单释放的时候进行排序，备料表第一行不是虚拟件的情况，把虚拟件置前
    [UFSoft.UBF.Eventing.Configuration.Failfast]

    class ClMoSortInsertedSubscrilber : UFSoft.UBF.Eventing.IEventSubscriber
    {
        private static readonly UFSoft.UBF.Util.Log.ILogger logger = LoggerManager.GetLogger(typeof(ClMoSortInsertedSubscrilber));

        public void Notify(params object[] args)
        {
            #region 从事件参数中取得当前业务实体
            //从事件参数中取得当前业务实体
            if (args == null || args.Length == 0 || !(args[0] is UFSoft.UBF.Business.EntityEvent))
                return;
            BusinessEntity.EntityKey key = ((UFSoft.UBF.Business.EntityEvent)args[0]).EntityKey;
            if (key == null)
            {
                return;
            }
            MO mo = key.GetEntity() as MO;
            if (mo == null)
            {
                return;
            }
            #endregion
            //料品形态属性
            int itemMasterItemFormAttribute = 0;

            foreach (var item in mo.MOPickLists)
            {
                if (item.DocLineNO == 10)
                {
                    itemMasterItemFormAttribute = item.ItemMaster.ItemFormAttribute.Value;
                }
            }

            if (mo.MOSourceDocType.Value != 9)
            {
                return;
            }

            //如果备料的第一行是虚拟件就跳出 
            //if (itemMasterItemFormAttribute == 6)
            //{
            //    return;
            //}

            //mo.SysState

            List<MOPickList> OldMoPiclLists = new List<MOPickList>();

            foreach (var item in mo.MOPickLists)
            {
                MOPickList mOPickList = new MOPickList();
                mOPickList = item;
                OldMoPiclLists.Add(mOPickList);
            }

            List<MOPickList> moPiclListsWithIMIFA = new List<MOPickList>();//只有虚拟件的备料

            List<MOPickList> moPiclListsNoIMIFA = new List<MOPickList>();//没有虚拟件的备料



            foreach (var item in mo.MOPickLists)
            {
                string se = item.ItemMaster.Code;

                se = se.Substring(0, 1);
                //取第一位

                if (item.ItemMaster.ItemFormAttribute.Value == 6)
                // if (se == "2")
                {
                    moPiclListsWithIMIFA.Add(item);
                }
            }

            foreach (var item in mo.MOPickLists)
            {
                if (item.ItemMaster.ItemFormAttribute.Value != 6)
                {
                    moPiclListsNoIMIFA.Add(item);
                }
            }

            //没有虚拟件，或者来源类型不为虚拟件就跳出
            //if (mo.MOSourceDocType.Value != 9 || moPiclListsWithIMIFA.Count == 0)
            if (moPiclListsWithIMIFA.Count == 0)
            {
                // return;
            }
            #region 排序操作
            //BOMMaster bOMMaster = BOMMaster.Finder.Find("ItemMaster.ID='" + srcDocKey + "' and Org.ID='" + +"'");

            List<Dto> dtos = new List<Dto>();//记录子项
            int i = 1;
            #region 查询bom子项
            foreach (var item in moPiclListsWithIMIFA)
            {
                if (item.ItemMaster.ItemFormAttribute.Value == 6)
                {
                    string BOMVersionCode = "";
                    DataTable dataTable_2 = new DataTable();
                    string sql_2 = "select top(1) BOMVersionCode as BOMVersionCode from CBO_BOMMaster where ItemMaster =" +
                        "(select ID from CBO_ItemMaster where ID = '" + item.ItemMaster.ID + "' and  Org='" + item.ItemMaster.Org.ID + "')" +
                        "order by BOMVersionCode desc";
                    DataSet dataSet_2 = new DataSet();
                    DataAccessor.RunSQL(DataAccessor.GetConn(), sql_2, null, out dataSet_2);
                    dataTable_2 = dataSet_2.Tables[0];
                    if (dataTable_2.Rows != null && dataTable_2.Rows.Count > 0)
                    {
                        BOMVersionCode = dataTable_2.Rows[0]["BOMVersionCode"].ToString();
                    }

                    BOMMaster bOMMaster = BOMMaster.Finder.Find("ItemMaster='" + item.ItemMaster.ID + "' and Org.ID='" + item.ItemMaster.Org.ID + "' and BOMVersionCode='" + BOMVersionCode + "'");

                    if (bOMMaster != null)
                    {
                        foreach (var bOM in bOMMaster.BOMComponents)
                        {
                            Dto dto = new Dto();
                            dto.No = i;
                            i = i + 1;
                            dto.ItemMasterID = bOM.ItemMaster.ID;
                            dto.ItemMasterCode = bOM.ItemMaster.Code;
                            dto.ItemMasterName = bOM.ItemMaster.Name;
                            dtos.Add(dto);
                        }
                    }
                }
            }
            #endregion


            #endregion


            List<Dto> dtos2 = new List<Dto>();//记录子项
            int ii = 1;
            #region 查询bom子项

            BOMMaster bOMMaster2 = BOMMaster.Finder.Find("ItemMaster='" + mo.ItemMaster.ID + "' and Org.ID='" + mo.Org.ID + "'");
            if (bOMMaster2 != null)
            {

                foreach (var bOM in bOMMaster2.BOMComponents)
                {
                    if (bOM.ItemMaster.ItemFormAttribute.Value != 6)
                    {
                        Dto dto = new Dto();
                        dto.No = ii;
                        ii = ii + 1;
                        dto.ItemMasterID = bOM.ItemMaster.ID;
                        dto.ItemMasterCode = bOM.ItemMaster.Code;
                        dto.ItemMasterName = bOM.ItemMaster.Name;
                        dtos2.Add(dto);

                    }
                }
            }


            #endregion


            List<MOPickList> newMoPickList = new List<MOPickList>();//新的虚拟件bom的子项的排序


            //int ii = 2;
            // i = 10;
            //foreach (var item in dtos)
            //{
            //    //if (item.ItemMaster.Code == "30302-00142")
            //    //{
            //    //    string fuck = "fuck";
            //    //}


            //    foreach (var itemii in mo.MOPickLists)
            //    {
            //        if (item.ItemMasterCode == itemii.ItemMaster.Code)
            //        {
            //            MOPickList mOPickList = new MOPickList();
            //            mOPickList = itemii;
            //            itemii.DocLineNO = i;
            //            i = i + 10;
            //            newMoPickList.Add(mOPickList);
            //        }
            //    }

            //    //int b = dtos.Where(x => x.ItemMasterCode == item.ItemMaster.Code).Count();

            //    //int c = 0;

            //    //bool go = false;

            //    //List<Dto> itemcode = dtos.Where(x => x.ItemMasterCode == item.ItemMaster.Code).ToList();

            //    //foreach (var itemii in itemcode)
            //    //{
            //    //    c = itemii.No;
            //    //    if (c == ii)
            //    //    {
            //    //        go = true;
            //    //    }
            //    //}

            //    //MOPickList mOPickList = new MOPickList();
            //    //mOPickList = item;

            //    //if (b > 0 && go == true)
            //    //{
            //    //    item.DocLineNO = i;
            //    //    i = i + 10;
            //    //    newMoPickList.Add(mOPickList);
            //    //}
            //}

            ////foreach (var item in moPiclListsNoIMIFA)
            ////{
            ////    int b = dtos.Where(x => x.ItemMasterID == item.ItemMaster.ID).Count();
            ////    MOPickList mOPickList = new MOPickList();
            ////    mOPickList = item;
            ////    item.DocLineNO = i;
            ////    i = i + 10;
            ////    if (b == 0)
            ////    {
            ////        newMoPickList.Add(mOPickList);
            ////    }
            ////}


            #region 调用服务修改备料


            UFIDA.U9.ISV.MO.Proxy.ModifyMO4ExternalProxy modifyMO4ExternalProxy = new UFIDA.U9.ISV.MO.Proxy.ModifyMO4ExternalProxy();

            #region 查询生产订单
            UFIDA.U9.ISV.MO.Proxy.QueryMO4ExternalProxy queryMO4ExternalProxy = new UFIDA.U9.ISV.MO.Proxy.QueryMO4ExternalProxy();

            List<MOKeyDTOData> mOKeyDTODatas = new List<MOKeyDTOData>();
            MOKeyDTOData mOKeyDTOData = new MOKeyDTOData();

            mOKeyDTOData.DocNo = mo.DocNo;
            mOKeyDTOData.ID = mo.ID;
            queryMO4ExternalProxy.TargetOrgCode = mo.Org.Code;
            queryMO4ExternalProxy.TargetOrgName = mo.Org.Name;

            mOKeyDTODatas.Add(mOKeyDTOData);


            queryMO4ExternalProxy.MOKeyDTOs = mOKeyDTODatas;

            List<MODTOData> mODTOData = queryMO4ExternalProxy.Do();

            MODTOData newMODTO = new MODTOData();

            foreach (var item in mODTOData)
            {
                newMODTO = item;
            }
            #endregion


            //修改生产订单
            List<MOModifyDTOData> moModifyDTOs = new List<MOModifyDTOData>();
            UFIDA.U9.ISV.MO.MOModifyDTOData moModifyDTO = new UFIDA.U9.ISV.MO.MOModifyDTOData();

            UFIDA.U9.ISV.MO.MOKeyDTOData moKey = new UFIDA.U9.ISV.MO.MOKeyDTOData();
            moModifyDTO.MOKeyDTO = moKey;

            moKey.DocNo = newMODTO.DocNo; //查询得到的单据号

            //cud=8是删除，只删除虚拟件

            //这边重新改造
            foreach (var item in newMODTO.MOPickListDTOs)
            {
                ItemMaster item1 = ItemMaster.Finder.FindByID(item.ItemMaster.ID);
                if (item1.ItemFormAttribute.Value == 6)
                {
                    item.CUD = 8;
                }
                //else
                //{
                //    //    //string se = item.ItemMaster.Code;

                //    //    //se = se.Substring(0, 1);//取第一位

                //    int b = dtos.Where(x => x.ItemMasterID == item.ItemMaster.ID).Count();

                //    if (b > 0)
                //    {
                //        item.DescFlexField.PrivateDescSeg27 = "子项";
                //        //item.DocLineNo = ij;
                //        //ij = ij + 10;
                //    }
                //    else
                //    {
                //        item.DescFlexField.PrivateDescSeg27 = "非子项";
                //    }
                //}

            }

            int cc = 10;

            string see123333 = "";

            foreach (var item in dtos)
            {
                foreach (var itemii in newMODTO.MOPickListDTOs)
                {
                    if (item.ItemMasterCode == itemii.ItemMaster.Code)
                    {
                        itemii.CUD = 4;
                        //int dln = int.Parse(itemii.DescFlexField.PrivateDescSeg25);

                        string DocLineNo = itemii.DocLineNo.ToString();
                        DocLineNo = DocLineNo.Substring(DocLineNo.Length - 1, 1);
                        if (int.Parse(DocLineNo) % 10 == 0)
                        {
                            itemii.DocLineNo = cc;
                            //item.DescFlexField.PrivateDescSeg26 = cc.ToString();
                            cc = cc + 10;

                        }
                        else
                        {
                            //item.DescFlexField.PrivateDescSeg26 = (cc - 10 + item.DocLineNo % 10).ToString();
                            itemii.DocLineNo = cc - 10 + int.Parse(DocLineNo) % 10;
                        }

                        see123333 = itemii.DocLineNo + see123333 + ",";
                    }
                }
            }


            string see123333123 = "";


            foreach (var itemii in newMODTO.MOPickListDTOs)
            {
                int b = dtos.Where(x => x.ItemMasterID == itemii.ItemMaster.ID).Count();
                if (b < 1)
                {
                    {
                        itemii.CUD = 4;
                        string DocLineNo = itemii.DocLineNo.ToString();
                        DocLineNo = DocLineNo.Substring(DocLineNo.Length - 1, 1);
                        if (int.Parse(DocLineNo) % 10 == 0)
                        {
                            itemii.DocLineNo = cc;
                            //item.DescFlexField.PrivateDescSeg26 = cc.ToString();
                            cc = cc + 10;
                        }
                        else
                        {
                            //item.DescFlexField.PrivateDescSeg26 = (cc - 10 + item.DocLineNo % 10).ToString();
                            itemii.DocLineNo = cc - 10 + int.Parse(DocLineNo) % 10;
                        }

                        see123333123 = itemii.DocLineNo + see123333123 + ",";

                    }
                }
            }
            foreach (var item in newMODTO.MOPickListDTOs)
            {
                see123333 = see123333 + item.DocLineNo + ",";
            }





            //foreach (var item in newMODTO.MOPickListDTOs)
            //{
            //    if (item.DescFlexField.PrivateDescSeg27 == "子项")
            //    {
            //        item.CUD = 4;
            //        if (item.DocLineNo % 10 == 0)
            //        {
            //            item.DocLineNo = cc;
            //            //item.DescFlexField.PrivateDescSeg26 = cc.ToString();
            //            cc = cc + 10;
            //        }
            //        else
            //        {
            //            //item.DescFlexField.PrivateDescSeg26 = (cc - 10 + item.DocLineNo % 10).ToString();
            //            item.DocLineNo = cc - 10 + item.DocLineNo % 10;
            //        }
            //    }
            //}

            //foreach (var item in newMODTO.MOPickListDTOs)
            //{

            //    if (item.DescFlexField.PrivateDescSeg27 == "非子项")
            //    {
            //        item.CUD = 4;
            //        if (item.DocLineNo % 10 == 0)
            //        {
            //            item.DocLineNo = cc;
            //            //item.DescFlexField.PrivateDescSeg26 = cc.ToString();
            //            cc = cc + 10;
            //        }
            //        else
            //        {
            //            //item.DescFlexField.PrivateDescSeg26 = (cc - 10 + item.DocLineNo % 10).ToString();
            //            item.DocLineNo = cc - 10 + item.DocLineNo % 10;
            //        }
            //    }
            //}
            //}

            //foreach (var item in dtos2)
            //{
            //    foreach (var itemii in newMODTO.MOPickListDTOs)
            //    {
            //        if (item.ItemMasterCode == itemii.ItemMaster.Code)
            //        {
            //            itemii.CUD = 4;
            //            //int dln = int.Parse(itemii.DescFlexField.PrivateDescSeg25);
            //            if (itemii.DocLineNo % 10 == 0)
            //            {
            //                itemii.DocLineNo = cc;
            //                //item.DescFlexField.PrivateDescSeg26 = cc.ToString();
            //                cc = cc + 10;
            //            }
            //            else
            //            {
            //                //item.DescFlexField.PrivateDescSeg26 = (cc - 10 + item.DocLineNo % 10).ToString();
            //                itemii.DocLineNo = cc - 10 + itemii.DocLineNo % 10;
            //            }
            //        }
            //    }
            //}


            moModifyDTO.MODTO = newMODTO; //查询得到的结果作为修改的基础输入
            moModifyDTOs.Add(moModifyDTO);
            int see = newMODTO.MOPickListDTOs.Count;

            #region 原来的方法注销完全不好使现在改完只修改行号试试
            //int c = 10;

            //CUD 新增标志, 默认为修改 2-Inserted;4-Updated;8-Deleted

            //foreach (var j in newMoPickList)
            //{
            //    MOPickListDTOData mOPickList = new MOPickListDTOData();
            //    mOPickList.DocLineNo = c;
            //    mOPickList.CUD = 2;
            //    mOPickList.OperationNum = j.OperationNum;
            //    c = c + 10;
            //    CommonArchiveDataDTOData itemMaster = new CommonArchiveDataDTOData();
            //    itemMaster.Code = j.ItemMaster.Code;
            //    itemMaster.ID = j.ItemMaster.ID;
            //    itemMaster.Name = j.ItemMaster.Name;
            //    mOPickList.ItemMaster = new UFIDA.U9.CBO.Pub.Controller.CommonArchiveDataDTOData();
            //    mOPickList.ItemMaster = itemMaster;
            //    mOPickList.IsSubstitute = j.IsSubstitute;
            //    mOPickList.IsCheckUTE = j.IsCheckUTE;
            //    if (mOPickList.IsSubstitute == true)
            //    {
            //        mOPickList.SubstitutedItem = new CommonArchiveDataDTOData();
            //        mOPickList.SubstitutedItem.ID = j.SubstitutedItem.ID;
            //        mOPickList.SubstitutedItem.Name = j.SubstitutedItem.Name;
            //        mOPickList.SubstitutedItem.Code = j.SubstitutedItem.Code;
            //    }
            //    mOPickList.ActualReqDate = j.ActualReqDate;
            //    mOPickList.QPA = j.QPA;
            //    mOPickList.IsCalcCost = j.IsCalcCost;
            //    mOPickList.BOMReqQty = j.BOMReqQty;
            //    mOPickList.ActualReqQty = j.ActualReqQty;
            //    mOPickList.STDReqQty = j.STDReqQty;
            //    mOPickList.ReserveQty = j.ReserveQty;
            //    mOPickList.IssuedQty = j.IssuedQty;
            //    mOPickList.IssueNotDeliverQty = j.IssueNotDeliverQty;
            //    mOPickList.IssueUOM = new CommonArchiveDataDTOData();
            //    mOPickList.IssueUOM.ID = j.IssueUOM.ID;
            //    mOPickList.IssueUOM.Code = j.IssueUOM.Code;
            //    mOPickList.IssueUOM.Name = j.IssueUOM.Name;
            //    mOPickList.BOMComponent = new CommonArchiveDataDTOData();
            //    if (j.BOMComponent != null)
            //    {
            //        mOPickList.BOMComponent.ID = j.BOMComponent.ID;
            //    }
            //    mOPickList.ReserveQty = j.ReserveQty;
            //    mOPickList.ActualReqDate = j.ActualReqDate;
            //    mOPickList.ItemVersion = new CommonArchiveDataDTOData();
            //    if (j.ItemVersion != null)
            //    {
            //        mOPickList.ItemVersion.ID = j.ItemVersion.ID;
            //    }
            //    mOPickList.DescFlexField = new UFIDA.U9.Base.FlexField.DescFlexField.DescFlexSegmentsData();
            //    mOPickList.DescFlexField.PrivateDescSeg1 = j.DescFlexField.PrivateDescSeg1;
            //    mOPickList.DescFlexField.PrivateDescSeg2 = j.DescFlexField.PrivateDescSeg2;
            //    mOPickList.DescFlexField.PrivateDescSeg3 = j.DescFlexField.PrivateDescSeg3;
            //    mOPickList.DescFlexField.PrivateDescSeg4 = j.DescFlexField.PrivateDescSeg4;
            //    mOPickList.DescFlexField.PrivateDescSeg5 = j.DescFlexField.PrivateDescSeg5;
            //    mOPickList.DescFlexField.PrivateDescSeg6 = j.DescFlexField.PrivateDescSeg6;
            //    mOPickList.DescFlexField.PrivateDescSeg7 = j.DescFlexField.PrivateDescSeg7;
            //    mOPickList.DescFlexField.PrivateDescSeg8 = j.DescFlexField.PrivateDescSeg8;
            //    mOPickList.DescFlexField.PrivateDescSeg9 = j.DescFlexField.PrivateDescSeg9;
            //    mOPickList.DescFlexField.PrivateDescSeg10 = j.DescFlexField.PrivateDescSeg10;
            //    mOPickList.DescFlexField.PrivateDescSeg11 = j.DescFlexField.PrivateDescSeg11;
            //    mOPickList.DescFlexField.PrivateDescSeg12 = j.DescFlexField.PrivateDescSeg12;
            //    newMODTO.MOPickListDTOs.Add(mOPickList);
            //}
            #endregion


            modifyMO4ExternalProxy.MOModifyDTOs = moModifyDTOs;
            modifyMO4ExternalProxy.TargetOrgCode = mo.Org.Code;
            modifyMO4ExternalProxy.TargetOrgName = mo.Org.Name;
            bool see123 = modifyMO4ExternalProxy.Do();

            #endregion

        }
    }


    public class Dto
    {
        public int No { get; set; }
        public long ItemMasterID { get; set; }
        public string ItemMasterCode { get; set; }
        public string ItemMasterName { get; set; }
    }

}


