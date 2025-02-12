using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using UFIDA.U9.Base;
using UFIDA.U9.Base.Profile;
using UFIDA.U9.Base.Profile.Proxy;
using UFIDA.U9.Complete.RCVRpt;
using UFIDA.U9.InvDoc.WhInit;
using UFIDA.UBF.MD.Business;
using UFSoft.UBF.PL;

namespace YY.U9.Cust.LI.AppPlugIn
{
    /// <summary>
    /// 公共通用类
    /// </summary>
    class Common
    {
        #region MES、WMS相关

        //是否接口流程
        public readonly static string I_PROFILE_CODE = "Z001";
        //MES服务地址
        public readonly static string S_PROFILE_CODE = "Z002";
        //WMS/MES身份秘钥
        public readonly static string IDENTITYKEY = "Z003";
        //是否箱号校验
        public readonly static string BOX_CHECK = "Z004";
        //料品前缀（电池）
        public readonly static string ITEM_PC_PRE = "Z005";
        //料品前缀（组件）
        public readonly static string ITEM_PM_PRE = "Z006";
        //存储地点（电池）
        public readonly static string WH_PC_PRE = "Z007";
        //存储地点（组件）
        public readonly static string WH_PM_PRE = "Z008";
        //传递到外部系统的料号前缀集合
        public readonly static string ITEM_LIST_PRE = "Z009";
        //是否写日志
        public readonly static string IS_WRITE_LOG = "Z999";
        //物料服务器地址
        public readonly static string ITEMMASTER_SV_URL = "/Material/SaveMaterial";
        public readonly static string ITEMMASTER_SV_DEL_URL = "/Material/DeleteMaterial";
        //客户服务地址
        public readonly static string CUSTOMER_SV_URL = "/Customer/SaveCusInfo";
        public readonly static string CUSTOMER_SV_DEL_URL = "/Customer/DeleteCusInfo";
        //供应商服务地址
        public readonly static string SUPPLIER_SV_URL = "/Supplier/SaveSupInfo";
        public readonly static string SUPPLIER_SV_DEL_URL = "/Supplier/DeleteSupInfo";
        //BOM服务地址
        public readonly static string BOMMASTER_SV_URL = "/Bom/SaveBomInfo";
        public readonly static string BOMMASTER_SV_DEL_URL = "/Bom/DeleteBomInfo";
        //生产订单服务地址
        public readonly static string MO_SV_URL = "/PMC/SaveWOInfo";
        public readonly static string MO_SV_DEL_URL = "/PMC/DeleteWOInfo";
        public readonly static string MO_SV_OPEN_URL = "/PMC/OpenWOInfo";
        public readonly static string MO_SV_CLOSE_URL = "/PMC/CloseWOInfo";
        public readonly static string MO_SV_START_URL = "/PMC/StartWOInfo";
        public readonly static string MO_SV_CANCEL_URL = "/PMC/CancelWOInfo";
        //材料出库服务地址
        public readonly static string ISSUE_SV_URL = "/OS/SavePickInfo";
        //材料入库服务地址
        public readonly static string ISSUERTN_SV_URL = "/OS/SaveRomInfo";
        //收货单服务地址（入库）
        public readonly static string RCV_SV_URL = "/OS/SaveIChInfo";
        public readonly static string RCV_SV_DEL_URL = "/OS/DeleteIChInfo";
        //采购退货服务地址（出库）
        public readonly static string RCVRTN_SV_URL = "/OS/SaveOSInfo";
        //出货计划服务地址
        public readonly static string SHIPPLAN_SV_URL = "/OS/SaveOSFInfo";
        //退回处理服务地址（销售退货入库）
        public readonly static string RMA_SV_URL = "/OS/SaveOSFInfo";
        //杂收服务地址（入库）
        public readonly static string MISCRCVTRANS_SV_URL = "/OS/SaveReceiveSendInfo";
        //杂发服务地址（出库）
        public readonly static string MISCSHIP_SV_URL = "/OS/SaveReceiveSendInfo";
        //调出服务地址
        public readonly static string TRANSOUT_SV_URL = "/OS/SaveAllocationInfo";
        //调入服务地址
        public readonly static string TRANSIN_SV_URL = "/OS/SaveAllocationInfo";
        //形态转换服务地址
        public readonly static string TRANSFERFORM_SV_URL = "/OS/SaveConversionInfo";
        //委外发料（出库）
        public readonly static string PMISSUEDOC_SV_URL = "/OS/SaveOSInfo";
        //委外退料（入库）
        public readonly static string PMISSUEDOCRTN_SV_URL = "/OS/SaveIChInfo";

