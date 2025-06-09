CREATE OR ALTER PROCEDURE [dbo].[sp_ManageChartOfAccounts]
    @Action NVARCHAR(20),
    @Id INT = NULL,
    @Name NVARCHAR(100) = NULL,
    @ParentId INT = NULL,
    @Type NVARCHAR(50) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    IF @Action = 'CREATE'
    BEGIN
        INSERT INTO Accounts (Name, ParentId, Type)
        VALUES (@Name, @ParentId, @Type);
        RETURN;
    END

    IF @Action = 'UPDATE'
    BEGIN
        UPDATE Accounts
        SET Name = @Name,
            ParentId = @ParentId,
            Type = @Type
        WHERE Id = @Id;
        RETURN;
    END

    IF @Action = 'DELETE'
    BEGIN
        WITH RecursiveChildren AS (
            SELECT Id FROM Accounts WHERE ParentId = @Id
            UNION ALL
            SELECT a.Id
            FROM Accounts a
            INNER JOIN RecursiveChildren rc ON a.ParentId = rc.Id
        )
        DELETE FROM Accounts WHERE Id IN (SELECT Id FROM RecursiveChildren);

        DELETE FROM Accounts WHERE Id = @Id;
        RETURN;
    END

    IF @Action = 'GETALL'
    BEGIN
        WITH RecursiveAccounts AS (
            SELECT
                Id,
                Name,
                ParentId,
                Type,
                0 AS Level,
                CAST(RIGHT('0000' + CAST(Id AS VARCHAR), 4) AS VARCHAR(MAX)) AS HierarchyPath
            FROM Accounts
            WHERE ParentId IS NULL

            UNION ALL

            SELECT
                a.Id,
                a.Name,
                a.ParentId,
                a.Type,
                ra.Level + 1,
                ra.HierarchyPath + '-' + RIGHT('0000' + CAST(a.Id AS VARCHAR), 4)
            FROM Accounts a
            INNER JOIN RecursiveAccounts ra ON a.ParentId = ra.Id
        )
        SELECT Id, Name, ParentId, Type, Level, HierarchyPath
        FROM RecursiveAccounts
        ORDER BY HierarchyPath;

        RETURN;
    END

    IF @Action = 'GETBYID'
    BEGIN
        SELECT Id, Name, ParentId, Type,
               NULL AS HierarchyPath
        FROM Accounts
        WHERE Id = @Id;
        RETURN;
    END

    IF @Action = 'GETLEAF'
    BEGIN
        SELECT a.Id, a.Name, a.ParentId, a.Type
        FROM Accounts a
        WHERE NOT EXISTS (
            SELECT 1 FROM Accounts c WHERE c.ParentId = a.Id
        )
        ORDER BY Name;

        RETURN;
    END
END