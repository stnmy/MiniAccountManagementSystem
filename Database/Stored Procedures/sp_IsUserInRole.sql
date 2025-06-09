CREATE OR ALTER PROCEDURE [dbo].[sp_IsUserInRole]
    @UserId INT,
    @RoleName NVARCHAR(50)
AS
BEGIN
    SELECT CAST(CASE WHEN EXISTS (
        SELECT 1 
        FROM UserRoles ur
        JOIN Roles r ON ur.RoleId = r.Id
        WHERE ur.UserId = @UserId AND r.Name = @RoleName
    ) THEN 1 ELSE 0 END AS BIT)
END