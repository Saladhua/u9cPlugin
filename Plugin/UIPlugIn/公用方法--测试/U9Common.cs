using System.Data;
using UFSoft.UBF.Util.DataAccess;

namespace YY.U9.Cust.LI.UIPlugIn
{
    /// <summary>
    /// 公用方法--测试
    /// </summary>
    class U9Common
    {
        /// <summary>
        /// 执行sql
        /// 该方法适用于，求和，取单个值
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public static DataTable GetDataTable(string sql)
        {
            #region 用法
            //string sql = string.Format("select ID from U9CCustNrknor_DeductDoc where BeginDate>='{0}' and EndDate<='{1}' and DocVersion='{2}' and Org={3} and ID!={4}", item.BeginDate.ToString("yyyy-MM-dd"), item.EndDate.ToString("yyyy-MM-dd"), item.DocVersion, PDContext.Current.OrgID, item.ID);
            //DataTable dt = GetDataTable(sql);
            //return dt;
            #endregion
            DataTable dt = new DataTable();
            System.Data.DataSet returnValue = null;
            DataAccessor.RunSQL(DataAccessor.GetConn(), sql, null, out returnValue);
            if (returnValue != null)
            {
                dt = returnValue.Tables[0];
            }
            return dt;
        }
        public static bool UpDateSQL(string sql)
        {
            bool upda = false;
            System.Data.DataSet returnValue = null;
            DataAccessor.RunSQL(DataAccessor.GetConn(), sql, null, out returnValue);
            upda = true;
            return upda;
        }
    }
}
