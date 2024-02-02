using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Web;
using UFIDA.U9.CBO.MFG.BOM;
using UFIDA.U9.ISV.MO;
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
    class MoCopyMoPickListInsertedSubscrilber : UFSoft.UBF.Eventing.IEventSubscriber
    {
        private static readonly UFSoft.UBF.Util.Log.ILogger logger = LoggerManager.GetLogger(typeof(MoCopyMoPickListInsertedSubscrilber));
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
            MOPickList mOPick = key.GetEntity() as MOPickList;
            if (mOPick == null)
            {
                return;
            }
            #endregion

            if (mOPick.MO.DescFlexField.PrivateDescSeg30 == "备料赋值")
            {
                if (mOPick.MO.DescFlexField.PrivateDescSeg4 == "False")
                {
                    return;
                }

                if (mOPick.DescFlexField.PrivateDescSeg30 != "是")
                {
                    mOPick.Remove();
                }
            }
        }   
    }
}
