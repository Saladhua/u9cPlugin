using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using UFIDA.U9.FI.AP.PayReqFundUIModel;
using UFIDA.U9.MFG.MO.CreateDemandDocModel;
using UFIDA.U9.MFG.MO.PullListModelUI;
using UFIDA.U9.UI.PDHelper;
using UFSoft.UBF.UI.ControlModel;
using UFSoft.UBF.UI.Custom;
using UFSoft.UBF.UI.IView;
using UFSoft.UBF.UI.MD.Runtime;
using UFSoft.UBF.UI.WebControls.Association.Adapter;
using UFSoft.UBF.UI.WebControls.ClientCallBack;
using UFSoft.UBF.Util.DataAccess;

namespace YY.U9.Cust.LI.UIPlugIn
{
    class CreateDemandDocUIFormWebPartExtended : ExtendedPartBase
    {
        private CreateDemandDocUIFormWebPart _part;

        public override void AfterInit(IPart part, EventArgs e)
        {
            base.AfterInit(part, e);
            this._part = (part as CreateDemandDocUIFormWebPart);
            string see = _part.Model.WipPicksDemandSupplyDTO.ToString();

            IList<IUIRecord> selectRecords = _part.Model.WipPicksDemandSupplyDTO.Cache.GetSelectRecord();
            foreach (var item in selectRecords)
            {
                string see1 = item.ContainerView.ToString();
            }
        }

        public override void AfterDataLoad(IPart part)
        {
            base.AfterDataLoad(part);
            this._part = (part as CreateDemandDocUIFormWebPart);
            foreach (var item in _part.Model.WipPicksDemandSupplyDTO.Records)
            {
                string itemmaster = item["ItemInfo_ItemID"].ToString();
                string ItemVersion = item["ItemInfo_ItemVersion"].ToString();//版本号
                string pro = "";
                try
                {
                    pro = Convert.ToInt64(item["Project"]).ToString();
                }
                catch (Exception)
                {
                    pro = "";
                    throw;
                }

                DataTable dataTable = new DataTable();
                //and  project='"+ pro+"'
                string privDescSeg20 = "SELECT ISNULL( CAST(DescFlexField_PrivateDescSeg2 AS FLOAT) ,0)  AS  DescFlexField_PrivateDescSeg2  from MO_MOPickList WHERE ItemMaster='" + itemmaster + "'and Project = '" + pro + "' and " +
                    " ItemVersion=" +
                    " (select TOP(1) ID from CBO_ItemMasterVersion where Version = '" + ItemVersion + "' and Item = '" + itemmaster + "') ";
                if (pro == "0")//|| pro == "1002107270111496"
                {
                    pro = "";
                    privDescSeg20 = "SELECT ISNULL( CAST(DescFlexField_PrivateDescSeg2 AS FLOAT) ,0)  AS  DescFlexField_PrivateDescSeg2  from MO_MOPickList WHERE ItemMaster='" + itemmaster + "'  and " +
                        " ItemVersion=" +
                        " (select TOP(1) ID from CBO_ItemMasterVersion where Version = '" + ItemVersion + "' and Item = '" + itemmaster + "') ";
                }
                DataSet dataSet = new DataSet();
                DataAccessor.RunSQL(DataAccessor.GetConn(), privDescSeg20, null, out dataSet);
                dataTable = dataSet.Tables[0];
                if (dataTable.Rows != null && dataTable.Rows.Count > 0)//dataTable.Rows[0]["DescFlexField_PrivateDescSeg21"].ToString();
                {
                    decimal pSeg20 = 0;
                    bool a = decimal.TryParse(dataTable.Rows[0]["DescFlexField_PrivateDescSeg2"].ToString(), out pSeg20);
                    decimal pDQTy = 0;
                    bool b = decimal.TryParse(item["DemandQty"].ToString(), out pDQTy);
                    DataTable data = new DataTable();
                    string invTranssql = "select sum(StoreQtyCU) StoreQtyCU from  InvTrans_WhQoh  where ItemInfo_ItemCode=(select code from CBO_ItemMaster where id = '" + itemmaster + "')  and Wh='1002107210116261' " +
                        " and  Project = '" + pro + "' and ItemInfo_ItemVersion='" + ItemVersion + "'";
                    decimal storeQtyCU = 0;
                    DataSet set = new DataSet();
                    DataAccessor.RunSQL(DataAccessor.GetConn(), invTranssql, null, out set);
                    data = set.Tables[0];
                    if (data.Rows != null && data.Rows.Count > 0)
                    {
                        //现有库存
                        try
                        {
                            bool c = decimal.TryParse(data.Rows[0]["StoreQtyCU"].ToString(), out storeQtyCU);
                        }
                        catch (Exception)
                        {
                            storeQtyCU = 0;
                        }
                    }

                    //修改
                    //需求数量 < 安全库存，净需求量 = 安全库存 - WIP仓库存
                    //需求数量 >= 安全库存，净需求量 = 需求数量 - WIP仓库存
                    if (a)
                    {
                        if (PDContext.Current.OrgRef.CodeColumn == "10")
                        {

                            if (pDQTy < pSeg20)
                            {
                                item["PureDemandQty"] = pSeg20 - storeQtyCU;
                            }
                            else if (pDQTy >= pSeg20)
                            {
                                item["PureDemandQty"] = pDQTy - storeQtyCU;
                            }
 
                            string see = item["PureDemandQty"].ToString();
                        }

                    }
                }
            }
        }




    }
}

