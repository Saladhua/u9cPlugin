--�µı���
--����Ʒ�����ϸ��
--TEXT
--���� ȥ����춯��
ALTER PROCEDURE Cust_Non_PerformingInventory
(
@Item nvarchar(2000),
@Project nvarchar(2000),
@Org nvarchar(2000),
@DateTime nvarchar(2000),
@DateTimeForS nvarchar(2000)
)
AS
BEGIN
	--��Ʒ
	IF(ISNULL(@Item, '')!='')
	BEGIN
		SET @Item='AND A.ItemInfo_ItemID LIKE '''+'%'+@Item+'%'+''''
	END
	--�汾
	IF(ISNULL(@Project, '')!='')
	BEGIN
		SET @Project='AND A.Project LIKE '''+'%'+@Project+'%'+''''
	END
	--��֯
	IF(ISNULL(@Org, '')!='')
	BEGIN
		SET @Org='AND A.Org LIKE '''+'%'+@Org+'%'+''''
	END
	--��������
		BEGIN	
			DECLARE  @DateTimeForS_1 nvarchar(2000)
			set @DateTimeForS_1=getdate()
		END
	IF(ISNULL(@DateTime, '')!='')
	BEGIN
		--SET @DateTimeForS='AND A10.ApprovedOn LIKE '''+'%'+CONVERT(nvarchar(10),@DateTime, 20)+'%'+''''	
		DECLARE @DateTimeForS_3 nvarchar(2000)
		set @DateTimeForS_3=right(@DateTime,18)--:��ȡ�ַ�������
		set @DateTime=left(@DateTime,18)--:��ȡ�ַ�������
		SET @DateTimeForS=' AND A10.ApprovedOn between '''+''+@DateTime+''+''' AND '''+''+@DateTimeForS_3+''+''''
		set @DateTimeForS_1=CONVERT(nvarchar(10), @DateTime, 20)
	END
	BEGIN	
			DECLARE  @DateTimeForS_2 nvarchar(2000)
			--set @DateTimeForS_2='convert(int,'''+@DateTimeForS_1+''', 20) - A10.ApprovedOn)) as Temp_Age'
			set @DateTimeForS_2='convert(int,(convert(datetime,'''+@DateTimeForS_1+''') - A10.ApprovedOn)) as Temp_Age'
			--convert(int,(convert(datetime,'2022-01-20') - A10.ApprovedOn)) as Temp_Age
	END
SELECT 
A.ItemInfo_ItemID,
case  when ((A.[DocType_EntityType] != 'UFIDA.U9.AP.APMatch.APMatchDocType') and (A.[BackLineType] = 0))
then (Power((-(1)),(A.[Direction] + A.[DisplayDirection])) * A4.[StoreUOMQty])
else convert(decimal(24,9),0) end  as [Main_StoreUOMQty]
into #sum_Main_StoreUOMQty_1
FROM InvTrans_TransLine A 
left join [InvTrans_TransLineBin] as A4 on (A.[ID] = A4.[TransLine]) 
left join [Base_Organization] as A7 on (A.[Org] = A7.[ID]) 
left join [CBO_Wh] as A3 on (A.[Wh] = A3.[ID]) 
WHERE
(A7.[Code] = N'10')
and A3.[Code] in ('MRB-Pend', 'MRB-RTV')
and A.[DisplayDirection] = 1 and A.[QtyPriceDealFlg] in (0, 1)
--���� Ҳ��ȥ����춯��
SELECT 
A.ItemInfo_ItemID,
case  when ((A.[DocType_EntityType] != 'UFIDA.U9.AP.APMatch.APMatchDocType') and (A.[BackLineType] = 0))
then (Power((-(1)),(A.[Direction] + A.[DisplayDirection])) * A4.[StoreUOMQty])
else convert(decimal(24,9),0) end  as [Main_StoreUOMQty]
into #sum_Main_StoreUOMQty_0
FROM InvTrans_TransLine A 
left join [InvTrans_TransLineBin] as A4 on (A.[ID] = A4.[TransLine]) 
left join [Base_Organization] as A7 on (A.[Org] = A7.[ID]) 
left join [CBO_Wh] as A3 on (A.[Wh] = A3.[ID]) 
WHERE
(A7.[Code] = N'10')
and A3.[Code] in ('MRB-Pend', 'MRB-RTV')
and A.[DisplayDirection] = 0 and A.[QtyPriceDealFlg] in (0, 1)
--drop table #sum_Main_StoreUOMQty
--�洢���̵ľ����SQL���-1.0�汾
--���β��š������ʩ���ƻ��������    ˽���ֶ�
		DECLARE @Sql NVARCHAR(MAX)
		DECLARE @Sql_2 NVARCHAR(MAX)

		--DECLARE @DateTime_2 datetime 
		--BEGIN
		--	SET  @DateTime_2=@DateTime
		--END
		SET��@Sql='SELECT
--TransInWh,ItemInfo_ItemID,Project,
(SELECT Name FROM CBO_ItemMaster WHERE ID=A.ItemInfo_ItemID) AS ��ƷName,
(SELECT Code FROM CBO_ItemMaster WHERE ID=A.ItemInfo_ItemID) AS ��ƷCode,
(SELECT Name FROM CBO_Project INNER JOIN CBO_Project_Trl ON CBO_Project.ID=CBO_Project_Trl.ID
WHERE CBO_Project.ID=A.Project AND SysMLFlag=''zh-CN'') AS ��ĿProjectName,
(SELECT Version FROM CBO_ItemMaster WHERE ID=A.ItemInfo_ItemID) AS �汾Version,
(SELECT Name FROM CBO_Category_Trl WHERE ID=(SELECT AssetCategory FROM CBO_ItemMaster WHERE ID=A.ItemInfo_ItemID) and SysMLFlag=''zh-CN'') AS ��Ʒ������,
(SELECT Name FROM Base_UOM_Trl WHERE ID=(SELECT InventoryUOM FROM CBO_ItemMaster WHERE ID=A.ItemInfo_ItemID) and SysMLFlag=''zh-CN'') AS ��λInventoryUOMName,
(SELECT Name FROM Base_Organization_Trl WHERE ID=A.Org and SysMLFlag=''zh-CN'') AS ��֯OrgName,
(SELECT sum(Main_StoreUOMQty) FROM #sum_Main_StoreUOMQty_1 WHERE ItemInfo_ItemID=A.ItemInfo_ItemID) as ��������CKSL,
(SELECT sum(Main_StoreUOMQty) FROM #sum_Main_StoreUOMQty_0 WHERE ItemInfo_ItemID=A.ItemInfo_ItemID) as �������RKSL,
((SELECT sum(Main_StoreUOMQty) FROM #sum_Main_StoreUOMQty_0 WHERE ItemInfo_ItemID=A.ItemInfo_ItemID)-
(SELECT sum(Main_StoreUOMQty) FROM #sum_Main_StoreUOMQty_1 WHERE ItemInfo_ItemID=A.ItemInfo_ItemID)) as totalQty,
a.CostPrice as ����CostPricce,
(((SELECT sum(Main_StoreUOMQty) FROM #sum_Main_StoreUOMQty_0 WHERE ItemInfo_ItemID=A.ItemInfo_ItemID)-
(SELECT sum(Main_StoreUOMQty) FROM #sum_Main_StoreUOMQty_1 WHERE ItemInfo_ItemID=A.ItemInfo_ItemID))*
a.CostPrice) as CostPriceQty,
(select x.Name from CBO_Department z inner join CBO_Department_Trl x on z.ID=x.ID where Code=a.DescFlexSegments_PrivateDescSeg4 and x.SysMLFlag=''zh-CN'') as ���β���,
DescFlexSegments_PrivateDescSeg5 as �����ʩ,
DescFlexSegments_PrivateDescSeg6 as �ƻ��������,
(select   A1.[Name]
from  Base_DefineValue as A  left join [Base_DefineValue_Trl] as A1 on (A1.SysMlFlag = ''zh-CN'') 
and (A.[ID] = A1.[ID]) where  (A.[ValueSetDef] = 1002108010458827) and Code=DescFlexSegments_PrivateDescSeg1)
 as �������,
 (select  A1.[Name] as [Name]
from  CBO_Operators as A 
left join [CBO_Operators_Trl] as A1 on (A1.SysMlFlag = ''zh-CN'')and (A.[ID] = A1.[ID])
left join [CBO_Department] as A2 on (A.[Dept] = A2.[ID])
left join [Base_Organization] as A3 on (A.[Org] = A3.[ID]) 
where A.Code=DescFlexSegments_PrivateDescSeg7) as ������,
DescFlexSegments_PrivateDescSeg2 as ����ԭ��,
(SELECT BinInfo_Code FROM InvDoc_TransInBin WHERE TransInLine=A.id) as ��λ,
A10.BusinessDate as �������,
(select Name from CBO_Wh_Trl where ID=TransInWh) as �洢�ص�,
 A10.ApprovedOn as Shrq,'
SET��@Sql_2='
FROM InvDoc_TransInLine A 
inner join InvDoc_TransferIn A10 on A.TransferIn=A10.ID
WHERE
TransInWh in(1002107200005442,1002107200005814,1002108160005856,1002108160005874) and 1=1'
exec (@Sql+@DateTimeForS_2+@Sql_2+@Item+@Project+@Org+@DateTimeForS)
--print  (@Sql+@DateTimeForS_2+@Sql_2)
--print  (@DateTimeForS)
--print  (@Sql+@PlanName+@StartTime+@EndTime)
--convert(int,('+@DateTime+' - A10.ApprovedOn)) as [Temp_Age]
--convert(int,(@DateTime_2 - A10.ApprovedOn)) as Temp_Age

END

--convert(int, - A10.ApprovedOn)) as [Temp_Age]
--convert(int,@DateTime - A10.ApprovedOn)) as [Temp_Age]
--SELECT
--TransInWh,ItemInfo_ItemID,Project,
--A.id,
--(SELECT Name FROM CBO_ItemMaster WHERE ID=A.ItemInfo_ItemID) AS ��ƷName,
--(SELECT Code FROM CBO_ItemMaster WHERE ID=A.ItemInfo_ItemID) AS ��ƷCode,
--(SELECT Name FROM CBO_Project INNER JOIN CBO_Project_Trl ON CBO_Project.ID=CBO_Project_Trl.ID
--WHERE CBO_Project.ID=A.Project AND SysMLFlag='zh-CN') AS ��ĿProjectName,
--(SELECT Version FROM CBO_ItemMaster WHERE ID=A.ItemInfo_ItemID) AS �汾Version,
--(SELECT Name FROM CBO_Category_Trl WHERE ID=(SELECT MainItemCategory FROM CBO_ItemMaster WHERE ID=A.ItemInfo_ItemID) and SysMLFlag='zh-CN') AS ��Ʒ������,
--(SELECT Name FROM Base_UOM_Trl WHERE ID=(SELECT InventoryUOM FROM CBO_ItemMaster WHERE ID=A.ItemInfo_ItemID) and SysMLFlag='zh-CN') AS ��λInventoryUOMName,
--(SELECT Name FROM Base_Organization_Trl WHERE ID=A.Org and SysMLFlag='zh-CN') AS ��֯OrgName,
--(SELECT sum(Main_StoreUOMQty) FROM #sum_Main_StoreUOMQty_1 WHERE ItemInfo_ItemID=A.ItemInfo_ItemID) as ��������CKSL,
--(SELECT sum(Main_StoreUOMQty) FROM #sum_Main_StoreUOMQty_0 WHERE ItemInfo_ItemID=A.ItemInfo_ItemID) as �������RKSL,
--((SELECT sum(Main_StoreUOMQty) FROM #sum_Main_StoreUOMQty_0 WHERE ItemInfo_ItemID=A.ItemInfo_ItemID)-
--(SELECT sum(Main_StoreUOMQty) FROM #sum_Main_StoreUOMQty_1 WHERE ItemInfo_ItemID=A.ItemInfo_ItemID)) as totalQty,
--a.CostPrice as ����CostPricce,
--(((SELECT sum(Main_StoreUOMQty) FROM #sum_Main_StoreUOMQty_0 WHERE ItemInfo_ItemID=A.ItemInfo_ItemID)-
--(SELECT sum(Main_StoreUOMQty) FROM #sum_Main_StoreUOMQty_1 WHERE ItemInfo_ItemID=A.ItemInfo_ItemID))*
--a.CostPrice) as CostPriceQty,
--(select x.Name from CBO_Department z inner join CBO_Department_Trl x on z.ID=x.ID where Code=a.DescFlexSegments_PrivateDescSeg4 and x.SysMLFlag='zh-CN') as ���β���,
--DescFlexSegments_PrivateDescSeg5 as �����ʩ,
--DescFlexSegments_PrivateDescSeg6 as �ƻ��������,
--DescFlexSegments_PrivateDescSeg1 as �������,
--DescFlexSegments_PrivateDescSeg2 as ����ԭ��,
--A10.BusinessDate as �������,

--FROM InvDoc_TransInLine A 
--inner join InvDoc_TransferIn A10 on A.TransferIn=A10.ID
--WHERE
--TransInWh in(1002107200005442,1002107200005814,1002108160005856,1002108160005874) and 1=1

--��������
--430407600322
--����
--convert(int,(convert(datetime,'2022/11/11 0:00:00') - max(A.[DocDate]))) as [Temp_Age],

--����
--select ApprovedOn from InvDoc_TransferIn

-- select top(10) * from InvDoc_TransInLine 



--�����뵥
--�洢�ص��ĸ�ԭ������֯�Ĳ�ͬ
--SELECT TransInWh,ItemInfo_ItemID,Project,* FROM InvDoc_TransInLine WHERE
--TransInWh in(1002107200005442,1002107200005814,1002108160005856,1002108160005874)
--TransInWh='1002107200005442'
--OR TransInWh='1002107200005814'
--OR TransInWh='1002108160005856'
--OR TransInWh='1002108160005874'

--�洢�ص�
--1002107200005442
--1002107200005814
--1002108160005856
--1002108160005874
--SELECT org,* FROM CBO_Wh a inner join CBO_Wh_Trl b on a.ID=b.ID WHERE Code='MRB-Pend' or Code='MRB-RTV'


--ȡ��Ʒ���йصĶ��� CBO_ItemMaster
--SELECT Code,Name,Version,InventoryUOM,* FROM CBO_ItemMaster WHERE ID='1002107310607591'
--

--��֯
--SELECT Name FROM Base_Organization_Trl WHERE ID='1002107150542063' and SysMLFlag='zh-CN'

--�������λ�Ĳ�ѯ
--SELECT Code,* FROM Base_UOM WHERE ID='1002107260440316'
--SELECT Name FROM Base_UOM_Trl WHERE ID='1002107260440316' AND	SysMLFlag='zh-CN'
--���
--SELECT Name FROM Base_UOM INNER JOIN Base_UOM_Trl ON Base_UOM.ID=Base_UOM_Trl.ID
--WHERE Base_UOM.ID='1002107260440316' AND SysMLFlag='zh-CN'
--��Ŀ
--SELECT Name FROM CBO_Project INNER JOIN CBO_Project_Trl ON CBO_Project.ID=CBO_Project_Trl.ID
--WHERE CBO_Project.ID='1002107270111476' AND SysMLFlag='zh-CN'
--��Ʒ-Ʒ��

--TEXT
--���β��ţ������ʩ���ƻ��������
--SELECT Code,* FROM CBO_Project WHERE ID='1002107270111476'
--���ʱ��
--��λ