        #endregion

        #region OA相关

        //是否OA接口流程
        public static string OA_IPROFILE_CODE = "Z021";
        //OA地址
        public static string OA_SPROFILE_CODE = "Z022";

        public static string OA_USERNAME = "Z023";

        public static string OA_PWD = "Z024";

        /*
         * 有单据类型的单据，单据对应OA的服务地址存放在单据类型的私有字段1上
         */

        //获取token
        public static string OA_TOKEN = "/rest/token";
        //厂商价目表PurPriceList
        public static string OA_PURPRICELIST = "/rest/oa/ERP2OA_CGJMB001";

        ////请款单
        //public static string OA_REQFUND = "/rest/oa/ERP2OA_FKD001";
        ////生产相关采购申请
        //public static string OA_PRA = "/rest/oa/ERP2OA_SCCGSQ001";
        ////供应链非生产采购申请
        //public static string OA_PRB = "/rest/oa/ERP2OA_FSCCGSQ001";
        ////行政类采购申请
        //public static string OA_PRC = "/rest/oa/ERP2OA_XZCGSQ001";
        ////采购订单PO
        //public static string OA_PO = "/rest/oa/ERP2OA_CGDD001";
        ////出货计划ShipPlan
        //public static string OA_SHIPPLAN = "/rest/oa/ERP2OA_CHJH001";
        ////厂商价表调整单PurPriceAdjustment
        //public static string OA_PURPRICEADJUST = "/rest/oa/ERP2OA_CSJGTZ001";

        #endregion

        /// <summary>
        /// 获取参数设置
        /// </summary>
        /// <param name="profileCode">参数设置编码</param>
        /// <param name="org">组织ID</param>
        /// <returns></returns>
        public static string GetProfileValue(string profileCode, long org)
        {
            string profileValue = "";
            GetProfileValueProxy getProfileValueProxy = new GetProfileValueProxy();
            getProfileValueProxy.ProfileCode = profileCode;
            getProfileValueProxy.ProfileOrg = org;
            PVDTOData pVDTOData = new PVDTOData();
            pVDTOData = getProfileValueProxy.Do();
            if (pVDTOData != null)
            {
                if (string.IsNullOrEmpty(pVDTOData.ProfileValue) == false)
                {
                    profileValue = pVDTOData.ProfileValue;
                }
            }
            return profileValue;
        }

        /// <summary>
        /// 是否写日志
        /// </summary>
        /// <returns></returns>
        public static string IsWriteLog()
        {
            string rtnValue = "1";
            string profileValue = GetProfileValue(IS_WRITE_LOG, Context.LoginOrg.ID);
            if (profileValue == "0" || profileValue.ToLower() == "false")  //不写日志
            {
                rtnValue = "0";
            }
            return rtnValue;
        }


