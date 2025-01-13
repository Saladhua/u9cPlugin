using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UFIDA.U9.Base;
using UFIDA.U9.CBO.MFG.BOM;
using UFSoft.UBF.Business;
using UFSoft.UBF.Util.Log;

namespace YY.U9.Cust.LI.AppPlugIn
{
    /// <summary>
    /// 物料清单
    /// </summary>
    [UFSoft.UBF.Eventing.Configuration.Failfast]
    class HxBomMasterSubsciber : UFSoft.UBF.Eventing.IEventSubscriber
    {
        private static readonly ILogger logger = LoggerManager.GetLogger(typeof(HxBomMasterSubsciber));

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

            BOMMaster bOMMaster = key.GetEntity() as BOMMaster;

            if (bOMMaster == null)
            {
                return;
            }
            #endregion

            #region 调用接口
            //if (payReqFundHead.DocStatus.Value == 1 && payReqFundHead.DocStatus.Value == 0)
            if (bOMMaster.SysState == UFSoft.UBF.PL.Engine.ObjectState.Updated)
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
                //  "operationType":1,
                //  "tenant": "slerealm1",
                //  "bomDTO":{
                //  "name": "联电TestES11HX59.4.0006",
                //  "revision": "01",
                //  "description": "测试",
                //  "state": 1,
                //  "siteName": "华夏1号工厂"
                //  },
                //  "bomItems":[
                //  {
                //  "partNumber": "020102040053",
                //  "partRevision": 1,
                //  "quantity": 100,
                //  "description": "描述",
                //  "replacementType": 0,
                //  "remark": "备注",
                //  "partAlterDTOs": [
                //  {
                //  "partNumber": "030400010019",
                //  "partRevision": 1,
                //  "quantity": 100,
                //  "effectiveStart": "2024-12-04T15:00:00Z",
                //  "effectiveEnd": "2024-12-04T17:00:00Z",
                //  "type": 0
                //  }
                //  ]
                //  }
                //  ]
                //} 
                #endregion

                string operation = "0";

                string tenant = "slerealm1";

                string siteName = "华旋工厂";

                StringBuilder formData = new StringBuilder();
                formData.Append("{");
                formData.Append("\"operationType\":\"0\",");
                formData.Append("\"tenant\":\"" + tenant + "\",");
                formData.Append("\"bomDTO\":{");
                formData.Append("\"name\":\"" + bOMMaster.ItemMaster.Name + "\",");
                formData.Append("\"revision\":\"" + bOMMaster.BOMVersionCode + "\",");
                formData.Append("\"description\":\"\",");
                formData.Append("\"state\":\"" + "1" + "\",");
                formData.Append("\"remark\":\"" + bOMMaster.Explain + "\",");
                formData.Append("\"siteName\":\"" + siteName + "\"");
                formData.Append("},");
                formData.Append("\"bomItems\":[");
                int i = 1;
                foreach (var item in bOMMaster.BOMComponents)
                {
                    formData.Append("{");
                    formData.Append("\"partNumber\":\"" + item.ItemMaster.Code + "\",");
                    formData.Append("\"partRevision\":\"" + "1" + "\",");
                    formData.Append("\"quantity\":\"" + item.UsageQty + "\",");
                    formData.Append("\"description\":\"\",");
                    if (item.SubstituteStyle.Name == "None")
                    {
                        formData.Append("\"replacementType\":\"" + "" + "\",");
                    }
                    else
                    {
                        formData.Append("\"replacementType\":\"" + item.SubstituteStyle.Name + "\",");
                    }
                    formData.Append("\"remark\":\"" + item.Remark + "\",");
                    formData.Append("\"partAlterDTOs\":[");
                    int k = 1;
                    foreach (var itemSubstitutes in item.Substitutes)
                    {
                        formData.Append("{");
                        formData.Append("\"partNumber\":\"" + itemSubstitutes.ItemMaster.Code + "\",");
                        formData.Append("\"partRevision\":\"" + "1" + "\",");
                        formData.Append("\"quantity\":\"" + itemSubstitutes.SubstituteQty + "\",");
                        formData.Append("\"description\":\"\",");
                        formData.Append("\"effectiveStart\":\"" + item.EffectiveDate + "\",");
                        formData.Append("\"effectiveEnd\":\"" + item.DisableDate + "\",");
                        formData.Append("\"type\":\"\"");
                        formData.Append("}");
                        if (k < item.Substitutes.Count - 1)
                        {
                            formData.Append(",");
                        }
                        k++;
                    }
                    if (item.Substitutes.Count == 0)
                    {
                        formData.Append("{");
                        formData.Append("\"partNumber\":\"" + "" + "\",");
                        formData.Append("\"partRevision\":\"" + "" + "\",");
                        formData.Append("\"quantity\":\"" + "" + "\",");
                        formData.Append("\"description\":\"\",");
                        formData.Append("\"effectiveStart\":\"" + "" + "\",");
                        formData.Append("\"effectiveEnd\":\"" + "" + "\",");
                        formData.Append("\"type\":\"\"");
                        formData.Append("}");
                        if (k < item.Substitutes.Count - 1)
                        {
                            formData.Append(",");
                        }
                    }
                    formData.Append("]");
                    formData.Append("}");
                    if (i < bOMMaster.BOMComponents.Count - 1)
                    {
                        formData.Append(",");
                    }
                    i++;
                }
                formData.Append("]}");

