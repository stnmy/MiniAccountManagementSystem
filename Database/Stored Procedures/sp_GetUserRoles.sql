CREATE OR ALTER PROCEDURE [dbo].[sp_GetUserRoles]
    @UserId INT
AS
BEGIN
    SELECT r.Name 
    FROM UserRoles ur
    JOIN Roles r ON ur.RoleId = r.Id
    WHERE ur.UserId = @UserId
END