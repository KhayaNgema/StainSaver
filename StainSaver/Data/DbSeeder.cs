using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using StainSaver.Models;

namespace StainSaver.Data
{
    public static class DbSeeder
    {
        public static async Task SeedRolesAndUsers(IServiceProvider serviceProvider)
        {
            // Create roles
            using var scope = serviceProvider.CreateScope();
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

            // Add roles
            string[] roleNames = { "Admin", "Customer", "Staff", "Driver" };
            foreach (var roleName in roleNames)
            {
                if (!await roleManager.RoleExistsAsync(roleName))
                {
                    await roleManager.CreateAsync(new IdentityRole(roleName));
                }
            }

            // Create admin users
            await CreateUser(userManager, "admin1@stainsaver.co.za", "Admin@123", "Admin", "Admin1", "9101015021088", "14 Main Road", "Sandton", "Johannesburg", "Gauteng", "2196", "0833456789");
            await CreateUser(userManager, "admin2@stainsaver.co.za", "Admin@123", "Admin", "Admin2", "8207125078083", "22 Long Street", "Green Point", "Cape Town", "Western Cape", "8001", "0825678901");

            // Create customer users
            await CreateUser(userManager, "customer1@gmail.com", "Customer@123", "Customer", "Sipho Nkosi", "8605125021083", "42 Vilakazi Street", "Soweto", "Johannesburg", "Gauteng", "1804", "0712345678");
            await CreateUser(userManager, "customer2@gmail.com", "Customer@123", "Customer", "Thandi Mbeki", "9010135082084", "12 Beach Road", "Umhlanga", "Durban", "KwaZulu-Natal", "4320", "0823456789");

            // Create staff users
            await CreateUser(userManager, "staff1@stainsaver.co.za", "Staff@123", "Staff", "John Botha", "7508185027083", "10 Market Street", "Maboneng", "Johannesburg", "Gauteng", "2094", "0734567890");
            await CreateUser(userManager, "staff2@stainsaver.co.za", "Staff@123", "Staff", "Lerato Ndlovu", "8312245028087", "5 Bree Street", "CBD", "Cape Town", "Western Cape", "8000", "0845678901");

            // Create driver users
            await CreateUser(userManager, "driver1@stainsaver.co.za", "Driver@123", "Driver", "Blessing Zuma", "9005125021083", "75 Main Avenue", "Diepsloot", "Johannesburg", "Gauteng", "2189", "0765432109");
            await CreateUser(userManager, "driver2@stainsaver.co.za", "Driver@123", "Driver", "Mandla van Wyk", "8504045082083", "23 Kloof Road", "Camps Bay", "Cape Town", "Western Cape", "8005", "0789876543");
        }

