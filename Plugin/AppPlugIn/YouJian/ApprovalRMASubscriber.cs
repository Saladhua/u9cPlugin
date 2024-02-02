using System.Collections.Generic;
using UFIDA.U9.SM.RMA;
using UFSoft.UBF.Business;
using UFSoft.UBF.Util.Log;

namespace YY.U9.Cust.LI.AppPlugIn
{
    /// 柚见项目
    /// 位置柚见接口开发列表(1).xlsx - 第15条
    /// 退回收货单审核
    [UFSoft.UBF.Eventing.Configuration.Failfast]

    class ApprovalRMASubscriber : UFSoft.UBF.Eventing.IEventSubscriber
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
            RMA item = key.GetEntity() as RMA;
            if (item == null)
            {
                return;
            }
            #endregion
            if (item.Status.Value != 2)
            {
                return;
            }
            DtoMO dtoMO = new DtoMO();
            dtoMO.DocType = item.DocType.Code;//单据类型取Code
            dtoMO.DocNo = item.DocNo;//单号
            dtoMO.OrderBy = "";//客户取Name
            dtoMO.OrderByWZ = "";//客户位置
            dtoMO.AC = item.AC.Name;//币种名字
            dtoMO.Date = item.BusinessDate;//出货日期
            dtoMO.dtoMOLines = new List<DtoMOLine>();
            foreach (var it in item.RMALines)
            {
                DtoMOLine dtoMOLine = new DtoMOLine();
                dtoMOLine.ItemMasterName = it.ItemInfo.ItemName;
                dtoMOLine.ItemMasterCode = it.ItemInfo.ItemCode;
                dtoMOLine.ShipQtyInvAmount = 0;
                dtoMOLine.Uom = "";//计量单位暂时没有找到，所以使用计价单位
                dtoMOLine.OrderPrice = "";//价目表价格没有找到
                dtoMOLine.FinallyPriceTC = 0;//
                dtoMOLine.TotalNetMoneyTC = 0;//
                dtoMO.dtoMOLines.Add(dtoMOLine);
            }
        }
    }
}
