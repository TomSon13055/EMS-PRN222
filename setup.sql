-- ================================================================
-- EventManagementDb - Schema + Seed Data
-- Run this script in SQL Server Management Studio (SSMS)
-- ================================================================

USE master;
GO

IF DB_ID('EventManagementDb') IS NOT NULL
BEGIN
    ALTER DATABASE EventManagementDb SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    DROP DATABASE EventManagementDb;
END
GO

CREATE DATABASE EventManagementDb;
GO

USE EventManagementDb;
GO

-- ================================================================
-- SCHEMA
-- ================================================================

CREATE TABLE EventCategories (
    EventCategoryId int NOT NULL IDENTITY(1,1) PRIMARY KEY,
    CategoryName nvarchar(100) NOT NULL
);

CREATE TABLE EventLocations (
    EventLocationId int NOT NULL IDENTITY(1,1) PRIMARY KEY,
    LocationName nvarchar(150) NOT NULL,
    Address nvarchar(255) NULL
);

CREATE TABLE Users (
    UserId int NOT NULL IDENTITY(1,1) PRIMARY KEY,
    FullName nvarchar(100) NOT NULL,
    Email nvarchar(150) NOT NULL,
    PasswordHash nvarchar(max) NOT NULL,
    Phone nvarchar(20) NULL,
    Role nvarchar(20) NOT NULL,
    WalletBalance decimal(18,2) NOT NULL DEFAULT 0,
    IsActive bit NOT NULL DEFAULT 1,
    CreatedAt datetime2 NOT NULL,
    UpdatedAt datetime2 NULL
);

CREATE UNIQUE INDEX IX_Users_Email ON Users(Email);

CREATE TABLE Events (
    EventId int NOT NULL IDENTITY(1,1) PRIMARY KEY,
    HostId int NOT NULL,
    EventCategoryId int NOT NULL,
    EventLocationId int NOT NULL,
    Title nvarchar(150) NOT NULL,
    Description nvarchar(max) NOT NULL,
    StartTime datetime2 NOT NULL,
    EndTime datetime2 NOT NULL,
    Status nvarchar(20) NOT NULL,
    ImageUrl nvarchar(500) NULL,
    CreatedAt datetime2 NOT NULL,
    UpdatedAt datetime2 NULL,
    CONSTRAINT FK_Events_Users_HostId FOREIGN KEY (HostId) REFERENCES Users(UserId) ON DELETE NO ACTION,
    CONSTRAINT FK_Events_EventCategories FOREIGN KEY (EventCategoryId) REFERENCES EventCategories(EventCategoryId) ON DELETE NO ACTION,
    CONSTRAINT FK_Events_EventLocations FOREIGN KEY (EventLocationId) REFERENCES EventLocations(EventLocationId) ON DELETE NO ACTION
);

CREATE INDEX IX_Events_HostId ON Events(HostId);
CREATE INDEX IX_Events_EventCategoryId ON Events(EventCategoryId);
CREATE INDEX IX_Events_EventLocationId ON Events(EventLocationId);

CREATE TABLE TicketTypes (
    TicketTypeId int NOT NULL IDENTITY(1,1) PRIMARY KEY,
    EventId int NOT NULL,
    TypeName nvarchar(100) NOT NULL,
    Price decimal(18,2) NOT NULL,
    Quantity int NOT NULL,
    SaleStart datetime2 NOT NULL,
    SaleEnd datetime2 NOT NULL,
    Status nvarchar(20) NOT NULL,
    CONSTRAINT FK_TicketTypes_Events FOREIGN KEY (EventId) REFERENCES Events(EventId) ON DELETE CASCADE
);

CREATE INDEX IX_TicketTypes_EventId ON TicketTypes(EventId);

