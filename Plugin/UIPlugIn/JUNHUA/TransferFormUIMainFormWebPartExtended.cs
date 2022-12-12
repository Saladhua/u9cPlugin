using System;
using System.Collections.Generic;
using System.Data;
using System.Threading;
using UFIDA.U9.Base.DTOs;
using UFIDA.U9.Base.WorkCalendar;
using UFIDA.U9.ISV.CBO.Lot;
using UFIDA.U9.ISV.CBO.Lot.Proxy;
using UFIDA.U9.SCM.INV.TransferFormUIModel;
using UFSoft.UBF.UI.Custom;
using UFSoft.UBF.UI.IView;
using UFSoft.UBF.Util.DataAccess;

namespace YY.U9.Cust.LI.UIPlugIn
{
    /// <summary>
    /// 形态转换
    /// </summary>
    class TransferFormUIMainFormWebPartExtended : ExtendedPartBase
    {
        private TransferFormUIMainFormWebPart _part;
        /// <summary>
        /// 批号通过sv服务生成
        /// </summary>
        /// <param name="part"></param>
        /// <param name="args"></param>
        public override void AfterRender(IPart part, EventArgs args)
        {
            base.AfterRender(part, args);
            this._part = (part as TransferFormUIMainFormWebPart);
            long n = (long)Math.Floor((new Random()).NextDouble() * 10000000D);
            try
            {
                _part.Model.TransferForm_TransferFormLs_TransferFormSLs.Records.ToString();
            }
            catch (Exception)
            {
                return;
                throw;
            }
            List<string> lotinfo = new List<string>();
            List<string> dp1code = new List<string>();
            List<string> dp2code = new List<string>();
            List<string> kgcode = new List<string>();
            foreach (var item in _part.Model.TransferForm_TransferFormLs.Records)
            {
                string lotinfocode = item["LotInfo_LotCode"] == null ? "" : item["LotInfo_LotCode"].ToString();
                string dp1 = item["DescFlexSegments_PrivateDescSeg1"] == null ? "" : item["DescFlexSegments_PrivateDescSeg1"].ToString();
                string dp2 = item["DescFlexSegments_PrivateDescSeg2"] == null ? "" : item["DescFlexSegments_PrivateDescSeg2"].ToString();
                string kgs = item["DescFlexSegments_PrivateDescSeg3"] == null ? "" : item["DescFlexSegments_PrivateDescSeg3"].ToString();
                lotinfo.Add(lotinfocode);
                dp1code.Add(dp1);
                dp2code.Add(dp2);
                kgcode.Add(kgs);
            }
            foreach (var item in _part.Model.TransferForm_TransferFormLs_TransferFormSLs.Records)
            {
                string longs = "";
                string wide = "";
                #region 之前的
                //try
                //{
                //    longs = item["DescFlexSegments_PrivateDescSeg1"].ToString();
                //}
                //catch (Exception)
                //{
                //    longs = "";
                //}
                //try
                //{
                //    wide = item["DescFlexSegments_PrivateDescSeg2"].ToString();
                //}
                //catch (Exception)
                //{
                //    wide = "";
                //}
                //if (string.IsNullOrEmpty(longs) && string.IsNullOrEmpty(wide))
                //{
                //    return;
                //}
                #endregion
                int pLineNo = (Convert.ToInt32(item["PLineNo"].ToString()) / 10) - 1;
                try
                {
                    #region 之前的
                    //string see = item["LotInfo_LotCode"].ToString();
                    string item_id = item["ItemInfo_ItemID"].ToString();
                    ////string see1 = item["LotInfo_LotMaster"].ToString();
                    string longsf = item["DescFlexSegments_PrivateDescSeg1"] == null ? "" : item["DescFlexSegments_PrivateDescSeg1"].ToString();
                    string widef = item["DescFlexSegments_PrivateDescSeg2"] == null ? "" : item["DescFlexSegments_PrivateDescSeg2"].ToString();
                    //string db = "InvDoc_TransferFormSL";
                    //string dbname = "Lotinfo_lotcode";
                    //item["LotInfo_LotCode"] = MiscRcvUIMainFormWebPartExtended.GetBatch(longs, wide, db, dbname);
                    #endregion

                    string itemTrans = item["TransferFormL"].ToString();

                    //批次号
                    string lotcode = "";
                    //单重
                    string kg = "";

                    #region 查询转换前的批次号和长宽
                    //DataTable dataTable_1 = new DataTable();
                    //string sql = "select LotCode,DescFlexSegments_PrivateDescSeg1,DescFlexSegments_PrivateDescSeg2,DescFlexSegments_PrivateDescSeg3  " +
                    //    " from Lot_LotMaster where ID=(select LotInfo_LotMaster from InvDoc_TransferFormL where ID='" + itemTrans + "')";
                    //DataSet dataSet_1 = new DataSet();
                    //DataAccessor.RunSQL(DataAccessor.GetConn(), sql, null, out dataSet_1);
                    //dataTable_1 = dataSet_1.Tables[0];
                    //if (dataTable_1.Rows != null && dataTable_1.Rows.Count > 0)
                    //{
                    //    lotcode = dataTable_1.Rows[0]["LotCode"].ToString();
                    //    wide = dataTable_1.Rows[0]["DescFlexSegments_PrivateDescSeg2"].ToString();
                    //    longs = dataTable_1.Rows[0]["DescFlexSegments_PrivateDescSeg1"].ToString();
                    //    kg = dataTable_1.Rows[0]["DescFlexSegments_PrivateDescSeg1"].ToString();
                    //}
                    //else
                    //{
                    lotcode = lotinfo[pLineNo];
                    if (!string.IsNullOrEmpty(longs) || !string.IsNullOrEmpty(wide))
                    {
                        longs = dp1code[pLineNo];
                        wide = dp2code[pLineNo];
                        kg = kgcode[pLineNo];
                    }
                    //}
                    if (lotcode.Contains("/"))
                    {
                        lotcode = lotcode.Substring(0, lotcode.IndexOf("/"));
                    }
                    if (!string.IsNullOrEmpty(longsf) || !string.IsNullOrEmpty(widef))
                    {

                        lotcode = lotcode + "/" + widef + "*" + longsf;
                        wide = widef;
                        longs = longsf;
                        #region 单中的计算 -- 不计算单重直接取
                        DataTable dataTable = new DataTable();
                        string sql_2 = "SELECT DescFlexField_PrivateDescSeg6 FROM cbo_ItemMaster WHERE ID='" + item_id + "'";
                        DataSet dataSet = new DataSet();
                        DataAccessor.RunSQL(DataAccessor.GetConn(), sql_2, null, out dataSet);
                        dataTable = dataSet.Tables[0];
                        bool ok = false;
                        if (dataTable.Rows != null && dataTable.Rows.Count > 0)
                        {
                            kg = dataTable.Rows[0]["DescFlexField_PrivateDescSeg6"].ToString();
                            if (string.IsNullOrEmpty(kg))
                                ok = true;
                        }
                        if (ok == false)
                        {
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
                        }
                        #endregion
                    }
                    #endregion
                    CommonCreateLotMasterSRVProxy lotMasterSRV = new CommonCreateLotMasterSRVProxy();
                    List<CreateLotMasterDTOData> createLotMasterDTOData = new List<CreateLotMasterDTOData>();
                    CreateLotMasterDTOData createLot = new CreateLotMasterDTOData();
                    createLot.Item = new UFIDA.U9.CBO.Pub.Controller.CommonArchiveDataDTOData();
                    createLot.Item.ID = (long)item["ItemInfo_ItemID"];
                    createLot.Item.Name = item["ItemInfo_ItemName"].ToString();
                    createLot.Item.Code = item["ItemInfo_ItemID"].ToString();
                    createLot.LotCode = lotcode;
                    createLotMasterDTOData.Add(createLot);
                    lotMasterSRV.CreateLotMasterDTOList = createLotMasterDTOData;
                    //lotMasterSRV.Do();
                    List<IDCodeNameDTOData> see2222 = lotMasterSRV.Do();
                    foreach (var k in see2222)
                    {
                        item["LotInfo_LotMaster"] = k.ID;
                    }
                    item["InureDate"] = DateTime.Now.ToString("yyyy.MM.dd");
                    item["LotInfo_DisabledDatetime"] = DateTime.Now.AddDays(+90).ToString("yyyy.MM.dd");
                    item["LotInfo_LotCode"] = lotcode;

                    #region 单中的计算 -- 不计算单重直接取
                    // string kg = "";
                    // DataTable dataTable = new DataTable();
                    // //string sql = "SELECT DescFlexField_PrivateDescSeg6 FROM cbo_ItemMaster WHERE ID='" + item_id + "'";
                    // DataSet dataSet = new DataSet();
                    //// DataAccessor.RunSQL(DataAccessor.GetConn(), sql, null, out dataSet);
                    // dataTable = dataSet.Tables[0];
                    // if (dataTable.Rows != null && dataTable.Rows.Count > 0)
                    // {
                    //     kg = dataTable.Rows[0]["DescFlexField_PrivateDescSeg6"].ToString();
                    //     if (string.IsNullOrEmpty(kg))
                    //         kg = "0";
                    // }
                    // if (!string.IsNullOrEmpty(longs) && !string.IsNullOrEmpty(wide))//都有值
                    // {
                    //     kg = Math.Round((decimal.Parse(longs) / 1000 * decimal.Parse(wide) / 1000) * decimal.Parse(kg), 3).ToString();
                    // }
                    // else if (!string.IsNullOrEmpty(longs) && string.IsNullOrEmpty(wide))//长有值，宽无值
                    // {
                    //     kg = Math.Round(decimal.Parse(longs) / 1000 * decimal.Parse(kg), 3).ToString();
                    // }
                    // else if (string.IsNullOrEmpty(longs) && !string.IsNullOrEmpty(wide))//宽有值，长无值
                    // {
                    //     kg = Math.Round(decimal.Parse(wide) / 1000 * decimal.Parse(kg), 3).ToString();
                    // }
                    // else
                    // {
                    //     kg = "0";
                    // }
                    #endregion
                    string update1 = "UPDATE Lot_LotMaster SET DescFlexSegments_PrivateDescSeg1 = '" + longs + "', DescFlexSegments_PrivateDescSeg2 = '" + wide + "', DescFlexSegments_PrivateDescSeg3 = '" + kg + "' WHERE id = '" + item["LotInfo_LotMaster"] + "'";
                    DataAccessor.RunSQL(DataAccessor.GetConn(), update1, null);

                }
                catch (Exception)
                {
                    item["LotInfo_LotMaster"] = n;
                    longs = string.IsNullOrEmpty(longs) ? "" : item["DescFlexSegments_PrivateDescSeg1"].ToString();
                    wide = string.IsNullOrEmpty(wide) ? "" : item["DescFlexSegments_PrivateDescSeg2"].ToString();
                    string db = "InvDoc_TransferFormSL";
                    string dbname = "Lotinfo_lotcode";
                    //item["LotInfo_LotCode"] = MiscRcvUIMainFormWebPartExtended.GetBatch(longs, wide, db, dbname);
                }
                string see2 = item["LotInfo_LotCode"].ToString();

            }
        }

    }
}
