using System;
using System.Data;
using UFIDA.U9.InvDoc.MiscShip;
using UFIDA.U9.UI.PDHelper;
using UFSoft.UBF.Business;
using UFSoft.UBF.Util.DataAccess;
using UFSoft.UBF.Util.Log;


namespace YY.U9.Cust.LI.AppPlugIn
{
    /// <summary>
    /// 杂发单
    /// 创联提交验证
    /// </summary>
    [UFSoft.UBF.Eventing.Configuration.Failfast]
    class CLMiscShipmentLInsertedSubscriber : UFSoft.UBF.Eventing.IEventSubscriber
    {
        private static readonly ILogger logger = LoggerManager.GetLogger(typeof(CLMiscShipmentLInsertedSubscriber));
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
            MiscShipment miscShipmentLs = key.GetEntity() as MiscShipment;


            if (miscShipmentLs == null)
            {
                return;
            }
            #endregion

            string miscRcvTransLDocType = miscShipmentLs.DocType.Code;

            //if (miscRcvTransLDocType != "ZF03")
            //{
            //    return;
            //}

            //插入的时候做计算

            foreach (var item in miscShipmentLs.MiscShipLs)
            {
                string OrgCode = PDContext.Current.OrgRef.CodeColumn;

                string ItemCode = item.ItemInfo.ItemCode;

                string xykcl = "0";//现有库存量

                string yjgdlll = "0";//预计工单领料量

                string yjwxll = "0";//预计外协领料

                string aqkcl = "0";//安全库存量

                string yjqgl = "0"; //预计请购量

                string yjcgl = "0";//预计采购量

                string cgzjl = "0";//采购在检量

                string yjscl = "0";//预计生产量

                string ycddsl = "0";//预测订单数量

                string xsddsl = "0";//销售订单数量

                string kykcl = "0";//可用库存量

                decimal kyl = 0;//杂发可用量

                DataTable dataTable = new DataTable();
                string sql = "exec Cust_GongXuMingxi @SPECS=NULL,@ItemName=NULL,@Org='" + OrgCode + "',@ItemCode='(ItemCode = ''" + ItemCode + "'') '";
                DataSet dataSet = new DataSet();
                DataAccessor.RunSQL(DataAccessor.GetConn(), sql, null, out dataSet);
                dataTable = dataSet.Tables[0];
                if (dataTable.Rows != null && dataTable.Rows.Count > 0)
                {
                    xykcl = dataTable.Rows[0]["xykcl"].ToString();

                    yjgdlll = dataTable.Rows[0]["yjgdlll"].ToString();

                    yjwxll = dataTable.Rows[0]["yjwxll"].ToString();

                    aqkcl = dataTable.Rows[0]["aqkcl"].ToString();

                    yjqgl = dataTable.Rows[0]["yjqgl"].ToString();

                    yjcgl = dataTable.Rows[0]["yjcgl"].ToString();

                    cgzjl = dataTable.Rows[0]["cgzjl"].ToString();

                    yjscl = dataTable.Rows[0]["yjscl"].ToString();

                    ycddsl = dataTable.Rows[0]["ycddsl"].ToString();

                    xsddsl = dataTable.Rows[0]["xsddsl"].ToString();

                    kykcl = dataTable.Rows[0]["kykcl"].ToString();

                }

                kyl = decimal.Parse(kykcl);

                kyl = Math.Round(kyl, 4);

                item.DescFlexSegments.PrivateDescSeg2 = kyl.ToString("0.####");

            }

            //DataSet dataSe1t = new DataSet();
            //string sql1 = "exec P_SyncFieldCombineName @FullName = 'UFIDA.U9.InvDoc.MiscShip.MiscShipmentL',@DescFieldName = 'DescFlexSegments_PrivateDescSeg2'";
            //DataAccessor.RunSQL(DataAccessor.GetConn(), sql1, null, out dataSe1t);

            //xykcl = "70";//现有库存量

            //yjgdlll = "0";//预计工单领料量

            //yjwxll = "0";//预计外协领料

            //aqkcl = "1000";//安全库存量

            //yjqgl = "0"; //预计请购量

            //yjcgl = "0";//预计采购量

            //cgzjl = "0";//采购在检量

            //yjscl = "1000";//预计生产量

            //ycddsl = "80";//预测订单数量

            //xsddsl = "0";//销售订单数量

            //kykcl = "-10";//可用库存量

            //SUM(现有库存量 + 预计请购量 + 预计采购量 + 采购在检量 + 预计生产量 - 预计工单领料量 - 预计外协领料 - 预测订单数量 - 销售订单数量 - 安全库存量)


            //miscShipmentLs.DescFlexSegments.PrivateDescSeg2 = kyl.ToString("0.####");
            //string sql1 = "exec P_SyncFieldCombineName @FullName = 'UFIDA.U9.InvDoc.MiscShip.MiscShipmentL',@DescFieldName = 'DescFlexSegments_PrivateDescSeg2'";
            //DataAccessor.RunSQL(DataAccessor.GetConn(), sql1, null, out dataSet);
        }
    }
}

