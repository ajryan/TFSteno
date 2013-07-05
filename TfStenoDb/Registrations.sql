CREATE TABLE [dbo].[Registrations]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY, 
    [Email] NVARCHAR(320) NOT NULL, 
    [TfsUrl] NVARCHAR(512) NOT NULL, 
    [TfsUsername] NVARCHAR(200) NOT NULL, 
    [TfsPassword] NVARCHAR(200) NOT NULL
)

GO

CREATE UNIQUE INDEX [IX_Registrations_Email] ON [dbo].[Registrations] ([Email])
