CREATE OR ALTER PROCEDURE [dbo].[sp_GetUserByUsername]
    @Username NVARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT Id, Username, PasswordHash
    FROM Users
    WHERE Username = @Username;
END