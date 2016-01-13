use Lelands
GO
------------------------------------------------------------------
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_AUCTIONCOLLECTIONS_COLLECTIONS]') AND parent_object_id = OBJECT_ID(N'[dbo].[AuctionCollections]'))
ALTER TABLE dbo.AuctionCollections DROP CONSTRAINT FK_AUCTIONCOLLECTIONS_COLLECTIONS
go
--
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Collections]') AND type in (N'U'))
DROP TABLE [dbo].Collections
GO
--
CREATE TABLE [dbo].Collections(	
	ID bigint identity(1,1) not for replication not null,		
	Title nvarchar(512) not null,
	Description nvarchar(MAX) not null
)
go 
alter table dbo.Collections add constraint PK_COLLECTIONS primary key (ID)
go
------------------------------------------------------------------
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[AuctionCollections]') AND type in (N'U'))
DROP TABLE [dbo].AuctionCollections
GO
--
CREATE TABLE [dbo].AuctionCollections(	
	ID bigint identity(1,1) not for replication not null,		
	AuctionID bigint not null,
	CollectionID bigint not null
)
go 
alter table dbo.AuctionCollections add constraint PK_AuctionCollections primary key (ID)
alter table dbo.AuctionCollections add constraint FK_AUCTIONCOLLECTIONS_AUCTIONS foreign key (AuctionID) references dbo.Auction (ID) on delete cascade
alter table dbo.AuctionCollections add constraint FK_AUCTIONCOLLECTIONS_COLLECTIONS foreign key (CollectionID) references dbo.Collections (ID) on delete cascade
go
------------------------------------------------------------------
------------------------------------------------------------------
------------------------------------------------------------------
------------------------------------------------------------------
------------------------------------------------------------------