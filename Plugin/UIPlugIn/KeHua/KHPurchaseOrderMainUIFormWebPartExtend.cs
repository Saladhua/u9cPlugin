using System;
using System.Data;
using UFIDA.U9.PM.PurchaseOrderUIModel;
using UFSoft.UBF.UI.ControlModel;
using UFSoft.UBF.UI.Custom;
using UFSoft.UBF.UI.IView;
using UFSoft.UBF.UI.MD.Runtime;
using UFSoft.UBF.UI.WebControlAdapter;
using UFSoft.UBF.UI.WebControls;

namespace YY.U9.Cust.LI.UIPlugIn
{
    /// <summary>
    /// 科华-标准采购-单据类型控制
    /// </summary>
    class KHPurchaseOrderMainUIFormWebPartExtend : ExtendedPartBase
    {
        private PurchaseOrderMainUIFormWebPart _part;

        /// <summary>
        /// 增加单据类型：PO24  标准订单-小零件
        ///开发1：PO24单据类型下，采购订单无法对任何价格字段做编辑修改
        ///开发2：除PO24单据类型，其余单据类型做小零件（料号左包含D0301）订单时报错：
        /// </summary>
        /// <param name="part"></param>   
        public override void AfterRender(IPart part, EventArgs args)
        {
            base.AfterRender(part, args);

            this._part = (part as PurchaseOrderMainUIFormWebPart);

            if (this._part.Model.PurchaseOrder.FocusedRecord["DocumentType_Code"] != null)
            {
                string DocTypeCode = this._part.Model.PurchaseOrder.FocusedRecord["DocumentType_Code"].ToString();

                if (DocTypeCode == "PO24")
                {
                    IUFDataGrid grid = (IUFDataGrid)_part.GetUFControlByName(_part.TopLevelContainer, "DataGrid4");
                    grid.Columns["FinallyPriceTC"].Enabled = false;//最终价
                    grid.Columns["TotalMnyTC"].Enabled = false;//价税合计
                    grid.Columns["NetMnyTC"].Enabled = false;//未税金额
                    grid.Columns["TotalTaxTC"].Enabled = false;//税额 
                    UFWebNumberControlAdapter grid2 = (UFWebNumberControlAdapter)_part.GetUFControlByName(_part.TopLevelContainer, "FinallyPriceTC1");
                    grid2.Enabled = false;//最终价
                    UFWebNumberControlAdapter grid3 = (UFWebNumberControlAdapter)_part.GetUFControlByName(_part.TopLevelContainer, "NetMnyTC0");
                    grid3.Enabled = false;//最终价
                    UFWebNumberControlAdapter grid4 = (UFWebNumberControlAdapter)_part.GetUFControlByName(_part.TopLevelContainer, "TotalMnyTC1");
                    grid4.Enabled = false;//最终价
                    UFWebReferenceAdapter grid5 = (UFWebReferenceAdapter)_part.GetUFControlByName(_part.TopLevelContainer, "TaxSchedule1");
                    grid5.Enabled = false;//最终价
                    #region 页面本来就是不能填的
                    //grid.Columns["NetMnyAC"].Enabled = true;//未税金额(核币)
                    //grid.Columns["TotalMnyAC"].Enabled = true;//价税合计(核币)
                    //grid.Columns["TotalTaxAC"].Enabled = true;//税额(核币)
                    //grid.Columns["PurchaseOrder_FC"].Enabled = true;//本币
                    //grid.Columns["NetMnyFC"].Enabled = true;//未税金额(本币)
                    //grid.Columns["TotalMnyFC"].Enabled = true;//价税合计(本币)
                    //grid.Columns["TotalTaxFC"].Enabled = true;//税额(本币)
                    #endregion
                }
                else
                {
                    foreach (var item in this._part.Model.PurchaseOrder_POLines.Records)
                    {
                        string itemCode = item["ItemInfo_ItemCode"].ToString();

                        int itemCodeLength = itemCode.Length;

                        if (itemCodeLength > 5)
                        {
                            if (itemCode.Substring(0, 5) == "D0301")
                            {
                                throw new Exception("此单据类型禁止购买小零件，请使用PO24  标准订单-小零件进行下单！");
                            }
                        }
                    }
                }
            }
        }

        public override void BeforeEventProcess(IPart part, string eventName, object sender, EventArgs args, out bool executeDefault)
        {
            base.BeforeEventProcess(part, eventName, sender, args, out executeDefault);

            UFSoft.UBF.UI.WebControlAdapter.UFWebButton4ToolbarAdapter webButton = sender as UFSoft.UBF.UI.WebControlAdapter.UFWebButton4ToolbarAdapter;


            this._part = (part as PurchaseOrderMainUIFormWebPart);

            if (webButton != null)
            {
                if (webButton.Action == "SaveClick")//保存时 
                {
                    if (this._part.Model.PurchaseOrder.FocusedRecord["DocumentType_Code"] != null)
                    {
                        string DocTypeCode = this._part.Model.PurchaseOrder.FocusedRecord["DocumentType_Code"].ToString();

                        if (DocTypeCode != "PO24")
                        {
                            foreach (var item in this._part.Model.PurchaseOrder_POLines.Records)
                            {
                                string itemCode = item["ItemInfo_ItemCode"].ToString();

                                int itemCodeLength = itemCode.Length;

                                if (itemCodeLength > 5)
                                {
                                    if (itemCode.Substring(0, 5) == "D0301")
                                    {
                                        throw new Exception("此单据类型禁止购买小零件，请使用PO24  标准订单-小零件进行下单！");
                                    }
                                }
                            }
                        }
                    }
                }

            }
        }



    }
}

