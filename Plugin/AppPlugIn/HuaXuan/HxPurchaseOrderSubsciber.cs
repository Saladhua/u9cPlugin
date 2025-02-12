
using System.Text;
using UFIDA.U9.Base;
using UFIDA.U9.PM.PO;
using UFIDA.U9.UI.PDHelper;
using UFSoft.UBF.Business;
using UFSoft.UBF.Util.Log;

namespace YY.U9.Cust.LI.AppPlugIn
{
    /// <summary>
    /// 接口
    /// 标准采购
    /// 文档：华旋WMS接口文档 V1.2
    /// </summary>
    [UFSoft.UBF.Eventing.Configuration.Failfast]
    class HxPurchaseOrderSubsciber : UFSoft.UBF.Eventing.IEventSubscriber
    {
        private static readonly ILogger logger = LoggerManager.GetLogger(typeof(HxPurchaseOrderSubsciber));

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
            if (purchaseOrder.SysState == UFSoft.UBF.PL.Engine.ObjectState.Updated && purchaseOrder.Status.Value == 1 && purchaseOrder.OriginalData.Status.Value == 0)
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

                if (purchaseOrder.BizType.Value != 326)
                {
                    //string operation = "0";

                    string tenant = "slerealm1";

                    //string siteName = "华旋工厂";

                    //StringBuilder formData = new StringBuilder();
                    //formData.Append("{");
                    //formData.Append("supplierNumber:" + purchaseOrder.Supplier.Name + ",");
                    //formData.Append("planDate:" + purchaseOrder.MaturityDate.ToString("yyyy-MM-dd") + ",");
                    //formData.Append("orderNumber:" + purchaseOrder.DocNo + ",");
                    //formData.Append("tenant:" + tenant + ",");
                    //formData.Append("type:" + "0" + ",");
                    //formData.Append("reMake:" + purchaseOrder.POMemos + ",");
                    //formData.Append("itemNumber:" + "" + ",");//不确定哪个值
                    //formData.Append("isNew:" + "True" + ",");//不确定哪个值
                    //formData.Append("list:[");
                    //int i = 0;
                    //foreach (var item in purchaseOrder.POLines)
                    //{
                    //    formData.Append("{");
                    //    formData.Append("supplierNumber:" + purchaseOrder.Supplier.Name + ",");
                    //    formData.Append("partNumber:" + item.ItemInfo.ItemCode + ",");
                    //    formData.Append("demandQuantity:" + item.ReqQtyTU + ",");
                    //    formData.Append("detailNo:" + item.DocLineNo + ",");
                    //    formData.Append("productionLine:" + item.DescFlexSegments.PrivateDescSeg2 + ",");
                    //    formData.Append("procedure:" + item.DescFlexSegments.PrivateDescSeg3 + ",");
                    //    if (item.Project != null)
                    //    {
                    //        formData.Append("itemNumber:" + item.Project.Code);
                    //    }
                    //    else
                    //    {
                    //        formData.Append("itemNumber:" + "");
                    //    }
                    //    formData.Append("}");
                    //    if (purchaseOrder.POLines.Count != i)
                    //    {
                    //        formData.Append(",");
                    //    }
                    //    i++;
                    //}
                    //formData.Append("]}");
                    StringBuilder formData = new StringBuilder();
                    formData.Append("{");
                    // 给键和值都添加双引号，并在键和值之间添加冒号，确保符合JSON格式
                    formData.Append("\"supplierNumber\":\"" + purchaseOrder.Supplier.Code + "\",");
                    if (purchaseOrder.MaturityDate != null)
                    {
                        formData.Append("\"planDate\":\"" + purchaseOrder.MaturityDate.ToString("yyyy-MM-dd") + "\",");
                    }
                    else
                    {
                        formData.Append("\"planDate\":null,");
                    }
                    formData.Append("\"orderNumber\":\"" + purchaseOrder.DocNo + "\",");
                    formData.Append("\"tenant\":\"" + tenant + "\",");
                    formData.Append("\"type\":\"0\",");
                    foreach (var item in purchaseOrder.POMemos)
                    {
                        formData.Append("\"reMake\":\"" + item.Description + "\",");
                    }
                    if (purchaseOrder.POMemos.Count == 0)
                    {
                        formData.Append("\"reMake\":\"" + "" + "\",");
                    }
                    formData.Append("\"itemNumber\":null,");
                    formData.Append("\"isNew\":\"True\",");
                    formData.Append("\"list\":[");
                    int i = 1;
                    foreach (var item in purchaseOrder.POLines)
                    {
                        formData.Append("{");
                        formData.Append("\"supplierNumber\":\"" + purchaseOrder.Supplier.Code + "\",");
                        formData.Append("\"partNumber\":\"" + item.ItemInfo.ItemCode + "\",");
                        formData.Append("\"demandQuantity\":\"" + item.ReqQtyTU + "\",");
                        formData.Append("\"detailNo\":\"" + item.DocLineNo + "\",");
                        formData.Append("\"productionLine\":\"" + item.DescFlexSegments.PrivateDescSeg2 + "\",");
                        formData.Append("\"procedure\":\"" + item.DescFlexSegments.PrivateDescSeg3 + "\",");
                        formData.Append("\"itemNumber\":\"" + item.DescFlexSegments.PubDescSeg1 + "\"");
                        formData.Append("}");
                        if (purchaseOrder.POLines.Count != i)
                        {
                            formData.Append(",");
                        }
                        i++;
                    }
                    formData.Append("]}");

                    //发送格式
                    StringBuilder formSendData = new StringBuilder();

                    formSendData.Append(formData.ToString());

                    logger.Error("标准采购新增传出数据：" + formSendData.ToString());

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
                    if (purchaseOrder.Status.Value <= 2)
                    {
                        strURL = "http://" + strURL + "/services/slewms/api/WmsOrder/PU";

                        logger.Error("标准采购新增传出数据：" + formSendData.ToString());

                        string responseText = HttpRequestClient.HttpPostJson(strURL, formSendDataGo, "", "");


                    }
                    #endregion
                }
                else
                {
                    //string operation = "0";

                    string tenant = "slerealm1";

                    //string siteName = "华旋工厂";
                    StringBuilder formData = new StringBuilder();
                    formData.Append("{");

                    // 给键和值添加双引号，并确保键值之间有冒号隔开，符合 JSON 格式要求
                    formData.Append("\"tenant\":\"" + tenant + "\",");
                    formData.Append("\"type\":\"" + "0" + "\",");

                    formData.Append("\"orderNo\":\"" + purchaseOrder.DocNo + "\",");

                    foreach (var item in purchaseOrder.POLines)
                    {
                        foreach (var itemPOShipLine in item.POShiplines)
                        {
                            formData.Append("\"planDate\":\"" + itemPOShipLine.PlanArriveDate.ToString("yyyy-MM-dd") + "\",");
                        }

                    }

                    formData.Append("\"isNew\":\"" + "True" + "\",");

                    formData.Append("\"itemNumber\":\"" + purchaseOrder.DescFlexField.PubDescSeg1 + "\",");

                    if (purchaseOrder.Supplier != null)
                    {
                        formData.Append("\"supplierCode\":\"" + purchaseOrder.Supplier.Code + "\",");
                    }
                    else
                    {
                        formData.Append("\"supplierCode\":\"" + "" + "\",");
                    }

                    formData.Append("\"list\":[");

                    int i = 1;

                    foreach (var item in purchaseOrder.POLines)
                    {
                        formData.Append("{");

                        formData.Append("\"quantity\":\"" + item.PurQtyTU + "\",");

                        formData.Append("\"partNumber\":\"" + item.ItemInfo.ItemCode + "\",");

                        formData.Append("\"productionLine\":\"" + item.DescFlexSegments.PrivateDescSeg2 + "\",");

                        formData.Append("\"procedure\":\"" + item.DescFlexSegments.PrivateDescSeg3 + "\",");

                        formData.Append("\"itemNumber\":\"" + item.DescFlexSegments.PubDescSeg1 + "\",");

                        formData.Append("\"detailNo\":\"" + item.DocLineNo + "\"");

                        formData.Append("}");

                        if (purchaseOrder.POLines.Count != i)
                        {
                            formData.Append(",");
                        }

                        i++;
                    }

                    formData.Append("]}");

                    //发送格式 
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

                    string formSendDataGo = formData.ToString();

                    strURL = "http://" + strURL + "/services/slewms/api/WmsOrder/outsourcing-entry-order/sync";

                    logger.Error("委外采购新增传出数据：" + formSendDataGo.ToString());

                    string responseText = HttpRequestClient.HttpPostJson(strURL, formSendDataGo, "", "");
                }
            }
            if (purchaseOrder.SysState == UFSoft.UBF.PL.Engine.ObjectState.Deleted)
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

