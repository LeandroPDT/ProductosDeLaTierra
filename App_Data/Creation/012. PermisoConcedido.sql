 if exists (select * from sysobjects where id = object_id('PermisoConcedido') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
	drop table PermisoConcedido
GO

create table dbo.PermisoConcedido (
	PermisoConcedidoID Int NOT NULL IDENTITY (1, 1) constraint PKPermisoConcedido PRIMARY KEY ,
	PermisoID int not null CONSTRAINT FKPermisoConcedido_Permiso FOREIGN KEY (PermisoID) REFERENCES Permiso(PermisoID),
	UsuarioID int null CONSTRAINT FKPermisoConcedido_Usuario FOREIGN KEY (UsuarioID) REFERENCES Usuario(UsuarioID),
	RolID int null CONSTRAINT FKPermisoConcedido_Rol FOREIGN KEY (RolID) REFERENCES Rol(RolID),
	PuedeEntrar bit not null,
	PuedeBorrar bit not null,
	PuedeEditar bit not null

)
go

CREATE INDEX PermisoConcedido_Usuario ON PermisoConcedido( UsuarioID ) 
GO

CREATE INDEX PermisoConcedido_Rol ON PermisoConcedido( RolID ) 
GO

CREATE INDEX PermisoConcedido_Permiso ON PermisoConcedido( PermisoID ) 
GO
