
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
-- Create tbl_printer_setting table if not exists
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'tbl_printer_setting')
BEGIN
    CREATE TABLE [dbo].[tbl_printer_setting](
        ID INT PRIMARY KEY IDENTITY(1,1),
		[Name] NVARCHAR(255),
		CourierName NVARCHAR(255),
    )
END
GO
-- Create tbl_waybill table if not exists
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'tbl_waybill')
BEGIN
    CREATE TABLE [dbo].[tbl_waybill](
        ID INT PRIMARY KEY IDENTITY(1,1),
		Order_ID NVARCHAR(250),
		Waybill NVARCHAR(250),
		Sorting_Code NVARCHAR(250),
		Sorting_No NVARCHAR(250),
		ReceiverName NVARCHAR(250),
		ReceiverProvince NVARCHAR(250),
		ReceiverCity NVARCHAR(250),
		ReceiverBarangay NVARCHAR(250),
		ReceiverAddress NVARCHAR(250),
		SenderName NVARCHAR(250),
		SenderAddress NVARCHAR(250),
		COD DECIMAL(18,2),
		Goods NVARCHAR(250),
		Price DECIMAL(18,2),
		[Weight] DECIMAL(18,2),
		Remarks NVARCHAR(Max),
    )
END
GO
-- Create tbl_users table if not exists
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'tbl_expenses')
BEGIN
   CREATE TABLE tbl_expenses(
	    ID INT PRIMARY KEY IDENTITY(1,1),
	    UserID INT,
	    AdSpent DECIMAL(18,2),
	    Utilities DECIMAL(18,2),
	    Miscellaneous DECIMAL(18,2),
	    [Date] DateTime2 DEFAULT GETDATE()
   )
END
GO
-- Create tbl_flashAddressing table if not exists
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'tbl_flashAddressing')
BEGIN
   CREATE TABLE tbl_flashAddressing (
		ID INT PRIMARY KEY IDENTITY(1,1),
		Province NVARCHAR(255),
		City NVARCHAR(255),
		Barangay NVARCHAR(255),
		PostalCode NVARCHAR(255),
	)
END
GO
-- Create tbl_bulk_order_temp table if not exists
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'tbl_bulk_order_temp')
BEGIN
   CREATE TABLE tbl_bulk_order_temp(
		ID INT PRIMARY KEY IDENTITY(1,1),
		Quantity INT,
		ItemName NVARCHAR(255),
		ReceiverName NVARCHAR(255),
		ReceiverContactNumber NVARCHAR(50),
		ReceiverAddress NVARCHAR(255),
		ReceiverProvince NVARCHAR(255),
		ReceiverCity NVARCHAR(255),
		ReceiverRegion NVARCHAR(255),
		ParcelName NVARCHAR(255),
		[Weight] DECIMAL(18,2),
		TotalParcel INT,
		ParcelValue DECIMAL(18,2),
		COD DECIMAL(18,2),
		Remarks NVARCHAR(255)
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
        [hourly_rate] [decimal](10, 2) NULL,
		[enabled] [bit] NOT NULL DEFAULT 1,
    )
END
GO

-- Check if column 'enabled' exists in tbl_roles
IF NOT EXISTS (SELECT * FROM sys.columns WHERE Name = N'enabled' AND Object_ID = Object_ID(N'dbo.tbl_roles'))
BEGIN
    -- Add column 'enabled' to tbl_roles
    ALTER TABLE dbo.tbl_roles
    ADD [enabled] [bit] NOT NULL DEFAULT 1
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
        (@roleId, 'Modify System Settings'),
		(@roleId, 'Modify Out For Pick Up'),
        (@roleId, 'View Out For Pick Up'), 
		(@roleId, 'Modify Shop/Pages'),
        (@roleId, 'View Shop/Pages'),
        (@roleId, 'View Suspicious Order')
END

