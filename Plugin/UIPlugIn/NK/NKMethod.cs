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
        public static string GetDataTable(string ID)
        {
            #region 用法
            string ZCode = "";
            string sql = string.Format("SELECT DescFlexField_PrivateDescSeg10 FROM  CBO_Project where ID='" + ID + "'");
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
