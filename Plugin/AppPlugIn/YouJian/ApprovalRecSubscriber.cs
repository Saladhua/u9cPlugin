using System.Collections.Generic;
using UFIDA.U9.PM.Rcv;
using UFSoft.UBF.Business;
using UFSoft.UBF.Util.Log;

namespace YY.U9.Cust.LI.AppPlugIn
{
    /// <summary>
    /// 柚见项目
    /// 位置柚见接口开发列表(1).xlsx - 第13条
    /// 销售退回收货审核，同步甩手冲减
    /// </summary>
    [UFSoft.UBF.Eventing.Configuration.Failfast]

    class ApprovalRecSubscriber : UFSoft.UBF.Eventing.IEventSubscriber
    {
        private static readonly ILogger logger = LoggerManager.GetLogger(typeof(ApprovalMOSubscriber));
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
            Receivement item = key.GetEntity() as Receivement;
            if (item == null)
            {
                return;
            }
            #endregion

            //单据状态为审核中
            if (item.Status.Value != 3)
            {
                return;
            }
            //收货单的单号为rev19
            if (item.DocType.Code != "RCV19")
            {
                return;
            }
            DtoMO dtoMO = new DtoMO();
            dtoMO.DocType = item.DocType.Code;//单据类型取Code
            dtoMO.DocNo = item.DocNo;//单号
            dtoMO.OrderBy = item.RtnCustomer.Name;//客户取Name
            dtoMO.OrderByWZ = "";//客户位置
            dtoMO.AC = item.AC.Name;//币种名字
            dtoMO.ShipConfirmDate = item.BusinessDate;//日期
            dtoMO.ShipMemo = item.Memo;//备注
            dtoMO.dtoMOLines = new List<DtoMOLine>();
            foreach (var it in item.RcvLines)
            {
                DtoMOLine dtoMOLine = new DtoMOLine();
                dtoMOLine.ItemMasterName = it.ItemInfo.ItemName;
                dtoMOLine.ItemMasterCode = it.ItemInfo.ItemCode;
                dtoMOLine.ShipQtyInvAmount = it.RcvQty;//实收数量1
                dtoMOLine.WH = it.Wh.Code;//存储地点
                dtoMOLine.Uom = "";//少料品的计价单位
                dtoMO.dtoMOLines.Add(dtoMOLine);
            }

        }
    }
}
