using System;
using System.Data;
using UFIDA.U9.InvDoc.MiscRcv;
using UFIDA.U9.InvDoc.MiscShip;
using UFIDA.U9.UI.PDHelper;
using UFSoft.UBF.Business;
using UFSoft.UBF.PL.Engine;
using UFSoft.UBF.Util.DataAccess;
using UFSoft.UBF.Util.Log;


namespace YY.U9.Cust.LI.AppPlugIn
{
    /// <summary>
    /// 杂发单
    /// 创联提交验证
    /// 状态要updated
    /// </summary>
    [UFSoft.UBF.Eventing.Configuration.Failfast]
    class CLMiscShipmentLUpdateSubscriber : UFSoft.UBF.Eventing.IEventSubscriber
    {
        private static readonly ILogger logger = LoggerManager.GetLogger(typeof(CLMiscShipmentLUpdateSubscriber));
        public void Notify(params object[] args)
        {
            #region 从事件参数中取得当前业务实体
            //从事件参数中取得当前业务实体
            if (args == null || args.Length == 0 || !(args[0] is UFSoft.UBF.Business.EntityEvent))
                return;

            var key = ((UFSoft.UBF.Business.EntityEvent)args[0]).EntityKey;
            if (key == null)
            {
                return;
            }


            var miscShipments = key.GetEntity() as MiscShipment;


            if (miscShipments == null)
            {
                return;
            }
            #endregion


            string miscRcvTransLDocType = miscShipments.DocType.Code;

            if (miscRcvTransLDocType != "ZF03")
            {
                return;
            }

            int miscRcvTransLDocState = miscShipments.Status.Value;

            bool SetGo = false;

            string LineNo = "";

            if (miscRcvTransLDocState == 1)
            {
                foreach (var miscShipmentLs in miscShipments.MiscShipLs)
                {

                    string clkyl = miscShipmentLs.DescFlexSegments.PrivateDescSeg2;

                    decimal DClkyl = 0;

                    if (!string.IsNullOrEmpty(clkyl))
                    {
                        DClkyl = decimal.Parse(clkyl);
                    }

                    //创联可用量小于杂发单领用数量

                    if (DClkyl < miscShipmentLs.StoreUOMQty)
                    {
                        int linedocno = miscShipmentLs.DocLineNo;
                        SetGo = true;
                        LineNo = LineNo + "," + linedocno;
                    }
                }
            }

            if (SetGo)
            {
                throw new Exception("行号:" + LineNo.Substring(1) + "行，当前可用量不满足领用需求");
            }
        }
    }
}
