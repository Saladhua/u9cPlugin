using System;
using System.Collections.Generic;
using System.Data;
using UFIDA.U9.ISV.MO;
using UFIDA.U9.ISV.MO.Proxy;
using UFIDA.U9.SCM.INV.MiscRcvUIModel;
using UFSoft.UBF.UI.Custom;
using UFSoft.UBF.UI.IView;
using UFSoft.UBF.Util.DataAccess;

namespace YY.U9.Cust.LI.UIPlugIn
{
    /// <summary>
    /// 杂收单--奥音功能置换
    /// </summary>
    internal class Cust_Li_MiscRcvTransLUIFormWebPartExtended : ExtendedPartBase
    {
        private MiscRcvUIMainFormWebPart _part;


        //public override void AfterRender(IPart part, EventArgs args)
        //{
        //    base.AfterRender(part, args);
        //    this._part = (part as MiscRcvUIMainFormWebPart);
        //    foreach (var item in _part.Model.MiscRcvTrans_MiscRcvTransLs.Records)
        //    {
        //        //生产订单单号
        //        string moDocNo = item["MoDocNo"].ToString();
        //        if (!string.IsNullOrEmpty(moDocNo))
        //        {
        //        }
        //    }
        //}

        public override void AfterEventProcess(IPart part, string eventName, object sender, EventArgs args)
        {
            base.AfterEventProcess(part, eventName, sender, args);
            UFSoft.UBF.UI.WebControlAdapter.UFWebButton4ToolbarAdapter webButton = sender as UFSoft.UBF.UI.WebControlAdapter.UFWebButton4ToolbarAdapter;
            this._part = (part as MiscRcvUIMainFormWebPart);
            //报废入库数量
            string scrapping = "";
            //生产订单单号
            string moDocNo = "";
            //生产订单的id
            string moDocNoID = "";
            //料品 -- 取料品的行
            string itemmaster = "";
            //入库数量
            string rcvQtyByProductUom = "";
            //精度
            string roundPrecision = "";
            //差异
            double difference = 0;
            //私有字段3
            string dprivateDescSeg3 = "";
            //私有字段4
            string dprivateDescSeg4 = "";
            //私有字段5
            string dprivateDescSeg5 = "";
            //已发放数量
            string issuedQty = "";
            //bom用量
            string bOMReqQty = "";
            //特别发料量
            string specialIssuedQty = "";
            try
            {
                string s = webButton.Action;
            }
            catch (Exception)
            {
                return;
            }
            if (webButton.Action == "ApproveClick")
            {
                foreach (var item in _part.Model.MiscRcvTrans_MiscRcvTransLs.Records)
                {
                    moDocNo = item["MoDocNo"].ToString();
                    scrapping = item["DescFlexSegments_PrivateDescSeg2"].ToString();
                    itemmaster = item["ItemInfo_ItemID"].ToString();
                    DataTable dataTable = new DataTable();
                    DataSet dataSet = new DataSet();
                    #region 获取当前生产订单单号的id
                    string sqlForMoDocNoID = "SELECT ID FROM MO_MO WHERE DocNo='" + moDocNo + "'";
                    DataAccessor.RunSQL(DataAccessor.GetConn(), sqlForMoDocNoID, null, out dataSet);
                    dataTable = dataSet.Tables[0];
                    if (dataTable != null && dataTable.Rows.Count > 0)
                    {
                        moDocNoID = dataTable.Rows[0]["ID"].ToString();
                    }
                    #endregion
                    //反写
                    if (!string.IsNullOrEmpty(scrapping))
                    {
                        //反写回去
                        if (!string.IsNullOrEmpty(moDocNoID))
                        {
                            string sqlForUpDate = "UPDATE MO_MOPickList  SET DescFlexField_PrivateDescSeg9='" + scrapping + "'WHERE ID = (SELECT a.ID FROM MO_MOPickList a" +
                                " INNER JOIN MO_MO bON a.MO = b.ID WHERE b.DocNo = '" + moDocNo + "' AND a.ItemMaster = '" + itemmaster + "')";
                            DataAccessor.RunSQL(DataAccessor.GetConn(), sqlForUpDate, null, out dataSet);
                        }
                    }
                    #region 四舍五入
                    //入库数量就是完工报告的完工数量--完工数量要求和
                    //SELECT sum(a.RcvQtyByProductUOM) RcvQtyByProductUOM FROM MO_CompleteRpt a 
                    //INNER JOIN MO_MO b ON a.MO = b.ID
                    //WHERE b.DocNo = 'SZ4813222101'
                    //string sqlForRBPUOM = "SELECT sum(a.RcvQtyByProductUOM) RcvQtyByProductUOM FROM MO_CompleteRpt a " +
                    //" INNER JOIN MO_MO b ON a.MO = b.ID WHERE b.DocNo = '" + moDocNo + "'";
                    //DataAccessor.RunSQL(DataAccessor.GetConn(), sqlForRBPUOM, null, out dataSet);
                    //dataTable = dataSet.Tables[0];
                    //if (dataTable != null && dataTable.Rows.Count > 0)
                    //{
                    //    rcvQtyByProductUom = dataTable.Rows[0]["RcvQtyByProductUOM"].ToString();
                    //}
                    //2.取料品上面的精度
                    //料品id
                    //SELECT Round_Precision FROM CBO_ItemMaster b
                    //INNER JOIN Base_UOM a ON a.ID = b.InventoryUOM
                    //WHERE b.Code = '111'
                    string sqlForPre = "SELECT Round_Precision FROM CBO_ItemMaster b " +
                        " INNER JOIN Base_UOM a ON a.ID = b.InventoryUOM WHERE b.Code = '" + item["ItemInfo_ItemCode"].ToString() + "'";
                    DataAccessor.RunSQL(DataAccessor.GetConn(), sqlForPre, null, out dataSet);
                    dataTable = dataSet.Tables[0];
                    if (dataTable != null && dataTable.Rows.Count > 0)
                    {
                        roundPrecision = dataTable.Rows[0]["Round_Precision"].ToString();
                    }

                    #region 测试使用值
                    //issuedQty = "9000.000000000";
                    //bOMReqQty = "0.022000000";
                    //specialIssuedQty = "0";
                    //dprivateDescSeg3 = "0";
                    //dprivateDescSeg4 = "0";
                    //dprivateDescSeg5 = "0.000000000";
                    //rcvQtyByProductUom = "9000.000000000";
                    //decimal seee = decimal.Parse(bOMReqQty);
                    //double q = double.Parse(rcvQtyByProductUom) * double.Parse(bOMReqQty);
                    //double rcvPer1 = Math.Round(q, Convert.ToInt32(roundPrecision));

                    //Difference = IssuedQty + SpecialIssuedQty - TotalRcvQty * QPA - ProcessLoss - ShuntingLoss - MassLoss;


                    //difference = Convert.ToDouble(issuedQty) + Convert.ToDouble(specialIssuedQty) - q - Convert.ToDouble(dprivateDescSeg3)
                    //    - Convert.ToDouble(dprivateDescSeg4) - Convert.ToDouble(dprivateDescSeg5);
                    #endregion


                    if (string.IsNullOrEmpty(roundPrecision) || string.IsNullOrEmpty(rcvQtyByProductUom))
                    {
                        //return;
                    }
                    string sqlForCy = "SELECT a.IssuedQty,a.DescFlexField_PrivateDescSeg3,a.DescFlexField_PrivateDescSeg4,a.DescFlexField_PrivateDescSeg5,a.QPA,a.SpecialIssuedQty,b.TotalRcvQty FROM MO_MOPickList a" +
                        " INNER JOIN MO_MO b ON a.MO = b.ID WHERE b.DocNo = '" + moDocNo + "' AND a.ItemMaster = '" + itemmaster + "'";
                    DataAccessor.RunSQL(DataAccessor.GetConn(), sqlForCy, null, out dataSet);
                    dataTable = dataSet.Tables[0];
                    if (dataTable != null && dataTable.Rows.Count > 0)
                    {
                        //issuedQty = dataTable.Rows[0]["IssuedQty"].ToString();
                        //bOMReqQty = dataTable.Rows[0]["QPA"].ToString();
                        //rcvQtyByProductUom = dataTable.Rows[0]["TotalRcvQty"].ToString();
                        //specialIssuedQty = dataTable.Rows[0]["SpecialIssuedQty"].ToString();
                        //dprivateDescSeg3 = dataTable.Rows[0]["DescFlexField_PrivateDescSeg3"].ToString() == "" ? "0" : dataTable.Rows[0]["DescFlexField_PrivateDescSeg3"].ToString();
                        //dprivateDescSeg4 = dataTable.Rows[0]["DescFlexField_PrivateDescSeg4"].ToString() == "" ? "0" : dataTable.Rows[0]["DescFlexField_PrivateDescSeg4"].ToString();
                        //dprivateDescSeg5 = dataTable.Rows[0]["DescFlexField_PrivateDescSeg5"].ToString() == "" ? "0" : dataTable.Rows[0]["DescFlexField_PrivateDescSeg5"].ToString();
                        issuedQty = dataTable.Rows[0]["IssuedQty"].ToString() == "" ? "0" : dataTable.Rows[0]["IssuedQty"].ToString();
                        bOMReqQty = dataTable.Rows[0]["QPA"].ToString() == "" ? "0" : dataTable.Rows[0]["QPA"].ToString();
                        rcvQtyByProductUom = dataTable.Rows[0]["TotalRcvQty"].ToString() == "" ? "0" : dataTable.Rows[0]["TotalRcvQty"].ToString();
                        specialIssuedQty = dataTable.Rows[0]["SpecialIssuedQty"].ToString() == "" ? "0" : dataTable.Rows[0]["SpecialIssuedQty"].ToString();
                        dprivateDescSeg3 = dataTable.Rows[0]["DescFlexField_PrivateDescSeg3"].ToString() == "" ? "0" : dataTable.Rows[0]["DescFlexField_PrivateDescSeg3"].ToString();
                        dprivateDescSeg4 = dataTable.Rows[0]["DescFlexField_PrivateDescSeg4"].ToString() == "" ? "0" : dataTable.Rows[0]["DescFlexField_PrivateDescSeg4"].ToString();
                        dprivateDescSeg5 = dataTable.Rows[0]["DescFlexField_PrivateDescSeg5"].ToString() == "" ? "0" : dataTable.Rows[0]["DescFlexField_PrivateDescSeg5"].ToString();


                    }
                    //double see1 = Convert.ToDouble(issuedQty);
                    //double see2 = Convert.ToDouble(dprivateDescSeg3);
                    //double see3 = Convert.ToDouble(dprivateDescSeg4);
                    //double see4 = Convert.ToDouble(dprivateDescSeg5);
                    //rcvQtyByProductUom = "50";bOMReqQty = "1";
                    double r = double.Parse(rcvQtyByProductUom) * double.Parse(bOMReqQty);
                    //double rcvPer = Math.Round(r, Convert.ToInt32(roundPrecision));
                    //差异结果
                    difference = Convert.ToDouble(issuedQty) + Convert.ToDouble(specialIssuedQty) - r - Convert.ToDouble(dprivateDescSeg3)
                        - Convert.ToDouble(dprivateDescSeg4) - Convert.ToDouble(dprivateDescSeg5);

                    //difference = Convert.ToDouble(issuedQty) - rcvPer - Convert.ToDouble(dprivateDescSeg3)
                    //    - Convert.ToDouble(dprivateDescSeg4) - Convert.ToDouble(dprivateDescSeg5);
                    //反写回去
                    if (!string.IsNullOrEmpty(moDocNoID) && !string.IsNullOrEmpty(difference.ToString()))
                    {
                        string sqlForUpDate = "UPDATE MO_MOPickList  SET DescFlexField_PrivateDescSeg8='" + difference + "'WHERE ID = (SELECT a.ID FROM MO_MOPickList a" +
                            " INNER JOIN MO_MO b ON a.MO = b.ID WHERE b.DocNo = '" + moDocNo + "' AND a.ItemMaster = '" + itemmaster + "')";
                        DataAccessor.RunSQL(DataAccessor.GetConn(), sqlForUpDate, null, out dataSet);
                    }
                    bool ok = Convert.ToDecimal(rcvQtyByProductUom) == Math.Round(Convert.ToDecimal(issuedQty) + Convert.ToDecimal(specialIssuedQty)) ? true : false;

                    #endregion
                    if (difference <= 0.0001 || ok == true)
                    {
                        #region 当关闭服务触发时，更新关闭人字段为yonyou
                        string sqlForUpDate = "UPDATE MO_MO  SET ClosedBy='yonyou' WHERE ID='" + moDocNoID + "'";
                        DataAccessor.RunSQL(DataAccessor.GetConn(), sqlForUpDate, null, out dataSet);
                        #endregion
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
                }
            }
        }


    }
}