CREATE TABLE Vouchers (
    VoucherId int NOT NULL IDENTITY(1,1) PRIMARY KEY,
    CreatedByUserId int NOT NULL,
    VoucherCode nvarchar(50) NOT NULL,
    DiscountPercentage decimal(5,2) NOT NULL,
    ValidFrom datetime2 NOT NULL,
    ValidTo datetime2 NOT NULL,
    IsActive bit NOT NULL DEFAULT 1,
    UsageLimit int NOT NULL,
    UsedCount int NOT NULL DEFAULT 0,
    CreatedAt datetime2 NOT NULL,
    UpdatedAt datetime2 NULL,
    CONSTRAINT FK_Vouchers_Users FOREIGN KEY (CreatedByUserId) REFERENCES Users(UserId) ON DELETE CASCADE
);

CREATE UNIQUE INDEX IX_Vouchers_VoucherCode ON Vouchers(VoucherCode);
CREATE INDEX IX_Vouchers_CreatedByUserId ON Vouchers(CreatedByUserId);

CREATE TABLE Wishlists (
    WishlistId int NOT NULL IDENTITY(1,1) PRIMARY KEY,
    CustomerId int NOT NULL,
    EventId int NOT NULL,
    CreatedAt datetime2 NOT NULL,
    CONSTRAINT FK_Wishlists_Events FOREIGN KEY (EventId) REFERENCES Events(EventId) ON DELETE CASCADE,
    CONSTRAINT FK_Wishlists_Users FOREIGN KEY (CustomerId) REFERENCES Users(UserId) ON DELETE CASCADE
);

CREATE UNIQUE INDEX IX_Wishlists_CustomerId_EventId ON Wishlists(CustomerId, EventId);
CREATE INDEX IX_Wishlists_EventId ON Wishlists(EventId);

CREATE TABLE UserNotifications (
    UserNotificationId int NOT NULL IDENTITY(1,1) PRIMARY KEY,
    UserId int NOT NULL,
    Title nvarchar(150) NOT NULL,
    Message nvarchar(max) NOT NULL,
    IsRead bit NOT NULL DEFAULT 0,
    CreatedAt datetime2 NOT NULL,
    CONSTRAINT FK_UserNotifications_Users FOREIGN KEY (UserId) REFERENCES Users(UserId) ON DELETE CASCADE
);

CREATE INDEX IX_UserNotifications_UserId ON UserNotifications(UserId);

CREATE TABLE WalletTransactions (
    TransactionId int NOT NULL IDENTITY(1,1) PRIMARY KEY,
    UserId int NOT NULL,
    Amount decimal(18,2) NOT NULL,
    TransactionType nvarchar(20) NOT NULL,
    Status nvarchar(20) NOT NULL,
    Description nvarchar(500) NULL,
    BalanceBefore decimal(18,2) NOT NULL,
    BalanceAfter decimal(18,2) NOT NULL,
    CreatedAt datetime2 NOT NULL,
    CONSTRAINT FK_WalletTransactions_Users FOREIGN KEY (UserId) REFERENCES Users(UserId) ON DELETE CASCADE
);

CREATE INDEX IX_WalletTransactions_UserId ON WalletTransactions(UserId);

CREATE TABLE Orders (
    OrderId int NOT NULL IDENTITY(1,1) PRIMARY KEY,
    CustomerId int NOT NULL,
    TotalAmount decimal(18,2) NOT NULL,
    DiscountAmount decimal(18,2) NOT NULL,
    FinalAmount decimal(18,2) NOT NULL,
    Status nvarchar(20) NOT NULL,
    VoucherId int NULL,
    CreatedAt datetime2 NOT NULL,
    PaidAt datetime2 NULL,
    CONSTRAINT FK_Orders_Users FOREIGN KEY (CustomerId) REFERENCES Users(UserId) ON DELETE NO ACTION,
    CONSTRAINT FK_Orders_Vouchers FOREIGN KEY (VoucherId) REFERENCES Vouchers(VoucherId) ON DELETE SET NULL
);

CREATE INDEX IX_Orders_CustomerId ON Orders(CustomerId);
CREATE INDEX IX_Orders_VoucherId ON Orders(VoucherId);

