 if exists (select * from sysobjects where id = object_id('Permiso') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
	drop table Permiso
GO

create table dbo.Permiso (
	PermisoID Int NOT NULL IDENTITY (1, 1) constraint PKPermiso PRIMARY KEY ,
	Nombre varchar (100) NOT NULL ,
	Notas text null,
	EsABM bit not null,
	Activo bit not null
)
go

CREATE INDEX Permiso_Nombre ON Permiso( Nombre ) 
GO
