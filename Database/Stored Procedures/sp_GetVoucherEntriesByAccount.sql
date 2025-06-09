CREATE OR ALTER PROCEDURE [dbo].[sp_GetVoucherEntriesByAccount]
    @AccountId INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        V.VoucherDate,
        V.ReferenceNo,
        V.VoucherType,
        A.Name AS AccountName,
        VE.Debit,
        VE.Credit
    FROM VoucherEntries VE
    INNER JOIN Vouchers V ON VE.VoucherId = V.Id
    INNER JOIN Accounts A ON VE.AccountId = A.Id
    WHERE VE.AccountId = @AccountId
    ORDER BY V.VoucherDate;
END