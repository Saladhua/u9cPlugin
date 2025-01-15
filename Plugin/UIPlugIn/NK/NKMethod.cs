using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UFSoft.UBF.Util.DataAccess;

namespace YY.U9.Cust.LI.UIPlugIn
{
    /// <summary>
    /// 纳科ui方法合集
    /// </summary>
    class NKMethod
    {
        public static string GetDataTable(string id)
        {
            #region 用法
            string ZCode = "";
            string sql = string.Format("select A3.code,A4.Name  from Base_ValueSetDef A1" +
             " left join Base_ValueSetDef_Trl A2 on A1.ID = A2.ID" +
             " left join Base_DefineValue A3 on A3.ValueSetDef = A1.ID " +
             " left join Base_DefineValue_Trl A4 on A3.ID = A4.ID " +
             " where A1.code = 'Z21' and A3.ID = '" + id + "'");
            //return dt;
            #endregion
            DataTable dt = new DataTable();
            DataSet dataSet = new DataSet(); 
            DataAccessor.RunSQL(DataAccessor.GetConn(), sql, null, out dataSet);
            dt = dataSet.Tables[0];
            if (dt != null && dt.Rows.Count > 0)
            {
                ZCode = dt.Rows[0]["Code"].ToString();//单号需要处理
            }

            return ZCode;
        }
    }
}
