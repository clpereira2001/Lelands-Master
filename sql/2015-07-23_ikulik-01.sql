use Lelands
GO
------------------------------------------------------------------
delete from dbo.Tags WHERE ID=2
go
------------------------------------------------------------------
------------------------------------------------------------------
if not exists(select * from sys.columns where Name = N'Type_ID' and Object_ID = Object_ID(N'Event'))    
begin
	alter table dbo.Event add Type_ID int null	
end
go
update dbo.Event set Type_ID = 1 
go
alter table dbo.Event alter column Type_ID int not null
go
------------------------------------------------------------------
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[sp_GetEventsList]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[sp_GetEventsList]
GO
--
CREATE PROCEDURE [dbo].[sp_GetEventsList]
	@event_id bigint=null, @title varchar(30)=null, @datestart datetime=null, @dateend datetime=null, @buyerfee decimal=null, @description varchar(30)=null, @closestep tinyint=null, @orderby tinyint
AS
BEGIN
	set nocount on;
	--select * from [Event];
	select ev.ID as [Event_ID]
	     , ev.*  
		 , case when ev.ID = @event_id then 10 else 0 end +
		   case when ev.ID like '' + CAST(@event_id as varchar) + '%' then 1 else 0 end +
		   case when ev.Title = @title and isnull(@title,'') <> '' then 10 else 0  end +
		   case when ev.Title like '' + @title + '%' and isnull(@title,'') <> '' then 1 else 0 end +
		   case when (ev.DateStart between CONVERT(varchar(10), @datestart, 20) + ' 00:00:00.000' and CONVERT(varchar(10), @datestart, 20) + ' 23:59:59.999') then 10 else 0 end +
		   case when (ev.DateEnd between CONVERT(varchar(10), @dateend, 20) + ' 00:00:00.000' and CONVERT(varchar(10), @dateend, 20) + ' 23:59:59.999') then 10 else 0 end +
		   case when ev.[Description] = @description and ISNULL(@description,'') <> '' then 10 else 0 end +
		   case when ev.[Description] like '' + @description + '%' and ISNULL(@description,'') <> '' then 1 else 0 end as rate
	into #Result
	from [Event] ev
	where (ev.ID like '' + CAST(@event_id as varchar) +'%' or @event_id is null)
	  and (ev.Title like '%'+@title +'%' or @title is null)
	  and ((ev.DateStart between CONVERT(varchar(10), @datestart, 20) + ' 00:00:00.000' and CONVERT(varchar(10), @datestart, 20) + ' 23:59:59.999')  or @datestart is null)	
	  and ((ev.DateEnd between CONVERT(varchar(10), @dateend, 20) + ' 00:00:00.000' and CONVERT(varchar(10), @dateend, 20) + ' 23:59:59.999')  or @dateend is null)	
	  and (ev.BuyerFee = @buyerfee or @buyerfee is null)	  
	  and (ev.[Description] like '%' + @description + '%' or @description is null)
      and (ev.CloseStep = @closestep or @closestep is null)
	  
	if (@orderby=0) select * from #Result order by rate desc, [Event_ID] desc
	else if (@orderby=1) select * from #Result order by rate desc, [Event_ID]
	else if (@orderby=2) select * from #Result order by rate desc, DateStart desc, [Event_ID]
	else if (@orderby=3) select * from #Result order by rate desc, DateStart, [Event_ID]
	else if (@orderby=4) select * from #Result order by rate desc, DateEnd desc, [Event_ID]
	else if (@orderby=5) select * from #Result order by rate desc, DateEnd, [Event_ID]
	else if (@orderby=6) select * from #Result order by rate desc, [Title] desc, [Event_ID]
	else if (@orderby=7) select * from #Result order by rate desc, [Title], [Event_ID]	
	else if (@orderby=10) select * from #Result order by rate desc, IsClickable desc, [Event_ID]
	else if (@orderby=11) select * from #Result order by rate desc, IsClickable, [Event_ID]
	else if (@orderby=12) select * from #Result order by rate desc, Ordinary desc, [Event_ID]
	else if (@orderby=13) select * from #Result order by rate desc, Ordinary, [Event_ID]
	else if (@orderby=14) select * from #Result order by rate desc, BuyerFee desc, [Event_ID]
	else if (@orderby=15) select * from #Result order by rate desc, BuyerFee, [Event_ID]
	else if (@orderby=16) select * from #Result order by rate desc, IsViewable desc, [Event_ID]
	else if (@orderby=17) select * from #Result order by rate desc, IsViewable, [Event_ID]
	else if (@orderby=18) select * from #Result order by rate desc, CloseStep desc, [Event_ID]
	else if (@orderby=19) select * from #Result order by rate desc, CloseStep, [Event_ID]
	else if (@orderby=21) select * from #Result order by rate desc, LastUpdate desc, [Event_ID]	
	else select * from #Result order by rate desc, LastUpdate, [Event_ID]
	
	drop table #Result;
