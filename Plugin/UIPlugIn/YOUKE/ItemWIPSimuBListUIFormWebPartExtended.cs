using System;
using System.Collections.Generic;
using System.Data;
using UFIDA.U9.CBO.Pub.Controller;
using UFIDA.U9.CBO.SCM.Item;
using UFIDA.U9.MFG.MO.ItemWIPSimuBListUI;
using UFIDA.U9.MFG.MO.StartAnalysisUIModel;
using UFIDA.U9.UI.PDHelper;
using UFSoft.UBF.PL.Engine;
using UFSoft.UBF.UI.ControlModel;
using UFSoft.UBF.UI.Custom;
using UFSoft.UBF.UI.IView;
using UFSoft.UBF.UI.WebControlAdapter;
using UFSoft.UBF.UI.WebControls;
using UFSoft.UBF.Util.DataAccess;

namespace YY.U9.Cust.LI.UIPlugIn
{
    class ItemWIPSimuBListUIFormWebPartExtended : ExtendedPartBase
    {
        private ManufactureSimuResultUIFormWebPart _part;

        IUFMenu BtnSettle;


        public override void AfterInit(IPart part, System.EventArgs e)
        {
            base.AfterInit(part, e);
            _part = part as ManufactureSimuResultUIFormWebPart;
            BtnSettle = new UFWebMenuAdapter();
            BtnSettle.Text = "生成调入单";
            BtnSettle.ID = "BtnSettle";
            BtnSettle.AutoPostBack = true;
            BtnSettle.ItemClick += new MenuItemHandle(BtnSettle_Click);
            //加入操作里面
            IUFDropDownButton DdbOperation = (IUFDropDownButton)_part.GetUFControlByName(part.TopLevelContainer, "DDBQuery");
            DdbOperation.MenuItems.Add(BtnSettle);
        }

