using System;
using System.Collections.Generic;

using UFIDA.U9.ISV.MO;
using UFSoft.UBF.Business;
using UFSoft.UBF.Util.Log;


namespace YY.U9.Cust.LI.AppPlugIn
{
    /// <summary>
    /// 高登客开页面，生产订单变更备料表，审核，对生产订单备料做增删改查
    /// </summary>
    [UFSoft.UBF.Eventing.Configuration.Failfast]
    class GDCusfMOPickListUpdateSubScrilber : UFSoft.UBF.Eventing.IEventSubscriber
    {
        private static readonly UFSoft.UBF.Util.Log.ILogger logger = LoggerManager.GetLogger(typeof(ForecastOrderInsertedSubscrilber));
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

            UFIDA.U9.Cust.CustMoPickListBE.CustMoPickListHead moPickListHead = key.GetEntity() as UFIDA.U9.Cust.CustMoPickListBE.CustMoPickListHead;
            if (moPickListHead == null)
            {
                return;
            }
            #endregion

            //循环行获取变更的状态
            //变更方式
            //新增--1
            //删除--2
            //料品替代--0
            //部分替代--3
            if (moPickListHead.Status.Value == 2)
            {
                foreach (var item in moPickListHead.CustMoPickListLine)
                {
                    int ChangeMethod = item.ChangeMethod.Value;

                    #region 调用服务修改备料

                    UFIDA.U9.ISV.MO.Proxy.ModifyMO4ExternalProxy modifyMO4ExternalProxy = new UFIDA.U9.ISV.MO.Proxy.ModifyMO4ExternalProxy();

                    #region 查询生产订单
                    UFIDA.U9.ISV.MO.Proxy.QueryMO4ExternalProxy queryMO4ExternalProxy = new UFIDA.U9.ISV.MO.Proxy.QueryMO4ExternalProxy();

                    List<MOKeyDTOData> mOKeyDTODatas = new List<MOKeyDTOData>();
                    MOKeyDTOData mOKeyDTOData = new MOKeyDTOData();

                    mOKeyDTOData.DocNo = item.MoDocNo.DocNo;
                    mOKeyDTOData.ID = item.MoDocNo.ID;
                    queryMO4ExternalProxy.TargetOrgCode = item.MoDocNo.Org.Code;
                    queryMO4ExternalProxy.TargetOrgName = item.MoDocNo.Org.Name;

                    mOKeyDTODatas.Add(mOKeyDTOData);


                    queryMO4ExternalProxy.MOKeyDTOs = mOKeyDTODatas;

                    List<MODTOData> mODTOData = queryMO4ExternalProxy.Do();

                    MODTOData newMODTO = new MODTOData();

                    foreach (var item2 in mODTOData)
                    {
                        newMODTO = item2;
                    }
                    #endregion


                    //修改生产订单
                    List<MOModifyDTOData> moModifyDTOs = new List<MOModifyDTOData>();
                    UFIDA.U9.ISV.MO.MOModifyDTOData moModifyDTO = new UFIDA.U9.ISV.MO.MOModifyDTOData();

                    UFIDA.U9.ISV.MO.MOKeyDTOData moKey = new UFIDA.U9.ISV.MO.MOKeyDTOData();
                    moModifyDTO.MOKeyDTO = moKey;

                    moKey.DocNo = newMODTO.DocNo; //查询得到的单据号

                    //cud=8是删除，只删除虚拟件
                    //2-Inserted;4-Updated;8-Deleted
                    //这边重新改造

                    if (ChangeMethod == 2)//2删除--保存时需校验备料中，该行料品 已发放数量 是否大于 0，大于0提示备料
                                          //行已发放，不允许替换。小于等于0时，审核后删除对应行。
                    {
                        foreach (var item3 in newMODTO.MOPickListDTOs)
                        {
                            if (item.OriItem.ID == item3.ItemMaster.ID)
                            {
                                if (item3.IssuedQty <= 0)
                                {
                                    item3.CUD = 8;
                                }
                                else
                                {
                                    throw new Exception("行号【" + item.DocLineNo + "】备料行已发放，不允许替换。");
                                }
                            }
                            //item3.CUD = 
                        }
                    }
                    else if (ChangeMethod == 1)//新增--不做条件检查。
                    {
                        MOPickListDTOData mOPickListDTO = new MOPickListDTOData();
                        mOPickListDTO.CUD = 2;
                        mOPickListDTO.ItemMaster = new UFIDA.U9.CBO.Pub.Controller.CommonArchiveDataDTOData();
                        mOPickListDTO.ItemMaster.ID = item.Item.ID;
                        mOPickListDTO.ItemMaster.Code = item.Item.Code;
                        mOPickListDTO.ItemMaster.Name = item.Item.Name;
                        mOPickListDTO.BOMReqQty = decimal.Parse(item.BOMQty == null ? "0" : item.BOMQty);
                        mOPickListDTO.ActualReqQty = decimal.Parse(item.ActualReqQty == null ? "0" : item.ActualReqQty);
                        mOPickListDTO.IssuedQty = decimal.Parse(item.IssuedQty == "" ? "0" : item.IssuedQty);
                        //item3.SpecialIssuedQty = item.OriSpecialIssuedQty.ToString();
                        mOPickListDTO.IssueStyle = item.IssueStyle.Value;
                        try
                        {
                            string se = item.WH.Name;
                            mOPickListDTO.SupplyWh = new UFIDA.U9.CBO.Pub.Controller.CommonArchiveDataDTOData();
                            mOPickListDTO.SupplyWh.ID = item.WH.ID;
                            mOPickListDTO.SupplyWh.Name = item.WH.Name;
                            mOPickListDTO.SupplyWh.Code = item.WH.Code;
                        }
                        catch (Exception ex)
                        {
                            string mse = ex.Message;
                        }
                        newMODTO.MOPickListDTOs.Add(mOPickListDTO);
                    }
                    else if (ChangeMethod == 0)//料品替代
                                               //保存时需校验备料中，该行料品 已发放数量 是否大于 0，大于0提示备料已发放，
                                               //不允许替换。小于等于0时，审核后替换备料中对应的行。
                    {
                        foreach (var item3 in newMODTO.MOPickListDTOs)
                        {
                            if (item.OriItem.ID == item3.ItemMaster.ID)
                            {
                                if (item3.IssuedQty <= 0)
                                {
                                    item3.CUD = 4;
                                    item3.ItemMaster.ID = item.Item.ID;
                                    item3.ItemMaster.Code = item.Item.Code;
                                    item3.ItemMaster.Name = item.Item.Name;

                                    item3.BOMReqQty = decimal.Parse(item.BOMQty == null ? "0" : item.BOMQty);
                                    item3.ActualReqQty = decimal.Parse(item.ActualReqQty == null ? "0" : item.ActualReqQty);
                                    item3.IssuedQty = decimal.Parse(item.IssuedQty == "" ? "0" : item.IssuedQty);
                                    //item3.SpecialIssuedQty = item.OriSpecialIssuedQty.ToString();
                                    item3.IssueStyle = item.IssueStyle.Value;
                                    //if (item.WH != null)
                                    //{
                                    //    item3.SupplyWh.ID = item.WH.ID;
                                    //    item3.SupplyWh.Name = item.WH.Name;
                                    //    item3.SupplyWh.Code = item.WH.Code;
                                    //}
                                    try
                                    {
                                        string se = item.WH.Name;
                                        item3.SupplyWh.ID = item.WH.ID;
                                        item3.SupplyWh.Name = item.WH.Name;
                                        item3.SupplyWh.Code = item.WH.Code;
                                    }
                                    catch (Exception ex)
                                    {
                                        string mse = ex.Message;
                                    }

                                }
                                else
                                {
                                    throw new Exception("行号【" + item.DocLineNo + "】备料行已发放，不允许替换。");
                                }
                            }
                            //item3.CUD = 
                        }
                    }
                    else if (ChangeMethod == 3)//部分替代
                                               //保存时许校验备料中，该行料品已发放数量大于调整数量(原数量-部分替换数量)提示备料已发放数量超过调整数量，不允许替换。
                                               //小于于调整数量时，审核后替换原备料数量为调整数量，新增一行为替代料的料品和数量。比如原备料数量 A 100 个，
                                               //部分替换为B60 个,调整数量为 40。审核时回写备料数量 A40个，B60个

                    {
                        foreach (var item3 in newMODTO.MOPickListDTOs)
                        {
                            if (item.OriItem.ID == item3.ItemMaster.ID)
                            {

                                decimal changeqty = decimal.Parse(item.OriIssuedQty) - decimal.Parse(item.IssuedQty);
                                decimal changeActualReqQty = decimal.Parse(item.OriActualReqQty) - decimal.Parse(item.ActualReqQty);
                                item3.CUD = 4;
                                if (item3.IssuedQty > changeqty)
                                {
                                    throw new Exception("行号【" + item.DocLineNo + "】备料行已发放数量超过调整数量，不允许替换。");
                                }
                                else
                                {
                                    item3.IssuedQty = changeqty;

                                }
                                if (0 > changeActualReqQty)
                                {
                                    throw new Exception("行号【" + item.DocLineNo + "】备料行BOM需求数量为负数，不允许替换。");
                                }
                                else
                                {
                                    item3.ActualReqQty = changeActualReqQty;

                                }
                            }
                            //item3.CUD = 
                        }
                        #region 新增
                        MOPickListDTOData mOPickListDTO = new MOPickListDTOData();
                        mOPickListDTO.CUD = 2;
                        mOPickListDTO.ItemMaster = new UFIDA.U9.CBO.Pub.Controller.CommonArchiveDataDTOData();
                        mOPickListDTO.ItemMaster.ID = item.Item.ID;
                        mOPickListDTO.ItemMaster.Code = item.Item.Code;
                        mOPickListDTO.ItemMaster.Name = item.Item.Name;
                        mOPickListDTO.BOMReqQty = decimal.Parse(item.BOMQty == null ? "0" : item.BOMQty);
                        mOPickListDTO.ActualReqQty = decimal.Parse(item.ActualReqQty == null ? "0" : item.ActualReqQty);
                        mOPickListDTO.IssuedQty = decimal.Parse(item.IssuedQty == "" ? "0" : item.IssuedQty);
                        //item3.SpecialIssuedQty = item.OriSpecialIssuedQty.ToString();
                        mOPickListDTO.IssueStyle = item.IssueStyle.Value;
                        //if (item.WH != null)
                        //{
                        //    mOPickListDTO.SupplyWh = new UFIDA.U9.CBO.Pub.Controller.CommonArchiveDataDTOData();
                        //    mOPickListDTO.SupplyWh.ID = item.WH.ID;
                        //    mOPickListDTO.SupplyWh.Name = item.WH.Name;
                        //    mOPickListDTO.SupplyWh.Code = item.WH.Code;
                        //}
                        try
                        {
                            string se = item.WH.Name;
                            mOPickListDTO.SupplyWh = new UFIDA.U9.CBO.Pub.Controller.CommonArchiveDataDTOData();
                            mOPickListDTO.SupplyWh.ID = item.WH.ID;
                            mOPickListDTO.SupplyWh.Name = item.WH.Name;
                            mOPickListDTO.SupplyWh.Code = item.WH.Code;
                        }
                        catch (Exception ex)
                        {
                            string mse = ex.Message;
                        }
                        newMODTO.MOPickListDTOs.Add(mOPickListDTO);
                        #endregion

                    }
                    moModifyDTO.MODTO = newMODTO; //查询得到的结果作为修改的基础输入
                    moModifyDTOs.Add(moModifyDTO);
                    modifyMO4ExternalProxy.MOModifyDTOs = moModifyDTOs;
                    modifyMO4ExternalProxy.TargetOrgCode = item.MoDocNo.Org.Code;
                    modifyMO4ExternalProxy.TargetOrgName = item.MoDocNo.Org.Name;
                    bool see123 = modifyMO4ExternalProxy.Do();

                    #endregion

                }
            }
        }
    }
}
