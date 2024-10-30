using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UFIDA.U9.AP.Payment;
using UFIDA.U9.UI.PDHelper;
using UFSoft.UBF.Business;
using UFSoft.UBF.Util.Log;

namespace YY.U9.Cust.LI.AppPlugIn
{
    /// <summary>
    /// 调用接口
    /// 中能创
    /// 位置D:\setups\porject\中能创\2024-08\中能创流程接口\中能创流程接口\3请款单
    /// </summary>
    [UFSoft.UBF.Eventing.Configuration.Failfast]
    class ZNCPayReqFundHeadUpdateSubscriber : UFSoft.UBF.Eventing.IEventSubscriber
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
            PayReqFundHead payReqFundHead = key.GetEntity() as PayReqFundHead;
            if (payReqFundHead == null)
            {
                return;
            }
            #endregion
            #region 调用接口
            //if (payReqFundHead.DocStatus.Value == 1 && payReqFundHead.DocStatus.Value == 0)
            if (payReqFundHead.DocStatus == UFIDA.U9.CBO.FI.Enums.InDirectHandleBillDocStatusEnum.InApprove && payReqFundHead.OriginalData.DocStatus == UFIDA.U9.CBO.FI.Enums.InDirectHandleBillDocStatusEnum.Open)

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

                string docSubject = "请款单";

                string fdTemplateId = "19130d974d241864f03aba74e3f99e29";

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

                formData.Append("\"fd_danJuHao\":");
                formData.Append("\"" + payReqFundHead.DocNo + "\",");

                formData.Append("\"fd_danJuLeiXing\":");
                formData.Append("\"" + payReqFundHead.DocType.Code + "\",");

                formData.Append("\"fd_shenQingRiQi\":");
                formData.Append("\"" + payReqFundHead.PayRFDate.ToString("yyyy-MM-dd") + "\",");

                formData.Append("\"fd_yuFuKuanRiQi\":");
                formData.Append("\"" + payReqFundHead.ExpectPayDate.ToString("yyyy-MM-dd") + "\",");

                formData.Append("\"fd_qingKuanYongTu\":");
                int qingKuanYongTu = 1;
                string fd_qingKuanYongTu = "预付款";
                foreach (var PayReqitem in payReqFundHead.PayReqFundUses)
                {
                    qingKuanYongTu = PayReqitem.ReqFundUse.Value;
                }
                switch (qingKuanYongTu)
                {
                    case 0:
                        fd_qingKuanYongTu = "保证金";
                        break;
                    case 1:
                        fd_qingKuanYongTu = "预付款";
                        break;
                    case 2:
                        fd_qingKuanYongTu = "标准";
                        break;
                    case 3:
                        fd_qingKuanYongTu = "质保金";
                        break;
                    case 4:
                        fd_qingKuanYongTu = "杂项";
                        break;
                    default:
                        fd_qingKuanYongTu = "预付款";
                        break;
                }
                formData.Append("\"" + fd_qingKuanYongTu + "\",");

                formData.Append("\"fd_biZhong\":");
                if (payReqFundHead.ReqFundAC != null)
                {
                    formData.Append("\"" + payReqFundHead.ReqFundAC.Name + "\",");
                }
                else
                {
                    formData.Append("\"" + "" + "\",");
                }

                //formData.Append("\"fd_heSuanZuZhi\":");
                //formData.Append("\"" + payReqFundHead.Org.Name + "\",");

                formData.Append("\"fd_fuKuanFangShi\":");
                formData.Append("\"" + "对外付款" + "\",");

                formData.Append("\"fd_zhuangTai\":");
                string StatusName = "开立";
                switch (payReqFundHead.DocStatus.Value)
                {
                    case 0:
                        StatusName = "开立";
                        break;
                    case 1:
                        StatusName = "核准中";
                        break;
                    case 2:
                        StatusName = "已审核";
                        break;
                    case 3:
                        StatusName = "关闭";
                        break;
                    default:
                        StatusName = "开立";
                        break;
                }
                formData.Append("\"" + StatusName + "\",");

                formData.Append("\"fd_CultureName\":");
                formData.Append("\"" + "zh-CN" + "\",");

