using System;
using System.Data;
using UFIDA.U9.InvDoc.TransferForm;
using UFIDA.U9.InvDoc.TransferIn;
using UFSoft.UBF.Business;
using UFSoft.UBF.Util.DataAccess;
using UFSoft.UBF.Util.Log;

namespace YY.U9.Cust.LI.AppPlugIn
{
    //宏巨 形态转换 功能 
    [UFSoft.UBF.Eventing.Configuration.Failfast]
    class TransferFormUpdatingSubscriber : UFSoft.UBF.Eventing.IEventSubscriber
    {
        /// <summary>
        /// 形态转换子行 更新
        /// </summary>
        private static readonly ILogger logger = LoggerManager.GetLogger(typeof(TransferFormSLInsertedSubscriber));
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
            TransferFormSL item = key.GetEntity() as TransferFormSL;
            if (item == null)
            {
                return;
            }

            string lineid = item.TransferFormL.ID.ToString();
            string lotinfo_docno = "";
            //string itemmasterid = item["ItemInfo_ItemID"].ToString();
            string itemmasterid = item.TransferFormL.ItemInfo.ItemID.ID.ToString();
            DataTable dataTable_1 = new DataTable();
            string sql_1 = "SELECT DocNo FROM Lot_LotMaster WHERE ID" +
                " =(SELECT LotInfo_LotMaster FROM InvDoc_TransferFormL INNER JOIN InvDoc_TransferForm ON InvDoc_TransferFormL.TransferForm=InvDoc_TransferForm.ID " +
                " WHERE InvDoc_TransferFormL.ID='" + lineid + "')";
            DataSet dataSet_1 = new DataSet();
            DataAccessor.RunSQL(DataAccessor.GetConn(), sql_1, null, out dataSet_1);
            dataTable_1 = dataSet_1.Tables[0];
            if (dataTable_1.Rows != null && dataTable_1.Rows.Count > 0)
            {
                lotinfo_docno = dataTable_1.Rows[0]["DocNo"].ToString();
            }
            else
            {
                return;
            }
            string see = lotinfo_docno.Substring(0, 3);
            string see2 = lotinfo_docno.Substring(2, 3);
            string rcvdocno = "";//标准收货单据的单号
            string gysname = "";
            string gyscode = "";
            long gysID = 0;
            string scdate = "";
            string confirmDate = "";
            string shortName = "";
            bool start = false;