CREATE TABLE OrderItems (
    OrderItemId int NOT NULL IDENTITY(1,1) PRIMARY KEY,
    OrderId int NOT NULL,
    TicketTypeId int NOT NULL,
    Quantity int NOT NULL,
    UnitPrice decimal(18,2) NOT NULL,
    SubTotal decimal(18,2) NOT NULL,
    CONSTRAINT FK_OrderItems_Orders FOREIGN KEY (OrderId) REFERENCES Orders(OrderId) ON DELETE CASCADE,
    CONSTRAINT FK_OrderItems_TicketTypes FOREIGN KEY (TicketTypeId) REFERENCES TicketTypes(TicketTypeId) ON DELETE CASCADE
);

CREATE INDEX IX_OrderItems_OrderId ON OrderItems(OrderId);
CREATE INDEX IX_OrderItems_TicketTypeId ON OrderItems(TicketTypeId);

CREATE TABLE Tickets (
    TicketId int NOT NULL IDENTITY(1,1) PRIMARY KEY,
    TicketTypeId int NOT NULL,
    OrderItemId int NOT NULL,
    SerialNumber nvarchar(100) NOT NULL,
    Status nvarchar(20) NOT NULL,
    IssuedAt datetime2 NOT NULL,
    UsedAt datetime2 NULL,
    RefundedAt datetime2 NULL,
    QrCodeText nvarchar(500) NOT NULL,
    CONSTRAINT FK_Tickets_OrderItems FOREIGN KEY (OrderItemId) REFERENCES OrderItems(OrderItemId) ON DELETE CASCADE,
    CONSTRAINT FK_Tickets_TicketTypes FOREIGN KEY (TicketTypeId) REFERENCES TicketTypes(TicketTypeId) ON DELETE NO ACTION
);

CREATE UNIQUE INDEX IX_Tickets_SerialNumber ON Tickets(SerialNumber);
CREATE INDEX IX_Tickets_OrderItemId ON Tickets(OrderItemId);
CREATE INDEX IX_Tickets_TicketTypeId ON Tickets(TicketTypeId);

-- ================================================================
-- SEED DATA
-- Password for all: 123456
-- SHA256: 8d969eef6ecad3c29a3a629280e686cf0c3f5d5a86aff3ca12020c923adc6c92
-- ================================================================

DECLARE @Now datetime2 = GETDATE();

-- Users
SET IDENTITY_INSERT Users ON;
INSERT INTO Users (UserId, FullName, Email, PasswordHash, Phone, Role, WalletBalance, IsActive, CreatedAt)
VALUES
    (1, N'System Admin', 'admin@ems.com', '8d969eef6ecad3c29a3a629280e686cf0c3f5d5a86aff3ca12020c923adc6c92', NULL, 'Admin', 0, 1, @Now),
    (2, N'Demo Host', 'host@ems.com', '8d969eef6ecad3c29a3a629280e686cf0c3f5d5a86aff3ca12020c923adc6c92', NULL, 'Host', 0, 1, @Now),
    (3, N'Demo Customer', 'customer@ems.com', '8d969eef6ecad3c29a3a629280e686cf0c3f5d5a86aff3ca12020c923adc6c92', NULL, 'Customer', 5000000.00, 1, @Now);
SET IDENTITY_INSERT Users OFF;

-- Categories
SET IDENTITY_INSERT EventCategories ON;
INSERT INTO EventCategories (EventCategoryId, CategoryName)
VALUES (1, N'Music'), (2, N'Sports'), (3, N'Workshop'), (4, N'Technology'), (5, N'Art');
SET IDENTITY_INSERT EventCategories OFF;

-- Locations
SET IDENTITY_INSERT EventLocations ON;
INSERT INTO EventLocations (EventLocationId, LocationName, Address)
VALUES
    (1, N'Da Nang', N'Da Nang city'),
    (2, N'Ho Chi Minh City', N'HCMC'),
    (3, N'Ha Noi', N'Ha Noi capital'),
    (4, N'Online', N'Zoom / Google Meet');
SET IDENTITY_INSERT EventLocations OFF;

