using System;
using System.Collections;
using System.Data;
using UFIDA.U9.ISV.CBO.Lot.Proxy;
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
            foreach (var item in _part.Model.TransferForm_TransferFormLs_TransferFormSLs.Records)
            {
                string longs = "";
                string wide = "";
                try
                {
                    longs = item["DescFlexSegments_PrivateDescSeg1"].ToString();
                }
                catch (Exception)
                {
                    longs = "";
                }
                try
                {
                    wide = item["DescFlexSegments_PrivateDescSeg2"].ToString();
                }
                catch (Exception)
                {
                    wide = "";
                }
                if (string.IsNullOrEmpty(longs) && string.IsNullOrEmpty(wide))
                {
                    return;
                }
                try
                {
                    //string see = item["LotInfo_LotCode"].ToString();
                    //string item_id = item["ItemInfo_ItemID"].ToString();
                    ////string see1 = item["LotInfo_LotMaster"].ToString();
                    //longs = string.IsNullOrEmpty(longs) ? "" : item["DescFlexSegments_PrivateDescSeg1"].ToString();
                    //wide = string.IsNullOrEmpty(wide) ? "" : item["DescFlexSegments_PrivateDescSeg2"].ToString();
                    //string db = "InvDoc_TransferFormSL";
                    //string dbname = "Lotinfo_lotcode";
                    //item["LotInfo_LotCode"] = MiscRcvUIMainFormWebPartExtended.GetBatch(longs, wide, db, dbname);
                    string 





                    CommonCreateLotMasterSRVProxy lotMasterSRV = new CommonCreateLotMasterSRVProxy();
                    List<CreateLotMasterDTOData> createLotMasterDTOData = new List<CreateLotMasterDTOData>();
                    CreateLotMasterDTOData createLot = new CreateLotMasterDTOData();
                    createLot.Item = new UFIDA.U9.CBO.Pub.Controller.CommonArchiveDataDTOData();
                    createLot.Item.ID = (long)item["ItemInfo_ItemID"];
                    createLot.Item.Name = item["ItemInfo_ItemName"].ToString();
                    createLot.Item.Code = item["ItemInfo_ItemID"].ToString();
                    createLot.LotCode = item["LotInfo_LotCode"].ToString();
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

                    #region 单中的计算
                    string kg = "";
                    DataTable dataTable = new DataTable();
                    string sql = "SELECT DescFlexField_PrivateDescSeg6 FROM cbo_ItemMaster WHERE ID='" + item_id + "'";
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
                    item["LotInfo_LotCode"] = MiscRcvUIMainFormWebPartExtended.GetBatch(longs, wide, db, dbname);
                }
                string see2 = item["LotInfo_LotCode"].ToString();

            }
        }

    }

}
