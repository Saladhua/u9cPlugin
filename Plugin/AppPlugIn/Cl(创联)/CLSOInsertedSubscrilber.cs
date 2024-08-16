using System;
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
    class CLSOInsertedSubscrilber : UFSoft.UBF.Eventing.IEventSubscriber
    {
        private static readonly UFSoft.UBF.Util.Log.ILogger logger = LoggerManager.GetLogger(typeof(CLSOInsertedSubscrilber));

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
            SO so = key.GetEntity() as SO;
            if (so == null)
            {
                return;
            }
            #endregion


            string DocNo = "";//单号

            string TDocNo = "";

            DocNo = so.DocNo;

            int LineNo = 0;//行号

            string des3 = "";//生产备注

            long extEnumTypeID = 0L;

            int maxEValue = -1;
            //枚举赋值
            //插入之后
            #region 计入计划勾选使用
            DateTime BusinessDate = DateTime.Now;
            DateTime FirstDayOfMonth = DateTime.Now;

            #endregion

            foreach (var item in so.SOLines)
            {
                foreach (var i in item.SOShiplines)
                {
                    if (!string.IsNullOrEmpty(i.SrcDocNo))
                    {
                        string see = i.SrcDocNo;
                        return;
                    }
                }
            } 

            #region 实际SQL
            //select* from SPL_SalePlan

            //select* from SPL_SalePLanPlot where SalePlan = '1002311130110004'

            //select* from SPL_SalePlanLine

            //可能使用fendbyID直接找
            #endregion
            //if (so.SOSrcType.Value == 0 && so.DocumentType.IsInSalePlan == true)
            //{
            //    return;
            //}

            //测试SQL
            //select DemandType from SPL_SalePlanLine a
            //INNER JOIN SPL_SalePLanPlot  b ON a.SalePlanPlot = b.ID
            //INNER JOIN SPL_SalePlan c ON c.ID = b.SalePlan
            //WHERE a.SalePlanPlot =
            //(select ID from SPL_SalePLanPlot where StartDate = '2023-11-01')
            //AND DemandType!= -1
            //AND c.PeriodStartDate = '2023-11-01'
            //AND c.Cancel_Canceled = 0

            //1、有来源单据的不需要自动生成需求分类
            //2、单据类型SO7的不需要自动生成需求分类

            if (so.DocumentType.Code == "SO7")
            {
                return;
            }



            if (so.DocumentType.IsInSalePlan == true && so.DocumentType.IsInSaleAchievement == true)
            {
                string DemandType = "";
                BusinessDate = so.BusinessDate;
                FirstDayOfMonth = new DateTime(BusinessDate.Year, BusinessDate.Month, 1); // 获取当前月份的第一天
                DataTable dataTable = new DataTable();
                DataSet dataSet = new DataSet();
                string sql = "select DemandType from SPL_SalePlanLine a" +
                    " INNER JOIN SPL_SalePLanPlot b ON a.SalePlanPlot = b.ID " +
                    " INNER JOIN SPL_SalePlan c ON c.ID = b.SalePlan " +
                    " WHERE a.SalePlanPlot in(select ID from SPL_SalePLanPlot where StartDate = '" + FirstDayOfMonth + "')" +
                    " AND DemandType!= -1 AND c.PeriodStartDate = '" + FirstDayOfMonth + "' AND c.Cancel_Canceled = 0";
                DataAccessor.RunSQL(DataAccessor.GetConn(), sql, null, out dataSet);
                dataTable = dataSet.Tables[0];
                if (dataTable.Rows != null && dataTable.Rows.Count > 0)
                {
                    DemandType = dataTable.Rows[0]["DemandType"].ToString();
                }
                if (!string.IsNullOrEmpty(DemandType))
                {
                    foreach (var item in so.SOLines)
                    {
                        foreach (var i in item.SOShiplines)
                        {

                            i.DemandType = UFIDA.U9.CBO.Enums.DemandCodeEnum.GetFromValue(DemandType);
                        }
                    }
                }

            }
            else if (so.DocumentType.IsInSalePlan == false)//如果没有勾选计入计划就执行这个代码，如果有就执行上面的代码
            {
                foreach (var item in so.SOLines)
                {
                    LineNo = item.DocLineNo;
                    TDocNo = DocNo + "_" + LineNo;
                    des3 = item.DescFlexField.PrivateDescSeg3;
                    foreach (var i in item.SOShiplines)
                    {
                        ExtEnumValue extValue = ExtEnumValue.Finder.Find("ExtEnumType.Code=@TypeCode and Code=@Code",
                            new OqlParam("TypeCode", "UFIDA.U9.CBO.Enums.DemandCodeEnum"), new OqlParam("Code", TDocNo));
                        if (extValue != null)
                        {
                            i.DemandType = UFIDA.U9.CBO.Enums.DemandCodeEnum.GetFromValue(extValue.EValue);
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
                                i.DemandType = UFIDA.U9.CBO.Enums.DemandCodeEnum.GetFromValue(newEnumVValue);
                            }
                        }
                    }
                }

            }

        }
    }
}