BEGIN
-- Check if the specified module names exist for role_id = 1
IF NOT EXISTS (
    SELECT *
    FROM tbl_module_access
    WHERE role_id = 1
        AND module_name IN (
            'Modify Out For Pick Up',
            'View Out For Pick Up',
            'Modify Shop/Pages',
            'View Shop/Pages',
            'View Suspicious Order'
        )
)
    -- Insert the missing module names for role_id = 1
    INSERT INTO tbl_module_access (role_id, module_name)
    VALUES
        (1, 'Modify Out For Pick Up'),
        (1, 'View Out For Pick Up'),
        (1, 'Modify Shop/Pages'),
        (1, 'View Shop/Pages'),
        (1, 'View Suspicious Order');
END


-- Create tbl_access_level table if not exists
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'tbl_access_level')
BEGIN
    CREATE TABLE [dbo].[tbl_access_level](
        [id] [int] IDENTITY(1,1) NOT NULL PRIMARY KEY,
        [user_id] [int] NOT NULL,
        [role_id] [int] NULL,
        CONSTRAINT FK_tbl_access_level_tbl_roles FOREIGN KEY ([role_id]) REFERENCES [dbo].tbl_roles
    )
END
GO

-- Check if [role_id] [int] column is NOT NULL
IF EXISTS (
    SELECT * 
    FROM sys.columns 
    WHERE Name = N'role_id' 
    AND Object_ID = Object_ID(N'dbo.tbl_access_level')
    AND is_nullable = 0
)
BEGIN
    -- Alter [role_id] column to allow NULL values
    ALTER TABLE dbo.tbl_access_level
    ALTER COLUMN [role_id] [int] NULL
END


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
        [receiver_address] [varchar](255) NOT NULL,
		)
END

IF EXISTS (SELECT * FROM sys.tables WHERE name = 'tbl_receiver')
BEGIN
    IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('dbo.tbl_receiver') AND name = 'receiver_address')
    BEGIN
        -- Alter column 'receiver_address' to VARCHAR(255)
        ALTER TABLE dbo.tbl_receiver
        ALTER COLUMN receiver_address VARCHAR(255) NOT NULL
    END
END

--IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'tbl_sender')
--BEGIN
--    CREATE TABLE [dbo].[tbl_sender](
--		sender_id INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
--		courier_id INT NOT NULL,
--		[sender_name] [varchar](255) NOT NULL,
--        [sender_province] [varchar](255) NOT NULL,
--        [sender_city] [varchar](255) NOT NULL,
--        [sender_baranggay] [varchar](255) NOT NULL,
--		[sender_postalCode] [varchar](50) NOT NULL,
--		[sender_phone] [varchar](50) NOT NULL,
--        [sender_address] [varchar](255) NOT NULL,
--		)
--END
IF EXISTS (SELECT * FROM sys.tables WHERE name = 'tbl_sender')
BEGIN
    IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('dbo.tbl_sender') AND name = 'sender_address')
    BEGIN
        -- Alter column 'receiver_address' to VARCHAR(255)
        ALTER TABLE dbo.tbl_sender
        ALTER COLUMN sender_address VARCHAR(255) NOT NULL
		ALTER TABLE dbo.tbl_sender
        ALTER COLUMN sender_name VARCHAR(255) NOT NULL
    END
END

IF OBJECT_ID('dbo.tbl_sender', 'U') IS NOT NULL
BEGIN
    IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('dbo.tbl_sender') AND name = 'sender_postalCode')
    BEGIN
        ALTER TABLE [dbo].[tbl_sender]
        ADD [sender_postalCode] Varchar(50) NULL
		ALTER TABLE [dbo].[tbl_sender]
        ADD [courier_id] INT NULL
    END
