using System;
using System.Collections.Generic;
using System.Data;
using UFIDA.U9.Base.DTOs;
using UFIDA.U9.InvDoc.MiscRcv;
using UFIDA.U9.ISV.CBO.Lot;
using UFIDA.U9.ISV.CBO.Lot.Proxy;
using UFIDA.U9.Lot;
using UFSoft.UBF.Business;
using UFSoft.UBF.Util.DataAccess;
using UFSoft.UBF.Util.Log;

namespace YY.U9.Cust.LI.AppPlugIn
{
    /// <summary>
    /// 杂发单--新增
    /// 成品入库单-批次单重计算
    /// </summary>
    [UFSoft.UBF.Eventing.Configuration.Failfast]

    class MiscShipmentInsertedSubscriber : UFSoft.UBF.Eventing.IEventSubscriber
    {

        private static readonly ILogger logger = LoggerManager.GetLogger(typeof(MiscShipmentInsertedSubscriber));
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
            MiscRcvTrans miscRcvTrans = key.GetEntity() as MiscRcvTrans;
            if (miscRcvTrans == null)
            {
                return;
            }

            //直接循环遍历
            foreach (var item in miscRcvTrans.MiscRcvTransLs)
            {
                if (!item.Wh.IsLot)
                {
                    return;
                }
                #region
                string itemmaster = item.ItemInfo.ItemID.ID.ToString();
                #region 
                DataTable dataTable_1 = new DataTable();
                string sql_1 = "select CBO_Category.Code from CBO_ItemMaster inner join CBO_Category on CBO_ItemMaster.MainItemCategory = CBO_Category.ID " +
                    "where CBO_ItemMaster.ID = '" + itemmaster + "'";
                DataSet dataSet_1 = new DataSet();
                DataAccessor.RunSQL(DataAccessor.GetConn(), sql_1, null, out dataSet_1);
                dataTable_1 = dataSet_1.Tables[0];
                string mainItemCategory = "";
                if (dataTable_1.Rows != null && dataTable_1.Rows.Count > 0)
                {
                    mainItemCategory = dataTable_1.Rows[0]["Code"].ToString().Substring(0, 2);
                }
                if (mainItemCategory == "10" || mainItemCategory == "11")
                {
                    return;
                }
                #endregion
                #endregion
                //item.DescFlexSegments.PrivateDescSeg2 = item.DescFlexSegments.PrivateDescSeg5;
                //item.DescFlexSegments.PrivateDescSeg3= item.DescFlexSegments.PrivateDescSeg6;
                string longs = string.IsNullOrEmpty(item.DescFlexSegments.PrivateDescSeg1) ? "" : item.DescFlexSegments.PrivateDescSeg1;
                string wide = string.IsNullOrEmpty(item.DescFlexSegments.PrivateDescSeg2) ? "" : item.DescFlexSegments.PrivateDescSeg2;
                #region 长宽没有值的情况下，手工录入批次号，不调用开发功能
                if (!string.IsNullOrEmpty(longs) && !string.IsNullOrEmpty(wide))
                {
                    return;
                }
                #endregion
                //item.LotInfo.LotMaster.LotCode = GetBatch(longs, wide);
                string db = "InvDoc_MiscRcvTransL";
                string dbname = "Lotinfo_lotcode";
                string newlotcode = GetBatch(longs, wide, db, dbname);
                #region 通过创建bp的方式创建批号
                CommonCreateLotMasterSRVProxy lotMasterSRV = new CommonCreateLotMasterSRVProxy();
                List<CreateLotMasterDTOData> createLotMasterDTOData = new List<CreateLotMasterDTOData>();
                CreateLotMasterDTOData createLot = new CreateLotMasterDTOData();
                createLot.Item = new UFIDA.U9.CBO.Pub.Controller.CommonArchiveDataDTOData();
                createLot.Item.ID = item.ItemInfo.ItemID.ID;//(long)item["ItemInfo_ItemID"];
                createLot.Item.Name = item.ItemInfo.ItemName;//item["ItemInfo_ItemName"].ToString();
                createLot.Item.Code = item.ItemInfo.ItemCode;//item["ItemInfo_ItemID"].ToString();
                createLot.LotCode = newlotcode;//item["LotInfo_LotCode"].ToString();
                createLotMasterDTOData.Add(createLot);
                lotMasterSRV.CreateLotMasterDTOList = createLotMasterDTOData;
                //lotMasterSRV.Do();
                List<IDCodeNameDTOData> see2222 = lotMasterSRV.Do();
                foreach (var k in see2222)
                {
                    item.LotInfo.LotMaster.ID = k.ID;
                }
                item.LotInfo.LotCode = newlotcode;

                #endregion
                string kg = "";
                #region  根据长宽进行赋值计算KG
                #region sql语句执行 kg 赋值
                DataTable dataTable = new DataTable();
                string sql = "SELECT DescFlexField_PrivateDescSeg6 FROM cbo_ItemMaster WHERE ID='" + item.ItemInfo.ItemID.ID + "'";
                DataSet dataSet = new DataSet();
                DataAccessor.RunSQL(DataAccessor.GetConn(), sql, null, out dataSet);
                dataTable = dataSet.Tables[0];
                if (dataTable.Rows != null && dataTable.Rows.Count > 0)
                {
                    kg = dataTable.Rows[0]["DescFlexField_PrivateDescSeg6"].ToString();
                    if (string.IsNullOrEmpty(kg))
                        kg = "0";
                }
                if (!string.IsNullOrEmpty(longs) && !string.IsNullOrEmpty(wide))//都有值
                {
                    kg = Math.Round((decimal.Parse(longs) / 1000 * decimal.Parse(wide) / 1000) * decimal.Parse(kg), 3).ToString();
                }
                else if (!string.IsNullOrEmpty(longs) && string.IsNullOrEmpty(wide))//长有值，宽无值
                {
                    kg = Math.Round(decimal.Parse(longs) / 1000 * decimal.Parse(kg), 3).ToString();
                }
                else if (string.IsNullOrEmpty(longs) && !string.IsNullOrEmpty(wide))//宽有值，长无值
                {
                    kg = Math.Round(decimal.Parse(wide) / 1000 * decimal.Parse(kg), 3).ToString();
                }
                else
                {
                    kg = "0";
                }
                #endregion
                #region 使用session的方式modelfind去修改
                LotMaster lotMaster = null;
                using (UFSoft.UBF.Business.ISession session = Session.Open())
                {
                    lotMaster = LotMaster.Finder.FindByID(item.LotInfo.LotMaster.ID);
                    lotMaster.LotCode = newlotcode;
                    lotMaster.DescFlexSegments.PrivateDescSeg1 = longs;
                    lotMaster.DescFlexSegments.PrivateDescSeg2 = wide;
                    //lotMaster.DescFlexSegments.PrivateDescSeg3 = kg;
                    session.Modify(lotMaster);
                    session.Commit();
                }
                #endregion

                #endregion
            }


