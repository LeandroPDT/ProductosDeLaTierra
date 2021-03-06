/*
   martes, 20 de septiembre de 201611:28:00 p. m.
   User: 
   Server: HP\SQLEXPRESS
   Database: ProductosDeLaTierra
   Application: 
*/

/* To prevent any potential data loss issues, you should review this script in detail before running it outside the context of the database designer.*/
BEGIN TRANSACTION
SET QUOTED_IDENTIFIER ON
SET ARITHABORT ON
SET NUMERIC_ROUNDABORT OFF
SET CONCAT_NULL_YIELDS_NULL ON
SET ANSI_NULLS ON
SET ANSI_PADDING ON
SET ANSI_WARNINGS ON
COMMIT
BEGIN TRANSACTION
GO
ALTER TABLE dbo.Cargamento ADD
	Cobrado bit NOT NULL CONSTRAINT DF_Cargamento_Cobrado DEFAULT 0
GO
COMMIT
