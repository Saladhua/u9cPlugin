using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using UFIDA.U9.CBO.Pub.Controller;
using UFIDA.U9.ISV.GL.ISVGLImportSV;
using UFIDA.U9.ISV.GL.ISVGLImportSV.Proxy;
using UFIDA.U9.UI.PDHelper;
using UFSoft.UBF.UI.ControlModel;
using UFSoft.UBF.UI.FormProcess;
using UFSoft.UBF.UI.MD.Runtime;
using UFSoft.UBF.UI.WebControlAdapter;
using UFSoft.UBF.Util.DataAccess;



/***********************************************************************************************
 * Form ID: 
 * UIFactory Auto Generator 
 ***********************************************************************************************/
namespace UFIDA.U9.Cust.JH.AnalyseDocTypeUIModel
{
    public partial class AnalyseUIFarmWebPart
    {
        #region Custome eventBind

        #endregion
        //BtnSave_Click...
        private void BtnSave_Click_Extend(object sender, EventArgs e)
        {
            //调用模版提供的默认实现.--默认实现可能会调用相应的Action.


            //需要把原来已经回写的单子筛选掉
            string isyiti_id = "";
            try
            {
                isyiti_id = this.Model.AnalyseHead.FocusedRecord["AccrualNo"].ToString();
            }
            catch (Exception ex)
            {
                throw new Exception("请选择单据类型");
            }
            BtnSave_Click_DefaultImpl(sender, e);
            #region 判断是否勾选预提
            string isyiti = "";
            DataTable dataTable = new DataTable();
            string set = "select IsYUTI from Cust_U9_AnalyseDocType where ID='" + isyiti_id + "'";
            DataSet dataSet = new DataSet();
            DataAccessor.RunSQL(DataAccessor.GetConn(), set, null, out dataSet);
            dataTable = dataSet.Tables[0];
            int datei = dataTable.Rows.Count;
            if (dataTable.Rows != null && dataTable.Rows.Count > 0)
            {
                isyiti = dataTable.Rows[0]["IsYUTI"].ToString();
            }
            if (isyiti == "True")
            {
                foreach (var item in this.Model.AnalyseHead_Analyseline.Records)
                {
                    if (item["ShipDocType"].ToString() == "标准出货单")
                    {
                        DataTable dataTable_1 = new DataTable();
                        string set_1 = "UPDATE SM_ShipLine SET DescFlexField_PrivateDescSeg29='已预提' WHERE ID='" + item["ForInID"].ToString() + "'";
                        DataSet dataSet_1 = new DataSet();
                        DataAccessor.RunSQL(DataAccessor.GetConn(), set_1, null, out dataSet_1);
                    }
                    if (item["ShipDocType"].ToString() == "退回处理单")
                    {
                        DataTable dataTable_2 = new DataTable();
                        string set_2 = "UPDATE SM_RMALine SET DescFlexField_PrivateDescSeg29 = '已预提' WHERE ID = '" + item["ForInID"].ToString() + "'";
                        DataSet dataSet_2 = new DataSet();
                        DataAccessor.RunSQL(DataAccessor.GetConn(), set_2, null, out dataSet_2);
                    }
                    OnWriteBack268_Click_DefaultImpl(sender, e);
                }
            }
            else
            {
                foreach (var item in this.Model.AnalyseHead_Analyseline.Records)
                {
                    if (item["ShipDocType"].ToString() == "标准出货单")
                    {
                        DataTable dataTable_3 = new DataTable();
                        string set_3 = "UPDATE SM_ShipLine SET DescFlexField_PrivateDescSeg30='已计提' WHERE ID='" + item["ForInID"].ToString() + "'";
                        DataSet dataSet_3 = new DataSet();
                        DataAccessor.RunSQL(DataAccessor.GetConn(), set_3, null, out dataSet_3);
                    }
                    if (item["ShipDocType"].ToString() == "退回处理单")
                    {
                        DataTable dataTable_4 = new DataTable();
                        string set_4 = "UPDATE SM_RMALine SET DescFlexField_PrivateDescSeg30 = '已计提' WHERE ID = '" + item["ForInID"].ToString() + "'";
                        DataSet dataSet_4 = new DataSet();
                        DataAccessor.RunSQL(DataAccessor.GetConn(), set_4, null, out dataSet_4);
                    }
                }
            }
            #endregion
        }

        //BtnCancel_Click...
        private void BtnCancel_Click_Extend(object sender, EventArgs e)
        {
            //调用模版提供的默认实现.--默认实现可能会调用相应的Action.


            BtnCancel_Click_DefaultImpl(sender, e);
        }

        //BtnAdd_Click...
        private void BtnAdd_Click_Extend(object sender, EventArgs e)
        {
            //调用模版提供的默认实现.--默认实现可能会调用相应的Action.


            BtnAdd_Click_DefaultImpl(sender, e);
        }

        //BtnDelete_Click...
        private void BtnDelete_Click_Extend(object sender, EventArgs e)
        {
            //调用模版提供的默认实现.--默认实现可能会调用相应的Action.
            //把值还原

            string isyiti_id = "";
            try
            {
                isyiti_id = this.Model.AnalyseHead.FocusedRecord["AccrualNo"].ToString();
            }
            catch (Exception ex)
            {
                throw new Exception("请选择单据类型");
            }
            string isyiti = "";
            DataTable dataTable = new DataTable();
            string set = "select IsYUTI from Cust_U9_AnalyseDocType where ID='" + isyiti_id + "'";
            DataSet dataSet = new DataSet();
            DataAccessor.RunSQL(DataAccessor.GetConn(), set, null, out dataSet);
            dataTable = dataSet.Tables[0];
            int datei = dataTable.Rows.Count;
            if (dataTable.Rows != null && dataTable.Rows.Count > 0)
            {
                isyiti = dataTable.Rows[0]["IsYUTI"].ToString();
            }
            if (isyiti == "True") //预提
            {
                foreach (var item in this.Model.AnalyseHead_Analyseline.Records)
                {
                    if (item["ShipDocType"].ToString() == "标准出货单")
                    {
                        string set_1 = "UPDATE SM_ShipLine SET DescFlexField_PrivateDescSeg29='' WHERE ID='" + item["ForInID"].ToString() + "'";
                        DataAccessor.RunSQL(DataAccessor.GetConn(), set_1, null, out dataSet);
                    }
                    if (item["ShipDocType"].ToString() == "退回处理单")
                    {
                        string set_2 = "UPDATE SM_RMALine SET DescFlexField_PrivateDescSeg29 = '' WHERE ID = '" + item["ForInID"].ToString() + "'";
                        DataAccessor.RunSQL(DataAccessor.GetConn(), set_2, null, out dataSet);
                    }
                }
            }
            else
            {
                foreach (var item in this.Model.AnalyseHead_Analyseline.Records)
                {
                    if (item["ShipDocType"].ToString() == "标准出货单")
                    {
                        string set_1 = "UPDATE SM_ShipLine SET DescFlexField_PrivateDescSeg30='' WHERE ID='" + item["ForInID"].ToString() + "'";
                        DataAccessor.RunSQL(DataAccessor.GetConn(), set_1, null, out dataSet);
                    }
                    if (item["ShipDocType"].ToString() == "退回处理单")
                    {
                        string set_2 = "UPDATE SM_RMALine SET DescFlexField_PrivateDescSeg30 = '' WHERE ID = '" + item["ForInID"].ToString() + "'";
                        DataAccessor.RunSQL(DataAccessor.GetConn(), set_2, null, out dataSet);
                    }
                }
            }
            BtnDelete_Click_DefaultImpl(sender, e);
        }

        //BtnCopy_Click...
        private void BtnCopy_Click_Extend(object sender, EventArgs e)
        {
            //调用模版提供的默认实现.--默认实现可能会调用相应的Action.


            BtnCopy_Click_DefaultImpl(sender, e);
        }

        //BtnSubmit_Click...
        private void BtnSubmit_Click_Extend(object sender, EventArgs e)
        {
            //调用模版提供的默认实现.--默认实现可能会调用相应的Action.
            Approc();
            BtnSubmit_Click_DefaultImpl(sender, e);
        }

        //BtnApprove_Click...
        private void BtnApprove_Click_Extend(object sender, EventArgs e)
        {
            //调用模版提供的默认实现.--默认实现可能会调用相应的Action.

            Approc();
            BtnApprove_Click_DefaultImpl(sender, e);
        }
        //BtnUndoApprove
        private void BtnUndoApprove_Click_Extend(object sender, EventArgs e)
        {

            //调用模版提供的默认实现.--默认实现可能会调用相应的Action.
            Approc();
            BtnUndoApprove_Click_DefaultImpl(sender, e);
        }


        //BtnFind_Click...
        private void BtnFind_Click_Extend(object sender, EventArgs e)
        {
            //调用模版提供的默认实现.--默认实现可能会调用相应的Action.


            BtnFind_Click_DefaultImpl(sender, e);
        }

        //BtnList_Click...
        private void BtnList_Click_Extend(object sender, EventArgs e)
        {
            //调用模版提供的默认实现.--默认实现可能会调用相应的Action.


            BtnList_Click_DefaultImpl(sender, e);
        }

        //BtnFirstPage_Click...
        private void BtnFirstPage_Click_Extend(object sender, EventArgs e)
        {
            //调用模版提供的默认实现.--默认实现可能会调用相应的Action.


            BtnFirstPage_Click_DefaultImpl(sender, e);
        }

        //BtnPrevPage_Click...
        private void BtnPrevPage_Click_Extend(object sender, EventArgs e)
        {
            //调用模版提供的默认实现.--默认实现可能会调用相应的Action.


            BtnPrevPage_Click_DefaultImpl(sender, e);
        }

        //BtnNextPage_Click...
        private void BtnNextPage_Click_Extend(object sender, EventArgs e)
        {
            //调用模版提供的默认实现.--默认实现可能会调用相应的Action.


            BtnNextPage_Click_DefaultImpl(sender, e);
        }

        //BtnLastPage_Click...
        private void BtnLastPage_Click_Extend(object sender, EventArgs e)
        {
            //调用模版提供的默认实现.--默认实现可能会调用相应的Action.


            BtnLastPage_Click_DefaultImpl(sender, e);
        }

        //BtnAttachment_Click...
        private void BtnAttachment_Click_Extend(object sender, EventArgs e)
        {
            //调用模版提供的默认实现.--默认实现可能会调用相应的Action.


            BtnAttachment_Click_DefaultImpl(sender, e);
        }

        //BtnFlow_Click...
        private void BtnFlow_Click_Extend(object sender, EventArgs e)
        {
            //调用模版提供的默认实现.--默认实现可能会调用相应的Action.


            BtnFlow_Click_DefaultImpl(sender, e);
        }

        //BtnOutput_Click...
        private void BtnOutput_Click_Extend(object sender, EventArgs e)
        {
            //调用模版提供的默认实现.--默认实现可能会调用相应的Action.


            BtnOutput_Click_DefaultImpl(sender, e);
        }

        //BtnPrint_Click...
        private void BtnPrint_Click_Extend(object sender, EventArgs e)
        {
            //调用模版提供的默认实现.--默认实现可能会调用相应的Action.


            BtnPrint_Click_DefaultImpl(sender, e);
        }



        private void Btn_YUTI444_Click_Extend(object sender, EventArgs e)
        {
            //调用模版提供的默认实现.--默认实现可能会调用相应的Action.

            #region 判断期间是否有值
            string time = this.Model.AnalyseHead.FocusedRecord["During_Code"].ToString();
            if (string.IsNullOrEmpty(time))
            {
                throw new Exception("请选择期间");
            }
            #region 通过期间设置开始结束时间
            string year = time.Substring(0, 4);
            string month = time.Substring(5);
            DateTime stime = new DateTime(Convert.ToInt32(year), Convert.ToInt32(month), 01);
            DateTime etime = stime.AddMonths(1).AddDays(-1);
            string starttime = stime.ToString("yyyy-MM-dd");
            string endtime = etime.ToString("yyyy-MM-dd");
            #endregion
            #endregion
            string isyiti_id = "";
            try
            {
                isyiti_id = this.Model.AnalyseHead.FocusedRecord["AccrualNo"].ToString();
            }
            catch (Exception ex)
            {
                throw new Exception("请选择单据类型");
            }
            #region 判断是否勾选预提
            string isyiti = "";
            DataTable dataTable = new DataTable();
            string set = "select IsYUTI from Cust_U9_AnalyseDocType where ID='" + isyiti_id + "'";
            DataSet dataSet = new DataSet();
            DataAccessor.RunSQL(DataAccessor.GetConn(), set, null, out dataSet);
            dataTable = dataSet.Tables[0];
            int datei = dataTable.Rows.Count;
            if (dataTable.Rows != null && dataTable.Rows.Count > 0)
            {
                isyiti = dataTable.Rows[0]["IsYUTI"].ToString();
            }
            if (isyiti == "False")
            {
                return;
            }
            #endregion
            LineAdd_YUTI(starttime, endtime);
            Btn_YUTI444_Click_DefaultImpl(sender, e);
        }

