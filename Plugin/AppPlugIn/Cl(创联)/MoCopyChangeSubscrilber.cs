using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Web;
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


            #region 取数-赋值
            //List<DotSet> dotSets = new List<DotSet>();

            //mo.MOPickLists.Clear();

            //int see123213 = mo.MOPickLists.Count;

            //foreach (var item in mO.MOPickLists)
            //{
            //    MOPickList mOPickList = new MOPickList();
            //    mOPickList.ItemCode = item.ItemCode;
            //    mOPickList.BOMReqQty = item.BOMReqQty;
            //    mOPickList.ActualReqQty = item.ActualReqQty;

            //    mo.MOPickLists.Add(mOPickList);
            //}
            #endregion


            #region 新-取数-赋值

            #region 查询
            UFIDA.U9.ISV.MO.Proxy.QueryMO4ExternalProxy queryMO4ExternalProxy = new UFIDA.U9.ISV.MO.Proxy.QueryMO4ExternalProxy();

            List<MOKeyDTOData> mOKeyDTODatas = new List<MOKeyDTOData>();

            MOKeyDTOData mOKeyDTOData = new MOKeyDTOData();

            mOKeyDTOData.DocNo = mo.DocNo;

            mOKeyDTODatas.Add(mOKeyDTOData);

            queryMO4ExternalProxy.MOKeyDTOs = mOKeyDTODatas;

            List<MODTOData> mODTODatas = queryMO4ExternalProxy.Do();

            MODTOData moData = new MODTOData();

            foreach (var item in mODTODatas)
            {
                moData = item;
            }
            #endregion


            #region 删除备料行
            UFIDA.U9.ISV.MO.Proxy.ModifyMO4ExternalProxy modifyMO4ExternalProxy = new UFIDA.U9.ISV.MO.Proxy.ModifyMO4ExternalProxy();
            List<MOModifyDTOData> mOModifyDTODatas = new List<MOModifyDTOData>();
            //修改生产订单
            UFIDA.U9.ISV.MO.MOModifyDTOData moModifyDTO = new UFIDA.U9.ISV.MO.MOModifyDTOData();

            UFIDA.U9.ISV.MO.MOKeyDTOData moKey = new UFIDA.U9.ISV.MO.MOKeyDTOData();
            moModifyDTO.MOKeyDTO = moKey;
            List<UFIDA.U9.ISV.MO.MOPickListDTOData> pickDTOList = new List<UFIDA.U9.ISV.MO.MOPickListDTOData>();

            pickDTOList.AddRange(moData.MOPickListDTOs);

            UFIDA.U9.ISV.MO.MOPickListDTOData moPickDTO = null;
            if (pickDTOList.Count > 0)
                moPickDTO = pickDTOList[0];

            if (moPickDTO != null)
            {
                moPickDTO.CUD = 8;//删除备料
            }

            moModifyDTO.MODTO = moData; //查询得到的结果作为修改的基础输入
            #endregion
            moKey.DocNo = mo.DocNo; //查询得到的单据号

            mOModifyDTODatas.Add(moModifyDTO);

            modifyMO4ExternalProxy.MOModifyDTOs = mOModifyDTODatas;

            bool see = modifyMO4ExternalProxy.Do();
            #endregion


            #region 新增备料行
            UFIDA.U9.ISV.MO.Proxy.ModifyMO4ExternalProxy modifyMO4ExternalProxy1 = new UFIDA.U9.ISV.MO.Proxy.ModifyMO4ExternalProxy();
            List<MOModifyDTOData> mOModifyDTODatas1 = new List<MOModifyDTOData>();
            //修改生产订单
            UFIDA.U9.ISV.MO.MOModifyDTOData moModifyDTO1 = new UFIDA.U9.ISV.MO.MOModifyDTOData();

            UFIDA.U9.ISV.MO.MOKeyDTOData moKey1 = new UFIDA.U9.ISV.MO.MOKeyDTOData();
            moModifyDTO1.MOKeyDTO = moKey1;
            moModifyDTO1.MODTO = moData; //查询得到的结果作为修改的基础输入

            moKey1.DocNo = mo.DocNo; //查询得到的单据号

            moData.ProductQty = mo.ProductQty; //生产数量   

            moData.Department = new UFIDA.U9.CBO.Pub.Controller.CommonArchiveDataDTOData();
            moData.Department.Code = mo.Department.Code; //生产部门


            //修改备料表
            List<UFIDA.U9.ISV.MO.MOPickListDTOData> pickDTOList1 = new List<UFIDA.U9.ISV.MO.MOPickListDTOData>();

            foreach (var item in mO.MOPickLists)
            {
                UFIDA.U9.ISV.MO.MOPickListDTOData moPickDTO1 = null;
                moPickDTO1 = new UFIDA.U9.ISV.MO.MOPickListDTOData();
                moPickDTO1.CUD = 2; //新增标志, 默认为修改 2-Inserted;4-Updated;8-Deleted
                moPickDTO1.OperationNum = item.OperationNum; //工序号
                moPickDTO1.ItemMaster = new UFIDA.U9.CBO.Pub.Controller.CommonArchiveDataDTOData();
                moPickDTO1.ItemMaster.Code = item.ItemMaster.Code; //料品
                moPickDTO1.ActualReqQty = item.ActualReqQty; //备料实际需求数量
                pickDTOList1.Add(moPickDTO1);
            }

            moData.MOPickListDTOs = pickDTOList1;

            mOModifyDTODatas1.Add(moModifyDTO1);

            modifyMO4ExternalProxy1.MOModifyDTOs = mOModifyDTODatas1;

            bool see1 = modifyMO4ExternalProxy1.Do();
            #endregion



            #region 将新的备料清空
            //foreach (var item in mo.MOPickLists)
            //{
            //    item = new MOPickList();
            //}
            #endregion


            long seeqe = mo.ID;
            string se12 = mo.DocNo;

            #region 赋值
            //foreach (var item in mo.MOPickLists)
            //{
            //    string see123 = item.DocNoMO.;
            //}
            #endregion
        }





        public class DotSet
        {
            /// <summary>
            /// 料品ID
            /// </summary>
            public long ItemID { get; set; }
            /// <summary>
            /// 料品Code
            /// </summary>
            public string ItemCode { get; set; }
            /// <summary>
            /// 料品Name
            /// </summary>
            public string ItemName { get; set; }
            /// <summary>
            /// BOM需求数量
            /// </summary>
            public decimal BOMReqQty { get; set; }
            /// <summary>
            /// 实际需求数量
            /// </summary>
            public decimal ActualReqQty { get; set; }

        }

    }
}
