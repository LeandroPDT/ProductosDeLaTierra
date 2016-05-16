 if exists (select * from sysobjects where id = object_id('Log') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
	drop table Log
GO

create table dbo.Log (
	LogID Int NOT NULL IDENTITY (1, 1) constraint PKLog PRIMARY KEY ,
	NombreTabla varchar(100) null, 
	ID int not null,
	Fecha datetime not null,
	Tipo varchar(50) null, 
	UsuarioID int not NULL CONSTRAINT FKLog_Usuario FOREIGN KEY (UsuarioID) REFERENCES Usuario(UsuarioID)
)
go

CREATE INDEX Log_general ON Log( NombreTabla, ID, Fecha ) 
GO
CREATE INDEX Log_Usuario ON Log( UsuarioID ) 
GO

