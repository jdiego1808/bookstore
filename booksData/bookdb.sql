
-- Create a new database called 'Bookstore'
-- Connect to the 'master' database to run this snippet
USE master
GO
-- Create the new database if it does not exist already
IF NOT EXISTS (
    SELECT name
        FROM sys.databases
        WHERE name = N'Bookstore'
)
CREATE DATABASE Bookstore
GO

USE Bookstore
GO

CREATE TABLE dbo.books
(
    [id] VARCHAR(24) NOT NULL PRIMARY KEY DEFAULT LEFT(REPLACE(LOWER(newid()),'-',''), 24), 
    [title] [NVARCHAR](255) NOT NULL,
    [isbn] [VARCHAR](13),
    [pageCount] INT CHECK([pageCount]>=0),
    [publishedDate] DATE,
    [thumbnailUrl] NVARCHAR(255),
    [description] NVARCHAR(MAX),
    [status] NVARCHAR(50),
    [price] MONEY CHECK([price]>=0),
    [quantity] INT CHECK([quantity]>=0)
);
GO



CREATE TABLE dbo.authors
(
    [bookId] VARCHAR(24) NOT NULL references dbo.books (id),
    [author] [NVARCHAR](255) NOT NULL,
    primary key (bookId, author)
);
GO

CREATE TABLE dbo.categories
(
    [bookId] VARCHAR(24) NOT NULL references dbo.books (id),
    [category] [NVARCHAR](255) NOT NULL,
    primary key (bookId, category)
);
GO

CREATE TABLE dbo.stationeries
(
    [Id] VARCHAR(24) NOT NULL PRIMARY KEY DEFAULT LEFT(REPLACE(LOWER(newid()),'-',''), 24),
    [product_name] [NVARCHAR](255),
    [thumbnail_url] [NVARCHAR](255),
    [unit] [NVARCHAR](50),
    [price] MONEY CHECK ([price]>=0),
    [quantity] INT CHECK ([quantity]>=0)
);
GO

CREATE TABLE dbo.sellers
(
    [Id] VARCHAR(24) NOT NULL PRIMARY KEY DEFAULT LEFT(REPLACE(LOWER(newid()),'-',''), 24),
    [name] [NVARCHAR](255), 
    [address] [NVARCHAR](255),
    [phone] [CHAR](10) unique,
    [hash_password] [NVARCHAR](255),
    [role] [NVARCHAR](50) DEFAULT 'seller',
    CHECK ([role] IN ('admin', 'seller')),
    UNIQUE ([name], [phone])
);
GO

INSERT INTO dbo.sellers (name, address, phone, hash_password, role)
VALUES ('admin', 'Sai Gon', '0987654321', '0WqLNdjBkiK18icCZJN5pCXsjhLqxUIeWqV6e4FQUyPK2+Pd', 'admin');


CREATE TABLE dbo.transactions
(
    [Id] VARCHAR(24) NOT NULL PRIMARY KEY DEFAULT LEFT(REPLACE(LOWER(newid()),'-',''), 24),
    [seller_id] VARCHAR(24) NOT NULL references dbo.sellers (Id),
    [create_date] DATETIME DEFAULT CURRENT_TIMESTAMP,
    [total_price] MONEY CHECK ([total_price]>=0)
);
GO

CREATE TABLE dbo.stationery_sells
(
    [trans_id] VARCHAR(24) REFERENCES dbo.transactions(Id),
    [stationery_id] VARCHAR(24) REFERENCES dbo.stationeries(Id),
    [quantity] INT CHECK (quantity > 0),
    PRIMARY KEY ([trans_id], [stationery_id])
);
GO


CREATE TABLE dbo.book_sells
(
    [trans_id] VARCHAR(24) REFERENCES dbo.transactions(Id),
    [book_id] VARCHAR(24) NOT NULL REFERENCES dbo.books(id),
    [quantity] INT CHECK (quantity > 0),
    PRIMARY KEY ([trans_id], [book_id])
);
GO

CREATE TABLE dbo.session 
(
    [id] VARCHAR(24) PRIMARY KEY DEFAULT LEFT(REPLACE(LOWER(newid()),'-',''), 24),
    [seller_id] CHAR(10) UNIQUE REFERENCES sellers(phone),
    [starting_time] DATETIME DEFAULT CURRENT_TIMESTAMP
);
GO


