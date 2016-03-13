if exists (select * from sysobjects where id = object_id('Config') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
	drop table Config
GO

create table dbo.Config (
	ConfigID Int NOT NULL IDENTITY (1, 1) constraint PKConfig PRIMARY KEY ,
	Aspecto varchar(100) not null,
	Valor varchar (250) NULL
)
go

CREATE INDEX Config_General ON Config( Aspecto ) 
GO

