using System;
using System.Collections.Generic;
using UFIDA.U9.SM.Ship;
using UFSoft.UBF.Business;
using UFSoft.UBF.Util.Log;

namespace YY.U9.Cust.LI.AppPlugIn
{
    /// <summary>
    /// 柚见项目
    /// 位置柚见接口开发列表(1).xlsx - 第12条
    /// 出库单弃审，同步甩手删除出货单
    /// </summary>
    [UFSoft.UBF.Eventing.Configuration.Failfast]
    class ApprovalMOSubscriber : UFSoft.UBF.Eventing.IEventSubscriber
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
            Ship item = key.GetEntity() as Ship;
            if (item == null)
            {
                return;
            }
            #endregion
            //状态为已审核，当状态在有变化就是弃审，就go
            if (item.Status.Value != 3)
            {
                return;
            }
            DtoMO dtoMO = new DtoMO();
            dtoMO.DocType = item.DocType.Code;//单据类型取Code
            dtoMO.DocNo = item.DocNo;//单号
            dtoMO.OrderBy = item.OrderBy.Name;//客户取Name
            dtoMO.OrderByWZ = "";//客户位置
            dtoMO.AC = item.AC.Name;//币种名字
            dtoMO.Date = item.BusinessDate;//出货日期
            dtoMO.ShipConfirmDate = item.ShipConfirmDate;//出货确认日期
            dtoMO.ShipMemo = item.ShipMemo;//备注
            dtoMO.dtoMOLines = new List<DtoMOLine>();
            foreach (var it in item.ShipLines)
            {
                DtoMOLine dtoMOLine = new DtoMOLine();
                dtoMOLine.ItemMasterName = it.ItemInfo.ItemName;
                dtoMOLine.ItemMasterCode = it.ItemInfo.ItemCode;
                dtoMOLine.ShipQtyInvAmount = it.ShipQtyInvAmount;
                dtoMOLine.Uom = it.PriceUom.Code;//计量单位暂时没有找到，所以使用计价单位
                dtoMOLine.OrderPrice = "";//价目表价格没有找到
                dtoMOLine.FinallyPriceTC = it.FinallyPriceTC;//
                dtoMOLine.TotalNetMoneyTC = it.TotalNetMoneyTC;//
                dtoMO.dtoMOLines.Add(dtoMOLine);
            }
        }
    }


    public class DtoMO
    {
        /// <summary>
        /// 单据类型
        /// </summary>
        public string DocType { get; set; }

        /// <summary>
        /// 单号
        /// </summary>
        public string DocNo { get; set; }

        /// <summary>
        /// 客户
        /// </summary>
        public string OrderBy { get; set; }
        /// <summary>
        /// 客户位置
        /// </summary>
        public string OrderByWZ { get; set; }

        /// <summary>
        /// 币种
        /// </summary>
        public string AC { get; set; }
        /// <summary>
        /// 日期
        /// </summary>
        public DateTime Date { get; set; }

        /// <summary>
        /// 出货确认日
        /// </summary>
        public DateTime ShipConfirmDate { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string ShipMemo { get; set; }
        /// <summary>
        /// 出货行内容
        /// </summary>
        public List<DtoMOLine> dtoMOLines;

    }

    public class DtoMOLine
    {
        /// <summary>
        /// 料品
        /// </summary>
        public string ItemMasterCode { get; set; }
        /// <summary>
        /// 料品品名
        /// </summary>
        public string ItemMasterName { get; set; }

        /// <summary>
        /// 出货数量
        /// </summary>
        public decimal ShipQtyInvAmount { get; set; }
        /// <summary>
        /// 单位
        /// </summary>
        public string Uom { get; set; }
        /// <summary>
        /// 定价
        /// </summary>
        public string OrderPrice { get; set; }
        /// <summary>
        /// 最终价
        /// </summary>
        public decimal FinallyPriceTC { get; set; }
        /// <summary>
        /// 未税金额
        /// </summary>
        public decimal TotalNetMoneyTC { get; set; }
        /// <summary>
        /// 存储地点
        /// </summary>
        public string WH { get; set; }

    }
}
