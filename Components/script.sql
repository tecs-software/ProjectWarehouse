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

-- Create tbl_selling_expenses table if not exists
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'tbl_selling_expenses')
BEGIN
    CREATE TABLE [dbo].[tbl_selling_expenses](
        [expense_id] [int] IDENTITY(1,1) PRIMARY KEY,
        [product_id] [varchar](50) NOT NULL REFERENCES [dbo].[tbl_products]([product_id]),
        [ads_budget] [decimal](18, 2) NULL,
        [roas] [int] NULL,
        [adspent_per_item] [decimal](18, 2) NULL,
        [platform_commission] [decimal](18, 2) NULL,
        [employee_commission] [decimal](18, 2) NULL,
        [shipping_fee] [decimal](18, 2) NULL,
        [rts_margin] [decimal](18, 2) NULL
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

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'tbl_work_hours')
BEGIN
    CREATE TABLE [dbo].[tbl_work_hours](
		id INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
		[user_id] [int] NOT NULL,
		[start_time] [datetime] NOT NULL,
		[end_time] [datetime] NULL,
		[hours_worked] [decimal](5, 2) NULL,
		[issued] [bit] NOT NULL DEFAULT 0,
		FOREIGN KEY ([user_id]) REFERENCES [tbl_users]([user_id])
    )
END
GO

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'tbl_wage')
BEGIN
    CREATE TABLE [dbo].[tbl_wage](
		id INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
		[user_id] [int] NOT NULL,
		[hourly_rate] [decimal](10, 2) NOT NULL,
		FOREIGN KEY ([user_id]) REFERENCES [tbl_users]([user_id])
    )
END
GO

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'tbl_reimbursement')
BEGIN
    CREATE TABLE [dbo].[tbl_reimbursement](
		id INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
		[user_id] [int] NOT NULL,
		[amount] [decimal](10, 2) NOT NULL,
		[description] [varchar](50) NOT NULL,
		[issued] [bit] NOT NULL DEFAULT 0,
		[is_valid] [bit] NOT NULL DEFAULT 0,
		FOREIGN KEY ([user_id]) REFERENCES [tbl_users]([user_id])
		)
END
GO


IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'tbl_overtime')
BEGIN
    CREATE TABLE [dbo].[tbl_overtime](
		id INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
		[user_id] [int] NOT NULL,
		[overtime] [decimal](10, 2) NOT NULL,
		[issued] [bit] NOT NULL DEFAULT 0,
		[is_valid] [bit] NOT NULL DEFAULT 0,
		FOREIGN KEY ([user_id]) REFERENCES [tbl_users]([user_id])
		)
END
GO

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'tbl_deductions')
BEGIN
    CREATE TABLE [dbo].[tbl_deductions](
		id INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
		[user_id] [int] NOT NULL,
		[amount] [decimal](10, 2) NOT NULL,
		[description] [varchar](50) NOT NULL,
		[issued] [bit] NOT NULL DEFAULT 0,
		[is_valid] [bit] NOT NULL DEFAULT 0,
		FOREIGN KEY ([user_id]) REFERENCES [tbl_users]([user_id])
		)
END
GO


IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'tbl_commissions')
BEGIN
    CREATE TABLE [dbo].[tbl_commissions](
		id INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
		[user_id] [int] NOT NULL,
		[commission_name] [varchar](50) NOT NULL,
		[commission_amount] [decimal](10, 2) NOT NULL,
		[issued] [bit] NOT NULL DEFAULT 0,
		[is_valid] [bit] NOT NULL DEFAULT 0,
		FOREIGN KEY ([user_id]) REFERENCES [tbl_users]([user_id])
		)
END
GO

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'tbl_address_book')
BEGIN
    CREATE TABLE tbl_address_book (
        id INT IDENTITY(1,1) PRIMARY KEY,
        first_name VARCHAR(50),
        middle_name VARCHAR(50),
        last_name VARCHAR(50),
        phone VARCHAR(20),
        province VARCHAR(50),
        city VARCHAR(50),
        barangay VARCHAR(50),
        address VARCHAR(255),
        created_at DATETIME DEFAULT GETDATE()
    );
END
GO

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'tbl_orders')
BEGIN
    CREATE TABLE tbl_orders (
        order_id INT PRIMARY KEY,
        user_id INT FOREIGN KEY REFERENCES tbl_users(user_id),
        sender_id INT FOREIGN KEY REFERENCES tbl_address_book(id),
        receiver_id INT FOREIGN KEY REFERENCES tbl_address_book(id),
        product_id VARCHAR(50) FOREIGN KEY REFERENCES tbl_products(product_id),
        quantity INT,
        total DECIMAL(10, 2),
        remarks VARCHAR(255),
        status VARCHAR(50),
        created_at DATETIME DEFAULT GETDATE(),
        updated_at DATETIME DEFAULT GETDATE()
    );
END
GO