        /// <summary>
        /// 特殊字符处理
        /// </summary>
        /// <param name="strJson"></param>
        /// <returns></returns>
        public static string ReplaceString(string strJson)
        {
            if (string.IsNullOrEmpty(strJson))
            {
                return strJson;
            }
            if (strJson.Contains("\\"))
            {
                strJson = strJson.Replace("\\", "\\\\");
            }
            if (strJson.Contains("\'"))
            {
                strJson = strJson.Replace("\'", "\\\'");
            }
            if (strJson.Contains("\""))
            {
                strJson = strJson.Replace("\"", "\\\"");
            }
            if (strJson.Contains("\t"))
            {
                strJson = strJson.Replace("\t", " ");
            }
            if (strJson.Contains("&"))
            {
                strJson = strJson.Replace("&", " ");
            }
            //去掉字符串的回车换行符
            strJson = Regex.Replace(strJson, @"[\n\r]", " ");
            strJson = strJson.Trim();
            return strJson;
        }

        /// <summary>
        /// 更新参数设置值
        /// </summary>
        /// <param name="profileCode">参数设置编码</param>
        /// <param name="pValue">更新值</param>
        /// <param name="org">组织ID</param>
        public static void ModifyProfileValue(string profileCode, string pValue, long org)
        {
            ProfileValue profileValue = ProfileValue.Finder.Find("Profile.Code=@Code,", new OqlParam[] { new OqlParam(profileCode) });
            profileValue.Value = pValue;
        }

        /// <summary>
        /// 获取枚举值
        /// </summary>
        /// <param name="enumTypeCode">枚举类型编码</param>
        /// <param name="code">编码</param>
        /// <param name="name">名称</param>
        /// <param name="value">值</param>
        public static ExtEnumValue GetEnumValue(string enumTypeCode, string code, string name, int value)
        {
            StringBuilder oqlBuilder = new StringBuilder();
            oqlBuilder.Append("ExtEnumType.Code='").Append(enumTypeCode).Append("'");
            if (string.IsNullOrEmpty(code) == false)
            {
                oqlBuilder.Append(" and Code='").Append(code).Append("'");
            }
            if (string.IsNullOrEmpty(name) == false)
            {
                oqlBuilder.Append(" and Name='").Append(name).Append("'");
            }
            if (value > -1)
            {
                oqlBuilder.Append(" and EValue=").Append(value);
            }
            if (string.IsNullOrEmpty(code) && string.IsNullOrEmpty(name) && value < 0)
            {
                return null;
            }
            ExtEnumValue extEnumValue = ExtEnumValue.Finder.Find(oqlBuilder.ToString(), null);
            return extEnumValue;
        }

        /// <summary>
        /// 获取枚举值，没找到返回-1
        /// </summary>
        /// <param name="enumTypeCode">枚举类型编码</param>
        /// <param name="code">编码</param>
        /// <param name="name">名称</param>
        /// <param name="retValue">返回值</param>
        public static ExtEnumValue GetEnumValue(string enumTypeCode, string code, string name, ref int retValue)
        {
            StringBuilder oqlBuilder = new StringBuilder();
            oqlBuilder.Append("ExtEnumType.Code='").Append(enumTypeCode).Append("'");
            if (string.IsNullOrEmpty(code) == false)
            {
                oqlBuilder.Append(" and Code='").Append(code).Append("'");
            }
            if (string.IsNullOrEmpty(name) == false)
            {
                oqlBuilder.Append(" and Name='").Append(name).Append("'");
            }
            ExtEnumValue extEnumValue = ExtEnumValue.Finder.Find(oqlBuilder.ToString(), null);
            if (extEnumValue != null)
            {
                retValue = extEnumValue.EValue;
            }
            return extEnumValue;
        }

        /// <summary>
        /// 设置MES登录数据
        /// </summary>
        /// <param name="idEntityKey"></param>
        /// <param name="orgCode"></param>
        /// <param name="actionType"></param>
        /// <param name="userCode"></param>
        /// <param name="materialType">物料形态（组件、电池）工厂识别码：M-组件、C-电池</param>
        /// <returns></returns>
        public static string SetBusData(string idEntityKey, string orgCode, string actionType, string userCode, string materialType)
        {
            //甲方特殊要求 mes账号统一用yonyou
            userCode = "yonyou";
            string busData = "\"BusData\":{";
            busData = busData + "\"IdEntityKey\": \"" + idEntityKey + "\",";
            busData = busData + "\"OrgCode\": \"" + orgCode + "\",";
            busData = busData + "\"ExecType\": \"" + actionType + "\",";
            busData = busData + "\"FactoryCode\": \"" + materialType + "\",";
            busData = busData + "\"UserName\": \"" + userCode + "\"";
            busData = busData + "},";

            return busData;
        }