END

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'tbl_status')
BEGIN
    CREATE TABLE [dbo].[tbl_status](
		status_id INT IDENTITY(1,1) PRIMARY KEY,
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
    (courier_id INT IDENTITY(1,1) PRIMARY KEY,
    [courier_name] [varchar](50) NOT NULL,
    [api_key] [varchar](255) NOT NULL,
    [eccompany_id] [varchar](50) NOT NULL,
    [customer_id] [varchar](50) NOT NULL,
    )
END

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'tbl_order_inquiry')
BEGIN
    CREATE TABLE [dbo].[tbl_order_inquiry](
	[order_inquiry_id] INT IDENTITY(1,1) PRIMARY KEY,
	[waybill#] [varchar](50) NOT NULL,
	[receiver_name] [varchar](50) NOT NULL,
	[contact_number] [varchar](50) NOT NULL,
	[address] [varchar](255) NOT NULL,
	[product_name] [varchar](100) NOT NULL,
	[qty] [varchar](50) NOT NULL,
	[weight] [varchar](50) NOT NULL,
	[remarks] [varchar](50) NOT NULL,
    [date_created] [varchar](50) NOT NULL,
	[session_id] [nvarchar](100) NOT NULL
    )
END

IF EXISTS (SELECT * FROM sys.tables WHERE name = 'tbl_order_inquiry')
BEGIN
    IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('dbo.tbl_order_inquiry') AND name = 'remarks')
    BEGIN
        ALTER TABLE dbo.tbl_order_inquiry
        ALTER COLUMN remarks VARCHAR(255) NOT NULL
    END
END

IF OBJECT_ID('dbo.tbl_order_inquiry', 'U') IS NOT NULL
BEGIN
    IF NOT EXISTS (SELECT * FROM sys.columns WHERE Name = 'session_id' AND Object_ID = OBJECT_ID('dbo.tbl_order_inquiry'))
    BEGIN
        ALTER TABLE [dbo].[tbl_order_inquiry]
        ADD [session_id] [nvarchar](100) NULL
    END
END

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'tbl_suspicious_order')
BEGIN
    CREATE TABLE [dbo].[tbl_suspicious_order](
	[suspicious_order_id] INT IDENTITY(1,1) PRIMARY KEY,
	[user_id] INT NOT NULL,
	[role_id] INT NOT NULL,
	[sender_id] INT NOT NULL,
	[product_id] [nvarchar](50) NOT NULL,
	[receiver_id] INT NOT NULL,
	[waybill] [varchar](100) NOT NULL,
	[courier] [varchar](50) NOT NULL,
	[status] [varchar](50) NOT NULL,
    [booked_date] DATETIME DEFAULT GETDATE(),
    [price] [decimal](18,2) NOT NULL
    )
END

