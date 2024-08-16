using System;
using System.Collections.Generic;
using System.Linq;
using UFIDA.U9.PM.PO;
using UFIDA.U9.UI.PDHelper;
using UFSoft.UBF.Business;
using UFSoft.UBF.Util.Log;

namespace YY.U9.Cust.LI.AppPlugIn
{
    /// <summary>
    /// 纳科--将最低采购价格(最终价)，回写到行上
    /// </summary>
    [UFSoft.UBF.Eventing.Configuration.Failfast]
    class NKLIPurchaseOrderInsertedSubscrilber : UFSoft.UBF.Eventing.IEventSubscriber
    {
        private static readonly UFSoft.UBF.Util.Log.ILogger logger = LoggerManager.GetLogger(typeof(NKLIPurchaseOrderInsertedSubscrilber));

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
            PurchaseOrder purchaseOrder = key.GetEntity() as PurchaseOrder;
            if (purchaseOrder == null)
            {
                return;
            }
            #endregion

            //获取供应商
            string SupID = "";

            //料品ID
            string itemID = "";

            string price = "0";


            //带组织过滤--只有邢台组织才行


            string orgcode = purchaseOrder.Org.Code;

            if (orgcode != "10")
            {
                return;
            }

            SupID = purchaseOrder.Supplier.Supplier.ID.ToString();

            foreach (var item in purchaseOrder.POLines)
            {

                itemID = item.ItemInfo.ItemID.Code;

                price = FindLowPrice(SupID, itemID, item.ID);

                item.DescFlexSegments.PrivateDescSeg18 = price;//本地是三,纳科环境是18

            }

        }

        public string FindLowPrice(string Sup, string Item, long lineID)
        {
            string price = "0";

            decimal finallyPriceTc = 0;

            POLine.EntityList poLines = POLine.Finder.FindAll("ItemInfo.ItemCode='" + Item + "'");

            List<Dto> dtos = new List<Dto>();

            if (poLines != null)
            {
                foreach (var itemPoLines in poLines)
                {

                    Dto dto = new Dto();
                    dto.Price = itemPoLines.FinallyPriceTC;
                    dtos.Add(dto);

                }
            }
            if (dtos.Count != 0)
            {
                finallyPriceTc = dtos.Min(x => x.Price);
            }
            else
            {
                finallyPriceTc = 0;
            }

            price = Math.Round(finallyPriceTc, 3).ToString();

            return price;
        }


        /// <summary>
        /// 数据
        /// </summary>
        public class Dto
        {
            public decimal Price { get; set; }
        }
    }

}
