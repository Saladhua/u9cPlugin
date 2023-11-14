using System;
using System.Collections.Generic;
using System.Data;
using UFIDA.U9.ISV.MO;
using UFIDA.U9.ISV.MO.Proxy;
using UFIDA.U9.MFG.MO.CompleteRptUIModel;
using UFSoft.UBF.UI.Custom;
using UFSoft.UBF.UI.IView;
using UFSoft.UBF.Util.DataAccess;

namespace YY.U9.Cust.LI.UIPlugIn
{
    /// <summary>
    /// 完工报告，奥音 自动完工
    /// </summary>
    internal class Cust_Li_CompleteRptMainUIFormWebPartWebPartExtended : ExtendedPartBase
    {
        private CompleteRptMainUIFormWebPart _part;


        public override void AfterEventProcess(IPart part, string eventName, object sender, EventArgs args)
        {
            base.AfterEventProcess(part, eventName, sender, args);
            UFSoft.UBF.UI.WebControlAdapter.UFWebButton4ToolbarAdapter webButton = sender as UFSoft.UBF.UI.WebControlAdapter.UFWebButton4ToolbarAdapter;
            this._part = (part as CompleteRptMainUIFormWebPart);
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
            Decimal difference = 0;
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

            if (webButton.Action == "ApproveClick")//SubmitClick  ApproveClick
            {
                string Yi = "是";//所有行差异是否满足

                foreach (var item in _part.Model.CompleteRpt.Records)
                {
                    moDocNo = item["MO"].ToString();
                    string completeQty = item["CompleteQty"].ToString();
                    //scrapping = item["DescFlexSegments_PrivateDescSeg2"].ToString();
                    //itemmaster = item["ItemInfo_ItemID"].ToString();
                    string donno = "";
                    DataTable dataTable = new DataTable();
                    DataSet dataSet = new DataSet();
                    #region 获取当前生产订单单号的id
                    string sqlForMoDocNoID = "SELECT DocNo,ID,DescFlexField_PrivateDescSeg2,ItemMaster FROM MO_MO WHERE ID='" + moDocNo + "'";
                    DataAccessor.RunSQL(DataAccessor.GetConn(), sqlForMoDocNoID, null, out dataSet);
                    dataTable = dataSet.Tables[0];
                    if (dataTable != null && dataTable.Rows.Count > 0)
                    {
                        moDocNoID = dataTable.Rows[0]["ID"].ToString();
                        scrapping = dataTable.Rows[0]["DescFlexField_PrivateDescSeg2"].ToString();
                        donno = dataTable.Rows[0]["DocNo"].ToString();
                        itemmaster = dataTable.Rows[0]["ItemMaster"].ToString();
                    }
                    #endregion
                    //反写
                    if (!string.IsNullOrEmpty(scrapping))
                    {
                        //反写回去
                        if (!string.IsNullOrEmpty(moDocNoID))
                        {
                            string sqlForUpDate = "UPDATE MO_MOPickList  SET DescFlexField_PrivateDescSeg9='" + scrapping + "'WHERE ID = (SELECT a.ID FROM MO_MOPickList a" +
                                " INNER JOIN MO_MO b ON a.MO = b.ID WHERE b.DocNo = '" + moDocNo + "' AND a.ItemMaster = '" + itemmaster + "')";
                            DataAccessor.RunSQL(DataAccessor.GetConn(), sqlForUpDate, null, out dataSet);
                        }
                    }
                    #region 四舍五入
                    //入库数量就是完工报告的完工数量--完工数量要求和
                    //SELECT sum(a.RcvQtyByProductUOM) RcvQtyByProductUOM FROM MO_CompleteRpt a 
                    //INNER JOIN MO_MO b ON a.MO = b.ID
                    //WHERE b.DocNo = 'SZ4813222101'
                    //string sqlForRBPUOM = "SELECT sum(a.RcvQtyByProductUOM) RcvQtyByProductUOM FROM MO_CompleteRpt a " +
                    //" INNER JOIN MO_MO b ON a.MO = b.ID WHERE b.ID = '" + moDocNo + "'";
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
                    //WHERE b.Code = '111'a
                    #endregion
                    //0.013500000 0.008489000 0   1002111170030995    0   0.000002700 0.000000000 1856.000000000
                    //1856.000000000  0   0   1002201180043856    0   1.000000000 0.000000000 1856.000000000
                    //issuedQty = 0.210000000,
                    //bOMReqQty = 0.105000000,rcvQtyByProductUom = 2,
                    //specialIssuedQty = 0.000000000,dprivateDescSeg3 = 0,dprivateDescSeg4 = 0,
                    //dprivateDescSeg5 = 0,r = 0.21,difference = 0.000000000

                    #region 测试使用值

                    //issuedQty = 0.000000000,bOMReqQty = 1.000000000,rcvQtyByProductUom = 8400,
                    //specialIssuedQty = 0.000000000,dprivateDescSeg3 = 0,dprivateDescSeg4 = 0,dprivateDescSeg5 = 0,
                    //r = 8400,

                    //issuedQty = "0.210000000";
                    //bOMReqQty = "0.105000000";
                    //specialIssuedQty = "0.000000000";
                    //dprivateDescSeg3 = "0";
                    //dprivateDescSeg4 = "0";
                    //dprivateDescSeg5 = "0";
                    //rcvQtyByProductUom = "2";
                    //double q = double.Parse(rcvQtyByProductUom) * double.Parse(bOMReqQty);
                    //// double rcvPer1 = Math.Round(q, Convert.ToInt32(roundPrecision));
                    ////Difference = IssuedQty + SpecialIssuedQty - TotalRcvQty * QPA - ProcessLoss - ShuntingLoss - MassLoss;
                    ////difference = Convert.ToDouble(issuedQty) + Convert.ToDouble(specialIssuedQty) - q - Convert.ToDouble(dprivateDescSeg3)
                    ////    - Convert.ToDouble(dprivateDescSeg4) - Convert.ToDouble(dprivateDescSeg5);
                    //issuedQty = issuedQty == "" ? "0" : issuedQty;
                    //bOMReqQty = bOMReqQty == "" ? "0" : bOMReqQty;
                    //rcvQtyByProductUom = Math.Round(double.Parse(rcvQtyByProductUom) + double.Parse("0")).ToString();
                    //specialIssuedQty = specialIssuedQty == "" ? "0" : specialIssuedQty;
                    //dprivateDescSeg3 = dprivateDescSeg3 == "" ? "0" : dprivateDescSeg3;
                    //dprivateDescSeg4 = dprivateDescSeg4 == "" ? "0" : dprivateDescSeg4;
                    //dprivateDescSeg5 = dprivateDescSeg5 == "" ? "0" : dprivateDescSeg5;
                    ////double qq = double.Parse(rcvQtyByProductUom) * double.Parse(bOMReqQty);
                    ////double rcvPer = Math.Round(r, Convert.ToInt32(roundPrecision));
                    ////差异结果
                    ////Difference = IssuedQty + SpecialIssuedQty - TotalRcvQty * QPA - ProcessLoss - ShuntingLoss - MassLoss;
                    //difference = Decimal.Parse(issuedQty) + Decimal.Parse(specialIssuedQty) - Decimal.Parse(q.ToString()) - Decimal.Parse(dprivateDescSeg3)
                    //    - Decimal.Parse(dprivateDescSeg4) - Decimal.Parse(dprivateDescSeg5);
                    //bool ok11 = Convert.ToDecimal(rcvQtyByProductUom) == Math.Round(Convert.ToDecimal(issuedQty) + Convert.ToDecimal(specialIssuedQty)) ? true : false;

                    //if (difference > Decimal.Parse((0.0001).ToString()))
                    //{ string q1qqqqq23123123123 = "13123123"; }
                    #endregion

                    string sqlForPre = "SELECT Round_Precision FROM CBO_ItemMaster b " +
                    " INNER JOIN Base_UOM a ON a.ID = b.InventoryUOM WHERE b.Code = '" + item["Item_Code"].ToString() + "'";
                    DataAccessor.RunSQL(DataAccessor.GetConn(), sqlForPre, null, out dataSet);
                    dataTable = dataSet.Tables[0];
                    if (dataTable != null && dataTable.Rows.Count > 0)
                    {
                        roundPrecision = dataTable.Rows[0]["Round_Precision"].ToString();
                    }
                    if (string.IsNullOrEmpty(roundPrecision) || string.IsNullOrEmpty(rcvQtyByProductUom))
                    {
                        //return;
                    }
                    string sqlForCy = "SELECT a.IssuedQty,a.DescFlexField_PrivateDescSeg3,a.DescFlexField_PrivateDescSeg4,a.ItemMaster,a.DescFlexField_PrivateDescSeg5,a.QPA,a.SpecialIssuedQty,b.TotalRcvQty  FROM MO_MOPickList a" +
                        " INNER JOIN MO_MO b ON a.MO = b.ID WHERE b.DocNo = '" + donno + "'";
                    DataAccessor.RunSQL(DataAccessor.GetConn(), sqlForCy, null, out dataSet);
                    dataTable = dataSet.Tables[0];
                    List<DtoDer> dtoDers = new List<DtoDer>();
                    if (dataTable != null && dataTable.Rows.Count > 0)
                    {
                        int i = 0;
                        string see = dataTable.Rows.Count.ToString();
                        while (i < dataTable.Rows.Count)
                        {
                            DtoDer dtoDer = new DtoDer();
                            issuedQty = dataTable.Rows[i]["IssuedQty"].ToString() == "" ? "0" : dataTable.Rows[i]["IssuedQty"].ToString();
                            bOMReqQty = dataTable.Rows[i]["QPA"].ToString() == "" ? "0" : dataTable.Rows[i]["QPA"].ToString();
                            //rcvQtyByProductUom = (double.Parse(dataTable.Rows[i]["TotalRcvQty"].ToString()) + double.Parse(item["CompleteQty"].ToString())).ToString();
                            rcvQtyByProductUom = Math.Round(double.Parse(dataTable.Rows[i]["TotalRcvQty"].ToString()) + double.Parse(item["CompleteQty"].ToString())).ToString();
                            specialIssuedQty = dataTable.Rows[i]["SpecialIssuedQty"].ToString() == "" ? "0" : dataTable.Rows[i]["SpecialIssuedQty"].ToString();
                            dprivateDescSeg3 = dataTable.Rows[i]["DescFlexField_PrivateDescSeg3"].ToString() == "" ? "0" : dataTable.Rows[i]["DescFlexField_PrivateDescSeg3"].ToString();
                            dprivateDescSeg4 = dataTable.Rows[i]["DescFlexField_PrivateDescSeg4"].ToString() == "" ? "0" : dataTable.Rows[i]["DescFlexField_PrivateDescSeg4"].ToString();
                            dprivateDescSeg5 = dataTable.Rows[i]["DescFlexField_PrivateDescSeg5"].ToString() == "" ? "0" : dataTable.Rows[i]["DescFlexField_PrivateDescSeg5"].ToString();
                            double r = double.Parse(rcvQtyByProductUom) * double.Parse(bOMReqQty);
                            double rcvPer = Math.Round(r, Convert.ToInt32(roundPrecision));
                            //差异结果
                            //Difference = IssuedQty + SpecialIssuedQty - TotalRcvQty * QPA - ProcessLoss - ShuntingLoss - MassLoss;

                            difference = Decimal.Parse(issuedQty) + Decimal.Parse(specialIssuedQty) - Decimal.Parse(r.ToString()) - Decimal.Parse(dprivateDescSeg3)
                                - Decimal.Parse(dprivateDescSeg4) - Decimal.Parse(dprivateDescSeg5);
                            string allsee = "issuedQty=" + issuedQty + "," + "bOMReqQty=" + bOMReqQty + "," + "rcvQtyByProductUom=" + rcvQtyByProductUom + ","
                                + "specialIssuedQty=" + specialIssuedQty + "," + "dprivateDescSeg3=" + dprivateDescSeg3 + ","
                                + "dprivateDescSeg4=" + dprivateDescSeg4 + "," + "dprivateDescSeg5=" + dprivateDescSeg5 + ","
                                + "r=" + r + "," + "difference=" + difference;
                            //difference = Convert.ToDouble(issuedQty) - rcvPer - Convert.ToDouble(dprivateDescSeg3)
                            //    - Convert.ToDouble(dprivateDescSeg4) - Convert.ToDouble(dprivateDescSeg5);
                            //反写回去
                            if (!string.IsNullOrEmpty(moDocNoID) && !string.IsNullOrEmpty(difference.ToString()))
                            {
                                string sqlForUpDate = "UPDATE MO_MOPickList  SET DescFlexField_PrivateDescSeg8='" + difference + "', DescFlexField_PrivateDescSeg29='" + allsee + "'  WHERE ID = (SELECT  top(1) a.ID FROM MO_MOPickList a" +
                                    " INNER JOIN MO_MO b ON a.MO = b.ID WHERE b.DocNo = '" + donno + "' AND a.ItemMaster = '" + dataTable.Rows[i]["ItemMaster"].ToString() + "')";
                                DataAccessor.RunSQL(DataAccessor.GetConn(), sqlForUpDate, null, out dataSet);
                            }
                            i++;
                            bool ok = Convert.ToDecimal(rcvQtyByProductUom) == Math.Round(Convert.ToDecimal(issuedQty) + Convert.ToDecimal(specialIssuedQty)) ? true : false;
                            dtoDer.ok = ok;
                            dtoDer.Difference = difference;
                            dtoDers.Add(dtoDer);
                        }
                        //if (difference <= Decimal.Parse((0.0001).ToString()) || ok == true)
                        foreach (var ite in dtoDers)
                        {
                            if (ite.Difference > Decimal.Parse((0.0001).ToString()))
                            {
                                Yi = "否";
                            }
                        }
                        if (Yi == "是")
                        {
                            #region 当关闭服务触发时，更新关闭人字段为yonyou
                            string sqlForUpDate = "UPDATE MO_MO  SET ClosedBy='yonyou' WHERE ID='" + moDocNoID + "'";
                            DataAccessor.RunSQL(DataAccessor.GetConn(), sqlForUpDate, null, out dataSet);
                            #endregion
                            CompleteMoProxy complete = new CompleteMoProxy();
                            List<MOOperateParamDTOData> mOOperates = new List<MOOperateParamDTOData>();
                            MOOperateParamDTOData mOOperate = new MOOperateParamDTOData();
                            mOOperate.MODocNo = donno;
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
        /// <summary>
        /// 记录差异
        /// </summary>
        public class DtoDer
        {
            public Decimal Difference { get; set; }
            public bool ok { get; set; }
        }
    }
}
