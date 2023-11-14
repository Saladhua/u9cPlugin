using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Web;
using UFIDA.U9.MO.MO;
using UFIDA.U9.SM.ForecastOrder;
using UFIDA.U9.SM.SO;
using UFSoft.UBF.Business;
using UFSoft.UBF.Util.DataAccess;
using UFSoft.UBF.Util.Log;

namespace YY.U9.Cust.LI.AppPlugIn
{
    /// <summary>
    /// 创联-生产订单备料复制
    /// </summary>
    [UFSoft.UBF.Eventing.Configuration.Failfast]
    class MoCopyChangeSubscrilber : UFSoft.UBF.Eventing.IEventSubscriber
    {
        private static readonly UFSoft.UBF.Util.Log.ILogger logger = LoggerManager.GetLogger(typeof(MoCopyChangeSubscrilber));

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
            MO mo = key.GetEntity() as MO;
            if (mo == null)
            {
                return;
            }
            #endregion
            string MOID = "";

            if (HttpContext.Current.Session["MOID"] != null)
            {
                MOID = HttpContext.Current.Session["MOID"].ToString();
                HttpContext.Current.Session["MOID"] = null;
            }
            MO mO = new MO();
            if (!string.IsNullOrEmpty(MOID))
            {
                mO = MO.Finder.FindByID(Convert.ToInt64(MOID));
            }
            else
            {
                return;
            }

            if (mo.DescFlexField.PrivateDescSeg4 == "False")
            {
                return;
            }

            #region 取数-赋值
            List<DotSet> dotSets = new List<DotSet>();
            mo.MOPickLists.Clear();

            foreach (var item in mO.MOPickLists)
            {
                mo.MOPickLists.Add(item);
            }
            #endregion

            long see = mo.ID;
            string se12 = mo.DocNo;
            #region 赋值
            foreach (var item in mo.MOPickLists)
            {
                string see123 = item.MO.DocNo;
            }

            #endregion
        }

        public class DotSet
        {
            /// <summary>
            /// 料品ID
            /// </summary>
            public long ItemID { get; set; }
            /// <summary>
            /// 料品Code
            /// </summary>
            public string ItemCode { get; set; }
            /// <summary>
            /// 料品Name
            /// </summary>
            public string ItemName { get; set; }
            /// <summary>
            /// BOM需求数量
            /// </summary>
            public decimal BOMReqQty { get; set; }
            /// <summary>
            /// 实际需求数量
            /// </summary>
            public decimal ActualReqQty { get; set; }

        }

    }
}
