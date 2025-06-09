CREATE OR ALTER PROCEDURE [dbo].[sp_CreateUser]
    @Username NVARCHAR(100),
    @PasswordHash NVARCHAR(255)
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO Users (Username, PasswordHash)
    VALUES (@Username, @PasswordHash);
END