        //OnAccrual928_Click...
        private void OnAccrual928_Click_Extend(object sender, EventArgs e)
        {
            //调用模版提供的默认实现.--默认实现可能会调用相应的Action.
            //计提计算
            string isyiti_id = "";
            try
            {
                isyiti_id = this.Model.AnalyseHead.FocusedRecord["AccrualNo"].ToString();
            }
            catch (Exception ex)
            {
                throw new Exception("请选择单据类型");
            }
            #region 判断是否勾选预提
            string isyiti = "";
            DataTable dataTable = new DataTable();
            string set = "select IsYUTI from Cust_U9_AnalyseDocType where ID='" + isyiti_id + "'";
            DataSet dataSet = new DataSet();
            DataAccessor.RunSQL(DataAccessor.GetConn(), set, null, out dataSet);
            dataTable = dataSet.Tables[0];
            int datei = dataTable.Rows.Count;
            if (dataTable.Rows != null && dataTable.Rows.Count > 0)
            {
                isyiti = dataTable.Rows[0]["IsYUTI"].ToString();
            }
            if (isyiti == "True")
            {
                throw new Exception("单据类型勾选预提");
            }
            #endregion
            LineAdd();
            OnAccrual928_Click_DefaultImpl(sender, e);
        }

        /// <summary>
        /// 确认回写
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        //OnWriteBack268_Click...
        private void OnWriteBack268_Click_Extend(object sender, EventArgs e)
        {
            //调用模版提供的默认实现.--默认实现可能会调用相应的Action.
            //确认回写
            string commissionltem = "";//可提成数
            string overdueDebit = "";//超期扣款
            string judgement = "";//差异
            //取值
            string interDate = "";//利率
            foreach (var item in this.Model.AnalyseHead.Records)
            {
                interDate = item["IntersetRate"] == null ? "0" : item["IntersetRate"].ToString();
            }
            if (this.Model.AnalyseHead_Analyseline.Records.Count == 0)
            {
                throw new Exception("请先计算");
            }
            foreach (var item in this.Model.AnalyseHead_Analyseline.Records)
            {
                commissionltem = item["commissionltem"] != null ? item["commissionltem"].ToString() : "0";
                overdueDebit = item["overdueDebit"] != null ? item["overdueDebit"].ToString() : "0";
                judgement = item["judgement"] != null ? item["judgement"].ToString() : "0";
                if (string.IsNullOrEmpty(judgement)) judgement = "0";
                if (string.IsNullOrEmpty(overdueDebit)) overdueDebit = "0";
                if (string.IsNullOrEmpty(commissionltem)) commissionltem = "0";
                //可提成数-超期扣款-差异调整额
                item["actualCommission"] = Math.Round(decimal.Parse(commissionltem) - decimal.Parse(overdueDebit) + decimal.Parse(judgement), 2);
                //item[""]=
            }
        }
        /// <summary>
        /// 生成凭证
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        //OnVoucher305_Click...
        private void OnVoucher305_Click_Extend(object sender, EventArgs e)
        {
            //调用模版提供的默认实现.--默认实现可能会调用相应的Action.

            //先判断状态防止误操作
            //string see123 = this.Model.AnalyseHead.FocusedRecord["State"].ToString();
            string org = PDContext.Current.OrgID;//组织
            if (this.Model.AnalyseHead.FocusedRecord["State"].ToString() != "2")
            {
                throw new Exception("审核通过才可以生成凭证");
                //return;
            }
            decimal sumCommissionltem = 0;//可提成数
            decimal sumOverdueDebit = 0;//超期扣款
            decimal sumActualCommission = 0;//实际应发提成
            string isyiti_id = "";
            try
            {
                isyiti_id = this.Model.AnalyseHead.FocusedRecord["AccrualNo"].ToString();
            }
            catch (Exception ex)
            {
                throw new Exception("请选择单据类型");
            }
            string isyiti = "";
            DataTable dataTable_1 = new DataTable();
            string set = "select IsYUTI from Cust_U9_AnalyseDocType where ID='" + isyiti_id + "'";
            DataSet dataSet_1 = new DataSet();
            DataAccessor.RunSQL(DataAccessor.GetConn(), set, null, out dataSet_1);
            dataTable_1 = dataSet_1.Tables[0];
            int datei = dataTable_1.Rows.Count;
            if (dataTable_1.Rows != null && dataTable_1.Rows.Count > 0)
            {
                isyiti = dataTable_1.Rows[0]["IsYUTI"].ToString();
            }
            DataTable dataTable = new DataTable();
            DataSet dataSet = new DataSet();
            if (isyiti == "True") //预提
            {
                #region 出货单提成求和
                List<Linedto> linedtos = new List<Linedto>();
                foreach (var item in this.Model.AnalyseHead_Analyseline.Records)
                {

                    Linedto linedto = new Linedto();
                    linedto.Seller = item["Operators"].ToString();
                    //linedto.Commissionltem = Convert.ToDecimal(item["Commissionltem"].ToString());
                    //linedto.OverdueDebit = Convert.ToDecimal(item["OverdueDebit"].ToString());
                    //linedto.ActualCommission = Convert.ToDecimal(item["ActualCommission"].ToString());
                    linedto.Commissionltem = decimal.Parse(item["TotalNMTC"].ToString());//Convert.ToDecimal(Int32.Parse());
                    sumCommissionltem = sumCommissionltem + linedto.Commissionltem;
                    linedtos.Add(linedto);
                }
                string name = "";
                for (int i = 0; i < linedtos.Count; i++)
                {
                    bool flag = false;
                    //每个元素都与其他这个元素前面的比较，如果前面没有，则添加，否则不添加
                    for (int z = 0; z < i; z++)
                    {
                        if (linedtos[z].Seller == linedtos[i].Seller)
                        {
                            flag = true;
                            break;
                        }
                    }
                    if (!flag)
                    {
                        name = name + linedtos[i].Seller + ",";
                    }
                }
                name = name.Substring(0, name.Length - 1);
                string see12313 = name.Split(',').Length.ToString();
                #endregion
                //生成凭证
                #region 生成凭证
                ISVGLImportSVProxy iSVGL = new ISVGLImportSVProxy();
                List<ISVImportVoucherDTOData> vDTOs = new List<ISVImportVoucherDTOData>();
                ISVImportVoucherDTOData vDTO = new ISVImportVoucherDTOData();//凭证维护头
                vDTO.AttachmentCount = 0;
                vDTO.CreateDate = DateTime.Now.AddMonths(-1);
                //记账期间
                vDTO.PostedPeriod = vDTO.CreateDate.ToString("yyyy-MM");
                //账簿
                vDTO.SOB = new CommonArchiveDataDTOData();
                vDTO.SOB.ID = 1002206140004887;
                vDTO.SOB.Code = "10";
                vDTO.SOB.Name = "君华主账簿";
                //凭证类型
                vDTO.VoucherCategory = new CommonArchiveDataDTOData();
                vDTO.VoucherCategory.ID = 1002206140140002;
                vDTO.VoucherCategory.Code = "01";//凭证类型	
                vDTO.VoucherCategory.Name = "记账凭证";//凭证类型	
                vDTO.VoucherCategory.SysState = new UFSoft.UBF.PL.Engine.ObjectState();
                //来源方式
                vDTO.VoucherDisplayCode = "(记)-797979";
                vDTO.SysState = new UFSoft.UBF.PL.Engine.ObjectState();
                #region 创建子数据
                List<ISVImportEntryDTOData> vDTO_lines = new List<ISVImportEntryDTOData>();
                ISVImportEntryDTOData vDTO_line1 = new ISVImportEntryDTOData();//借方
                                                                               //ISVImportEntryDTOData vDTO_line2 = new ISVImportEntryDTOData();//贷方
                #region 借方
                //摘要
                vDTO_line1.Abstracts = "计提提成凭证";
                //科目
                vDTO_line1.Account = new CommonArchiveDataDTOData();
                vDTO_line1.Account.Code = "660101|0|0|0301|0|0|0|0|0";
                //vDTO_line1.Account.Name = "工资|||内贸销售|||||";
                //vDTO_line1.Account.ID = 1002210220007967;

                //贷方金额(本币)
                //vDTO_line1.AccountedCr = 0;
                //借方金额(本币)
                //vDTO_line1.AccountedDr = 154;
                ////贷方金额(原币)
                //vDTO_line1.EnteredCr = 0;
                ////借方金额(原币)
                //vDTO_line1.EnteredDr = 0;
                ////贷方数量
                //vDTO_line1.AmountCr = 0;
                //借方金额(本币)
                vDTO_line1.AccountedDr = Math.Round(sumCommissionltem, 3);
                //vDTO_line1.EnteredDr = Math.Round(sumCommissionltem, 3);
                //vDTO_line1.AccountedDr = 0;
                //币种
                vDTO_line1.Currency = new CommonArchiveDataDTOData();
                vDTO_line1.Currency.ID = 1;
                vDTO_line1.Currency.Code = "C001";
                vDTO_line1.Currency.Name = "人民币";
                #endregion
                /////////////////////////////////////////////
                string[] s = name.Split(',');
                for (int i = 0; i < name.Split(',').Length; i++)
                {
                    ISVImportEntryDTOData vDTO_line2 = new ISVImportEntryDTOData();//贷方
                    string see777 = s[i];
                    //摘要
                    vDTO_line2.Abstracts = "计提提成凭证";
                    //科目
                    string sellerName = FindOperatorsCode(s[i]);

                    vDTO_line2.Account = new CommonArchiveDataDTOData();
                    #region 执行SQL

                    string set_2 = "SELECT A1.Code,A3.Code AS DeptCode FROM CBO_Operators A1 INNER JOIN CBO_Operators_Trl A2 ON A1.ID = A2.ID" +
                        " INNER JOIN CBO_Department A3 ON A3.ID = A1.Dept WHERE A2.Name = '" + sellerName + "' AND A1.Org = '" + org + "'";
                    DataAccessor.RunSQL(DataAccessor.GetConn(), set_2, null, out dataSet);
                    dataTable = dataSet.Tables[0];
                    //int datei = dataTable.Rows.Count;
                    string sellcode = "";
                    string deptcode = "";
                    if (dataTable.Rows != null && dataTable.Rows.Count > 0)
                    {
                        sellcode = dataTable.Rows[0]["Code"].ToString();
                        deptcode = dataTable.Rows[0]["DeptCode"].ToString();
                    }
                    #endregion
                    vDTO_line2.Account.Code = "224103|0|0|" + deptcode + "|" + sellcode + "|0|0|0";
                    //vDTO_line2.Account.Code = "224103|0|0|0301|2018120102|0|0|0|0";
                    //vDTO_line2.Account.Name = "个人|||内贸销售|胡学玲||||";
                    //vDTO_line2.Account.ID = 1002212050660012;
                    //vDTO_line2.m_account.m_name = "银行存款";
                    //sumCommissionltem = 0;//可提成数
                    //sumOverdueDebit = 0;//超期扣款
                    //sumActualCommission = 0;//实际应发提成
                    sumCommissionltem = linedtos.Where(x => x.Seller == s[i]).Sum(x => x.Commissionltem);
                    //贷方金额(本币)
                    vDTO_line2.AccountedCr = Math.Round(sumCommissionltem, 3);//赋值
                    //vDTO_line2.EnteredCr = Math.Round(sumCommissionltem, 3);//赋值
                    //vDTO_line2.AccountedCr = 0;//赋值
                    ////借方金额(本币)
                    //vDTO_line2.AccountedDr = 0;
                    ////贷方金额(原币)
                    //vDTO_line2.EnteredCr = 0;
                    ////借方金额(原币)
                    //vDTO_line2.EnteredDr = 0;
                    ////贷方数量
                    //vDTO_line2.AmountCr = 0;
                    //借方数量
                    //vDTO_line2.AccountedDr = 0;
                    //币种
                    vDTO_line2.Currency = new CommonArchiveDataDTOData();
                    vDTO_line2.Currency.ID = 1;
                    vDTO_line2.Currency.Code = "C001";
                    vDTO_line2.Currency.Name = "人民币";
                    vDTO_lines.Add(vDTO_line2);
                }
                #region TesT
                ////摘要
                //vDTO_line2.Abstracts = "计提提成凭证";
                ////科目
                //vDTO_line2.Account = new CommonArchiveDataDTOData();
                //vDTO_line2.Account.Code = "224103|0|0|0301|2018120102|0|0|0|0";
                //vDTO_line2.Account.Name = "个人|||内贸销售|胡学玲||||";
                //vDTO_line2.Account.ID = 1002212050660012;
                ////vDTO_line2.m_account.m_name = "银行存款";

                ////贷方金额(本币)
                //vDTO_line2.AccountedCr = 154;//赋值
                ////借方金额(本币)
                ////vDTO_line2.AccountedDr = 0;
                ////贷方金额(原币)
                //vDTO_line2.EnteredCr = 0;
                ////借方金额(原币)
                //vDTO_line2.EnteredDr = 0;
                ////贷方数量
                //vDTO_line2.AmountCr = 0;
                ////借方数量
                //vDTO_line2.AccountedDr = 0;

                ////币种
                //vDTO_line2.Currency = new CommonArchiveDataDTOData();
                //vDTO_line2.Currency.ID = 1;
                //vDTO_line2.Currency.Code = "C001";
                //vDTO_line2.Currency.Name = "人民币";

                #endregion
                vDTO_lines.Add(vDTO_line1);
                //vDTO_lines.Add(vDTO_line2);
                vDTO.Entries = vDTO_lines;
                vDTOs.Add(vDTO);
                iSVGL.ImportVoucherDTOs = vDTOs;
                //iSVGL.Do();

                List<ISVReturnVoucherDTOData> see = iSVGL.Do();
                string vDonNo = "";//凭证号
                foreach (var item in see)
                {
                    vDonNo = item.DocNo;
                }

                foreach (var item in this.Model.AnalyseHead.Records)
                {
                    item["VoucherDocNo"] = vDonNo;
                }
                string id = this.Model.AnalyseHead.FocusedRecord["ID"].ToString();
                string set_1 = "UPDATE Cust_JH_AnalyseHead SET VoucherDocNo='" + vDonNo + "' WHERE ID='" + id + "'";
                DataAccessor.RunSQL(DataAccessor.GetConn(), set_1, null, out dataSet);
                #endregion


                #endregion
            }
            else
            {
                #region 出货单提成求和
                List<Linedto> linedtos = new List<Linedto>();
                foreach (var item in this.Model.AnalyseHead_Analyseline.Records)
                {
                    //if (item["ShipDocType"].ToString() == "标准出货单")
                    //{
                    Linedto linedto = new Linedto();
                    linedto.Seller = item["Operators"].ToString();
                    //linedto.Commissionltem = Convert.ToDecimal(item["Commissionltem"].ToString());
                    //linedto.OverdueDebit = Convert.ToDecimal(item["OverdueDebit"].ToString());
                    //linedto.ActualCommission = Convert.ToDecimal(item["ActualCommission"].ToString());
                    linedto.Commissionltem = decimal.Parse(item["Commissionltem"].ToString());//Convert.ToDecimal(Int32.Parse());
                    linedto.OverdueDebit = decimal.Parse(item["OverdueDebit"].ToString());
                    linedto.ActualCommission = decimal.Parse(item["ActualCommission"].ToString());
                    sumCommissionltem = sumCommissionltem + linedto.Commissionltem;
                    sumOverdueDebit = sumOverdueDebit + linedto.OverdueDebit;
                    sumActualCommission = sumActualCommission + linedto.ActualCommission;
                    linedtos.Add(linedto);
                    // }
                }
                string name = "";
                for (int i = 0; i < linedtos.Count; i++)
                {
                    bool flag = false;
                    //每个元素都与其他这个元素前面的比较，如果前面没有，则添加，否则不添加
                    for (int z = 0; z < i; z++)
                    {
                        if (linedtos[z].Seller == linedtos[i].Seller)
                        {
                            flag = true;
                            break;
                        }
                    }
                    if (!flag)
                    {
                        name = name + linedtos[i].Seller + ",";
                    }
                }
                name = name.Substring(0, name.Length - 1);
                string see12313 = name.Split(',').Length.ToString();
                #endregion
                //生成凭证
                #region 生成凭证
                ISVGLImportSVProxy iSVGL = new ISVGLImportSVProxy();
                List<ISVImportVoucherDTOData> vDTOs = new List<ISVImportVoucherDTOData>();
                ISVImportVoucherDTOData vDTO = new ISVImportVoucherDTOData();//凭证维护头
                vDTO.AttachmentCount = 0;
                vDTO.CreateDate = DateTime.Now.AddMonths(-1);
                //记账期间
                vDTO.PostedPeriod = vDTO.CreateDate.ToString("yyyy-MM");
                //vDTO.PostedPeriod ="2022-11";
                //账簿
                vDTO.SOB = new CommonArchiveDataDTOData();
                vDTO.SOB.ID = 1002206140004887;
                vDTO.SOB.Code = "10";
                vDTO.SOB.Name = "君华主账簿";
                //凭证类型
                vDTO.VoucherCategory = new CommonArchiveDataDTOData();
                vDTO.VoucherCategory.ID = 1002206140140002;
                vDTO.VoucherCategory.Code = "01";//凭证类型	
                vDTO.VoucherCategory.Name = "记账凭证";//凭证类型	
                vDTO.VoucherCategory.SysState = new UFSoft.UBF.PL.Engine.ObjectState();
                //来源方式
                vDTO.VoucherDisplayCode = "(记)-797979";
                vDTO.SysState = new UFSoft.UBF.PL.Engine.ObjectState();
                #region 创建子数据
                List<ISVImportEntryDTOData> vDTO_lines = new List<ISVImportEntryDTOData>();
                ISVImportEntryDTOData vDTO_line1 = new ISVImportEntryDTOData();//借方
                                                                               //ISVImportEntryDTOData vDTO_line2 = new ISVImportEntryDTOData();//贷方
                #region 借方
                //摘要
                vDTO_line1.Abstracts = "计提提成凭证";
                //科目
                vDTO_line1.Account = new CommonArchiveDataDTOData();
                vDTO_line1.Account.Code = "660101|0|0|0301|0|0|0|0|0";
                //vDTO_line1.Account.Name = "工资|||内贸销售|||||";
                //vDTO_line1.Account.ID = 1002210220007967;

                //贷方金额(本币)
                //vDTO_line1.AccountedCr = 0;
                //借方金额(本币)
                //vDTO_line1.AccountedDr = 154;
                ////贷方金额(原币)
                //vDTO_line1.EnteredCr = 0;
                ////借方金额(原币)
                //vDTO_line1.EnteredDr = 0;
                ////贷方数量
                //vDTO_line1.AmountCr = 0;
                //借方金额(本币)
                vDTO_line1.AccountedDr = Math.Round(sumCommissionltem + sumOverdueDebit + sumActualCommission, 3);
                vDTO_line1.EnteredDr = Math.Round(sumCommissionltem + sumOverdueDebit + sumActualCommission, 3);
                //vDTO_line1.AccountedDr = 0;
                //币种
                vDTO_line1.Currency = new CommonArchiveDataDTOData();
                vDTO_line1.Currency.ID = 1;
                vDTO_line1.Currency.Code = "C001";
                vDTO_line1.Currency.Name = "人民币";
                #endregion
                /////////////////////////////////////////////
                string[] s = name.Split(',');
                for (int i = 0; i < name.Split(',').Length; i++)
                {
                    ISVImportEntryDTOData vDTO_line2 = new ISVImportEntryDTOData();//贷方
                    string see777 = s[i];
                    //摘要
                    vDTO_line2.Abstracts = "计提提成凭证";
                    //科目
                    string sellerName = FindOperatorsCode(s[i]);

                    vDTO_line2.Account = new CommonArchiveDataDTOData();
                    #region 执行SQL

                    string set_2 = "SELECT A1.Code,A3.Code AS DeptCode FROM CBO_Operators A1 INNER JOIN CBO_Operators_Trl A2 ON A1.ID = A2.ID" +
                        " INNER JOIN CBO_Department A3 ON A3.ID = A1.Dept WHERE A2.Name = '" + sellerName + "' AND A1.Org = '" + org + "'";
                    DataAccessor.RunSQL(DataAccessor.GetConn(), set_2, null, out dataSet);
                    dataTable = dataSet.Tables[0];
                    //int datei = dataTable.Rows.Count;
                    string sellcode = "";
                    string deptcode = "";
                    if (dataTable.Rows != null && dataTable.Rows.Count > 0)
                    {
                        sellcode = dataTable.Rows[0]["Code"].ToString();
                        deptcode = dataTable.Rows[0]["DeptCode"].ToString();
                    }
                    #endregion
                    vDTO_line2.Account.Code = "224103|0|0|" + deptcode + "|" + sellcode + "|0|0|0";
                    //vDTO_line2.Account.Code = "224103|0|0|0301|2018120102|0|0|0|0";
                    //vDTO_line2.Account.Name = "个人|||内贸销售|胡学玲||||";
                    //vDTO_line2.Account.ID = 1002212050660012;
                    //vDTO_line2.m_account.m_name = "银行存款";
                    //sumCommissionltem = 0;//可提成数
                    //sumOverdueDebit = 0;//超期扣款
                    //sumActualCommission = 0;//实际应发提成
                    sumCommissionltem = linedtos.Where(x => x.Seller == s[i]).Sum(x => x.Commissionltem);
                    sumOverdueDebit = linedtos.Where(x => x.Seller == s[i]).Sum(x => x.OverdueDebit);
                    sumActualCommission = linedtos.Where(x => x.Seller == s[i]).Sum(x => x.ActualCommission);
                    //贷方金额(本币)
                    vDTO_line2.AccountedCr = Math.Round(sumCommissionltem + sumOverdueDebit + sumActualCommission, 3);//赋值
                    vDTO_line2.EnteredCr = Math.Round(sumCommissionltem + sumOverdueDebit + sumActualCommission, 3);//赋值
                    //vDTO_line2.AccountedCr = 0;//赋值
                    ////借方金额(本币)
                    //vDTO_line2.AccountedDr = 0;
                    ////贷方金额(原币)
                    //vDTO_line2.EnteredCr = 0;
                    ////借方金额(原币)
                    //vDTO_line2.EnteredDr = 0;
                    ////贷方数量
                    //vDTO_line2.AmountCr = 0;
                    //借方数量
                    //vDTO_line2.AccountedDr = 0;
                    //币种
                    vDTO_line2.Currency = new CommonArchiveDataDTOData();
                    vDTO_line2.Currency.ID = 1;
                    vDTO_line2.Currency.Code = "C001";
                    vDTO_line2.Currency.Name = "人民币";
                    vDTO_lines.Add(vDTO_line2);
                }
                #region TesT
                ////摘要
                //vDTO_line2.Abstracts = "计提提成凭证";
                ////科目
                //vDTO_line2.Account = new CommonArchiveDataDTOData();
                //vDTO_line2.Account.Code = "224103|0|0|0301|2018120102|0|0|0|0";
                //vDTO_line2.Account.Name = "个人|||内贸销售|胡学玲||||";
                //vDTO_line2.Account.ID = 1002212050660012;
                ////vDTO_line2.m_account.m_name = "银行存款";

                ////贷方金额(本币)
                //vDTO_line2.AccountedCr = 154;//赋值
                ////借方金额(本币)
                ////vDTO_line2.AccountedDr = 0;
                ////贷方金额(原币)
                //vDTO_line2.EnteredCr = 0;
                ////借方金额(原币)
                //vDTO_line2.EnteredDr = 0;
                ////贷方数量
                //vDTO_line2.AmountCr = 0;
                ////借方数量
                //vDTO_line2.AccountedDr = 0;

                ////币种
                //vDTO_line2.Currency = new CommonArchiveDataDTOData();
                //vDTO_line2.Currency.ID = 1;
                //vDTO_line2.Currency.Code = "C001";
                //vDTO_line2.Currency.Name = "人民币";

                #endregion
                vDTO_lines.Add(vDTO_line1);
                //vDTO_lines.Add(vDTO_line2);
                vDTO.Entries = vDTO_lines;
                vDTOs.Add(vDTO);
                iSVGL.ImportVoucherDTOs = vDTOs;
                //iSVGL.Do();

                List<ISVReturnVoucherDTOData> see = iSVGL.Do();
                string vDonNo = "";//凭证号
                foreach (var item in see)
                {
                    vDonNo = item.DocNo;
                }

                foreach (var item in this.Model.AnalyseHead.Records)
                {
                    item["VoucherDocNo"] = vDonNo;
                }
                string id = this.Model.AnalyseHead.FocusedRecord["ID"].ToString();
                string set_1 = "UPDATE Cust_JH_AnalyseHead SET VoucherDocNo='" + vDonNo + "' WHERE ID='" + id + "'";
                #endregion


                #endregion
            }
            #region TesT
            //voucherDTOData.VoucherCategory.ID = 1002206140140002;//凭证类型	
            //voucherDTOData.VoucherCategory.Code = "01";//凭证类型	
            //voucherDTOData.VoucherCategory.Name = "记账凭证";//凭证类型	
            //voucherDTOData.CreateDate = DateTime.Now.AddMonths(-1);

            ////ISVImportEntryDTOData entryDTODatas= new ISVImportEntryDTOData();
            //#region text
            ////1
            ////List<ISVImportEntryDTOData> iSVImports = new List<ISVImportEntryDTOData>();
            //ISVImportEntryDTOData line1 = new ISVImportEntryDTOData();//凭证维护行
            //line1.Account.Code = "660101|0|0|0301|0|0|0|0|0";
            //line1.Account.Name = "工资|||内贸销售|||||";
            //line1.Account.ID = 1002210220007967;
            //line1.Abstracts = "计提提成凭证";
            //line1.AccountedDr = 11;
            ////List<ISVImportEntryDTOData> iSVImports = new List<ISVImportEntryDTOData>();
            //ISVImportEntryDTOData line2 = new ISVImportEntryDTOData();//凭证维护行
            //line2.Account.Code = "660101|0|0|0301|0|0|0|0|0";
            //line2.Account.Name = "工资|||内贸销售|||||";
            //line2.Account.ID = 1002210220007967;
            //line2.Abstracts = "计提提成凭证";
            //line2.AccountedCr = 12;
            ////entryDTODatas[0] = line1;
            ////entryDTODatas[1] = line2;
            //voucherDTOData.Entries.Add(line1);
            //voucherDTOData.Entries.Add(line2);
            ////voucherDTODatas.Add(voucherDTOData);
            ////iSVImports.Add(entryDTOData);
            //#endregion
            //iSVGL.ImportVoucherDTOs.Add(voucherDTOData);
            //iSVGL.Do();
            //List<ISVReturnVoucherDTOData> see2222 = iSVGL.Do();
            //iSVGL.ImportVoucherDTOs.

            #region 创建批号的服务
            //CommonCreateLotMasterSRVProxy lotMasterSRV = new CommonCreateLotMasterSRVProxy();
            //List<CreateLotMasterDTOData> createLotMasterDTOData = new List<CreateLotMasterDTOData>();
            //CreateLotMasterDTOData createLot = new CreateLotMasterDTOData();
            //createLot.Item = new UFIDA.U9.CBO.Pub.Controller.CommonArchiveDataDTOData();
            //createLot.Item.ID = item.ItemInfo.ItemID.ID;//(long)item["ItemInfo_ItemID"];
            //createLot.Item.Name = item.ItemInfo.ItemName;//item["ItemInfo_ItemName"].ToString();
            //createLot.Item.Code = item.ItemInfo.ItemCode;//item["ItemInfo_ItemID"].ToString();
            //createLot.LotCode = newlotcode;//item["LotInfo_LotCode"].ToString();
            //createLotMasterDTOData.Add(createLot);
            //lotMasterSRV.CreateLotMasterDTOList = createLotMasterDTOData;
            //lotMasterSRV.Do();
            //List<IDCodeNameDTOData> see2222 = lotMasterSRV.Do();
            #endregion

            #endregion
            OnVoucher305_Click_DefaultImpl(sender, e);
        }



