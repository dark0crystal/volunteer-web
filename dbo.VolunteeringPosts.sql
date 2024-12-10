CREATE TABLE [dbo].[VolunteeringPosts] (
    [Id]             INT            IDENTITY (1, 1) NOT NULL,
    [Title]          NVARCHAR (100) NOT NULL,
    [Category]       NVARCHAR (50)  NOT NULL,
    [Description]    NVARCHAR (500) NOT NULL,
    [NumOfDays]      INT            NOT NULL,
    [Location]       NVARCHAR (100) NOT NULL,
    [OrgName]        NVARCHAR (100) NOT NULL,
    [StartDate]      DATE           NULL,
    [EndDate]        DATE           NULL,
    [PostAdminEmail] VARCHAR (50)   NOT NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC),
    CHECK (len([Title])>(0)),
    CHECK (len([Category])>(0)),
    CHECK (len([Description])>(0)),
    CHECK ([NumOfDays]>(0)),
    CHECK (len([Location])>(0)),
    CHECK (len([OrgName])>(0)),
    CHECK ([PostAdminEmail] like '%@%'),
	Constraint FK_Post_Register Foreign key (PostAdminEmail) references register(email)
);

