using ItemUIModel;
using System;
using System.Collections.Generic;
using System.Data;
using UFIDA.U9.CBO.MFG.BOM.BOMUIModel;
using UFIDA.U9.FI.AP.PayReqFundUIModel;
using UFSoft.UBF.UI.ControlModel;
using UFSoft.UBF.UI.Custom;
using UFSoft.UBF.UI.IView;
using UFSoft.UBF.UI.MD.Runtime;
using UFSoft.UBF.Util.DataAccess;

namespace YY.U9.Cust.LI.UIPlugIn
{
    class BomMainUIFormWebPartExtended : UFSoft.UBF.UI.Custom.ExtendedPartBase
    {
        private BomMainUIFormWebPart _part;

        IUIRecordBuilder _builder;

        IUFMenu BtnSettle;
        /// <summary>
        /// 初始化后扩展
        /// UFIDA.U9.BS.Job.RequestClient.dll
        /// </summary>
        /// <param name="part"></param>
        /// <param name="e"></param>        
        public override void AfterInit(IPart part, System.EventArgs e)
        {
            base.AfterInit(part, e);
            this._part = (part as BomMainUIFormWebPart);




        }



        public override void BeforeRender(IPart Part, EventArgs args)
        {
            try
            {
                long ID = this._part.Model.BOMMaster.FocusedRecord.ID;
                DataTable dataTableWh = new DataTable();
                string sqlWh = "select ID from CBO_BOMMaster where ID= '" + ID + "'";
                DataSet dataSetWh = new DataSet();
                DataAccessor.RunSQL(DataAccessor.GetConn(), sqlWh, null, out dataSetWh);
                dataTableWh = dataSetWh.Tables[0];
                if (dataTableWh == null || dataTableWh.Rows.Count == 0)
                {
                    this._part.Model.BOMMaster_BOMComponents.FocusedRecord.IsSpecialUseItem = true;
                    if (this._part.Model.BOMMaster_BOMComponents.FocusedRecord.IssueStyle == 0 || this._part.Model.BOMMaster_BOMComponents.FocusedRecord.IssueStyle == 3)
                    {
                        this._part.Model.BOMMaster_BOMComponents.FocusedRecord.SetChkAtComplete = true;
                    }
                }
            }
            catch (Exception)
            {

                return;
            }


        }


    }
}