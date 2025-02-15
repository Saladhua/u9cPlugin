﻿using System;
using System.Data;
using System.Reflection;
using UFIDA.U9.SCM.SD.ShipBListUIModel;
using UFIDA.U9.SCM.SM.SOUIModel;
using UFIDA.U9.UI.PDHelper;
using UFSoft.UBF.UI.ControlModel;
using UFSoft.UBF.UI.Custom;
using UFSoft.UBF.UI.IView;
using UFSoft.UBF.UI.WebControlAdapter;
using UFSoft.UBF.Util.DataAccess;

namespace YY.U9.Cust.LI.UIPlugIn
{
    /// <summary>
    /// 10月份宏巨开发-标准销售取数
    /// </summary>
    class StandardSOMainUIFormWebPartExtend : ExtendedPartBase
    {
        private StandardSOMainUIFormWebPart _part;

        IUFButton BtnSettle;


        public override void AfterInit(IPart part, EventArgs args)
        {
            base.AfterInit(part, args);
            _part = part as StandardSOMainUIFormWebPart;
            //实例化按钮
            BtnSettle = new UFWebButtonAdapter();
            new UFWebButtonAdapter();
            BtnSettle.Text = "取数";
            BtnSettle.ID = "BtnSettle";
            BtnSettle.AutoPostBack = true;
            BtnSettle.Click += new EventHandler(BtnSettle_Click);
            //加入Card容器
            IUFCard card = (IUFCard)part.GetUFControlByName(part.TopLevelContainer, "Card0");
            card.Controls.Add(BtnSettle);
            CommonFunction.Layout(card, BtnSettle, 18, 0);
        }






        //1、标准销售

        //取数规则：标准销售增加取数，标准销售单行增加字段库存量、复判数量、在制数量、待下工单数量

        //点击按钮取数查询，

        //库存量取数：取存储地点为“02” 成品仓+“38” 外购成品仓加“12” 样品仓+“09” 委外仓，对应料号（参考料号1）的库存可用量

        //复判可用量取数：取存储地点为“41” 成品复判仓（新），对应料号（参考料号1）的库存可用量

        //在制数量取数：取对应料号（参考料号1）的生产订单未入库数量（生产订单列表查询方案里面有一个未入库数量，只取正数）
        // +采购订单未入库数量加委外订单未入库数量

        //订单未交数量：销售订单类别未交数量，对应料号（参考料号1）

        //待下工单数量取数： 库存数量＋在制数量－销售订单未交数量-本次订单数量

        //销售订单列表：销售订单取数完成之后把信息回写到销售订单列表界面对应字段里面


        public void BtnSettle_Click(object sender, EventArgs e)
        {
            string ItemMaster_Code1 = "";

            string KCKUL = "";
            string FPKYL = "";
            string ZZSL = "";
            string FGGDWLL = "";
            string LineID = "";

            foreach (var item in this._part.Model.SO_SOLines.Records)
            {
                ItemMaster_Code1 = item["ItemInfo_ItemID"].ToString();//ItemMaster_Code1=ItemInfo_ItemID;
                string Code1 = item["ItemInfo_ItemID_Code1"].ToString();
                LineID = item["ID"].ToString();
                KCKUL = GetKCKYLUom(ItemMaster_Code1);
                FPKYL = GetFPKYL(ItemMaster_Code1);
                ZZSL = GetZZSL(ItemMaster_Code1);
                item["DescFlexField_PrivateDescSeg9"] = FPKYL;
                item["DescFlexField_PrivateDescSeg6"] = ZZSL;
                item["DescFlexField_PrivateDescSeg5"] = KCKUL;
                string Ddsl = item["OrderByQtyPU"].ToString();
                //string Chsl = GetSumShipQtyPU(LineID);//item["SOLineSumInfo_SumShipQtyPU"].ToString();
                decimal Wjl = Convert.ToDecimal(GetOrderByQtyPUSumShipQtyPU(Code1));//Convert.ToDecimal(Ddsl) - Convert.ToDecimal(Chsl);
                item["DescFlexField_PrivateDescSeg8"] = Wjl.ToString();
                //item["DescFlexField_PrivateDescSeg7"] = (Convert.ToDecimal(KCKUL) + Convert.ToDecimal(ZZSL) - Wjl - Convert.ToDecimal(Ddsl)).ToString();--12/19--本次订单数量取消
                //DescFlexField_PrivateDescSeg7--待下单数量--修改公式：新公式为待下单数量=库存数量+在制数量 -下单未发货-返工工单未领料
                FGGDWLL = GetFGGDWLL(ItemMaster_Code1);
                item["DescFlexField_PrivateDescSeg7"] = (Convert.ToDecimal(KCKUL) + Convert.ToDecimal(ZZSL) - Wjl - Convert.ToDecimal(FGGDWLL)).ToString();
            }
        }

