CREATE TABLE [dbo].[Complaints] (
    [id]            INT            IDENTITY (1, 1) NOT NULL,
    [PostId]        INT            NOT NULL,
    [ComplaintType] NVARCHAR (100) NOT NULL,
    [ComplaintText] NVARCHAR (MAX) NOT NULL,
    PRIMARY KEY CLUSTERED ([id] ASC),
	Constraint FK_Complaints_Post foreign key (PostId) references VolunteeringPosts (Id) 
);

