drop table ItemCobranza

create table dbo.ItemCobranza (
	ItemCobranzaID Int NOT NULL IDENTITY (1, 1) constraint PKItemCobranza PRIMARY KEY ,
	Monto decimal (10, 2) not null,
	EventoID int null CONSTRAINT FKItemCobranza_Evento FOREIGN KEY (EventoID) REFERENCES Evento(EventoID),
	CobranzaID int null CONSTRAINT FKItemCobranza_Cobranza FOREIGN KEY (CobranzaID) REFERENCES Cobranza(CobranzaID),
	CargamentoID int null CONSTRAINT FKItemCobranzaCargamento_Cargamento FOREIGN KEY (CargamentoID) REFERENCES Cargamento(CargamentoID),
	Referencia text null
)
go

CREATE INDEX ItemCobranza_general ON ItemCobranza( ItemCobranzaID) 
GO

CREATE INDEX ItemCobranza_Cobranza ON ItemCobranza( CobranzaID ) 
GO

CREATE INDEX ItemCobranza_Cargamento ON ItemCobranza( CargamentoID ) 
GO