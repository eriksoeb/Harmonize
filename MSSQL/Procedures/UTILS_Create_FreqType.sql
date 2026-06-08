USE [Harmonize]
GO

CREATE TABLE FreqType (
    Id          tinyint      NOT NULL PRIMARY KEY,
    Name        char(3)      NOT NULL UNIQUE,
    Description nvarchar(32) NOT NULL,
    Active      bit          NULL
);

INSERT INTO FreqType (Id, Name, Description, Active) VALUES
(1, 'ANN', 'Annual',    1),
(2, 'MON', 'Monthly',   1),
(3, 'Q',   'Quarterly', 1),
(4, 'DAY', 'Daily',     1);
GO
