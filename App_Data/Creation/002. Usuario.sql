 if exists (select * from sysobjects where id = object_id('Usuario') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
	drop table Usuario
GO

create table dbo.Usuario (
	UsuarioID				Int NOT NULL IDENTITY (1, 1) constraint PKUsuario PRIMARY KEY ,
	Username				varchar (50) NOT NULL ,
	Nombre   				varchar (100) NOT NULL ,
	Email   				varchar (250) NULL ,
	Password   				varchar (250) NULL ,
	Activo					bit NOT NULL,
	Notas					text null,
	IsLockedOut				bit not null,
	CreationDate			datetime not null default getdate(),
	LastActivityDate		datetime null,
	LastPasswordChangedDate datetime null,
	LastLockedOutDate			datetime null,
	FailedPasswordAttemptCount int not null,
	FailedPasswordAttemptWindowStart datetime null,
    PasswordResetToken UNIQUEIDENTIFIER null,
    PasswordResetExpiration DateTime null,
	LastLoginDate DateTime null, 
	LastLoginIP varchar(50) null,
	Avatar varchar(100) null, 
	AvatarChico varchar(100) null,
	Comision decimal(10,2) not null default 0
)
go

CREATE INDEX Usuario_general ON Usuario( UserName ) 
GO

Insert into Usuario ( Username, Nombre, Email, activo, IsLockedOut, FailedPasswordAttemptCount)
values ('Leo', 'Leandro Palma', 'leandropalma@live.com.ar', 1, 0, 0)



