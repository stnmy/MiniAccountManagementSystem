CREATE OR ALTER PROCEDURE [dbo].[sp_AssignRoleToUser]
    @UserId INT,
    @RoleName NVARCHAR(50)
AS
BEGIN
    SET NOCOUNT ON;
    
    DECLARE @RoleId INT;
    
    SELECT @RoleId = Id 
    FROM Roles 
    WHERE Name = @RoleName;
    
    IF @RoleId IS NOT NULL 
       AND NOT EXISTS (
           SELECT 1 
           FROM UserRoles 
           WHERE UserId = @UserId 
             AND RoleId = @RoleId
       )
    BEGIN
        INSERT INTO UserRoles (UserId, RoleId)
        VALUES (@UserId, @RoleId);
    END
END