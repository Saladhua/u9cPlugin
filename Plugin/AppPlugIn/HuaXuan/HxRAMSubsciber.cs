using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UFIDA.U9.Base;
using UFIDA.U9.PM.Rcv;
using UFIDA.U9.SM.RMA;
using UFSoft.UBF.Business;
using UFSoft.UBF.Util.Log;

namespace YY.U9.Cust.LI.AppPlugIn
{
    /// <summary>
    /// 退货
    /// </summary>
    [UFSoft.UBF.Eventing.Configuration.Failfast]

    class HxRAMSubsciber : UFSoft.UBF.Eventing.IEventSubscriber
    {
        private static readonly ILogger logger = LoggerManager.GetLogger(typeof(HxRAMSubsciber));

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

            RMA rMA = key.GetEntity() as RMA;

            if (rMA == null)
            {
                return;
            }
            #endregion

            #region 调用接口
            //if (payReqFundHead.DocStatus.Value == 1 && payReqFundHead.DocStatus.Value == 0) 
            if (rMA.SysState == UFSoft.UBF.PL.Engine.ObjectState.Updated && rMA.OriginalData.Status.Value == 0)
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
                //      "customerNumber": "客户编码值",
                //      "planDate": "yyyy-MM-dd 值",
                //      "tenant": "租户值",
                //      "orderNumber": "单号值",
                //      "type": "操作类型值（0 或 1）",
                //      "reMake": "备注值",
                //      "list": [
                //          {
                //          "partNumber": "物料编码值",
                //          "returnQuantity": "退货数量值",
                //          "detailNo": "行号值",
                //          "lotName": "批次码值",
                //          "warehouseNumber": "仓库码值"
                //          }
                //          ]
                //} 
                #endregion

                string operation = "0";

                string tenant = "slerealm1";

                string siteName = "华旋工厂";

                StringBuilder formData = new StringBuilder();

                formData.Append("{");

                // 给键和值添加双引号，并确保键值之间有冒号隔开，符合 JSON 格式要求
                formData.Append("\"tenant\":\"" + tenant + "\",");

                formData.Append("\"type\":\"" + "0" + "\",");

                if (rMA.Customer != null)
                {
                    formData.Append("\"customerNumber\":\"" + rMA.Customer.Code + "\",");
                }
                else
                {
                    formData.Append("\"customerNumber\":\"" + "" + "\",");
                }

                formData.Append("\"tenant\":\"" + tenant + "\",");

                formData.Append("\"orderNumber\":\"" + rMA.DocNo + "\",");

                formData.Append("\"type\":\"" + "0" + "\",");

                formData.Append("\"reMake\":\"" + rMA.Remark + "\",");

                formData.Append("\"list\":[");

                int i = 1;

                foreach (var item in rMA.RMALines)
                {
                    formData.Append("{");

                    formData.Append("\"partNumber\":\"" + item.ItemInfo.ItemCode + "\",");

                    formData.Append("\"returnQuantity\":\"" + item.ApplyQtyTU1 + "\",");

                    if (item.Warehouse != null)
                    {
                        formData.Append("\"warehouseNumber\":\"" + item.Warehouse.Code + "\",");
                    }
                    else
                    {
                        formData.Append("\"warehouseNumber\":\"" + "" + "\",");
                    }

                    if (item.LotInfo != null)
                    {
                        formData.Append("\"lotName\":\"" + item.LotInfo.LotCode + "\",");
                    }
                    else
                    {
                        formData.Append("\"lotName\":\"" + "" + "\",");
                    }

                    formData.Append("\"detailNo\":\"" + item.DocLineNo + "\"");

                    formData.Append("}");

                    if (rMA.RMALines.Count != i)
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

                strURL = "http://" + strURL + "/services/slewms/api/WmsOrder/SR";

                string responseText = HttpRequestClient.HttpPostJson(strURL, formSendDataGo, "", "");

                #endregion
            }

        }

    }
}