        #region 自定义数据初始化加载和数据收集
        private void OnLoadData_Extend(object sender)
        {
            OnLoadData_DefaultImpl(sender);
        }
        private void OnDataCollect_Extend(object sender)
        {
            OnDataCollect_DefaultImpl(sender);
        }
        #endregion

        #region 自己扩展 Extended Event handler 
        public void AfterOnLoad()
        {

        }

        public void AfterCreateChildControls()
        {


            //开启个性化
            UFIDA.U9.UI.PDHelper.PersonalizationHelper.SetPersonalizationEnable((BaseWebForm)this, true);

            //弹性域设置
            FlexFieldHelper.SetDescFlexField(new DescFlexFieldParameter[] { new DescFlexFieldParameter(this.FlexFieldPicker0, this.Model.AnalyseHead) });//表头的弹性域
            FlexFieldHelper.SetDescFlexField(this.DataGrid5, this.DataGrid5.Columns.Count - 1, "DescFlexField");

            //查询按钮设置
            PDFormMessage.ShowConfirmDialog(this.Page, "e333c443-b906-49e1-bd10-1174c1638262", "580", "408",
                Title, wpFindID.ClientID, this.BtnFind, null);

            //取得提示信息资源：是否删除当前记录
            string message = PDResource.GetDeleteConfirmInfo();
            //绑定注册弹出对话框到删除按钮
            PDFormMessage.ShowConfirmDialog(this.Page, message, "", this.BtnDelete);
            PDFormMessage.ShowConfirmDialog(this.Page, "确认放弃当前记录？", "", this.BtnCancel);
            //设置默认日期
            #region MyRegion
            try
            {
                string businessdate = this.Model.AnalyseHead.FieldBusinessDate.DefaultValue.ToString();
            }
            catch (Exception ex)
            {
                this.Model.AnalyseHead.FieldBusinessDate.DefaultValue = DateTime.Now;
            }
            #endregion
            //设置默认行号
            //GetProfileValueProxy bpObj = new GetProfileValueProxy();
            //bpObj.ProfileCode = "SysLineNo";
            //PVDTOData pVTDOData = bpObj.Do();
            //行GRID
            ((IAutoRowNo)this.DataGrid5.Columns["AccrualCalculationlinesNo"]).Sequence = true;
            ((IAutoRowNo)this.DataGrid5.Columns["AccrualCalculationlinesNo"]).SequenceStep = 10;
            ((IAutoRowNo)this.DataGrid5.Columns["AccrualCalculationlinesNo"]).SequenceStart = 10;
        }