-- Events (dates relative to GETDATE so they stay in the future)
SET IDENTITY_INSERT Events ON;
INSERT INTO Events (EventId, HostId, EventCategoryId, EventLocationId, Title, Description, StartTime, EndTime, Status, ImageUrl, CreatedAt)
VALUES
    (1, 2, 1, 2, N'Saigon Music Night', N'Live indie bands under the stars.', DATEADD(hour, 7*24+19, DATEADD(minute, 0, @Now)), DATEADD(hour, 7*24+22, DATEADD(minute, 0, @Now)), N'Published', NULL, @Now),
    (2, 2, 4, 1, N'Da Nang Tech Summit 2026', N'Annual conference for software engineers.', DATEADD(hour, 14*24+9, DATEADD(minute, 0, @Now)), DATEADD(hour, 14*24+17, DATEADD(minute, 0, @Now)), N'Published', NULL, @Now),
    (3, 2, 2, 3, N'Hanoi Sports Festival', N'Friendly football & running tournament.', DATEADD(hour, 21*24+7, DATEADD(minute, 0, @Now)), DATEADD(hour, 21*24+12, DATEADD(minute, 0, @Now)), N'Published', NULL, @Now),
    (4, 2, 5, 1, N'Modern Art Workshop', N'Hands-on painting with local artists.', DATEADD(hour, 10*24+14, DATEADD(minute, 0, @Now)), DATEADD(hour, 10*24+17, DATEADD(minute, 0, @Now)), N'Published', NULL, @Now),
    (5, 2, 4, 4, N'AI for Beginners', N'Workshop covering AI basics and hands-on.', DATEADD(hour,  5*24+18, DATEADD(minute, 0, @Now)), DATEADD(hour,  5*24+21, DATEADD(minute, 0, @Now)), N'Published', NULL, @Now),
    (6, 2, 1, 4, N'Jazz Evening Online', N'Streamed live jazz concert from HCMC.', DATEADD(hour,  3*24+20, DATEADD(minute, 0, @Now)), DATEADD(hour,  3*24+22, DATEADD(minute, 30, @Now)), N'Published', NULL, @Now),
    (7, 2, 2, 2, N'Marathon 2026', N'City marathon for charity.', DATEADD(hour, 30*24+5, DATEADD(minute, 0, @Now)), DATEADD(hour, 30*24+12, DATEADD(minute, 0, @Now)), N'Published', NULL, @Now),
    (8, 2, 5, 3, N'Photography Masterclass', N'Pro techniques in street photography.', DATEADD(hour, 12*24+10, DATEADD(minute, 0, @Now)), DATEADD(hour, 12*24+16, DATEADD(minute, 0, @Now)), N'Published', NULL, @Now),
    (9, 2, 1, 2, N'Draft: EDM Party', N'Planning stage - not yet public.', DATEADD(hour, 45*24+21, DATEADD(minute, 0, @Now)), DATEADD(hour, 45*24+23, DATEADD(minute, 30, @Now)), N'Draft', NULL, @Now),
    (10, 2, 3, 3, N'Draft: Cooking Class', N'Planning stage - not yet public.', DATEADD(hour, 60*24+10, DATEADD(minute, 0, @Now)), DATEADD(hour, 60*24+13, DATEADD(minute, 0, @Now)), N'Draft', NULL, @Now);
SET IDENTITY_INSERT Events OFF;

-- TicketTypes for Published events (EventId 1-8)
DECLARE @SaleStart datetime2 = DATEADD(day, -1, @Now);

