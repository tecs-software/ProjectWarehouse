
USE db_warehouse_management
GO

-- Create tbl_users table if not exists
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'tbl_users')
BEGIN
    CREATE TABLE [dbo].[tbl_users](
        [user_id] [int] IDENTITY(1,1) NOT NULL PRIMARY KEY,
        [sender_id] [int] DEFAULT 1,
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
--Create tbl_trial
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'tbl_trial')
BEGIN
    CREATE TABLE tbl_trial(
	    ID INT PRIMARY KEY IDENTITY(1,1),
	    [Date] DATE 
    )
END
GO
--Create tbl_trial_key
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'tbl_trial_key')
BEGIN
    CREATE TABLE tbl_trial_key(
	    ID INT PRIMARY KEY IDENTITY(1,1),
	    Product_Key NVARCHAR(255)
    )
END
GO
-- Create tbl_products table if not exists
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'tbl_address_delivery')
BEGIN
   CREATE TABLE tbl_address_delivery(
	    ID INT PRIMARY KEY IDENTITY(1,1),
	    Province NVARCHAR(255),
	    City NVARCHAR(255),
	    AreaName NVARCHAR(255),
	    CanDeliver BIT
    )
END
GO

-- Create tbl_products table if not exists
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'tbl_products')
BEGIN
    CREATE TABLE [dbo].[tbl_products](
        [product_id] [varchar](50) NOT NULL PRIMARY KEY,
        [sender_id] [int] DEFAULT 1,
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

-- Create tbl_roles table if not exists
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'tbl_roles')
BEGIN
    CREATE TABLE [dbo].[tbl_roles](
        [role_id] [int] IDENTITY(1,1) PRIMARY KEY,
        [role_name] [varchar](50) NOT NULL,
        [hourly_rate] [decimal](10, 2) NULL
    )
END
GO

-- Create tbl_module_access table if not exists
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'tbl_module_access')
BEGIN
    CREATE TABLE [dbo].[tbl_module_access](
        [role_id] [int] NOT NULL,
        [module_name] [varchar](120) NOT NULL,
        CONSTRAINT FK_tbl_module_access_tbl_roles FOREIGN KEY (role_id) REFERENCES tbl_roles(role_id)
    )
END
GO

-- Insert initial data into tbl_module_access if it's empty
IF NOT EXISTS (SELECT * FROM tbl_roles)
BEGIN
    -- Insert into tbl_roles and retrieve the last inserted ID
    DECLARE @roleId INT;
    INSERT INTO tbl_roles (role_name, hourly_rate)
    VALUES ('admin', NULL);
    SET @roleId = SCOPE_IDENTITY();

    -- Insert into tbl_module_access using the last inserted ID
    INSERT INTO tbl_module_access (role_id, module_name)
    VALUES
        (@roleId, 'View Dashboard'),
        (@roleId, 'View Inventory'),
        (@roleId, 'Modify Inventory'),
        (@roleId, 'View Order'),
        (@roleId, 'Modify Order'),
        (@roleId, 'View Employee'),
        (@roleId, 'Modify Employee'),
        (@roleId, 'Modify System Settings')
END

-- Create tbl_access_level table if not exists
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'tbl_access_level')
BEGIN
    CREATE TABLE [dbo].[tbl_access_level](
        [id] [int] IDENTITY(1,1) NOT NULL PRIMARY KEY,
        [user_id] [int] NOT NULL,
        [role_id] [int] NOT NULL,
        CONSTRAINT FK_tbl_access_level_tbl_roles FOREIGN KEY ([role_id]) REFERENCES [dbo].tbl_roles
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
        created_at DATETIME DEFAULT GETDATE(),
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
        created_at DATETIME DEFAULT GETDATE(),
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
        created_at DATETIME DEFAULT GETDATE(),
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
        created_at DATETIME DEFAULT GETDATE(),
		FOREIGN KEY ([user_id]) REFERENCES [tbl_users]([user_id])
		)
END
GO

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'tbl_incentives')
BEGIN
    CREATE TABLE [dbo].[tbl_incentives](
		id INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
		[user_id] [int] NOT NULL,
		[incentive_for] [varchar](50) NOT NULL,
        [quantity] [int] NOT NULL,
		[total_incentive] [decimal](10, 2) NOT NULL,
		[issued] [bit] NOT NULL DEFAULT 0,
		[is_valid] [bit] NOT NULL DEFAULT 0,
        created_at DATETIME DEFAULT GETDATE(),
		FOREIGN KEY ([user_id]) REFERENCES [tbl_users]([user_id])
		)
END
GO

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'tbl_orders')
BEGIN
    CREATE TABLE tbl_orders (
        order_id VARCHAR(50) PRIMARY KEY,
        courier VARCHAR(50),
        [waybill_number] VARCHAR(50),
        user_id INT FOREIGN KEY REFERENCES tbl_users(user_id),
        sender_id INT,
        receiver_id INT,
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

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'tbl_receiver')
BEGIN
    CREATE TABLE [dbo].[tbl_receiver](
		receiver_id INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
		[receiver_name] [varchar](50) NOT NULL,
		[receiver_phone] [varchar](50) NOT NULL,
        [receiver_address] [varchar](50) NOT NULL,
		)
