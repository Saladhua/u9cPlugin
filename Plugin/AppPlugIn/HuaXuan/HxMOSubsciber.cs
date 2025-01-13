using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UFIDA.U9.Base;
using UFIDA.U9.MO.MO;
using UFSoft.UBF.Business;
using UFSoft.UBF.Util.Log;

namespace YY.U9.Cust.LI.AppPlugIn
{
    /// <summary>
    /// 生产订单接口
    /// </summary>
    [UFSoft.UBF.Eventing.Configuration.Failfast]

    class HxMOSubsciber : UFSoft.UBF.Eventing.IEventSubscriber
    {
        private static readonly ILogger logger = LoggerManager.GetLogger(typeof(HxIssueDocSubsciber));

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

            MO mO = key.GetEntity() as MO;

            if (mO == null)
            {
                return;
            }
            #endregion

            #region 调用接口
            //if (payReqFundHead.DocStatus.Value == 1 && payReqFundHead.DocStatus.Value == 0)
            if (mO.SysState == UFSoft.UBF.PL.Engine.ObjectState.Updated && mO.DocState.Value == 4)
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
                //  {
                //  "tenant": "slerelm1",
                //    "orderNumber": "H1000024",
                //    "partNumber": "23011002",
                //    "projectType":  "量产",
                //    "projectName": "三/五合一",
                //    "quantity": 1000,
                //    "customerNumber": "6132-0",
                //    "planStartDate": "2024-11-25",
                //    "planEndDate": "2024-11-25",
                //    "operationType": 0,
                //   "comment": "",
                //    "mo":[
                //        {
                //                        "partNumber": "020102040053",
                //        "quantity": 100,
                //        "moPickListId": 155336
                //    }
                //    ] 
                //} 

                #endregion

                string operation = "0";

                string tenant = "slerealm1";

                string siteName = "华旋工厂";

                StringBuilder formData = new StringBuilder();
                formData.Append("{");
                formData.Append("\"tenant\":\"" + tenant + "\",");
                formData.Append("\"orderNumber\":\"" + mO.DocNo + "\",");
                formData.Append("\"partNumber\":\"" + mO.ItemMaster.Code + "\",");
                formData.Append("\"projectType\":\"\",");
                formData.Append("\"projectName\":\"\",");
                formData.Append("\"quantity\":" + mO.ProductQty + ",");
                formData.Append("\"customerNumber\":\"\",");
                formData.Append("\"planStartDate\":\"" + mO.StartDate.ToString("yyyy-MM-dd") + "\",");
                formData.Append("\"planEndDate\":\"" + mO.CompleteDate.ToString("yyyy-MM-dd") + "\",");
                formData.Append("\"operationType\":\"0\",");
                formData.Append("\"comment\":\"" + mO.Memo + "\",");
                formData.Append("\"mo\":[");
                int i = 1;
                foreach (var item in mO.MOPickLists)
                {
                    formData.Append("{");
                    formData.Append("\"partNumber\":\"" + item.ItemMaster.Code + "\",");
                    formData.Append("\"quantity\":" + item.ActualReqQty + ",");
                    formData.Append("\"moPickListId\":\"" + item.ID + "\"");
                    formData.Append("}");
                    if (mO.MOPickLists.Count > i)
                    {
                        formData.Append(",");
                    }
                    i++;
                }
                formData.Append("]}");

                //发送格式
                StringBuilder formSendData = new StringBuilder();

                formSendData.Append(formData.ToString());

                logger.Error("生产订单新增传出数据：" + formSendData.ToString());

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

                string formSendDataGo = formSendData.ToString();

                strURL = "http://" + strURL + "/services/slemes/api/erp-orders/sync";

                string responseText = HttpRequestClient.HttpPostJson(strURL, formSendDataGo, "", "");

                #endregion
            }
            if (mO.SysState == UFSoft.UBF.PL.Engine.ObjectState.Deleted)
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
                //  {
                //  "tenant": "slerelm1",
                //    "orderNumber": "H1000024",
                //    "partNumber": "23011002",
                //    "projectType":  "量产",
                //    "projectName": "三/五合一",
                //    "quantity": 1000,
                //    "customerNumber": "6132-0",
                //    "planStartDate": "2024-11-25",
                //    "planEndDate": "2024-11-25",
                //    "operationType": 0,
                //   "comment": "",
                //    "mo":[
                //        {
                //                        "partNumber": "020102040053",
                //        "quantity": 100,
                //        "moPickListId": 155336
                //    }
                //    ] 
                //} 

                #endregion

                string operation = "1";

                string tenant = "slerealm1";

                string siteName = "华旋工厂";

                StringBuilder formData = new StringBuilder();
                formData.Append("{");
                formData.Append("\"tenant\":\"" + tenant + "\",");
                formData.Append("\"orderNumber\":\"" + mO.DocNo + "\",");
                formData.Append("\"partNumber\":\"" + mO.ItemMaster.Code + "\",");
                formData.Append("\"projectType\":\"\",");
                formData.Append("\"projectName\":\"\",");
                formData.Append("\"quantity\":" + mO.ProductQty + ",");
                formData.Append("\"customerNumber\":\"\",");
                formData.Append("\"planStartDate\":\"" + mO.StartDate.ToString("yyyy-MM-dd") + "\",");
                formData.Append("\"planEndDate\":\"" + mO.CompleteDate.ToString("yyyy-MM-dd") + "\",");
                formData.Append("\"operationType\":\"1\",");
                formData.Append("\"comment\":\"" + mO.Memo + "\",");
                formData.Append("\"mo\":[");
                int i = 1;
                foreach (var item in mO.MOPickLists)
                {
                    formData.Append("{");
                    formData.Append("\"partNumber\":\"" + item.ItemMaster.Code + "\",");
                    formData.Append("\"quantity\":" + item.ActualReqQty + ",");
                    formData.Append("\"moPickListId\":\"" + item.ID + "\"");
                    formData.Append("}");
                    if (mO.MOPickLists.Count > i)
                    {
                        formData.Append(",");
                    }
                    i++;
                }
                formData.Append("]}");

                //发送格式
                StringBuilder formSendData = new StringBuilder();

                formSendData.Append(formData.ToString());

                logger.Error("生产订单删除传出数据：" + formSendData.ToString());

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

                string formSendDataGo = formSendData.ToString();

                strURL = "http://" + strURL + "/services/slemes/api/erp-orders/sync";

                string responseText = HttpRequestClient.HttpPostJson(strURL, formSendDataGo, "", "");

 
            }
        }

    }
}
