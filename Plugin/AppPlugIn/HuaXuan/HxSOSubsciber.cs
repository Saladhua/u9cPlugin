﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UFIDA.U9.Base;
using UFIDA.U9.SM.SO;
using UFSoft.UBF.Business;
using UFSoft.UBF.Util.Log;

namespace YY.U9.Cust.LI.AppPlugIn
{
    [UFSoft.UBF.Eventing.Configuration.Failfast]
    class HxSOSubsciber : UFSoft.UBF.Eventing.IEventSubscriber
    {
        private static readonly ILogger logger = LoggerManager.GetLogger(typeof(HxSOSubsciber));

        //OA 系统地址
        public readonly static string S_PROFILE_CODE = "Z002";

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

            SO sO = key.GetEntity() as SO;

            if (sO == null)
            {
                return;
            }
            #endregion

            #region 调用接口
            //if (payReqFundHead.DocStatus.Value == 1 && payReqFundHead.DocStatus.Value == 0) 
            if (sO.SysState == UFSoft.UBF.PL.Engine.ObjectState.Updated && sO.Status.Value == 3)
            {
                //string appid = TokenManager.appid;

                //string appsecret = TokenManager.appsecret;

                //string timeFormat = DateTime.Now.ToString("yyyyMMddhhmmss");

                //Random random = new Random();
                //string number = Convert.ToString(random.Next(10000000, 99999999));

                //string transid = appid + timeFormat + number;

                //string token = TokenManager.GetAccessToken(appid, appsecret, transid);

                //if (string.IsNullOrEmpty(token))
                //{
                //    throw new Exception("未获取到Token，同步MES失败！");
                //}

                #region 报文
                //                {
                //                    "tenant":"slerealm1",
                //    "type":0,
                //    "order":{
                //                        "oderNumber":"order-20241204-001",
                //        "planDate":"2024-12-04",
                //        "comment":"备注",
                //        "supplier":"020002",
                //        "warehouseNumber":"OID241010001"
                //    },
                //    "details":[
                //        {
                //                        "lotName":"lot-20241204-001",
                //            "partNumber":"020402040130.1",
                //            "sublotName":"容器",
                //"warehouseNumber":"OID241010001"，
                //            "quantity":100
                //        }
                //    ]
                //}



                #endregion

                //string operation = "0";

                string tenant = "slerealm1";

                //string siteName = "华旋工厂";

                StringBuilder formData = new StringBuilder();
                formData.Append("{");
                formData.Append("\"tenant\":\"" + tenant + "\",");

                formData.Append("\"type\":\"0\",");

                if (sO.OrderBy != null)
                {
                    formData.Append("\"customerNumber\":\"" + sO.OrderBy.Code + "\",");
                }
                else
                {
                    formData.Append("\"customerNumber\":\"" + "" + "\",");
                }
                foreach (var item in sO.SOLines)
                {
                    foreach (var itemSOShiplines in item.SOShiplines)
                    {
                        formData.Append("\"planDate\":\"" + itemSOShiplines.RequireDate.ToString("yyyy-MM-dd") + "\",");
                    }
                }

                formData.Append("\"orderNumber\":\"" + sO.DocNo + "\",");

                formData.Append("\"orderType\":\"" + "11" + "\",");

                formData.Append("\"reMake\":\"" + sO.Memo + "\",");

                formData.Append("\"list\":[");
                int i = 1;
                foreach (var item in sO.SOLines)
                {
                    formData.Append("{");

                    formData.Append("\"customerNumber\":\"" + sO.OrderBy.Code + "\",");

                    formData.Append("\"partNumber\":\"" + item.ItemInfo.ItemID.Code + "\",");

                    formData.Append("\"lotName\":\"" + "" + "\",");

                    formData.Append("\"detailNo\":\"" + item.DocLineNo + "\",");

                    formData.Append("\"baseEntry\":\"" + "" + "\",");

                    formData.Append("\"baseline\":\"" + "" + "\",");

                    formData.Append("\"demandQuantity\":\"" + item.OrderByQtyPU + "\"");
                    formData.Append("}");
                    if (sO.SOLines.Count > i)
                    {
                        formData.Append(",");
                    }
                    i++;
                }
                formData.Append("]}");

                //发送格式
                StringBuilder formSendData = new StringBuilder();

                formSendData.Append(formData.ToString());



                string strURL = null;

                //测试
                //strURL = "http://118.195.189.35:8900/accessPlatform/platformAPI";

                //正式
                //strURL = "http://58.216.169.102:9081/ekp/sys/webservice/kmReviewWebserviceService?wsdl";


                long orgID = Context.LoginOrg.ID;


                //OA服务器地址
                string oAURL = Common.GetProfileValue(Common.S_PROFILE_CODE, orgID);

                if (string.IsNullOrEmpty(oAURL))
                {
                    return;
                }

                strURL = oAURL;

                logger.Error("销售发货新增传出数据：" + formSendData.ToString());

                string formSendDataGo = formSendData.ToString();

                strURL = "http://" + strURL + "/services/slewms/api/WmsOrder/SO";

                string responseText = HttpRequestClient.HttpPostJson(strURL, formSendDataGo, "", "");

                #endregion
            }

