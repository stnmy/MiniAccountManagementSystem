CREATE TABLE VoucherEntries (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    VoucherId INT NOT NULL,
    AccountId INT NOT NULL,
    Debit DECIMAL(18, 2) NOT NULL,
    Credit DECIMAL(18, 2) NOT NULL,
    FOREIGN KEY (VoucherId) REFERENCES Vouchers(Id),
    FOREIGN KEY (AccountId) REFERENCES Accounts(Id)
);
