using System.Data;
using System.Text;
using UFIDA.U9.MO.MO;
using UFIDA.U9.SM.ForecastOrder;
using UFIDA.UBF.MD.Business;
using UFSoft.UBF.Business;
using UFSoft.UBF.PL;
using UFSoft.UBF.Util.DataAccess;
using UFSoft.UBF.Util.Log;

namespace YY.U9.Cust.LI.AppPlugIn
{
    /// <summary>
    /// 对应创联，开发文档需求分类
    /// </summary>
    [UFSoft.UBF.Eventing.Configuration.Failfast]
    class ForecastOrderInsertedSubscrilber : UFSoft.UBF.Eventing.IEventSubscriber
    {
        private static readonly UFSoft.UBF.Util.Log.ILogger logger = LoggerManager.GetLogger(typeof(ForecastOrderInsertedSubscrilber));
          

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
            ForecastOrder forecastOrder = key.GetEntity() as ForecastOrder;
            if (forecastOrder == null)
            {
                return;
            }
            #endregion 

            string DocNo = "";//单号

            string TDocNo = "";

            DocNo = forecastOrder.DocNo;

            int LineNo = 0;//行号

            long extEnumTypeID = 0L;

            string des1 = "";

            int maxEValue = -1;
            //枚举赋值

            //插入之后
            foreach (var item in forecastOrder.ForecastOrderLines)
            {
                LineNo = item.DocLineNo;
                TDocNo = DocNo + "_" + LineNo;
                des1 = item.DescFlexField.PrivateDescSeg1;
                ExtEnumValue extValue = ExtEnumValue.Finder.Find("ExtEnumType.Code=@TypeCode and Code=@Code",
                    new OqlParam("TypeCode", "UFIDA.U9.CBO.Enums.DemandCodeEnum"), new OqlParam("Code", TDocNo));
                if (extValue != null)
                {
                    item.DemandType = UFIDA.U9.CBO.Enums.DemandCodeEnum.GetFromValue(extValue.EValue);
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
                        item.DemandType = UFIDA.U9.CBO.Enums.DemandCodeEnum.GetFromValue(newEnumVValue);
                    }
                }
            }
        }
    }
}