            if (sO.SysState == UFSoft.UBF.PL.Engine.ObjectState.Deleted)
            {
                //string appid = TokenManager.appid;

                //string appsecret = TokenManager.appsecret;

                //string timeFormat = DateTime.Now.ToString("yyyyMMddhhmmss");

                //Random random = new Random();
                //string number = Convert.ToString(random.Next(10000000, 99999999));

                //string transid = appid + timeFormat + number;

                //string token = TokenManager.GetAccessToken(appid, appsecret, transid);

                //if (string.IsNullOrEmpty(token))
                //{
                //    throw new Exception("未获取到Token，同步MES失败！");
                //}

                #region 报文
                //                {
                //                    "tenant":"slerealm1",
                //    "type":0,
                //    "order":{
                //                        "oderNumber":"order-20241204-001",
                //        "planDate":"2024-12-04",
                //        "comment":"备注",
                //        "supplier":"020002",
                //        "warehouseNumber":"OID241010001"
                //    },
                //    "details":[
                //        {
                //                        "lotName":"lot-20241204-001",
                //            "partNumber":"020402040130.1",
                //            "sublotName":"容器",
                //"warehouseNumber":"OID241010001"，
                //            "quantity":100
                //        }
                //    ]
                //}



                #endregion

                //string operation = "0";

                string tenant = "slerealm1";

                //string siteName = "华旋工厂";

                StringBuilder formData = new StringBuilder();
                formData.Append("{");
                formData.Append("\"tenant\":\"" + tenant + "\",");

                formData.Append("\"type\":\"1\",");

                formData.Append("\"orderNumber\":\"" + sO.DocNo + "\"");

                formData.Append("}");

                //发送格式
                StringBuilder formSendData = new StringBuilder();

                formSendData.Append(formData.ToString());



                string strURL = null;

                //测试
                //strURL = "http://118.195.189.35:8900/accessPlatform/platformAPI";

                //正式
                //strURL = "http://58.216.169.102:9081/ekp/sys/webservice/kmReviewWebserviceService?wsdl";


                long orgID = Context.LoginOrg.ID;


                //OA服务器地址
                string oAURL = Common.GetProfileValue(Common.S_PROFILE_CODE, orgID);

                if (string.IsNullOrEmpty(oAURL))
                {
                    return;
                }

                strURL = oAURL;

                logger.Error("销售发货删除传出数据：" + formSendData.ToString());

                string formSendDataGo = formSendData.ToString();

                strURL = "http://" + strURL + "/services/slewms/api/WmsOrder/SO";

                string responseText = HttpRequestClient.HttpPostJson(strURL, formSendDataGo, "", "");


            }

        }
    }
}
