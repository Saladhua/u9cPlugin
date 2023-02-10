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
            string srcDocNo = "";//单据号
            string itemCode = "";//料号
            string docNo = "";//来源单据号
            string org = Context.LoginOrg.ID.ToString();
            string date = item.BusinessDate.ToString("yyyy.MM.dd");//日期
            foreach (var shipline in item.ShipLines)
            {
                if (item.Status.Value == 3)// 放到宏巨测试时在添加  && org == "1002208260110532"
                {
                    hJShipDocNo = shipline.DescFlexField.PrivateDescSeg5;//送货单号
                    srcDocNo = shipline.DescFlexField.PubDescSeg3;//备注 来源单据号
                    itemCode = shipline.ItemInfo.ItemCode;//料号
                    srcDocNo = shipline.SrcDocNo;
                    //select b.SrcDocInfo_SrcDocNo from PM_PurchaseOrder a inner join PM_POLine b on a.ID = b.PurchaseOrder
                    //where DocNo = (select TOP(1) b.SrcDocNo from SM_SO a inner join SM_SOLine b
                    //on a.ID = b.SO where a.DocNo = '30SO2302100001') and b.ItemInfo_ItemID = '1002211110149074'
                    string update1 = "update SM_ShipLine  set DescFlexField_PrivateDescSeg5='" + hJShipDocNo + "', DescFlexField_PrivateDescSeg6='" + date + "' " +
                        "where SrcDocNo = '" + srcDocNo + "' and Org = '1002208260110060' and ItemInfo_ItemCode = '" + itemCode + "'";
                    DataAccessor.RunSQL(DataAccessor.GetConn(), update1, null);
                }
            }
        }
    }
}
