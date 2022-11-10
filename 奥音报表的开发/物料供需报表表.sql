ALTER PROCEDURE Cust_MRP_BOMMasterInfo
(
@Item nvarchar(2000),
@ItemMasterCode nvarchar(2000),
@ItemVersions nvarchar(2000),
@PlanName nvarchar(2000),
@InstalmentType nvarchar(2000),
@Outlook nvarchar(2000),
@Org nvarchar(2000),
@StartTime DATETIME,
@EndTime DATETIME,
@SectionTime nvarchar(2000),
@Sorting nvarchar(2000),
@SrcPlanVersion nvarchar(2000)
--,@itemOwnOrg nvarchar(2000)
)
AS
BEGIN
	--料品
	IF(ISNULL(@Item, '')!='')
	BEGIN
		SET @Item='AND Code LIKE '''+'%'+@Item+'%'+''''
		SET @ItemMasterCode='SELECT Code FROM CBO_ItemMaster WHERE ID = ''+@Item+'''
	END
	--料品版本
	IF(ISNULL(@ItemVersions, '')!='')
	BEGIN
		SET @ItemVersions=' AND Code LIKE '''+'%'+@ItemVersions+'%'+''''
	END
	--组织
	IF(ISNULL(@Org, '')!='')
	BEGIN
		SET @Org='AND Code LIKE '''+'%'+@Org+'%'+''''
		--SET @itemOwnOrg='AND itemOwnOrg='+@Org+''
	END
	--计划名称
	--IF(ISNULL(@PlanName, '')!='')
	--BEGIN
		--SET @PlanName=' AND SrcPlanVersion LIKE '''+'%'+@PlanName+'%'+''''
	--END
	--时间区间
	IF(ISNULL(@StartTime, '')!='' and ISNULL(@EndTime, '')!='' )
	BEGIN
		--SET @SectionTime=' AND WorkDate between '''+'%'+@StartTime+'%'+''' AND '''+'%'+@EndTime+'%'+''''
		SET @SectionTime=' AND WorkDate between '''+''+CONVERT(nvarchar(10),@StartTime, 20)+''+''' AND '''+''+CONVERT(nvarchar(10), @EndTime, 20)+''+''''
	END
	--排序
	BEGIN
		SET @Sorting='order by WorkDate'
	END
	--计划名称
	IF(ISNULL(@PlanName, '')!='')
	BEGIN
		SET @PlanName=' AND SrcPlanVersion LIKE '''+'%'+@PlanName+'%'+''''
	END
	--计划名称
	--IF(ISNULL(@PlanName, '')!='')
	--BEGIN
		--SET @PlanName=' AND Code LIKE '''+'%'+@PlanName+'%'+''''
	--END
		SELECT distinct CBO_ItemMaster.Id,
		(select PlanCode from MRP_PlanVersion inner join MRP_PlanName on  MRP_PlanVersion.PlanName=MRP_PlanName.ID
		where MRP_PlanVersion.ID=MRP_PlanOrder.SrcPlanVersion ) as SrcPlanVersion,
		CBO_ItemMaster.Code,CBO_ItemMaster.Name,CBO_ItemMaster.Version,MO_MO.Project,CBO_ItemMaster.InventorySecondUOM,CBO_ItemMaster.DescFlexField_PrivateDescSeg4,CBO_ItemMaster.Org INTO #t789 FROM  (CBO_ItemMaster left JOIN MO_MO ON CBO_ItemMaster.ID=MO_MO.ItemMaster)
		left JOIN MRP_PlanOrder ON CBO_ItemMaster.ID=MRP_PlanOrder.Item
		--left JOIN SPL_SalePlanLine ON CBO_ItemMaster.ID=SPL_SalePlanLine.ItemInfo_ItemID)
		--left JOIN SM_ForecastOrderLine ON CBO_ItemMaster.ID=SM_ForecastOrderLine.ItemInfo_ItemID)
		--left JOIN PR_PRLine ON CBO_ItemMaster.ID=PR_PRLine.ItemInfo_ItemID)
		WHERE MO_MO.DocState!=3
		
		--项目，料号，品名，版本,用量,单位,类别,采购周期
		--select * from	#t789
		-- Id,Code,Name,Version,Project,InventorySecondUOM,DescFlexField_PrivateDescSeg4
		--drop table #T_ItemMaster_INFO_02
		--UsageQty,
		SELECT  UsageQty,FixedScrap,CBO_ItemMaster.id,CBO_ItemMaster.Version,CBO_ItemMaster.InventoryUOM,CBO_ItemMaster.mrpinfo INTO #T_ItemMaster FROM 
		CBO_BOMComponent INNER JOIN CBO_ItemMaster ON CBO_BOMComponent.itemmaster=CBO_ItemMaster.id AND CBO_BOMComponent.itemversioncode=CBO_ItemMaster.Version		
		
		--拿到料品的基础信息--通过上面的两次SQL语句，筛选拿到基础的信息
		--#T_ItemMaster.UsageQty,
		SELECT #T_ItemMaster.UsageQty,#t789.id,#t789.SrcPlanVersion,#T_ItemMaster.FixedScrap,#t789.Org, #t789.Version, #t789.code,#t789.name,#t789.DescFlexField_PrivateDescSeg4, #T_ItemMaster.InventoryUOM,#T_ItemMaster.mrpinfo 	INTO #T_ItemMaster_INFO  FROM    #T_ItemMaster INNER JOIN #t789 ON #t789.id=#T_ItemMaster.id AND #t789.Version=#T_ItemMaster.Version
		-- 获取mrpinfo的信息
		--SELECT * FROM #T_ItemMaster_INFO
		--获取问的临时表
		
		-- SELECT * FROM CBO_Operation_Trl WHERE id=
		-- (SELECT CBO_Operation.ID FROM CBO_Routing INNER JOIN 
		-- CBO_Operation ON CBO_Routing.ID=CBO_Operation.Routing WHERE CBO_Routing.ItemMaster= '''+@Item+''')
		-- AND SysMLFlag='zh-CN'
		--站别的获取，--只有制造件才有

		SELECT  #T_ItemMaster_INFO.ID,#T_ItemMaster_INFO.SrcPlanVersion,#T_ItemMaster_INFO.DescFlexField_PrivateDescSeg4,FixedScrap,Version,Code,Name,#T_ItemMaster_INFO.Org,InventoryUOM,MrpInfo,CBO_Routing.ID 
		AS CBO_RoutingID 
		INTO #T_ItemMaster_INFO_02 FROM #T_ItemMaster_INFO  LEFT JOIN CBO_Routing  ON  #T_ItemMaster_INFO.ID=CBO_Routing.ItemMaster

		--Alter table #T_ItemMaster_INFO_02 add Fdate date not null default  convert(varchar(10),getdate(),120)
		--select * from	#T_ItemMaster_INFO_02
		--select * from	#TEMP1		
		--把上面的整合一下
		--结果02 站别只存了一个CBO_Routing 的 itemmaster

		DECLARE @Sql NVARCHAR(MAX)
		SET　@Sql='SELECT 
		(SELECT sum(mrpqty)  FROM MRP_PlanOrder WHERE ItemCode=#T_ItemMaster_INFO_02.Code  AND adjustactiondate>=adjustactiondate AND 		adjustactiondate<=DATEADD(DAY, 7,adjustactiondate)) AS Anquankucun,
		((select  SUM(StoreQty) StoreQty from  InvTrans_WhQoh where ItemInfo_ItemID=#T_ItemMaster_INFO_02.ID AND Wh = ''1002107210116126'' AND itemOwnOrg=''1002107150542063'')+(select  SUM(StoreQty) StoreQty from  InvTrans_WhQoh where ItemInfo_ItemID=#T_ItemMaster_INFO_02.ID AND Wh = ''1002107210116261'' AND itemOwnOrg=''1002107150542063'')) AS KUCUN,		
	    (SELECT SUM(mrpqty) FROM MRP_PlanOrder WHERE item=#T_ItemMaster_INFO_02.ID AND adjustactiondate>=adjustactiondate AND adjustactiondate<=
	    DATEADD(DAY, 7, adjustactiondate)) 	AS LTAnquankucun,	
	   (SELECT sum(mrpqty) FROM MRP_PlanOrder WHERE item=#T_ItemMaster_INFO_02.ID AND adjustactiondate>=adjustactiondate AND adjustactiondate<=DATEADD(DAY, 28, adjustactiondate)) 	AS FAnquankucun,				
	   (SELECT  description FROM CBO_Operation_Trl WHERE id=(SELECT CBO_Operation.ID FROM CBO_Routing INNER JOIN 		 CBO_Operation ON CBO_Routing.ID=CBO_Operation.Routing WHERE CBO_Routing.ItemMaster=#T_ItemMaster_INFO_02.ID)
		AND SysMLFlag=''zh-CN'')AS StandBy,
		(select Base_UOM_Trl.Name from	Base_UOM inner join Base_UOM_Trl on Base_UOM.ID=Base_UOM_Trl.ID 
		 where	Base_UOM.ID=#T_ItemMaster_INFO_02.InventoryUOM and Base_UOM_Trl.SysMLFlag=''zh-CN'') AS InventoryUOM_1,
		 DescFlexField_PrivateDescSeg4,FixedScrap,Version,Code,Name,Org,InventoryUOM,
		 (select PurProcessLT from CBO_MrpInfo where ID=mrpinfo) as mrpinfo
		 ,CBO_RoutingID,CONVERT(nvarchar(10), WorkDate, 120) AS WorkDate,SrcPlanVersion
		 FROM #T_ItemMaster_INFO_02 left join Base_WorkCalendarDay on Base_WorkCalendarDay.WorkCalendar=''1002107150541805''
        where 1=1 '

		
		--UPDATE #T_ItemMaster_INFO_02 SET Fdate = '' 
		
		--UPDATE #T_ItemMaster_INFO_02 SET Fdate = '2022-09-24' WHERE ID = '1002108010244189' 
		--UPDATE #T_ItemMaster_INFO_02 SET Fdate = '2022-09-23' WHERE ID = '1002108010244213' 


		--DECLARE   @BDate varchar(10);   --起始日期，格式:'YYYY-MM-DD'

		--DECLARE   @EDate varchar(10);   --结束日期，格式:'YYYY-MM-DD'

		--SET @BDate='2014-01-01';

		--SET @EDate='2014-02-28';

		----CREATE TABLE #T_ItemMaster_INFO_02 ADD  FDAY char(128);
		----drop table #TEMP1
		----update #T_ItemMaster_INFO_02 set FDAY=''
		--CREATE TABLE #TEMP1(FDAY VARCHAR(10)  NULL );

		--DECLARE @dtDay DATETIME;

		--DECLARE @smDay VARCHAR(10);

		--SET @smDay=@BDate;

		--WHILE (@smDay<=@EDate)

		--BEGIN

		--INSERT INTO #TEMP1(FDAY) VALUES (@smDay);

		--SET @dtDay = CAST(@smDay AS DATETIME);

		--SET @smDay= CONVERT(VARCHAR(10),@dtDay+2 ,120);

		--END
				

		--CREATE TABLE #TEMP1 ADD  ID  VARCHAR(100);

		--insert into #T_ItemMaster_INFO_02(ID) 
		--select ID
		--from #TEMP1
		--select *from #TEMP1	

		--SELECT 
		--(SELECT sum(mrpqty)  FROM MRP_PlanOrder WHERE ItemCode=#T_ItemMaster_INFO_02.Code  AND adjustactiondate>=adjustactiondate AND 		adjustactiondate<=DATEADD(DAY, 7,adjustactiondate)) AS Anquankucun,
		--((select  SUM(StoreQty) StoreQty from  InvTrans_WhQoh where ItemInfo_ItemID=#T_ItemMaster_INFO_02.ID AND Wh = '1002107210116126' AND itemOwnOrg='1002107150542063')+(select  SUM(StoreQty) StoreQty from  InvTrans_WhQoh where ItemInfo_ItemID=#T_ItemMaster_INFO_02.ID AND Wh = '1002107210116261' AND itemOwnOrg='1002107150542063')) AS KUCUN,		
	    --(SELECT SUM(mrpqty) FROM MRP_PlanOrder WHERE item=#T_ItemMaster_INFO_02.ID AND adjustactiondate>=adjustactiondate AND adjustactiondate<=
	    --DATEADD(DAY, 7, adjustactiondate)) 	AS LTAnquankucun,	
	    --(SELECT sum(mrpqty) FROM MRP_PlanOrder WHERE item=#T_ItemMaster_INFO_02.ID AND adjustactiondate>=adjustactiondate AND adjustactiondate<=DATEADD(DAY, 28, adjustactiondate)) 	AS FAnquankucun,				
	    --(SELECT  description FROM CBO_Operation_Trl WHERE id=(SELECT CBO_Operation.ID FROM CBO_Routing INNER JOIN CBO_Operation ON CBO_Routing.ID=CBO_Operation.Routing WHERE CBO_Routing.ItemMaster=#T_ItemMaster_INFO_02.ID)
		--AND SysMLFlag='zh-CN')AS StandBy,
		--(select Base_UOM_Trl.Name from	Base_UOM inner join Base_UOM_Trl on Base_UOM.ID=Base_UOM_Trl.ID 
		--where	Base_UOM.ID=#T_ItemMaster_INFO_02.InventoryUOM and Base_UOM_Trl.SysMLFlag='zh-CN') AS InventoryUOM,
		--* FROM #T_ItemMaster_INFO_02 where 1=1


--RAW库存和WIP库存 这个必须要有料品才可以查询
--select  A5.[Name] as [Wh_Name],A.[ItemInfo_ItemCode] as [Item_ItemCode], isnull( case  when A2.[ItemFormAttribute] in (16, 22) then A.[ItemInfo_ItemName] else A2.[Name] end ,'') as 
--[ItemSeg_Name], sum(A.[ToRetStQty]) as [PUToRetQty], A3.[Round_Precision] as [Round1_Precision], convert(decimal(24,9),0) as [InOnWayQty], convert(decimal(24,9),0) as [OutOnWayQty], sum( case  when ((((A.[IsProdCancel] = 1) or (A.[MO_EntityID] != 0)) or A.[ProductDate] is not null) or (A.[WP_EntityID] != 0)) then A.[StoreQty] else convert(decimal(24,9),0) end ) as [NotUseQty], sum((((A.[StoreQty] - A.[ResvStQty]) - A.[ResvOccupyStQty]) -  case  when ((((A.[IsProdCancel] = 1) or (A.[MO_EntityID] != 0)) or A.[ProductDate] is not null) or (A.[WP_EntityID] != 0)) then A.[StoreQty] else convert(decimal(24,9),0) end )) as [CanUseQty], sum(A.[ResvStQty]) as [ReservQty], sum((A.[StoreQty] + A.[ToRetStQty])) as [BalQty], sum((A.[StoreMainQty] + A.[ToRetStMainQty])) as [BalQty_Main], sum((((((A.[StoreQty] - A.[ResvStQty]) - A.[ResvOccupyStQty]) -  case  when ((((A.[IsProdCancel] = 1) or (A.[MO_EntityID] != 0)) or A.[ProductDate] is not null) or (A.[WP_EntityID] != 0)) then A.[StoreQty] else convert(decimal(24,9),0) end ) + A.[SupplyQtySU]) - A.[DemandQtySU])) as [Temp_PAB], 
--convert(bigint,0) as [Item_ItemID], convert(bigint,0) as [W_Uom], convert(bigint,0) as [MainBaseSU_ID] into #TempTableQoh from  InvTrans_WhQoh as A  left join [CBO_Wh] as A1 
--on (A.[Wh] = A1.[ID])  left join [CBO_ItemMaster] as A2 on (A.[ItemInfo_ItemID] = A2.[ID])  left join [Base_UOM] as A3 on (A.[StoreUOM] = A3.[ID]) 
--left join [Base_Organization] as A4 on (A.[LogisticOrg] = A4.[ID])  left join [CBO_Wh_Trl] as A5 on (A5.SysMlFlag = 'zh-CN') and (A1.[ID] = A5.[ID])
--where  ((((A2.[Name] is not null and (A2.[Name] != '')) and (A4.[Code] = N'')) and (A1.[Code] = N'RAW') or (A1.[Code] = N'WIP')) and (A.[ItemInfo_ItemCode] = '@ItemMasterCode')) 
--group by A5.[Name], A.[ItemInfo_ItemCode], isnull( case  when A2.[ItemFormAttribute] in (16, 22) then A.[ItemInfo_ItemName] else A2.[Name] end ,''), A3.[Round_Precision]

--TempTableQoh -> 指的是库存查来的。
--SELECT * FROM #TempTableQoh WHERE Item_ItemCode= '@Item'
--安全库存
--SELECT TOP(100) mrpqty,adjustactiondate,* FROM MRP_PlanOrder WHERE item= '@Item' AND adjustactiondate>=adjustactiondate AND adjustactiondate<=
--DATEADD(DAY, 7, adjustactiondate) 

--LT内下单
--SELECT TOP(100) mrpqty,adjustactiondate,* FROM MRP_PlanOrder WHERE item= '@Item' AND adjustactiondate>=adjustactiondate AND adjustactiondate<=
--DATEADD(DAY, 7, adjustactiondate) 

--未来4W需求
--SELECT TOP(100) mrpqty,adjustactiondate,* FROM MRP_PlanOrder WHERE item='@Item' AND adjustactiondate>=adjustactiondate AND adjustactiondate<=
--DATEADD(DAY, 28, adjustactiondate) 

--安全库存和LT内下单和未来4W需求三个变量子查询解决

exec (@Sql+@Item+@ItemVersions+@PlanName+@InstalmentType+@Outlook+@Org+@SectionTime+@Sorting)
--print  (@SectionTime)
--print  (@Sql+@PlanName+@StartTime+@EndTime)

END