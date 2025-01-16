using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UFIDA.U9.Base.FlexField.ValueSet;
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
    class NKMiscShipUpdateSubscrilber : UFSoft.UBF.Eventing.IEventSubscriber
    {
        private static readonly UFSoft.UBF.Util.Log.ILogger logger = LoggerManager.GetLogger(typeof(NKMiscShipUpdateSubscrilber));
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
            MiscShipment miscShip = key.GetEntity() as MiscShipment;
            if (miscShip == null)
            {
                return;
            }
            #endregion
            //同步项目字段
            foreach (var item in miscShip.MiscShipLs)
            {
                if (item.DescFlexSegments.PrivateDescSeg6 == "")
                {
                    if (item.Project != null)
                    {
                        item.DescFlexSegments.PrivateDescSeg6 = item.Project.DescFlexField.PrivateDescSeg10;
                    } 
                }
            }
        }
    }
}
