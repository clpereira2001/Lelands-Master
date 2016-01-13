USE [Lelands]
GO

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[spInvoice_ConsignorStatements]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[spInvoice_ConsignorStatements]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

create procedure [dbo].[spInvoice_ConsignorStatements]	
	@user_id bigint
as
begin
	set nocount on;
	select 
		con.Id as Consignment_ID,		
		e.Title as EventTitle		
	from dbo.[Consignments] con
		join dbo.[Event] e on e.ID=con.Event_ID		
	where con.[User_ID]=ISNULL(@user_id, -1) and e.ID>=910 --and e.CloseStep=2 
	group by con.Id, e.Title, con.ConsDate
	order by con.ConsDate desc
end

GO