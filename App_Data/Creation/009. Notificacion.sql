--drop table Notificacion

create table dbo.Notificacion (
	NotificacionID Int NOT NULL IDENTITY (1, 1) constraint PKNotificacion PRIMARY KEY ,
	Fecha datetime not null,
	Texto Varchar(500) null,
	URL Varchar(500) null,
	UsuarioID int not null CONSTRAINT FKNotificacion_Usuario FOREIGN KEY (UsuarioID) REFERENCES Usuario(UsuarioID),
	Leido bit not null,
	Cuerpo TEXT null
)
go

CREATE INDEX Notificacion_general ON Notificacion( UsuarioID, Leido ) 
GO

CREATE INDEX Notificacion_general2 ON Notificacion( UsuarioID, Fecha, Leido ) 
GO

