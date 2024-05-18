using System.Data;
using UFIDA.U9.Base;
using UFIDA.U9.SM.Ship;
using UFSoft.UBF.Business;
using UFSoft.UBF.Util.DataAccess;
using UFSoft.UBF.Util.Log;

namespace YY.U9.Cust.LI.AppPlugIn
{
    /// <summary>
    /// 出货单行--审核后
    /// 插件配置是根据ship出货单判断
    /// 2023-2-10 暂时属于直接覆盖
    /// </summary>
    [UFSoft.UBF.Eventing.Configuration.Failfast]
    class ShipLinesUpdatingSubscriber : UFSoft.UBF.Eventing.IEventSubscriber
    {
        private static readonly ILogger logger = LoggerManager.GetLogger(typeof(ShipLinesUpdatingSubscriber));

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
            Ship item = key.GetEntity() as Ship;
            if (item == null)
            {
                return;
            }
            #endregion
            string hJShipDocNo = "";//送货单单号
            string hjReceiptedDate = "";//送货单签收日期
            string hjLogistics = "";//物流公司
            string courierDocNo = "";//快递单号
            string srcDocNo = "";//单据号
            string itemCode = "";//料号
            string itemid = "";//料号
            string docNo = "";//来源单据号
            string DocLineNo = "";//来源单据行号
            string receivementid = "";//收货单行的ID
            string org = Context.LoginOrg.ID.ToString();
            string date = item.BusinessDate.ToString("yyyy.MM.dd");//日期
            string ddocno = item.DescFlexField.PrivateDescSeg3;//私有字段3签收单号
            string ddate = item.DescFlexField.PrivateDescSeg10;//私有字段10签收日期
            foreach (var shipline in item.ShipLines)
            {
                //if (item.Status.Value == 3)// 放到宏巨测试时在添加  && org == "1002208260110532"
                //{
                    hJShipDocNo = shipline.DescFlexField.PrivateDescSeg5;//送货单号
                    hjLogistics=shipline.DescFlexField.PrivateDescSeg8;//物流公司
                    courierDocNo = shipline.DescFlexField.PrivateDescSeg9;//快递单号
                    hjReceiptedDate = shipline.DescFlexField.PrivateDescSeg10;//送货单签收日期
                    //srcDocNo = shipline.DescFlexField.PubDescSeg3;//备注 来源单据号
                    itemCode = shipline.ItemInfo.ItemCode;//料号
                    itemid = shipline.ItemInfo.ItemID.ID.ToString();//料号id
                    srcDocNo = shipline.SrcDocNo;
                    //select b.SrcDocInfo_SrcDocNo from PM_PurchaseOrder a inner join PM_POLine b on a.ID = b.PurchaseOrder
                    //where DocNo = (select TOP(1) b.SrcDocNo from SM_SO a inner join SM_SOLine b
                    //on a.ID = b.SO where a.DocNo = '30SO2302100001') and b.ItemInfo_ItemID = '1002211110149074'
                    //2023-4,获取私有字段3和10赋值
                    //ddocno = shipline.DescFlexField.PrivateDescSeg3;//签收单号
                    //ddate = shipline.DescFlexField.PrivateDescSeg10;//签收日期
                    if (org == "1002208260110532" || org == "1002208260110297")
                    {
                        #region 惠州的出货对常州的出货单赋值
                        DataTable dataTable = new DataTable();
                        string sqlFor = "select b.SrcDocInfo_SrcDocNo,b.SrcDocInfo_SrcDocLineNo from PM_PurchaseOrder a inner join PM_POLine b on a.ID = b.PurchaseOrder " +
                            "where DocNo = (select TOP(1) b.SrcDocNo from SM_SO a inner join SM_SOLine b" +
                            " on a.ID = b.SO where a.DocNo = '" + srcDocNo + "')";
                        DataSet dataSet = new DataSet();
                        DataAccessor.RunSQL(DataAccessor.GetConn(), sqlFor, null, out dataSet);
                        dataTable = dataSet.Tables[0];
                        if (dataTable != null && dataTable.Rows.Count > 0)
                        {
                            docNo = dataTable.Rows[0]["SrcDocInfo_SrcDocNo"].ToString();
                            DocLineNo = dataTable.Rows[0]["SrcDocInfo_SrcDocLineNo"].ToString();
                        }
                        string update1 = "update SM_ShipLine  set DescFlexField_PrivateDescSeg5='" + hJShipDocNo + "'," +
                            " DescFlexField_PrivateDescSeg6='" + date + "'," +
                            " DescFlexField_PrivateDescSeg8='" + hjLogistics + "'," +
                            " DescFlexField_PrivateDescSeg9='" + courierDocNo + "'," +
                            " DescFlexField_PrivateDescSeg10='" + hjReceiptedDate + "'" +
                            "where SrcDocNo = '" + docNo + "' and Org = '1002208260110060'  and DocLineNo = '"+ DocLineNo + "'  and ItemInfo_ItemCode = '" + itemCode + "'";
                        DataAccessor.RunSQL(DataAccessor.GetConn(), update1, null);
                        //2023 - 4 - 新增出货单头增加赋值两个字段
                        string docnonew = "";
                        string sqlFor4 = "select b.DocNo,b.ID from SM_ShipLine a inner join SM_Ship b on a.Ship= b.ID where a.SrcDocNo='" + docNo + "' " +
                            " and a.Org = '1002208260110060'";
                        string update3 = "";
                        DataAccessor.RunSQL(DataAccessor.GetConn(), sqlFor4, null, out dataSet);
                        dataTable = dataSet.Tables[0];
                        if (dataTable != null)
                        {
                            for (int i = 0; i < dataTable.Rows.Count; i++)
                            {
                                docnonew = dataTable.Rows[i]["DocNo"].ToString();
                                update3 = "update SM_Ship set DescFlexField_PrivateDescSeg3='" + ddocno + "',DescFlexField_PrivateDescSeg10='" + ddate + "' " +
                                    "  where DocNo='" + docnonew + "' and Org='1002208260110060';";
                                DataAccessor.RunSQL(DataAccessor.GetConn(), update3, null);
                            }
                        }
                        #endregion
                        #region 惠州的出货对常州的收货单赋值
                        string sqlForRcv = "select b.SrcDoc_SrcDocNo  from PM_Receivement a inner join PM_RcvLine b on a.ID = b.Receivement where SrcPO_SrcDocNo = " +
                            " (select TOP(1) b.SrcDocNo from SM_SO a inner join SM_SOLine b on a.ID = b.SO where a.DocNo = '" + srcDocNo + "')";
                        DataAccessor.RunSQL(DataAccessor.GetConn(), sqlForRcv, null, out dataSet);
                        dataTable = dataSet.Tables[0];
                        if (dataTable != null && dataTable.Rows.Count > 0)
                        {
                            receivementid = dataTable.Rows[0]["SrcDoc_SrcDocNo"].ToString();
                        }
                        string update2 = "update PM_RcvLine  set DescFlexSegments_PrivateDescSeg14='" + hJShipDocNo + "',DescFlexSegments_PrivateDescSeg15 = '" + date + "'" +
                            " where SrcDoc_SrcDocNo = '" + receivementid + "'  and ItemInfo_ItemCode = '" + itemCode + "'";
                        DataAccessor.RunSQL(DataAccessor.GetConn(), update2, null);
                        #endregion

                    }
                //}
            }
        }
    }
}