INSERT INTO TicketTypes (EventId, TypeName, Price, Quantity, SaleStart, SaleEnd, Status)
SELECT 1, N'General', 200000.00, 200, @SaleStart, DATEADD(hour, -1, DATEADD(hour, 7*24+19, DATEADD(minute, 0, @Now))), N'Active'
UNION ALL
SELECT 1, N'VIP', 500000.00, 50, @SaleStart, DATEADD(hour, -1, DATEADD(hour, 7*24+19, DATEADD(minute, 0, @Now))), N'Active'
UNION ALL
SELECT 2, N'General', 200000.00, 200, @SaleStart, DATEADD(hour, -1, DATEADD(hour, 14*24+9, DATEADD(minute, 0, @Now))), N'Active'
UNION ALL
SELECT 2, N'VIP', 500000.00, 50, @SaleStart, DATEADD(hour, -1, DATEADD(hour, 14*24+9, DATEADD(minute, 0, @Now))), N'Active'
UNION ALL
SELECT 3, N'General', 200000.00, 200, @SaleStart, DATEADD(hour, -1, DATEADD(hour, 21*24+7, DATEADD(minute, 0, @Now))), N'Active'
UNION ALL
SELECT 3, N'VIP', 500000.00, 50, @SaleStart, DATEADD(hour, -1, DATEADD(hour, 21*24+7, DATEADD(minute, 0, @Now))), N'Active'
UNION ALL
SELECT 4, N'General', 200000.00, 200, @SaleStart, DATEADD(hour, -1, DATEADD(hour, 10*24+14, DATEADD(minute, 0, @Now))), N'Active'
UNION ALL
SELECT 4, N'VIP', 500000.00, 50, @SaleStart, DATEADD(hour, -1, DATEADD(hour, 10*24+14, DATEADD(minute, 0, @Now))), N'Active'
UNION ALL
SELECT 5, N'General', 200000.00, 200, @SaleStart, DATEADD(hour, -1, DATEADD(hour,  5*24+18, DATEADD(minute, 0, @Now))), N'Active'
UNION ALL
SELECT 5, N'VIP', 500000.00, 50, @SaleStart, DATEADD(hour, -1, DATEADD(hour,  5*24+18, DATEADD(minute, 0, @Now))), N'Active'
UNION ALL
SELECT 6, N'General', 200000.00, 200, @SaleStart, DATEADD(hour, -1, DATEADD(hour,  3*24+20, DATEADD(minute, 0, @Now))), N'Active'
UNION ALL
SELECT 6, N'VIP', 500000.00, 50, @SaleStart, DATEADD(hour, -1, DATEADD(hour,  3*24+20, DATEADD(minute, 0, @Now))), N'Active'
UNION ALL
SELECT 7, N'General', 200000.00, 200, @SaleStart, DATEADD(hour, -1, DATEADD(hour, 30*24+5, DATEADD(minute, 0, @Now))), N'Active'
UNION ALL
SELECT 7, N'VIP', 500000.00, 50, @SaleStart, DATEADD(hour, -1, DATEADD(hour, 30*24+5, DATEADD(minute, 0, @Now))), N'Active'
UNION ALL
SELECT 8, N'General', 200000.00, 200, @SaleStart, DATEADD(hour, -1, DATEADD(hour, 12*24+10, DATEADD(minute, 0, @Now))), N'Active'
UNION ALL
SELECT 8, N'VIP', 500000.00, 50, @SaleStart, DATEADD(hour, -1, DATEADD(hour, 12*24+10, DATEADD(minute, 0, @Now))), N'Active';

-- Vouchers
DECLARE @VoucherValidFrom datetime2 = DATEADD(day, -1, @Now);
DECLARE @VoucherValidTo datetime2 = DATEADD(day, 60, @Now);
DECLARE @VoucherExpired datetime2 = DATEADD(day, -1, @Now);

SET IDENTITY_INSERT Vouchers ON;
INSERT INTO Vouchers (VoucherId, CreatedByUserId, VoucherCode, DiscountPercentage, ValidFrom, ValidTo, IsActive, UsageLimit, UsedCount, CreatedAt)
VALUES
    (1, 2, N'EMS10', 10.00, @VoucherValidFrom, @VoucherValidTo, 1, 100, 0, @Now),
    (2, 2, N'EMS30', 30.00, @VoucherValidFrom, @VoucherValidTo, 1,   5, 0, @Now),
    (3, 2, N'EXPIRED', 20.00, DATEADD(day, -30, @Now), @VoucherExpired, 1, 100, 0, @Now);
SET IDENTITY_INSERT Vouchers OFF;

PRINT 'EventManagementDb created and seeded successfully.';
