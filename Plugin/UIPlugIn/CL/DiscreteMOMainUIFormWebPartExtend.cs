using System;
using System.Collections.Generic;
using System.Data;
using System.Web;
using UFIDA.U9.MFG.MO.DiscreteMOUIModel;
using UFSoft.UBF.UI.Custom;
using UFSoft.UBF.UI.IView;

namespace YY.U9.Cust.LI.UIPlugIn
{
    class DiscreteMOMainUIFormWebPartExtend : ExtendedPartBase
    {
        private DiscreteMOMainUIFormWebPart _part;



        public override void BeforeEventProcess(IPart part, string eventName, object sender, EventArgs args, out bool executeDefault)
        {
            base.BeforeEventProcess(part, eventName, sender, args, out executeDefault);
            UFSoft.UBF.UI.WebControlAdapter.UFWebButton4ToolbarAdapter webButton = sender as UFSoft.UBF.UI.WebControlAdapter.UFWebButton4ToolbarAdapter;
            this._part = (part as DiscreteMOMainUIFormWebPart);

            string Des = "";//字段是否勾选

            List<DtoSet> dotSets = new List<DtoSet>();

            if (webButton != null)
            {
                if (webButton.Action == "OnCopy")
                {
                    #region 在判断备料复制字段是否勾选
                    foreach (var item in this._part.Model.MO.Records)
                    {
                        Des = item["DescFlexField_PrivateDescSeg4"].ToString();
                    }
                    #endregion

                    if (Des == "False")
                    {
                        return;
                    }

                    #region 获取数据
                    foreach (var item in this._part.Model.MO.Records)
                    {
                        DtoSet dtoSet = new DtoSet();
                        dtoSet.ID = item["ID"].ToString();
                        dotSets.Add(dtoSet);
                    }
                    #endregion

                    #region 放到内存
                    DataTable dt = new DataTable();

                    foreach (var item in dotSets)
                    {
                        HttpContext.Current.Session["MOID"] = item.ID;
                    }


                    #endregion
                }
            }
        }


        public override void BeforeDataBinding(IPart part, out bool executeDefault)
        {
            this._part = (part as DiscreteMOMainUIFormWebPart);
            string MoID = "";
            string Dp5 = "";
            foreach (var item in _part.Model.MO.Records)
            {
                MoID = item["ID"].ToString();
            }
            UFIDA.U9.Cust.CLLH.CustMoStateDP1SV.Proxy.MoIDStateProxy moIDState = new UFIDA.U9.Cust.CLLH.CustMoStateDP1SV.Proxy.MoIDStateProxy();
            moIDState.DocNo = MoID;
            if (!string.IsNullOrEmpty(MoID))
            {
                Dp5 = moIDState.Do();
            }

            base.BeforeDataBinding(part, out executeDefault);

            if (MoID.Length > 5)
            {
                try
                {
                    this._part.Model.MO.FocusedRecord["DescFlexField_PrivateDescSeg5"] = Dp5;
                    this._part.Model.MO.FocusedRecord["DescFlexField_PrivateDescSeg5_Name"] = Dp5;
                }
                catch (Exception ex)
                {
                    string mse = ex.Message;
                    return;
                }

            }
        }


        public class DtoSet
        {
            /// <summary>
            /// ID
            /// </summary>
            public string ID { get; set; }

        }
    }
}
