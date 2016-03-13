drop table Cargamento

create table dbo.Cargamento (
	CargamentoID Int NOT NULL IDENTITY (1, 1) constraint PKCargamento PRIMARY KEY ,
	NumeroRemito int not null,
	FechaEnvio datetime not null,	
	MercaderiaID int not null CONSTRAINT FKCargamento_Mercaderia FOREIGN KEY (MercaderiaID) REFERENCES Mercaderia(MercaderiaID),
	ProveedorID int not null CONSTRAINT FKCargamento_Proveedor FOREIGN KEY (ProveedorID) REFERENCES Usuario(UsuarioID),
	ClienteID int not null CONSTRAINT FKCargamento_Cliente FOREIGN KEY (ClienteID) REFERENCES Usuario(UsuarioID),
	Ganancia decimal (10, 2) not null,
	CostoFlete decimal (10, 2) not null,
	CostoDescarga decimal (10, 2) not null,
	Observaciones text null,
	Recibido bit not null,
	Vendido bit not null,
	PrecioCerrado bit not null
)
go

CREATE INDEX Cargamento_general ON Cargamento( CargamentoID ) 
GO

CREATE INDEX Cargamento_general2 ON Cargamento(  FechaEnvio, NumeroRemito ) 
GO

CREATE INDEX Cargamento_Usuario_Proveedor ON Cargamento(  ProveedorID ) 
GO

CREATE INDEX Cargamento_Usuario_Cliente ON Cargamento(  ClienteID ) 
GO