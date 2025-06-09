CREATE OR ALTER PROCEDURE [dbo].[sp_GetUserById]
    @Id INT
AS
BEGIN
    SELECT Id, Username, PasswordHash
    FROM Users
    WHERE Id = @Id
END