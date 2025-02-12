using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UFIDA.U9.Base;
using UFIDA.U9.MO.Issue;
using UFIDA.U9.PM.PMIssue;
using UFSoft.UBF.Business;
using UFSoft.UBF.Util.Log;

namespace YY.U9.Cust.LI.AppPlugIn
{
    /// <summary>
    /// 非成套领料接口
    /// </summary>
    [UFSoft.UBF.Eventing.Configuration.Failfast]
    class HxIssueDocSubsciber : UFSoft.UBF.Eventing.IEventSubscriber
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

            PMIssueDoc issueDoc = key.GetEntity() as PMIssueDoc;

            if (issueDoc == null)
            {
                return;
            }
            #endregion

            #region 调用接口
            //if (payReqFundHead.DocStatus.Value == 1 && payReqFundHead.DocStatus.Value == 0) 
            if (issueDoc.SysState == UFSoft.UBF.PL.Engine.ObjectState.Updated && issueDoc.DocState.Value == 1 && issueDoc.OriginalData.DocState.Value == 0)
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

                string SupplierCode = "";

                string WhCode = "";

                string got = "F";

                foreach (var item in issueDoc.IssueDocLines)
                {
                    if (item.SubconstractSupplier != null)
                    {
                        SupplierCode = item.SubconstractSupplier.Code;
                    }
                    if (item.Wh != null)
                    {
                        WhCode = item.Wh.Code;
                    }

                    try
                    {
                        string RecedeReasonName = item.RecedeReason.Name;

                        if (!string.IsNullOrEmpty(RecedeReasonName))
                        {
                            got = "T";//代表是委外退料调用委外半成品入库
                        }
                    }
                    catch (Exception ex)
                    {
                        string mes = ex.Message;
                        got = "F";
                    }

                }

