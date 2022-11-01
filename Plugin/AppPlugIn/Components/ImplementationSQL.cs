using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UFSoft.UBF.Util.DataAccess;

namespace YY.U9.Cust.LI.AppPlugIn
{
    public class ImplementationSQL
    {
        /// <summary>
        /// 执行SQL语句-单个的赋值应该没有问题
        /// </summary>
        /// <param name="sql">sql语句</param>//不能 select * 
        /// <param name="items"></param>//条件，数据库的列
        /// <returns></returns>
        /// 这边的返回值应该使用list，方便后续使用linq语句.foreach
        public static List<string> ImplSqlRun(string sql, string items)
        {
            //sql = "SELECT Lot_LotMaster.DocNo,ConfirmDate,(SELECT  Name  FROM CBO_Supplier_Trl WHERE ID = Supplier_Supplier AND SysMLFlag = 'zh-CN') AS GYS," +
            //    " (SELECT  Code  FROM CBO_Supplier WHERE ID = Supplier_Supplier) AS GYSCode," +
            //    " InvLotEnableDate, Supplier_Supplier  FROM Lot_LotMaster INNER JOIN PM_Receivement ON Lot_LotMaster.DocNo = PM_Receivement.DocNo" +
            //    " LEFT JOIN PM_RcvLine ON PM_Receivement.ID = PM_RcvLine.Receivement WHERE Lot_LotMaster.DocNo = 'RCV102204080025'";
            List<string> list = new List<string>();
            string[] s2 = items.Split(new char[1] { ',' });
            DataTable dataTable = new DataTable();
            DataSet dataSet = new DataSet();
            DataAccessor.RunSQL(DataAccessor.GetConn(), sql, null, out dataSet);
            dataTable = dataSet.Tables[0];
            if (dataTable.Rows != null && dataTable.Rows.Count > 0)
            {
                foreach (var item in s2)
                {
                    list.Add(dataTable.Rows[0][item].ToString());
                }
            }
            return list;
        } 

        //#region 测试代码
        //多值赋值应该使用linq语句就结束了
        //string testsql = "";
        //string testitem = "ConfirmDate,InvLotEnableDate,GYS,GYSCode,Supplier_Supplier";
        //string returnVtem = "";
        //returnVtem = CustRunSql.CustSqlRun(testsql, testitem);
        //    string confirmDate_TTT = "";
        //string scdate_TTT = "";
        //string gysname_TTT = "";
        //string gyscode_TTT = "";
        //string gysID_TTT = "";

        //string[] s = returnVtem.Split(new char[1] { ',' });
        //#endregion

    }
}
