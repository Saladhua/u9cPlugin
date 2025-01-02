using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UFIDA.U9.Base;
using UFIDA.U9.PM.Rcv;
using UFSoft.UBF.Business;
using UFSoft.UBF.Util.Log;

namespace YY.U9.Cust.LI.AppPlugIn
{
    /// <summary>
    /// 采购退货
    /// </summary>
    [UFSoft.UBF.Eventing.Configuration.Failfast]
    class HxReceivementSubsciber : UFSoft.UBF.Eventing.IEventSubscriber
    {
        private static readonly ILogger logger = LoggerManager.GetLogger(typeof(HxReceivementSubsciber));

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

            Receivement receivement = key.GetEntity() as Receivement;

            if (receivement == null)
            {
                return;
            }
            #endregion

            #region 调用接口
            //if (payReqFundHead.DocStatus.Value == 1 && payReqFundHead.DocStatus.Value == 0) 
            if (receivement.SysState == UFSoft.UBF.PL.Engine.ObjectState.Updated && receivement.OriginalData.Status.Value == 0)
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
                //                    "supplierNumber": "test001",
                //  "planDate": "2024-11-25",
                //  "orderNumber": "11010101-1",
                //  "type": 0,
                //  "reMake": "测试用例",
                //  "list": [
                //    {
                //                        "supplierNumber": "test001",
                //      "partNumber": "030300010130",
                //      "lotName": "20241126001",
                //      "demandQuantity": 1000,
                //      "warehouseNumber": "OID20241126001",
                //      "detailNo": 1
                //    }
                //  ]
                //}

                #endregion


                if (receivement.BizType.Value != 326)
                {
                    string operation = "0";

                    string tenant = "slerealm1";

                    string siteName = "华旋工厂";

                    StringBuilder formData = new StringBuilder();

                    formData.Append("{");

                    // 给键和值添加双引号，并确保键值之间有冒号隔开，符合JSON格式要求


                    formData.Append("\"tenant\":\"" + tenant + "\",");

                    foreach (var item in receivement.RcvLines)
                    {
                        formData.Append("\"planDate\":\"" + item.PlanArrivedDate.ToString("yyyy-MM-dd") + "\",");  // 如果这里需要根据实际情况填充正确的值，可以后续修改此处逻辑
                    }

                    formData.Append("\"orderNumber\":\"" + receivement.DocNo + "\",");

                    formData.Append("\"type\":\"" + "0" + "\",");

                    string supCode = "";

                    if (receivement.Supplier != null)
                    {
                        formData.Append("\"supplierNumber\":\"" + receivement.Supplier.Code + "\",");
                        supCode = receivement.Supplier.Code;
                    }
                    else
                    {
                        formData.Append("\"supplierNumber\":\"" + "" + "\",");
                        supCode = "";
                    }

                    formData.Append("\"list\":[");

                    int i = 1;

                    foreach (var item in receivement.RcvLines)
                    {
                        formData.Append("{");

                        formData.Append("\"supplierNumber\":\"" + supCode + "\",");

                        formData.Append("\"partNumber\":\"" + item.ItemInfo.ItemCode + "\",");

                        formData.Append("\"demandQuantity\":\"" + item.RejectQtyPU + "\",");

                        if (item.InvLot != null)
                        {
                            formData.Append("\"lotName\":\"" + item.InvLot.LotCode + "\",");
                        }
                        else
                        {
                            formData.Append("\"lotName\":\"" + "1" + "\",");
                        }

                        if (item.Wh != null)
                        {
                            formData.Append("\"warehouseNumber\":\"" + item.Wh.Code + "\",");
                        }
                        else
                        {
                            formData.Append("\"warehouseNumber\":\"" + "" + "\",");
                        }

                        formData.Append("\"detailNo\":\"" + item.DocLineNo + "\"");

                        formData.Append("}");
                        if (receivement.RcvLines.Count != i)
                        {
                            formData.Append(",");
                        }
                        i++;
                    }

                    formData.Append("]}");

                    //发送格式
                    StringBuilder formSendData = new StringBuilder();

                    formSendData.Append(formData.ToString());

                    logger.Error("采购退货新增传出数据：" + formSendData.ToString());

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

                    strURL = "http://" + strURL + "/services/slewms/api/WmsOrder/PR";

                    string responseText = HttpRequestClient.HttpPostJson(strURL, formSendDataGo, "", "");

                }
                else//委外退货
                {
                    string operation = "0";

                    string tenant = "slerealm1";

                    string siteName = "华旋工厂";

                    StringBuilder formData = new StringBuilder();

                    formData.Append("{");

                    // 给键和值添加双引号，并确保键值之间有冒号隔开，符合 JSON 格式要求
                    formData.Append("\"tenant\":\"" + tenant + "\",");
                    formData.Append("\"type\":\"" + "0" + "\",");

                    formData.Append("\"orderNo\":\"" + receivement.DocNo + "\",");

                    foreach (var item in receivement.RcvLines)
                    {
                        formData.Append("\"planDate\":\"" + item.ArrivedTime.ToString("yyyy-MM-dd") + "\",");
                    }

                    formData.Append("\"isNew\":\"" + "True" + "\",");

                    formData.Append("\"itemNumber\":\"" + receivement.DescFlexField.PubDescSeg1 + "\",");

                    if (receivement.Supplier != null)
                    {
                        formData.Append("\"supplierCode\":\"" + receivement.Supplier.Code + "\",");
                    }
                    else
                    {
                        formData.Append("\"supplierCode\":\"" + "" + "\",");
                    }

                    formData.Append("\"list\":[");

                    int i = 1;

                    foreach (var item in receivement.RcvLines)
                    {
                        formData.Append("{");

                        formData.Append("\"quantity\":\"" + item.RcvQtyTU + "\",");

                        formData.Append("\"partNumber\":\"" + item.ItemInfo.ItemCode + "\",");

                        formData.Append("\"productionLine\":\"" + item.DescFlexSegments.PrivateDescSeg2 + "\",");

                        formData.Append("\"procedure\":\"" + item.DescFlexSegments.PrivateDescSeg3 + "\",");

                        formData.Append("\"itemNumber\":\"" + item.DescFlexSegments.PubDescSeg1 + "\",");

                        formData.Append("\"detailNo\":\"" + item.DocLineNo + "\"");

                        formData.Append("}");

                        if (receivement.RcvLines.Count != i)
                        {
                            formData.Append(",");
                        }

                        i++;
                    }

                    formData.Append("]}");

                    //发送格式
                    StringBuilder formSendData = new StringBuilder();

                    formSendData.Append(formData.ToString());

                    logger.Error("委外收货新增传出数据：" + formSendData.ToString());

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

                    strURL = "http://" + strURL + "/services/slewms/api/WmsOrder/outsourcing-entry-order/sync";

                    string responseText = HttpRequestClient.HttpPostJson(strURL, formSendDataGo, "", "");
                }
                #endregion
            }

        }

    }
}
