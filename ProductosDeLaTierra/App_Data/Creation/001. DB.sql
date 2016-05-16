CREATE DATABASE [ProductosDeLaTierra] ON  PRIMARY 
    ( NAME = N'ProductosDeLaTierra', FILENAME = N'D:\SQLDATA\SQL2012\ProductosDeLaTierra.mdf' , SIZE = 167872KB , MAXSIZE = UNLIMITED, FILEGROWTH = 16384KB )
     LOG ON 
	 ( NAME = N'ProductosDeLaTierra_Log', FILENAME = N'D:\SQLDATA\SQL2012\ProductosDeLaTierra_Log.ldf' , SIZE = 2048KB , MAXSIZE = 2048GB , FILEGROWTH = 16384KB )
GO

BACKUP DATABASE [ProductosDeLaTierra] TO  DISK = N'd:\temp\ProductosDeLaTierra.bak' WITH NOFORMAT, INIT,  NAME = N'ProductosDeLaTierra Backup', SKIP, NOREWIND, NOUNLOAD,  STATS = 10