END

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'tbl_sender')
BEGIN
    CREATE TABLE [dbo].[tbl_sender](
		sender_id INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
		[sender_name] [varchar](50) NOT NULL,
        [sender_province] [varchar](50) NOT NULL,
        [sender_city] [varchar](50) NOT NULL,
        [sender_baranggay] [varchar](50) NOT NULL,
		[sender_phone] [varchar](50) NOT NULL,
        [sender_address] [varchar](50) NOT NULL,
		)
END

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'tbl_status')
BEGIN
    CREATE TABLE [dbo].[tbl_status](
		status_id INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
		[waybill#] [varchar](50) NOT NULL,
		[scan_type] [varchar](50) NOT NULL,
        [description] [varchar](255) NOT NULL,
        [scan_time] DATETIME DEFAULT GETDATE()
		)
END
GO
--Cejo tries store proc
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'tbl_couriers')
BEGIN
    CREATE TABLE [dbo].[tbl_couriers]
    (courier_id INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    [courier_name] [varchar](50) NOT NULL,
    [api_key] [varchar](255) NOT NULL,
    [eccompany_id] [varchar](50) NOT NULL,
    [customer_id] [varchar](50) NOT NULL,
    )
END

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'tbl_order_inquiry')
BEGIN
    CREATE TABLE [dbo].[tbl_order_inquiry](
	[order_inquiry_id] [int] IDENTITY(1,1) NOT NULL,
	[waybill#] [varchar](50) NOT NULL,
	[receiver_name] [varchar](50) NOT NULL,
	[contact_number] [varchar](50) NOT NULL,
	[address] [varchar](255) NOT NULL,
	[product_name] [varchar](100) NOT NULL,
	[qty] [varchar](50) NOT NULL,
	[weight] [varchar](50) NOT NULL,
	[remarks] [varchar](50) NOT NULL
    )
END

IF NOT EXISTS (SELECT * FROM sys.procedures WHERE name = 'SPadd_sender_info')
BEGIN
    EXEC('
    CREATE PROC SPadd_sender_info
    @sender_name VARCHAR(255),
    @sender_province VARCHAR(100),
    @sender_city VARCHAR(100),
    @sender_baranggay VARCHAR(100),
    @sender_phone VARCHAR(50),
    @sender_address VARCHAR(255)
    AS
    BEGIN
        INSERT INTO tbl_sender(sender_name, sender_province, sender_city, sender_baranggay, sender_phone, sender_address) VALUES
        (@sender_name, @sender_province, @sender_city, @sender_baranggay, @sender_phone, @sender_address)
    END
    ');
END;
    
--CREATION OF STORE PROCS

-- StoreProc for Importing Address 
IF NOT EXISTS (SELECT * FROM sys.procedures WHERE name = 'SpAddress_Import')
BEGIN
	EXEC('
	CREATE PROC SpAddress_Import
	@Province NVARCHAR(255),
	@City NVARCHAR(255),
	@AreaName NVARCHAR(255),
	@CanDeliver INT
	AS
	BEGIN
		INSERT INTO tbl_address_delivery(Province, City, AreaName, CanDeliver) VALUES
		(@Province, @City, @AreaName, @CanDeliver);
	END;
	');
END;

-- StoreProc for Truncating Address table
IF NOT EXISTS (SELECT * FROM sys.procedures WHERE name = 'SpAddress_Truncate')
BEGIN
	EXEC('
	CREATE PROC SpAddress_Truncate
	AS
	BEGIN
		DECLARE @AddressCount INT = (SELECT COUNT(ID) FROM tbl_address_delivery)

		IF @AddressCount != 0
		BEGIN
			TRUNCATE TABLE tbl_address_delivery;
		END;
	END;
	');
END;
-- StoreProc for Trial Insertion
IF NOT EXISTS (SELECT * FROM sys.procedures WHERE name = 'Sp_Trial_Insertion')
BEGIN
	EXEC('
	CREATE PROC Sp_Trial_Insertion
    AS
    BEGIN
	    DECLARE @DateNow DATE = (SELECT GETDATE())
	    DECLARE @IsExist INT = (SELECT COUNT(*) FROM tbl_trial WHERE [Date] = @DateNow )
	    IF @IsExist = 0
	    BEGIN
		    INSERT INTO tbl_trial([Date]) VALUES (@DateNow)
	    END;
    END
	');
END;
-- StoreProc for Trial Validation
IF NOT EXISTS (SELECT * FROM sys.procedures WHERE name = 'Sp_Trial_Validation')
BEGIN
	EXEC('
	CREATE PROC Sp_Trial_Validation
    AS
    BEGIN
	    SELECT COUNT(*) FROM tbl_trial 
    END;
	');
END;
--StoreProc for Trial Product Key checker
IF NOT EXISTS (SELECT * FROM sys.procedures WHERE name = 'SpTrial_HaveKey')
BEGIN
	EXEC('
	CREATE PROC SpTrial_HaveKey
    AS
    BEGIN
	    SELECT COUNT(*) FROM tbl_trial_key
    END;
	');
END;