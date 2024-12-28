using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UFIDA.U9.Base;
using UFIDA.U9.SM.Ship;
using UFSoft.UBF.Business;
using UFSoft.UBF.Util.Log;

namespace YY.U9.Cust.LI.AppPlugIn
{
    /// <summary>
    /// 标准出货
    /// </summary>
    [UFSoft.UBF.Eventing.Configuration.Failfast]
    class HxShpiSubsciber : UFSoft.UBF.Eventing.IEventSubscriber
    {
        private static readonly ILogger logger = LoggerManager.GetLogger(typeof(HxRcvRptDocSubsciber));

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

            Ship ship = key.GetEntity() as Ship;

            if (ship == null)
            {
                return;
            }
            #endregion

            #region 调用接口
            //if (payReqFundHead.DocStatus.Value == 1 && payReqFundHead.DocStatus.Value == 0)
            if (ship.SysState == UFSoft.UBF.PL.Engine.ObjectState.Updated)
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
                //  "tenant":"slerealm1",
                //    "type":0,
                //    "orderNo":"order-20241204001-001",
                //    "supplierCode":"gys",
                //    "planDate":"2024-12-10",
                //    "list":[
                //        {
                //                        "lotName":"lot-20241204-001",
                //            "partNumber":"020402040130.1",
                //            "quantity":100
                //        }
                //    ]
                //}




                #endregion

                string operation = "0";

                string tenant = "slerealm1";

                string siteName = "华旋工厂";

                StringBuilder formData = new StringBuilder();
                formData.Append("{");
                formData.Append("customerNumber:" + ship.OrderBy.Code + ",");
                formData.Append("planDate:" + ship.ShipConfirmDate.ToString("yyyy-MM-dd") + ",");
                formData.Append("orderNumber:" + ship.DocNo + ",");
                formData.Append("type:" + "0" + ",");//没找到
                formData.Append("orderType:" + ship.DocType.Name + ",");
                formData.Append("reMake:" + ship.ShipMemo + ",");
                formData.Append("list:[");
                int i = 0;
                foreach (var item in ship.ShipLines)
                {
                    formData.Append("{");
                    formData.Append("customerNumber:" + "" + ",");//行里没有
                    formData.Append("partNumber:" + item.ItemInfo.ItemCode + ",");
                    if (item.LotInfo != null)
                    {
                        formData.Append("lotName:" + item.LotInfo.LotCode + ",");
                    }
                    else
                    {
                        formData.Append("lotName:" + "" + ",");
                    }

                    formData.Append("demandQuantity:" + item.QtyPriceAmount + ",");
                    formData.Append("detailNo:" + item.DocLineNo + ",");
                    formData.Append("baseEntry:" + item.SrcDocNo + ",");
                    formData.Append("baseline:" + item.SrcDocLineNo);
                    formData.Append("}");
                    if (ship.ShipLines.Count != i)
                    {
                        formData.Append(",");
                    }
                    i++;
                }
                formData.Append("]}");

                //发送格式
                StringBuilder formSendData = new StringBuilder();

                formSendData.Append(formData.ToString());

                logger.Error("标准出货新增传出数据：" + formSendData.ToString());

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

                strURL = "http://" + strURL + "/services/slewms/api/WmsOrder/SO";

                string responseText = HttpRequestClient.HttpPostJson(strURL, formSendDataGo, "", "");

                #endregion
            }

        }
    }
}
