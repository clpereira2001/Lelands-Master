use Lelands
go
----------------------------------------------------------------------------------------------------------------
If not exists(select * from Information_Schema.Columns where Table_Name='User' and Column_Name='IsCatalog')
begin
	alter table [User] add IsCatalog bit not null constraint DF_USER_ISCATALOG default(0)
end
go
----------------------------------------------------------------------------------------------------------------
If not exists(select * from Information_Schema.Columns where Table_Name='User' and Column_Name='IsPostCards')
begin
	alter table [User] add IsPostCards bit not null constraint DF_USER_ISPOSTCARDS default(0)
end
go
