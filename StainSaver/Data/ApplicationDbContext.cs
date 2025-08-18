using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using StainSaver.Models;

namespace StainSaver.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<RefundPolicyEntry>RefundPolicyEntries { get; set; }
        public DbSet<RefundValidationEntry> RefundValidationEntries { get; set; }
        public DbSet<Refund> Refunds { get; set; }

        public DbSet<Package> Packages { get; set; }

        public DbSet<Delivery> Delivery { get; set; }

        public DbSet<Feedback>Feedbacks { get; set; }

        public DbSet<DeliveryItem> DeliveryItems { get; set; }

        public DbSet<RefundItem> RefundItems { get; set; }
        public DbSet<LostOrFoundItem> LostOrFoundItems { get; set; }
        public DbSet<Delivery> Deliveries { get; set; }
        public DbSet<PickUp> PickUps { get; set; }

        public DbSet<Complain> Complains { get; set; }

        public DbSet<LaundryService> LaundryServices { get; set; }
        public DbSet<Booking> Bookings { get; set; }
        public DbSet<BookingDetail> BookingDetails { get; set; }
        public DbSet<BookingPreferences> BookingPreferences { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<CustomerNotification> CustomerNotifications { get; set; }
        public DbSet<AdminNotification> AdminNotifications { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure Booking.DriverId foreign key with NO ACTION for delete
            modelBuilder.Entity<Booking>()
                .HasOne(b => b.Driver)
                .WithMany()
                .HasForeignKey(b => b.DriverId)
                .OnDelete(DeleteBehavior.NoAction);

            // Configure BookingDetail.StaffId foreign key with NO ACTION for delete
            modelBuilder.Entity<BookingDetail>()
                .HasOne(bd => bd.Staff)
                .WithMany()
                .HasForeignKey(bd => bd.StaffId)
                .OnDelete(DeleteBehavior.NoAction);

            // Configure decimal precision
            modelBuilder.Entity<LaundryService>()
                .Property(ls => ls.Price)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<Booking>()
                .Property(b => b.TotalAmount)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<BookingDetail>()
                .Property(bd => bd.Price)
                .HasColumnType("decimal(18,2)");
                
            // Configure one-to-one relationship between Booking and BookingPreferences
            modelBuilder.Entity<BookingPreferences>()
                .HasOne(bp => bp.Booking)
                .WithOne()
                .HasForeignKey<BookingPreferences>(bp => bp.BookingId);
                
            // Configure one-to-one relationship between Booking and Payment
            modelBuilder.Entity<Payment>()
                .HasOne(p => p.Booking)
                .WithOne()
                .HasForeignKey<Payment>(p => p.BookingId);
                
            // Configure one-to-many relationship between Booking and Reviews
            modelBuilder.Entity<Review>()
                .HasOne(r => r.Booking)
                .WithMany()
                .HasForeignKey(r => r.BookingId);
                
            // Configure Booking Customer foreign key relationship
            modelBuilder.Entity<Review>()
                .HasOne(r => r.Customer)
                .WithMany()
                .HasForeignKey(r => r.CustomerId)
                .OnDelete(DeleteBehavior.NoAction);
                
            // Configure CustomerNotification relationships    
            modelBuilder.Entity<CustomerNotification>()
                .HasOne(cn => cn.Customer)
                .WithMany()
                .HasForeignKey(cn => cn.CustomerId)
                .OnDelete(DeleteBehavior.Cascade);
                
            modelBuilder.Entity<CustomerNotification>()
                .HasOne(cn => cn.Booking)
                .WithMany()
                .HasForeignKey(cn => cn.BookingId)
                .OnDelete(DeleteBehavior.NoAction);
                
            // Configure AdminNotification relationships
            modelBuilder.Entity<AdminNotification>()
                .HasOne(an => an.Booking)
                .WithMany()
                .HasForeignKey(an => an.BookingId)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
