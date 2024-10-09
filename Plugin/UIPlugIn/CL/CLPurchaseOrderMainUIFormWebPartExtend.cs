using System;
using System.Data;
using UFIDA.U9.PM.PurchaseOrderUIModel;
using UFSoft.UBF.UI.Custom;
using UFSoft.UBF.UI.IView;
using UFSoft.UBF.Util.DataAccess;
using UFSoft.UBF.Util.Log;

namespace YY.U9.Cust.LI.UIPlugIn
{

    /// <summary>
    /// 期初委外订单
    /// 对于3个私有子段进行计算
    ///1.委外订单增加“销售备注”--私有字段6，创建和保存的时候触发，根据需求分类取预测订单表头的销售备注，如果获取不到，就取销售订单表头的销售备注
    ///2.委外订单增加“客户”--私有字段7，创建和保存的时候触发，根据需求分类取预测订单表头的客户，如果获取不到，就取销售订单表头的客户
    ///3.委外订单增加“销售业务员”--私有字段8，创建和保存的时候触发，根据需求分类取预测订单表头的业务员，如果获取不到，就取销售订单表头的业务员
    /// </summary>
    class CLPurchaseOrderMainUIFormWebPartExtend : ExtendedPartBase
    {
        private PurchaseOrderMainUIFormWebPart _part;

        private static readonly ILogger logger = LoggerManager.GetLogger(typeof(CLPurchaseOrderMainUIFormWebPartExtend));

        public override void AfterRender(IPart part, EventArgs args)
        {
            base.AfterRender(part, args);

            this._part = (part as PurchaseOrderMainUIFormWebPart);

            if (this._part.Model.PurchaseOrder.FocusedRecord["BizType"] != null)
            {
                string BizType = this._part.Model.PurchaseOrder.FocusedRecord["BizType"].ToString();//业务类型

                string PurDocNo= this._part.Model.PurchaseOrder.FocusedRecord["DocNo"].ToString();//单号
             
                if (BizType != "326")
                {
                    return;
                }

                foreach (var item in this._part.Model.PurchaseOrder_POLines.Records)
                {
                    if (item["DemondCode"] != null)
                    {
                        string DemondCode = item["DemondCode"].ToString();

                        string DocNo = "";

                        string sqle = "select A.Code from UBF_Sys_ExtEnumValue A left join UBF_Sys_ExtEnumType B on A.ExtEnumType = B.ID" +
                            " where B.Code = 'UFIDA.U9.CBO.Enums.DemandCodeEnum' AND A.EValue = '" + DemondCode + "'";

                        DataTable dt = U9Common.GetDataTable(sqle);

                        if (dt.Rows != null && dt.Rows.Count > 0)
                        {
                            DocNo = dt.Rows[0]["Code"].ToString();//单号需要处理
                        }

                        if (!string.IsNullOrEmpty(DocNo))
                        {
                            string desc6 = "";//销售备注

                            string desc7 = "";//客户

                            string desc8 = "";//销售业务员

                            int firstIndexOfColon = DocNo.IndexOf('_');

                            DocNo = DocNo.Substring(0, firstIndexOfColon);

                            string sqlforyuc = "SELECT " +
                                " B.Note, " +
                                " (SELECT Name FROM CBO_Customer_Trl C WHERE ID = A.Customer_Customer) AS CustomerName," +
                                " (SELECT Name FROM CBO_Operators_Trl C WHERE ID = A.OrderOperator) AS OperatorsName" +
                                " FROM SM_ForecastOrder A LEFT JOIN SM_ForecastOrder_Trl B ON A.ID = B.ID WHERE A.DocNo = '" + DocNo + "'";

                            DataTable dt2 = U9Common.GetDataTable(sqlforyuc);

                            if (dt2.Rows != null && dt2.Rows.Count > 0)
                            {
                                desc6 = dt2.Rows[0]["Note"].ToString();//销售备注

                                desc7 = dt2.Rows[0]["CustomerName"].ToString();//客户

                                desc8 = dt2.Rows[0]["OperatorsName"].ToString();//销售业务员

                                item["DescFlexSegments_PrivateDescSeg6"] = desc6;

                                item["DescFlexSegments_PrivateDescSeg7"] = desc7;

                                item["DescFlexSegments_PrivateDescSeg8"] = desc8;

                                logger.Error("外协订单单据号："+ PurDocNo + "" + "成功，对应单号:"+ DocNo + "");
                            }
                            else//如果预测订单没找到，就销售订单
                            {
                                string sqlforxs = " SELECT" +
                                    " B.Memo," +
                                    " (SELECT Name FROM CBO_Customer_Trl C WHERE ID = A.OrderBy_Customer) AS CustomerName," +
                                    " (SELECT Name FROM CBO_Operators_Trl C WHERE ID = A.Seller) AS SellerName" +
                                    " FROM" +
                                    " SM_SO A LEFT JOIN SM_SO_Trl B ON A.ID = B.ID  WHERE A.DocNo = '" + DocNo + "'";

                                DataTable dt3 = U9Common.GetDataTable(sqlforxs);

                                if (dt3.Rows != null && dt3.Rows.Count > 0)
                                {
                                    desc6 = dt3.Rows[0]["Memo"].ToString();//销售备注

                                    desc7 = dt3.Rows[0]["CustomerName"].ToString();//客户

                                    desc8 = dt3.Rows[0]["SellerName"].ToString();//销售业务员

                                    item["DescFlexSegments_PrivateDescSeg6"] = desc6;

                                    item["DescFlexSegments_PrivateDescSeg7"] = desc7;

                                    item["DescFlexSegments_PrivateDescSeg8"] = desc8;

                                    logger.Error("外协订单单据号：" + PurDocNo + "" + "成功，对应单号:" + DocNo + "");
                                }
                            }
                        }
                    }
                }

            }
        }
    }
}
