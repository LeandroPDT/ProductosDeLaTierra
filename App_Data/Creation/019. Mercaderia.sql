--drop table Mercaderia

create table dbo.Mercaderia (
	MercaderiaID Int NOT NULL IDENTITY (1, 1) constraint PKMercaderia PRIMARY KEY ,
	Peso decimal (10, 2) not null,
	Precio decimal (10, 2) not null,
	Bultos int null
)
go

CREATE INDEX Mercaderia_general ON Mercaderia( MercaderiaID ) 
GO