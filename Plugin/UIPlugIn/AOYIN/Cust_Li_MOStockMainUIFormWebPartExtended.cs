using System;
using System.Collections.Generic;
using System.Data;
using UFIDA.U9.ISV.MO;
using UFIDA.U9.ISV.MO.Proxy;
using UFIDA.U9.MFG.MO.DiscreteMOUIModel;
using UFIDA.U9.MO.MO.Proxy;
using UFIDA.U9.UI.PDHelper;
using UFSoft.UBF.UI.Custom;
using UFSoft.UBF.UI.IView;
using UFSoft.UBF.Util.DataAccess;

namespace YY.U9.Cust.AY.UIPlugIn
{
    /// <summary>
    /// 生产订单
    /// </summary>
    internal class Cust_Li_MOStockMainUIFormWebPartExtended : ExtendedPartBase
    {
        private MOStockMainUIFormWebPart _part;
        /// <summary>
        /// 进行赋值操作
        /// </summary>
        /// <param name="part"></param>
        /// <param name="args"></param>
        public override void BeforeRender(IPart part, EventArgs args)
        {
            base.BeforeRender(part, args);
            this._part = (part as MOStockMainUIFormWebPart);
            //获取到当前的生产订单
            foreach (var item in _part.Model.MO_MOPickLists.Records)
            {
                //生产订单的单号
                string moDocNo = item["MO_DocNo"].ToString();
                //该生产订单的id
                string moDocNoID = "";
                //报废入库数量
                string scrapping = "";
                //入库数量
                string rcvQtyByProductUom = "";
                //BOM用量 BOMReqQty
                string bomReqQty = item["BOMComponent_UsageQty"].ToString();
                //精度
                string roundPrecision = "";
                //差异
                double difference = 0;
                //当前组织
                string org = PDContext.Current.OrgID;
                #region  通过单号去查杂收单已经审核的报废入库数量(他是一个私有字段),并且杂收单弃审需要反写修改累计报废入库数量
                DataTable dataTable = new DataTable();
                string sqlForZS = "SELECT b.DescFlexSegments_PrivateDescSeg2,b.MoDocNo,a.Status" +
                    " FROM InvDoc_MiscRcvTransL b " +
                    " INNER JOIN InvDoc_MiscRcvTrans a " +
                    " ON a.ID = b.MiscRcvTrans WHERE MoDocNo = '" + moDocNo + "' AND a.Status = 2";
                DataSet dataSet = new DataSet();
                DataAccessor.RunSQL(DataAccessor.GetConn(), sqlForZS, null, out dataSet);
                dataTable = dataSet.Tables[0];
                if (dataTable != null && dataTable.Rows.Count > 0)
                {
                    scrapping = dataTable.Rows[0]["DescFlexSegments_PrivateDescSeg2"].ToString();
                }//如果报废入库数量审核状态查出来没有,那么就可能是弃审的时候要看scrapping这个东西是不是不为0
                else
                {
                    string sqlForZS_1 = "SELECT b.DescFlexSegments_PrivateDescSeg2,b.MoDocNo,a.Status" +
                        " FROM InvDoc_MiscRcvTransL b " +
                        " INNER JOIN InvDoc_MiscRcvTrans a " +
                        " ON a.ID = b.MiscRcvTrans WHERE MoDocNo = '" + moDocNo + "' AND a.Status = 0";
                    DataAccessor.RunSQL(DataAccessor.GetConn(), sqlForZS_1, null, out dataSet);
                    dataTable = dataSet.Tables[0];
                    if (dataTable != null && dataTable.Rows.Count > 0)
                    {
                        scrapping = dataTable.Rows[0]["DescFlexSegments_PrivateDescSeg2"].ToString();
                    }
                }
                #region 
                #endregion
                #endregion
                #region 获取当前生产订单单号的id
                string sqlForMoDocNoID = "SELECT ID FROM MO_MO WHERE DocNo='" + moDocNo + "'";
                DataAccessor.RunSQL(DataAccessor.GetConn(), sqlForMoDocNoID, null, out dataSet);
                dataTable = dataSet.Tables[0];
                if (dataTable != null && dataTable.Rows.Count > 0)
                {
                    moDocNoID = dataTable.Rows[0]["ID"].ToString();
                }
                #endregion
                //scrapping = "123";
                if (!string.IsNullOrEmpty(scrapping))
                {
                    CompleteMoProxy complete = new CompleteMoProxy();
                    List<MOOperateParamDTOData> mOOperates = new List<MOOperateParamDTOData>();
                    MOOperateParamDTOData mOOperate = new MOOperateParamDTOData();
                    mOOperate.MODocNo = moDocNo;
                    mOOperate.OperateType = true;
                    mOOperate.OperateResult = true;
                    mOOperates.Add(mOOperate);
                    complete.MOOperateParamDTOs = mOOperates;
                    List<MOOperateParamDTOData> see2222 = complete.Do();
                }
                item["DescFlexField_PrivateDescSeg9"] = scrapping;
                #region 四舍五入
                //入库数量就是完工报告的完工数量--完工数量要求和
                //SELECT sum(a.RcvQtyByProductUOM) RcvQtyByProductUOM FROM MO_CompleteRpt a 
                //INNER JOIN MO_MO b ON a.MO = b.ID
                //WHERE b.DocNo = 'SZ4813222101'
                string sqlForRBPUOM = "SELECT sum(a.RcvQtyByProductUOM) RcvQtyByProductUOM FROM MO_CompleteRpt a " +
                " INNER JOIN MO_MO b ON a.MO = b.ID WHERE b.DocNo = '" + moDocNo + "'";
                DataAccessor.RunSQL(DataAccessor.GetConn(), sqlForZS, null, out dataSet);
                dataTable = dataSet.Tables[0];
                if (dataTable != null && dataTable.Rows.Count > 0)
                {
                    rcvQtyByProductUom = dataTable.Rows[0]["RcvQtyByProductUOM"].ToString();
                }
                //2.取料品上面的精度
                //料品id
                //SELECT Round_Precision FROM CBO_ItemMaster b
                //INNER JOIN Base_UOM a ON a.ID = b.InventoryUOM
                //WHERE b.Code = '111'
                string sqlForPre = "SELECT Round_Precision FROM CBO_ItemMaster b " +
                    " INNER JOIN Base_UOM a ON a.ID = b.InventoryUOM WHERE b.Code = '" + item["ItemMaster_Code"].ToString() + "'";
                DataAccessor.RunSQL(DataAccessor.GetConn(), sqlForZS, null, out dataSet);
                dataTable = dataSet.Tables[0];
                if (dataTable != null && dataTable.Rows.Count > 0)
                {
                    roundPrecision = dataTable.Rows[0]["Round_Precision"].ToString();
                }
                if (string.IsNullOrEmpty(roundPrecision) || string.IsNullOrEmpty(rcvQtyByProductUom))
                {
                    return;
                }
                double rcvPer = Math.Round(double.Parse(rcvQtyByProductUom) * double.Parse(roundPrecision), Convert.ToInt32(roundPrecision));
                //差异结果
                difference = Int64.Parse(item["IssuedQty"].ToString()) - rcvPer - Int64.Parse(item["DescFlexSegments_PrivateDescSeg5"].ToString())
                    - Int64.Parse(item["DescFlexSegments_PrivateDescSeg3"].ToString()) - Int64.Parse(item["DescFlexSegments_PrivateDescSeg4"].ToString());

                item["DescFlexField_PrivateDescSeg8"] = difference;
                #endregion
            }
        }

    }
}
