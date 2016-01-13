use Lelands
GO
------------------------------------------------------------------
if not exists(select * from sys.columns where Name = N'WebTitle' and Object_ID = Object_ID(N'Collections'))    
begin
	alter table dbo.Collections add WebTitle nvarchar(512) null	
end
go
update dbo.Collections set WebTitle = Title 
go
alter table dbo.Collections alter column WebTitle nvarchar(512) not null
go
------------------------------------------------------------------

