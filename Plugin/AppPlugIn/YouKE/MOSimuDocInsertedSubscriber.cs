using System;
using System.Collections.Generic;
using UFIDA.U9.Base;
using UFIDA.U9.CBO.Pub.Controller;
using UFIDA.U9.CBO.SCM.Item;
using UFIDA.U9.MO.MO;
using UFSoft.UBF.Business;
using UFSoft.UBF.PL.Engine;
using UFSoft.UBF.Util.Log;

namespace YY.U9.Cust.LI.AppPlugIn
{
    /// <summary>
    /// 齐套分析
    /// 备料的全部存量-存量
    /// </summary>
    [UFSoft.UBF.Eventing.Configuration.Failfast]

    class MOSimuDocInsertedSubscriber : UFSoft.UBF.Eventing.IEventSubscriber
    {
        private static readonly ILogger logger = LoggerManager.GetLogger(typeof(MOSimuDocInsertedSubscriber));

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
            UFIDA.U9.MO.MO.MO simuDoc = key.GetEntity() as MO;
            if (simuDoc == null)
            {
                return;
            }


            #region 作废
            //UFIDA.U9.MO.PullList.Proxy.CreatePullListByPullProxy createPullListByPullProxy = new UFIDA.U9.MO.PullList.Proxy.CreatePullListByPullProxy();
            //List<CreatPullListInputDTOData> creatPullLists = new List<CreatPullListInputDTOData>();

            //foreach (var moitem in simuDoc.MOPickLists)
            //{
            //    ItemMaster item1 = ItemMaster.Finder.FindByID(moitem.ItemMaster.ID);
            //    if (item1.DescFlexField.PrivateDescSeg15 == "1" && !string.IsNullOrEmpty(simuDoc.SetableTime.Date.ToString()) && simuDoc.SetableStatus.Value == 2)
            //    {
            //        //select StoreQty from InvTrans_WhQoh where Wh = '1002302240000591'
            //        //and ItemInfo_ItemCode = '003'
            //        UFIDA.U9.MO.MO.MO mO = UFIDA.U9.MO.MO.MO.Finder.FindByID(simuDoc.ID);

            //        CreatPullListInputDTOData creatPullList = new CreatPullListInputDTOData();
            //        creatPullList.MO = simuDoc.ID;
            //        creatPullList.ItemMaster = moitem.ItemMaster.ID;

            //        creatPullList.DemandQty = moitem.ActualReqQty;
            //        #region 存储地点

            //        creatPullList.ToWh = mO.CompleteWh.ID;

            //        creatPullLists.Add(creatPullList);
            //        #endregion
            //    }
            //}
            //#region 原来的
            ////foreach (var item in simuDoc.MO)
            ////{
            ////    if (!string.IsNullOrEmpty(item.MO.SetableTime.Date.ToString()))
            ////    {
            ////        foreach (var moitem in item.MO.MOPickLists)
            ////        {
            ////            ItemMaster item1 = ItemMaster.Finder.FindByID(moitem.ItemMaster.ID);

            ////            if (item1.DescFlexField.PrivateDescSeg15 != "0")
            ////            {
            ////                //select StoreQty from InvTrans_WhQoh where Wh = '1002302240000591'
            ////                //and ItemInfo_ItemCode = '003'
            ////                UFIDA.U9.MO.MO.MO mO = UFIDA.U9.MO.MO.MO.Finder.FindByID(item.MO.ID);

            ////                CreatPullListInputDTOData creatPullList = new CreatPullListInputDTOData();
            ////                creatPullList.MO = item.MO.ID;
            ////                creatPullList.ItemMaster = moitem.ItemMaster.ID;

            ////                UFIDA.U9.InvTrans.WhQoh.WhQoh.EntityList whQoh = UFIDA.U9.InvTrans.WhQoh.WhQoh.Finder.FindAll("ItemInfo.ItemCode='" + moitem.ItemMaster.Code + "'and Wh='" + moitem.SupplyWh.ID + "'");
            ////                decimal qtys = 0;//供应地点的库存量
            ////                foreach (var qty in whQoh)
            ////                {
            ////                    qtys = qtys + qty.StoreQty;
            ////                }

            ////                decimal qqq = moitem.ActualReqQty - qtys;
            ////                if (qqq > 0)
            ////                {
            ////                    creatPullList.DemandQty = moitem.ActualReqQty;
            ////                }
            ////                else
            ////                {
            ////                    creatPullList.DemandQty = 0;
            ////                }

            ////                #region 存储地点

            ////                creatPullList.ToWh = mO.CompleteWh.ID;

            ////                creatPullLists.Add(creatPullList);
            ////                #endregion
            ////            }
            ////        }
            ////    }

            ////}

            //#endregion

            //createPullListByPullProxy.PullListDocType = 1002301123709586;//1002206140170625--测试环境1002301123709586
            //createPullListByPullProxy.CreatPullListInputDTO = creatPullLists;
            //List<string> see = createPullListByPullProxy.Do();

