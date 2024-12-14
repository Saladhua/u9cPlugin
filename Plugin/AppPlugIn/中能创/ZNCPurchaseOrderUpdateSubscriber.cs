using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using UFIDA.U9.Base;
using UFIDA.U9.CBO.SCM.Supplier;
using UFIDA.U9.PM.PO;
using UFIDA.U9.UI.PDHelper;
using UFSoft.UBF.Business;
using UFSoft.UBF.Util.DataAccess;
using UFSoft.UBF.Util.Log;

namespace YY.U9.Cust.LI.AppPlugIn
{
    /// <summary>
    ///  位置D:\setups\porject\中能创\2024-08\中能创流程接口\中能创流程接口\2采购订单
    /// 调用接口
    /// 中能创
    /// </summary>
    [UFSoft.UBF.Eventing.Configuration.Failfast]
    class ZNCPurchaseOrderUpdateSubscriber : UFSoft.UBF.Eventing.IEventSubscriber
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
            PurchaseOrder purchaseOrder = key.GetEntity() as PurchaseOrder;
            if (purchaseOrder == null)
            {
                return;
            }
            #endregion
            #region 调用接口
            //if (purchaseOrder.Status.Value == 1 && purchaseOrder.OriginalData.Status.Value == 0)
            if (purchaseOrder.Status == PODOCStatusEnum.Approving && purchaseOrder.OriginalData.Status == PODOCStatusEnum.Opened)
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

                string docSubject = "采购订单";

                string fdTemplateId = "19130d973dbab4db6e2135f4e1292fc2";

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
                formData.Append("\"" + purchaseOrder.DocNo + "\",");

                formData.Append("\"fd_danJuLeiXing\":");
                formData.Append("\"" + purchaseOrder.DocType.Code + "\",");

                formData.Append("\"fd_BeiZhu\":");
                string Description = "";
                foreach (var item in purchaseOrder.POMemos)
                {
                    Description = item.Description;
                }

                formData.Append("\"" + Description + "\",");

                formData.Append("\"fd_DaoHuoRiQi\":");

                string NeedPODate = "";

                foreach (var item in purchaseOrder.POLines)
                {
                    foreach (var pOShiplines in item.POShiplines)
                    {
                        if (pOShiplines != null)
                        NeedPODate = pOShiplines.NeedPODate.ToString("yyyy-MM-dd");
                    }
                }

                formData.Append("\"" + NeedPODate + "\",");

                formData.Append("\"fd_FuKuanTiaoJian\":");
                string paymentTerm = "";
                foreach (var item in purchaseOrder.POLines)
                {
                    if (item.PaymentTerm != null)
                    {
                        paymentTerm = item.PaymentTerm.Name;
                    }
                }
                formData.Append("\"" + paymentTerm + "\",");


                formData.Append("\"fd_gongYingShang\":");
                if (purchaseOrder.Supplier != null)
                {
                    formData.Append("\"" + purchaseOrder.Supplier.Supplier.Name + "\",");
                }
                else
                {
                    formData.Append("\"" + " " + "\",");
                }

                //formData.Append("\"fd_heSuanZuZhi\":");
                //formData.Append("\"" + purchaseOrder.Org.Name + "\",");

                formData.Append("\"fd_riQi\":");
                formData.Append("\"" + purchaseOrder.BusinessDate.ToString("yyyy-MM-dd") + "\",");

                formData.Append("\"fd_zhuangTai\":");
                string StatusName = "开立";
                switch (purchaseOrder.Status.Value)
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

                formData.Append("\"fd_biZhong\":");
                if (purchaseOrder.TC != null)
                {
                    formData.Append("\"" + purchaseOrder.TC.Name + "\",");
                }
                else
                {
                    formData.Append("\"" + "" + "\",");
                }
                formData.Append("\"fd_yWYuan\":");
                if (purchaseOrder.PurOper != null)
                {
                    formData.Append("\"" + purchaseOrder.PurOper.Name + "\",");
                }
                else
                {
                    formData.Append("\"" + " " + "\",");
                }


                formData.Append("\"fd_shuiZuHe\":");
                if (purchaseOrder.TaxSchedule != null)
                {
                    formData.Append("\"" + purchaseOrder.TaxSchedule.Name + "\",");
                }
                else
                {
                    formData.Append("\"" + "" + "\",");
                }