            //找到了该行的批号的创建单据，需要取出来进行判断
            if (lotinfo_docno.Substring(0, 3) == "Tra")//tra就是形态转换的单号，就能找到对应的收货的东西
            {
                #region 执行SQL
                DataTable dataTable_2 = new DataTable();
                string sql_2 = "SELECT DocNo FROM Lot_LotMaster WHERE ID=(SELECT LotInfo_LotMaster FROM InvDoc_TransferForm " +
                    " INNER JOIN InvDoc_TransferFormL ON InvDoc_TransferForm.ID = InvDoc_TransferFormL.TransferForm" +
                    " WHERE DocNo = '" + lotinfo_docno + "')"; ;
                DataSet dataSet_2 = new DataSet();
                DataAccessor.RunSQL(DataAccessor.GetConn(), sql_2, null, out dataSet_2);
                dataTable_2 = dataSet_2.Tables[0];
                if (dataTable_2.Rows != null && dataTable_2.Rows.Count > 0)
                {
                    rcvdocno = dataTable_2.Rows[0]["DocNo"].ToString();
                }
                #endregion

                #region 执行SQL
                DataTable dataTable_3 = new DataTable();
                string sql_3 = "SELECT ConfirmDate,InvLotEnableDate,Supplier_Supplier,(SELECT  Name  FROM CBO_Supplier_Trl WHERE ID = Supplier_Supplier AND SysMLFlag='zh-CN') AS GYS, " +
                    "(SELECT  Code  FROM CBO_Supplier WHERE ID = Supplier_Supplier) AS GYSCode," +
                    "(SELECT  ShortName  FROM CBO_Supplier WHERE ID = Supplier_Supplier) AS ShortName" +
                    " FROM PM_Receivement INNER JOIN PM_RcvLine ON PM_Receivement.ID = PM_RcvLine.Receivement " +
                    " WHERE DocNo = '" + rcvdocno + "' AND PM_RcvLine.ItemInfo_ItemID='" + itemmasterid + "'";
                DataSet dataSet_3 = new DataSet();
                DataAccessor.RunSQL(DataAccessor.GetConn(), sql_3, null, out dataSet_3);
                dataTable_3 = dataSet_3.Tables[0];
                if (dataTable_3.Rows != null && dataTable_3.Rows.Count > 0)
                {
                    confirmDate = dataTable_3.Rows[0]["ConfirmDate"].ToString();
                    scdate = dataTable_3.Rows[0]["InvLotEnableDate"].ToString();
                    gysname = dataTable_3.Rows[0]["GYS"].ToString();
                    gyscode = dataTable_3.Rows[0]["GYSCode"].ToString();
                    shortName = dataTable_3.Rows[0]["ShortName"].ToString();
                    gysID = long.Parse(dataTable_3.Rows[0]["Supplier_Supplier"].ToString());
                }
                #endregion
            }
            else if (lotinfo_docno.Substring(2, 3) == "RCV")//rcv就是收货的单号，
            {
                rcvdocno = lotinfo_docno;
                #region 执行SQL
                DataTable dataTable_3 = new DataTable();
                string sql_3 = "SELECT ConfirmDate,InvLotEnableDate,Supplier_Supplier,(SELECT  Name  FROM CBO_Supplier_Trl WHERE ID = Supplier_Supplier AND SysMLFlag='zh-CN') AS GYS, " +
                    "(SELECT  Code  FROM CBO_Supplier WHERE ID = Supplier_Supplier) AS GYSCode," +
                    "(SELECT  ShortName  FROM CBO_Supplier WHERE ID = Supplier_Supplier) AS ShortName" +
                    " FROM PM_Receivement INNER JOIN PM_RcvLine ON PM_Receivement.ID = PM_RcvLine.Receivement " +
                    " WHERE DocNo = '" + rcvdocno + "' AND PM_RcvLine.ItemInfo_ItemID='" + itemmasterid + "'";
                DataSet dataSet_3 = new DataSet();
                DataAccessor.RunSQL(DataAccessor.GetConn(), sql_3, null, out dataSet_3);
                dataTable_3 = dataSet_3.Tables[0];
                if (dataTable_3.Rows != null && dataTable_3.Rows.Count > 0)
                {
                    confirmDate = dataTable_3.Rows[0]["ConfirmDate"].ToString();
                    scdate = dataTable_3.Rows[0]["InvLotEnableDate"].ToString();
                    gysname = dataTable_3.Rows[0]["GYS"].ToString();
                    gyscode = dataTable_3.Rows[0]["GYSCode"].ToString();
                    shortName = dataTable_3.Rows[0]["ShortName"].ToString();
                    gysID = long.Parse(dataTable_3.Rows[0]["Supplier_Supplier"].ToString());
                }

                
                start = true;
                #endregion
            }
            if (start == true)
            {
                item.SupplierInfo.Code = gyscode;
                item.SupplierInfo.Name = gysname;
                item.SupplierInfo.ShortName = shortName;
                //item.SupplierInfo.Supplier.ID = gysID;
                #region 正式专用
                item.DescFlexSegments.PrivateDescSeg2 = scdate;
                item.DescFlexSegments.PrivateDescSeg3 = confirmDate;
                #endregion
                #region 测试专用
                //item.DescFlexSegments.PrivateDescSeg3 = scdate;
                //item.DescFlexSegments.PrivateDescSeg4 = confirmDate;
                #endregion
            }
            #endregion
        }

    }
}
