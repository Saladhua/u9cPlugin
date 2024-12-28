--   刷新生产领料单行

select  ID from Base_Organization where Code=''

select IsCalcCost,ConsignProcessItemSrc from  MO_IssueDocLine  where ID in (select b.ID from MO_IssueDoc a
left join MO_IssueDocLine b on a.ID = b.IssueDoc
left join MO_MO c on b.MO = c.ID
left join CBO_Project d on c.Project = d.ID
where  a.Org = 1002202243062150 and Convert(char(7),a.BusinessCreatedOn,120) = '2024-11') and IsCalcCost = 0 

--刷新生产领料单成本
select IsProductCost from  MO_MOIssueCost   where ID in
(select d.ID from MO_IssueDoc a
left join MO_IssueDocLine b on a.ID = b.IssueDoc
left join MO_MO c on b.MO = c.ID
left join MO_MOIssueCost d on b.ID = d.IssueDocLine
left join CBO_Project e on c.Project = e.ID
where   a.Org = 1002202243062150 and Convert(char(7),a.BusinessCreatedOn,120) = '2024-11') and IsProductCost = 0 
 

--更新备料:MO_MOPickList，计算成本:IsCalcCost = 0（不勾），项目号包含NY
select    b.IsCalcCost  ,b.ConsignProcessItemSrc  from MO_MO a
left join MO_MOPickList b on a.ID = b.MO
left join CBO_Project c on b.Project = c.ID
where  a.Org = 1002202243062150 and  b.IsCalcCost = 0