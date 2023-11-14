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
    /// 创联-需求分类-新增
    /// </summary>
    [UFSoft.UBF.Eventing.Configuration.Failfast]

    class CLSalePlanInsertedSubscrilber : UFSoft.UBF.Eventing.IEventSubscriber
    {
        private static readonly UFSoft.UBF.Util.Log.ILogger logger = LoggerManager.GetLogger(typeof(CLSalePlanInsertedSubscrilber));

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
            UFIDA.U9.SPL.SalePlan.SalePlan SalePlan = key.GetEntity() as UFIDA.U9.SPL.SalePlan.SalePlan;
            if (SalePlan == null)
            {
                return;
            }
            #endregion

            string DocNo = "";//单号

            int DocLineNo = 0;//分期

            string TDocNo = "";

            long extEnumTypeID = 0L;

            int maxEValue = -1;

            DocNo = SalePlan.PlanNumber;

            foreach (var item in SalePlan.SalePlanPlots)
            {
                DocLineNo = item.DocLineNo;
                TDocNo = DocNo + "_" + DocLineNo;
                foreach (var it in item.SalePlanLines)
                {
                    #region 先查询是否有有则直接使用现有的
                    DataTable dataTable1 = new DataTable();
                    string sqlFor1 = "select A.Code,A.EValue from UBF_Sys_ExtEnumValue A" +
                        " left join UBF_Sys_ExtEnumType B on A.ExtEnumType = B.ID " +
                        " where B.Code = 'UFIDA.U9.CBO.Enums.DemandCodeEnum' " +
                        " and A.Code = '" + TDocNo + "'";
                    DataSet dataSet1 = new DataSet();
                    DataAccessor.RunSQL(DataAccessor.GetConn(), sqlFor1, null, out dataSet1);
                    dataTable1 = dataSet1.Tables[0];
                    string ACode = "";
                    string AEValue = "";
                    if (dataTable1 != null && dataTable1.Rows.Count > 0)
                    {
                        ACode = dataTable1.Rows[0]["Code"].ToString();
                        AEValue = dataTable1.Rows[0]["EValue"].ToString();
                    }
                    #endregion

                    if (string.IsNullOrEmpty(AEValue))
                    {
                        it.DemandType = UFIDA.U9.CBO.Enums.DemandCodeEnum.GetFromValue(AEValue);                        
                    }
                    else
                    {
                        #region 创建需求分类
                        ExtEnumValue extValue = ExtEnumValue.Finder.Find("ExtEnumType.Code=@TypeCode and Code=@Code",
                        new OqlParam("TypeCode", "UFIDA.U9.CBO.Enums.DemandCodeEnum"), new OqlParam("Code", TDocNo));
                        if (extValue != null)
                        {
                            it.DemandType = UFIDA.U9.CBO.Enums.DemandCodeEnum.GetFromValue(extValue.EValue);
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
                                it.DemandType = UFIDA.U9.CBO.Enums.DemandCodeEnum.GetFromValue(newEnumVValue);
                            }
                        }
                        #endregion
                    }
                }
            }
        }
    }
}