            #endregion



            #region 调入单
            UFIDA.U9.ISV.TransferInISV.Proxy.CommonCreateTransferInSVProxy transferInSVProxy = new UFIDA.U9.ISV.TransferInISV.Proxy.CommonCreateTransferInSVProxy();

            UFIDA.U9.ISV.TransferInISV.IC_TransferInDTOData[] Boms;
            List<UFIDA.U9.ISV.TransferInISV.IC_TransferInDTOData> listBom = new List<UFIDA.U9.ISV.TransferInISV.IC_TransferInDTOData>();
            List<UFIDA.U9.ISV.TransferInISV.IC_TransInLineDTOData> listBomLine = new List<UFIDA.U9.ISV.TransferInISV.IC_TransInLineDTOData>();

            //头
            UFIDA.U9.ISV.TransferInISV.IC_TransferInDTOData Bom = new UFIDA.U9.ISV.TransferInISV.IC_TransferInDTOData();
            Bom.TransInDocType = new UFIDA.U9.CBO.Pub.Controller.CommonArchiveDataDTOData();
            Bom.TransInDocType.Code = "TransIn010";//单据类型
            Bom.TransferType = 0;//调入类型 0为一步式 1为两步式
            Bom.Org = new UFIDA.U9.CBO.Pub.Controller.CommonArchiveDataDTOData();
            Bom.Org.Code = Context.LoginOrg.Code;
            Bom.BusinessDate = new DateTime();
            //Bom.Memo=//备注
            Bom.TransInLines = new List<UFIDA.U9.ISV.TransferInISV.IC_TransInLineDTOData>();
            Bom.SysState = ObjectState.Inserted;
            //行
            foreach (var item in simuDoc.MOPickLists)
            {
                ItemMaster item1 = ItemMaster.Finder.FindByID(item.ItemMaster.ID);
                if (item1.DescFlexField.PrivateDescSeg15 == "1")
                {

                    UFIDA.U9.ISV.TransferInISV.IC_TransInLineDTOData Bom_line = new UFIDA.U9.ISV.TransferInISV.IC_TransInLineDTOData();
                    Bom_line.TransInOwnerOrg = new UFIDA.U9.CBO.Pub.Controller.CommonArchiveDataDTOData();
                    Bom_line.TransInOwnerOrg.Code = Context.LoginOrg.Code;
                    Bom_line.TransInWh = new UFIDA.U9.CBO.Pub.Controller.CommonArchiveDataDTOData();
                    Bom_line.TransInWh.Code = item.MO.CompleteWh.Code;//调入存储地点
                    Bom_line.StoreUOMQty = item.ActualReqQty;//调入数量
                    Bom_line.CostUOMQty = item.ActualReqQty;//成本数量
                    Bom_line.TransInWh = new UFIDA.U9.CBO.Pub.Controller.CommonArchiveDataDTOData();
                    Bom_line.TransInWh.Code = item.MO.CompleteWh.Code;

                    Bom_line.CostUOM = new CommonArchiveDataDTOData();

                    Bom_line.CostUOM.Code = item1.CostUOM.Code;

                    Bom_line.ItemInfo = new ItemInfoData();

                    Bom_line.ItemInfo.ItemCode = item.ItemCode;

                    Bom_line.SysState = ObjectState.Inserted;

                    Bom_line.DescFlexSegments = new UFIDA.U9.Base.FlexField.DescFlexField.DescFlexSegmentsData();
                    Bom_line.DescFlexSegments.PrivateDescSeg1 = item.MO.DocNo;

                    Bom_line.TransInSubLines = new List<UFIDA.U9.ISV.TransferInISV.IC_TransInSubLineDTOData>();
                    //子行

                    List<UFIDA.U9.ISV.TransferInISV.IC_TransInSubLineDTOData> listBomSubline = new List<UFIDA.U9.ISV.TransferInISV.IC_TransInSubLineDTOData>();
                    UFIDA.U9.ISV.TransferInISV.IC_TransInSubLineDTOData Bom_subLine = new UFIDA.U9.ISV.TransferInISV.IC_TransInSubLineDTOData();

                    Bom_subLine.TransOutOrg = new UFIDA.U9.CBO.Pub.Controller.CommonArchiveDataDTOData();

                    Bom_subLine.TransOutWh = new UFIDA.U9.CBO.Pub.Controller.CommonArchiveDataDTOData();
                    Bom_subLine.TransOutWh.Code = "01";

                    Bom_subLine.StoreUOMQty = item.ActualReqQty;

                    listBomSubline.Add(Bom_subLine);//加载子行
                    Bom_line.TransInSubLines = listBomSubline;
                    listBomLine.Add(Bom_line);//加载行 

                    Bom.TransInLines = listBomLine;
                    listBom.Add(Bom);
                }
            }

            transferInSVProxy.TransferInDTOList = listBom;

            

            List<CommonArchiveDataDTOData> see = transferInSVProxy.Do();

            #endregion


            #endregion
        }
    }
}
