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
    class CLMOStockMainUIFormWebPartExtend : ExtendedPartBase
    {

        private MOStockMainUIFormWebPart _part;




        public override void BeforeRender(IPart part, EventArgs args)
        {
            this._part = (part as MOStockMainUIFormWebPart);

            _part.Model.MO_MOPickLists.Records.Sort(_part.Model.MO_MOPickLists.FieldDocLineNO, RecordOrderType.ASC);

            base.BeforeRender(part, args);  
        }

    }
}
