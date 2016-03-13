--drop table Producto

create table dbo.Producto (
	ProductoID Int NOT NULL IDENTITY (1, 1) constraint PKProducto PRIMARY KEY ,
	PesoUnitario decimal (10, 2) not null,
	PrecioKg decimal (10, 2) not null,
	PrecioUnitario decimal (10, 2) not null,
	CodigoArticulo int null,
	Descripcion text null,
	UsuarioID int not null Constraint FKProducto_Usuario FOREIGN KEY (UsuarioID) REFERENCES Usuario(UsuarioID),
	Notas text null
)
go

CREATE INDEX Producto_general ON Producto( ProductoID ) 
GO

CREATE INDEX Producto_Usuario ON Producto(UsuarioID)
GO 

CREATE INDEX Producto_general2 ON Producto(  CodigoArticulo ) 
GO

CREATE INDEX Producto_general3 ON Producto(UsuarioID,CodigoArticulo)
GO