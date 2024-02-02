using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;
using NUnit.Core;
using NUnit.Framework;
using UFSoft.UBF.Util.Context;
using www.ufida.org.EntityData;
using UFSoft.UBF.Exceptions1;
using UFSoft.UBF.Business.BusinessEntity;
using UFSoft.UBF.Exceptions; 
using UFSoft.UBF.PL.Engine;
using UFSoft.UBF.Service;

namespace API.TransferIn.Test
{
    //[TestFixture]
    public class TransferInCreateSVTest
    {       
        //[Test]
        public static void TestCreateDo()
        {
            //try
            //{
                UFIDAU9ISVTransferInISVICommonCreateTransferInSVClient client = new UFIDAU9ISVTransferInISVICommonCreateTransferInSVClient();

                UFIDAU9ISVTransferInISVIC_TransferInDTOData[] Boms;
                List<UFIDAU9ISVTransferInISVIC_TransferInDTOData> listBom = new List<UFIDAU9ISVTransferInISVIC_TransferInDTOData>();
                List<UFIDAU9ISVTransferInISVIC_TransInLineDTOData> listBomLine = new List<UFIDAU9ISVTransferInISVIC_TransInLineDTOData>();
                List<UFIDAU9ISVTransferInISVIC_TransInSubLineDTOData> listBomSubline = new List<UFIDAU9ISVTransferInISVIC_TransInSubLineDTOData>();

                //头
                UFIDAU9ISVTransferInISVIC_TransferInDTOData Bom = new UFIDAU9ISVTransferInISVIC_TransferInDTOData();
                Bom.m_transInDocType = new UFIDAU9CBOPubControllerCommonArchiveDataDTOData();
                Bom.m_transInDocType.m_code = "TransIn002";//单据类型
                Bom.m_transferType = 0;//调入类型 0为一步式 1为两步式
                Bom.m_org = new UFIDAU9CBOPubControllerCommonArchiveDataDTOData();
                Bom.m_org.m_code = "All001";
                Bom.m_businessDate = new DateTime(2010, 9, 17);
                Bom.m_memo = "sundt1";
                Bom.m_transInLines = new UFIDAU9ISVTransferInISVIC_TransInLineDTOData[] { };
                Bom.sysState = ObjectState.Inserted;

                //行
                UFIDAU9ISVTransferInISVIC_TransInLineDTOData Bom_line = new UFIDAU9ISVTransferInISVIC_TransInLineDTOData();
                Bom_line.m_itemInfo = new UFIDAU9CBOSCMItemItemInfoData();
                Bom_line.m_itemInfo.m_itemCode = "Item001";//料品                
                Bom_line.m_storeUOMQty = 10;//调入数量
                Bom_line.m_costUOMQty = 10;  //成本数量             
                Bom_line.m_transInWh = new UFIDAU9CBOPubControllerCommonArchiveDataDTOData();
                Bom_line.m_transInWh.m_code = "WH001";//存储地点
                Bom_line.m_lotInfo = new UFIDAU9CBOSCMPropertyTypesLotInfoData();
                Bom_line.m_lotInfo.m_lotCode = "API001";
                Bom_line.m_lotInfo.m_disabledDatetime = new DateTime(2013, 4, 4);
                Bom_line.m_lotInfo.m_lotMaster = new UFIDAU9BasePropertyTypesBizEntityKeyData();
                Bom_line.m_lotInfo.m_lotMaster.m_entityID = 1001007189629053;
                Bom_line.sysState = ObjectState.Inserted;
                Bom_line.m_transInSubLines = new UFIDAU9ISVTransferInISVIC_TransInSubLineDTOData[] { };

                //子行
                UFIDAU9ISVTransferInISVIC_TransInSubLineDTOData Bom_subLine = new UFIDAU9ISVTransferInISVIC_TransInSubLineDTOData();
                Bom_subLine.m_transOutOrg = new UFIDAU9CBOPubControllerCommonArchiveDataDTOData();
                Bom_subLine.m_transOutOrg.m_code = "All001";
                Bom_subLine.m_transOutWh = new UFIDAU9CBOPubControllerCommonArchiveDataDTOData();
                Bom_subLine.m_transOutWh.m_code = "WH002";
                Bom_subLine.m_lotInfo = new UFIDAU9CBOSCMPropertyTypesLotInfoData();
                Bom_subLine.m_lotInfo.m_lotCode = "API001";
                Bom_subLine.m_lotInfo.m_disabledDatetime = new DateTime(2013,4,4);
                Bom_subLine.m_lotInfo.m_lotMaster = new UFIDAU9BasePropertyTypesBizEntityKeyData();
                Bom_subLine.m_lotInfo.m_lotMaster.m_entityID = 1001007189629053;
                Bom_subLine.sysState = ObjectState.Inserted;

                listBomSubline.Add(Bom_subLine);//加载子行
                listBomLine.Add(Bom_line);//加载行             

                Bom_line.m_transInSubLines = listBomSubline.ToArray();

                Bom.m_transInLines = listBomLine.ToArray();

                listBom.Add(Bom);
                Boms = listBom.ToArray();

                // 返回参数：消息数组
                MessageBase[] returnMsg;
                UFIDAU9CBOPubControllerCommonArchiveDataDTOData[] returnVal;
                ThreadContext context = TransferInCreateSVTest.CreateContextObj();// 入口参数：线程上下文              


                // 服务调用
                returnVal = client.Do(out returnMsg, context, Boms);
                Console.WriteLine("创建成功");

            //}
            //catch (Exception err)
            //{
            //    if (err is System.ServiceModel.FaultException<UFSoft.UBF.Service.ServiceException>)
            //    {
            //        Console.WriteLine((((System.ServiceModel.FaultException<UFSoft.UBF.Service.ServiceException>)(err)).Detail).StackTrace);
            //        Console.WriteLine((((System.ServiceModel.FaultException<UFSoft.UBF.Service.ServiceException>)(err)).Detail).Message);
            //    }
            //    else
            //    {
            //        Console.WriteLine(err.StackTrace); //堆栈段异常
            //        Console.WriteLine(err.Message); //异常信息
            //    }
            //}
        }

        private static ThreadContext CreateContextObj()
        {
            // 实例化应用上下文对象
            ThreadContext thContext = new ThreadContext();

            System.Collections.Generic.Dictionary<object, object> ns = new Dictionary<object, object>();
            ns.Add("OrgID", "1001007094250320");//code=All001
            ns.Add("UserID", "1001007094251326");//code=API001
            ns.Add("CultureName", "zh-CN");
            ns.Add("EnterpriseID", "001");//系统管理平台里的ID
            ns.Add("EnterpriseName", "");
            ns.Add("DefaultCultureName", "zh-CN");
            ns.Add("Support_CultureNameList", "zh-CN");
            thContext.nameValueHas = ns;

            return thContext;
        }
    }
}        