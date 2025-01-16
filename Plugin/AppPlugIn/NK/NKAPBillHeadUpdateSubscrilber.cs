using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UFIDA.U9.AP.APBill;
using UFSoft.UBF.Business;
using UFSoft.UBF.Util.DataAccess;
using UFSoft.UBF.Util.Log;

namespace YY.U9.Cust.LI.AppPlugIn
{
    /// <summary>
    /// 同步项目进度字段
    /// 赋值操作统一使用AfterDefaultValue时序进行
    /// </summary>
    [UFSoft.UBF.Eventing.Configuration.Failfast]
    class NKAPBillHeadUpdateSubscrilber : UFSoft.UBF.Eventing.IEventSubscriber
    {
        private static readonly UFSoft.UBF.Util.Log.ILogger logger = LoggerManager.GetLogger(typeof(NKAPBillHeadUpdateSubscrilber));
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
            APBillHead aPBillHead = key.GetEntity() as APBillHead;
            if (aPBillHead == null)
            {
                return;
            }
            #endregion
            //同步项目字段
            foreach (var item in aPBillHead.APBillLines)
            {
                if (item.DescFlexField.PrivateDescSeg6 == "")
                {
                    if (item.Project != null)
                    {
                        item.DescFlexField.PrivateDescSeg6 = item.Project.DescFlexField.PrivateDescSeg10;
                    }
                }
            }
        }

    }
}