                formData.Append("\"fd_shuiLv\":");
                formData.Append("\"" + purchaseOrder.TaxRate.ToString("0.####") + "\",");

                formData.Append("\"fd_caiGouHeTongBianHao\":");
                formData.Append("\"" + purchaseOrder.DescFlexField.PubDescSeg12 + "\",");

                formData.Append("\"fd_ZbaoQi\":");//必须是日期格式
                formData.Append("\"" + purchaseOrder.DescFlexField.PrivateDescSeg2 + "\",");
                //formData.Append("\"" + " " + "\",");

                formData.Append("\"fd_CultureName\":");
                formData.Append("\"" + "zh-CN" + "\",");

                formData.Append("\"fd_u9_OrgCode\":");
                formData.Append("\"" + purchaseOrder.Org.Code + "\",");

                formData.Append("\"fd_u9_UserCode\":");
                formData.Append("\"" + PDContext.Current.UserCode + "\",");

                //formData.Append("\"fd_OrgCode\":");
                //formData.Append("\"" + purchaseOrder.Org.Code + "\",");

                //formData.Append("\"fd_UserCode\":");
                //formData.Append("\"" + PDContext.Current.UserCode + "\",");

                //--不传

                formData.Append("\"fd_EntCode\":");
                formData.Append("\"" + "011" + "\",");

                formData.Append("\"fd_businessType\":");
                formData.Append("\"" + "PR" + "\",");

                formData.Append("\"fd_requestData\":");
                formData.Append("\"" + " " + "\",");

