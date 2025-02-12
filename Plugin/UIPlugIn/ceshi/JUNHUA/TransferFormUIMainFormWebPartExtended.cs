using System;
using System.Collections.Generic;
using System.Data;
using UFIDA.U9.Base.DTOs;
using UFIDA.U9.CBO.Pub.Controller;
using UFIDA.U9.ISV.CBO.Lot;
using UFIDA.U9.ISV.CBO.Lot.Proxy;
using UFIDA.U9.Lot;
using UFIDA.U9.Lot.Proxy;
using UFIDA.U9.SCM.INV.TransferFormUIModel;
using UFIDA.U9.UI.PDHelper;
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
            string doctypeid = "";
            string doccode = "";
            foreach (var item in _part.Model.TransferForm.Records)//新建个单据类型，根据单据类型区分 那种的话手工让他们修改批次号  就不用你自己生成了
            {
                doctypeid = item["TransferFormDocType"] == null ? "" : item["TransferFormDocType"].ToString();
            }
            if (!string.IsNullOrEmpty(doctypeid))
            {
                doccode = findDocTypeCode(doctypeid);
                if (doccode == "TransForm006")
                {
                    return;
                }
            }
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


                    string whid = item["WH"].ToString();

                    string sql = "select IsLot from CBO_Wh where ID='" + whid + "'";

                    DataTable dt = U9Common.GetDataTable(sql);

                    string IsLot = "False";

                    if (dt.Rows != null && dt.Rows.Count > 0)
                    {
                        IsLot = dt.Rows[0]["IsLot"].ToString();
                    }
                    if (IsLot == "False")
                    {
                        return;
                    }
 
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
                        DataTable dataTable2 = new DataTable();
                        string sql_2 = "SELECT DescFlexField_PrivateDescSeg6 FROM cbo_ItemMaster WHERE ID='" + item_id + "'";
                        DataSet dataSet = new DataSet();
                        DataAccessor.RunSQL(DataAccessor.GetConn(), sql_2, null, out dataSet);
                        dataTable2 = dataSet.Tables[0];
                        bool ok = false;
                        if (dataTable2.Rows != null && dataTable2.Rows.Count > 0)
                        {
                            kg = dataTable2.Rows[0]["DescFlexField_PrivateDescSeg6"].ToString();
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
                            //进行赋值操作
                            try
                            {
                                string error = item["DescFlexSegments_PrivateDescSeg3"].ToString();
                                if (error == "")
                                {
                                    item["DescFlexSegments_PrivateDescSeg3"] = kg;
                                }
                                else
                                {
                                    //没有异常说明不是第一进入进行对比
                                    if (error != kg)
                                    {
                                        kg = error;
                                    }
                                    item["DescFlexSegments_PrivateDescSeg3"] = kg;
                                }
                                //if (string.IsNullOrEmpty(error))
                                //{
                                //    item["DescFlexSegments_PrivateDescSeg3"] = error;
                                //    kg = error;
                                //}
                            }
                            catch (Exception)
                            {
                                //说明是第一次进入对其赋值
                                item["DescFlexSegments_PrivateDescSeg3"] = kg;
                            }
                        }
                        #endregion
                    }
                    else
                    {
                        return;
                    }
                    #endregion

                    #region 测试
                    //UFIDA.U9.Lot.Proxy.BatchCreateLotMasterProxy masterProxy = new UFIDA.U9.Lot.Proxy.BatchCreateLotMasterProxy();
                    //List<LotMasterCreateDTOData> lotMasterCreateDTODatas = new List<LotMasterCreateDTOData>();
                    //LotMasterCreateDTOData lotMasterCreateDTOData = new LotMasterCreateDTOData();
                    //lotMasterCreateDTOData.ItemMaster = (long)item["ItemInfo_ItemID"];
                    //lotMasterCreateDTOData.LotCode = lotcode;
                    //lotMasterCreateDTOData.UsingOrg = 1002206140000035;
                    //lotMasterCreateDTOData.UsingOrg_SKey = new UFSoft.UBF.Business.BusinessEntity.EntityKey();
                    //lotMasterCreateDTOData.UsingOrg_SKey.ID = 1002206140000035;
                    //masterProxy.TargetOrgName = "江苏君华特种工程塑料制品有限公司";
                    //masterProxy.TargetOrgCode = "10";
                    //lotMasterCreateDTODatas.Add(lotMasterCreateDTOData);
                    //masterProxy.LotMasterCreateDTOS = lotMasterCreateDTODatas;
                    //List<LotMasterInfoData> see2222 = masterProxy.Do();
                    //foreach (var k in see2222)
                    //{
                    //    item["LotInfo_LotMaster"] = k.LotMaster;
                    //}
                    #endregion

                    CommonCreateLotMasterSRVProxy lotMasterSRV = new CommonCreateLotMasterSRVProxy();
                    List<CreateLotMasterDTOData> createLotMasterDTOData = new List<CreateLotMasterDTOData>();
                    CreateLotMasterDTOData createLot = new CreateLotMasterDTOData();
                    createLot.Item = new UFIDA.U9.CBO.Pub.Controller.CommonArchiveDataDTOData();
                    DataTable dataTable = new DataTable();
                    string itemfor10 = "";
                    if (PDContext.Current.OrgID != "1002206140000035")
                    {
                        string sqlFor = "SELECT ID FROM CBO_ItemMaster WHERE Name='" + item["ItemInfo_ItemName"].ToString() + "' AND Org = '1002206140000035' AND SPECS='" + item["ItemInfo_ItemID_SPECS"].ToString() + "'";
                        DataSet dataSet = new DataSet();
                        DataAccessor.RunSQL(DataAccessor.GetConn(), sqlFor, null, out dataSet);
                        dataTable = dataSet.Tables[0];
                        if (dataTable != null && dataTable.Rows.Count > 0)
                        {
                            itemfor10 = dataTable.Rows[0]["ID"].ToString();
                        }
                        createLot.Item.ID = Int64.Parse(itemfor10);
                    }
                    else
                    {
                        createLot.Item.ID = (long)item["ItemInfo_ItemName"];
                    }
                    createLot.Item.Name = item["ItemInfo_ItemName"].ToString();
                    createLot.Item.Code = item["ItemInfo_ItemID"].ToString();
                    createLot.UsedToOrg = new CommonArchiveDataDTOData();
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
                catch (Exception e)
                {
                    item["LotInfo_LotMaster"] = n;
                    longs = string.IsNullOrEmpty(longs) ? "" : item["DescFlexSegments_PrivateDescSeg1"].ToString();
                    wide = string.IsNullOrEmpty(wide) ? "" : item["DescFlexSegments_PrivateDescSeg2"].ToString();
                    //string db = "InvDoc_TransferFormSL";
                    //string dbname = "Lotinfo_lotcode";
                    //item["LotInfo_LotCode"] = MiscRcvUIMainFormWebPartExtended.GetBatch(longs, wide, db, dbname);
                    throw new Exception(e.Message);
                }
                string see2 = item["LotInfo_LotCode"].ToString();
            }
        }

        /// <summary>
        /// 通过单据类型的id查到对应的code
        /// </summary>
        /// <param name="doctypeid"></param>
        /// <returns></returns>
        public string findDocTypeCode(string doctypeid)
        {
            string doccode = "";
            DataTable dataTable_1 = new DataTable();
            string sql = "select Code from InvDoc_TransferFormDocType where ID='" + doctypeid + "'";
            DataSet dataSet_1 = new DataSet();
            DataAccessor.RunSQL(DataAccessor.GetConn(), sql, null, out dataSet_1);
            dataTable_1 = dataSet_1.Tables[0];
            if (dataTable_1.Rows != null && dataTable_1.Rows.Count > 0)
            {
                doccode = dataTable_1.Rows[0]["Code"].ToString();
            }
            return doccode;
        }

    }
}
