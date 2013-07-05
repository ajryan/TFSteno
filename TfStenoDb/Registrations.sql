CREATE TABLE [dbo].[Registrations]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY, 
    [Email] NVARCHAR(1024) NOT NULL, 
    [TfsUrl] NVARCHAR(1024) NOT NULL, 
    [TfsUsername] NVARCHAR(1024) NOT NULL, 
    [TfsPassword] NVARCHAR(1024) NOT NULL
)

GO

CREATE UNIQUE INDEX [IX_Registrations_Email] ON [dbo].[Registrations] ([Email])
