USE [ProductosDeLaTierra]
GO
/****** Object:  Table [dbo].[IncidenteComentario]    Script Date: 02/24/2016 13:21:16 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[IncidenteComentario](
	[IncidenteComentarioID] [int] IDENTITY(1,1) NOT NULL,
	[IncidenteID] [int] NULL,
	[US_ID] [int] NULL,
	[Fecha] [datetime] NOT NULL DEFAULT (getdate()),
	[Mensaje] [text] NULL,
	[IsDeleted] [bit] NOT NULL DEFAULT ((0)),
	[Archivo] [varchar](250) NULL,
	[Status] [varchar](250) NULL,
	[ArchivoNombre] [varchar](250) NULL,
 CONSTRAINT [PKIncidenteComentario] PRIMARY KEY CLUSTERED 
(
	[IncidenteComentarioID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
ALTER TABLE [dbo].[IncidenteComentario]  WITH CHECK ADD  CONSTRAINT [FKIncidenteComentario_Incidente] FOREIGN KEY([IncidenteID])
REFERENCES [dbo].[Incidente] ([IncidenteID])
GO
ALTER TABLE [dbo].[IncidenteComentario] CHECK CONSTRAINT [FKIncidenteComentario_Incidente]
GO
ALTER TABLE [dbo].[IncidenteComentario]  WITH CHECK ADD  CONSTRAINT [FKIncidenteComentario_Usuario] FOREIGN KEY([US_ID])
REFERENCES [dbo].[Usuario] ([UsuarioID])
GO
ALTER TABLE [dbo].[IncidenteComentario] CHECK CONSTRAINT [FKIncidenteComentario_Usuario]