        public void AfterEventBind()
        {
        }

        public void BeforeUIModelBinding()
        {

        }


        private void Approc()
        {
            if (this.Model.AnalyseHead.FocusedRecord != null)
            {
                UFIDA.U9.Cust.JH.AnalyseDocBP.Proxy.ApproveDocProxy proxy = new AnalyseDocBP.Proxy.ApproveDocProxy();
                proxy.ID = this.Model.AnalyseHead.FocusedRecord.ID;
                proxy.SysVersion = this.Model.AnalyseHead.FocusedRecord.SysVersion ?? 0;

                bool result = proxy.Do();
                this.Action.NavigateAction.Refresh(null);
            }
        }

        public void AfterUIModelBinding()
        {
            SetControlStatus();

            ((UFWebDataGridAdapter)this.DataGrid5).ClearTotalValue(); //清除所有列的合计值
            decimal saleQty = 0M;
            decimal priAndTaxUOM = 0M;
            decimal totalNMTC = 0M;
            decimal totalTax = 0M;
            decimal commissionltem = 0M;
            decimal overdueDebit = 0M;
            decimal judgement = 0M;
            decimal actualCommission = 0M;
            for (int i = 0; i < this.Model.AnalyseHead_Analyseline.Records.Count; i++)
            {
                saleQty += decimal.Parse(this.Model.AnalyseHead_Analyseline.Records[i]["SaleQty"].ToString());
                priAndTaxUOM += decimal.Parse(this.Model.AnalyseHead_Analyseline.Records[i]["PriAndTaxUOM"].ToString());
                totalNMTC += decimal.Parse(this.Model.AnalyseHead_Analyseline.Records[i]["TotalNMTC"].ToString());
                totalTax += decimal.Parse(this.Model.AnalyseHead_Analyseline.Records[i]["TotalTax"].ToString());
                commissionltem += decimal.Parse(this.Model.AnalyseHead_Analyseline.Records[i]["Commissionltem"].ToString());
                overdueDebit += decimal.Parse(this.Model.AnalyseHead_Analyseline.Records[i]["OverdueDebit"].ToString());
                judgement += decimal.Parse(this.Model.AnalyseHead_Analyseline.Records[i]["Judgement"].ToString());
                actualCommission += decimal.Parse(this.Model.AnalyseHead_Analyseline.Records[i]["ActualCommission"].ToString());
            }
            ((UFWebDataGridAdapter)this.DataGrid5).SetTotalValue("SaleQty", saleQty);
            ((UFWebDataGridAdapter)this.DataGrid5).SetTotalValue("PriAndTaxUOM", priAndTaxUOM);
            ((UFWebDataGridAdapter)this.DataGrid5).SetTotalValue("TotalNMTC", totalNMTC);
            ((UFWebDataGridAdapter)this.DataGrid5).SetTotalValue("TotalTax", totalTax);
            ((UFWebDataGridAdapter)this.DataGrid5).SetTotalValue("Commissionltem", commissionltem);
            ((UFWebDataGridAdapter)this.DataGrid5).SetTotalValue("OverdueDebit", overdueDebit);
            ((UFWebDataGridAdapter)this.DataGrid5).SetTotalValue("Judgement", judgement);
            ((UFWebDataGridAdapter)this.DataGrid5).SetTotalValue("ActualCommission", actualCommission);
            //系统参数设置只读
            ((UFSoft.UBF.UI.ControlModel.IUIFieldBindingDataBindControl)this.State92).ReadOnly = true;

        }

        #region 控制按钮状态
        private void SetControlStatus()
        {
            this.DataBind();
            this.DataCollect();
            //单据类型自动编号，单号控件不可用；单据类型手工编号，单号控件可用
            if (this.Model.AnalyseHead.FocusedRecord != null)
            {
                if (this.Model.AnalyseHead.FocusedRecord.AccrualNo == 0)
                    ((UFSoft.UBF.UI.ControlModel.IUIFieldBindingDataBindControl)this.DocNo53).ReadOnly = true;
                else
                    ((UFSoft.UBF.UI.ControlModel.IUIFieldBindingDataBindControl)this.DocNo53).ReadOnly = false;
            }

            //if (this.Model.AnalyseHead.FocusedRecord.ID < 0L)
            //    this.Card2.ReadOnly = false;
            //else
            if (this.Model.AnalyseHead.FocusedRecord.State >= 2)
                this.Card2.ReadOnly = true;
            if (this.Model.AnalyseHead.FocusedRecord.State == 0)
            {
                this.Card2.ReadOnly = false;
            }
            this.Toolbar2.Enabled = true;
            //下列暂时隐藏      
            this.BtnCopy.Enabled = true;//复制
            this.BtnSubmit.Enabled = true;
            this.BtnApprove.Enabled = true;
            this.BtnUndoApprove.Enabled = true;// 弃审
            this.BtnDelete.Enabled = true;
            this.BtnSave.Enabled = true;

            //Opened 0     System::Int32  开立  
            //Approving 1  System::Int32  审核中  
            //Approved 2   System::Int32  已审核  
            switch (this.Model.AnalyseHead.FocusedRecord.State)
            {
                case -1:
                case 0:
                    this.BtnApprove.Enabled = false;
                    this.BtnUndoApprove.Enabled = false;
                    if (this.Model.AnalyseHead.FocusedRecord.ID < 1)
                    {
                        this.BtnCopy.Enabled = false;
                        this.BtnSubmit.Enabled = false;
                    }
                    break;
                case 1:
                    this.BtnSubmit.Enabled = false;
                    this.BtnApprove.Enabled = true;
                    this.BtnUndoApprove.Enabled = false;
                    //this.BtnSave.Enabled = false;

                    break;
                case 2:
                    this.BtnSubmit.Enabled = false;
                    this.BtnApprove.Enabled = false;
                    this.BtnDelete.Enabled = false;
                    this.BtnSave.Enabled = false;
                    break;
                case 3:
                    this.Toolbar2.Enabled = false;
                    break;
                default:
                    break;
            }
            ((UFSoft.UBF.UI.ControlModel.IUIFieldBindingDataBindControl)this.DocNo53).ReadOnly = true;
        }
        #endregion
        public void LineAdd()
        {
            //收集界面错误信息
            if (this.Model.ErrorMessage.hasErrorMessage)
            {
                this.Model.ClearErrorMessage();
            }
            this.OnDataCollect(this);
            //取值
            string interDate = "";//利率
            foreach (var item in this.Model.AnalyseHead.Records)
            {
                interDate = item["IntersetRate"] == null ? "0" : item["IntersetRate"].ToString();
            }
            string org = PDContext.Current.OrgID.ToString();
            DateTime datetimes = Convert.ToDateTime(this.Model.AnalyseHead.FocusedRecord["BusinessDate"]);
            string datetime = datetimes.AddDays(1).ToString("yyyy-MM-dd");
            IUIRecordCollection records = this.Model.AnalyseHead_Analyseline.Records;
            AnalyseHead_AnalyselineRecord record = this.Model.AnalyseHead_Analyseline.FocusedRecord;
            //清理
            this.Model.AnalyseHead_Analyseline.Clear();
            #region 出货单
            List<Linedto> linedtos = new List<Linedto>();
            DataTable dataTable = new DataTable();
            string set = "SELECT  A2.ID,ItemInfo_ItemID,ItemInfo_ItemCode,ItemInfo_ItemName,TotalARMoneyTC,TotalAccountMoneyTC,TotalARQtyPriceAmount,A2.Seller,A1.DocNo,A2.DescFlexField_PrivateDescSeg1,A2.DescFlexField_PrivateDescSeg2," +
                "A2.InvUom,A2.ShipQtyInvAmount,A2.DescFlexField_PrivateDescSeg5,A2.DescFlexField_PubDescSeg14,A2.FinallyPriceTC,A2.TotalMoneyTC,A2.TotalNetMoney,A2.InvMainUOM,A2.ShipConfirmDate,A2.MaturityDate,A2.SrcDocNo,A2.TotalTaxTC,A2.TotalNetMoneyTC,A2.TaxRate " +
                "FROM SM_Ship A1 INNER JOIN SM_ShipLine A2 ON A1.ID = A2.Ship WHERE A1.Status = 3 AND TotalARMoneyTC!= 0 AND TotalAccountMoneyTC!= 0 AND TotalARQtyPriceAmount!= 0 AND A2.DescFlexField_PrivateDescSeg30!='已计提' AND A2.MaturityDate is not NULL   AND A1.Org='" + org + "'AND" +
                " (SELECT TOP(1) AR_ARApplyHead.ApplyDate FROM AR_ARApplyHead" +
                " INNER JOIN AR_ARApplyLine  ON AR_ARApplyHead.ID = AR_ARApplyLine.ARApplyHead WHERE   AR_ARApplyLine.DRDocNo =" +
                " (SELECT TOP(1) A9.DocNo FROM AR_ARBillLine A8 INNER JOIN AR_ARBillHead A9 ON A9.ID = A8.ARBillHead WHERE A8.SrcBillNum = A1.DocNo)) is not NULL " +
                " and A1.DocumentType!=(SELECT ID FROM SM_ShipDocType WHERE Org='" + org + "' AND Code='SM3')" +
                " and A1.DocumentType != (SELECT ID FROM SM_ShipDocType WHERE Org = '" + org + "' AND Code = 'SM4') " +
                "AND A2.ShipConfirmDate<='" + datetime + "'" +
                " AND (SELECT Code FROM SM_SODocType WHERE ID=(SELECT DocumentType FROM SM_SO WHERE DocNo=A2.SrcDocNo))!='SO9'";
            DataSet dataSet = new DataSet();
            DataAccessor.RunSQL(DataAccessor.GetConn(), set, null, out dataSet);
            dataTable = dataSet.Tables[0];
            int datei = dataTable.Rows.Count;
            if (dataTable.Rows != null && dataTable.Rows.Count > 0)
            {
                int k = 0;
                while (k < datei)
                {
                    Linedto linedto = new Linedto();
                    linedto.ID = dataTable.Rows[k]["ID"].ToString();
                    linedto.Operators = Convert.ToInt64(dataTable.Rows[k]["Seller"].ToString());
                    linedto.DocNo = dataTable.Rows[k]["DocNo"].ToString();
                    linedto.MaterialColor = GetMaterialColor(dataTable.Rows[k]["ItemInfo_ItemID"].ToString());
                    linedto.SaleUOM = dataTable.Rows[k]["InvMainUOM"].ToString();
                    linedto.WXL = dataTable.Rows[k]["DescFlexField_PrivateDescSeg2"].ToString();
                    linedto.SaleQty = dataTable.Rows[k]["ShipQtyInvAmount"].ToString();
                    linedto.SupSaleQty = GetSupSaleQty(dataTable.Rows[k]["ItemInfo_ItemID"].ToString());
                    linedto.SupSaleUOM = dataTable.Rows[k]["DescFlexField_PubDescSeg14"].ToString();
                    linedto.ShipFinallyPriTC = dataTable.Rows[k]["FinallyPriceTC"].ToString();
                    linedto.PriAndTaxUOM = dataTable.Rows[k]["TotalMoneyTC"].ToString();
                    linedto.TotalNMTC = dataTable.Rows[k]["TotalNetMoneyTC"].ToString();
                    linedto.ShipConfirmDate = dataTable.Rows[k]["ShipConfirmDate"].ToString();
                    linedto.MaturityDate = dataTable.Rows[k]["MaturityDate"].ToString();//到期日
                    linedto.Iteminfo_ID = Convert.ToInt64(dataTable.Rows[k]["ItemInfo_ItemID"].ToString());
                    linedto.Iteminfo_Code = dataTable.Rows[k]["ItemInfo_ItemCode"].ToString();
                    linedto.Iteminfo_Name = dataTable.Rows[k]["ItemInfo_ItemName"].ToString();
                    linedto.SrcDocNo = dataTable.Rows[k]["SrcDocNo"].ToString();
                    linedto.TotalTax = dataTable.Rows[k]["TotalTaxTC"].ToString();
                    linedto.TaxRate = dataTable.Rows[k]["TaxRate"].ToString();
                    linedtos.Add(linedto);
                    k++;
                }
            }
            int i = 10;//行号标识
            //出货单赋值
            foreach (var item in linedtos)
            {
                record = this.Model.AnalyseHead_Analyseline.AddNewUIRecord();
                //record.ID = i;
                //record.SysVersion = i;
                record.AccrualCalculationlinesNo = i.ToString(); i = i + 10;
                record.ForInID = item.ID;//源数据的ID
                record.Operators = item.Operators;
                record.NoForShipOrReturn = item.DocNo;
                record.ShipDocType = "标准出货单";
                record.Operators_Code = FindOperatorsName(item.Operators.ToString());
                record.Operators_Name = FindOperatorsCode(item.Operators.ToString());
                record.Iteminfo_ID = item.Iteminfo_ID;
                record.Iteminfo_ID_Code = item.Iteminfo_Code;
                record.Iteminfo_ID_Name = item.Iteminfo_Name;
                record.MaterialColor = item.MaterialColor;
                record.WXL = item.WXL;
                record.SaleUOM = FindBaseUom(item.SaleUOM);//主单位
                record.SaleQty = Convert.ToDecimal(item.SaleQty);//主数量
                record.SupSaleQty = item.SupSaleQty;//辅单位
                record.SupSaleUOM = item.SupSaleUOM;//辅数量
                record.ShipFinallyPriTC = item.ShipFinallyPriTC;//单位
                record.PriAndTaxUOM = Convert.ToDecimal(item.PriAndTaxUOM);//价税合计
                record.TotalNMTC = Convert.ToDecimal(item.TotalNMTC);
                string date = Convert.ToDateTime(FindARARApply(item.DocNo)).ToString("yyyy.MM.dd");
                if (date == "1999.12.31") continue;
                record.DateReceived = date;//收款日
                TimeSpan time = Convert.ToDateTime(Convert.ToDateTime(item.MaturityDate).ToString("yyyy-MM-dd")) - Convert.ToDateTime(Convert.ToDateTime(item.ShipConfirmDate).ToString("yyyy-MM-dd"));//偏离日-出货确认日期
                record.OffsetDay = Convert.ToDateTime(item.MaturityDate).ToString("yyyy.MM.dd");//偏离人------也就是到期日
                record.OffsetDay = time.ToString().Substring(0, 2) == "00" ? "0" : time.ToString().Substring(0, 2);//偏离人------也就是到期日
                record.LastDateOfDelivery = Convert.ToDateTime(item.ShipConfirmDate).ToString("yyyy.MM.dd");
                record.DateOflnvoice = Convert.ToDateTime(FindAR_ARBillLine(item.DocNo)).ToString("yyyy.MM.dd");//发票日期
                                                                                                                //record.Commissionltem = Convert.ToDecimal(Calculate(item.TotalNMTC, item.SrcDocNo, item.Iteminfo_ID.ToString()));//可提成数
                string day = this.Model.AnalyseHead.FocusedRecord["EstimateDay"].ToString();
                if (string.IsNullOrEmpty(day))
                {
                    record.OverdueDays = "0";
                }
                else
                {
                    record.OverdueDays = GetOverDueDays(record.DateReceived, record.LastDateOfDelivery, record.OffsetDay, day);//逾期天数=收款日期-出货确认日期-账期-预估天数
                }
                record.OverdueDebit = Convert.ToDecimal(GetoverDueDebit(item.TotalNMTC, interDate, record.OverdueDays));//超期扣款
                record.TotalTax = Convert.ToDecimal(item.TotalTax);//超期扣款
                record.RMTaxRate = item.TaxRate;//税率
                record.SetParentRecord(this.Model.AnalyseHead.FocusedRecord);
            }
            #endregion
            #region 退回处理
            List<Rmlinedoto> rmlinedotos = new List<Rmlinedoto>();
            string dateset = "SELECT A2.ID,A2.ItemInfo_ItemCode,A2.ItemInfo_ItemID,A2.ItemInfo_ItemName, A1.Seller,A1.DocNo,A2.PU,A2.ApplyQtyTU1,A2.TU,A2.RtnPice,A2.ApplyMoneyTC,A2.RtnMoneyTCNet,A2.RtnMoneyTC,A2.DescFlexField_PrivateDescSeg1,A2.MaturityDate" +
                ",A2.DescFlexField_PrivateDescSeg4,A1.BusinessDate,A2.DescFlexField_PrivateDescSeg5,A2.DescFlexField_PubDescSeg14,A2.OrderPrice,A2.RtnQtyTU1,A2.RtnPice," +
                "A2.TotalTaxTC,	A2.TaxRate  FROM SM_RMA A1 INNER JOIN SM_RMALine A2 ON A1.ID = A2.RMA WHERE A2.Status = 4 AND A2.DescFlexField_PrivateDescSeg30!='已计提'  AND A2.MaturityDate is not NULL AND A1.Org='" + org + "'AND" +
                " (SELECT TOP(1) AR_ARApplyHead.ApplyDate FROM AR_ARApplyHead" +
                " INNER JOIN AR_ARApplyLine  ON AR_ARApplyHead.ID = AR_ARApplyLine.ARApplyHead WHERE   AR_ARApplyLine.DRDocNo =" +
                " (SELECT TOP(1) A9.DocNo FROM AR_ARBillLine A8 INNER JOIN AR_ARBillHead A9 ON A9.ID = A8.ARBillHead WHERE A8.SrcBillNum = A1.DocNo)) is not NULL" +
                " and A1.DocumentType!=(SELECT ID FROM SM_RMADocType WHERE Org='" + org + "' AND Code='H0007')" +
                " and A1.DocumentType != (SELECT ID FROM SM_RMADocType WHERE Org = '" + org + "' AND Code = 'H0004')" +
                " AND A1.BusinessDate <=' " + datetime + "'";
            DataAccessor.RunSQL(DataAccessor.GetConn(), dateset, null, out dataSet);
            dataTable = dataSet.Tables[0];
            int datej = dataTable.Rows.Count;
            if (dataTable.Rows != null && dataTable.Rows.Count > 0)
            {
                int k = 0;
                while (k < datej)
                {
                    Rmlinedoto rmlinedoto = new Rmlinedoto();
                    rmlinedoto.ID = dataTable.Rows[k]["ID"].ToString();
                    rmlinedoto.Operators = Convert.ToInt64(dataTable.Rows[k]["Seller"].ToString());
                    rmlinedoto.Iteminfo_ID = Convert.ToInt64(dataTable.Rows[k]["ItemInfo_ItemID"].ToString());
                    rmlinedoto.Iteminfo_Code = dataTable.Rows[k]["ItemInfo_ItemCode"].ToString();
                    rmlinedoto.Iteminfo_Name = dataTable.Rows[k]["ItemInfo_ItemName"].ToString();
                    rmlinedoto.MaterialColor = GetMaterialColor(dataTable.Rows[k]["ItemInfo_ItemID"].ToString());
                    rmlinedoto.DocNo = dataTable.Rows[k]["DocNo"].ToString();
                    rmlinedoto.PU = dataTable.Rows[k]["PU"].ToString();
                    rmlinedoto.ApplyQtyTU1 = dataTable.Rows[k]["ApplyQtyTU1"].ToString();
                    rmlinedoto.ApplyMoneyTC = dataTable.Rows[k]["ApplyMoneyTC"].ToString();
                    rmlinedoto.RtnPice = dataTable.Rows[k]["RtnPice"].ToString();
                    rmlinedoto.RtnMoneyTCNet = dataTable.Rows[k]["RtnMoneyTCNet"].ToString();
                    rmlinedoto.RtnMoneyTC = dataTable.Rows[k]["RtnMoneyTC"].ToString();
                    rmlinedoto.TU = dataTable.Rows[k]["TU"].ToString();
                    rmlinedoto.LastDateOfDelivery = dataTable.Rows[k]["BusinessDate"].ToString();
                    rmlinedoto.MaturityDate = dataTable.Rows[k]["MaturityDate"].ToString();//到期日
                    rmlinedoto.RMRtnQtyTU1 = dataTable.Rows[k]["RtnQtyTU1"].ToString();//核定数量
                    rmlinedoto.RtnPice = dataTable.Rows[k]["RtnPice"].ToString();
                    rmlinedoto.RMOrderPrice = dataTable.Rows[k]["OrderPrice"].ToString();//定价
                    rmlinedoto.SupSaleQty = GetSupSaleQty(dataTable.Rows[k]["ItemInfo_ItemID"].ToString()); //辅单位
                    rmlinedoto.SupSaleUOM = dataTable.Rows[k]["DescFlexField_PubDescSeg14"].ToString();//辅数量
                    rmlinedoto.TotalTaxTC = dataTable.Rows[k]["TotalTaxTC"].ToString();//税额
                    rmlinedoto.TaxRate = dataTable.Rows[k]["TaxRate"].ToString();//税率
                    rmlinedotos.Add(rmlinedoto);
                    k++;
                }
            }
            #endregion
            //退回处理单赋值
            foreach (var item in rmlinedotos)
            {
                record = this.Model.AnalyseHead_Analyseline.AddNewUIRecord();
                //record.ID = i;
                //record.SysVersion = i;
                record.AccrualCalculationlinesNo = i.ToString(); i = i + 10;
                record.ForInID = item.ID;//来源ID
                record.Operators = item.Operators;
                record.NoForShipOrReturn = item.DocNo;
                record.ShipDocType = "退回处理单";
                record.Operators_Code = FindOperatorsName(item.Operators.ToString());
                record.Operators_Name = FindOperatorsCode(item.Operators.ToString());
                record.Iteminfo_ID = item.Iteminfo_ID;
                record.Iteminfo_ID_Code = item.Iteminfo_Code;
                record.Iteminfo_ID_Name = item.Iteminfo_Name;
                record.MaterialColor = item.MaterialColor;
                record.SaleUOM = FindBaseUom(item.PU);//计价单位--就是主单位
                record.SaleQty = Convert.ToDecimal("-" + item.RMRtnQtyTU1);//主数量
                record.SupSaleUOM = item.SupSaleUOM;//辅单位
                record.SupSaleQty = item.SupSaleQty;//辅数量
                record.RMApplyQtyTU = item.ApplyQtyTU1;//申请数量
                record.RMApplyMoneyTC = item.ApplyMoneyTC;//申请金额
                record.RMRtnPrice = item.RtnPice;//核定价格
                record.RMRtnMoneyTCNet = item.RtnMoneyTCNet;//核定未税额
                record.PriAndTaxUOM = Convert.ToDecimal("-" + item.RtnMoneyTC);//核定含税额
                record.RMTU = FindBaseUom(item.TU);//交易单位
                record.TotalNMTC = Convert.ToDecimal("-" + item.RtnPice);// 未税金额
                record.LastDateOfDelivery = item.LastDateOfDelivery == "" ? "" : Convert.ToDateTime(item.LastDateOfDelivery).ToString("yyyy.MM.dd");
                record.DateOflnvoice = FindAR_ARBillLine(item.DocNo) == "" ? "" : Convert.ToDateTime(FindAR_ARBillLine(item.DocNo)).ToString("yyyy.MM.dd");//发票日期
                record.Commissionltem = Convert.ToDecimal(Calculate(record.TotalNMTC.ToString(), record.NoForShipOrReturn, item.Iteminfo_ID.ToString(), "1"));//可提成数
                string date = Convert.ToDateTime(FindARARApply(item.DocNo)).ToString("yyyy.MM.dd");
                if (date == "1999.12.31") continue;
                record.DateReceived = date;//收款日
                record.DateReceived = FindARARApply(item.DocNo) == "" ? "" : Convert.ToDateTime(FindARARApply(item.DocNo)).ToString("yyyy.MM.dd");//收款日
                string day = this.Model.AnalyseHead.FocusedRecord["EstimateDay"].ToString();
                if (string.IsNullOrEmpty(day))
                {
                    record.OverdueDays = "0";
                }
                else
                {
                    record.OverdueDays = GetOverDueDays(record.DateReceived, record.LastDateOfDelivery, record.OffsetDay, day);//逾期天数=收款日期-出货确认日期-账期-预估天数
                }
                //record.DateReceived = date;//收款日
                TimeSpan time = Convert.ToDateTime(Convert.ToDateTime(item.MaturityDate).ToString("yyyy-MM-dd")) - Convert.ToDateTime(Convert.ToDateTime(record.LastDateOfDelivery).ToString("yyyy-MM-dd"));//偏离日-出货确认日期
                //TimeSpan time = Convert.ToDateTime(item.MaturityDate) - Convert.ToDateTime();//偏离日-出货确认日期
                //record.OffsetDay = Convert.ToDateTime(item.MaturityDate).ToString("yyyy.MM.dd");//偏离人------也就是到期日
                record.OffsetDay = time.ToString().Substring(0, 2) == "00" ? "0" : time.ToString().Substring(0, 2);//偏离人------也就是到期日
                //record.OffsetDay = item.MaturityDate == "" ? "" : Convert.ToDateTime(item.MaturityDate).ToString("yyyy.MM.dd");//偏离日------也就是到期日
                record.ShipFinallyPriTC = "-" + item.RMOrderPrice;//单价
                record.RMTaxRate = item.TaxRate;//税率
                record.TotalTax = Convert.ToDecimal("-" + item.TotalTaxTC);//税额
                record.SetParentRecord(this.Model.AnalyseHead.FocusedRecord);
            }
        }

