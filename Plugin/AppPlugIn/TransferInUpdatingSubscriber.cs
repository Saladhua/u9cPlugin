using System.Data;
using UFIDA.U9.InvDoc.TransferIn;
using UFSoft.UBF.Business;
using UFSoft.UBF.Util.DataAccess;
using UFSoft.UBF.Util.Log;

namespace YY.U9.Cust.LI.AppPlugIn
{
    [UFSoft.UBF.Eventing.Configuration.Failfast]

    class TransferInUpdatingSubscriber : UFSoft.UBF.Eventing.IEventSubscriber
    {
        private static readonly ILogger logger = LoggerManager.GetLogger(typeof(TransferInUpdatingSubscriber));
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
            TransInLine transferIn = key.GetEntity() as TransInLine;
            if (transferIn == null)
            {
                return;
            }
            long transOutWh = 0;
            foreach (var item in transferIn.TransInSubLines)
            {
                transOutWh = item.TransOutWh.ID;
            }
            #endregion
            #region 获取到状态
            DataTable dataTable_1 = new DataTable();
            string sql_1 = "SELECT InvDoc_TransferIn.Status FROM InvDoc_TransInLine INNER JOIN InvDoc_TransferIn ON InvDoc_TransInLine.TransferIn = InvDoc_TransferIn.ID " +
                            " WHERE InvDoc_TransInLine.ID = '" + transferIn.ID + "'";
            DataSet set_1 = new DataSet();
            DataAccessor.RunSQL(DataAccessor.GetConn(), sql_1, null, out set_1);
            dataTable_1 = set_1.Tables[0];
            string statu = "0";
            if (dataTable_1.Rows != null && dataTable_1.Rows.Count > 0)
            {
                statu = dataTable_1.Rows[0]["Status"].ToString();
            }
            #endregion

            #region 拿到调出存储地点，拿到对应的料号
            DataTable dataTable_2 = new DataTable();
            string sql_2 = "select  SUM(StoreQty) StoreQty  from  InvTrans_WhQoh where ItemInfo_ItemID='" + transferIn.ItemInfo.ItemID.ID + "' AND Wh = '" + transOutWh + "'";
            DataSet set_2 = new DataSet();
            DataAccessor.RunSQL(DataAccessor.GetConn(), sql_2, null, out set_2);
            dataTable_2 = set_2.Tables[0];
            decimal qty = 0;
            if (dataTable_2.Rows != null && dataTable_2.Rows.Count > 0)
            {
                bool a = decimal.TryParse(dataTable_2.Rows[0]["StoreQty"].ToString(), out qty);
            }
            if (statu == "1"|| statu == "2")
            {
                transferIn.DescFlexSegments.PrivateDescSeg1 = qty.ToString();
            }
            else
            {
                transferIn.DescFlexSegments.PrivateDescSeg1 = (qty - transferIn.StoreUOMQty).ToString();
            }

            #endregion
        }
    }
}