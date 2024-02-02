using System.Data;
using System.Text;
using UFIDA.U9.MO.MO;
using UFIDA.U9.SM.ForecastOrder;
using UFIDA.U9.SM.SO;
using UFIDA.UBF.MD.Business;
using UFSoft.UBF.Business;
using UFSoft.UBF.PL;
using UFSoft.UBF.Util.DataAccess;
using UFSoft.UBF.Util.Log;

namespace YY.U9.Cust.LI.AppPlugIn
{
    /// <summary>
    /// 创联-需求分类
    /// </summary>
    [UFSoft.UBF.Eventing.Configuration.Failfast]
    class MOInsertedSubscrilber : UFSoft.UBF.Eventing.IEventSubscriber
    {
        private static readonly UFSoft.UBF.Util.Log.ILogger logger = LoggerManager.GetLogger(typeof(MOInsertedSubscrilber));

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
            MO mo = key.GetEntity() as MO;
            if (mo == null)
            {
                return;
            }
            #endregion

            try
            {
                string Des = "";

                ///string MoDemand = mo.DemandCode.Name;
                string MoDemand = "";
                string TDocNo = mo.DocNo;
                if (mo.MOSourceDocType.Value == 3 && !string.IsNullOrEmpty(TDocNo))
                {
                    long extEnumTypeID = 0L;

                    int maxEValue = -1;

                    #region 创建需求分类
                    ExtEnumValue extValue = ExtEnumValue.Finder.Find("ExtEnumType.Code=@TypeCode and Code=@Code",
                    new OqlParam("TypeCode", "UFIDA.U9.CBO.Enums.DemandCodeEnum"), new OqlParam("Code", TDocNo));
                    if (extValue != null)
                    {
                        mo.DemandCode = UFIDA.U9.CBO.Enums.DemandCodeEnum.GetFromValue(extValue.EValue);
                    }
                    else
                    {
                        StringBuilder sql = new StringBuilder(5000);
                        DataTable dataTable = new DataTable();
                        DataSet dataSet = new DataSet();
                        sql.Append(" select MAX(EValue) as Evalue,B.ID from UBF_Sys_ExtEnumValue A");
                        sql.Append(" left join UBF_Sys_ExtEnumType B on A.ExtEnumType=B.ID");
                        sql.Append(" where B.Code='UFIDA.U9.CBO.Enums.DemandCodeEnum'");
                        sql.Append(" group by B.ID");
                        DataAccessor.RunSQL(DataAccessor.GetConn(), sql.ToString(), null, out dataSet);
                        dataTable = dataSet.Tables[0];
                        if (dataTable.Rows != null && dataTable.Rows.Count > 0)
                        {
                            if (!string.IsNullOrEmpty(dataTable.Rows[0]["Evalue"].ToString()))
                            {
                                maxEValue = int.Parse(dataTable.Rows[0]["Evalue"].ToString());
                            }
                            if (!string.IsNullOrEmpty(dataTable.Rows[0]["ID"].ToString()))
                            {
                                extEnumTypeID = long.Parse(dataTable.Rows[0]["ID"].ToString());
                            }
                        }
                    }
                    if (extEnumTypeID > 0L)
                    {
                        ExtEnumType enumType = ExtEnumType.Finder.FindByID(extEnumTypeID);
                        if (enumType != null)
                        {
                            int newEnumVValue = maxEValue + 1;
                            using (ISession session = Session.Open())
                            {
                                ExtEnumValue newEnumValue = ExtEnumValue.Create(enumType);
                                newEnumValue.Code = TDocNo;
                                newEnumValue.Name = TDocNo;
                                newEnumValue.EValue = newEnumVValue;
                                session.Commit();
                            }
                            mo.DemandCode = UFIDA.U9.CBO.Enums.DemandCodeEnum.GetFromValue(newEnumVValue);
                        }
                    }
                    #endregion

                }

                if (mo.DemandCode == null)
                {
                    return;
                }

                int MoDemandVal = mo.DemandCode.Value;//值是作为查询条件

                if (MoDemandVal == -1)
                {
                    return;
                }

                string MoDocNo = "";

                string MoDocNoLine = "";

                string MoDemandCode = "";

                string FindSoID = "";

                string FindForID = "";

                string Des2 = "";//客户

                string Des3 = "";//销售业务员

                DataTable dataTable1e = new DataTable();
                DataSet dataSet1e = new DataSet();
                string sqle = "select A.Code from UBF_Sys_ExtEnumValue A left join UBF_Sys_ExtEnumType B on A.ExtEnumType = B.ID" +
                    " where B.Code = 'UFIDA.U9.CBO.Enums.DemandCodeEnum' AND A.EValue = '" + MoDemandVal + "'";
                DataAccessor.RunSQL(DataAccessor.GetConn(), sqle, null, out dataSet1e);
                dataTable1e = dataSet1e.Tables[0];
                if (dataTable1e.Rows != null && dataTable1e.Rows.Count > 0)
                {
                    MoDemandCode = dataTable1e.Rows[0]["Code"].ToString();
                }

                MoDemand = MoDemandCode.Substring(0, 2);

                MoDocNoLine = MoDemandCode.Substring(MoDemandCode.Length - 2);

                MoDocNo = MoDemandCode.Substring(0, MoDemandCode.Length - 3);

                if (!string.IsNullOrEmpty(MoDemandVal.ToString()))//生产订单的需求分类的值不能为空
                {
                    DataTable dataTable = new DataTable();
                    DataSet dataSet = new DataSet();
                    string sql = "";
                    if (MoDemand == "CL")//标准销售
                    {
                        sql = "SELECT b.DescFlexField_PrivateDescSeg1,a.ID FROM SM_SO a INNER JOIN SM_SOLine b ON  a.ID=b.SO WHERE a.DocNo='" + MoDocNo + "' AND DocLineNo='" + MoDocNoLine + "'";
                    }
                    else if (MoDemand == "FO")//预测订单
                    {
                        sql = "SELECT b.DescFlexField_PrivateDescSeg1,a.ID FROM SM_ForecastOrder a INNER JOIN SM_ForecastOrderLine b ON a.ID=b.ForecastOrder WHERE a.DocNo='" + MoDocNo + "' AND DocLineNo='" + MoDocNoLine + "'";
                    }
                    DataAccessor.RunSQL(DataAccessor.GetConn(), sql, null, out dataSet);
                    dataTable = dataSet.Tables[0];
                    if (dataTable.Rows != null && dataTable.Rows.Count > 0)
                    {
                        if (MoDemand == "CL")
                        {
                            Des = dataTable.Rows[0]["DescFlexField_PrivateDescSeg1"].ToString();
                            FindSoID = dataTable.Rows[0]["ID"].ToString();
                            SO sO = SO.Finder.FindByID(FindSoID);
                            if (sO.OrderBy.Customer != null)
                            {
                                Des2 = sO.OrderBy.Customer.Code;
                                Des3 = sO.OrderBy.Customer.Saleser.Name;
                            }

                        }
                        else if (MoDemand == "FO")
                        {
                            Des = dataTable.Rows[0]["DescFlexField_PrivateDescSeg1"].ToString();
                            FindForID = dataTable.Rows[0]["ID"].ToString();
                            ForecastOrder forecastOrder = ForecastOrder.Finder.FindByID(FindForID);
                            if (forecastOrder.Customer.Customer != null)
                            {
                                Des2 = forecastOrder.Customer.Customer.Code;
                                Des3 = forecastOrder.Customer.Customer.Saleser.Name;
                            }

                        }
                    }

                    mo.DescFlexField.PrivateDescSeg1 = Des;
                    mo.DescFlexField.PrivateDescSeg2 = Des2;
                    mo.DescFlexField.PrivateDescSeg3 = Des3;

                }

            }
            catch (System.Exception)
            {

                return;
            }
        }
    }
}
