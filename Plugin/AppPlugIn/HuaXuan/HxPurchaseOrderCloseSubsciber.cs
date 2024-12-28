using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UFIDA.U9.Base;
using UFIDA.U9.PM.PO;
using UFSoft.UBF.Business;
using UFSoft.UBF.Util.Log;

namespace YY.U9.Cust.LI.AppPlugIn
{
    /// <summary>
    /// 采购关闭
    /// </summary>
    [UFSoft.UBF.Eventing.Configuration.Failfast]
    class HxPurchaseOrderCloseSubsciber : UFSoft.UBF.Eventing.IEventSubscriber
    {
        private static readonly ILogger logger = LoggerManager.GetLogger(typeof(HxPurchaseOrderCloseSubsciber));

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

            PurchaseOrder purchaseOrder = key.GetEntity() as PurchaseOrder;

            if (purchaseOrder == null)
            {
                return;
            }
            #endregion

            #region 调用接口
            //if (payReqFundHead.DocStatus.Value == 1 && payReqFundHead.DocStatus.Value == 0)
            if (purchaseOrder.SysState == UFSoft.UBF.PL.Engine.ObjectState.Updated)
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
                //{
                //    "supplierNumber":"test",
                //    "planDate":"2024-11-20",
                //    "orderNumber":"XTH112010",
                //    "tenant":"slerealm1",
                //    "type":0,
                //    "reMake":"这是一个测试",
                //    "itemNumber":"IN20241210001",
                //    "isNew":true,
                //    "list": [
                //    {
                //      "detailNo": "1",
                //      "supplierNumber":"test001",
                //      "partNumber": "020101040045",
                //      "productionLine": "旋变6线",
                //      "procedure": "匀浆",
                //      "itemNumber": "IN20241210002",
                //      "demandQuantity":5000
                //    },
                //    {
                //                        "detailNo": "2",
                //    "supplierNumber":"test001",
                //    "partNumber": "030400010035",
                //    "productionLine": "旋变6线",
                //    "procedure": "切叠",
                //    "itemNumber": "IN20241210003",
                //    "demandQuantity":5000
                //    }]
                //} 
                #endregion 

                string operation = "0";

                string tenant = "slerealm1";

                string siteName = "华旋工厂";

                StringBuilder formData = new StringBuilder();
                formData.Append("{");
                // 给键和值添加双引号，并确保键值之间有冒号隔开，符合JSON格式要求
                formData.Append("\"orderNumber\":\"" + purchaseOrder.DocNo + "\",");
                formData.Append("\"tenant\":\"" + "slerealm1" + "\"");
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

                string formSendDataGo = formSendData.ToString();

                strURL = "http://" + strURL + "/services/slewms/api/WmsOrder/PU/close";

                if (purchaseOrder.IsBizClosed == true)
                {
                    logger.Error("标准采购关闭传出数据：" + formSendData.ToString());
                    string responseText = HttpRequestClient.HttpPostJson(strURL, formSendDataGo, "", "");
                } 

                #endregion
            }

        }

    }
}
