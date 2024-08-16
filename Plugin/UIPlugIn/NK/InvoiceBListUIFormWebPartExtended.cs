using System;
using System.Collections.Generic;
using System.Data;
using System.Web;
using System.Web.UI.WebControls;
using UFIDA.U9.FI.AR.ReceivalUI.ReceivalUIModel;
using UFIDA.U9.FI.TI.InvoiceUI.InvoiceBListUIModel;
using UFIDA.U9.UI.PDHelper;
using UFSoft.UBF.UI.ControlModel;
using UFSoft.UBF.UI.Custom;
using UFSoft.UBF.UI.Engine.Builder;
using UFSoft.UBF.UI.IView;
using UFSoft.UBF.UI.MD.Runtime;
using UFSoft.UBF.UI.WebControlAdapter;
using UFSoft.UBF.Util.DataAccess;

namespace YY.U9.Cust.LI.UIPlugIn
{
    /// <summary>
    /// 在开蓝字专用发票、开蓝字普通发票列表，同软硬件收入按钮新增一个按钮“硬件收入计算”
    /// 开发文档位置  D:\setups\porject\纳科\2024-07 -纳科补充开发事项711(2)
    /// </summary>
    class InvoiceBListUIFormWebPartExtended : ExtendedPartBase
    {
        private InvoiceBListUIFormWebPart _part;

        private IUFButton BtnCalcMoney2;

        public override void AfterInit(IPart part, System.EventArgs e)
        {
            base.AfterInit(part, e);
            this._part = (part as InvoiceBListUIFormWebPart);
            IUFToolbar iuftoolbar = (IUFToolbar)this._part.GetUFControlByName(part.TopLevelContainer, "Toolbar1");
            bool flag = iuftoolbar != null;
            if (flag)
            {
                string text = "9884A728-4551-4966-BBC7-82E247603C9D";
                this.BtnCalcMoney2 = UIControlBuilder.BuilderToolbarButton(iuftoolbar, "True", "BtnCalcMoney", "True", "True", 70, 28, "8", "", true, false, text, text, text);
                UIControlBuilder.SetButtonAccessKey(this.BtnCalcMoney2);
                this.BtnCalcMoney2.Text = "硬件收入计算";
                this.BtnCalcMoney2.ID = "BtnCalcMoney2";
                this.BtnCalcMoney2.AutoPostBack = true;
                this.BtnCalcMoney2.UIModel = this._part.Model.ElementID;
                ((UFWebToolbarAdapter)iuftoolbar).Items.Add(this.BtnCalcMoney2 as WebControl);
                this.BtnCalcMoney2.Click += this.BtnCalcMoney_Click2;
            }
        }

        public void BtnCalcMoney_Click2(object sender, EventArgs e)
        {
            bool hasErrorMessage = this._part.Model.ErrorMessage.hasErrorMessage;
            if (hasErrorMessage)
            {
                this._part.Model.ClearErrorMessage();
            }
            this._part.OnDataCollect(this);
            try
            {
                IList<IUIRecord> selectRecord = this._part.Model.SaleInvoiceHead.Cache.GetSelectRecord();
                bool flag = selectRecord == null || selectRecord.Count == 0;
                if (flag)
                {
                    throw new Exception("未选择任何税务发票行！");
                }
                List<string> list = new List<string>();
                //行私有段1软件收入=0
                //行私有段3软件税额 = 0
                //行私有段2硬件收入 = 优先找对应税控行实际开票未税额，如果为空找行未税金额
                //行私有段4硬件税金=优先找对应税控行实际开票税额，如果为空找行税额
                foreach (IUIRecord iuirecord in selectRecord)
                {
                    iuirecord["SaleInvoiceLine_DescFlexField_PrivateDescSeg1"] = "0";

                    string d1 = iuirecord["SaleInvoiceLine_DescFlexField_PrivateDescSeg1"].ToString();

                    iuirecord["SaleInvoiceLine_DescFlexField_PrivateDescSeg3"] = "0";

                    string d3 = iuirecord["SaleInvoiceLine_DescFlexField_PrivateDescSeg3"].ToString();

                    string Code = iuirecord["SaleInvoiceLine_Code"].ToString();

                    string DocNo = iuirecord["DocNo"].ToString();

                    string NotTaxed = iuirecord["SaleInvoiceLine_SaleTaxInvoiceLine_ActualInvoiceAmountNotTaxed"].ToString();//销售发票行数据.销售发票税控行.实际开票未税金额

                    string Amount = iuirecord["SaleInvoiceLine_SaleTaxInvoiceLine_ActualInvoiceAmount"].ToString();//销售发票行数据.销售发票税控行.实际开票税额

                    string d2 = "0";

                    string d4 = "0";

                    if (NotTaxed == "0.000000000")
                    {
                        iuirecord["SaleInvoiceLine_DescFlexField_PrivateDescSeg2"] = iuirecord["SaleInvoiceLine_FunctionalCurrencyNonTax"];
                        d2 = iuirecord["SaleInvoiceLine_DescFlexField_PrivateDescSeg2"].ToString();
                    }
                    else
                    {
                        iuirecord["SaleInvoiceLine_DescFlexField_PrivateDescSeg2"] = NotTaxed;
                        d2 = iuirecord["SaleInvoiceLine_DescFlexField_PrivateDescSeg2"].ToString();

                    }
                    if (Amount == "0.000000000")
                    {
                        iuirecord["SaleInvoiceLine_DescFlexField_PrivateDescSeg4"] = iuirecord["SaleInvoiceLine_FunctionalCurrencyNonTax"];
                        d4 = iuirecord["SaleInvoiceLine_DescFlexField_PrivateDescSeg4"].ToString();

                    }
                    else
                    {
                        iuirecord["SaleInvoiceLine_DescFlexField_PrivateDescSeg4"] = Amount;
                        d4 = iuirecord["SaleInvoiceLine_DescFlexField_PrivateDescSeg4"].ToString();

                    }
                    DataTable dataTable = new DataTable();
                    DataSet dataSet = new DataSet();
                    string sql_1 = "update  A2  set " +
                         " A2.DescFlexField_PrivateDescSeg4 = '" + d4 + "'," +
                         " A2.DescFlexField_PrivateDescSeg3 = '" + d3 + "'," +
                         " A2.DescFlexField_PrivateDescSeg2 = '" + d2 + "'," +
                         " A2.DescFlexField_PrivateDescSeg1 = '" + d1 + "'" +
                         " from TI_SaleInvoiceLine A2" +
                         " left join TI_SaleInvoiceHead A1" +
                         " on A1.ID = A2.SaleInvoiceHead" +
                         " where A1.DocNo = '" + DocNo + "' and A2.Code = '" + Code + "'";
                    DataAccessor.RunSQL(DataAccessor.GetConn(), sql_1, null, out dataSet);
                }

            }
            catch (Exception ex)
            {
                IUIModel model = this._part.Model;
                this._part.Model.ErrorMessage.SetErrorMessage(ref model, ex);
            }
        }
    }
}
