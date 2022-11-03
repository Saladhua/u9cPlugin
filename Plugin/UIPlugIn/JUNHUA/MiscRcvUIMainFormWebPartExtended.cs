using System;
using System.Data;
using System.Net.NetworkInformation;
using System.Reflection.Emit;
using UFIDA.U9.SCM.INV.MiscRcvUIModel;
using UFSoft.UBF.UI.Custom;
using UFSoft.UBF.UI.IView;
using UFSoft.UBF.UI.JModel;
using UFSoft.UBF.Util.DataAccess;

namespace YY.U9.Cust.JH.UIPlugIn
{
    /// <summary>
    /// 杂收
    /// </summary>
    class MiscRcvUIMainFormWebPartExtended : ExtendedPartBase
    {
        private MiscRcvUIMainFormWebPart _part;

        public override void BeforeEventProcess(IPart part, string eventName, object sender, EventArgs args, out bool executeDefault)
        {
            base.BeforeEventProcess(part, eventName, sender, args, out executeDefault);
            this._part = (part as MiscRcvUIMainFormWebPart);
            long n = (long)Math.Floor((new Random()).NextDouble() * 10000000D);
            foreach (var item in _part.Model.MiscRcvTrans_MiscRcvTransLs.Records)
            {
                string isLotControl = item["IsLotControl"].ToString();
                if (isLotControl == "False")
                {
                    return;
                }
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

                #region 料品分类为10和11下面改为手工录入不需自动录入
                //获取料品
                string itemmaster = item["ItemInfo_ItemID"].ToString();
                #region 
                DataTable dataTable = new DataTable();
                string sql = "select CBO_Category.Code from CBO_ItemMaster inner join CBO_Category on CBO_ItemMaster.MainItemCategory = CBO_Category.ID " +
                    "where CBO_ItemMaster.ID = '" + itemmaster + "'";
                DataSet dataSet = new DataSet();
                DataAccessor.RunSQL(DataAccessor.GetConn(), sql, null, out dataSet);
                dataTable = dataSet.Tables[0];
                string mainItemCategory = "";
                #endregion
                if (dataTable.Rows != null && dataTable.Rows.Count > 0)
                {

                    mainItemCategory = dataTable.Rows[0]["Code"].ToString().Substring(0, 2);

                }
                if (mainItemCategory == "10" || mainItemCategory == "11")
                {
                    return;
                }
                #endregion

                #region 长宽没有值的情况下，手工录入批次号，不调用开发功能
                if (string.IsNullOrEmpty(item["DescFlexSegments_PrivateDescSeg1"].ToString()) && string.IsNullOrEmpty(item["DescFlexSegments_PrivateDescSeg2"].ToString()) && string.IsNullOrEmpty(item["LotInfo_LotCode"].ToString()))
                {
                    return;
                }
                #endregion

                #region 判断批号主档不一致
                DataTable table_1 = new DataTable();
                string id = item["ID"].ToString();
                string lotcode_1 = item["LotInfo_LotCode"].ToString();
                string lotcode = "";
                string sql_1 = "SELECT LotInfo_LotCode FROM InvDoc_MiscRcvTransL WHERE ID='" + id + "'";
                DataSet dataSet_1 = new DataSet();
                DataAccessor.RunSQL(DataAccessor.GetConn(), sql_1, null, out dataSet_1);
                table_1 = dataSet_1.Tables[0];
                if (table_1.Rows != null && table_1.Rows.Count > 0)
                {
                    lotcode = table_1.Rows[0]["LotInfo_LotCode"].ToString();
                }
                if (lotcode != lotcode_1 && !string.IsNullOrEmpty(lotcode))
                {
                    throw new Exception("批号主档不一致");

                }
                #endregion
                string see = item["LotInfo_LotCode"].ToString();
                longs = string.IsNullOrEmpty(longs) ? "" : item["DescFlexSegments_PrivateDescSeg1"].ToString();
                wide = string.IsNullOrEmpty(wide) ? "" : item["DescFlexSegments_PrivateDescSeg2"].ToString();
                //string db = "InvDoc_MiscRcvTransL";
                //string dbname = "Lotinfo_lotcode";
                //item["LotInfo_LotCode"] = GetBatch(longs, wide, db, dbname);
                if (string.IsNullOrEmpty(longs) && (string.IsNullOrEmpty(wide)))
                {
                    item["LotInfo_LotCode"] = see;
                }
                else
                {
                    try
                    {
                        string newlotcode = item["LotInfo_LotCode"].ToString().Substring(0, 9);
                        string newlotcode_1 = newlotcode + "/" + wide + "*" + longs;
                        item["LotInfo_LotCode"] = newlotcode_1;

                    }
                    catch (Exception)
                    {
                        if (!string.IsNullOrEmpty(longs) || !string.IsNullOrEmpty(wide))
                        {
                            string db = "InvDoc_MiscRcvTransL";
                            string dbname = "Lotinfo_lotcode";
                            item["LotInfo_LotCode"] = GetBatch(longs, wide, db, dbname);
                        }
                    }
                }

            }
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
            int lotcode = 0;
            #region 
            DataTable dataTable = new DataTable();
            string valueSet = "SELECT TOP(1)  " + dbname + " FROM " + db + " WHERE " + dbname + " LIKE '" + time + "%' ORDER BY CreatedOn DESC";
            DataSet dataSet = new DataSet();
            DataAccessor.RunSQL(DataAccessor.GetConn(), valueSet, null, out dataSet);
            dataTable = dataSet.Tables[0];
            string code = "";
            bool state = false;
            if (dataTable.Rows != null && dataTable.Rows.Count > 0)
            {
                state = true;
                try
                {
                    lotcode = Convert.ToInt32(dataTable.Rows[0][dbname].ToString().Substring(6, 3));
                    lotcode = lotcode + 1;
                    code = lotcode.ToString("000");
                }
                catch (Exception)
                {
                    code = lotcode.ToString("000");
                    // throw;
                }
            }
            #endregion
            if (state == false)
            {
                code = lotcode.ToString("000");
            }
            string lotcode_info = time + code + "/" + item2 + "*" + item1;
            return lotcode_info;
        }

    }
}
