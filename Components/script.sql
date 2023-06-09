USE db_warehouse_management
GO

-- Create tbl_users table if not exists
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'tbl_users')
BEGIN
    CREATE TABLE [dbo].[tbl_users](
        [user_id] [int] IDENTITY(1,1) NOT NULL PRIMARY KEY,
        [first_name] [varchar](50) NULL,
        [middle_name] [varchar](50) NULL,
        [last_name] [varchar](50) NULL,
        [email] [varchar](50) NULL,
        [contact_number] [varchar](50) NULL,
        [authentication_code] [varchar](50) NOT NULL,
        [username] [varchar](50) NULL,
        [password] [nvarchar](100) NULL,
        [status] [varchar](10) NULL
    )
END
GO

-- Create tbl_products table if not exists
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'tbl_products')
BEGIN
    CREATE TABLE [dbo].[tbl_products](
        [product_id] [varchar](50) NOT NULL PRIMARY KEY,
        [item_name] [varchar](50) NULL,
        [acq_cost] [decimal](18, 2) NULL,
        [nominated_price] [decimal](18, 2) NULL,
        [barcode] [varchar](50) NULL,
        [unit_quantity] [int] NULL,
        [status] [varchar](50) NULL,
        [reorder_point] [int] NULL,
        [timestamp] [datetime] NULL
    )
END
GO

-- Create tbl_active_users table if not exists
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'tbl_active_users')
BEGIN
    CREATE TABLE [dbo].[tbl_active_users](
        [id] [int] IDENTITY(1,1) NOT NULL PRIMARY KEY,
        [user_id] [int] NULL,
        [login_time] [datetime] NULL,
        [logout_time] [datetime] NULL,
        FOREIGN KEY ([user_id]) REFERENCES [tbl_users]([user_id])
    )
END
GO

-- Create tbl_user_access table if not exists
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'tbl_user_access')
BEGIN
    CREATE TABLE [dbo].[tbl_user_access](
        [id] [int] IDENTITY(1,1) NOT NULL PRIMARY KEY,
        [user_id] [int] NOT NULL,
        [access_level] [varchar](20) NOT NULL,
        FOREIGN KEY ([user_id]) REFERENCES [tbl_users]([user_id])
    )
END
GO