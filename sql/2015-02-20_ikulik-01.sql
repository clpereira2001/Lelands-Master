use Lelands
GO
------------------------------------------------------------------
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_AUCTIONTAGS_TAGS]') AND parent_object_id = OBJECT_ID(N'[dbo].[AuctionTags]'))
ALTER TABLE dbo.AuctionTags DROP CONSTRAINT FK_AUCTIONTAGS_TAGS
go
--
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Tags]') AND type in (N'U'))
DROP TABLE [dbo].Tags
GO
--
CREATE TABLE [dbo].Tags(	
	ID bigint identity(1,1) not for replication not null,		
	Title nvarchar(512) not null,
	IsSystem bit not null,
	IsViewable bit not null	
)
go 
alter table dbo.Tags add constraint PK_TAGS primary key (ID)
go
insert into dbo.Tags (Title, IsSystem, IsViewable) values ('Best of the best', 1, 1)
go
DBCC CHECKIDENT (Tags, RESEED, 50)
go
------------------------------------------------------------------
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[AuctionTags]') AND type in (N'U'))
DROP TABLE [dbo].AuctionTags
GO
--
CREATE TABLE [dbo].AuctionTags(	
	ID bigint identity(1,1) not for replication not null,		
	AuctionID bigint not null,
	TagID bigint not null
)
go 
alter table dbo.AuctionTags add constraint PK_AUCTIONTAGS primary key (ID)
alter table dbo.AuctionTags add constraint FK_AUCTIONTAGS_AUCTIONS foreign key (AuctionID) references dbo.Auction (ID) on delete cascade
alter table dbo.AuctionTags add constraint FK_AUCTIONTAGS_TAGS foreign key (TagID) references dbo.Tags (ID) on delete cascade
go
------------------------------------------------------------------
------------------------------------------------------------------
------------------------------------------------------------------
------------------------------------------------------------------
------------------------------------------------------------------
------------------------------------------------------------------