        public static async Task SeedLaundryServices(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            
            try
            {
                // Ensure database is created
                await context.Database.EnsureCreatedAsync();
                
                // Try to create the LaundryServices table if it doesn't exist
                bool tableExists = false;
                try
                {
                    // Check if services already exist
                    tableExists = await context.LaundryServices.AnyAsync();
                }
                catch (Exception)
                {
                    // Table doesn't exist - we'll create it
                    try
                    {
                        await context.Database.ExecuteSqlRawAsync(@"
                            IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'LaundryServices')
                            BEGIN
                                CREATE TABLE [LaundryServices] (
                                    [Id] int NOT NULL IDENTITY,
                                    [Name] nvarchar(max) NOT NULL,
                                    [Description] nvarchar(max) NULL,
                                    [Size] int NOT NULL,
                                    [Price] decimal(18,2) NOT NULL,
                                    [IsActive] bit NOT NULL,
                                    [IsPremium] bit NOT NULL,
                                    CONSTRAINT [PK_LaundryServices] PRIMARY KEY ([Id])
                                );
                            END
                        ");
                        
                        // Table should be created now
                        tableExists = false;
                    }
                    catch (Exception ex)
                    {
                        // Log the exception
                        var logger = scope.ServiceProvider.GetRequiredService<ILogger<ApplicationDbContext>>();
                        logger.LogError(ex, "An error occurred creating the LaundryServices table");
                        return;
                    }
                }
                
                // Only seed if no services exist
                if (!tableExists)
                {
                    // Create the standard services
                    var services = new List<LaundryService>
                    {
                        // Wash Only services
                        new LaundryService { Name = "Wash Only - Small Basket", Description = "Basic wash service for small loads", Size = BasketSize.Small, Price = 80.00m },
                        new LaundryService { Name = "Wash Only - Medium Basket", Description = "Basic wash service for medium loads", Size = BasketSize.Medium, Price = 120.00m },
                        new LaundryService { Name = "Wash Only - Large Basket", Description = "Basic wash service for large loads", Size = BasketSize.Large, Price = 160.00m },
                        
                        // Wash & Dry services
                        new LaundryService { Name = "Wash & Dry - Small Basket", Description = "Wash and dry service for small loads", Size = BasketSize.Small, Price = 120.00m },
                        new LaundryService { Name = "Wash & Dry - Medium Basket", Description = "Wash and dry service for medium loads", Size = BasketSize.Medium, Price = 180.00m },
                        new LaundryService { Name = "Wash & Dry - Large Basket", Description = "Wash and dry service for large loads", Size = BasketSize.Large, Price = 240.00m },
                        
                        // Iron Only services
                        new LaundryService { Name = "Iron Only - Small Basket", Description = "Ironing service for small loads", Size = BasketSize.Small, Price = 100.00m },
                        new LaundryService { Name = "Iron Only - Medium Basket", Description = "Ironing service for medium loads", Size = BasketSize.Medium, Price = 150.00m },
                        new LaundryService { Name = "Iron Only - Large Basket", Description = "Ironing service for large loads", Size = BasketSize.Large, Price = 200.00m },
                        
                        // Wash, Dry & Iron services
                        new LaundryService { Name = "Wash, Dry & Iron - Small Basket", Description = "Complete laundry service for small loads", Size = BasketSize.Small, Price = 200.00m },
                        new LaundryService { Name = "Wash, Dry & Iron - Medium Basket", Description = "Complete laundry service for medium loads", Size = BasketSize.Medium, Price = 300.00m },
                        new LaundryService { Name = "Wash, Dry & Iron - Large Basket", Description = "Complete laundry service for large loads", Size = BasketSize.Large, Price = 400.00m },
                        
                        // Fold and packaging
                        new LaundryService { Name = "Fold and Packaging", Description = "Folding and packaging service", Size = BasketSize.Small, Price = 0.00m },
                        
                        // Premium services (blankets)
                        new LaundryService { Name = "Premium Blanket Cleaning - Single", Description = "Cleaning service for single blankets", Size = BasketSize.Small, Price = 150.00m, IsPremium = true },
                        new LaundryService { Name = "Premium Blanket Cleaning - Double", Description = "Cleaning service for double blankets", Size = BasketSize.Medium, Price = 200.00m, IsPremium = true },
                        new LaundryService { Name = "Premium Blanket Cleaning - King", Description = "Cleaning service for king size blankets", Size = BasketSize.Large, Price = 250.00m, IsPremium = true }
                    };

                    await context.LaundryServices.AddRangeAsync(services);
                    await context.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                // Log the exception
                var logger = scope.ServiceProvider.GetRequiredService<ILogger<ApplicationDbContext>>();
                logger.LogError(ex, "An error occurred seeding the LaundryServices");
            }
        }
        
        public static async Task EnsureBookingTablesExist(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var logger = scope.ServiceProvider.GetRequiredService<ILogger<ApplicationDbContext>>();
            
            try
            {
                // Ensure database is created
                await context.Database.EnsureCreatedAsync();
                
                // Check if Bookings table exists
                bool bookingsTableExists = false;
                try
                {
                    // Try to query the table
                    bookingsTableExists = await context.Bookings.AnyAsync();
                }
                catch (Exception)
                {
                    // Table doesn't exist
                    try
                    {
                        // Create Bookings table
                        await context.Database.ExecuteSqlRawAsync(@"
                            IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Bookings')
                            BEGIN
                                CREATE TABLE [Bookings] (
                                    [Id] int NOT NULL IDENTITY,
                                    [CustomerId] nvarchar(450) NOT NULL,
                                    [BookingDate] datetime2 NOT NULL,
                                    [PickupDate] datetime2 NOT NULL,
                                    [DeliveryDate] datetime2 NULL,
                                    [SpecialInstructions] nvarchar(max) NULL,
                                    [Status] int NOT NULL,
                                    [DriverId] nvarchar(450) NULL,
                                    [TotalAmount] decimal(18,2) NOT NULL,
                                    CONSTRAINT [PK_Bookings] PRIMARY KEY ([Id]),
                                    CONSTRAINT [FK_Bookings_AspNetUsers_CustomerId] FOREIGN KEY ([CustomerId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE,
                                    CONSTRAINT [FK_Bookings_AspNetUsers_DriverId] FOREIGN KEY ([DriverId]) REFERENCES [AspNetUsers] ([Id])
                                );
                                
                                CREATE INDEX [IX_Bookings_CustomerId] ON [Bookings] ([CustomerId]);
                                CREATE INDEX [IX_Bookings_DriverId] ON [Bookings] ([DriverId]);
                            END
                        ");
                        
                        logger.LogInformation("Created Bookings table");
                    }
                    catch (Exception ex)
                    {
                        logger.LogError(ex, "Error creating Bookings table");
                    }
                }
                
                // Check if BookingDetails table exists
                bool detailsTableExists = false;
                try
                {
                    // Try to query the table
                    detailsTableExists = await context.BookingDetails.AnyAsync();
                }
                catch (Exception)
                {
                    // Table doesn't exist
                    try
                    {
                        // Create BookingDetails table
                        await context.Database.ExecuteSqlRawAsync(@"
                            IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'BookingDetails')
                            BEGIN
                                CREATE TABLE [BookingDetails] (
                                    [Id] int NOT NULL IDENTITY,
                                    [BookingId] int NOT NULL,
                                    [LaundryServiceId] int NOT NULL,
                                    [Quantity] int NOT NULL,
                                    [Price] decimal(18,2) NOT NULL,
                                    [StaffId] nvarchar(450) NULL,
                                    [Status] nvarchar(max) NOT NULL,
                                    [CompletedOn] datetime2 NULL,
                                    CONSTRAINT [PK_BookingDetails] PRIMARY KEY ([Id]),
                                    CONSTRAINT [FK_BookingDetails_Bookings_BookingId] FOREIGN KEY ([BookingId]) REFERENCES [Bookings] ([Id]) ON DELETE CASCADE,
                                    CONSTRAINT [FK_BookingDetails_LaundryServices_LaundryServiceId] FOREIGN KEY ([LaundryServiceId]) REFERENCES [LaundryServices] ([Id]) ON DELETE CASCADE,
                                    CONSTRAINT [FK_BookingDetails_AspNetUsers_StaffId] FOREIGN KEY ([StaffId]) REFERENCES [AspNetUsers] ([Id])
                                );
                                
                                CREATE INDEX [IX_BookingDetails_BookingId] ON [BookingDetails] ([BookingId]);
                                CREATE INDEX [IX_BookingDetails_LaundryServiceId] ON [BookingDetails] ([LaundryServiceId]);
                                CREATE INDEX [IX_BookingDetails_StaffId] ON [BookingDetails] ([StaffId]);
                            END
                        ");
                        
                        logger.LogInformation("Created BookingDetails table");
                    }
                    catch (Exception ex)
                    {
                        logger.LogError(ex, "Error creating BookingDetails table");
                    }
                }

                // Check if BookingPreferences table exists
                bool preferencesTableExists = false;
                try
                {
                    // Try to query the table
                    preferencesTableExists = await context.BookingPreferences.AnyAsync();
                }
                catch (Exception)
                {
                    // Table doesn't exist
                    try
                    {
                        // Create BookingPreferences table
                        await context.Database.ExecuteSqlRawAsync(@"
                            IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'BookingPreferences')
                            BEGIN
                                CREATE TABLE [BookingPreferences] (
                                    [Id] int NOT NULL IDENTITY,
                                    [BookingId] int NOT NULL,
                                    [DetergentType] int NOT NULL,
                                    [LaundryBagRequired] bit NOT NULL,
                                    [TShirtsCount] int NOT NULL,
                                    [DressesCount] int NOT NULL,
                                    [TrousersCount] int NOT NULL,
                                    [BlanketsCount] int NOT NULL,
                                    [TermsAccepted] bit NOT NULL,
                                    CONSTRAINT [PK_BookingPreferences] PRIMARY KEY ([Id]),
                                    CONSTRAINT [FK_BookingPreferences_Bookings_BookingId] FOREIGN KEY ([BookingId]) REFERENCES [Bookings] ([Id]) ON DELETE CASCADE
                                );
                                
                                CREATE UNIQUE INDEX [IX_BookingPreferences_BookingId] ON [BookingPreferences] ([BookingId]);
                            END
                        ");
                        
                        logger.LogInformation("Created BookingPreferences table");
                    }
                    catch (Exception ex)
                    {
                        logger.LogError(ex, "Error creating BookingPreferences table");
                    }
                }

                // Check if Payments table exists
                bool paymentsTableExists = false;
                try
                {
                    // Try to query the table
                    paymentsTableExists = await context.Payments.AnyAsync();
                }
                catch (Exception)
                {
                    // Table doesn't exist
                    try
                    {
                        // Create Payments table
                        await context.Database.ExecuteSqlRawAsync(@"
                            IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Payments')
                            BEGIN
                                CREATE TABLE [Payments] (
                                    [Id] int NOT NULL IDENTITY,
                                    [BookingId] int NOT NULL,
                                    [PaymentDate] datetime2 NOT NULL,
                                    [Status] int NOT NULL,
                                    [Amount] decimal(18,2) NOT NULL,
                                    [CardNumber] nvarchar(max) NOT NULL,
                                    [ExpiryDate] nvarchar(max) NOT NULL,
                                    [CVV] nvarchar(max) NOT NULL,
                                    [CardHolderName] nvarchar(max) NOT NULL,
                                    [TransactionReference] nvarchar(max) NULL,
                                    CONSTRAINT [PK_Payments] PRIMARY KEY ([Id]),
                                    CONSTRAINT [FK_Payments_Bookings_BookingId] FOREIGN KEY ([BookingId]) REFERENCES [Bookings] ([Id]) ON DELETE CASCADE
                                );
                                
                                CREATE UNIQUE INDEX [IX_Payments_BookingId] ON [Payments] ([BookingId]);
                            END
                        ");
                        
                        logger.LogInformation("Created Payments table");
                    }
                    catch (Exception ex)
                    {
                        logger.LogError(ex, "Error creating Payments table");
                    }
                }

                // Check if Reviews table exists
                bool reviewsTableExists = false;
                try
                {
                    // Try to query the table
                    reviewsTableExists = await context.Reviews.AnyAsync();
                }
                catch (Exception)
                {
                    // Table doesn't exist
                    try
                    {
                        // Create Reviews table
                        await context.Database.ExecuteSqlRawAsync(@"
                            IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Reviews')
                            BEGIN
                                CREATE TABLE [Reviews] (
                                    [Id] int NOT NULL IDENTITY,
                                    [BookingId] int NOT NULL,
                                    [Rating] int NOT NULL,
                                    [Comments] nvarchar(500) NOT NULL,
                                    [ReviewDate] datetime2 NOT NULL,
                                    [CustomerId] nvarchar(450) NOT NULL,
                                    CONSTRAINT [PK_Reviews] PRIMARY KEY ([Id]),
                                    CONSTRAINT [FK_Reviews_Bookings_BookingId] FOREIGN KEY ([BookingId]) REFERENCES [Bookings] ([Id]) ON DELETE CASCADE,
                                    CONSTRAINT [FK_Reviews_AspNetUsers_CustomerId] FOREIGN KEY ([CustomerId]) REFERENCES [AspNetUsers] ([Id])
                                );
                                
                                CREATE INDEX [IX_Reviews_BookingId] ON [Reviews] ([BookingId]);
                                CREATE INDEX [IX_Reviews_CustomerId] ON [Reviews] ([CustomerId]);
                            END
                        ");
                        
                        logger.LogInformation("Created Reviews table");
                    }
                    catch (Exception ex)
                    {
                        logger.LogError(ex, "Error creating Reviews table");
                    }
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred ensuring booking tables exist");
            }
        }

        private static async Task CreateUser(UserManager<ApplicationUser> userManager, string email, string password, string role, string fullName, string idNumber, string streetAddress, string suburb, string city, string province, string postalCode, string altContact)
        {
            var user = await userManager.FindByEmailAsync(email);

            if (user == null)
            {
                user = new ApplicationUser
                {
                    UserName = email,
                    Email = email,
                    EmailConfirmed = true,
                    FullName = fullName,
                    IdNumber = idNumber,
                    StreetAddress = streetAddress,
                    Suburb = suburb,
                    City = city,
                    Province = province,
                    PostalCode = postalCode,
                    AlternativeContactNumber = altContact
                };

                var result = await userManager.CreateAsync(user, password);
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(user, role);
                }
            }
        }
    }
} 