                if (got == "F")
                {

                    //string operation = "0";

                    string tenant = "slerealm1";

                    //string siteName = "华旋工厂";

                    StringBuilder formData = new StringBuilder();
                    formData.Append("{");
                    formData.Append("\"tenant\":\"" + tenant + "\",");
                    formData.Append("\"type\":\"0\",");
                    formData.Append("\"order\":{");
                    formData.Append("\"orderNo\":\"" + issueDoc.DocNo + "\",");
                    formData.Append("\"planDate\":\"" + issueDoc.BusinessCreatedOn.ToString("yyyy-MM-dd") + "\",");
                    formData.Append("\"comment\":\"" + issueDoc.Memo + "\",");


                    if (!string.IsNullOrEmpty(SupplierCode))
                    {
                        formData.Append("\"supplierCode\":\"" + SupplierCode + "\",");
                    }
                    else
                    {
                        formData.Append("\"supplierCode\":\"" + "" + "\",");
                    }

                    if (!string.IsNullOrEmpty(WhCode))
                    {
                        formData.Append("\"warehouseNumber\":\"" + WhCode + "\"");
                    }
                    else
                    {
                        formData.Append("\"warehouseNumber\":\"" + "" + "\"");
                    }

                    formData.Append("  },");
                    formData.Append("\"details\":[");
                    int i = 1;
                    foreach (var item in issueDoc.IssueDocLines)
                    {
                        formData.Append("{");
                        if (item.LotMaster != null)
                        {
                            formData.Append("\"lotName\":\"" + item.LotMaster.LotCode + "\",");
                        }
                        else
                        {
                            formData.Append("\"lotName\":\"" + "1" + "\",");
                        }
                        formData.Append("\"partNumber\":\"" + item.ItemInfo.ItemCode + "\",");
                        formData.Append("\"detailNo\":\"" + item.LineNum + "\",");
                        if (item.Wh != null)
                        {
                            formData.Append("\"warehouseNumber\":\"" + item.Wh.Code + "\",");
                        }
                        else
                        {
                            formData.Append("\"warehouseNumber\":\"\",");
                        }
                        formData.Append("\"quantity\":\"" + item.IssuedQty + "\"");
                        formData.Append("}");
                        if (issueDoc.IssueDocLines.Count > i)
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

                    logger.Error("非成套领料新增传出数据：" + formSendData.ToString());

                    string formSendDataGo = formSendData.ToString();


                    strURL = "http://" + strURL + "/services/slewms/api/WmsOrder/outsourcing-pick-order/sync";

                    if (got == "T")
                    {
                        strURL = "http://" + strURL + "/services/slewms/api/WmsOrder/outsourcing-entry-order/sync";
                    }
                    string responseText = HttpRequestClient.HttpPostJson(strURL, formSendDataGo, "", "");
                }
                else if (got == "T")
                {
                    //string operation = "0";

                    string tenant = "slerealm1";

                    //string siteName = "华旋工厂";

                    StringBuilder formData = new StringBuilder();
                    formData.Append("{");
                    formData.Append("\"tenant\":\"" + tenant + "\",");
                    formData.Append("\"type\":\"0\",");
                    formData.Append("\"orderNo\":\"" + issueDoc.DocNo + "\",");
                    formData.Append("\"planDate\":\"" + issueDoc.BusinessCreatedOn.ToString("yyyy-MM-dd") + "\",");
                    if (!string.IsNullOrEmpty(SupplierCode))
                    {
                        formData.Append("\"supplierCode\":\"" + SupplierCode + "\",");
                    }
                    else
                    {
                        formData.Append("\"supplierCode\":\"" + "" + "\",");
                    }

                    formData.Append("\"list\":[");
                    int i = 1;
                    foreach (var item in issueDoc.IssueDocLines)
                    {
                        formData.Append("{");

                        formData.Append("\"partNumber\":\"" + item.ItemInfo.ItemCode + "\",");

                        formData.Append("\"detailNo\":\"" + item.LineNum + "\",");


                        if (item.Wh != null)
                        {
                            formData.Append("\"warehouseNumber\":\"" + item.Wh.Code + "\",");
                        }
                        else
                        {
                            formData.Append("\"warehouseNumber\":\"" + "" + "\",");
                        }


                        if (item.LotMaster != null)
                        {
                            formData.Append("\"lotName\":\"" + item.LotMaster.LotCode + "\",");
                        }
                        else
                        {
                            formData.Append("\"lotName\":\"" + "" + "\",");
                        }

                        formData.Append("\"quantity\":\"" + item.IssuedQty + "\"");
                        formData.Append("}");
                        if (issueDoc.IssueDocLines.Count > i)
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

                    logger.Error("委外退料新增传出数据：" + formSendData.ToString());

                    string formSendDataGo = formSendData.ToString();

                    strURL = "http://" + strURL + "/services/slewms/api/WmsOrder/outsourcing-return-material-order/sync";


                    string responseText = HttpRequestClient.HttpPostJson(strURL, formSendDataGo, "", "");

                }

                #endregion
            }
            if (issueDoc.SysState == UFSoft.UBF.PL.Engine.ObjectState.Deleted)
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

                string SupplierCode = "";

                string WhCode = "";

                string got = "F";

                foreach (var item in issueDoc.IssueDocLines)
                {
                    if (item.SubconstractSupplier != null)
                    {
                        SupplierCode = item.SubconstractSupplier.Code;
                    }
                    if (item.Wh != null)
                    {
                        WhCode = item.Wh.Code;
                    }

                    try
                    {
                        string RecedeReasonName = item.RecedeReason.Name;

                        if (!string.IsNullOrEmpty(RecedeReasonName))
                        {
                            got = "T";//代表是委外退料调用委外半成品入库
                        }
                    }
                    catch (Exception ex)
                    {
                        string meses = ex.Message;
                        got = "F";
                    }

                }


                int see = issueDoc.IssueDocType.BusinessType.Value;

                if (see != 326)
                {

                    //string operation = "0";

                    string tenant = "slerealm1";

                    //string siteName = "华旋工厂";

                    StringBuilder formData = new StringBuilder();
                    formData.Append("{");
                    formData.Append("\"tenant\":\"" + tenant + "\",");
                    formData.Append("\"type\":\"1\",");
                    formData.Append("\"order\":{");
                    formData.Append("\"orderNo\":\"" + issueDoc.DocNo + "\"");
                    formData.Append("}");
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

                    logger.Error("非成套领料删除传出数据：" + formSendData.ToString());

                    string formSendDataGo = formSendData.ToString();


                    strURL = "http://" + strURL + "/services/slewms/api/WmsOrder/outsourcing-pick-order/sync";

                    if (got == "T")
                    {
                        strURL = "http://" + strURL + "/services/slewms/api/WmsOrder/outsourcing-entry-order/sync";
                    }

                    string responseText = HttpRequestClient.HttpPostJson(strURL, formSendDataGo, "", "");

                    logger.Error("非成套领料删除返回报文：" + responseText.ToString());
                }
                else if (see == 326)
                {
                    //string operation = "0";

                    string tenant = "slerealm1";

                    //string siteName = "华旋工厂";

                    StringBuilder formData = new StringBuilder();
                    formData.Append("{");
                    formData.Append("\"tenant\":\"" + tenant + "\",");
                    formData.Append("\"type\":\"1\",");
                    formData.Append("\"orderNo\":\"" + issueDoc.DocNo + "\"");
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

                    logger.Error("委外退料删除传出数据：" + formSendData.ToString());

                    string formSendDataGo = formSendData.ToString();

                    strURL = "http://" + strURL + "/services/slewms/api/WmsOrder/outsourcing-return-material-order/sync";

                    string responseText = HttpRequestClient.HttpPostJson(strURL, formSendDataGo, "", "");

                    logger.Error("委外退料删除返回报文:" + responseText.ToString());

                }

            }

        }
    }
}
