using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Web;
using UFIDA.U9.CBO.MFG.BOM;
using UFIDA.U9.ISV.MO;
using UFIDA.U9.MO.MO;
using UFIDA.U9.SM.ForecastOrder;
using UFIDA.U9.SM.SO;
using UFSoft.UBF.Business;
using UFSoft.UBF.Util.DataAccess;
using UFSoft.UBF.Util.Log;

namespace YY.U9.Cust.LI.AppPlugIn
{
    /// <summary>
    /// 创联-生产订单备料复制
    /// </summary>
    [UFSoft.UBF.Eventing.Configuration.Failfast]
    class MoCopyChangeSubscrilber : UFSoft.UBF.Eventing.IEventSubscriber
    {
        private static readonly UFSoft.UBF.Util.Log.ILogger logger = LoggerManager.GetLogger(typeof(MoCopyChangeSubscrilber));

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
            MO mo = key.GetEntity() as MO;
            if (mo == null)
            {
                return;
            }
            #endregion
            string MOID = "";

            if (HttpContext.Current.Session["MOID"] != null)
            {
                MOID = HttpContext.Current.Session["MOID"].ToString();
                HttpContext.Current.Session["MOID"] = null;
            }
            MO mO = new MO();
            if (!string.IsNullOrEmpty(MOID))
            {
                mO = MO.Finder.FindByID(Convert.ToInt64(MOID));

            }
            else
            {
                return;
            }

            if (mo.DescFlexField.PrivateDescSeg12 == "False")
            {
                return;
            }
            decimal ProductQty = mo.ProductQty;

            //新
            string see = mo.DocNo;

            #region 取数-赋值

            //老
            string see333=mO.DocNo;

            if (mo.DocNo==mO.DocNo)
            {
                return;
            }

            //mo.CompleteQtyCtlType

            if (mo.SysState == UFSoft.UBF.PL.Engine.ObjectState.Inserted)
            {
                mo.MOPickLists.Clear();
                MO moer = mo;
                MO mO1 = new MO();
                foreach (var item in mO.MOPickLists)
                {
                    //MOPickList mOPickList = new MOPickList();
                    //mOPickList = item;

                    item.MO.ID = mo.ID;


                    mO1 = CreatSCMPickList(moer, item);

                    //新
                    string see33 = mO1.DocNo;

                    Session.Current.InList(mO1);
                }


                MO seemo = mO1;

                long see13 = seemo.ID;


                foreach (var item in mO1.MOPickLists)
                {
                    
                    long seeqqq = item.MOKey.ID;

                    long seewww = item.MO.ID;

                    MOPickList see123123 = item;
                }
            }
            #endregion

        }


        public static MO CreatSCMPickList(MO mo, MOPickList mOPick)
        {
            MO moHead = null;
            //moHead = MO.Create();
            moHead = mo;
            mo.MOPickLists.Add(mOPick);
            return moHead;
        }
    }
}