IF NOT EXISTS (SELECT * FROM sys.procedures WHERE name = 'SPadd_sender_info')
BEGIN
    EXEC('
    CREATE PROC SPadd_sender_info
	@sender_id INT,
    @sender_name VARCHAR(255),
    @sender_province VARCHAR(100),
    @sender_city VARCHAR(100),
    @sender_baranggay VARCHAR(100),
    @sender_phone VARCHAR(50),
    @sender_address VARCHAR(255)
	@courier_id INT,
	@sender_postalCode VARCHAR(100)
    AS
    BEGIN
      IF @sender_id = 0
		BEGIN
			INSERT INTO tbl_sender(sender_name, sender_province, sender_city, sender_baranggay, sender_phone, sender_address, courier_id, sender_postalCode) VALUES
			(@sender_name, @sender_province, @sender_city, @sender_baranggay, @sender_phone, @sender_address, @courier_id, @sender_postalCode)
	  END
	  ELSE
	  BEGIN
			UPDATE tbl_sender SET
				sender_name = @sender_name,
				sender_province = @sender_province,
				sender_city = @sender_city,
				sender_baranggay = @sender_baranggay,
				sender_phone = @sender_phone,
				sender_address = @sender_address,
				courier_id = @courier_id,
				sender_postalCode = @sender_postalCode
			WHERE sender_id = @sender_id
	  END
    END;
    ');
END;
--ALTER
IF EXISTS (SELECT * FROM sys.procedures WHERE name = 'SPadd_sender_info')
BEGIN
    EXEC('
    ALTER PROC SPadd_sender_info
	@sender_id INT,
    @sender_name VARCHAR(255),
    @sender_province VARCHAR(100),
    @sender_city VARCHAR(100),
    @sender_baranggay VARCHAR(100),
    @sender_phone VARCHAR(50),
    @sender_address VARCHAR(255),
	@courier_id INT,
	@sender_postalCode VARCHAR(100)
    AS
    BEGIN
      IF @sender_id = 0
		BEGIN
			INSERT INTO tbl_sender(sender_name, sender_province, sender_city, sender_baranggay, sender_phone, sender_address, courier_id, sender_postalCode) VALUES
			(@sender_name, @sender_province, @sender_city, @sender_baranggay, @sender_phone, @sender_address, @courier_id, @sender_postalCode)
	  END
	  ELSE
	  BEGIN
			UPDATE tbl_sender SET
				sender_name = @sender_name,
				sender_province = @sender_province,
				sender_city = @sender_city,
				sender_baranggay = @sender_baranggay,
				sender_phone = @sender_phone,
				sender_address = @sender_address,
				courier_id = @courier_id,
				sender_postalCode = @sender_postalCode
			WHERE sender_id = @sender_id
	  END
    END;
    ');
END;
IF EXISTS (SELECT * FROM sys.procedures WHERE name = 'SPadd_receiver')
BEGIN
    EXEC('
    ALTER PROC [dbo].[SPadd_receiver]
    @receiver_name VARCHAR(255),
    @receiver_phone VARCHAR(100),
    @receiver_address VARCHAR(MAX)
    AS
    BEGIN
        INSERT INTO tbl_receiver(receiver_name, receiver_phone, receiver_address) VALUES
        (@receiver_name, @receiver_phone,@receiver_address)
    END;
    ');
END;

IF NOT EXISTS (SELECT * FROM sys.procedures WHERE name = 'SPadd_receiver')
BEGIN
    EXEC('
    CREATE PROC SPadd_receiver
    @receiver_name VARCHAR(255),
    @receiver_phone VARCHAR(100),
    @receiver_address VARCHAR(MAX)
    AS
    BEGIN
        INSERT INTO tbl_receiver(receiver_name, receiver_phone, receiver_address) VALUES
        (@receiver_name, @receiver_phone,@receiver_address)
    END;
    ');
END;

IF EXISTS (SELECT * FROM sys.procedures WHERE name = 'SPadd_orders')
BEGIN
    EXEC('
    ALTER PROC [dbo].[SPadd_orders]
	@order_id VARCHAR(255),
	@courier VARCHAR(50),
	@waybill_number VARCHAR(255),
	@user_id INT,
	@product_name VARCHAR(255),
	@quantity INT,
	@total DECIMAL (18, 2),
	@remarks VARCHAR(255),
	@status VARCHAR(255),
	@receiver_phone VARCHAR(255),
	@receiver_address VARCHAR(MAX),
	@sender_id INT
	AS
    BEGIN
		DECLARE @receiver_id INT = (SELECT TOP 1(receiver_id) FROM tbl_receiver WHERE receiver_phone = @receiver_phone AND receiver_address LIKE ''%'' + @receiver_address + ''%'' ORDER BY receiver_id DESC)
		DECLARE @product_id VARCHAR(255) = (SELECT product_id FROM tbl_products WHERE item_name = @product_name)

        INSERT INTO tbl_orders(order_id, courier, waybill_number, user_id, receiver_id, product_id, quantity, total, remarks, status, sender_id) VALUES
        (@order_id, @courier, @waybill_number, @user_id, @receiver_id, @product_id, @quantity, @total, @remarks, @status, @sender_id)
		
    END;
    ');
END;

IF NOT EXISTS (SELECT * FROM sys.procedures WHERE name = 'SPadd_orders')
BEGIN
    EXEC('
    CREATE PROC SPadd_orders
	@order_id VARCHAR(255),
	@courier VARCHAR(50),
	@waybill_number VARCHAR(255),
	@user_id INT,
	@product_name VARCHAR(255),
	@quantity INT,
	@total DECIMAL (18, 2),
	@remarks VARCHAR(255),
	@status VARCHAR(255),
	@receiver_phone VARCHAR(255),
	@receiver_address VARCHAR(MAX),
	@sender_id INT
	AS
    BEGIN
		DECLARE @receiver_id INT = (SELECT TOP 1(receiver_id) FROM tbl_receiver WHERE receiver_phone = @receiver_phone AND receiver_address = @receiver_address ORDER BY receiver_id DESC)
		DECLARE @product_id VARCHAR(255) = (SELECT product_id FROM tbl_products WHERE item_name = @product_name)

        INSERT INTO tbl_orders(order_id, courier, waybill_number, user_id, receiver_id, product_id, quantity, total, remarks, status, sender_id) VALUES
        (@order_id, @courier, @waybill_number, @user_id, @receiver_id, @product_id, @quantity, @total, @remarks, @status, @sender_id)
		
    END;
    ');
END;

IF NOT EXISTS (SELECT * FROM sys.procedures WHERE name = 'SPadd_incentives')
BEGIN
    EXEC('
    CREATE PROC SPadd_incentives
	@user_id INT,
	@order_id VARCHAR(255),
	@quantity INT,
	@is_valid BIT,
	@product_name VARCHAR(255)
	AS
    BEGIN
		DECLARE @product_id VARCHAR(255) = (SELECT product_id FROM tbl_products WHERE item_name = @product_name)
		DECLARE @employee_commission DECIMAL (18,2) = (SELECT employee_commission FROM tbl_selling_expenses WHERE product_id = @product_id)

		DECLARE @total_commissions DECIMAL (18, 2) = (@employee_commission * @quantity)

		INSERT INTO tbl_incentives(user_id, incentive_for, quantity, total_incentive, is_valid) VALUES
		(@user_id, @product_id, @quantity, @total_commissions, @is_valid)
		
    END;
    ');
END;

IF NOT EXISTS (SELECT * FROM sys.procedures WHERE name = 'SPupdate_stocks')
BEGIN
    EXEC('
    CREATE PROC SPupdate_stocks
	@quantity INT,
	@product_name VARCHAR(255)
	AS
    BEGIN
		DECLARE @product_id VARCHAR(255) = (SELECT product_id FROM tbl_products WHERE item_name = @product_name)

		UPDATE tbl_products SET unit_quantity = unit_quantity - @quantity WHERE product_id = @product_id
    END;
    ');
END;


IF NOT EXISTS (SELECT * FROM sys.procedures WHERE name = 'SPadd_temptable')
BEGIN
    EXEC('
    CREATE PROC SPadd_temptable
	@quantity INT,
	@product_name NVARCHAR(255),
	@receiver_name NVARCHAR(255),
	@receiver_phone NVARCHAR(255),
	@receiver_address NVARCHAR(255),
	@receiver_province NVARCHAR(255),
	@receiver_city NVARCHAR(255),
	@receiver_area NVARCHAR(255),
	@parcel_name NVARCHAR(255),
	@weight DECIMAL (18, 2),
	@total_parcel INT,
	@parcel_value DECIMAL(18, 2),
	@cod DECIMAL(18, 2),
	@remarks NVARCHAR(255)
	AS
    BEGIN
		INSERT INTO tbl_bulk_order_temp (Quantity, ItemName, ReceiverName, ReceiverContactNumber, ReceiverAddress, ReceiverProvince, ReceiverCity, ReceiverRegion, ParcelName, Weight,
		TotalParcel, ParcelValue, COD, Remarks) VALUES (@quantity, @product_name, @receiver_name, @receiver_phone, @receiver_address, @receiver_province, @receiver_city, @receiver_area,
		@parcel_name, @weight, @total_parcel, @parcel_value, @cod, @remarks)
    END;
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
-- StoreProc for Trial Insertion
IF EXISTS (SELECT * FROM sys.procedures WHERE name = 'Sp_Trial_Insertion')
BEGIN
	EXEC('
	ALTER PROC [dbo].[Sp_Trial_Insertion]
    AS
    BEGIN
		DECLARE @LastDate DATE = (SELECT TOP 1 [Date] FROM tbl_trial ORDER BY ID DESC)
	    DECLARE @DateNow DATE = (SELECT GETDATE())
		DECLARE @DateCount INT = (SELECT DATEDIFF(DAY, @LastDate, @DateNow) DateCount)
	    IF @DateCount <> 0
		BEGIN
			DECLARE @Counter INT  = 1;
			DECLARE @DateAppend DATE = (SELECT DATEADD(DAY, 1, @LastDate));
			WHILE ( @Counter <= @DateCount)
			BEGIN
				INSERT INTO tbl_trial([Date]) VALUES (@DateAppend);
				SET @DateAppend = (SELECT DATEADD(DAY, 1, @DateAppend));
				SET @Counter  = @Counter  + 1
			END
		END
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
--StoreProc for Insertion of Expenses_Insert
IF NOT EXISTS (SELECT * FROM sys.procedures WHERE name = 'SpExpenses_Insert')
BEGIN
	EXEC('
	CREATE PROC SpExpenses_Insert
    @UserID INT,
    @AdSpent DECIMAL(18,2),
    @Utilities DECIMAL(18,2),
    @Miscellaneous DECIMAL(18,2)
    AS
    BEGIN
	    INSERT INTO tbl_expenses(AdSpent, Utilities, Miscellaneous, UserID) VALUES 
	    (@AdSpent, @Utilities, @Miscellaneous, @UserID)
    END;
	');
END;
--StoreProc for Getting Data of expenses
IF NOT EXISTS (SELECT * FROM sys.procedures WHERE name = 'SpExpenses_GetData')
BEGIN
	EXEC('
	CREATE PROC SpExpenses_GetData
    AS
    BEGIN
	    SELECT ID, UserID, AdSpent, Utilities, Miscellaneous, SUM(AdSpent) + SUM(Utilities) + SUM(Miscellaneous)
	    FROM tbl_expenses
	    GROUP BY ID,UserID,AdSpent, Utilities, Miscellaneous 
	    UNION ALL
	    SELECT 0 AS ID, 0 AS UserID, 
		       SUM(AdSpent) AS TotalAdSpent, 
		       SUM(Utilities) AS TotalUtilities, 
		       SUM(Miscellaneous) AS TotalMiscellaneous,
		       SUM(AdSpent) + SUM(Utilities) + SUM(Miscellaneous) AS GrandTotal
	    FROM tbl_expenses
    END;
	');

END;
--StoreProc for Getting Data of expenses filter by Date
IF NOT EXISTS (SELECT * FROM sys.procedures WHERE name = 'SpExpenses_GetDataFilterByDate')
BEGIN
	EXEC('
	CREATE PROC SpExpenses_GetDataFilterByDate
    @DateFrom Date,
    @DateCountAdd INT
    AS
    BEGIN
	    DECLARE @DateTo Date = DATEADD(DAY, -@DateCountAdd, @DateFrom)

	    SELECT ID, UserID, AdSpent, Utilities, Miscellaneous, SUM(AdSpent) + SUM(Utilities) + SUM(Miscellaneous)
	    FROM tbl_expenses WHERE [Date] BETWEEN @DateTo AND  @DateFrom 
	    GROUP BY ID, UserID, AdSpent, Utilities, Miscellaneous 
	    UNION ALL
	    SELECT NULL AS ID, NULL AS UserID, 
		       SUM(AdSpent) AS TotalAdSpent, 
		       SUM(Utilities) AS TotalUtilities, 
		       SUM(Miscellaneous) AS TotalMiscellaneous,
		       SUM(AdSpent) + SUM(Utilities) + SUM(Miscellaneous) AS GrandTotal
	    FROM tbl_expenses  WHERE [Date] BETWEEN @DateTo AND @DateFrom
    END
	');
END;
--StoreProc for Displaying suspucious bulk order
IF NOT EXISTS (SELECT * FROM sys.procedures WHERE name = 'SpDisplay_SuspeciousBulk')
BEGIN
	EXEC('
	CREATE PROC SpDisplay_SuspeciousBulk
	AS
	BEGIN
		SELECT 
			ID,
			Quantity,
			ItemName [Item Name],
			ReceiverName [Receiver Name],
			ReceiverContactNumber [Receiver Contact Number],
			ReceiverAddress [Receiver Address],
			ReceiverProvince [Receiver Province],
			ReceiverCity [Receiver City],
			ReceiverRegion [Receiver Region],
			ParcelName [Parcel Name],
			[Weight],
			TotalParcel [Total Parcel],
			ParcelValue [Parcel Value],
			COD,
			Remarks
			FROM tbl_bulk_order_temp
	END;
	');
END;

--StoreProc for Saving  tbl_bulk_order_temp data
IF NOT EXISTS (SELECT * FROM sys.procedures WHERE name = 'SpSave_BulkOrderTemp')
BEGIN
	EXEC('
	CREATE PROC SpSave_BulkOrderTemp
	@ID INT,
	@ItemName NVARCHAR(255),
	@Quantity INT,
	@ReceiverName NVARCHAR(255),
	@ReceiverContactNumber NVARCHAR(50),
	@ReceiverAddress NVARCHAR(255),
	@ReceiverProvince NVARCHAR(255),
	@ReceiverCity NVARCHAR(255),
	@ReceiverRegion NVARCHAR(255),
	@ParcelName NVARCHAR(255),
	@Weight DECIMAL(18,2),
	@TotalParcel INT,
	@ParcelValue DECIMAL(18,2),
	@COD DECIMAL(18,2),
	@Remarks NVARCHAR(255)
	AS
	BEGIN
		IF @ID = 0 
		BEGIN
			INSERT INTO tbl_bulk_order_temp(ItemName,Quantity,ReceiverName,ReceiverContactNumber,ReceiverAddress,ReceiverProvince,ReceiverCity,ReceiverRegion,ParcelName,[Weight], TotalParcel,ParcelValue,COD, Remarks) VALUES
			(@ItemName, @Quantity, @ReceiverName, @ReceiverContactNumber, @ReceiverAddress, @ReceiverProvince, @ReceiverCity, @ReceiverRegion, @ParcelName, @Weight, @TotalParcel, @ParcelValue, @COD, @Remarks);
		END;
		ELSE
		BEGIN
			UPDATE tbl_bulk_order_temp SET
				ItemName = @ItemName,
				Quantity = @Quantity,
				ReceiverName = @ReceiverName,
				ReceiverContactNumber = @ReceiverContactNumber,
				ReceiverAddress = @ReceiverAddress,
				ReceiverProvince = @ReceiverProvince,
				ReceiverCity = @ReceiverCity,
				ReceiverRegion = @ReceiverRegion,
				ParcelName = @ParcelName,
				[Weight] = @Weight,
				TotalParcel = @TotalParcel,
				ParcelValue = @ParcelValue,
				COD = @COD,
				Remarks = @Remarks
			WHERE ID = @ID
		END;
	END;
	');
END;


