CREATE TABLE [dbo].[Registrations]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY, 
    [Email] NVARCHAR(320) NOT NULL, 
    [TfsUrl] NVARCHAR(1024) NOT NULL, 
    [TfsUsername] NVARCHAR(1024) NOT NULL, 
    [TfsPassword] NVARCHAR(1024) NOT NULL, 
    [ConfirmationCode] CHAR(32) NOT NULL, 
    [Confirmed] BIT NOT NULL DEFAULT 0
)

GO

CREATE UNIQUE INDEX [IX_Registrations_Email] ON [dbo].[Registrations] ([Email])
