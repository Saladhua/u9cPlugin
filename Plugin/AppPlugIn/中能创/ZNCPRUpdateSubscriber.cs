using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using UFIDA.U9.CBO.SCM.ProjectTask;
using UFIDA.U9.CBO.SCM.Supplier;
using UFIDA.U9.PR.PurchaseRequest;
using UFIDA.U9.UI.PDHelper;
using UFSoft.UBF.Business;
using UFSoft.UBF.Util.Log;

namespace YY.U9.Cust.LI.AppPlugIn
{
    /// <summary>
    /// 调用接口
    /// 中能创
    /// 位置D:\setups\porject\中能创\2024-08\中能创流程接口\中能创流程接口\4请购单
    /// </summary>
    [UFSoft.UBF.Eventing.Configuration.Failfast]
    class ZNCPRUpdateSubscriber : UFSoft.UBF.Eventing.IEventSubscriber
    {
        private static readonly ILogger logger = LoggerManager.GetLogger(typeof(MOSimuDocInsertedSubscriber));
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
            PR pR = key.GetEntity() as PR;
            if (pR == null)
            {
                return;
            }
            #endregion
            #region 调用接口
            if (pR.Status.Value == 1)
            {
                UFSoft.UBF.Transactions.UBFTransactionContext.Current.Committed += new System.Transactions.TransactionCompletedEventHandler(BusinessTransactionSucess);
                void BusinessTransactionSucess(object obj, System.Transactions.TransactionEventArgs e)
                {

                    string docSubject = "请购单";

                    string fdTemplateId = "19130d9730bc73f74630a2d4ace82cc3";

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
                    formData.Append("\"" + pR.DocNo + "\",");

                    formData.Append("\"fd_danJuLeiXing\":");
                    formData.Append("\"" + pR.DocType.Code + "\",");

                    formData.Append("\"fd_riQi\":");
                    formData.Append("\"" + pR.BusinessDate.ToString("yyyy-MM-dd") + "\",");

                    formData.Append("\"fd_heSuanZuZhi\":");
                    formData.Append("\"" + pR.Org.Name + "\",");

                    formData.Append("\"fd_xuQiuBuMen\":");
                    formData.Append("\"" + pR.ReqDepartment.Name + "\",");

                    formData.Append("\"fd_yanFaXiangMu\":");
                    //Project project = Project.Finder.Find("Code='" + pR.DescFlexField.PubDescSeg14 + "' and Org.ID='" + pR.Org.ID + "'");
                    Project project = Project.Finder.Find("Code='" + "A01" + "' and Org.ID='" + pR.Org.ID + "'");
                    formData.Append("\"" + project.Name + "\",");

                    formData.Append("\"fd_zhuangTai\":");
                    string StatusName = "开立";
                    switch (pR.Status.Value)
                    {
                        case 0:
                            StatusName = "开立";
                            break;
                        case 1:
                            StatusName = "审核中";
                            break;
                        case 2:
                            StatusName = "已审核";
                            break;
                        case 3:
                            StatusName = "自然关闭";
                            break;
                        case 4:
                            StatusName = "短缺关闭";
                            break;
                        case 5:
                            StatusName = "超额关闭";
                            break;
                        default:
                            StatusName = "开立";
                            break;
                    }
                    formData.Append("\"" + StatusName + "\",");

                    formData.Append("\"fd_CultureName\":");
                    formData.Append("\"" + "zh-CN" + "\",");

                    formData.Append("\"fd_OrgCode\":");
                    formData.Append("\"" + pR.Org.Code + "\",");

                    formData.Append("\"fd_UserCode\":");
                    formData.Append("\"" + PDContext.Current.UserCode + "\",");

                    formData.Append("\"fd_EntCode\":");
                    formData.Append("\"" + "011" + "\",");

                    formData.Append("\"fd_businessType\":");
                    formData.Append("\"" + "PR" + "\",");

                    formData.Append("\"fd_requestData\":");
                    formData.Append("\"" + " " + "\",");

                    #region 循环行
                    string fd_qingGouMX_fd_liaoHao = "";
                    string fd_qingGouMX_fd_pinMing = "";
                    string fd_qingGouMX_fd_guiGe = "";
                    string fd_qingGouMX_fd_dengJi = "";
                    string fd_qingGouMX_fd_xiaoLv = "";
                    string fd_qingGouMX_fd_gongLv = "";
                    string fd_qingGouMX_fd_zongGongLv = "";
                    string fd_qingGouMX_fd_xuQiuShuLiang = "";
                    string fd_qingGouMX_fd_yaoQiuJiaoHuoRiQi = "";
                    
                    foreach (var item in pR.PRLineList)
                    {
                        string DH = "\"";
                        string iteminfoCode = DH + item.ItemInfo.ItemID.Code + DH + ",";
                        fd_qingGouMX_fd_liaoHao = iteminfoCode + fd_qingGouMX_fd_liaoHao;

                        string iteminfoName = DH + item.ItemInfo.ItemID.Name + DH + ",";
                        fd_qingGouMX_fd_pinMing = iteminfoName + fd_qingGouMX_fd_pinMing;

                        string iteminfoSPEC = DH + item.ItemInfo.ItemID.SPECS + DH + ",";
                        fd_qingGouMX_fd_guiGe = iteminfoSPEC + fd_qingGouMX_fd_guiGe;

                        string iteminfoGrade = DH + item.ItemInfo.ItemGrade.Name + DH + ",";
                        fd_qingGouMX_fd_dengJi = iteminfoGrade + fd_qingGouMX_fd_dengJi;

                        string xiaoLv = DH + item.ItemInfo.ItemPotency.Name + DH + ",";
                        fd_qingGouMX_fd_xiaoLv = xiaoLv + fd_qingGouMX_fd_xiaoLv;

                        string itemDP1 = DH + item.DescFlexSegments.PubDescSeg1 + DH + ",";
                        fd_qingGouMX_fd_gongLv = itemDP1 + fd_qingGouMX_fd_gongLv;

                        string itemDP2 = DH + item.DescFlexSegments.PubDescSeg2 + DH + ",";
                        fd_qingGouMX_fd_zongGongLv = itemDP2 + fd_qingGouMX_fd_zongGongLv;

                        string xuQiuShuLiang = DH + item.ReqQtyTU + DH + ",";
                        fd_qingGouMX_fd_xuQiuShuLiang = xuQiuShuLiang + fd_qingGouMX_fd_xuQiuShuLiang;

                        string requiredDeliveryDate = DH + item.RequiredDeliveryDate.ToString("yyyy-MM-dd") + DH + ",";
                        fd_qingGouMX_fd_yaoQiuJiaoHuoRiQi = requiredDeliveryDate + fd_qingGouMX_fd_yaoQiuJiaoHuoRiQi;
                    }
                    #endregion

                    formData.Append("\"fd_qingGouMX.fd_liaoHao\":");
                    formData.Append("[" + fd_qingGouMX_fd_liaoHao.Substring(0, fd_qingGouMX_fd_liaoHao.Length - 1) + "],");

                    formData.Append("\"fd_qingGouMX.fd_pinMing\":");
                    formData.Append("[" + fd_qingGouMX_fd_pinMing.Substring(0, fd_qingGouMX_fd_pinMing.Length - 1) + "],");

                    formData.Append("\"fd_qingGouMX.fd_guiGe\":");
                    formData.Append("[" + fd_qingGouMX_fd_guiGe.Substring(0, fd_qingGouMX_fd_guiGe.Length - 1) + "],");

                    formData.Append("\"fd_qingGouMX.fd_dengJi\":");
                    formData.Append("[" + fd_qingGouMX_fd_dengJi.Substring(0, fd_qingGouMX_fd_dengJi.Length - 1) + "],");

                    formData.Append("\"fd_qingGouMX.fd_xiaoLv\":");
                    formData.Append("[" + fd_qingGouMX_fd_xiaoLv.Substring(0, fd_qingGouMX_fd_xiaoLv.Length - 1) + "],");

                    formData.Append("\"fd_qingGouMX.fd_gongLv\":");
                    formData.Append("[" + fd_qingGouMX_fd_gongLv.Substring(0, fd_qingGouMX_fd_gongLv.Length - 1) + "],");

                    formData.Append("\"fd_qingGouMX.fd_zongGongLv\":");
                    formData.Append("[" + fd_qingGouMX_fd_zongGongLv.Substring(0, fd_qingGouMX_fd_zongGongLv.Length - 1) + "],");

                    formData.Append("\"fd_qingGouMX.fd_xuQiuShuLiang\":");
                    formData.Append("[" + fd_qingGouMX_fd_xuQiuShuLiang.Substring(0, fd_qingGouMX_fd_xuQiuShuLiang.Length - 1) + "],");

                    formData.Append("\"fd_qingGouMX.fd_yaoQiuJiaoHuoRiQi\":");
                    formData.Append("[" + fd_qingGouMX_fd_yaoQiuJiaoHuoRiQi.Substring(0, fd_qingGouMX_fd_yaoQiuJiaoHuoRiQi.Length - 1) + "],");

                    formData.Append("}</formValues>");

                    #endregion

                    formData.Append("<docStatus>20</docStatus >");
                    formData.Append("<docCreator>{");
                    formData.Append("\"LoginName\":");
                    formData.Append("\"" + "ss1" + "\"}");
                    formData.Append("</docCreator>");
                    formData.Append("</arg0>");
                    formData.Append("</web:addReview>");
                    formData.Append("</soapenv:Body>");
                    formData.Append("</soapenv:Envelope>");

                    //发送格式
                    StringBuilder formSendData = new StringBuilder();

                    formSendData.Append(formData.ToString());

                    logger.Error("客户新增传出数据：" + formSendData.ToString());

                    string strURL = null;

                    //测试
                    //strURL = "http://118.195.189.35:8900/accessPlatform/platformAPI";

                    //正式
                    strURL = "http://58.216.169.102:9081/ekp/sys/webservice/kmReviewWebserviceService?wsdl";


                    string formSendDataGo = formSendData.ToString();

                    string responseText = HttpRequestClient.HttpPost(strURL, formSendDataGo, "", "");

                    #endregion
                }
            }
        }
    }
}