        /// <summary>
        /// 库存量取数取数
        /// </summary>
        /// <param name="ItemCode"></param>
        /// <returns></returns>
        public string GetKCKYLUom(string ItemCode)
        {
            #region 原例子
            //SELECT sum((StoreQty -ResvStQty - ResvOccupyStQty))  AS 库存可用量 FROM InvTrans_WhQoh WHERE Wh in (select ID from CBO_Wh where Code = '02' or Code = '38'
            //or Code = '12'or Code = '09')
            //and ItemInfo_ItemID = '1002211110116938'
            #endregion

            string Wen = "0";

            #region 执行SQL
            DataTable dataTable = new DataTable();
            DataSet dataSet = new DataSet();
            string sqlForShipLine = "SELECT sum((StoreQty - ResvStQty - ResvOccupyStQty))  AS KCKYL " +
                " FROM InvTrans_WhQoh WHERE Wh in (select ID from CBO_Wh where Code = '02' or Code = '38' " +
                " or Code = '12'or Code = '09') and ItemInfo_ItemID = '" + ItemCode + "'and LogisticOrg = '" + PDContext.Current.OrgID + "' ";
            DataAccessor.RunSQL(DataAccessor.GetConn(), sqlForShipLine, null, out dataSet);
            dataTable = dataSet.Tables[0];
            if (dataTable != null && dataTable.Rows.Count > 0)
            {
                for (int i = 0; i < dataTable.Rows.Count; i++)
                {
                    Wen = dataTable.Rows[i]["KCKYL"].ToString();
                }
                if (string.IsNullOrEmpty(Wen))
                {
                    Wen = "0";
                }
            }
            #endregion

            return Wen;
        }
        /// <summary>
        /// 复判可用量取数
        /// </summary>
        /// <param name="ItemCode"></param>
        /// <returns></returns>
        public string GetFPKYL(string ItemCode)
        {
            string FPKYL = "0";

            #region 执行SQL
            DataTable dataTable = new DataTable();
            DataSet dataSet = new DataSet();
            string sqlForShipLine = " SELECT sum((StoreQty - ResvStQty - ResvOccupyStQty))  AS KCKYL FROM InvTrans_WhQoh WHERE Wh in (select ID from CBO_Wh where Code='41')" +
                " and ItemInfo_ItemID = '" + ItemCode + "' and LogisticOrg = '" + PDContext.Current.OrgID + "' ";
            DataAccessor.RunSQL(DataAccessor.GetConn(), sqlForShipLine, null, out dataSet);
            dataTable = dataSet.Tables[0];
            if (dataTable != null && dataTable.Rows.Count > 0)
            {
                for (int i = 0; i < dataTable.Rows.Count; i++)
                {
                    FPKYL = dataTable.Rows[i]["KCKYL"].ToString();
                }
                if (string.IsNullOrEmpty(FPKYL))
                {
                    FPKYL = "0";
                }
            }
            #endregion

            return FPKYL;
        }
        /// <summary>
        /// 在制数量取数
        /// </summary>
        public string GetZZSL(string ItemCode)
        {
            string ZZSL = "0";
            string CGWSSL = "0";
            string WWWSSL = "0";
            string JieGuo = "0";

            #region 执行SQL1
            DataTable dataTable = new DataTable();
            DataSet dataSet = new DataSet();
            string sqlForShipLine = "select sum(ProductQty-TotalRcvQty) AS WRKSL from MO_MO where ItemMaster= '" + ItemCode + "' " +
                " and DocState!= 3 and Org='" + PDContext.Current.OrgID + "' " +
                " and MODocType not in (select ID from MO_MODocType where Code in ('7','MES6') and Org='" + PDContext.Current.OrgID + "')";
            DataAccessor.RunSQL(DataAccessor.GetConn(), sqlForShipLine, null, out dataSet);
            dataTable = dataSet.Tables[0];
            if (dataTable != null && dataTable.Rows.Count > 0)
            {
                for (int i = 0; i < dataTable.Rows.Count; i++)
                {
                    ZZSL = dataTable.Rows[i]["WRKSL"].ToString();
                }
                if (string.IsNullOrEmpty(ZZSL))
                {
                    ZZSL = "0";
                }
            }
            #endregion
            #region 执行SQL2 采购订单未入库数量
            DataTable dataTable1 = new DataTable();
            DataSet dataSet1 = new DataSet();
            string sql = "select sum(PurQtyPU-TotalRecievedQtyPU) AS CGWSSL from PM_POLine inner join PM_PurchaseOrder on PM_POLine.PurchaseOrder=PM_PurchaseOrder.ID where " +
                " PM_POLine.Status in(0, 1, 2) and SUBSTRING(PM_PurchaseOrder.DocNo, 3, 2) = 'PO' and PM_PurchaseOrder.Org = '" + PDContext.Current.OrgID + "' " +
                " and PM_POLine.ItemInfo_ItemID='" + ItemCode + "'";
            DataAccessor.RunSQL(DataAccessor.GetConn(), sql, null, out dataSet1);
            dataTable1 = dataSet1.Tables[0];
            if (dataTable1 != null && dataTable1.Rows.Count > 0)
            {
                for (int i = 0; i < dataTable1.Rows.Count; i++)
                {
                    CGWSSL = dataTable1.Rows[i]["CGWSSL"].ToString();
                }
                if (string.IsNullOrEmpty(CGWSSL))
                {
                    CGWSSL = "0";
                }
            }
            #endregion
            #region 执行SQL3 委外订单未入库数量
            DataTable dataTable2 = new DataTable();
            DataSet dataSet2 = new DataSet();
            string sql2 = "select sum(PurQtyPU-TotalRecievedQtyPU) AS WWWSSL from PM_POLine inner join PM_PurchaseOrder on PM_POLine.PurchaseOrder=PM_PurchaseOrder.ID where " +
                " PM_POLine.Status in(0, 1, 2) and SUBSTRING(PM_PurchaseOrder.DocNo, 3, 2) = 'PE' and PM_PurchaseOrder.Org = '" + PDContext.Current.OrgID + "'" +
                " and PM_POLine.ItemInfo_ItemID='" + ItemCode + "'";
            DataAccessor.RunSQL(DataAccessor.GetConn(), sql2, null, out dataSet2);
            dataTable2 = dataSet2.Tables[0];
            if (dataTable2 != null && dataTable2.Rows.Count > 0)
            {
                for (int i = 0; i < dataTable2.Rows.Count; i++)
                {
                    WWWSSL = dataTable2.Rows[i]["WWWSSL"].ToString();
                }
                if (string.IsNullOrEmpty(WWWSSL))
                {
                    WWWSSL = "0";
                }
            }
            #endregion



            decimal sum = Convert.ToDecimal(ZZSL) + Convert.ToDecimal(CGWSSL) + Convert.ToDecimal(WWWSSL);

            if (sum < 0)
            {
                sum = 0;
            }

            JieGuo = sum.ToString();
            // JieGuo = (Convert.ToDecimal(ZZSL) + Convert.ToDecimal(CGWSSL) + Convert.ToDecimal(WWWSSL)).ToString();



            return JieGuo;
        }
        public string GetSumShipQtyPU(string ID)
        {
            string SSQPU = "0";
            #region 执行SQL1
            DataTable dataTable = new DataTable();
            DataSet dataSet = new DataSet();
            string sqlForShipLine = "select SOLineSumInfo_SumShipQtyPU from SM_SOLine where ID='" + ID + "' ";
            DataAccessor.RunSQL(DataAccessor.GetConn(), sqlForShipLine, null, out dataSet);
            dataTable = dataSet.Tables[0];
            if (dataTable != null && dataTable.Rows.Count > 0)
            {
                for (int i = 0; i < dataTable.Rows.Count; i++)
                {
                    SSQPU = dataTable.Rows[i]["SOLineSumInfo_SumShipQtyPU"].ToString();
                }
                if (string.IsNullOrEmpty(SSQPU))
                {
                    SSQPU = "0";
                }
            }
            #endregion

            return SSQPU;
        }
        /// <summary>
        /// 求和的未收数量
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        public string GetOrderByQtyPUSumShipQtyPU(string ID)
        {
            string SSQPU = "0";
            #region 执行SQL1
            DataTable dataTable = new DataTable();
            DataSet dataSet = new DataSet();
            string sqlForShipLine = "select sum(A.OrderByQtyPU-A.SOLineSumInfo_SumShipQtyPU) AS WSH from SM_SOLine A inner" +
                " join SM_SO B on A.SO = B.ID" +
                " where A.ItemInfo_ItemID in (select id from CBO_ItemMaster where Code1 = '" + ID + "' and Org = '" + PDContext.Current.OrgID + "')" +
                " and B.Org = '" + PDContext.Current.OrgID + "'and A.Status in('1', '2', '3','0') and B.Cancel_Canceled !=1";
            DataAccessor.RunSQL(DataAccessor.GetConn(), sqlForShipLine, null, out dataSet);
            dataTable = dataSet.Tables[0];
            if (dataTable != null && dataTable.Rows.Count > 0)
            {
                for (int i = 0; i < dataTable.Rows.Count; i++)
                {
                    SSQPU = dataTable.Rows[i]["WSH"].ToString();
                }
                if (string.IsNullOrEmpty(SSQPU))
                {
                    SSQPU = "0";
                }
            }
            #endregion

            return SSQPU;
        }

