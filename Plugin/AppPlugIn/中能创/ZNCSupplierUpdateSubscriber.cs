using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using UFIDA.U9.CBO.SCM.Supplier;
using UFIDA.U9.UI.PDHelper;
using UFSoft.UBF.Business;
using UFSoft.UBF.Util.Log;

namespace YY.U9.Cust.LI.AppPlugIn
{
    /// <summary>
    /// 调用接口
    /// 中能创
    /// 位置D:\setups\porject\中能创\2024-08\中能创流程接口\中能创流程接口\1供应商准入
    /// </summary>
    [UFSoft.UBF.Eventing.Configuration.Failfast]
    class ZNCSupplierUpdateSubscriber : UFSoft.UBF.Eventing.IEventSubscriber
    {
        private static readonly ILogger logger = LoggerManager.GetLogger(typeof(MOSimuDocInsertedSubscriber));

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
            Supplier supplier = key.GetEntity() as Supplier;
            if (supplier == null)
            {
                return;
            }
            #endregion

            //if (supplier.State.Value == 0)
            //{
            #region 调用接口

            //int a = 0;
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

            string docSubject = "供应商准入";

            string fdTemplateId = "19130d97218574fccfa7cf248449f0b5";

            StringBuilder formData = new StringBuilder();

            StringBuilder formData12 = new StringBuilder();

            formData.Append("<soapenv:Envelope xmlns:soapenv=\"http://schemas.xmlsoap.org/soap/envelope/\" xmlns:web=\"http://webservice.review.km.kmss.landray.com/\" >");
            formData.Append("<soapenv:Header/>");
            formData.Append("<soapenv:Body>");
            formData.Append("<web:addReview>");
            formData.Append("<arg0>");
            #region 参数
            formData.Append("<docSubject>" + docSubject + "</docSubject>");
            formData.Append("<fdTemplateId>" + fdTemplateId + "</fdTemplateId>");
            formData.Append("<formValues>{");

            formData.Append("\"fd_gongYingShangMingChen\":");
            formData.Append("\"" + supplier.Name + "\",");

            formData.Append("\"fd_gongYingShangBianHao\":");
            formData.Append("\"" + supplier.Code + "\",");

            formData.Append("\"fd_lianXiRen\":");
            formData.Append("\"" + supplier.DescFlexField.PrivateDescSeg3 + "\",");

            formData.Append("\"fd_dianHua\":");
            formData.Append("\"" + supplier.DescFlexField.PrivateDescSeg4 + "\",");

            formData.Append("\"fd_kaiHuXing\":");
            formData.Append("\"" + supplier.DescFlexField.PrivateDescSeg5 + "\",");

            formData.Append("\"fd_gongYingShangYinXingZhangHa\":");
            formData.Append("\"" + supplier.DescFlexField.PrivateDescSeg6 + "\",");

            formData.Append("\"fd_xingHao\":");
            formData.Append("\"" + supplier.DescFlexField.PrivateDescSeg7 + "\",");

            formData.Append("\"fd_shuiZuHe\":");
            formData.Append("\"" + supplier.TaxSchedule.Name + "\",");

            formData.Append("\"fd_gongYingShangDiZhi\":");
            formData.Append("\"" + supplier.DescFlexField.PrivateDescSeg2 + "\",");

            formData.Append("\"fd_gongYingShangZhuangTai\":");

            //string gongYingShangZhuangTai = "待核准";

            //switch (supplier.State.Value)
            //{
            //    case 0:
            //        gongYingShangZhuangTai = "已核准";
            //        break;
            //    case 1:
            //        gongYingShangZhuangTai = "待核准";
            //        break;
            //    case 2:
            //        gongYingShangZhuangTai = "不准交易";
            //        break;
            //    case 3:
            //        gongYingShangZhuangTai = "核准中";
            //        break;
            //    default:
            //        gongYingShangZhuangTai = "待核准";
            //        break;
            //}

            formData.Append("\"" + supplier.State.Value + "\",");

            formData.Append("\"fd_CultureName\":");
            formData.Append("\"" + "zh-CN" + "\",");

            formData.Append("\"fd_u9_OrgCode\":");
            formData.Append("\"" + supplier.Org.Code + "\",");

            formData.Append("\"fd_u9_UserCode\":");
            formData.Append("\"" + PDContext.Current.UserCode + "\",");

            //formData.Append("\"fd_OrgCode\":");
            //formData.Append("\"" + supplier.Org.Code + "\",");

            //formData.Append("\"fd_UserCode\":");
            //formData.Append("\"" + PDContext.Current.UserCode + "\",");

            //--不传

            formData.Append("\"fd_EntCode\":");
            formData.Append("\"" + "011" + "\",");

            formData.Append("\"fd_businessType\":");
            formData.Append("\"" + "PR" + "\",");

            formData.Append("\"fd_requestData\":");
            formData.Append("\"" + " " + "\"");

            formData.Append("}</formValues>");
            formData.Append("<docStatus>20</docStatus >");
            formData.Append("<docCreator>{");
            formData.Append("\"LoginName\":");
            formData.Append("\"" + PDContext.Current.UserCode + "\"}");
            formData.Append("</docCreator>");
            #endregion
            formData.Append("</arg0>");
            formData.Append("</web:addReview>");
            formData.Append("</soapenv:Body>");
            formData.Append("</soapenv:Envelope>");

            //发送格式
            StringBuilder formSendData = new StringBuilder();

            formSendData.Append(formData.ToString());

            logger.Error("供应商准入新增传出数据：" + formSendData.ToString());

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


            string formSendDataGo = formSendData.ToString();

            string responseText = HttpRequestClient.HttpPost(strURL, formSendDataGo, "SESSION=MjBlMTdkZTctYjk3OS00ZDc4LTk3M2QtMGY3YzRkMmM0Nzhj", "");

            #endregion
        }
        //} 
    }
}