        /// <summary>
        /// 计算功率
        /// </summary>
        /// <param name="power">mes传入的功率</param>
        /// <param name="lotCode">批号</param>
        /// <param name="itemArea">面积</param>
        /// <param name="itemCode">料号</param>
        /// <param name="qty">数量</param>
        /// <param name="prePCItem">电池料号前缀</param>
        /// <param name="prePMItem">组件料号前缀</param>
        /// <returns></returns>
        public static decimal CalculatePower(decimal power, string lotCode, decimal itemArea, string itemCode, decimal qty, string prePCItem, string prePMItem)
        {
            if (power != 0)
            {
                return power;
            }
            if (itemCode.StartsWith(prePCItem))//电池
            {
                //功率 = 效率*面积/1000000
                decimal singlePower = 0;
                if (lotCode.Length >= 5)
                {
                    string strPower = lotCode.Substring(2, 3);
                    try
                    {
                        singlePower = Convert.ToDecimal(strPower);
                    }
                    catch (Exception ex)
                    {
                        string meses = ex.Message;
                    }
                }
                //总功率 = 功率 * 数量(功率计算之后四舍五入再乘以数量)
                power = Math.Round(singlePower * itemArea / 1000000M, 2, MidpointRounding.AwayFromZero) * qty;
            }
            if (itemCode.StartsWith(prePMItem))//组件
            {
                decimal singlePower = 0;
                if (lotCode.Length >= 5)
                {
                    string strPower = lotCode.Substring(2, 3);
                    try
                    {
                        singlePower = Convert.ToDecimal(strPower);
                    }
                    catch (Exception ex)
                    {
                        string mes = ex.Message;
                    }
                }
                //总功率 = 功率 * 数量
                power = singlePower * qty;
            }
            return power;
        }