DECLARE @json VARCHAR(max)
SELECT @json = BulkColumn FROM OPENROWSET(BULK  '/home/books.json', SINGLE_CLOB) import
insert into books([id], [title], [isbn], [pageCount], [publishedDate], [thumbnailUrl], [description], [status])
select [id], [title], [isbn], [pageCount], [publishedDate], [thumbnailUrl], [shortDescription], [status] from OPENJSON(@json)
WITH (
    id VARCHAR(255) '$._id.oid',
    title VARCHAR(255) '$.title',
    isbn VARCHAR(13) '$.isbn',
    [pageCount] INT '$.pageCount',
    publishedDate DATETIME '$.publishedDate.date',
    thumbnailUrl VARCHAR(255) '$.thumbnailUrl',
    shortDescription varchar(max) '$.shortDescription',
    [status] VARCHAR(255) '$.status' 
)
GO

--- Populate book's authors into table
DECLARE @id VARCHAR(24), @authors NVARCHAR(max)
DECLARE @json VARCHAR(max)
SELECT @json = BulkColumn FROM OPENROWSET(BULK '/home/books.json', SINGLE_CLOB) import
DECLARE db_cursor CURSOR FOR
SELECT * FROM OPENJSON(@json)
WITH(
    id VARCHAR(24) '$._id.oid',
    authors NVARCHAR(max) '$.authors' AS JSON
)

OPEN db_cursor
FETCH NEXT FROM db_cursor INTO @id, @authors

WHILE @@FETCH_STATUS = 0
BEGIN
    INSERT INTO authors([bookId], [author])
    SELECT @id, value FROM OPENJSON(@authors) WHERE value<>''
    
    FETCH NEXT FROM db_cursor INTO @id, @authors
END

CLOSE db_cursor
DEALLOCATE db_cursor
GO

--- Populate book's categories into table
DECLARE @id VARCHAR(24), @categories NVARCHAR(max)
DECLARE @json VARCHAR(max)
SELECT @json = BulkColumn FROM OPENROWSET(BULK  '/home/books.json', SINGLE_CLOB) import
DECLARE db_cursor CURSOR FOR
SELECT * FROM OPENJSON(@json)
WITH(
    id VARCHAR(24) '$._id.oid',
    categories NVARCHAR(max) '$.categories' AS JSON
)

OPEN db_cursor
FETCH NEXT FROM db_cursor INTO @id, @categories

WHILE @@FETCH_STATUS = 0
BEGIN
    INSERT INTO categories([bookId], [category])
    SELECT @id, value FROM OPENJSON(@categories) WHERE value<>''
    
    FETCH NEXT FROM db_cursor INTO @id, @categories
END

CLOSE db_cursor
DEALLOCATE db_cursor
GO


DECLARE @json VARCHAR(max)
SELECT @json = BulkColumn FROM OPENROWSET(BULK  '/home/stationery.json', SINGLE_CLOB) import
INSERT INTO stationeries(product_name, price, unit, thumbnail_url)
select [product_name], [price], [Unit], [Thumbnail]from OPENJSON(@json)
WITH (
    -- id VARCHAR(10) '$.ID',
    [product_name] VARCHAR(255) '$.ProductName',
    [price] MONEY '$.Price',
    [unit] VARCHAR(10) '$.Unit',
    [thumbnail] VARCHAR(255) '$.Thumbnail'
)
GO

select books.*, author, category from books
join authors on books.id = authors.bookId
join categories on books.id = categories.bookId

update books 
set price= (abs(round((CHECKSUM(NEWID())%100*4.0)/15, 2)) + 10),
    quantity =(abs(cast(newid() as binary(6)) % 150)+50)
go


update stationeries
set quantity = (abs(cast(newid() as binary(6)) % 150)+50)
go

-- select * from books where id in ('53c2ae8528d75d572c06ad9f', '53c2ae8528d75d572c06ad9d')
-- select * from stationeries where id in ('00ac74ced6ad4425a69ed6d0', '02caaf41bc3649dab0278175')

-- select * from transactions
-- SELECT * from book_sells
-- select * from stationery_sells

