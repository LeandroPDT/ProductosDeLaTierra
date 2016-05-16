--drop table dbo.PaginaVisita
create table dbo.PaginaVisita (
	PaginaVisitaID	int NOT NULL IDENTITY (1, 1) constraint PKPaginaVisita PRIMARY KEY ,
	UsuarioID			int NOT NULL,
	Path			Varchar(250) null,
	Titulo 			Varchar(100) null,
	Cantidad		int not null,
	IsBookmarked    bit not null,
	Orden			int not null,
	LastVisited		datetime not null
)
go

CREATE INDEX PaginaVisita_general ON PaginaVisita( UsuarioID, Path ) 
GO

CREATE INDEX PaginaVisita_general2 ON PaginaVisita( UsuarioID, IsBookmarked, Cantidad DESC) 
GO

CREATE INDEX PaginaVisita_general3 ON PaginaVisita( UsuarioID, IsBookmarked, Orden ) 
GO

CREATE INDEX PaginaVisita_general4 ON PaginaVisita( UsuarioID, LastVisited DESC) 
GO


