using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UFIDA.U9.Cust.CL.LI.Cust_FindPlanOrderBP.Proxy;
using UFIDA.U9.MFG.MRP.PlanOrderUIModel;
using UFSoft.UBF.UI.Custom;
using UFSoft.UBF.UI.IView;

namespace YY.U9.Cust.LI.UIPlugIn
{
    class PlanOrderMainUIFormWebPartExtend : ExtendedPartBase
    {
        private PlanOrderMainUIFormWebPart _part;


        public override void BeforeRender(IPart _part, EventArgs args)
        {
            //获取料品
            string Item = "";

            string DocNo = "";

            string yjlyl = "";
            string yjhjl = "";
            string qgsl = "";

            this._part = (_part as PlanOrderMainUIFormWebPart);

            foreach (var item in this._part.Model.PlanOrder.Records)
            {
                try
                {
                    Item = item["Item"] == null ? "" : item["Item"].ToString();
                    DocNo = item["DocNo"] == null ? "" : item["DocNo"].ToString();
                }
                catch (Exception ex)
                {
                    //string message = "料号或单号为空";
                    //throw new Exception(message);
                    continue;
                }
                if (!string.IsNullOrEmpty(Item))
                {
                    UFIDA.U9.Cust.CL.LI.Cust_FindPlanOrderBP.Proxy.FindOperationProxy operationProxy = new FindOperationProxy();
                    operationProxy.Item = Item;
                    operationProxy.DocNo = DocNo;
                    UFIDA.U9.Cust.CL.LI.Cust_FindPlanOrderBP.ResultDtoData see1 = operationProxy.Do();

                    yjlyl = see1.Yjlyl;
                    yjhjl = see1.Yjlyl;
                    qgsl = see1.Qgsl;
                    try
                    {
                        item["DescFlexField_PrivateDescSeg3"] = yjlyl;
                        item["DescFlexField_PrivateDescSeg4"] = yjhjl;
                        item["DescFlexField_PrivateDescSeg5"] = qgsl;
                        string D2 = item["DescFlexField_PrivateDescSeg2"] == null ? "0" : item["DescFlexField_PrivateDescSeg2"].ToString();
                        if (string.IsNullOrEmpty(D2))
                        {
                            D2 = "0";
                        }
                        string D8 = item["DescFlexField_PrivateDescSeg8"] == null ? "0" : item["DescFlexField_PrivateDescSeg8"].ToString();
                        if (string.IsNullOrEmpty(D8))
                        {
                            D8 = "0";
                        }
                        string yjkll = (decimal.Parse(yjlyl) + decimal.Parse(yjhjl) + decimal.Parse(qgsl) + decimal.Parse(D2) + decimal.Parse(D8)).ToString();
                        item["DescFlexField_PrivateDescSeg7"] = yjkll;
                        item["DescFlexField_PrivateDescSeg1"] = see1.Dtjz;
                    }
                    catch (Exception ex)
                    {
                        continue;
                    }
                }
            }

            base.BeforeRender(_part, args);
        }

    }
}
