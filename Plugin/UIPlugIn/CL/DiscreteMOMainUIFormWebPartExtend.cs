using System;
using System.Collections.Generic;
using System.Data;
using System.Web;
using UFIDA.U9.MFG.MO.DiscreteMOUIModel;
using UFSoft.UBF.UI.Custom;
using UFSoft.UBF.UI.IView;
using UFSoft.UBF.UI.MD.Runtime;

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
                        Des = item["DescFlexField_PrivateDescSeg12"].ToString();
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

        public class DtoSet
        {
            /// <summary>
            /// ID
            /// </summary>
            public string ID { get; set; }

        }
    }
}