                #region 循环行
                string fd_caiGouDingDanMXfd_liaoHao = "";
                string fd_caiGouDingDanMXfd_pinMing = "";
                string fd_caiGouDingDanMXfd_guiGe = "";
                string fd_caiGouDingDanMXfd_dengJi = "";
                string fd_caiGouDingDanMXfd_xiaoLv = "";
                string fd_caiGouDingDanMXfd_gongLv = "";
                string fd_caiGouDingDanMXfd_zongGongLv = "";
                string fd_caiGouDingDanMXfd_xuQiuShuLiang = "";
                string fd_caiGouDingDanMXfd_caiGouShuLiang = "";
                string fd_caiGouDingDanMXfd_caiGouDanWei = "";
                string fd_caiGouDingDanMXfd_jiaoQi = "";
                string fd_caiGouDingDanMXfd_zuiZhongJia = "";
                string fd_caiGouDingDanMXfd_weiShuiJinE = "";
                string fd_caiGouDingDanMXfd_shuiE = "";
                string fd_caiGouDingDanMXfd_jiaShuiHeJi = "";
                foreach (var item in purchaseOrder.POLines)
                {
                    string DH = "\"";
                    string iteminfoCode = DH + item.ItemInfo.ItemID.Code + DH + ",";
                    fd_caiGouDingDanMXfd_liaoHao = iteminfoCode + fd_caiGouDingDanMXfd_liaoHao;

                    string iteminfoName = DH + item.ItemInfo.ItemID.Name + DH + ",";
                    fd_caiGouDingDanMXfd_pinMing = iteminfoName + fd_caiGouDingDanMXfd_pinMing;

                    string iteminfoSPEC = DH + item.ItemInfo.ItemID.SPECS + DH + ",";
                    fd_caiGouDingDanMXfd_guiGe = iteminfoSPEC + fd_caiGouDingDanMXfd_guiGe;

                    string iteminfoGrade = DH + item.ItemInfo.ItemGrade.Name + DH + ",";
                    fd_caiGouDingDanMXfd_dengJi = iteminfoGrade + fd_caiGouDingDanMXfd_dengJi;

                    //string iteminfoPotency = DH + item.ItemInfo.ItemPotency.Name + DH + ",";
                    string iteminfoPotency = "";
                    string iteminfoPotencyName = "";
                    if (item.ItemInfo.ItemPotency != null)
                    {
                        DataTable dataTable = new DataTable();
                        DataSet dataSet = new DataSet();
                        string sql = "select D.Name from UBF_Sys_ExtEnumValue A " +
                            " left join UBF_Sys_ExtEnumType B on A.ExtEnumType = B.ID " +
                            " left join UBF_Sys_ExtEnumType_Trl C on C.ID = B.ID " +
                            " left join UBF_Sys_ExtEnumValue_Trl D on D.ID = A.ID " +
                            " where B.Code = 'UFIDA.U9.CBO.SCM.Item.ElementEnum' and A.EValue = '" + item.ItemInfo.ItemPotency.Value + "'";
                        DataAccessor.RunSQL(DataAccessor.GetConn(), sql, null, out dataSet);
                        dataTable = dataSet.Tables[0];
                        if (dataTable.Rows != null && dataTable.Rows.Count > 0)
                        {
                            iteminfoPotencyName = dataTable.Rows[0]["Name"].ToString();
                        }
                        iteminfoPotency = DH + iteminfoPotencyName + DH + ",";
                    }
                    else
                    {
                        iteminfoPotency = DH + "" + DH + ",";
                    }
                    fd_caiGouDingDanMXfd_xiaoLv = iteminfoPotency + fd_caiGouDingDanMXfd_xiaoLv;

                    string itemDP1 = DH + item.DescFlexSegments.PubDescSeg1 + DH + ",";
                    fd_caiGouDingDanMXfd_gongLv = itemDP1 + fd_caiGouDingDanMXfd_gongLv;

                    string itemDP2 = DH + item.DescFlexSegments.PubDescSeg2 + DH + ",";
                    fd_caiGouDingDanMXfd_zongGongLv = itemDP2 + fd_caiGouDingDanMXfd_zongGongLv;

                    string itemReqQtyTU = DH + item.ReqQtyTU + DH + ",";
                    fd_caiGouDingDanMXfd_xuQiuShuLiang = itemReqQtyTU + fd_caiGouDingDanMXfd_xuQiuShuLiang;

                    string itemPurQtyPU = DH + item.PurQtyPU + DH + ",";
                    fd_caiGouDingDanMXfd_caiGouShuLiang = itemPurQtyPU + fd_caiGouDingDanMXfd_caiGouShuLiang;

                    string itemTradeUOM = DH + item.TradeUOM.Name + DH + ",";
                    fd_caiGouDingDanMXfd_caiGouDanWei = itemTradeUOM + fd_caiGouDingDanMXfd_caiGouDanWei;

                    string posldate = "";//交期
                    foreach (var item2 in item.POShiplines)
                    {
                        posldate = item2.DeliveryDate.ToString("yyyy-MM-dd");
                    }
                    string itemposldate = DH + posldate + DH + ",";
                    fd_caiGouDingDanMXfd_jiaoQi = itemposldate + fd_caiGouDingDanMXfd_jiaoQi;

                    string itemFinallyPriceTC = DH + item.FinallyPriceTC + DH + ",";
                    fd_caiGouDingDanMXfd_zuiZhongJia = itemFinallyPriceTC + fd_caiGouDingDanMXfd_zuiZhongJia;

                    string itemNetMnyTC = DH + item.NetMnyTC + DH + ",";
                    fd_caiGouDingDanMXfd_weiShuiJinE = itemNetMnyTC + fd_caiGouDingDanMXfd_weiShuiJinE;

                    string itemTotalTaxTC = DH + item.TotalTaxTC + DH + ",";
                    fd_caiGouDingDanMXfd_shuiE = itemTotalTaxTC + fd_caiGouDingDanMXfd_shuiE;

                    string itemTotalMnyTC = DH + item.TotalMnyTC + DH + ",";
                    fd_caiGouDingDanMXfd_jiaShuiHeJi = itemTotalMnyTC + fd_caiGouDingDanMXfd_jiaShuiHeJi;

                }
                #endregion

                formData.Append("\"fd_caiGouDingDanMX.fd_liaoHao\":");
                formData.Append("[" + fd_caiGouDingDanMXfd_liaoHao.Substring(0, fd_caiGouDingDanMXfd_liaoHao.Length - 1) + "],");

                formData.Append("\"fd_caiGouDingDanMX.fd_pinMing\":");
                formData.Append("[" + fd_caiGouDingDanMXfd_pinMing.Substring(0, fd_caiGouDingDanMXfd_pinMing.Length - 1) + "],");

                formData.Append("\"fd_caiGouDingDanMX.fd_guiGe\":");
                formData.Append("[" + fd_caiGouDingDanMXfd_guiGe.Substring(0, fd_caiGouDingDanMXfd_guiGe.Length - 1) + "],");

