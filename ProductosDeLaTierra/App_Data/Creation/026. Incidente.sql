USE [ProductosDeLaTierra]
GO
/****** Object:  Table [dbo].[Incidente]    Script Date: 02/24/2016 13:21:02 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Incidente](
	[IncidenteID] [int] IDENTITY(1,1) NOT NULL,
	[Titulo] [varchar](200) NULL,
	[Notas] [text] NULL,
	[Fecha] [datetime] NOT NULL,
	[US_ID] [int] NULL,
	[Cerrado] [bit] NOT NULL,
	[Area] [varchar](50) NULL,
	[Tipo] [varchar](20) NULL,
	[ResueltoDev] [bit] NOT NULL DEFAULT ((0)),
	[EnPausa] [bit] NOT NULL DEFAULT ((0)),
 CONSTRAINT [PKIncidente] PRIMARY KEY CLUSTERED 
(
	[IncidenteID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
ALTER TABLE [dbo].[Incidente]  WITH CHECK ADD  CONSTRAINT [FKIncidente_Usuario] FOREIGN KEY([US_ID])
REFERENCES [dbo].[Usuario] ([UsuarioID])
GO
ALTER TABLE [dbo].[Incidente] CHECK CONSTRAINT [FKIncidente_Usuario]