                //formData.Append("\"fd_OrgCode\":");
                //formData.Append("\"" + payReqFundHead.Org.Code + "\",");
                //--不传

                //formData.Append("\"fd_UserCode\":");
                //formData.Append("\"" + PDContext.Current.UserCode + "\",");
                //--不传

                formData.Append("\"fd_u9_OrgCode\":");
                formData.Append("\"" + payReqFundHead.Org.Code + "\",");

                formData.Append("\"fd_u9_UserCode\":");
                formData.Append("\"" + PDContext.Current.UserCode + "\",");

                formData.Append("\"fd_EntCode\":");
                formData.Append("\"" + "011" + "\",");

                formData.Append("\"fd_businessType\":");
                formData.Append("\"" + "PR" + "\",");

                formData.Append("\"fd_requestData\":");
                formData.Append("\"" + " " + "\",");

                #region 循环行
                string fd_qingKUanMX_fd_gongYingShang = "";
                string fd_qingKUanMX_fd_xuanDan = "";
                string fd_qingKUanMX_fd_zheKouQianQingKuanJinE = "";
                string fd_qingKUanMX_fd_zheKouHouQingKuanJinE = "";
                string fd_qingKUanMX_fd_fuKuanBenBiJinE = "";
                string fd_qingKUanMX_fd_jieSuanFangShi = "";
                string fd_qingKUanMX_fd_gYsYinXingZhangHa = "";
                string fd_qingKUanMX_fd_gYsKaiHuXing = "";
                string fd_qingKUanMX_fd_qingKuanShengYuJinE = "";

                foreach (var item in payReqFundHead.PayReqFundUses)
                {
                    string DH = "\"";
                    string gongYingShang = DH + item.Supp.Supplier.Name + DH + ",";
                    fd_qingKUanMX_fd_gongYingShang = gongYingShang + fd_qingKUanMX_fd_gongYingShang;

                    string APDocNo = "";//应付单单号
                    foreach (var PayRUIitem in item.PayReqFundDetailForUIs)
                    {
                        APDocNo = PayRUIitem.APBillHead.DocNo;
                    }
                    string xuanDan = DH + APDocNo + DH + ",";
                    fd_qingKUanMX_fd_xuanDan = xuanDan + fd_qingKUanMX_fd_xuanDan;

                    string RFTotalMoney = "0";//折扣前请款金额
                    foreach (var PayRUIitem in item.PayReqFundDetailForUIs)
                    {
                        RFTotalMoney = PayRUIitem.RFTotalMoney.ToString();
                    }
                    string zheKouQianQingKuanJinE = DH + RFTotalMoney + DH + ",";//折扣付款前
                    fd_qingKUanMX_fd_zheKouQianQingKuanJinE = zheKouQianQingKuanJinE + fd_qingKUanMX_fd_zheKouQianQingKuanJinE;

                    string zheKouHouQingKuanJinE = DH + RFTotalMoney + DH + ",";//折扣付款后
                    fd_qingKUanMX_fd_zheKouHouQingKuanJinE = zheKouHouQingKuanJinE + fd_qingKUanMX_fd_zheKouHouQingKuanJinE;

                    string fuKuanBenBiJinE = DH + item.AcmPayFCMoney + DH + ",";
                    fd_qingKUanMX_fd_fuKuanBenBiJinE = fuKuanBenBiJinE + fd_qingKUanMX_fd_fuKuanBenBiJinE;

                    string SttlMethod = "";
                    if (item.SttlMethod != null)
                    {
                        SttlMethod = DH + item.SttlMethod.Name + DH + ",";
                        fd_qingKUanMX_fd_jieSuanFangShi = SttlMethod + fd_qingKUanMX_fd_jieSuanFangShi;
                    }
                    else
                    {
                        SttlMethod = DH + SttlMethod + DH + ",";
                        fd_qingKUanMX_fd_jieSuanFangShi = SttlMethod + fd_qingKUanMX_fd_jieSuanFangShi;
                    }

                    string BankName = "";
                    //foreach (var supitem in item.Supp.Supplier.SupplierBankAccount)//
                    //{
                    BankName = item.Supp.Supplier.DescFlexField.PrivateDescSeg6;
                    //}
                    string SupplierBankAccount = DH + BankName + DH + ",";
                    fd_qingKUanMX_fd_gYsYinXingZhangHa = SupplierBankAccount + fd_qingKUanMX_fd_gYsYinXingZhangHa;

                    string OppAccBk = DH + item.Supp.Supplier.DescFlexField.PrivateDescSeg5 + DH + ",";
                    fd_qingKUanMX_fd_gYsKaiHuXing = OppAccBk + fd_qingKUanMX_fd_gYsKaiHuXing;

                    string D1 = DH + item.DescFlexField.PrivateDescSeg1 + DH + ",";//私有字段1
                    fd_qingKUanMX_fd_qingKuanShengYuJinE = D1 + fd_qingKUanMX_fd_qingKuanShengYuJinE;

                }
                #endregion
                