        /// <summary>
        /// 出货单
        /// </summary>
        public class Linedto
        {
            public string ID { get; set; }
            /// <summary>
            /// 业务员
            /// </summary>
            public long Operators { get; set; }
            /// <summary>
            ///单据类型
            /// </summary>
            public string ShipDocType { get; set; }
            /// <summary>
            /// 料品
            /// </summary>
            public long Iteminfo_ID { get; set; }
            /// <summary>
            /// 编码
            /// </summary>
            public string Iteminfo_Code { get; set; }
            /// <summary>
            /// 名称
            /// </summary>
            public string Iteminfo_Name { get; set; }
            /// <summary>
            /// 出货/退货单号
            /// </summary>
            public string NoForShipOrReturn { get; set; }
            /// <summary>
            /// 材质颜色
            /// </summary>
            public string MaterialColor { get; set; }
            /// <summary>
            /// 宽乘长
            /// </summary>
            public string WXL { get; set; }
            /// <summary>
            ///主单位
            /// </summary>
            public string SaleUOM { get; set; }
            /// <summary>
            /// 主数量
            /// </summary>
            public string SaleQty { get; set; }
            /// <summary>
            ///辅单位
            /// </summary>
            public string SupSaleQty { get; set; }
            /// <summary>
            /// 辅数量
            /// </summary>
            public string SupSaleUOM { get; set; }
            /// <summary>
            /// 单价
            /// </summary>
            public string ShipFinallyPriTC { get; set; }
            /// <summary>
            /// 价税合计
            /// </summary>
            public string PriAndTaxUOM { get; set; }
            /// <summary>
            /// 未税金额
            /// </summary>
            public string TotalNMTC { get; set; }
            /// <summary>
            /// 单号
            /// </summary>
            public string DocNo { get; set; }

            /// <summary>
            /// 最后发货确认日期
            /// </summary>
            public string LastDateOfDelivery { get; set; }
            /// <summary>
            /// 出货确认日期
            /// </summary>
            public string ShipConfirmDate { get; set; }
            /// <summary>
            /// 到期日
            /// </summary>
            public string MaturityDate { get; set; }
            /// <summary>
            /// 来源单号-销售订单
            /// </summary>
            public string SrcDocNo { get; set; }
            /// <summary>
            /// 凭证-业务员
            /// </summary>
            public string Seller { get; set; }
            /// <summary>
            /// 凭证-可提成数
            /// </summary>
            public decimal Commissionltem { get; set; }
            /// <summary>
            /// 凭证-超期扣款
            /// </summary>
            public decimal OverdueDebit { get; set; }
            /// <summary>
            /// 凭证-实际应发提成
            /// </summary>
            public decimal ActualCommission { get; set; }
            /// <summary>
            /// 税额
            /// </summary>
            public string TotalTax { get; set; }
            /// <summary>
            /// 税率
            /// </summary>
            public string TaxRate { get; set; }
        }
        /// <summary>
        /// 退货单
        /// </summary>
        public class Rmlinedoto
        {
            public string ID { get; set; }

            /// <summary>
            /// 业务员
            /// </summary>
            public long Operators { get; set; }
            /// <summary>
            ///单据类型
            /// </summary>
            public string ShipDocType { get; set; }
            /// <summary>
            /// 出货/退货单号
            /// </summary>
            public string NoForShipOrReturn { get; set; }
            /// <summary>
            /// 料品
            /// </summary>
            public long Iteminfo_ID { get; set; }
            /// <summary>
            /// 编码
            /// </summary>
            public string Iteminfo_Code { get; set; }
            /// <summary>
            /// 名称
            /// </summary>
            public string Iteminfo_Name { get; set; }
            /// <summary>
            /// 单号
            /// </summary>
            public string DocNo { get; set; }
            /// <summary>
            /// 计价单位
            /// </summary>
            public string PU { get; set; }
            /// <summary>
            /// 申请数量
            /// </summary>
            public string ApplyQtyTU1 { get; set; }
            /// <summary>
            /// 交易单位
            /// </summary>
            public string TU { get; set; }
            /// <summary>
            /// 核定价格
            /// </summary>
            public string RtnPice { get; set; }
            /// <summary>
            /// 申请金额
            /// </summary>
            public string ApplyMoneyTC { get; set; }
            /// <summary>
            /// 核定未税额
            /// </summary>
            public string RtnMoneyTCNet { get; set; }
            /// <summary>
            /// 核定含税额
            /// </summary>
            public string RtnMoneyTC { get; set; }
            /// <summary>
            /// 材质颜色
            /// </summary>
            public string MaterialColor { get; set; }
            /// <summary>
            /// 规格型号
            /// </summary>
            //public string MaterialColor { get; set; }

            /// <summary>
            /// 最后发货确认日期
            /// </summary>
            public string LastDateOfDelivery { get; set; }
            /// <summary>
            /// 到期日
            /// </summary>
            public string MaturityDate { get; set; }
            /// <summary>
            /// 退货定价
            /// </summary>
            public string RMOrderPrice { get; set; }
            /// <summary>
            /// 退货核定数量
            /// </summary>
            public string RMRtnQtyTU1 { get; set; }
            /// <summary>
            /// 退货核定价格
            /// </summary>
            public string RmRtnPice { get; set; }
            /// <summary>
            ///辅单位
            /// </summary>
            public string SupSaleQty { get; set; }
            /// <summary>
            /// 辅数量
            /// </summary>
            public string SupSaleUOM { get; set; }
            /// <summary>
            /// 税额
            /// </summary>
            public string TotalTaxTC { get; set; }
            /// <summary>
            /// 税率
            /// </summary>
            public string TaxRate { get; set; }
        }

        /// <summary>
        /// 通过业务员的id查询Code
        /// </summary>
        /// <param name="opName"></param>
        /// <returns></returns>
        public string FindOperatorsName(string opName)
        {
            string ret = "";
            DataTable dataTable = new DataTable();
            string set = "SELECT Code FROM CBO_Operators WHERE ID='" + opName + "'";
            DataSet dataSet = new DataSet();
            DataAccessor.RunSQL(DataAccessor.GetConn(), set, null, out dataSet);
            dataTable = dataSet.Tables[0];
            int datei = dataTable.Rows.Count;
            if (dataTable.Rows != null && dataTable.Rows.Count > 0)
            {
                ret = dataTable.Rows[0]["Code"].ToString();
            }
            return ret;
        }
        /// <summary>
        ///  通过业务员的id查询Name
        /// </summary>
        /// <param name="opCode"></param>
        /// <returns></returns>
        public string FindOperatorsCode(string opCode)
        {
            string ret = "";
            DataTable dataTable = new DataTable();
            string set = "SELECT Name FROM CBO_Operators_Trl WHERE ID='" + opCode + "' AND sysmlflag = 'zh-CN'";
            DataSet dataSet = new DataSet();
            DataAccessor.RunSQL(DataAccessor.GetConn(), set, null, out dataSet);
            dataTable = dataSet.Tables[0];
            int datei = dataTable.Rows.Count;
            if (dataTable.Rows != null && dataTable.Rows.Count > 0)
            {
                ret = dataTable.Rows[0]["Name"].ToString();
            }
            return ret;
        }

        /// <summary>
        /// 主单位
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public string FindBaseUom(string code)
        {
            string ret = "";
            DataTable dataTable = new DataTable();
            string set = "SELECT Name FROM Base_UOM_Trl WHERE ID = '" + code + "' AND sysmlflag = 'zh-CN'";
            DataSet dataSet = new DataSet();
            DataAccessor.RunSQL(DataAccessor.GetConn(), set, null, out dataSet);
            dataTable = dataSet.Tables[0];
            int datei = dataTable.Rows.Count;
            if (dataTable.Rows != null && dataTable.Rows.Count > 0)
            {
                ret = dataTable.Rows[0]["Name"].ToString();
            }
            return ret;
        }


        /// <summary>
        /// 通过出货单号找到核销日期
        /// 退回处理也能套用
        /// </summary>
        /// <param name="docno"></param>
        /// <returns></returns>
        public string FindARARApply(string docno)
        {
            string r = "";

            DataTable dataTable = new DataTable();
            string set = "SELECT TOP(1) A1.ApplyDate FROM AR_ARApplyHead A1 INNER JOIN AR_ARApplyLine A2 ON A1.ID = A2.ARApplyHead WHERE " +
                " A2.DRDocNo = (SELECT TOP(1) A2.DocNo FROM AR_ARBillLine A1 INNER JOIN AR_ARBillHead A2 ON A2.ID = A1.ARBillHead " +
                "WHERE A1.SrcBillNum = '" + docno + "')";
            DataSet dataSet = new DataSet();
            DataAccessor.RunSQL(DataAccessor.GetConn(), set, null, out dataSet);
            dataTable = dataSet.Tables[0];
            int datei = dataTable.Rows.Count;
            if (dataTable.Rows != null && dataTable.Rows.Count > 0)
            {
                r = dataTable.Rows[0]["ApplyDate"].ToString();
            }
            else
            {
                r = "1999-12-31";
            }
            return r;
        }

        /// <summary>
        /// 应收单日期---发票日期
        /// </summary>
        /// <param name="docno"></param>
        /// <returns></returns>
        public string FindAR_ARBillLine(string docno)
        {
            string s = "";
            DataTable dataTable = new DataTable();
            string set = "SELECT  TOP(1) A2.AccrueDate FROM AR_ARBillLine A1 INNER JOIN AR_ARBillHead A2 ON A2.ID = A1.ARBillHead" +
                " WHERE A1.SrcBillNum = '" + docno + "'";
            DataSet dataSet = new DataSet();
            DataAccessor.RunSQL(DataAccessor.GetConn(), set, null, out dataSet);
            dataTable = dataSet.Tables[0];
            int datei = dataTable.Rows.Count;
            if (dataTable.Rows != null && dataTable.Rows.Count > 0)
            {
                s = dataTable.Rows[0]["AccrueDate"].ToString();
            }
            else
            {
                s = "1999-12-31";
            }
            return s;
        }

