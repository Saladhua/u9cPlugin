
using System;
using System.Data;
using UFIDA.U9.Base;
using UFIDA.U9.Base.FlexField.ValueSet;
using UFIDA.U9.CBO.SCM.Item;
using UFIDA.U9.PM.PO;
using UFSoft.UBF.Business;
using UFSoft.UBF.Util.DataAccess;
using UFSoft.UBF.Util.Log;

namespace YY.U9.Cust.LI.AppPlugIn
{
    /// <summary>
    /// 恺之，对应供应商信息准入，供应商采购订单料品筛选规则
    /// 供应商的值集要和料品的值集一一对应
    /// </summary>
    [UFSoft.UBF.Eventing.Configuration.Failfast]
    class PMPOLineInsertedSubscriber : UFSoft.UBF.Eventing.IEventSubscriber
    {
        /// <summary>
        /// 形态转换子行 更新
        /// </summary>
        private static readonly ILogger logger = LoggerManager.GetLogger(typeof(PMPOLineInsertedSubscriber));
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
            PurchaseOrder purchase = key.GetEntity() as PurchaseOrder;
            if (purchase == null)
            {
                return;
            }
            #endregion
            //先找到供应商所对应的值集
            string supplierCode = purchase.Supplier.Code.Substring(0, 2);
            string vSetDef = "";
            string itemCategoryCode = "";
            string supcategoryName = purchase.Supplier.Name.ToString();
            string supcategoryCode = purchase.Supplier.Code.ToString();
            string df29 = "";

            #region 先判断是不是新的
            DataTable table_2 = new DataTable();

            string sql_2 = "select b.Effective_EffectiveDate,a.Name,b.Code from CBO_Supplier_Trl" +
               "  a inner join  CBO_Supplier b on a.ID=b.ID where b.Code='" + supcategoryCode + "' and Org='" + Context.LoginOrg.ID + "'";//本地测试环境
            DataSet dataSet_2 = new DataSet();
            DataAccessor.RunSQL(DataAccessor.GetConn(), sql_2, null, out dataSet_2);
            table_2 = dataSet_2.Tables[0];

            #region 
            DataSet dataSet_11 = new DataSet();
            DataSet dataSet_12 = new DataSet();
            DataTable table_11 = new DataTable();
            DataTable table_12 = new DataTable();
            #endregion


            #region 对应的值集 Z010
            DataTable table_1 = new DataTable();
            // A.[ValueSetDef]该值需要随着环境的变化而变化[1002304110330338]正式环境1002304120110012

            //1002304110015795--恺之正式

            string sql_1 = "select  A.[ID], A.[Code], A1.[Name], A.[DependantCode] from  Base_DefineValue as A  left join [Base_DefineValue_Trl] as A1 on (A1.SysMlFlag = 'zh-CN') and (A.[ID] = A1.[ID]) where  (A.[ValueSetDef] = 1002304110015795)" +
                "AND A.Code = '" + supplierCode + "'";
            DataSet dataSet_1 = new DataSet();
            DataAccessor.RunSQL(DataAccessor.GetConn(), sql_1, null, out dataSet_1);
            table_1 = dataSet_1.Tables[0];
            if (table_1.Rows != null && table_1.Rows.Count > 0)
            {
                for (int i = 0; i < table_1.Rows.Count; i++)
                {
                    vSetDef = vSetDef + table_1.Rows[i]["Name"].ToString() + ",";
                }
                foreach (var item in purchase.POLines)
                {
                    ItemMaster itemMaster = ItemMaster.Finder.FindByID(item.ItemInfo.ItemID.ID);
                    itemCategoryCode = itemMaster.MainItemCategory.Code.Substring(0, 2);
                    if (!vSetDef.Contains(itemCategoryCode))
                    {
                        throw new Exception("该供应商下料品【" + itemMaster.Code + "】不能采购此料品,与值集Z010不匹配");
                    }
                }
            }
            #endregion



