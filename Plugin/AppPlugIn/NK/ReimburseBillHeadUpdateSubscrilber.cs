using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UFIDA.U9.ER.ReimburseBill;
using UFIDA.U9.InvDoc.MiscShip;
using UFSoft.UBF.Business;
using UFSoft.UBF.Util.DataAccess;
using UFSoft.UBF.Util.Log;

namespace YY.U9.Cust.LI.AppPlugIn
{
    /// <summary>
    /// 同步项目进度字段
    /// </summary>
    [UFSoft.UBF.Eventing.Configuration.Failfast]
    class ReimburseBillHeadUpdateSubscrilber : UFSoft.UBF.Eventing.IEventSubscriber
    {
        private static readonly UFSoft.UBF.Util.Log.ILogger logger = LoggerManager.GetLogger(typeof(ReimburseBillHeadUpdateSubscrilber));
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

            ReimburseBillHead reimburseBillHead = key.GetEntity() as ReimburseBillHead;
            if (reimburseBillHead == null)
            {
                return;
            }
            #endregion
            //同步项目字段
            foreach (var item in reimburseBillHead.ReimbuurseBillDetails)
            {
                if (item.DescFlexField.PrivateDescSeg10 == "")
                {
                    if (item.ExpensePayProject != null)
                    {
                        item.DescFlexField.PrivateDescSeg10 = item.ExpensePayProject.DescFlexField.PrivateDescSeg10;
                    }
                }
            }
        }
    }
}
