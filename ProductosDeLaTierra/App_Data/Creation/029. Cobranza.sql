--drop table ItemCobranza
--drop table Cobranza

create table dbo.Cobranza (
	CobranzaID Int NOT NULL IDENTITY (1, 1) constraint PKCobranza PRIMARY KEY ,
	Referencia text null,
	ProveedorID int null CONSTRAINT FKCobranzaProveedor_Usuario FOREIGN KEY (ProveedorID) REFERENCES Usuario(UsuarioID),
	Fecha datetime not null,	
	Monto decimal (10, 2) not null,
	Notas text null
)
go

CREATE INDEX Cobranza_general ON Cobranza( CobranzaID ) 
GO

CREATE INDEX Cobranza_general3 ON Cobranza(  Fecha, ProveedorID )
GO

CREATE INDEX Cobranza_Usuario ON Cobranza(  ProveedorID )
GO
