using System;
using System.Collections.Generic;
using UFIDA.U9.Base.DTOs;
using UFIDA.U9.CBO.Pub.Controller;
using UFIDA.U9.ISV.CBO.Lot;
using UFIDA.U9.ISV.CBO.Lot.Proxy;
using UFIDA.U9.Lot;
using UFIDA.U9.PM.ASN;
using UFSoft.UBF.Business;
using UFSoft.UBF.Util.Log;

namespace YY.U9.Cust.LI.AppPlugIn
{
    /// <summary>
    /// D:\setups\porject\恺之\2023-5-月底开发文档-恺之科技补充开发内容
    /// </summary>
    [UFSoft.UBF.Eventing.Configuration.Failfast]
    class ASNLineInsertedSubscriber : UFSoft.UBF.Eventing.IEventSubscriber
    {
        private static readonly ILogger logger = LoggerManager.GetLogger(typeof(PMPOLineInsertedSubscriber));
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
            ASN aSN = key.GetEntity() as ASN;
            if (aSN == null)
            {
                return;
            }
            #endregion

            string itemLotParam = "";

            //string itemLotDate = "";

            string nowDate = DateTime.Now.ToString("yyyyMMdd");

            string lotElotDate = "";//生效日期

            int ValidDate = 0;//有效期天数

            DateTime InvalidDate = DateTime.Now;//失效日期

            //long lotMasterID = 0;

            foreach (var item in aSN.ASNLine)
            {
                if (item.ItemInfo.ItemID.InventoryInfo.LotParam != null)
                {
                    itemLotParam = item.ItemInfo.ItemID.InventoryInfo.LotParam.Name;//批号参数
                    //itemLotDate = item.ItemInfo.ItemID.InventoryInfo.LotValidDate.ToString();//批号有效期天数
                    if (!string.IsNullOrEmpty(item.LotMater))
                    {
                        continue;
                    }
                    if (itemLotParam == "供应商批号（有效期管控）" || itemLotParam == "生产订单号批号-有效期")//有效期赋值
                    {
                        ValidDate = item.ItemInfo.ItemID.InventoryInfo.LotValidDate;
                        lotElotDate = nowDate;
                        InvalidDate = DateTime.Now.AddDays(ValidDate);
                        item.LotEnableDate = DateTime.Now;
                        item.LotDisabledDate = item.LotEnableDate.AddDays(ValidDate);
                        item.LotMater = nowDate;
                    }
                    else if (itemLotParam == "供应商批号" || itemLotParam == "生产订单号批号")//没有有效期赋值
                    {
                        //ValidDate = 0;
                        item.LotMater = nowDate;
                    }
                    else
                    {
                        return;
                    }

                }


                //CommonCreateLotMasterSRVProxy lotMasterSRV = new CommonCreateLotMasterSRVProxy();
                //List<CreateLotMasterDTOData> createLotMasterDTOData = new List<CreateLotMasterDTOData>();
                //CreateLotMasterDTOData createLot = new CreateLotMasterDTOData();
                //createLot.Item = new UFIDA.U9.CBO.Pub.Controller.CommonArchiveDataDTOData();
                //createLot.Item.Name = item.ItemInfo.ItemName;
                //createLot.Item.Code = item.ItemInfo.ItemCode;
                //createLot.UsedToOrg = new CommonArchiveDataDTOData();
                //createLot.LotCode = nowDate;
                //createLotMasterDTOData.Add(createLot);
                //lotMasterSRV.CreateLotMasterDTOList = createLotMasterDTOData;
                //List<IDCodeNameDTOData> see2222 = lotMasterSRV.Do();
                //foreach (var k in see2222)
                //{
                //    lotMasterID = k.ID;
                //}



                //if (ValidDate != 0)
                //{
                //    using (ISession session = Session.Open())
                //    {
                //        LotMaster lotMaster = LotMaster.Finder.FindByID(lotMasterID);
                //        if (lotMaster != null)
                //        {
                //            lotMaster.EffectiveDatetime = DateTime.Parse(lotElotDate);
                //            lotMaster.InvalidTime = DateTime.Parse(InvalidDate);
                //            lotMaster.ValidDate = ValidDate;
                //        }
                //        session.Commit();
                //    }
                //}
                #region 例子
                //CommonCreateLotMasterSRVProxy lotMasterSRV = new CommonCreateLotMasterSRVProxy();
                //List<CreateLotMasterDTOData> createLotMasterDTOData = new List<CreateLotMasterDTOData>();
                //CreateLotMasterDTOData createLot = new CreateLotMasterDTOData();
                //createLot.Item = new UFIDA.U9.CBO.Pub.Controller.CommonArchiveDataDTOData();
                //DataTable dataTable = new DataTable();
                //createLot.Item.Name = item["ItemInfo_ItemName"].ToString();
                //createLot.Item.Code = item["ItemInfo_ItemID"].ToString();
                //createLot.UsedToOrg = new CommonArchiveDataDTOData();
                //createLot.LotCode = lotcode;
                //createLotMasterDTOData.Add(createLot);
                //lotMasterSRV.CreateLotMasterDTOList = createLotMasterDTOData;
                //List<IDCodeNameDTOData> see2222 = lotMasterSRV.Do();
                //foreach (var k in see2222)
                //{
                //    item["LotInfo_LotMaster"] = k.ID;
                //}
                #endregion
            }
        }
    }
}
