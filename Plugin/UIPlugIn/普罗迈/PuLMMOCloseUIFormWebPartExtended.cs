using System;
using System.Collections.Generic;
using System.Data;
using System.Web;
using UFIDA.U9.ISV.MO;
using UFIDA.U9.ISV.MO.Proxy;
using UFIDA.U9.MFG.MO.DiscreteMOUIModel;
using UFIDA.U9.UI.PDHelper;
using UFSoft.UBF.UI.ControlModel;
using UFSoft.UBF.UI.Custom;
using UFSoft.UBF.UI.IView;
using UFSoft.UBF.UI.WebControlAdapter;
using UFSoft.UBF.Util.DataAccess;

namespace YY.U9.Cust.LI.UIPlugIn
{
    /// <summary>
    /// 普偌迈
    /// 生产订单手工关闭时有校验提示。
    /// 当生产订单入库数量(包含报废入库)小于生产备料表中物料领料数量
    /// (只考虑每装配件数为1且发料方式为推式的物料)，
    /// 弹出提示框（报错内容:入库数量小于领料数量，是否强制关闭订单），
    /// 点是后，生产订单私有段2填入内容强制关闭。
    /// </summary>
    class PuLMMOCloseUIFormWebPartExtended : ExtendedPartBase
    {
        private MOCloseUIFormWebPart _part;


        //public override void AfterInit(IPart part, System.EventArgs e)
        //{
        //    base.AfterInit(part, e);

        //    _part = part as MOCloseUIFormWebPart;


        //    string MONo = this._part.Model.MO.FocusedRecord["DocNo"].ToString();




        //    string IssuedQty = "0";//已发放数量


        //    DataTable dataTable = new DataTable();
        //    DataSet dataSet = new DataSet();
        //    string sqlForCy = "select sum(IssuedQty) AS IssuedQty from MO_MOPickList where QPA = 1 and IssueStyle = 0" +
        //        " and MO = (select ID from MO_MO where DocNo = '" + MONo + "')";
        //    DataAccessor.RunSQL(DataAccessor.GetConn(), sqlForCy, null, out dataSet);
        //    dataTable = dataSet.Tables[0];
        //    if (dataTable != null && dataTable.Rows.Count > 0)
        //    {
        //        IssuedQty = dataTable.Rows[0]["IssuedQty"].ToString() == "" ? "0" : dataTable.Rows[0]["IssuedQty"].ToString();
        //    }


        //    string TotalRcvQty = this._part.Model.MO.FocusedRecord["TotalRcvQty"].ToString();

        //    string TotalScrapQty = this._part.Model.MO.FocusedRecord["TotalScrapQty"].ToString();


        //    decimal sum = decimal.Parse(TotalRcvQty) + decimal.Parse(TotalScrapQty);

        //    //if (sum < decimal.Parse(IssuedQty))
        //    //{
        //    //this._part.Model.ErrorMessage.Message = "入库数量小于领料数量，是否强制关闭订单";

        //    //throw new Exception("入库数量小于领料数量，是否强制关闭订单");



        //    // string sqlForUpDate = "UPDATE MO_MO  SET DescFlexField_PrivateDescSeg2='强制关闭' WHERE DocNo='" + MONo + "'";
        //    // DataAccessor.RunSQL(DataAccessor.GetConn(), sqlForUpDate, null, out dataSet);

        //    // }

        //}



        public override void AfterDataLoad(IPart part)
        {

            _part = part as MOCloseUIFormWebPart;




            string MONo = this._part.Model.MO.FocusedRecord["DocNo"].ToString();




            string IssuedQty = "0";//已发放数量


            DataTable dataTable = new DataTable();
            DataSet dataSet = new DataSet();
            string sqlForCy = "select sum(IssuedQty) AS IssuedQty from MO_MOPickList where QPA = 1 and IssueStyle = 0" +
                " and MO = (select ID from MO_MO where DocNo = '" + MONo + "')";
            DataAccessor.RunSQL(DataAccessor.GetConn(), sqlForCy, null, out dataSet);
            dataTable = dataSet.Tables[0];
            if (dataTable != null && dataTable.Rows.Count > 0)
            {
                IssuedQty = dataTable.Rows[0]["IssuedQty"].ToString() == "" ? "0" : dataTable.Rows[0]["IssuedQty"].ToString();
            }


            string TotalRcvQty = this._part.Model.MO.FocusedRecord["TotalRcvQty"].ToString();

            string TotalScrapQty = this._part.Model.MO.FocusedRecord["TotalScrapQty"].ToString();


            decimal sum = decimal.Parse(TotalRcvQty) + decimal.Parse(TotalScrapQty);

            if (sum < decimal.Parse(IssuedQty))
            {
                this._part.Model.ErrorMessage.Message = "入库数量小于领料数量，是否强制关闭订单";
                string sqlForUpDate = "UPDATE MO_MO  SET DescFlexField_PrivateDescSeg2='强制关闭' WHERE DocNo='" + MONo + "'";
                DataAccessor.RunSQL(DataAccessor.GetConn(), sqlForUpDate, null, out dataSet);
            }

            base.AfterDataLoad(part);
        }


    }
}