        /// <summary>
        /// 返工工单未领料
        /// 1.返工工单状态-未完工
        /// 2.返工工单未领料--1.返工工单备料的料品等于返工工单的料品 2.返工工单备料的实际需求数量-己发放数量
        /// select SUM(A2.ActualReqQty-A2.IssuedQty) AS FGGDWLL from MO_MO A1 left join MO_MOPickList A2
        /// ON A1.ID=A2.MO where A2.ItemMaster = '1002211230025299'
        /// and A1.MODocType= (select ID from MO_MODocType where Code= '2' and Org = '1002208260110060')
        /// and A1.DocState in ('0','1','2','4')
        /// </summary>
        /// <param name="ItemID"></param>
        /// <returns></returns>
        public string GetFGGDWLL(string ItemID)
        {
            string FGGDWLL = "0";
            #region 执行SQL1
            DataTable dataTable = new DataTable();
            DataSet dataSet = new DataSet();
            string sqlForShipLine = "select SUM(A2.ActualReqQty-A2.IssuedQty) AS FGGDWLL from MO_MO A1 left join MO_MOPickList A2" +
                " ON A1.ID = A2.MO where A2.ItemMaster = '" + ItemID + "' " +
                " and A1.MODocType = (select ID from MO_MODocType where Code = '2' and Org = '" + PDContext.Current.OrgID + "') " +
                " and A1.DocState in ('0', '1', '2', '4')";
            DataAccessor.RunSQL(DataAccessor.GetConn(), sqlForShipLine, null, out dataSet);
            dataTable = dataSet.Tables[0];
            if (dataTable != null && dataTable.Rows.Count > 0)
            {
                for (int i = 0; i < dataTable.Rows.Count; i++)
                {
                    FGGDWLL = dataTable.Rows[i]["FGGDWLL"].ToString();
                }
                if (string.IsNullOrEmpty(FGGDWLL))
                {
                    FGGDWLL = "0";
                }
            }
            #endregion 
            return FGGDWLL;
        }

        /// <summary>
        /// 废弃
        /// </summary>
        public class DtoSea
        {
            /// <summary>
            /// 库存量取数
            /// </summary>
            public string WH { get; set; }

            /// <summary>
            /// 复判可用量取数
            /// </summary>
            public string ReWH { get; set; }

            /// <summary>
            /// 在制数量
            /// </summary>
            public string QuUom { get; set; }

            /// <summary>
            /// 订单未交数量
            /// </summary>
            public string OrderUom { get; set; }

            /// <summary>
            /// 待下工单数量
            /// </summary>
            public string PendingUom { get; set; }
        }
    }
}
