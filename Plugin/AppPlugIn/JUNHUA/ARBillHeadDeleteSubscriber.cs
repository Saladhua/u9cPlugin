using Newtonsoft.Json.Linq;
using System;
using UFIDA.U9.AR.ARBill;
using UFIDA.U9.PM.Rcv;
using UFIDA.U9.SM.Quotation;
using UFIDA.U9.SM.Ship;
using UFIDA.U9.SM.SO;
using UFSoft.UBF.Business;
using UFSoft.UBF.Exceptions;
using UFSoft.UBF.Util.DataAccess;
using UFSoft.UBF.Util.Log;

namespace YY.U9.Cust.LI.AppPlugIn
{
    /// <summary>
    /// 应收单
    /// </summary>
    [UFSoft.UBF.Eventing.Configuration.Failfast]
    class ARBillHeadDeleteSubscriber : UFSoft.UBF.Eventing.IEventSubscriber
    {
        private static readonly ILogger logger = LoggerManager.GetLogger(typeof(ARBillHeadDeleteSubscriber));
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

            ARBillHead aRBillHead = key.GetEntity() as ARBillHead;

            if (aRBillHead == null)
            {
                return;
            }

            #endregion

            //if (bOMMaster.SysState == UFSoft.UBF.PL.Engine.ObjectState.Updated)
            if (aRBillHead.SysState == UFSoft.UBF.PL.Engine.ObjectState.Deleted)
            {
                string ApiTokenAndID = JHBassApiData.GetApiTokenAndID();
                // 使用 Newtonsoft.Json 库中的 JObject 来解析 JSON 字符串
                JObject obj = JObject.Parse(ApiTokenAndID);
                // 获取 corpAccessToken 的值
                string corpAccessToken = (string)obj["corpAccessToken"];
                // 获取 corpId 的值
                string corpId = (string)obj["corpId"];

                string strulr = "https://open.fxiaoke.com/cgi/crm/v2/data/query";

                string fieldValue = aRBillHead.DocNo;//单号 

                string dataObjectApiName = "object_C2Pcu__c";//实体

                string fieldName = "name";//实体字段 

                string CrmData = JHBassApiData.GetDatas(strulr, corpAccessToken, corpId, fieldValue, dataObjectApiName, fieldName);

                JObject objCrmID = JObject.Parse(CrmData);

                string CrmID = "";

                try
                {
                    CrmID = (string)objCrmID["data"]["dataList"][0]["_id"];
                }
                catch (Exception ex)
                {
                    logger.Error("删除数据：" + CrmData.ToString() + "单号：" + aRBillHead.DocNo + ex.ToString());
                    return;
                }
                // 获取 corpAccessToken 的值


                string Invastrulr = "https://open.fxiaoke.com/cgi/crm/custom/v2/data/invalid";

                string InvaDate = JHBassApiData.InvaDate(Invastrulr, corpAccessToken, corpId, CrmID, dataObjectApiName);

                JObject returnDate = JObject.Parse(InvaDate);

                logger.Error("删除数据：" + returnDate.ToString() + "单号：" + aRBillHead.DocNo);

            }
        }
    }
}