        /// <summary>
        /// 根据箱号匹配 成品入库单 期初开账
        /// </summary>
        /// <param name="boxNo">箱号 已空格分开</param>
        /// <param name="itemCode">料号</param>
        /// <param name="lotNo">批号</param>
        /// <param name="grade">等级</param>
        /// <param name="whCode">存储地点</param>
        /// <param name="msg">匹配到结果的信息</param>
        /// <param name="msg">错误信息（重复箱号、没匹配到的箱号）</param>
        /// <returns>匹配到T 没匹配到F </returns>
        public static decimal CheckBoxNo(string boxNos, long orgID, string itemCode, string lotNo, int gradeValue, string whCode, ref string resultMsg, ref string errMsg, ref Dictionary<string, decimal> boxInfoDic)
        {
            Regex reg = new Regex(" +");//中间随便多少空格
            decimal storeQty = 0m;//剩余总数量
            //Dictionary<string, decimal> boxInfoDic = new Dictionary<string, decimal>(); //箱号与数量的键值对

            //if (string.IsNullOrEmpty(boxNos))
            //{
            //    errMsg = "箱号格式不正确或为空，请检查";
            //    return storeQty;
            //}
            string[] boxs = reg.Replace(boxNos.Trim(), " ").Split(' ');//多个空格替换成一个空格 方便拆分
            List<string> boxListRemain = new List<string>();
            foreach (string box in boxs)
            {
                //重复返回并报错
                if (boxListRemain.Contains(box))
                {
                    errMsg = "重复单号：" + box;
                    return storeQty;
                }
                boxListRemain.Add(box);
            }

            boxNos = "'" + reg.Replace(boxNos.Trim(), "','") + "'";//左右去空格后替换空格
            StringBuilder oql = new StringBuilder();

            #region 匹配成品入库单
            oql.AppendFormat(" 1=1 ");
            oql.AppendFormat(" and RcvRptDoc.Org = {0}", orgID);//单据组织
            oql.AppendFormat(" and DescFlexField.PrivateDescSeg4 in ({0}) ", boxNos);//单据箱号
            RcvRptDocLine.EntityList rcvRptDocLines = RcvRptDocLine.Finder.FindAll(oql.ToString(), null);
            resultMsg += "<br> 成品入库单匹配信息：<br>";

            foreach (RcvRptDocLine rcvRptLine in rcvRptDocLines)
            {
                if (null != rcvRptLine.Item && rcvRptLine.Item.Code == itemCode
                    && null != rcvRptLine.RcvLotMaster && rcvRptLine.RcvLotMaster.Code == lotNo
                    && null != rcvRptLine.Grade && rcvRptLine.Grade.Value == gradeValue
                    && null != rcvRptLine.Wh && rcvRptLine.Wh.Code == whCode)
                {
                    //剩余数量 = 入库数量(库存单位)-累计出库数量(库存单位)
                    storeQty += (rcvRptLine.RcvQtyByWhUOM - rcvRptLine.TotalAntiRcvQtyByWhUOM);
                    resultMsg += "箱号：" + rcvRptLine.DescFlexField.PrivateDescSeg4 + "数量：" + (rcvRptLine.RcvQtyByWhUOM - rcvRptLine.TotalAntiRcvQtyByWhUOM) + "\n";
                    boxInfoDic.Add(rcvRptLine.DescFlexField.PrivateDescSeg4, rcvRptLine.RcvQtyByWhUOM - rcvRptLine.TotalAntiRcvQtyByWhUOM);
                    boxListRemain.Remove(rcvRptLine.DescFlexField.PrivateDescSeg4);//排除找到的 留下没找到的箱号
                }
            }
            #endregion 匹配成品入库单

            #region 匹配库存期初开账
            //没匹配到成品入库
            oql.Clear();
            oql.AppendFormat(" 1=1 ");
            oql.AppendFormat(" and WhInit.Org = {0}", orgID);//单据组织
            oql.AppendFormat(" and DescFlexSegments.PrivateDescSeg1 in  ({0}) ", boxNos);
            WhInitLine.EntityList whInitLines = WhInitLine.Finder.FindAll(oql.ToString(), null);
            resultMsg += "库存期初开账匹配信息：<br>";
            foreach (WhInitLine whInitLine in whInitLines)
            {
                if (null != whInitLine.ItemInfo && whInitLine.ItemInfo.ItemCode == itemCode
                    && null != whInitLine.LotMaster && whInitLine.LotMaster.LotCode == lotNo
                    && null != whInitLine.ItemInfo.ItemGrade && whInitLine.ItemInfo.ItemGrade.Value == gradeValue
                    && null != whInitLine.Wh && whInitLine.Wh.Code == whCode)
                {
                    storeQty += whInitLine.SUQty;
                    resultMsg += "箱号：" + whInitLine.DescFlexSegments.PrivateDescSeg1 + "数量：" + whInitLine.SUQty + "<br>";
                    boxInfoDic.Add(whInitLine.DescFlexSegments.PrivateDescSeg1, whInitLine.SUQty);
                    boxListRemain.Remove(whInitLine.DescFlexSegments.PrivateDescSeg1);//排除找到的 留下没找到的箱号
                }
            }
            #endregion 匹配库存期初开账
            if (boxListRemain.Count > 0)
            {
                errMsg = "<br> 这些箱号：" + string.Join("|", boxListRemain.ToArray()) + " 没有匹配到信息，请检查！！！<br>";
            }
            return storeQty;
        }
    }
}