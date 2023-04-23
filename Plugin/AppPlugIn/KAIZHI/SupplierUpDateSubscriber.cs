using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UFIDA.U9.CBO.SCM.Supplier;
using UFSoft.UBF.Business;
using UFSoft.UBF.Util.Log;

namespace YY.U9.Cust.LI.AppPlugIn
{
    [UFSoft.UBF.Eventing.Configuration.Failfast]
    class SupplierUpDateSubscriber : UFSoft.UBF.Eventing.IEventSubscriber
    {
        private static readonly ILogger logger = LoggerManager.GetLogger(typeof(SupplierUpDateSubscriber));

        public void Notify(params object[] args)
        {
            #region 从事件参数中取得当前业务实体
            //从事件参数中取得当前业务实体
            if (args == null || args.Length == 0 || !(args[0] is UFSoft.UBF.Business.EntityEvent))
                return;

            BusinessEntity.EntityKey key = ((UFSoft.UBF.Business.EntityEvent)args[0]).EntityKey;
            if (key == null)
            {
                return;
            }
            Supplier  item = key.GetEntity() as Supplier;
            if (item == null)
            {
                return;
            }
            #endregion
            try
            {
                if (string.IsNullOrEmpty(item.ReceiptRule.ID.ToString()))
                {
                    throw new Exception("收货原则的值不能为空。");
                }
            }
            catch (Exception)
            {
                throw new Exception("收货原则的值不能为空。");
            }

            try
            {
                if (string.IsNullOrEmpty(item.APConfirmTerm.ID.ToString()))
                {
                    throw new Exception("立账条件的值不能为空。");
                }
            }
            catch (Exception)
            {
                throw new Exception("立账条件的值不能为空。");
            }
            string see = item.State.Value.ToString();
        }
    }
}
