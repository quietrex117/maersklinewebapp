USE [DDAC20180411112109_db]
GO

/****** Object: Table [dbo].[ScheduleBooking] Script Date: 12/4/2018 8:48:56 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[ScheduleBooking] (
    [BookingId]     INT          IDENTITY (1, 1) NOT NULL,
    [Cargo]         VARCHAR (50) NULL,
    [ContainerType] VARCHAR (50) NULL,
    [ScheduleId]    INT          NULL,
    [UsersId]       INT          NULL
);


UPDATE ScheduleBooking SET UsersId = 2 WHERE BookingId = 4;