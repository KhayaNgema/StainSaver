-- Check if the table already exists
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'CustomerNotifications')
BEGIN
    -- Create the CustomerNotifications table
    CREATE TABLE [CustomerNotifications] (
        [Id] INT NOT NULL IDENTITY,
        [CustomerId] NVARCHAR(450) NOT NULL,
        [Title] NVARCHAR(MAX) NOT NULL,
        [Message] NVARCHAR(MAX) NOT NULL,
        [BookingId] INT NULL,
        [CreatedDate] DATETIME2 NOT NULL,
        [IsRead] BIT NOT NULL,
        CONSTRAINT [PK_CustomerNotifications] PRIMARY KEY ([Id])
    );
    
    -- Add foreign key to AspNetUsers (Customer)
    ALTER TABLE [CustomerNotifications] 
    ADD CONSTRAINT [FK_CustomerNotifications_AspNetUsers_CustomerId] 
    FOREIGN KEY ([CustomerId]) REFERENCES [AspNetUsers] ([Id]) 
    ON DELETE CASCADE;
    
    -- Add foreign key to Bookings
    ALTER TABLE [CustomerNotifications] 
    ADD CONSTRAINT [FK_CustomerNotifications_Bookings_BookingId] 
    FOREIGN KEY ([BookingId]) REFERENCES [Bookings] ([Id]) 
    ON DELETE SET NULL;
    
    PRINT 'CustomerNotifications table created successfully.';
END
ELSE
BEGIN
    PRINT 'CustomerNotifications table already exists.';
END 