
using UFSoft.UBF.Business;
using UFSoft.UBF.Util.DataAccess;
using UFSoft.UBF.Util.Log;
using UFIDA.U9.CBO.MFG.BOM;
using UFIDA.U9.MO.MO;
using UFIDA.U9.MO.ManufactureSimu;
 
using System.Text;
using System;
using System.Data;
using YY.U9.Cust.LI.AppPlugIn;

namespace YY.U9.Cust.AY.Liu.AppPlugIn.YK
{
    [UFSoft.UBF.Eventing.Configuration.Failfast]
    class SimuDocSubscriber : UFSoft.UBF.Eventing.IEventSubscriber
    {

        private static readonly ILogger logger = LoggerManager.GetLogger(typeof(SimuDocSubscriber));
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
            SimuDoc simuDoc = key.GetEntity() as SimuDoc;
            if (simuDoc == null)
            {
                return;
            }

            UFSoft.UBF.Transactions.UBFTransactionContext.Current.Committed +=

                    new System.Transactions.TransactionCompletedEventHandler(BusinessTransactionSucess);

             void BusinessTransactionSucess(object obj, System.Transactions.TransactionEventArgs e)

            {
                int a = 0;
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
                string txid = "U9E0003";
                string systype = "U9";
                string termno = "U901";
                string seqno = "GU9000U90120230321121201000001";
                StringBuilder formData = new StringBuilder();

                formData.Append("\"header\":{");
                formData.Append("\"txid\":\"");
                formData.Append(txid).Append("\",");
                formData.Append("\"systype\":\"");
                formData.Append(systype).Append("\",");
                formData.Append("\"termno\":\"");
                formData.Append(termno).Append("\",");
                formData.Append("\"seqno\":\"");
                formData.Append(seqno).Append("\"},");
                formData.Append("\"body\":{\"");
                formData.Append("marketOrderDetailIdArray\":[");

                if (simuDoc.SimuDocFilter.Count > 0)
                {
                
                    for (int i=a; i < simuDoc.SimuDocFilter.Count; i++)
                    {
                        DataTable dataTable1 = new DataTable();
                        string ver1 = "select  SetableStatus from  MO_ItemWIPSimu where MO=" + simuDoc.SimuDocFilter[i].MO.ID + " and SimuDoc=" + simuDoc.ID + "";

                        DataSet set1 = new DataSet();
                        DataAccessor.RunSQL(DataAccessor.GetConn(), ver1, null, out set1);
                        dataTable1 = set1.Tables[0];
                        if (dataTable1.Rows != null && dataTable1.Rows.Count > 0)
                        {

                         

                            if (dataTable1.Rows[0]["SetableStatus"].ToString() == "2" || dataTable1.Rows[0]["SetableStatus"].ToString() == "3")
                            {

                                formData.Append("\"" + simuDoc.SimuDocFilter[i].MO.DocNo + "\"");
                                a = i;
                                break;
                            }

                        }
                    }

                    for (int i = a+1; i < simuDoc.SimuDocFilter.Count; i++)
                    {
                        DataTable dataTable1 = new DataTable();
                        string ver1 = "select  SetableStatus from  MO_ItemWIPSimu where MO=" + simuDoc.SimuDocFilter[i].MO.ID + " and SimuDoc=" + simuDoc.ID + "";

                        DataSet set1 = new DataSet();
                        DataAccessor.RunSQL(DataAccessor.GetConn(), ver1, null, out set1);
                        dataTable1 = set1.Tables[0];
                        if (dataTable1.Rows != null && dataTable1.Rows.Count > 0)
                        {



                            if (dataTable1.Rows[0]["SetableStatus"].ToString() == "2"|| dataTable1.Rows[0]["SetableStatus"].ToString() == "3" )
                            {

                                formData.Append(",").Append("\"" + simuDoc.SimuDocFilter[i].MO.DocNo + "\"");
                             
                            }

                        }

                    }
                      

                    formData.Append("]}");

                }
                 


                //发送格式
                StringBuilder formSendData = new StringBuilder();
                formSendData.Append("{");

                formSendData.Append(formData.ToString());

                formSendData.Append("}");

                logger.Error("客户新增传出数据：" + formSendData.ToString());
                string strURL = null;
                //测试
                strURL = "http://118.195.189.35:8900/accessPlatform/platformAPI";
                //正式
                //strURL = "http://81.68.204.126:9900/accessPlatform/platformAPI";
                string responseText = HttpRequestClient.HttpPost(strURL, formSendData.ToString());

                #endregion
            }




        }
    }
}