            #endregion

        }


        /// <summary>
        /// 生产批号
        /// </summary>
        /// <param name="item1">长</param>
        /// <param name="item2">宽</param>
        /// <param name="db">表名</param>
        /// <param name="dbname">表的字段</param>
        /// <returns></returns>
        public static string GetBatch(string item1, string item2, string db, string dbname)
        {
            //获取当前日期
            string time = DateTime.Now.ToString("yyMMdd");
            //SELECT TOP(1) Lotinfo_lotcode FROM InvDoc_MiscShipL WHERE Lotinfo_lotcode LIKE '20220822%' ORDER BY CreatedOn DESC
            int lotcode = 1;
            DataTable dataTable = new DataTable();
            string valueSet = "SELECT TOP(1)  " + dbname + " FROM " + db + " WHERE " + dbname + " LIKE '" + time + "%' ORDER BY CreatedOn DESC";
            DataSet dataSet = new DataSet();
            DataAccessor.RunSQL(DataAccessor.GetConn(), valueSet, null, out dataSet);
            dataTable = dataSet.Tables[0];
            if (dataTable.Rows != null && dataTable.Rows.Count > 0)
            {
                lotcode = Convert.ToInt32(dataTable.Rows[0][dbname].ToString().Substring(6, 3));
                lotcode = lotcode + 1;
            }
            string code = lotcode.ToString("000");
            string lotcode_info = time + code + "/" + item2 + "*" + item1;
            return lotcode_info;
        }

    }
}


