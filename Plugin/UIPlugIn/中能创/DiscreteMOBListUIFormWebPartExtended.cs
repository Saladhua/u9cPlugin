using System;
using System.Collections.Generic;
using System.Data;
using System.Web.UI.WebControls;
using UFIDA.U9.MFG.MO.DiscreteMOBListUIModel;
using UFIDA.U9.UI.PDHelper;
using UFSoft.UBF.UI.ControlModel;
using UFSoft.UBF.UI.Custom;
using UFSoft.UBF.UI.Engine.Builder;
using UFSoft.UBF.UI.IView;
using UFSoft.UBF.UI.MD.Runtime;
using UFSoft.UBF.UI.WebControlAdapter;

namespace YY.U9.Cust.LI.UIPlugIn
{
    /// <summary>
    /// 生产订单列表自动获取采购最新价
    /// 文档名称--中能创-生产订单列表自动获取采购最新价
    /// 位置porject\中能创\2024-09
    /// </summary>
    class DiscreteMOBListUIFormWebPartExtended : ExtendedPartBase
    {
        private DiscreteMOBListUIFormWebPart _part;

        private IUFButton BtnCalcMoney2;

        public override void AfterInit(IPart part, System.EventArgs e)
        {
            base.AfterInit(part, e);
            this._part = (part as DiscreteMOBListUIFormWebPart);
            IUFToolbar iuftoolbar = (IUFToolbar)this._part.GetUFControlByName(part.TopLevelContainer, "Toolbar1");
            bool flag = iuftoolbar != null;
            if (flag)
            {
                string text = "9884A728-4551-4966-BBC7-82E247600C3C";
                this.BtnCalcMoney2 = UIControlBuilder.BuilderToolbarButton(iuftoolbar, "True", "BtnCalcMoney", "True", "True", 70, 28, "8", "", true, false, text, text, text);
                UIControlBuilder.SetButtonAccessKey(this.BtnCalcMoney2);
                this.BtnCalcMoney2.Text = "最新价";
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
                IList<IUIRecord> selectRecord = this._part.Model.MO.Cache.GetSelectRecord();
                bool flag = selectRecord == null || selectRecord.Count == 0;
                if (flag)
                {
                    throw new Exception("未选择任何生产订单备料行！");
                }
                List<string> list = new List<string>();
                string orgID = PDContext.Current.OrgID;
                //最新价--对应生产订单备料私有字段4
                //金额--对应生产订单备料私有字段5
                //公式 = 最新价 * 已发放数量 
                foreach (IUIRecord iuirecord in selectRecord)
                {

                    string MoID = iuirecord["ID"].ToString();

                    string ItemCode = iuirecord["MOPickLists_ItemMaster_Code"].ToString();//料号

                    decimal IssuedQty = decimal.Parse(iuirecord["MOPickLists_IssuedQty"].ToString());//已发放数量

                    //string ItemGrade = iuirecord["MOPickLists_ItemMaster_StandardGrade"].ToString(); //等级

                    //string ItemPotency = iuirecord["MOPickLists_ItemMaster_StandardPotency"].ToString();//成分

                    //if (ItemGrade != "-1" && ItemPotency != "-1")
                    //{
                    //    string see = "";
                    //}
                    //else
                    //{
                    string sql = "SELECT TOP(1) A2.FinallyPriceTC  FROM PM_PurchaseOrder A1" +
                        " LEFT JOIN PM_POLine A2 ON A1.ID = A2.PurchaseOrder" +
                        " LEFT JOIN CBO_ItemMaster B1 ON B1.ID = A2.ItemInfo_ItemID " +
                        " WHERE " +
                        " B1.Code = '" + ItemCode + "' AND B1.Org = '" + orgID + "'" +
                        " AND A1.Status>1 " +
                        " ORDER BY A1.BusinessDate DESC";
                    DataTable dt = U9Common.GetDataTable(sql);
                    string FinallyriceTC = "0";
                    if (dt.Rows != null && dt.Rows.Count > 0)
                    {
                        FinallyriceTC = dt.Rows[0]["FinallyPriceTC"].ToString();
                    }
                    FinallyriceTC = Math.Round(decimal.Parse(FinallyriceTC), 4).ToString();
                    decimal D5 = Math.Round(IssuedQty * decimal.Parse(FinallyriceTC), 4);
                    string update = "UPDATE A2 " +
                        " SET A2.DescFlexField_PrivateDescSeg5='" + D5 + "'," +
                        " A2.DescFlexField_PrivateDescSeg4='" + FinallyriceTC + "'" +
                        " FROM MO_MO A1 " +
                        " LEFT JOIN MO_MOPickList A2 ON A1.ID = A2.MO " +
                        " WHERE A2.ID = '" + MoID + "' " +
                        " AND A2.ItemMaster = (SELECT ID FROM CBO_ItemMaster WHERE Code = '" + ItemCode + "' AND Org = '" + orgID + "')";
                    bool dt1 = U9Common.UpDateSQL(update);
                    //}
                    iuirecord["MOPickLists_DescFlexField_PrivateDescSeg5"] = D5; //金额
                    iuirecord["MOPickLists_DescFlexField_PrivateDescSeg4"] = FinallyriceTC;//最新价
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
