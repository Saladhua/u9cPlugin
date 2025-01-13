using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using UFIDA.U9.Base.DTOs;
using UFIDA.U9.InvDoc.MiscRcv;
using UFIDA.U9.ISV.CBO.Lot;
using UFIDA.U9.ISV.CBO.Lot.Proxy;
using UFIDA.U9.Lot;
using UFIDA.U9.MO.MO;
using UFIDA.U9.SM.Ship;
using UFIDA.U9.SM.SO;
using UFSoft.UBF.Business;
using UFSoft.UBF.Exceptions;
using UFSoft.UBF.Util.DataAccess;
using UFSoft.UBF.Util.Log;


namespace YY.U9.Cust.LI.AppPlugIn
{
    /// <summary>
    /// 销售订单删除
    /// </summary>
    [UFSoft.UBF.Eventing.Configuration.Failfast]
    class ShipDeleteSubscriber : UFSoft.UBF.Eventing.IEventSubscriber
    {
        private static readonly ILogger logger = LoggerManager.GetLogger(typeof(ShipDeleteSubscriber));
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

            Ship ship = key.GetEntity() as Ship;

            if (ship == null)
            {
                return;
            }

            #endregion

            //if (bOMMaster.SysState == UFSoft.UBF.PL.Engine.ObjectState.Updated)
            if (ship.SysState == UFSoft.UBF.PL.Engine.ObjectState.Deleted)
            {
                string bwen1 = "";
                string bwen2 = "";
                try
                {
                    string ApiTokenAndID = JHBassApiData.GetApiTokenAndID();
                    // 使用 Newtonsoft.Json 库中的 JObject 来解析 JSON 字符串
                    JObject obj = JObject.Parse(ApiTokenAndID);
                    bwen1 = obj.ToString();
                    // 获取 corpAccessToken 的值
                    string corpAccessToken = (string)obj["corpAccessToken"];
                    // 获取 corpId 的值
                    string corpId = (string)obj["corpId"];

                    string strulr = "https://open.fxiaoke.com/cgi/crm/v2/data/query";

                    string fieldValue = ship.DocNo;//单号

                    string dataObjectApiName = "object_Kn5Cd__c";//实体

                    string fieldName = "name";//实体字段

                    string CrmData = JHBassApiData.GetDatas(strulr, corpAccessToken, corpId, fieldValue, dataObjectApiName, fieldName);

                    JObject objCrmID = JObject.Parse(CrmData);
                    // 获取 corpAccessToken 的值
                    string CrmID = "";

                    try
                    {
                        CrmID = (string)objCrmID["data"]["dataList"][0]["_id"];
                    }
                    catch (Exception ex)
                    {
                        logger.Error("删除数据：" + CrmData.ToString() + "单号：" + ship.DocNo + ex.ToString());
                        return;
                    }

                    string InvaDate = JHBassApiData.InvaDate(strulr, corpAccessToken, corpId, CrmID, dataObjectApiName);

                    JObject ReturnDate = JObject.Parse(InvaDate); 

                    bwen2 = ReturnDate.ToString();
                }
                catch (Exception ex)
                {
                    logger.Error("删除数据报文1：" + bwen1 + "删除数据报文2：" + bwen2);
                }
            }
        }
    }
}
