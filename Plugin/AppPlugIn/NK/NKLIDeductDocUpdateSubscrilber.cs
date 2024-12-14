using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UFIDA.U9.CBO.Pub.Controller;
using UFIDA.U9.CBO.SCM.Supplier;
using UFIDA.U9.ISV.AP;
using UFIDA.U9.ISV.AP.Proxy;
using UFIDA.U9.UI.PDHelper;
using UFIDA.U9C.Cust.Nrknor.DeductDocBE;
using UFSoft.UBF.Business;
using UFSoft.UBF.Util.DataAccess;
using UFSoft.UBF.Util.Log;

namespace YY.U9.Cust.LI.AppPlugIn
{
    /// <summary>
    /// 审核创建应付单
    /// </summary>
    [UFSoft.UBF.Eventing.Configuration.Failfast]
    class NKLIDeductDocUpdateSubscrilber : UFSoft.UBF.Eventing.IEventSubscriber
    {
        private static readonly UFSoft.UBF.Util.Log.ILogger logger = LoggerManager.GetLogger(typeof(NKLIPurchaseOrderInsertedSubscrilber));

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

            DeductDoc deductDoc = key.GetEntity() as DeductDoc;
            if (deductDoc == null)
            {
                return;
            }
            #endregion


            //判断是审核的审核
            if (deductDoc.State.Value == 2)
            {
                long id = deductDoc.ID;
                CreateDoc(id);
            }


        }
        private void CreateDoc(long DocID)
        {
            CreateImportAPBillSVProxy sv = new CreateImportAPBillSVProxy();
            List<ImportAPBillHeadDTOData> heads = new List<ImportAPBillHeadDTOData>();
            DeductDoc record = DeductDoc.Finder.FindByID(DocID);
            if (record == null) return;
            foreach (var item in record.DeductDocLine)
            {
                //本期实际扣款大于0才发生扣款
                if (item.RealityAmount > 0)
                {
                    ImportAPBillHeadDTOData head = SetAPData(item, record.DocNo);
                    heads.Add(head);
                }
            } 
            //foreach (record.DeductDocLine item in this.Model.DeductDoc_DeductDocLine.Records)
            //{
            //    //本期实际扣款大于0才发生扣款
            //    if (item.RealityAmount > 0)
            //    {
            //        ImportAPBillHeadDTOData head = SetAPData(item, record.DocNo);
            //        heads.Add(head);
            //    }
            //}
            sv.ImportAPBillHeadDTOs = heads;
            List<ImportAPBillResultDTOData> listap = sv.Do();
        }

        private ImportAPBillHeadDTOData SetAPData(DeductDocLine item, string docNo)
        {
            ImportAPBillHeadDTOData head = new ImportAPBillHeadDTOData();
            if (item == null) return head;
            DataTable dt = GetSupplier(item.Supplier.ID);
            if (dt == null || dt.Rows.Count == 0)
            {
                throw new Exception(string.Format("行号{0}根据立账供应商编号，未找到有效供应商", item.DocLineNo));
            }
            head.AC = new CommonArchiveDataDTOData();
            DataTable acData = GetFC();
            if (acData != null && acData.Rows.Count > 0)
            {
                head.AC.ID = long.Parse(acData.Rows[0][0].ToString());
                head.AC.Code = acData.Rows[0][1].ToString();
                head.AC.Name = acData.Rows[0][2].ToString();
            }
            head.AccrueSupp = null;
            head.AccrueSupp = new SupplierMISCInfoData();
            head.AccrueSupp.Code = dt.Rows[0][0].ToString();
            head.AccrueSupp.Name = dt.Rows[0][1].ToString();
            head.AccrueSuppSite = new SupplierSiteMISCInfoData();
            head.BizOrg = new CommonArchiveDataDTOData();
            head.DocumentType = new CommonArchiveDataDTOData();
            DataTable docData = GetDocType();
            if (docData != null && docData.Rows.Count > 0)
            {
                head.DocumentType.ID = long.Parse(docData.Rows[0][0].ToString());
                head.DocumentType.Code = docData.Rows[0][1].ToString();
                head.DocumentType.Name = docData.Rows[0][2].ToString();
            }
            head.PaySupp = new SupplierMISCInfoData();
            head.PaySupp.Code = dt.Rows[0][0].ToString();
            head.PaySupp.Name = dt.Rows[0][1].ToString();
            head.PaySuppSite = new SupplierSiteMISCInfoData();
            head.SrcOrg = new CommonArchiveDataDTOData();
            head.BusinessType = 190;

            head.ImportAPBillLineDTOs = new List<ImportAPBillLineDTOData>();
            ImportAPBillLineDTOData lineDTOData = new ImportAPBillLineDTOData();
            lineDTOData.Money = item.RealityAmount;
            lineDTOData.Dept = new CommonArchiveDataDTOData();
            lineDTOData.Dept.ID = item.Deparment.ID;
            head.ImportAPBillLineDTOs.Add(lineDTOData);
            head.Memo = "扣款单号：" + docNo;
            return head;
        }


        private DataTable GetDataTable(string sql)
        {
            DataTable dt = new DataTable();
            System.Data.DataSet returnValue = null;
            DataAccessor.RunSQL(DataAccessor.GetConn(), sql, null, out returnValue);
            if (returnValue != null)
            {
                dt = returnValue.Tables[0];
            }
            return dt;
        }

        private DataTable GetSupplier(long id)
        {
            DataTable dt = new DataTable();
            string sql = string.Format("select A.Code,B.Name from CBO_Supplier A left join CBO_Supplier_Trl B on A.ID=B.ID where B.SysMLFlag='{0}' and A.ID={1}", "zh-CN", id);
            dt = GetDataTable(sql);
            return dt;
        }
        private DataTable GetFC()
        {
            DataTable dt = new DataTable();
            string sql = string.Format(" select A.ID,A.Code,B.Name from Base_Currency A left join Base_Currency_Trl B on A.ID=B.ID where A.Code='{0}' and B.SysMLFlag='{1}'", "C001", "zh-CN");
            dt = GetDataTable(sql);
            return dt;
        }

        private DataTable GetDocType()
        {
            DataTable dt = new DataTable();
            string sql = string.Format("select A.ID,A.Code,B.Name from AP_APDocType A left join AP_APDocType_Trl B on A.ID=B.ID where A.Code='{0}' and B.SysMLFlag='{1}'", "17", "zh-CN");
            dt = GetDataTable(sql);
            return dt;
        }
    }
}
