 if exists (select * from sysobjects where id = object_id('Rol') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
	drop table Rol
GO

create table dbo.Rol (
	RolID Int NOT NULL IDENTITY (1, 1) constraint PKRol PRIMARY KEY ,
	Nombre varchar (100) NOT NULL ,
	Notas text null
)
go

CREATE INDEX Rol_Nombre ON Rol( Nombre ) 
GO

Insert into Rol(Nombre,Notas)
values ('Administrador','')
