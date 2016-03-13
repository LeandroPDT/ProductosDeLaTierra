--drop table Evento

create table dbo.Evento (
	EventoID Int NOT NULL IDENTITY (1, 1) constraint PKEvento PRIMARY KEY ,
	TipoEventoID int NOT null,
	Fecha datetime not null,	
	CargamentoID int null CONSTRAINT FKEvento_Cargamento FOREIGN KEY (CargamentoID) REFERENCES Cargamento(CargamentoID),
	MercaderiaID int not null CONSTRAINT FKEvento_Mercaderia FOREIGN KEY (MercaderiaID) REFERENCES Mercaderia(MercaderiaID),
	Ganancia decimal (10, 2) not null,
	Notas text null
)
go

CREATE INDEX Evento_general ON Evento( EventoID ) 
GO

CREATE INDEX Evento_general2 ON Evento(  Fecha, TipoEventoID ) 
GO

CREATE INDEX Evento_general3 ON Evento(  Fecha, CargamentoID )
GO

CREATE INDEX Evento_Cargamento ON Evento(  CargamentoID )
GO