                if (purchaseOrder.BizType.Value != 326)
                {
                    //string operation = "0";

                    string tenant = "slerealm1";

                    //string siteName = "华旋工厂";

                    //StringBuilder formData = new StringBuilder();
                    //formData.Append("{");
                    //formData.Append("supplierNumber:" + purchaseOrder.Supplier.Name + ",");
                    //formData.Append("planDate:" + purchaseOrder.MaturityDate.ToString("yyyy-MM-dd") + ",");
                    //formData.Append("orderNumber:" + purchaseOrder.DocNo + ",");
                    //formData.Append("tenant:" + tenant + ",");
                    //formData.Append("type:" + "0" + ",");
                    //formData.Append("reMake:" + purchaseOrder.POMemos + ",");
                    //formData.Append("itemNumber:" + "" + ",");//不确定哪个值
                    //formData.Append("isNew:" + "True" + ",");//不确定哪个值
                    //formData.Append("list:[");
                    //int i = 0;
                    //foreach (var item in purchaseOrder.POLines)
                    //{
                    //    formData.Append("{");
                    //    formData.Append("supplierNumber:" + purchaseOrder.Supplier.Name + ",");
                    //    formData.Append("partNumber:" + item.ItemInfo.ItemCode + ",");
                    //    formData.Append("demandQuantity:" + item.ReqQtyTU + ",");
                    //    formData.Append("detailNo:" + item.DocLineNo + ",");
                    //    formData.Append("productionLine:" + item.DescFlexSegments.PrivateDescSeg2 + ",");
                    //    formData.Append("procedure:" + item.DescFlexSegments.PrivateDescSeg3 + ",");
                    //    if (item.Project != null)
                    //    {
                    //        formData.Append("itemNumber:" + item.Project.Code);
                    //    }
                    //    else
                    //    {
                    //        formData.Append("itemNumber:" + "");
                    //    }
                    //    formData.Append("}");
                    //    if (purchaseOrder.POLines.Count != i)
                    //    {
                    //        formData.Append(",");
                    //    }
                    //    i++;
                    //}
                    //formData.Append("]}");
                    StringBuilder formData = new StringBuilder();
                    formData.Append("{");
                    // 给键和值都添加双引号，并在键和值之间添加冒号，确保符合JSON格式
                    formData.Append("\"orderNumber\":\"" + purchaseOrder.DocNo + "\",");
                    formData.Append("\"tenant\":\"" + tenant + "\",");
                    formData.Append("\"type\":\"1\"");
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

                    strURL = "http://" + strURL + "/services/slewms/api/WmsOrder/PU";

                    logger.Error("标准采购删除传出数据：" + formSendData.ToString());

                    string responseText = HttpRequestClient.HttpPostJson(strURL, formSendDataGo, "", "");

                    logger.Error("标准采购删除报文结果：" + responseText.ToString());
                }
                else
                {
                    //string operation = "1";

                    string tenant = "slerealm1";

                    //string siteName = "华旋工厂";
                    StringBuilder formData = new StringBuilder();

                    formData.Append("{");

                    // 给键和值添加双引号，并确保键值之间有冒号隔开，符合 JSON 格式要求
                    formData.Append("\"tenant\":\"" + tenant + "\",");

                    formData.Append("\"type\":\"" + "1" + "\",");

                    formData.Append("\"orderNo\":\"" + purchaseOrder.DocNo + "\"");

                    formData.Append("}");

                    //发送格式 
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

                    string formSendDataGo = formData.ToString();

                    strURL = "http://" + strURL + "/services/slewms/api/WmsOrder/outsourcing-entry-order/sync";

                    logger.Error("委外采购删除传出数据：" + formSendDataGo.ToString());

                    string responseText = HttpRequestClient.HttpPostJson(strURL, formSendDataGo, "", "");

                    logger.Error("标准采购删除报文结果：" + responseText.ToString());
                }
            }

        }
    }
}