            if (table_2.Rows != null && table_2.Rows.Count > 0)
            {
                df29 = table_2.Rows[0]["Effective_EffectiveDate"].ToString();
                DateTime date = DateTime.Parse("2023.4.19");

                string sqlFor1 = "select  A1.[Name],Description from Base_DefineValue as A  left join[Base_DefineValue_Trl] as A1 on(A1.SysMlFlag = 'zh-CN') " +
                    " and(A.[ID] = A1.[ID]) where(A.[ValueSetDef] = 1002302090000285) and A.Code = '" + supcategoryCode.Substring(0, 2) + "'";
                DataAccessor.RunSQL(DataAccessor.GetConn(), sqlFor1, null, out dataSet_12);
                table_12 = dataSet_12.Tables[0];
                if (DateTime.Parse(df29) < date )
                {
                    return;
                }
            }
            #endregion

            #region 对应的值集 Z009
            DataTable dataTable = new DataTable();
            //select  A.[ID], A.[Code], A1.[Name], A.[DependantCode] from  Base_DefineValue as A  left join [Base_DefineValue_Trl] as A1 on (A1.SysMlFlag = 'zh-CN') and (A.[ID] = A1.[ID]) where  (A.[ValueSetDef] = 1002302090000285) //测试环境的集值
            //select  A.[ID], A.[Code], A1.[Name], A.[DependantCode] from  Base_DefineValue as A  left join [Base_DefineValue_Trl] as A1 on (A1.SysMlFlag = 'zh-CN') and (A.[ID] = A1.[ID]) where  (A.[ValueSetDef] = 1002302090000285) //正式环境的集值
            //select  A1.[Name],Description from             Base_DefineValue as A  left join[Base_DefineValue_Trl] as A1 on(A1.SysMlFlag = 'zh-CN') and(A.[ID] = A1.[ID]) where(A.[ValueSetDef] = 1002304110770010) and Name = '原材料' //本地测试环境的集值
            string sqlFor = "select  A1.[Name],Description from Base_DefineValue as A  left join[Base_DefineValue_Trl] as A1 on(A1.SysMlFlag = 'zh-CN') " +
                " and(A.[ID] = A1.[ID]) where(A.[ValueSetDef] = 1002302090000285) and A.Code = '" + supcategoryCode.Substring(0, 2) + "'";//本地测试环境
            //string sqlFor = "select  A.[ID], A.[Code], A1.[Name], A.[DependantCode] from  Base_DefineValue as A  left join [Base_DefineValue_Trl] " +
            //    "as A1 on (A1.SysMlFlag = 'zh-CN') and (A.[ID] = A1.[ID]) where  (A.[ValueSetDef] = 1002302090000285)  and Name = '" + supcategoryName + "'";//测试环境
            //string sqlFor = "select  A.[ID], A.[Code], A1.[Name], A.[DependantCode] from  Base_DefineValue as A  left join [Base_DefineValue_Trl] " +
            //    "as A1 on (A1.SysMlFlag = 'zh-CN') and (A.[ID] = A1.[ID]) where  (A.[ValueSetDef] = 1002302090000285)  and Name = '" + supcategoryName + "'";//正式环境 修改 ValueSetDef
            DataSet dataSet = new DataSet();
            DataAccessor.RunSQL(DataAccessor.GetConn(), sqlFor, null, out dataSet);
            dataTable = dataSet.Tables[0];
            string typedocDescription = "";
            if (dataTable != null && dataTable.Rows.Count > 0)
            {
                typedocDescription = dataTable.Rows[0]["Description"].ToString();
                if (typedocDescription == "1")//去供应商单据里面根据id+ 组织 找到该单据的私有字段30不为是就报错，报错内容为供应商的值集描述为1需要走需要走供应商评审流程
                {
                    string codeno = "";//本地测试环境
                    dataSet = new DataSet();
                    string sql = "select b.DescFlexField_PrivateDescSeg30,a.Name,b.Code from CBO_Supplier_Trl" +
                       "  a inner join  CBO_Supplier b on a.ID=b.ID where b.Code='" + supcategoryCode + "' and Org='" + Context.LoginOrg.ID + "' and b.DescFlexField_PrivateDescSeg30='是' ";//本地测试环境
                    DataAccessor.RunSQL(DataAccessor.GetConn(), sql, null, out dataSet);
                    dataTable = new DataTable();
                    dataTable = dataSet.Tables[0];
                    if (dataTable != null && dataTable.Rows.Count > 0)
                    {
                        codeno = dataTable.Rows[0]["DescFlexField_PrivateDescSeg30"].ToString();
                    }
                    else
                    {
                        throw new Exception("供应类别的值集描述为1需要走需要走供应商评审流程");
                    }
                }
            }

            #endregion
        }
    }
}
