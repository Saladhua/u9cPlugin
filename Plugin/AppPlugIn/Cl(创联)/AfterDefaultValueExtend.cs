

namespace UFIDA.U9C.Cust.Coos.SCMPickList.PlugBE
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using U9.CBO.ParaConfigResultBE;
    using UFIDA.U9.Base;
    using UFIDA.U9.Base.FlexField.DescFlexField;
    using UFIDA.U9.Base.Profile;
    using UFIDA.U9.Base.Profile.Proxy;
    using UFIDA.U9.CBO.DTOs;
    using UFIDA.U9.CBO.Enums;
    using UFIDA.U9.CBO.MFG.BOM;
    using UFIDA.U9.CBO.MFG.Enums;
    using UFIDA.U9.CBO.ParaConfig;
    using UFIDA.U9.CBO.SCM.Item;
    using UFIDA.U9.CBO.SCM.PickList;
    using UFIDA.U9.CBO.SCM.Seiban;
    using UFIDA.U9.PM.PO;
    using UFIDA.U9.PM.Util;
    using UFIDA.U9C.Cust.Coos.SCMPickList.PlugBE.Util;
    using UFSoft.UBF.Business;
    using UFSoft.UBF.PL;
    using UFSoft.UBF.Services.Session;
    using UFSoft.UBF.Util.Context;
    using UFSoft.UBF.Util.DataAccess;
    using static UFSoft.UBF.Business.Entity;

    public partial class AfterDefaultValue
    {
        // 查询出上游单据和下游单据的描述性弹性域段的mappingdto
        static List<string> srcStrings = new List<string>();
        static List<string> targetStrings = new List<string>();
        static string targetString = "UFIDA.U9.CBO.SCM.PickList.SCMPickList";
        static FlexFieldMapingDTO flexFieldMapingDTO = null;
        private void Do_Notify(object[] args)
        {
            #region 从事件参数中取得当前业务实体													 
            if (args != null && args.Length != 0 && args[0] is EntityEvent)
            {
                BusinessEntity.EntityKey key = ((EntityEvent)args[0]).EntityKey;
                PicKHead pickList = key.GetEntity() as PicKHead;
                if (pickList == null)
                    return;
                
                //确保是更新且是重算备料的时候才参与业务
                if (pickList.SysState == UFSoft.UBF.PL.Engine.ObjectState.Updated)
                {
                    try {

                        PurchaseOrder holder = pickList.POLine.PurchaseOrder;
                        SCMPickList.EntityList list = pickList.SCMPickListS;

                        POLine.EntityList POLines = new POLine.EntityList();
                        List<BusinessEntity.EntityKey> inParamList = new List<BusinessEntity.EntityKey>();
                        if (ServiceSession.ThreadStorage["CoosReCalPickByPoLine"] != null)
                        {
                            inParamList = (List<BusinessEntity.EntityKey>)ServiceSession.ThreadStorage["CoosReCalPickByPoLine"];
                            //做备料重算
                            foreach (BusinessEntity.EntityKey keys in inParamList)
                            {
                                long poLineID = keys.ID;
                                if (poLineID > 0 && keys.EntityType == "UFIDA.U9.PM.PO.POLine")
                                {
                                    POLine line = POLine.Finder.FindByID(poLineID);
                                    if (line != null)
                                    {
                                        POLines.Add(line);
                                    }
                                }
                            }
                        }
                        ServiceSession.ThreadStorage["CoosReCalPickByPoLine"] = null;
                        if (POLines.Count == 0)
                        {
                            return;
                        }

                        List<ComponentUsageQtyDTO> componentUsageQtys = new List<ComponentUsageQtyDTO>();
                        List<ComponentUsageQtyDTOData> ComponentUsageQtyDTOList = new List<ComponentUsageQtyDTOData>();

                        List<ComponentUsageQtyDTO> virtualUsageQtys = new List<ComponentUsageQtyDTO>();
                        // 根据BOM计算备料用量和时间
                        bool isShrinkageNeeded = IsPickListWasteNeeded(holder.OrgKey);

                        foreach (POLine poLine in POLines)
                        {
                            //获取行号+料品+番号
                            long lineID = poLine.ID;
                            int DocLineNo = poLine.DocLineNo;
                            poLine.IsAutoExpandBOM = false;
                            if (poLine.ItemInfo == null)
                            {
                                break;
                            }
                            if (poLine.SeiBan == null)
                            {
                                break;
                            }
                            //以下部分因参照选择料品条件不符合，需研发进行修改，当前为固定化测试数据
                            //SeibanMaster seibanMaster = SeibanMaster.Finder.FindByID(1002209050220756);
                            //ItemMaster itemMaster = ItemMaster.Finder.FindByID(1002206280008953);
                            //long itemId = itemMaster.ID;
                            ////string SeiBanCode = seibanMaster.SeibanNO;
                            //long SeiBan = seibanMaster.ID;

                            ItemMaster itemMaster = poLine.ItemInfo.ItemID;
                            long itemId = poLine.ItemInfo.ItemID.ID;
                            long SeiBan = poLine.SeiBan.ID;
                            decimal remainQty = 0;
                            //只有可选配的才去选配结果找备料
                            if (itemMaster.IsMFGConfigEnable)
                            {
                                ComponentUsageQtyDTOList = CalcPickListForParaCfgUtil
                                  .CalcPickList(SeiBan,
                                   itemMaster.IsMFGConfigEnable ? itemMaster.Key : null,
                                   itemMaster.IsSKU ? itemMaster.Key : null,
                                  poLine.PurQtyPU,
                                  -1M,
                                  poLine.TradeUOMKey,
                                  isShrinkageNeeded,
                                  DateTime.Now,
                                  DateTime.Now,
                                  false);
                            }
                            if (poLine.SCMPickHead != null)
                            {
                                int counts = poLine.SCMPickHead.SCMPickListS.Count;
                                for (int i = counts - 1; i >= 0; i--)
                                {
                                    poLine.SCMPickHead.SCMPickListS.RemoveAt(i);
                                }
                            }


                            foreach (ComponentUsageQtyDTOData tempUsageQtyDtoData in ComponentUsageQtyDTOList)
                            {
                                ComponentUsageQtyDTO tmpCompUsageQtyDto = new ComponentUsageQtyDTO();
                                tmpCompUsageQtyDto.FromEntityData(tempUsageQtyDtoData);
                                if (tempUsageQtyDtoData.BOMComponentKey > 0)
                                {
                                    BOMComponent comp = BOMComponent.Finder.FindByID(tempUsageQtyDtoData.BOMComponentKey);
                                    if (comp != null)
                                    {
                                        if (comp.ItemMasterKey != tmpCompUsageQtyDto.ItemMaster)
                                        {
                                            ItemMaster item1 = tmpCompUsageQtyDto.ItemMaster.GetEntity();
                                            if (item1 != null &&
                                                comp.ItemMaster.ManufactureUOMKey != item1.ManufactureUOMKey)
                                            {
                                                decimal pararadio = PubMethod.GetUOMRationForV2WithOutRound(comp.ItemMasterKey, comp.ItemMaster.MaterialOutUOMKey, item1.MaterialOutUOMKey);
                                                if (pararadio > 0)
                                                {
                                                    tmpCompUsageQtyDto.Qty = tmpCompUsageQtyDto.Qty * pararadio;
                                                    tmpCompUsageQtyDto.STDReqQty = tmpCompUsageQtyDto.STDReqQty * pararadio;
                                                }
                                                else
                                                {
                                                    throw new Exception(string.Format("料品{0}单位{1}和料品{2}单位{3}之间没有设置转换率，请检查！", comp.ItemMaster.Code, comp.ItemMaster.MaterialOutUOM.Code, item1.Code, item1.MaterialOutUOM.Code));
                                                }

                                            }
                                        }
                                        tmpCompUsageQtyDto.BOMComponent = comp;
                                        tmpCompUsageQtyDto.UsageQtyType = comp.UsageQtyType;
                                        tmpCompUsageQtyDto.SubcontractItemSource = comp.SubcontractItemSrc;
                                        tmpCompUsageQtyDto.IssueStyle = comp.IssueStyle;
                                        tmpCompUsageQtyDto.SupplyWareHouse = comp.SupplyWareHouseKey;
                                        tmpCompUsageQtyDto.SupplyBin = comp.SupplyBinKey;
                                        tmpCompUsageQtyDto.RCVOpen = comp.RCVOpen;
                                        tmpCompUsageQtyDto.RCVApproved = comp.RCVApproved;
                                        componentUsageQtys.Add(tmpCompUsageQtyDto);
                                    }
                                }
                                else
                                {
                                    componentUsageQtys.Add(tmpCompUsageQtyDto);
                                }
                            }

                            if (ComponentUsageQtyDTOList.Count > 0)
                            {

                                //获取虚拟件数据
                                virtualUsageQtys = GetComponentUsageQtys(componentUsageQtys, poLine, isShrinkageNeeded, SeiBan);
                                PicKHead picKHead = CreatSCMPickList(virtualUsageQtys, poLine, holder);

                                //poLine.SCMPickHead = picKHead;
                                Session.Current.InList(picKHead);
                            }
                        }

                        ServiceSession.ThreadStorage["CoosReCalPickByPoLine"] = null;
                    }
                    catch {

                        ServiceSession.ThreadStorage["CoosReCalPickByPoLine"] = null;

                    }
                   
                }
            }
            #endregion
        }
        /// <summary>
        /// 创建备料
        /// </summary>
        /// <param name="ComponentUsageQtyDTOList"></param>
        /// <param name="poline"></param>
        /// <param name="purchaseOrder"></param>
        /// <returns></returns>
        public static PicKHead CreatSCMPickList(List<ComponentUsageQtyDTO> ComponentUsageQtyDTOList, POLine poline,PurchaseOrder purchaseOrder)
        {
            PicKHead pickHead = null;
            bool isExitReDoPick = false;
            int maxpickLineNo = 0;
            int Idx = 0;

            if (poline.SCMPickHeadKey != null)
            {
                pickHead = poline.SCMPickHead;
                if (poline.SCMPickHead.SCMPickListS.Count > 0)
                {
                    maxpickLineNo = poline.SCMPickHead.SCMPickListS[poline.SCMPickHead.SCMPickListS.Count - 1].PickLineNo;
                }

            }
            else
            {
                pickHead = PicKHead.Create();

                pickHead.ProjectKey = poline.ProjectKey;
                pickHead.TaskKey = poline.TaskKey;
                pickHead.POLine = poline;
                try {

                    pickHead.PoShipLine = poline.POShiplines[0];
                    //来源单据
                    pickHead.SrcDoc = new UFIDA.U9.CBO.SCM.PropertyTypes.SrcDocInfo();
                    if (poline.SrcDocInfo.SrcDoc != null)
                    {
                        pickHead.SrcDoc.SrcDoc = poline.SrcDocInfo.SrcDoc;
                        pickHead.SrcDoc.SrcDoc.EntityID = poline.SrcDocInfo.SrcDoc.EntityID;
                        pickHead.SrcDoc.SrcDoc.EntityType = poline.SrcDocInfo.SrcDoc.EntityType;
                        pickHead.SrcDoc.SrcDocBusiType = poline.SrcDocInfo.SrcDocBusiType;
                        pickHead.SrcDoc.SrcDocDate = poline.SrcDocInfo.SrcDocDate;

                        pickHead.SrcDoc.SrcDocLine = new U9.Base.PropertyTypes.BizEntityKey();
                        pickHead.SrcDoc.SrcDocLine.EntityID = poline.SrcDocInfo.SrcDocLine.EntityID;
                        pickHead.SrcDoc.SrcDocLine.EntityType = poline.SrcDocInfo.SrcDocLine.EntityType;
                        pickHead.SrcDoc.SrcDocLineNo = poline.SrcDocInfo.SrcDocLineNo;

                        pickHead.SrcDoc.SrcDocNo = poline.SrcDocInfo.SrcDocNo;
                        pickHead.SrcDoc.SrcDocOrgKey = poline.SrcDocInfo.SrcDocOrgKey;
                        pickHead.OrgKey = poline.SrcDocInfo.SrcDocOrgKey==null?Context.LoginOrg.Key: poline.SrcDocInfo.SrcDocOrgKey;
                    }

                    //MRP需要
                    if (poline.POShiplines != null && poline.POShiplines.Count > 0)
                    {
                        pickHead.SrcDoc.SrcDocSubLine = new U9.Base.PropertyTypes.BizEntityKey();
                        pickHead.SrcDoc.SrcDocSubLine.EntityID = poline.POShiplines[0].ID;
                        pickHead.SrcDoc.SrcDocSubLine.EntityType = poline.POShiplines[0].Key.EntityType;
                        pickHead.SrcDoc.SrcDocSubLineNo = poline.POShiplines[0].SubLineNo;
                    }
                }
                catch(Exception ex) {
                    string msg = ex.Message;

                }
                

            }
            //是否已经存在返工备料
            if (purchaseOrder.IsReDo && pickHead.SCMPickListS != null
                && pickHead.SCMPickListS.Count == 1
                && pickHead.SCMPickListS[0].ItemInfo.ItemIDKey == poline.ItemInfo.ItemIDKey)
            {
                isExitReDoPick = true;
                Idx++;
            }

            int Step = pickHead.Step;

            foreach (ComponentUsageQtyDTO usageQty in ComponentUsageQtyDTOList)
            {
                BOMComponent bomCom = null;
                if (usageQty.BOMComponent != null && usageQty.BOMComponent.ID > 0)
                {
                    bomCom = BOMComponent.Finder.FindByID(usageQty.BOMComponent.ID);
                }
                if (bomCom == null || bomCom.ID <= 0) continue;
                
                SCMPickList pick = SCMPickList.Create(pickHead);
                //筛选数据  根据料品+BOMComponent 查找数据 如果已发料，跳过
                SCMPickList oldPick = SCMPickList.Finder.Find("BOMComponent='" + bomCom.ID + "' and ItemInfo.ItemID='" + usageQty.ItemMaster.ID + "' and PicKHead='"+ pickHead.ID+ "'");
                if (oldPick!=null)
                {
                    if (oldPick.IssuedQty > 0 || oldPick.IssueNotDeliverQty > 0)
                    {
                        SetOldPickInfo(pick, oldPick);
                    }
                    else
                    {
                        //设置料品信息
                        SetPickInfo(poline, usageQty, bomCom, -10, pick);
                    }
                }
                else
                {
                    //设置料品信息
                    SetPickInfo(poline, usageQty, bomCom, -10, pick);
                }
                Idx++;
                //存在手工增加备料，后又增加bom 行号错误
                if (Idx == 1)
                {
                    pick.PickLineNo = maxpickLineNo + Step * Idx;
                }
                else
                {
                    pick.PickLineNo = Step * Idx;
                }
                maxpickLineNo = pick.PickLineNo;


            }
            if (!isExitReDoPick)
            {
                CreateReDoPickLine(poline, maxpickLineNo, pickHead);
            }
            
            return pickHead;
        }

        /// <summary>
        /// 创建新备料 or 更新维度
        /// </summary>
        /// <param name="poline"></param>
        /// <param name="usageQty"></param>
        /// <param name="bomCom"></param>
        /// <param name="maxpickLineNo"></param>
        /// <param name="pick"></param>
        private static void SetPickInfo(POLine poline, ComponentUsageQtyDTO usageQty, BOMComponent bomCom, int maxpickLineNo, SCMPickList pick)
        {
            //设置料品信息
            pick.ItemInfo.ItemID = ItemMaster.Finder.FindByID(usageQty.ItemMaster.ID); 
            pick.ItemInfo.ItemCode = usageQty.ItemMaster_Code;
            pick.ItemInfo.ItemName = usageQty.ItemMaster_Name;
            pick.ItemInfo.ItemVersion = usageQty.ItemVersionCode;
            pick.ItemVersion = ItemMasterVersion.Finder.Find("Item=" + usageQty.ItemMaster.ID + " and Version='" + usageQty.ItemVersionCode + "'");// bomCom.ItemVersionCode;

            pick.FromGrade = GradeEnum.GetFromValue(usageQty.FromDegree.Value);
            pick.ToGrade = GradeEnum.GetFromValue(usageQty.ToDegree.Value);
            pick.FromElement = ElementEnum.GetFromValue(usageQty.FromPotency.Value);
            pick.ToElement = ElementEnum.GetFromValue(usageQty.ToPotency.Value);

            #region 其他维度
            //========日期赋值
            if (poline.POShiplines != null && poline.POShiplines.Count > 0)
            {
                pick.PlanReqDate = poline.POShiplines[0].NeedPODate;
            }
            else
            {
                pick.PlanReqDate = PlatformContext.Current.LoginDate;
            }
            pick.ActualReqDate = pick.PlanReqDate;


            pick.BOMComponentKey = bomCom.Key;

            //pick.BOMReqQty = usageQty.Qty;
            pick.BOMReqQty = 0;//BOM用量初始化为0

            pick.StdReqQty = usageQty.STDReqQty;
            pick.ActualReqQty = usageQty.STDReqQty;//实际需求数保持与标准用量数量相同
            //pick.ActualReqQty = pick.BOMReqQty;
            pick.ConsignProcessItemSrc = bomCom.ConsignProcessItemSrc;

            pick.FixedScrap = bomCom.FixedScrap; 
            pick.WasteRate = bomCom.Scrap;
            pick.IsAutoCreate = true;
            pick.IsCheckATP = bomCom.IsATP;
            pick.IsControlSupplier = bomCom.IsControlSupplier;
            pick.IsDiffentLotCtl = bomCom.IsDiffentBatchCtl;
            pick.IsIssueOrgFixed = bomCom.IsIssueOrgFixed;
            pick.IsOverIssue = bomCom.IsOverIssue;
            pick.MaterialType = bomCom.MaterialType;
            pick.StandardMaterialScale = bomCom.StandardMaterialScale;
            pick.FixedMaterialNum = bomCom.FixedMaterialNum;

            pick.IsProjectTask = bomCom.IsSpecialUseItem;
            pick.ProjectKey = bomCom.CompProjectKey;
            pick.TaskKey = bomCom.CompTaskKey;

            if (pick.IsProjectTask
                && (pick.ProjectKey == null || pick.ProjectKey.ID <= 0))
            {
                pick.ProjectKey = poline.ProjectKey;
                pick.TaskKey = poline.TaskKey;
            }

            if (usageQty.IssueStyle == IssueStyleEnum.Push)
            {
                pick.IssueStyle = PMIssueStyleEnum.Push;
            }
            else if (usageQty.IssueStyle == IssueStyleEnum.Phantom)
            {
                pick.IssueStyle = PMIssueStyleEnum.Phantom;
            }
            else
            {
                pick.IssueStyle = PMIssueStyleEnum.Pull;
            }

            pick.JIT = bomCom.JIT;
            pick.OverlapRate = bomCom.OverlapRate;

            pick.OwnerOrgKey = bomCom.OwnerOrgKey;
            if (maxpickLineNo != -10)
            {
                pick.PickLineNo = pick.PicKHead.Step + maxpickLineNo;
            }
            pick.QtyType = bomCom.UsageQtyType;

            if (pick.PicKHeadKey != null && pick.PicKHead.CalQPAType == 0)
            {
                pick.QPA = usageQty.QPA;
            }
            else
            {
                pick.QPA = usageQty.ActualQPA;
            }


            if (poline.SCMPickHeadKey != null)
            {
                pick.PicKHeadKey = poline.SCMPickHead.Key;
            }

            pick.ScrapType = bomCom.ScrapType;

            //用量类型为批量则转为固定
            if (UsageQuantityTypeEnum.GetFromValue(usageQty.UsageQtyType.Value) == UsageQuantityTypeEnum.Lot)
            {
                pick.QtyType = UsageQuantityTypeEnum.FixedQuantity;
            }
            else
            {
                pick.QtyType = UsageQuantityTypeEnum.GetFromValue(usageQty.UsageQtyType.Value);
            }
            //委外料品来源           
            pick.SubcItemSrcType = bomCom.SubcontractItemSrc;


            pick.SupplyBinKey = bomCom.SupplyBinKey;
            pick.SupplyOrgKey = bomCom.IssueOrgKey;
            pick.SupplyStyle = bomCom.SupplyStyle;
            pick.SupplyWhKey = bomCom.SupplyWareHouseKey;
            //设置齐套标志=收货开立或者收货审核
            pick.RcvApproveSetCheck = bomCom.RCVApproved;
            pick.RcvSaveSetCheck = bomCom.RCVOpen;
            #endregion

            #region 计量单位赋值
            pick.IssueUOMKey = bomCom.IssueUOMKey;
            pick.IsWholeSetIssue = bomCom.IsWholeSetIssue;
            pick.IUToIBURate = ItemMaster.GetUOMRatio4MainSub(pick.ItemInfo.ItemIDKey, pick.IssueUOMKey);
            pick.IssueBaseUOMKey = pick.IssueUOM.BaseUOMKey;

            pick.CoUOMKey = pick.ItemInfo.ItemID.CostUOMKey;
            pick.CostBaseUOMKey = pick.ItemInfo.ItemID.CostUOM.BaseUOMKey;
            pick.CUToCBURate = ItemMaster.GetUOMRatio4MainSub(pick.ItemInfo.ItemIDKey, pick.CoUOMKey);

            pick.RcvUOMKey = pick.ItemInfo.ItemID.InventorySecondUOMKey;
            pick.RcvBaseUOMKey = pick.ItemInfo.ItemID.InventorySecondUOM.BaseUOMKey;
            pick.SUToSBURate = ItemMaster.GetUOMRatio4MainSub(pick.ItemInfo.ItemIDKey, pick.RcvUOMKey);

            ItemUOMRate itemRate = null;
            ItemUOMRate itemRateRcv = null;
            TempCache cache = new TempCache();
            if (pick.ItemInfo.ItemID.IsVarRatio)
            {
                itemRate = new ItemUOMRate(pick.IssueUOMKey, pick.CoUOMKey, pick.ItemInfo.ItemIDKey);
                itemRateRcv = new ItemUOMRate(pick.IssueUOMKey, pick.RcvUOMKey, pick.ItemInfo.ItemIDKey);
                if (cache.UOMRateMap.ContainsKey(itemRate))
                {
                    pick.IBUToCBURate = cache.UOMRateMap[itemRate];
                }
                else
                {
                    pick.IBUToCBURate = UOMRate.GetUOMRate(pick.IssueUOMKey, pick.IssueBaseUOMKey, pick.IUToIBURate, pick.CoUOMKey, pick.CostBaseUOMKey, pick.CUToCBURate, pick.ItemInfo.ItemIDKey);
                }
                if (cache.UOMRateMap.ContainsKey(itemRateRcv))
                {
                    pick.IBUToSBURate = cache.UOMRateMap[itemRateRcv];
                }
                else
                {
                    pick.IBUToSBURate = UOMRate.GetUOMRate(pick.IssueUOMKey, pick.IssueBaseUOMKey, pick.IUToIBURate, pick.RcvUOMKey, pick.RcvBaseUOMKey, pick.SUToSBURate, pick.ItemInfo.ItemIDKey);
                }
            }
            else
            {
                pick.IBUToCBURate = UOMRate.GetUOMRate(pick.IssueUOMKey, pick.IssueBaseUOMKey, pick.IUToIBURate, pick.CoUOMKey, pick.CostBaseUOMKey, pick.CUToCBURate, pick.ItemInfo.ItemIDKey);
                pick.IBUToSBURate = UOMRate.GetUOMRate(pick.IssueUOMKey, pick.IssueBaseUOMKey, pick.IUToIBURate, pick.RcvUOMKey, pick.RcvBaseUOMKey, pick.SUToSBURate, pick.ItemInfo.ItemIDKey);
            }
            #endregion

            #region 弹性域
            if (flexFieldMapingDTO == null)
            {
                srcStrings.Add("UFIDA.U9.CBO.MFG.BOM.BOMComponent");
                targetStrings.Add(targetString);
                flexFieldMapingDTO = PubMethod.GetFlexFieldMaping(srcStrings, targetStrings);
            }

            if (flexFieldMapingDTO != null && flexFieldMapingDTO.PublicFlexFieldReferenceDTOs != null && flexFieldMapingDTO.PublicFlexFieldReferenceDTOs.Count > 0)
            {
                SourceEntityAndDescFieldsDTO sourceEntityAndDescFieldsDTO = new SourceEntityAndDescFieldsDTO();
                sourceEntityAndDescFieldsDTO.DescFlexSegments = pick.BOMComponent.DescFlexField;
                sourceEntityAndDescFieldsDTO.SourceEntity = pick.BOMComponent;
                pick.DescFlexField = PubMethod.GetFlexField("UFIDA.U9.CBO.MFG.BOM.BOMComponent", sourceEntityAndDescFieldsDTO, targetString, flexFieldMapingDTO);
            }

            pick.Remarks = pick.BOMComponent.Remark;
            #endregion
        }
       
        private static void CreateReDoPickLine(POLine poline, int maxpickLineNo, PicKHead pickHead)
        {
            if (!poline.PurchaseOrder.IsReDo)
                return;
            TempCache cache = poline.PurchaseOrder.TempCache;
            SCMPickList pickline = SCMPickList.Create(pickHead);
            if (maxpickLineNo != -10)
            {
                pickline.PickLineNo = pickHead.Step + maxpickLineNo;
            }
            SetPickInfo4ReDO(poline, cache, pickline);
        }
        private static void SetPickInfo4ReDO(POLine poline, TempCache cache, SCMPickList pickline)
        {
            pickline.ItemInfo = poline.ItemInfo;

            ItemMaster item = poline.ItemInfo.ItemID;

            pickline.ItemVersionKey = new ItemMasterVersion.EntityKey(item.VersionID);

            pickline.BOMReqQty = 0;
            //发料单位 
            pickline.IssueUOM = item.MaterialOutUOM;
            pickline.IssueBaseUOMKey = item.MaterialOutUOM.BaseUOMKey;

            pickline.IUToIBURate = item.MaterialOutUOM.RatioToBase;

            //库存单位
            pickline.RcvUOMKey = item.InventorySecondUOMKey;//库存单位

            pickline.RcvBaseUOMKey = item.InventorySecondUOM.BaseUOMKey; ;

            pickline.SUToSBURate = item.InventorySecondUOM.RatioToBase;
            //成本
            pickline.CoUOMKey = item.CostUOMKey;
            pickline.CostBaseUOMKey = item.CostUOM.BaseUOMKey;
            pickline.CUToCBURate = item.CostUOM.RatioToBase;

            ItemUOMRate itemRate = null;
            ItemUOMRate itemRateRcv = null;

            if (item.IsVarRatio)
            {
                itemRate = new ItemUOMRate(pickline.IssueUOMKey, pickline.CoUOMKey, pickline.ItemInfo.ItemIDKey);
                itemRateRcv = new ItemUOMRate(pickline.IssueUOMKey, pickline.RcvUOMKey, pickline.ItemInfo.ItemIDKey);
                if (cache.UOMRateMap.ContainsKey(itemRate))
                {
                    pickline.IBUToCBURate = cache.UOMRateMap[itemRate];
                }
                else
                {
                    pickline.IBUToCBURate = UOMRate.GetUOMRate(pickline.IssueUOMKey, pickline.IssueBaseUOMKey, pickline.IUToIBURate, pickline.CoUOMKey, pickline.CostBaseUOMKey, pickline.CUToCBURate, pickline.ItemInfo.ItemIDKey);
                }
                if (cache.UOMRateMap.ContainsKey(itemRateRcv))
                {
                    pickline.IBUToSBURate = cache.UOMRateMap[itemRateRcv];
                }
                else
                {
                    pickline.IBUToSBURate = UOMRate.GetUOMRate(pickline.IssueUOMKey, pickline.IssueBaseUOMKey, pickline.IUToIBURate, pickline.RcvUOMKey, pickline.RcvBaseUOMKey, pickline.SUToSBURate, pickline.ItemInfo.ItemIDKey);
                }
            }
            else
            {
                pickline.IBUToCBURate = UOMRate.GetUOMRate(pickline.IssueUOMKey, pickline.IssueBaseUOMKey, pickline.IUToIBURate, pickline.CoUOMKey, pickline.CostBaseUOMKey, pickline.CUToCBURate, pickline.ItemInfo.ItemIDKey);
                pickline.IBUToSBURate = UOMRate.GetUOMRate(pickline.IssueUOMKey, pickline.IssueBaseUOMKey, pickline.IUToIBURate, pickline.RcvUOMKey, pickline.RcvBaseUOMKey, pickline.SUToSBURate, pickline.ItemInfo.ItemIDKey);
            }

            DoubleQuantity supplierqty = new DoubleQuantity(poline.SupplierConfirmQtyTU, poline.TradeUOMKey, poline.TradeBaseUOMKey,
    poline.TUToTBURate, poline.SupplierConfirmQtyTBU, poline.TradeUOMBKey, poline.TradeBaseUOMBKey, poline.TUToTBURateB);


            //2020.8.21 202008190258 
            decimal TUToBOMUOMRate = UOMRate.GetUOMRate(poline.TradeUOMKey, pickline.IssueUOMKey, poline.ItemInfo.ItemIDKey);
            // UOMHelper.GetUOMConvertRatio(poline.TradeUOMKey, pickline.IssueUOMKey);
            //if (poline.TUToBOMUOMRate != 0)
            //{
            //    TUToBOMUOMRate = poline.TUToBOMUOMRate;
            //}



            Decimal qty = pickline.IssueUOM.Round.GetRoundValue((supplierqty.GetTotalAmount().Amount) * TUToBOMUOMRate);
            pickline.StdReqQty = qty;
            pickline.ActualReqQty = qty;


            pickline.JIT = item.InventoryInfo.IsLimitWarehouse;
            pickline.SupplyWhKey = item.InventoryInfo.WarehouseKey;

            pickline.SupplyBinKey = item.InventoryInfo.BinKey;


            pickline.FromGrade = item.StartGrade;
            pickline.ToGrade = item.EndGrade;

            pickline.QPA = pickline.StdReqQty / supplierqty.GetTotalAmount().Amount;

            pickline.FromElement = item.StartPotency;
            pickline.ToElement = item.EndPotency;

            pickline.IsControlSupplier = item.IsTrademark;
            pickline.IsCheckATP = item.SaleInfo.IsATPCheck;

            //pickline["ItemMaster_ConverRatioRule"] = int.Parse(curRec["ConverRatioRule"]);
            pickline.IsOverIssue = item.MfgInfo.IsAllowExcessMaterial;
            //pickline["ItemMaster_MfgInfo_IsAllowExcessMaterial"] = bool.Parse(curRec["MfgInfo_IsAllowExcessMaterial"]);

            pickline.IsProjectTask = false;
            if (item.IsSpecialItem)
            {
                pickline.IsProjectTask = true;
            }
            //pickline["ItemMaster_IsSpecialItem"] = bool.Parse(curRec["IsSpecialItem"]);
            pickline.ScrapType = ScrapTypeEnum.GetFromValue(item.MfgInfo.ProduceWasteRateType.Value);
            pickline.FixedScrap = item.MfgInfo.ImmovableWaste;
            pickline.WasteRate = item.MfgInfo.FluctuantWaste;
            pickline.SubcItemSrcType = null;

            pickline.IsDiffentLotCtl = item.MfgInfo.IsMixLotControl;

            pickline.IsWholeSetIssue = false;
            //计划需求日赋值
            pickline.PlanReqDate = poline.POShiplines[0].NeedPODate;//this.Model.PicKHead.FocusedRecord == null ? PlatformContext.Current.LoginDate : this.Model.PicKHead.FocusedRecord.PlanReqDate;
            pickline.ActualReqDate = pickline.PlanReqDate;

            pickline.MaterialType = item.MfgInfo.IsSueOverType;
            pickline.StandardMaterialScale = item.MfgInfo.StandardMaterialScale;
            pickline.FixedMaterialNum = item.MfgInfo.StandardMaterialQty;
            pickline.IssueStyle = PMIssueStyleEnum.Push;
            pickline.SupplyOrgKey = poline.CurrentOrgKey;
            pickline.QtyType = UsageQuantityTypeEnum.Variable;
        }

        /// <summary>
        /// 组装虚拟件数据
        /// </summary>
        /// <param name="ComponentUsageQtyDTOList"></param>
        /// <returns></returns>
        public static List<ComponentUsageQtyDTO> GetComponentUsageQtys(List<ComponentUsageQtyDTO> componentUsageQtys, POLine poLine,bool isShrinkageNeeded, long SeiBan)
        {
            List<ComponentUsageQtyDTO> virtualUsageQtys = new List<ComponentUsageQtyDTO>();
            decimal remainQty = 0;
            remainQty = poLine.PurQtyPU;
            foreach (ComponentUsageQtyDTO tempUsageQtyDtoData in componentUsageQtys)
            {
                //2022-08-23 如果本身是虚拟件，则不展示，取子件
                //virtualUsageQtys.Add(tempUsageQtyDtoData);
                
                BOMComponent bomComponent = tempUsageQtyDtoData.BOMComponent;
                ItemMaster item1 = tempUsageQtyDtoData.ItemMaster.GetEntity();

                if (bomComponent.IsPhantomPart && bomComponent.IssueStyle != IssueStyleEnum.Phantom)
                {
                    if (item1.IsMFGConfigEnable)
                    {
                        List<ComponentUsageQtyDTOData> compUsageForParaCfgList = CalcPickListForParaCfgUtil.
                            CalcPickList(SeiBan,
                          item1.Key,
                          item1.IsSKU ? item1.Key : null,
                          remainQty,
                          -1M,
                          poLine.TradeUOMKey,
                          isShrinkageNeeded,
                          DateTime.Now,
                          DateTime.Now,
                          false);
                        foreach (ComponentUsageQtyDTOData usageQtyDTOData in compUsageForParaCfgList)
                        {
                            ComponentUsageQtyDTO componentUsageQty = new ComponentUsageQtyDTO();
                            componentUsageQty.FromEntityData(usageQtyDTOData);
                            if (componentUsageQty.BOMComponentKey > 0)
                            {
                                BOMComponent comp = BOMComponent.Finder.FindByID(usageQtyDTOData.BOMComponentKey);
                                if (comp != null)
                                {
                                    if (comp.ItemMasterKey != componentUsageQty.ItemMaster)
                                    {
                                        ItemMaster item = componentUsageQty.ItemMaster.GetEntity();
                                        if (item != null &&
                                            comp.ItemMaster.ManufactureUOMKey != item.ManufactureUOMKey)
                                        {
                                            decimal pararadio = PubMethod.GetUOMRationForV2WithOutRound(comp.ItemMasterKey, comp.ItemMaster.MaterialOutUOMKey, item.MaterialOutUOMKey);
                                            if (pararadio > 0)
                                            {
                                                componentUsageQty.Qty = componentUsageQty.Qty * pararadio;
                                                componentUsageQty.STDReqQty = componentUsageQty.STDReqQty * pararadio;
                                            }
                                            else
                                            {
                                                throw new Exception(string.Format("料品{0}单位{1}和料品{2}单位{3}之间没有设置转换率，请检查！", comp.ItemMaster.Code, comp.ItemMaster.MaterialOutUOM.Code, item.Code, item.MaterialOutUOM.Code));
                                            }
                                        }
                                    }
                                    componentUsageQty.BOMComponent = comp;
                                    componentUsageQty.UsageQtyType = comp.UsageQtyType;
                                    componentUsageQty.SubcontractItemSource = comp.SubcontractItemSrc;
                                    componentUsageQty.IssueStyle = comp.IssueStyle;
                                    componentUsageQty.SupplyWareHouse = comp.SupplyWareHouseKey;
                                    componentUsageQty.SupplyBin = comp.SupplyBinKey;
                                    componentUsageQty.RCVOpen = comp.RCVOpen;
                                    componentUsageQty.RCVApproved = comp.RCVApproved;
                                    // 如果BOMComponentID>0则判断按照此ID能否找到BOM子项
                                    // 如找到则处理该记录,否则不处理(表明该ID对应的BOM子项已被删除)
                                    virtualUsageQtys.Add(componentUsageQty);
                                }
                            }
                            else
                            {
                                virtualUsageQtys.Add(componentUsageQty);
                            }
                        }

                    }
                }
                else
                {
                    virtualUsageQtys.Add(tempUsageQtyDtoData);
                }
            }
            return virtualUsageQtys;
        }
        /// <summary>
        /// 备料考虑损耗
        /// </summary>
        /// <param name="org"></param>
        /// <returns></returns>
        public static bool IsPickListWasteNeeded(UFIDA.U9.Base.Organization.Organization.EntityKey org)
        {
            bool result = (bool)UFIDA.U9.Base.Profile.Profile.GetValue("WIP002", org);
            return result;
        }
        public static bool GetSystemParam(string ProfileCode, long org)
        {
            bool strValue = false;
            UFSoft.UBF.Util.Cache.ICache sessionCache = ServiceSession.Cache;
            string CacheKey = "MFG_DefaultProfileCode" + org + ProfileCode;
            if (sessionCache[CacheKey] != null)
            {
                return Convert.ToBoolean(sessionCache[CacheKey].ToString());
            }

            GetProfileValueProxy qryParameter = new GetProfileValueProxy();
            qryParameter.ProfileCode = ProfileCode;
            qryParameter.ProfileOrg = org;
            PVDTOData rtnDTOData = qryParameter.Do();
            strValue = rtnDTOData.ProfileValue == null ?
                false :
                Convert.ToBoolean(rtnDTOData.ProfileValue.Trim());
            return strValue;
        }
        /// <summary>
        /// 创建新备料 or 更新维度
        /// </summary>
        /// <param name="poline"></param>
        /// <param name="usageQty"></param>
        /// <param name="bomCom"></param>
        /// <param name="maxpickLineNo"></param>
        /// <param name="pick"></param>
        private static void SetOldPickInfo(SCMPickList pick, SCMPickList oldPick)
        {
            //设置料品信息
            pick.ItemInfo.ItemID = oldPick.ItemInfo.ItemID;
            pick.ItemInfo.ItemCode = oldPick.ItemInfo.ItemCode;
            pick.ItemInfo.ItemName = oldPick.ItemInfo.ItemName;
            pick.ItemInfo.ItemVersion = oldPick.ItemInfo.ItemVersion;
            pick.ItemVersion = oldPick.ItemVersion;// bomCom.ItemVersionCode;

            pick.FromGrade = oldPick.FromGrade;
            pick.ToGrade = oldPick.ToGrade;
            pick.FromElement = oldPick.FromElement;
            pick.ToElement = oldPick.ToElement;
            pick.IssuedQty = oldPick.IssuedQty;
            pick.IssueNotDeliverQty = oldPick.IssueNotDeliverQty;
            #region 其他维度
            //========日期赋值
            pick.PlanReqDate = oldPick.PlanReqDate;
            pick.ActualReqDate = oldPick.ActualReqDate;


            pick.BOMComponentKey = oldPick.BOMComponentKey;

            pick.BOMReqQty = oldPick.BOMReqQty;//BOM用量初始化为0

            pick.StdReqQty = oldPick.StdReqQty;
            pick.ActualReqQty = oldPick.ActualReqQty;//实际需求数保持与标准用量数量相同
            pick.ConsignProcessItemSrc = oldPick.ConsignProcessItemSrc;

            pick.FixedScrap = oldPick.FixedScrap;
            pick.WasteRate = oldPick.WasteRate;
            pick.IsAutoCreate = true;
            pick.IsCheckATP = oldPick.IsCheckATP;
            pick.IsControlSupplier = oldPick.IsControlSupplier;
            pick.IsDiffentLotCtl = oldPick.IsDiffentLotCtl;
            pick.IsIssueOrgFixed = oldPick.IsIssueOrgFixed;
            pick.IsOverIssue = oldPick.IsOverIssue;
            pick.MaterialType = oldPick.MaterialType;
            pick.StandardMaterialScale = oldPick.StandardMaterialScale;
            pick.FixedMaterialNum = oldPick.FixedMaterialNum;

            pick.IsProjectTask = oldPick.IsProjectTask;
            pick.ProjectKey = oldPick.ProjectKey;
            pick.TaskKey = oldPick.TaskKey;
            
            pick.IssueStyle = oldPick.IssueStyle;

            pick.JIT = oldPick.JIT;
            pick.OverlapRate = oldPick.OverlapRate;

            pick.OwnerOrgKey = oldPick.OwnerOrgKey;
            pick.PickLineNo = oldPick.PickLineNo;
            pick.QtyType = oldPick.QtyType;

            pick.QPA = oldPick.QPA;
            pick.PicKHeadKey = oldPick.PicKHeadKey;
            pick.ScrapType = oldPick.ScrapType;

            pick.QtyType = oldPick.QtyType;
            pick.SubcItemSrcType = oldPick.SubcItemSrcType;


            pick.SupplyBinKey = oldPick.SupplyBinKey;
            pick.SupplyOrgKey = oldPick.SupplyOrgKey;
            pick.SupplyStyle = oldPick.SupplyStyle;
            pick.SupplyWhKey = oldPick.SupplyWhKey;
            //设置齐套标志=收货开立或者收货审核
            pick.RcvApproveSetCheck = oldPick.RcvApproveSetCheck;
            pick.RcvSaveSetCheck = oldPick.RcvSaveSetCheck;
            #endregion

            #region 计量单位赋值
            pick.IssueUOMKey = oldPick.IssueUOMKey;
            pick.IsWholeSetIssue = oldPick.IsWholeSetIssue;
            pick.IUToIBURate = oldPick.IUToIBURate;
            pick.IssueBaseUOMKey = oldPick.IssueBaseUOMKey;

            pick.CoUOMKey = oldPick.CoUOMKey;
            pick.CostBaseUOMKey = oldPick.CostBaseUOMKey;
            pick.CUToCBURate = oldPick.CUToCBURate;

            pick.RcvUOMKey = oldPick.RcvUOMKey;
            pick.RcvBaseUOMKey = oldPick.RcvBaseUOMKey;
            pick.SUToSBURate = oldPick.SUToSBURate;
            pick.IBUToCBURate = oldPick.IBUToCBURate;
            pick.IBUToSBURate = oldPick.IBUToSBURate;

            pick.DescFlexField = oldPick.DescFlexField;
            pick.Remarks = oldPick.Remarks;
            #endregion

        }


    }
}