                formData.Append("\"fd_caiGouDingDanMX.fd_dengJi\":");
                formData.Append("[" + fd_caiGouDingDanMXfd_dengJi.Substring(0, fd_caiGouDingDanMXfd_dengJi.Length - 1) + "],");

                formData.Append("\"fd_caiGouDingDanMX.fd_xiaoLv\":");
                formData.Append("[" + fd_caiGouDingDanMXfd_xiaoLv.Substring(0, fd_caiGouDingDanMXfd_xiaoLv.Length - 1) + "],");

                formData.Append("\"fd_caiGouDingDanMX.fd_gongLv\":");
                formData.Append("[" + fd_caiGouDingDanMXfd_gongLv.Substring(0, fd_caiGouDingDanMXfd_gongLv.Length - 1) + "],");

                formData.Append("\"fd_caiGouDingDanMX.fd_zongGongLv\":");
                formData.Append("[" + fd_caiGouDingDanMXfd_zongGongLv.Substring(0, fd_caiGouDingDanMXfd_zongGongLv.Length - 1) + "],");

                formData.Append("\"fd_caiGouDingDanMX.fd_xuQiuShuLiang\":");
                formData.Append("[" + fd_caiGouDingDanMXfd_xuQiuShuLiang.Substring(0, fd_caiGouDingDanMXfd_xuQiuShuLiang.Length - 1) + "],");

                formData.Append("\"fd_caiGouDingDanMX.fd_caiGouShuLiang\":");
                formData.Append("[" + fd_caiGouDingDanMXfd_caiGouShuLiang.Substring(0, fd_caiGouDingDanMXfd_caiGouShuLiang.Length - 1) + "],");

                formData.Append("\"fd_caiGouDingDanMX.fd_caiGouDanWei\":");
                formData.Append("[" + fd_caiGouDingDanMXfd_caiGouDanWei.Substring(0, fd_caiGouDingDanMXfd_caiGouDanWei.Length - 1) + "],");

                formData.Append("\"fd_caiGouDingDanMX.fd_jiaoQi\":");
                formData.Append("[" + fd_caiGouDingDanMXfd_jiaoQi.Substring(0, fd_caiGouDingDanMXfd_jiaoQi.Length - 1) + "],");

                formData.Append("\"fd_caiGouDingDanMX.fd_zuiZhongJia\":");
                formData.Append("[" + fd_caiGouDingDanMXfd_zuiZhongJia.Substring(0, fd_caiGouDingDanMXfd_zuiZhongJia.Length - 1) + "],");

                formData.Append("\"fd_caiGouDingDanMX.fd_weiShuiJinE\":");
                formData.Append("[" + fd_caiGouDingDanMXfd_weiShuiJinE.Substring(0, fd_caiGouDingDanMXfd_weiShuiJinE.Length - 1) + "],");

                formData.Append("\"fd_caiGouDingDanMX.fd_shuiE\":");
                formData.Append("[" + fd_caiGouDingDanMXfd_shuiE.Substring(0, fd_caiGouDingDanMXfd_shuiE.Length - 1) + "],");

                formData.Append("\"fd_caiGouDingDanMX.fd_jiaShuiHeJi\":");
                formData.Append("[" + fd_caiGouDingDanMXfd_jiaShuiHeJi.Substring(0, fd_caiGouDingDanMXfd_jiaShuiHeJi.Length - 1) + "]");

                formData.Append("}</formValues>");

                #endregion

                formData.Append("<docStatus>20</docStatus >");
                formData.Append("<docCreator>{");
                formData.Append("\"LoginName\":");
                formData.Append("\"" + PDContext.Current.UserCode + "\"}");
                formData.Append("</docCreator>");
                formData.Append("</arg0>");
                formData.Append("</web:addReview>");
                formData.Append("</soapenv:Body>");
                formData.Append("</soapenv:Envelope>");

                //发送格式
                StringBuilder formSendData = new StringBuilder();

                formSendData.Append(formData.ToString());

                logger.Error("采购订单新增传出数据：" + formSendData.ToString());

                string strURL = null;

                //测试
                //strURL = "http://118.195.189.35:8900/accessPlatform/platformAPI";

                //正式
                //strURL = "http://58.216.169.102:9081/ekp/sys/webservice/kmReviewWebserviceService?wsdl";
                long orgID = purchaseOrder.Org.ID;
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
