using System;
using System.Collections.Generic;
using System.Data;
using UFIDA.U9.Base.DTOs;
using UFIDA.U9.InvDoc.MiscRcv;
using UFIDA.U9.ISV.CBO.Lot;
using UFIDA.U9.ISV.CBO.Lot.Proxy;
using UFIDA.U9.ISV.MO;
using UFIDA.U9.ISV.MO.Proxy;
using UFIDA.U9.Lot;
using UFIDA.U9.MO.MO;
using UFSoft.UBF.Business;
using UFSoft.UBF.Exceptions;
using UFSoft.UBF.Util.DataAccess;
using UFSoft.UBF.Util.Log;

namespace YY.U9.Cust.LI.AppPlugIn
{
    [UFSoft.UBF.Eventing.Configuration.Failfast]

    class Cust_MOClonseSubscriber : UFSoft.UBF.Eventing.IEventSubscriber
    {
        private static readonly ILogger logger = LoggerManager.GetLogger(typeof(Cust_MOClonseSubscriber));
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
            MO moitem = key.GetEntity() as MO;
            if (moitem == null)
            {
                return;
            }
            #endregion
            foreach (var item in moitem.MOPickLists)
            {
                //if (!string.IsNullOrEmpty(item.DescFlexField.PrivateDescSeg8))
                //{
                    #region 
                    CompleteMoProxy complete = new CompleteMoProxy();
                    List<MOOperateParamDTOData> mOOperates = new List<MOOperateParamDTOData>();
                    MOOperateParamDTOData mOOperate = new MOOperateParamDTOData(); 
                    mOOperate.MODocNo = moitem.DocNo;
                    mOOperates.Add(mOOperate);
                    List<MOOperateParamDTOData> see2222 = complete.Do();
                    #endregion
                //}
            }
        }
    }
}