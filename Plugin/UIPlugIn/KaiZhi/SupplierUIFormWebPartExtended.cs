using System;
using System.Web.UI;
using UFIDA.U9.CBO.Supplier.SupplierUIModel;
using UFSoft.UBF.UI.ControlModel;
using UFSoft.UBF.UI.IView;

namespace YY.U9.Cust.LI.UIPlugIn
{
    /// <summary>
    /// 恺之供应商权限控制
    /// </summary>
    class SupplierUIFormWebPartExtended : UFSoft.UBF.UI.Custom.ExtendedPartBase
    {
        private SupplierMainUIFormWebPart _part;


        IUFMenu BtnSettle;




        /// <summary>
        /// 初始化后扩展
        /// UFIDA.U9.BS.Job.RequestClient.dll
        /// </summary>
        /// <param name="part"></param>
        /// <param name="e"></param>        
        public override void BeforeEventProcess(IPart part, string eventName, object sender, EventArgs args, out bool executeDefault)
        {
            base.BeforeEventProcess(part, eventName, sender, args, out executeDefault);

            if (sender is IUFButton)
            {
                this._part = (part as SupplierMainUIFormWebPart);
                UFSoft.UBF.UI.WebControlAdapter.UFWebButton4ToolbarAdapter webButton = sender as UFSoft.UBF.UI.WebControlAdapter.UFWebButton4ToolbarAdapter;

                try
                {
                    string s = webButton.Action;
                }
                catch (Exception)
                {
                    return;
                }
                if (webButton.Action == "ApproveClick")//SubmitClick  ApproveClick
                {
                    foreach (var item in _part.Model.Supplier.Records)
                    {
                        try
                        {
                            item["ReceiptRule"].ToString();
                        }
                        catch (Exception)
                        {
                            throw new Exception("收货原则的值不能为空。");
                        }

                        try
                        {
                            item["APConfirmTerm"].ToString();
                        }
                        catch (Exception)
                        {
                            throw new Exception("立账条件的值不能为空。");
                        }
                    }
                }
            }
        }
    }
}
