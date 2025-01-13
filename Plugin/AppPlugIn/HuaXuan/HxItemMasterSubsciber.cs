using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UFIDA.U9.AP.Payment;
using UFIDA.U9.CBO.SCM.Item;
using UFIDA.U9.UI.PDHelper;
using UFSoft.UBF.Business;
using UFSoft.UBF.Util.Log;
namespace YY.U9.Cust.LI.AppPlugIn
{
    /// <summary>
    /// 华旋料品
    /// 参考文档：华旋WMS接口文档 V1.2
    /// </summary>
    [UFSoft.UBF.Eventing.Configuration.Failfast]
    class HxItemMasterSubsciber : UFSoft.UBF.Eventing.IEventSubscriber
    {
        private static readonly ILogger logger = LoggerManager.GetLogger(typeof(HxItemMasterSubsciber));

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
            ItemMaster itemMaster = key.GetEntity() as ItemMaster;
            if (itemMaster == null)
            {
                return;
            }
            #endregion

            #region 调用接口
            //if (payReqFundHead.DocStatus.Value == 1 && payReqFundHead.DocStatus.Value == 0)
            if (itemMaster.SysState == UFSoft.UBF.PL.Engine.ObjectState.Updated)
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
                //"operation":0,
                //"tenant":"slerealm1",
                //"partNumber":"030400010218",
                //"revision":"/",
                //"name":"旋变定子组件-F30-70",
                //"siteName":"华旋工厂",
                //"type":"成品",
                //"state":1,
                //"uomName":"pcs",
                //"description":"小鹏F30-润邦-御马-pin镀金",
                //"innerCode":"2321554",
                //"brand":"小鹏"，
                //"textureName":"铜"，
                //"specification":"DZZJHX70.4.0035"，
                //"validDay":100,
                //}
                #endregion 

                string operation = "0";

                string tenant = "slerealm1";

                string siteName = "华旋工厂";

                string m = "\"";

                StringBuilder formData = new StringBuilder();
                formData.Append("{");
                formData.Append("\"operationType\":\"" + operation + "\",");
                formData.Append("\"tenant\":\"" + tenant + "\",");
                formData.Append("\"partNumber\":\"" + itemMaster.Code + "\",");
                formData.Append("\"revision\":\"" + "1" + "\",");
                formData.Append("\"name\":\"" + itemMaster.Name + "\",");
                formData.Append("\"siteName\":\"" + siteName + "\",");
                formData.Append("\"type\":\"" + itemMaster.MainItemCategory.Name + "\",");
                formData.Append("\"state\":\"" + "0" + "\",");
                formData.Append("\"uomName\":\"" + itemMaster.InventorySecondUOM.Name + "\",");
                formData.Append("\"description\":\"" + itemMaster.Description + "\",");
                formData.Append("\"innerCode\":\"" + "" + "\",");
                formData.Append("\"brand\":\"" + "" + "\",");
                formData.Append("\"textureName\":\"" + "" + "\",");
                formData.Append("\"specification\":\"" + "" + "\",");
                formData.Append("\"validDay\":\"" + itemMaster.InventoryInfo.LotValidDate + "\"");
                formData.Append("}");

                //发送格式
                StringBuilder formSendData = new StringBuilder();

                formSendData.Append(formData.ToString());

                string formDataStr = formSendData.ToString();

                logger.Error("料品新增传出数据：" + formDataStr.ToString());

                string strURL = null;

                //测试
                //strURL = "http://118.195.189.35:8900/accessPlatform/platformAPI";

                //正式
                //strURL = "http://58.216.169.102:9081/ekp/sys/webservice/kmReviewWebserviceService?wsdl";


                long orgID = long.Parse(PDContext.Current.OrgID);


                //OA服务器地址
                string oAURL = Common.GetProfileValue(Common.S_PROFILE_CODE, orgID);

                if (string.IsNullOrEmpty(oAURL))
                {
                    return;
                }

                strURL = oAURL;

                string formSendDataGo = formDataStr.ToString();

                strURL = "http://" + strURL + "/services/slemaindata/api/parts/sync";

                string responseText = HttpRequestClient.HttpPostJson(strURL, formSendDataGo, "", "");

                #endregion
            }

            if (itemMaster.SysState == UFSoft.UBF.PL.Engine.ObjectState.Deleted)
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
                //"operation":0,
                //"tenant":"slerealm1",
                //"partNumber":"030400010218",
                //"revision":"/",
                //"name":"旋变定子组件-F30-70",
                //"siteName":"华旋工厂",
                //"type":"成品",
                //"state":1,
                //"uomName":"pcs",
                //"description":"小鹏F30-润邦-御马-pin镀金",
                //"innerCode":"2321554",
                //"brand":"小鹏"，
                //"textureName":"铜"，
                //"specification":"DZZJHX70.4.0035"，
                //"validDay":100,
                //}
                #endregion 

                string operation = "1";

                string tenant = "slerealm1";

                string siteName = "华旋工厂";

                string m = "\"";

                StringBuilder formData = new StringBuilder();
                formData.Append("{");
                formData.Append("\"operationType\":\"" + operation + "\",");
                formData.Append("\"tenant\":\"" + tenant + "\",");
                formData.Append("\"partNumber\":\"" + itemMaster.Code + "\",");
                formData.Append("\"revision\":\"" + "1" + "\"");
                formData.Append("}");

                //发送格式
                StringBuilder formSendData = new StringBuilder();

                formSendData.Append(formData.ToString());

                string formDataStr = formSendData.ToString();

                logger.Error("料品删除传出数据：" + formDataStr.ToString());


                string strURL = null;

                //测试
                //strURL = "http://118.195.189.35:8900/accessPlatform/platformAPI";

                //正式
                //strURL = "http://58.216.169.102:9081/ekp/sys/webservice/kmReviewWebserviceService?wsdl";


                long orgID = long.Parse(PDContext.Current.OrgID);


                //OA服务器地址
                string oAURL = Common.GetProfileValue(Common.S_PROFILE_CODE, orgID);

                if (string.IsNullOrEmpty(oAURL))
                {
                    return;
                }

                strURL = oAURL;

                string formSendDataGo = formDataStr.ToString();

                strURL = "http://" + strURL + "/services/slemaindata/api/parts/sync";

                string responseText = HttpRequestClient.HttpPostJson(strURL, formSendDataGo, "", "");

                logger.Error("料品删除返回报文：" + responseText.ToString());

            }

        }
    }


}