END	
GO
------------------------------------------------------------------
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[spAuction_View_Sales]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].spAuction_View_Sales
GO
--
create procedure [dbo].spAuction_View_Sales
	@eventID bigint
as
begin
	set nocount on;		
	select
		a.ID as Auction_ID,
		e.Title as EventTitle,
		mc.Name as MainCategoryTitle,
		c.Title as CategoryTitle,		
		a.Lot,						
		IsNull(img.ThumbNailPath,'') as ThumbnailPath,
		a.Title,
		a.Description,
		a.Price,
		isnull(a.Estimate, '') as Estimate
	from dbo.Auction a join dbo.[Event] e on e.ID = a.Event_ID
		join dbo.[EventCategory] ec on ec.ID = a.Category_ID
		join dbo.[MainCategories] mc on mc.ID = ec.MainCategory_ID
		join dbo.[Category] c on c.ID = ec.Category_ID		
		left join dbo.[Image] img on img.Auction_ID=a.ID and img.[Default]=1		
	where e.ID = @eventID
	  and a.Status = 2
	  and isnull(a.Lot, 0) > 0
end
GO
------------------------------------------------------------------
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[spAuction_View_Detail]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[spAuction_View_Detail]
GO
--
create procedure [dbo].[spAuction_View_Detail]
	@auction_id bigint = -1
as
begin
	SET NOCOUNT ON;
	select
		a.ID as Auction_ID,
		IsNUll(a.Lot, 0) as Lot,
		a.Title,
		IsNull(a.PulledOut, 0) as IsPulledOut,
		a.[Description],		
		a.Addendum,
		a.[Status],
		a.StartDate,
		a.EndDate,
		e.DateStart as EventStartDate,
		e.DateEnd as EventEndDate,
		e.Title as EventTitle,
		e.ID as Event_ID,
		e.CloseStep,
		e.IsCurrent,
		ec.ID as EventCategory_ID,
		ec.MainCategory_ID,
		ec.Category_ID,
		mc.Name as MainCategoryTitle,		
		c.Title as CategoryTitle,		
		aP.ID as PrevAuction_ID,
		aP.Lot as PrevLot,
		aP.Title as PrevTitle,
		mcP.Name as PrevMainCategoryTitle,
		cP.Title as PrevCategoryTitle,
		aN.ID as NextAuction_ID,
		aN.Lot as NextLot,
		aN.Title as NextTitle,
		mcN.Name as NextMainCategoryTitle,
		cN.Title as NextCategoryTitle,
		IsNUll(img.PicturePath,'') as PicturePath,
		a.Owner_ID,
		a.Price,
		isnull(a.Estimate, '') as Estimate
	from dbo.Auction a
		join dbo.[Event] e on e.ID = a.Event_ID
		join dbo.[EventCategory] ec on ec.ID = a.Category_ID
		join dbo.[MainCategories] mc on mc.ID = ec.MainCategory_ID
		join dbo.[Category] c on c.ID = ec.Category_ID		
		left join dbo.PrevNextLot(@auction_id,0) aP on 1=1
		left join dbo.PrevNextLot(@auction_id,1) aN on 1=1
		left join dbo.[EventCategory] ecP on ecP.ID = aP.Category_ID
		left join dbo.[MainCategories] mcP on mcP.ID = ecP.MainCategory_ID
		left join dbo.[Category] cP on cP.ID = ecP.Category_ID
		left join dbo.[EventCategory] ecN on ecN.ID = aN.Category_ID
		left join dbo.[MainCategories] mcN on mcN.ID = ecN.MainCategory_ID
		left join dbo.[Category] cN on cN.ID = ecN.Category_ID
		left join [Image] img on img.Auction_ID=a.ID and (img.[Default]=1 or img.[Order]=1)
	where (ISNULL(@auction_id,-1)=-1 or a.ID=@auction_id)
		and e.IsViewable=1 
		and e.IsClickable=1
		and a.[Status] <> 4
		and a.AuctionType_ID<>3		
end
GO
------------------------------------------------------------------
