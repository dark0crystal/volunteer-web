CREATE TABLE [dbo].[register] (
    [id]         INT          IDENTITY (1, 1) NOT NULL,
    [firstName]  VARCHAR (12) NOT NULL,
    [familyName] VARCHAR (12) NOT NULL,
    [email]      VARCHAR (50) NOT NULL unique,
    [password]   VARCHAR (12) NOT NULL,
    CONSTRAINT [PK_register] PRIMARY KEY CLUSTERED ([id] ASC, [email] ASC),
    CHECK (len([firstName])>(0)),
    CHECK (len([familyName])>(0)),
    CHECK ([email] like '%@%'),
    CHECK (len([password])>=(8))
);