                //发送格式
                StringBuilder formSendData = new StringBuilder();

                formSendData.Append(formData.ToString());

                logger.Error("物料清单新增传出数据：" + formSendData.ToString());

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

                strURL = "http://" + strURL + "/services/slemaindata/api/boms/sync";

                string responseText = HttpRequestClient.HttpPostJson(strURL, formSendDataGo, "", "");

                #endregion
            }

            if (bOMMaster.SysState == UFSoft.UBF.PL.Engine.ObjectState.Deleted)
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
                //  "operationType":1,
                //  "tenant": "slerealm1",
                //  "bomDTO":{
                //  "name": "联电TestES11HX59.4.0006",
                //  "revision": "01",
                //  "description": "测试",
                //  "state": 1,
                //  "siteName": "华夏1号工厂"
                //  },
                //  "bomItems":[
                //  {
                //  "partNumber": "020102040053",
                //  "partRevision": 1,
                //  "quantity": 100,
                //  "description": "描述",
                //  "replacementType": 0,
                //  "remark": "备注",
                //  "partAlterDTOs": [
                //  {
                //  "partNumber": "030400010019",
                //  "partRevision": 1,
                //  "quantity": 100,
                //  "effectiveStart": "2024-12-04T15:00:00Z",
                //  "effectiveEnd": "2024-12-04T17:00:00Z",
                //  "type": 0
                //  }
                //  ]
                //  }
                //  ]
                //} 
                #endregion

                string operation = "0";

                string tenant = "slerealm1";

                string siteName = "华旋工厂";

                StringBuilder formData = new StringBuilder();
                formData.Append("{");
                formData.Append("\"operationType\":\"1\",");
                formData.Append("\"tenant\":\"" + tenant + "\",");
                formData.Append("\"bomDTO\":{");
                formData.Append("\"name\":\"" + bOMMaster.ItemMaster.Name + "\",");
                formData.Append("\"revision\":\"" + bOMMaster.BOMVersionCode + "\",");
                formData.Append("}");
                formData.Append("}");

                //发送格式
                StringBuilder formSendData = new StringBuilder();

                formSendData.Append(formData.ToString());

                logger.Error("物料清单删除传出数据：" + formSendData.ToString());

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

                strURL = "http://" + strURL + "/services/slemaindata/api/boms/sync";

                string responseText = HttpRequestClient.HttpPostJson(strURL, formSendDataGo, "", "");

 
            }

        }
    }
}