                formData.Append("\"fd_detail_Entries.fd_gongYingShang\":");
                formData.Append("[" + fd_qingKUanMX_fd_gongYingShang.Substring(0, fd_qingKUanMX_fd_gongYingShang.Length - 1) + "],");

                formData.Append("\"fd_detail_Entries.fd_xuanDan\":");
                formData.Append("[" + fd_qingKUanMX_fd_xuanDan.Substring(0, fd_qingKUanMX_fd_xuanDan.Length - 1) + "],");

                formData.Append("\"fd_detail_Entries.fd_zheKouQianQingKuanJinE\":");
                formData.Append("[" + fd_qingKUanMX_fd_zheKouQianQingKuanJinE.Substring(0, fd_qingKUanMX_fd_zheKouQianQingKuanJinE.Length - 1) + "],");

                formData.Append("\"fd_detail_Entries.fd_zheKouHouQingKuanJinE\":");
                formData.Append("[" + fd_qingKUanMX_fd_zheKouHouQingKuanJinE.Substring(0, fd_qingKUanMX_fd_zheKouHouQingKuanJinE.Length - 1) + "],");

                formData.Append("\"fd_detail_Entries.fd_fuKuanBenBiJinE\":");
                formData.Append("[" + fd_qingKUanMX_fd_fuKuanBenBiJinE.Substring(0, fd_qingKUanMX_fd_fuKuanBenBiJinE.Length - 1) + "],");

                formData.Append("\"fd_detail_Entries.fd_jieSuanFangShi\":");
                formData.Append("[" + fd_qingKUanMX_fd_jieSuanFangShi.Substring(0, fd_qingKUanMX_fd_jieSuanFangShi.Length - 1) + "],");

                formData.Append("\"fd_detail_Entries.fd_gYsYinXingZhangHa\":");
                formData.Append("[" + fd_qingKUanMX_fd_gYsYinXingZhangHa.Substring(0, fd_qingKUanMX_fd_gYsYinXingZhangHa.Length - 1) + "],");

                formData.Append("\"fd_detail_Entries.fd_gYsKaiHuXing\":");
                formData.Append("[" + fd_qingKUanMX_fd_gYsKaiHuXing.Substring(0, fd_qingKUanMX_fd_gYsKaiHuXing.Length - 1) + "],");

                formData.Append("\"fd_detail_Entries.fd_qingKuanShengYuJinE\":");
                formData.Append("[" + fd_qingKUanMX_fd_qingKuanShengYuJinE.Substring(0, fd_qingKUanMX_fd_qingKuanShengYuJinE.Length - 1) + "]");

                formData.Append("}</formValues>");

                #endregion

                formData.Append("<docStatus>20</docStatus >");
                formData.Append("<docCreator>{");
                formData.Append("\"LoginName\":");
                formData.Append("\"" + PDContext.Current.UserCode+ "\"}");
                formData.Append("</docCreator>");
                formData.Append("</arg0>");
                formData.Append("</web:addReview>");
                formData.Append("</soapenv:Body>");
                formData.Append("</soapenv:Envelope>");

                //发送格式
                StringBuilder formSendData = new StringBuilder();

                formSendData.Append(formData.ToString());

                logger.Error("请款单新增传出数据：" + formSendData.ToString());

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

                string responseText = HttpRequestClient.HttpPost(strURL, formSendDataGo, "", "");

                #endregion
            }

        }
    }
}
