 if exists (select * from sysobjects where id = object_id('UsuarioRol') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
	drop table UsuarioRol
GO

create table dbo.UsuarioRol (
	UsuarioRolID Int NOT NULL IDENTITY (1, 1) constraint PKUsuarioRol PRIMARY KEY ,
	UsuarioID int not null CONSTRAINT FKUsuarioRol_Usuario FOREIGN KEY (UsuarioID) REFERENCES Usuario(UsuarioID),
	RolID int not null CONSTRAINT FKUsuarioRol_Rol FOREIGN KEY (RolID) REFERENCES Rol(RolID)
)
go

CREATE INDEX UsuarioRol_Usuario ON UsuarioRol( UsuarioID ) 
GO

CREATE INDEX UsuarioRol_Rol ON UsuarioRol( RolID ) 
GO

INSERT INTO UsuarioRol(UsuarioID,RolID)
VALUES(1,1)
