
CREATE TABLE [dbo].[ArchivoAdjunto](
	[ArchivoAdjuntoID] [int] IDENTITY(1,1) NOT NULL,
	[Fecha] [datetime] NOT NULL DEFAULT (getdate()),
	[ID] [int] NOT NULL,
	[Entidad] [varchar](50) NOT NULL,
	[NombreArchivo] [varchar](250) NULL,
	[Titulo] [varchar](250) NULL,
	[Archivo] [image] NULL,
 CONSTRAINT [PKArchivoAdjunto] PRIMARY KEY CLUSTERED 
(
	[ArchivoAdjuntoID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF