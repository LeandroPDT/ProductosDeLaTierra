USE [ProductosDeLaTierra]
GO
/****** Object:  Table [dbo].[IncidenteSubscripcion]    Script Date: 02/24/2016 13:21:33 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[IncidenteSubscripcion](
	[IncidenteSubscripcionID] [int] IDENTITY(1,1) NOT NULL,
	[IncidenteID] [int] NOT NULL,
	[US_ID] [int] NULL,
 CONSTRAINT [PKIncidenteSubscripcion] PRIMARY KEY CLUSTERED 
(
	[IncidenteSubscripcionID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
ALTER TABLE [dbo].[IncidenteSubscripcion]  WITH CHECK ADD  CONSTRAINT [FKIncidenteSubscripcion_Incidente] FOREIGN KEY([IncidenteID])
REFERENCES [dbo].[Incidente] ([IncidenteID])
GO
ALTER TABLE [dbo].[IncidenteSubscripcion] CHECK CONSTRAINT [FKIncidenteSubscripcion_Incidente]
GO
ALTER TABLE [dbo].[IncidenteSubscripcion]  WITH CHECK ADD  CONSTRAINT [FKIncidenteSubscripcion_Usuario] FOREIGN KEY([US_ID])
REFERENCES [dbo].[Usuario] ([UsuarioID])
GO
ALTER TABLE [dbo].[IncidenteSubscripcion] CHECK CONSTRAINT [FKIncidenteSubscripcion_Usuario]