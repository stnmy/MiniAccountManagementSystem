CREATE OR ALTER PROCEDURE [dbo].[sp_RemoveRoleFromUser]
    @UserId INT,
    @RoleName NVARCHAR(50)
AS
BEGIN
    DELETE ur
    FROM UserRoles ur
    JOIN Roles r ON ur.RoleId = r.Id
    WHERE ur.UserId = @UserId AND r.Name = @RoleName
END