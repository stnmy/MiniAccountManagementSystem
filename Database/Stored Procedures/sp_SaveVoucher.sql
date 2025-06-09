CREATE OR ALTER PROCEDURE [dbo].[sp_SaveVoucher]
    @VoucherDate DATE,
    @ReferenceNo NVARCHAR(50),
    @VoucherType NVARCHAR(50),
    @VoucherEntries VoucherEntryType READONLY
AS
BEGIN
    SET NOCOUNT ON;
    
    BEGIN TRY
        BEGIN TRANSACTION;
        
        DECLARE @VoucherId INT;

        INSERT INTO Vouchers (VoucherDate, ReferenceNo, VoucherType)
        VALUES (@VoucherDate, @ReferenceNo, @VoucherType);

        SET @VoucherId = SCOPE_IDENTITY();

        INSERT INTO VoucherEntries (VoucherId, AccountId, Debit, Credit)
        SELECT
            @VoucherId,
            AccountId,
            Debit,
            Credit
        FROM @VoucherEntries;
        
        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION;
            
        THROW;
    END CATCH
END