        /// <summary>
        /// 可提成数
        /// 公式-未税金额*销售订单行上的私有字段
        /// </summary>
        /// <param name="TotalNMTC">未税金额</param>
        /// <param name="SrcDocNo">销售订单单号</param>
        /// <param name="Item">销售订单单号</param>
        /// <returns></returns>
        public string Calculate(string TotalNMTC, string SrcDocNo, string Item, string Set)
        {
            if (Set != "1")
            {
                string r = "";
                string sodocno = "";
                DataTable dataTable = new DataTable();
                string set = "SELECT A2.DescFlexField_PrivateDescSeg2 FROM SM_SO A1 INNER JOIN SM_SOLine A2 ON A1.ID = A2.SO WHERE A1.DocNo = '" + SrcDocNo + "' " +
                    "AND A2.ItemInfo_ItemID = '" + Item + "'";
                DataSet dataSet = new DataSet();
                DataAccessor.RunSQL(DataAccessor.GetConn(), set, null, out dataSet);
                dataTable = dataSet.Tables[0];
                int datei = dataTable.Rows.Count;
                if (dataTable.Rows != null && dataTable.Rows.Count > 0)
                {
                    sodocno = dataTable.Rows[0]["DescFlexField_PrivateDescSeg2"].ToString();
                }
                //未税金额
                decimal mtc = string.IsNullOrEmpty(TotalNMTC) ? 0 : Convert.ToDecimal(TotalNMTC);
                //提成
                decimal docno = string.IsNullOrEmpty(sodocno) ? 0 : Convert.ToDecimal(sodocno);
                r = (mtc * docno).ToString();
                return r;
            }
            else
            {
                string r = "";
                string sodocno = "";
                DataTable dataTable = new DataTable();

                //SELECT A2.DescFlexField_PrivateDescSeg2 FROM SM_SO A1 INNER JOIN SM_SOLine A2 ON A1.ID = A2.SO WHERE A1.DocNo=
                //(SELECT SM_RMALine.SrcDocNo FROM SM_RMALine
                //INNER JOIN SM_RMA  ON SM_RMALine.RMA = SM_RMA.ID WHERE SM_RMALine.SrcDocNo = '10SM2209150020')
                //string set = "SELECT A2.DescFlexField_PrivateDescSeg2 FROM SM_SO A1 INNER JOIN SM_SOLine A2 ON A1.ID = A2.SO WHERE A1.DocNo = '" + SrcDocNo + "' " +
                //    "AND A2.ItemInfo_ItemID = '" + Item + "'";
                string set = "SELECT A2.DescFlexField_PrivateDescSeg2 FROM SM_SO A1 INNER JOIN SM_SOLine A2 ON A1.ID = A2.SO WHERE A1.DocNo = (" +
                    "SELECT A2.DescFlexField_PrivateDescSeg2 FROM SM_SO A1 INNER JOIN SM_SOLine A2 ON A1.ID = A2.SO WHERE A1.DocNo=" +
                    "(SELECT SM_RMALine.SrcDocNo FROM SM_RMALine" +
                    " INNER JOIN SM_RMA  ON SM_RMALine.RMA = SM_RMA.ID WHERE SM_RMALine.SrcDocNo = '" + SrcDocNo + "')) " +
                    "AND A2.ItemInfo_ItemID = '" + Item + "'";
                DataSet dataSet = new DataSet();
                DataAccessor.RunSQL(DataAccessor.GetConn(), set, null, out dataSet);
                dataTable = dataSet.Tables[0];
                int datei = dataTable.Rows.Count;
                if (dataTable.Rows != null && dataTable.Rows.Count > 0)
                {
                    sodocno = dataTable.Rows[0]["DescFlexField_PrivateDescSeg2"].ToString();
                }
                //未税金额
                decimal mtc = string.IsNullOrEmpty(TotalNMTC) ? 0 : Convert.ToDecimal(TotalNMTC);
                //提成
                decimal docno = string.IsNullOrEmpty(sodocno) ? 0 : Convert.ToDecimal(sodocno);
                r = (mtc * docno).ToString();
                return r;
            }
        }
        /// <summary>
        /// 逾期天数+预估天数
        /// </summary>
        /// <param name="docno">单号</param>
        /// <param name=""></param>
        /// <returns></returns>        
        public string GetOverDueDays(string dateReceived, string lastDateOfDelivery, string offsetDay, string day)
        {

            //record.OverdueDays = GetOverDueDays(record.DateReceived, record.LastDateOfDelivery, record.OffsetDay, day);//逾期天数=收款日期-出货确认日期-账期-预估天数
            #region TexT
            //string r = "";
            //string time = "";
            //DataTable dataTable = new DataTable();
            //string set = "SELECT TOP(1) A1.ApplyDate FROM AR_ARApplyHead A1 INNER JOIN AR_ARApplyLine A2 ON A1.ID = A2.ARApplyHead WHERE " +
            //    "A2.DRDocNo = (SELECT TOP(1)  A2.DocNo FROM AR_ARBillLine A1 INNER JOIN AR_ARBillHead A2 ON A2.ID = A1.ARBillHead " +
            //    "WHERE A1.SrcBillNum = '" + docno + "' ORDER BY A2.CreatedOn DESC) ORDER BY A2.CreatedOn DESC";
            //DataSet dataSet = new DataSet();
            //DataAccessor.RunSQL(DataAccessor.GetConn(), set, null, out dataSet);
            //dataTable = dataSet.Tables[0];
            //int datei = dataTable.Rows.Count;
            //if (dataTable.Rows != null && dataTable.Rows.Count > 0)
            //{
            //    time = dataTable.Rows[0]["ApplyDate"].ToString();
            //}
            //TimeSpan timeSpan = Convert.ToDateTime(time).AddDays(-Convert.ToDouble(day)) - Convert.ToDateTime(lastDateOfDelivery);
            //r = timeSpan.TotalDays.ToString();
            ////r = (Convert.ToDateTime(time) - Convert.ToDateTime(lastDateOfDelivery)).ToString();
            //return r;
            #endregion
            string r = "";
            //TimeSpan time = Convert.ToDateTime(offsetDay) - Convert.ToDateTime(lastDateOfDelivery);//偏离日-出货确认日期
            //decimal zhangqi = Convert.ToInt64(time.ToString().Substring(0, 2));
            //DateTime seew = Convert.ToDateTime(dateReceived).AddDays(-Convert.ToDouble(day));
            //DateTime seet = Convert.ToDateTime(lastDateOfDelivery).AddDays(Convert.ToDouble(time.TotalDays.ToString()));
            TimeSpan timeSpan = Convert.ToDateTime(dateReceived) - Convert.ToDateTime(lastDateOfDelivery);
            r = timeSpan.TotalDays.ToString();
            decimal days = Convert.ToDecimal(r) - Convert.ToDecimal(offsetDay) - Convert.ToDecimal(day);
            return days.ToString();


            //r = (Convert.ToDateTime(time) - Convert.ToDateTime(lastDateOfDelivery)).ToString();
        }

        /// <summary>
        /// 超期扣款
        /// 出库金额*利率*50%*逾期天数
        /// </summary>
        /// <param name="totalNMTC">出库金额</param>
        /// <param name="intersetRate">利率</param>
        /// <param name="overdueDays">逾期天数</param>
        /// <returns></returns>
        public string GetoverDueDebit(string totalNMTC, string intersetRate, string overdueDays)
        {
            string r = "";
            if (string.IsNullOrEmpty(intersetRate))
            {
                intersetRate = "0";
            }
            r = Math.Round(decimal.Parse(totalNMTC) * decimal.Parse(intersetRate) * decimal.Parse(overdueDays), 2).ToString();
            if (r.Substring(0, 1) == "-")
            {
                r = "0";
            }
            return r;
        }


        /// <summary>
        /// 材质颜色，取值的是料品的私有字段一
        /// </summary>
        public string GetMaterialColor(string itemid)
        {
            string ret = "";
            DataTable dataTable = new DataTable();
            string set = "SELECT DescFlexField_PrivateDescSeg1 FROM CBO_ItemMaster WHERE ID='" + itemid + "'";
            DataSet dataSet = new DataSet();
            DataAccessor.RunSQL(DataAccessor.GetConn(), set, null, out dataSet);
            dataTable = dataSet.Tables[0];
            int datei = dataTable.Rows.Count;
            if (dataTable.Rows != null && dataTable.Rows.Count > 0)
            {
                ret = dataTable.Rows[0]["DescFlexField_PrivateDescSeg1"].ToString();
            }
            return ret;
        }

        /// <summary>
        /// 辅单位，取值的是料品的私有字段五
        /// </summary>
        public string GetSupSaleQty(string itemid)
        {
            string ret = "";
            DataTable dataTable = new DataTable();
            string set = "SELECT DescFlexField_PrivateDescSeg5 FROM CBO_ItemMaster WHERE ID='" + itemid + "'";
            DataSet dataSet = new DataSet();
            DataAccessor.RunSQL(DataAccessor.GetConn(), set, null, out dataSet);
            dataTable = dataSet.Tables[0];
            int datei = dataTable.Rows.Count;
            if (dataTable.Rows != null && dataTable.Rows.Count > 0)
            {
                ret = dataTable.Rows[0]["DescFlexField_PrivateDescSeg5"].ToString();
            }
            return ret;
        }



