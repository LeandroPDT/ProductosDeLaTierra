-- DROP  Table ArchivoAdjunto

CREATE TABLE ArchivoAdjunto (
	ArchivoAdjuntoID int IDENTITY(1,1) NOT NULL CONSTRAINT PKArchivoAdjunto PRIMARY KEY CLUSTERED ( ArchivoAdjuntoID ) ,
	ID int not null,	
	Entidad varchar(500) NULL,
	NombreArchivo varchar(500) NULL,
	Titulo varchar(500) NULL,
	Fecha datetime not null default getdate()
)

GO


CREATE INDEX ArchivoAdjunto_general ON dbo.ArchivoAdjunto (Entidad, ID, Fecha ) 
GO