        public void BtnSettle_Click(object sender, EventArgs e)
        {
            //收集界面错误信息
            if (this._part.Model.ErrorMessage.hasErrorMessage)
            {
                this._part.Model.ClearErrorMessage();
            }
            this._part.OnDataCollect(this);
            DataTable dataTable = new DataTable();
            DataSet dataSet = new DataSet();
            List<MoItem> mos = new List<MoItem>();
            foreach (var item in _part.Model.ItemWIPSimu.SelectRecords)
            {
                string docno = item["MO_DocNo"].ToString();
                string itemcode = item["ItemMaster_Code"].ToString();
                string sqlForMoDocNoID = "SELECT a.ItemMaster,a.ActualReqQty,b.CompleteWh," +
                    " (SELECT DescFlexField_PrivateDescSeg15 FROM CBO_ItemMaster WHERE ID = a.ItemMaster) AS PrivateDescSeg15," +
                    " (SELECT CostUOM FROM CBO_ItemMaster WHERE ID=a.ItemMaster) AS CostUOM" +
                    " FROM MO_MOPickList a INNER JOIN MO_MO b ON a.MO = b.ID WHERE b.DocNo = '" + docno + "'" +
                    " AND b.ItemMaster = (SELECT ID FROM CBO_ItemMaster WHERE Code = '" + itemcode + "' AND Org = '" + PDContext.Current.OrgID + "')" +
                    " AND b.DescFlexField_PrivateDescSeg2 =''";
                DataAccessor.RunSQL(DataAccessor.GetConn(), sqlForMoDocNoID, null, out dataSet);
                dataTable = dataSet.Tables[0];
                if (dataTable != null && dataTable.Rows.Count > 0)
                {
                    for (int i = 0; i < dataTable.Rows.Count; i++)
                    {
                        MoItem moItem = new MoItem();
                        moItem.MoID = docno;
                        moItem.ItemMasterCode = long.Parse(dataTable.Rows[i]["ItemMaster"].ToString());
                        moItem.CompleteWhCode = long.Parse(dataTable.Rows[i]["CompleteWh"].ToString()); ;
                        moItem.ActualReqQty = decimal.Parse(dataTable.Rows[i]["ActualReqQty"].ToString());
                        moItem.PrivateDescSeg15 = dataTable.Rows[i]["PrivateDescSeg15"].ToString();//dataTable.Rows[i]["PrivateDescSeg15"].ToString();//"1"
                        moItem.CostUOM = long.Parse(dataTable.Rows[i]["CostUOM"].ToString());
                        mos.Add(moItem);
                    }
                }
                //string sqlupdate = "UPDATE MO_MO SET DescFlexField_PrivateDescSeg2 = '" + DateTime.Now.ToString("F") + "' WHERE DocNo = '" + docno + "'";
                //DataAccessor.RunSQL(DataAccessor.GetConn(), sqlupdate, null, out dataSet);
            }
            #region 调入单
            UFIDA.U9.ISV.TransferInISV.Proxy.CommonCreateTransferInSVProxy transferInSVProxy = new UFIDA.U9.ISV.TransferInISV.Proxy.CommonCreateTransferInSVProxy();

            UFIDA.U9.ISV.TransferInISV.IC_TransferInDTOData[] Boms;
            List<UFIDA.U9.ISV.TransferInISV.IC_TransferInDTOData> listBom = new List<UFIDA.U9.ISV.TransferInISV.IC_TransferInDTOData>();
            List<UFIDA.U9.ISV.TransferInISV.IC_TransInLineDTOData> listBomLine = new List<UFIDA.U9.ISV.TransferInISV.IC_TransInLineDTOData>();

            //头
            UFIDA.U9.ISV.TransferInISV.IC_TransferInDTOData Bom = new UFIDA.U9.ISV.TransferInISV.IC_TransferInDTOData();
            Bom.TransInDocType = new UFIDA.U9.CBO.Pub.Controller.CommonArchiveDataDTOData();
            Bom.TransInDocType.Code = "TransIn010";//单据类型
            Bom.TransferType = 0;//调入类型 0为一步式 1为两步式
            Bom.Org = new UFIDA.U9.CBO.Pub.Controller.CommonArchiveDataDTOData();
            Bom.Org.ID = long.Parse(PDContext.Current.OrgID);
            Bom.BusinessDate = new DateTime();
            //Bom.Memo=//备注
            Bom.TransInLines = new List<UFIDA.U9.ISV.TransferInISV.IC_TransInLineDTOData>();
            Bom.SysState = ObjectState.Inserted;
            //行
            foreach (var item in mos)
            {

                if (item.PrivateDescSeg15 == "1")
                {

                    UFIDA.U9.ISV.TransferInISV.IC_TransInLineDTOData Bom_line = new UFIDA.U9.ISV.TransferInISV.IC_TransInLineDTOData();
                    Bom_line.TransInOwnerOrg = new UFIDA.U9.CBO.Pub.Controller.CommonArchiveDataDTOData();
                    Bom_line.TransInOwnerOrg.ID = long.Parse(PDContext.Current.OrgID);
                    Bom_line.TransInWh = new UFIDA.U9.CBO.Pub.Controller.CommonArchiveDataDTOData();
                    Bom_line.TransInWh.ID = item.CompleteWhCode;//调入存储地点
                    Bom_line.StoreUOMQty = item.ActualReqQty;//调入数量
                    Bom_line.CostUOMQty = item.ActualReqQty;//成本数量
                    Bom_line.CostUOM = new CommonArchiveDataDTOData();

                    Bom_line.CostUOM.ID = item.CostUOM;

                    Bom_line.ItemInfo = new ItemInfoData();

                    Bom_line.ItemInfo.ItemID = item.ItemMasterCode;

                    Bom_line.SysState = ObjectState.Inserted;

                    Bom_line.DescFlexSegments = new UFIDA.U9.Base.FlexField.DescFlexField.DescFlexSegmentsData();
                    Bom_line.DescFlexSegments.PrivateDescSeg1 = item.MoID;

                    Bom_line.TransInSubLines = new List<UFIDA.U9.ISV.TransferInISV.IC_TransInSubLineDTOData>();
                    //子行

                    List<UFIDA.U9.ISV.TransferInISV.IC_TransInSubLineDTOData> listBomSubline = new List<UFIDA.U9.ISV.TransferInISV.IC_TransInSubLineDTOData>();
                    UFIDA.U9.ISV.TransferInISV.IC_TransInSubLineDTOData Bom_subLine = new UFIDA.U9.ISV.TransferInISV.IC_TransInSubLineDTOData();

                    Bom_subLine.TransOutOrg = new UFIDA.U9.CBO.Pub.Controller.CommonArchiveDataDTOData();

                    Bom_subLine.TransOutWh = new UFIDA.U9.CBO.Pub.Controller.CommonArchiveDataDTOData();
                    Bom_subLine.TransOutWh.Code = "01";

                    Bom_subLine.StoreUOMQty = item.ActualReqQty;

                    listBomSubline.Add(Bom_subLine);//加载子行
                    Bom_line.TransInSubLines = listBomSubline;
                    listBomLine.Add(Bom_line);//加载行 
                }
            }
            Bom.TransInLines = listBomLine;
            listBom.Add(Bom);
            transferInSVProxy.TransferInDTOList = listBom;

            if (mos.Count == 0)//单据行上面一个都没有
            {
                throw new Exception("检查生产订单是否被使用，或者料品是否能被调用");
            }

            List<CommonArchiveDataDTOData> see = transferInSVProxy.Do();

            string seee11 = DateTime.Now.ToString("F") + see[0].ToString();

            foreach (var item in mos)
            {
                string sqlupdate = "UPDATE MO_MO SET DescFlexField_PrivateDescSeg2 = '" + seee11 + "' WHERE DocNo = '" + item.MoID + "'";
                DataAccessor.RunSQL(DataAccessor.GetConn(), sqlupdate, null, out dataSet);
            }
            #endregion
        }


        public class MoItem
        {
            public string MoID { get; set; }
            public long ItemMasterCode { get; set; }
            public long CompleteWhCode { get; set; }
            public decimal ActualReqQty { get; set; }

            public string PrivateDescSeg15 { get; set; }
            public long CostUOM { get; set; }
        }
    }
}