        public void LineAdd_YUTI(string starttime, string endtime)
        {
            //收集界面错误信息
            if (this.Model.ErrorMessage.hasErrorMessage)
            {
                this.Model.ClearErrorMessage();
            }
            this.OnDataCollect(this);

            string org = PDContext.Current.OrgID.ToString();

            decimal money = 0;
            //取值
            string interDate = "";//利率
            foreach (var item in this.Model.AnalyseHead.Records)
            {
                interDate = item["IntersetRate"] == null ? "0" : item["IntersetRate"].ToString();
            }
            IUIRecordCollection records = this.Model.AnalyseHead_Analyseline.Records;
            AnalyseHead_AnalyselineRecord record = this.Model.AnalyseHead_Analyseline.FocusedRecord;
            //清理
            this.Model.AnalyseHead_Analyseline.Clear();
            #region 出货单
            List<Linedto> linedtos = new List<Linedto>();
            DataTable dataTable = new DataTable();
            string set = "SELECT  A2.ID,ItemInfo_ItemID,ItemInfo_ItemCode,ItemInfo_ItemName,TotalARMoneyTC,TotalAccountMoneyTC,TotalARQtyPriceAmount,A2.Seller,A1.DocNo,A2.DescFlexField_PrivateDescSeg1,A2.DescFlexField_PrivateDescSeg2," +
                "A2.InvUom,A2.ShipQtyInvAmount,A2.DescFlexField_PrivateDescSeg5,A2.DescFlexField_PubDescSeg14,A2.FinallyPriceTC,A2.TotalMoneyTC,A2.TotalNetMoney,A2.InvMainUOM,A2.ShipConfirmDate,A2.MaturityDate,A2.SrcDocNo,A2.TotalTaxTC,A2.TotalNetMoneyTC,A2.TaxRate " +
                "FROM SM_Ship A1 INNER JOIN SM_ShipLine A2 ON A1.ID = A2.Ship WHERE A1.Status = 3 AND A2.DescFlexField_PrivateDescSeg29!='已预提'" +
                "AND A2.CreatedOn  >=  '" + starttime + "' AND A2.CreatedOn  <='" + endtime + "'" +
                "AND A1.Org='" + org + "'" +
                " and A1.DocumentType!=(SELECT ID FROM SM_ShipDocType WHERE Org='" + org + "' AND Code='SM3')" +
                " and A1.DocumentType != (SELECT ID FROM SM_ShipDocType WHERE Org = '" + org + "' AND Code = 'SM4')";
            DataSet dataSet = new DataSet();
            DataAccessor.RunSQL(DataAccessor.GetConn(), set, null, out dataSet);
            dataTable = dataSet.Tables[0];
            int datei = dataTable.Rows.Count;
            if (dataTable.Rows != null && dataTable.Rows.Count > 0)
            {
                int k = 0;
                while (k < datei)
                {
                    Linedto linedto = new Linedto();
                    linedto.ID = dataTable.Rows[k]["ID"].ToString();
                    linedto.Operators = Convert.ToInt64(dataTable.Rows[k]["Seller"].ToString());
                    linedto.DocNo = dataTable.Rows[k]["DocNo"].ToString();
                    linedto.MaterialColor = GetMaterialColor(dataTable.Rows[k]["ItemInfo_ItemID"].ToString());
                    linedto.SaleUOM = dataTable.Rows[k]["InvMainUOM"].ToString();
                    linedto.WXL = dataTable.Rows[k]["DescFlexField_PrivateDescSeg2"].ToString();
                    linedto.SaleQty = dataTable.Rows[k]["ShipQtyInvAmount"].ToString();
                    linedto.SupSaleQty = GetSupSaleQty(dataTable.Rows[k]["ItemInfo_ItemID"].ToString());
                    linedto.SupSaleUOM = dataTable.Rows[k]["DescFlexField_PubDescSeg14"].ToString();
                    linedto.ShipFinallyPriTC = dataTable.Rows[k]["FinallyPriceTC"].ToString();
                    linedto.PriAndTaxUOM = dataTable.Rows[k]["TotalMoneyTC"].ToString();
                    linedto.TotalNMTC = dataTable.Rows[k]["TotalNetMoneyTC"].ToString();
                    linedto.ShipConfirmDate = dataTable.Rows[k]["ShipConfirmDate"].ToString();
                    linedto.MaturityDate = dataTable.Rows[k]["MaturityDate"].ToString();//到期日
                    linedto.Iteminfo_ID = Convert.ToInt64(dataTable.Rows[k]["ItemInfo_ItemID"].ToString());
                    linedto.Iteminfo_Code = dataTable.Rows[k]["ItemInfo_ItemCode"].ToString();
                    linedto.Iteminfo_Name = dataTable.Rows[k]["ItemInfo_ItemName"].ToString();
                    linedto.SrcDocNo = dataTable.Rows[k]["SrcDocNo"].ToString();
                    linedto.TotalTax = dataTable.Rows[k]["TotalTaxTC"].ToString();
                    linedto.TaxRate = dataTable.Rows[k]["TaxRate"].ToString();
                    linedtos.Add(linedto);
                    k++;
                }
            }
            int i = 10;//行号标识
                       //出货单赋值
            foreach (var item in linedtos)
            {
                record = this.Model.AnalyseHead_Analyseline.AddNewUIRecord();
                //record.ID = i;
                //record.SysVersion = i;
                record.AccrualCalculationlinesNo = i.ToString(); i = i + 10;
                record.ForInID = item.ID;//源数据的ID
                record.Operators = item.Operators;
                record.NoForShipOrReturn = item.DocNo;
                record.ShipDocType = "标准出货单";
                record.Operators_Code = FindOperatorsName(item.Operators.ToString());
                record.Operators_Name = FindOperatorsCode(item.Operators.ToString());
                record.Iteminfo_ID = item.Iteminfo_ID;
                record.Iteminfo_ID_Code = item.Iteminfo_Code;
                record.Iteminfo_ID_Name = item.Iteminfo_Name;
                record.MaterialColor = item.MaterialColor;
                record.WXL = item.WXL;
                record.SaleUOM = FindBaseUom(item.SaleUOM);//主单位
                record.SaleQty = Convert.ToDecimal(item.SaleQty);//主数量
                record.SupSaleQty = item.SupSaleQty;//辅单位
                record.SupSaleUOM = item.SupSaleUOM;//辅数量
                record.ShipFinallyPriTC = item.ShipFinallyPriTC;//单位
                record.PriAndTaxUOM = Convert.ToDecimal(item.PriAndTaxUOM);//价税合计
                record.TotalNMTC = Convert.ToDecimal(item.TotalNMTC);
                //string date = Convert.ToDateTime(FindARARApply(item.DocNo)).ToString("yyyy.MM.dd");
                //if (date == "1999-12-31") continue;
                //record.DateReceived = date;//收款日
                //record.OffsetDay = Convert.ToDateTime(item.MaturityDate).ToString("yyyy.MM.dd");//偏离人------也就是到期日
                //record.LastDateOfDelivery = Convert.ToDateTime(item.ShipConfirmDate).ToString("yyyy.MM.dd");
                //record.DateOflnvoice = Convert.ToDateTime(FindAR_ARBillLine(item.DocNo)).ToString("yyyy.MM.dd");//发票日期
                //record.Commissionltem = Calculate(item.TotalNMTC, item.SrcDocNo, item.Iteminfo_ID.ToString());//可提成数
                //string day = this.Model.AnalyseHead.FocusedRecord["EstimateDay"].ToString();
                //if (string.IsNullOrEmpty(day))
                //{
                //    record.OverdueDays = "0";
                //}
                //else
                //{
                //    record.OverdueDays = GetOverDueDays(record.DateReceived, record.LastDateOfDelivery, record.OffsetDay, day);//逾期天数=收款日期-出货确认日期-账期-预估天数
                //    //record.OverdueDays = GetOverDueDays(item.DocNo, record.LastDateOfDelivery, day);//逾期天数
                //}
                //record.OverdueDebit = Convert.ToDecimal(GetoverDueDebit(item.TotalNMTC, interDate, record.OverdueDays));//超期扣款
                record.TotalTax = Convert.ToDecimal(item.TotalTax);//超期扣款
                record.RMTaxRate = item.TaxRate;//税率
                record.SetParentRecord(this.Model.AnalyseHead.FocusedRecord);
            }
            #endregion
            #region 退回处理
            List<Rmlinedoto> rmlinedotos = new List<Rmlinedoto>();
            string dateset = "SELECT  A2.ID,A2.ItemInfo_ItemCode,A2.ItemInfo_ItemID,A2.ItemInfo_ItemName, A1.Seller,A1.DocNo,A2.PU,A2.ApplyQtyTU1,A2.TU,A2.RtnPice,A2.ApplyMoneyTC,A2.RtnMoneyTCNet,A2.RtnMoneyTC,A2.DescFlexField_PrivateDescSeg1,A2.MaturityDate" +
                ",A2.DescFlexField_PrivateDescSeg4,A1.BusinessDate,A2.DescFlexField_PrivateDescSeg5,A2.DescFlexField_PubDescSeg14,A2.OrderPrice,A2.RtnQtyTU1,A2.RtnPice," +
                "A2.TotalTaxTC,	A2.TaxRate  FROM SM_RMA A1 INNER JOIN SM_RMALine A2 ON A1.ID = A2.RMA WHERE A2.Status = 4 AND A2.DescFlexField_PrivateDescSeg29!='已预提'" +
                "AND A2.CreatedOn  >=  '" + starttime + "' AND A2.CreatedOn  <='" + endtime + "'" +
                " and A1.DocumentType!=(SELECT ID FROM SM_ShipDocType WHERE Org='" + org + "' AND Code='SM3')" +
                " and A1.DocumentType != (SELECT ID FROM SM_ShipDocType WHERE Org = '" + org + "' AND Code = 'SM4')";
            DataAccessor.RunSQL(DataAccessor.GetConn(), dateset, null, out dataSet);
            dataTable = dataSet.Tables[0];
            int datej = dataTable.Rows.Count;
            if (dataTable.Rows != null && dataTable.Rows.Count > 0)
            {
                int k = 0;
                while (k < datej)
                {
                    Rmlinedoto rmlinedoto = new Rmlinedoto();
                    rmlinedoto.ID = dataTable.Rows[k]["ID"].ToString();
                    rmlinedoto.Operators = Convert.ToInt64(dataTable.Rows[k]["Seller"].ToString());
                    rmlinedoto.Iteminfo_ID = Convert.ToInt64(dataTable.Rows[k]["ItemInfo_ItemID"].ToString());
                    rmlinedoto.Iteminfo_Code = dataTable.Rows[k]["ItemInfo_ItemCode"].ToString();
                    rmlinedoto.Iteminfo_Name = dataTable.Rows[k]["ItemInfo_ItemName"].ToString();
                    rmlinedoto.MaterialColor = GetMaterialColor(dataTable.Rows[k]["ItemInfo_ItemID"].ToString());
                    rmlinedoto.DocNo = dataTable.Rows[k]["DocNo"].ToString();
                    rmlinedoto.PU = dataTable.Rows[k]["PU"].ToString();
                    rmlinedoto.ApplyQtyTU1 = dataTable.Rows[k]["ApplyQtyTU1"].ToString();
                    rmlinedoto.ApplyMoneyTC = dataTable.Rows[k]["ApplyMoneyTC"].ToString();
                    rmlinedoto.RtnPice = dataTable.Rows[k]["RtnPice"].ToString();
                    rmlinedoto.RtnMoneyTCNet = dataTable.Rows[k]["RtnMoneyTCNet"].ToString();
                    rmlinedoto.RtnMoneyTC = dataTable.Rows[k]["RtnMoneyTC"].ToString();
                    rmlinedoto.TU = dataTable.Rows[k]["TU"].ToString();
                    rmlinedoto.LastDateOfDelivery = dataTable.Rows[k]["BusinessDate"].ToString();
                    rmlinedoto.MaturityDate = dataTable.Rows[k]["MaturityDate"].ToString();//到期日
                    rmlinedoto.RMRtnQtyTU1 = dataTable.Rows[k]["RtnQtyTU1"].ToString();//核定数量
                    rmlinedoto.RtnPice = dataTable.Rows[k]["RtnPice"].ToString();
                    rmlinedoto.RMOrderPrice = dataTable.Rows[k]["OrderPrice"].ToString();//定价
                    rmlinedoto.SupSaleQty = GetSupSaleQty(dataTable.Rows[k]["ItemInfo_ItemID"].ToString()); //辅单位
                    rmlinedoto.SupSaleUOM = dataTable.Rows[k]["DescFlexField_PubDescSeg14"].ToString();//辅数量
                    rmlinedoto.TotalTaxTC = dataTable.Rows[k]["TotalTaxTC"].ToString();//税额
                    rmlinedoto.TaxRate = dataTable.Rows[k]["TaxRate"].ToString();//税率
                    rmlinedotos.Add(rmlinedoto);
                    k++;
                }
            }
            #endregion
            //退回处理单赋值
            foreach (var item in rmlinedotos)
            {
                record = this.Model.AnalyseHead_Analyseline.AddNewUIRecord();
                //record.ID = i;
                //record.SysVersion = i;
                record.AccrualCalculationlinesNo = i.ToString(); i = i + 10;
                record.ForInID = item.ID;//来源ID
                record.Operators = item.Operators;
                record.NoForShipOrReturn = item.DocNo;
                record.ShipDocType = "退回处理单";
                record.Operators_Code = FindOperatorsName(item.Operators.ToString());
                record.Operators_Name = FindOperatorsCode(item.Operators.ToString());
                record.Iteminfo_ID = item.Iteminfo_ID;
                record.Iteminfo_ID_Code = item.Iteminfo_Code;
                record.Iteminfo_ID_Name = item.Iteminfo_Name;
                record.MaterialColor = item.MaterialColor;
                record.SaleUOM = FindBaseUom(item.PU);//计价单位--就是主单位
                record.SaleQty = Convert.ToDecimal("-" + item.RMRtnQtyTU1);//主数量
                record.SupSaleUOM = item.SupSaleUOM;//辅单位
                record.SupSaleQty = item.SupSaleQty;//辅数量
                record.RMApplyQtyTU = item.ApplyQtyTU1;//申请数量
                record.RMApplyMoneyTC = item.ApplyMoneyTC;//申请金额
                record.RMRtnPrice = item.RtnPice;//核定价格
                record.RMRtnMoneyTCNet = item.RtnMoneyTCNet;//核定未税额
                record.PriAndTaxUOM = Convert.ToDecimal("-" + item.RtnMoneyTC);//核定含税额
                record.RMTU = FindBaseUom(item.TU);//交易单位
                record.TotalNMTC = Convert.ToDecimal("-" + item.RtnPice);// 未税金额
                //record.Commissionltem = Calculate(item.TotalNMTC, item.SrcDocNo, item.Iteminfo_ID.ToString());//可提成数
                //string date = Convert.ToDateTime(FindARARApply(item.DocNo)).ToString("yyyy.MM.dd");
                //if (date == "1999-12-31") continue;
                //record.DateReceived = date;//收款日
                //record.DateReceived = FindARARApply(item.DocNo) == "" ? "" : Convert.ToDateTime(FindARARApply(item.DocNo)).ToString("yyyy.MM.dd");//收款日
                //record.OffsetDay = item.MaturityDate == "" ? "" : Convert.ToDateTime(item.MaturityDate).ToString("yyyy.MM.dd");//偏离人------也就是到期日
                //record.LastDateOfDelivery = item.LastDateOfDelivery == "" ? "" : Convert.ToDateTime(item.LastDateOfDelivery).ToString("yyyy.MM.dd");
                //record.DateOflnvoice = FindAR_ARBillLine(item.DocNo) == "" ? "" : Convert.ToDateTime(FindAR_ARBillLine(item.DocNo)).ToString("yyyy.MM.dd");//发票日期
                record.ShipFinallyPriTC = "-" + item.RMOrderPrice;//单价
                record.RMTaxRate = item.TaxRate;//税率
                record.TotalTax = Convert.ToDecimal("-" + item.TotalTaxTC);//税额
                record.SetParentRecord(this.Model.AnalyseHead.FocusedRecord);
            }
            //计算
            money = linedtos.Sum(x => Convert.ToDecimal(x.TotalNMTC));
            decimal money2 = rmlinedotos.Sum(x => Convert.ToDecimal(x.RtnPice));
            decimal money3 = money - money2;
            foreach (var item in this.Model.AnalyseHead.Records)
            {
                item["YuTiMoney"] = money3;
            }
            #region 原方法
            //for (int i = 0; i < datei; i++)
            //{
            //    record = this.Model.AnalyseHead_Analyseline.AddNewUIRecord();
            //    record.ID = i;
            //    record.SysVersion = i;
            //    record.Operators = i;
            //    record.NoForShipOrReturn = i.ToString();
            //    record.ShipDocType = i.ToString();
            //    record.Iteminfo_ID = i;
            //    record.MaterialColor = i.ToString();
            //    record.WXL = i.ToString();
            //    record.SaleUOM = i.ToString();
            //    record.SaleQty = i.ToString();
            //    record.SupSaleQty = i.ToString();
            //    record.SupSaleUOM = i.ToString();
            //    record.ShipFinallyPriTC = i.ToString();
            //    record.PriAndTaxUOM = i.ToString();
            //    record.TotalNMTC = i.ToString();
            //    //退货
            //    record.RMPU = i.ToString();
            //    record.RMApplyQtyTU = i.ToString();
            //    record.RMTU = i.ToString();
            //    record.RMOrderPri = i.ToString();
            //    record.RMTaxRate = i.ToString();
            //    record.RMApplyMoneyTC = i.ToString();
            //    record.RMRtnPrice = i.ToString();
            //    record.RMRtnMoneyTCNet = i.ToString();
            //    record.RMRtnMoneyTC = i.ToString();
            //    record.SetParentRecord(this.Model.AnalyseHead.FocusedRecord);
            //}
            #endregion
        }
        #endregion
    }
}