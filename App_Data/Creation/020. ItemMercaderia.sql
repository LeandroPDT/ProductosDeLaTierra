--drop table ItemMercaderia

create table dbo.ItemMercaderia (
	MercaderiaProductoID Int NOT NULL IDENTITY (1, 1) constraint PKMercaderiaProducto PRIMARY KEY ,
	Peso decimal (10, 2) not null,
	Precio decimal (10, 2) not null,
	PrecioKg decimal (10, 2) not null,
	PrecioUnitario decimal (10, 2) not null,
	Cantidad int null,
	Bultos int null,
	MercaderiaID int null CONSTRAINT FKMercaderiaProducto_Mercaderia FOREIGN KEY (MercaderiaID) REFERENCES Mercaderia(MercaderiaID),
	ProductoID int null CONSTRAINT FKMercaderiaProducto_Producto FOREIGN KEY (ProductoID) REFERENCES Producto(ProductoID),
	Observaciones text null
)
go

CREATE INDEX MercaderiaProducto_general ON MercaderiaProducto( MercaderiaProductoID ) 
GO

CREATE INDEX MercaderiaProducto_Mercaderia ON MercaderiaProducto( MercaderiaID ) 
GO

CREATE INDEX MercaderiaProducto_Producto ON MercaderiaProducto(  ProductoID ) 
GO