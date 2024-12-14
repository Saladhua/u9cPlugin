using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UFIDA.U9.MO.MO;
using UFSoft.UBF.Business;
using UFSoft.UBF.Util.Log;

namespace YY.U9.Cust.LI.AppPlugIn
{
    /// <summary>
    /// 创联-生产订单私有字段状态赋值
    /// </summary>
    [UFSoft.UBF.Eventing.Configuration.Failfast]
    class MOUpAndInsertedSubscrilber : UFSoft.UBF.Eventing.IEventSubscriber
    {
        private static readonly UFSoft.UBF.Util.Log.ILogger logger = LoggerManager.GetLogger(typeof(MOUpAndInsertedSubscrilber));

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

            #region 私有字段赋值

            decimal IssQty = 0;
            foreach (var item in mo.MOPickLists)
            {
                IssQty = item.IssuedQty + IssQty;//已发放数量
            }

            decimal Storage = 0;//入库数量

            Storage = mo.TotalRcvQty + mo.TotalScrapQty;//入库数量=累计入库+累计报废

            string MoID = mo.ID.ToString();

            #region 原来的
            //if (!string.IsNullOrEmpty(mo.DescFlexField.PrivateDescSeg5))
            //{
            //    if (mo.DescFlexField.PrivateDescSeg5 == "指定完工")
            //    {
            //        return;
            //    }
            //}
            //if (mo.DocState.Value == 0)
            //{
            //    mo.DescFlexField.PrivateDescSeg5 = "开立";
            //}
            //else if (mo.DocState.Value == 1 && IssQty == 0)//已审核且领料数量=0
            //{
            //    mo.DescFlexField.PrivateDescSeg5 = "未生产";
            //}
            //else if (IssQty > 0 && Storage == 0)//已审核且领料数量>0,入库数量=0
            //{
            //    mo.DescFlexField.PrivateDescSeg5 = "已发料";
            //}
            //else if (Storage > 0)//已审核且入库数量>0
            //{
            //    mo.DescFlexField.PrivateDescSeg5 = "生产中";
            //}
            //else if (mo.ProductQty == Storage)//已审核且全部入库
            //{
            //    mo.DescFlexField.PrivateDescSeg5 = "已完工";
            //}
            //else if (mo.DocState.Value == 3)//手动关闭且领料数量=0
            //{
            //    mo.DescFlexField.PrivateDescSeg5 = "指定完工";
            //}
            #endregion

            string D5 = "";

            if (mo.DocState.Value == 0)
            {
                mo.DescFlexField.PrivateDescSeg5 = "开立";
            }
            else if (mo.DocState.Value != 3 && IssQty == 0)//已审核且领料数量=0
            {
                mo.DescFlexField.PrivateDescSeg5 = "未生产";
            }
            else if (mo.DocState.Value != 3 && IssQty == 0)//开工且领料数量=0
            {
                mo.DescFlexField.PrivateDescSeg5 = "未生产";
            }
            else if (mo.DocState.Value != 3 && IssQty > 0 && Storage == 0)//已审核且领料数量>0,入库数量=0
            {
                mo.DescFlexField.PrivateDescSeg5 = "已发料";
            }
            else if (mo.DocState.Value != 3 && Storage > 0)//已审核且入库数量>0
            {
                mo.DescFlexField.PrivateDescSeg5 = "生产中";
            }
            else if (mo.DocState.Value == 3 && mo.ProductQty == Storage)//已审核且全部入库
            {
                mo.DescFlexField.PrivateDescSeg5 = "已完工";
            }
            else if (mo.DocState.Value == 3 && IssQty == 0)//手动关闭且领料数量=0
            {
                mo.DescFlexField.PrivateDescSeg5 = "指定完工";
            }
            else if (mo.DocState.Value == 3 && IssQty != 0 && Storage < mo.ProductQty)//手动关闭且领料数量>0且入库数量<生产数量
            {
                //mo.DescFlexField.PrivateDescSeg5 = "异常关闭";
                mo.DescFlexField.PrivateDescSeg5 = "异常关闭";
            }

            #endregion

        }


    }
}
