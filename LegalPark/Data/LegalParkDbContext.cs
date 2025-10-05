using LegalPark.Models.Entities;
using Microsoft.AspNetCore.Identity; 
using Microsoft.AspNetCore.Identity.EntityFrameworkCore; 
using Microsoft.EntityFrameworkCore;

namespace LegalPark.Data
{
    
    public class LegalParkDbContext : IdentityDbContext<User, IdentityRole<Guid>, Guid>
    {
        public LegalParkDbContext(DbContextOptions<LegalParkDbContext> options) : base(options)
        {
        }

        
        public DbSet<Merchant> Merchants { get; set; }
        
        public DbSet<Vehicle> Vehicles { get; set; }
        public DbSet<ParkingSpot> ParkingSpots { get; set; }
        public DbSet<PaymentVerificationCode> PaymentVerificationCodes { get; set; }
        public DbSet<ParkingTransaction> ParkingTransactions { get; set; }
        public DbSet<LogVerification> LogVerifications { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            
            base.OnModelCreating(modelBuilder);
            

            
            modelBuilder.Entity<Merchant>()
                .HasIndex(m => m.MerchantCode)
                .IsUnique();

            
            modelBuilder.Entity<Vehicle>()
                .HasIndex(v => v.LicensePlate)
                .IsUnique();

            
            modelBuilder.Entity<LogVerification>()
                .HasIndex(lv => lv.Code)
                .IsUnique();

            
            modelBuilder.Entity<User>()
                .Property(u => u.Balance)
                .HasColumnType("decimal(10,2)"); 

            
            modelBuilder.Entity<ParkingTransaction>()
                .Property(pt => pt.TotalCost)
                .HasColumnType("decimal(10,2)"); 


            
            modelBuilder.Entity<Merchant>()
                .Property(m => m.CreatedAt)
                .HasDefaultValueSql("GETDATE()");

            modelBuilder.Entity<User>()
                .Property(u => u.CreatedAt)
                .HasDefaultValueSql("GETDATE()");

            modelBuilder.Entity<Vehicle>()
                .Property(v => v.CreatedAt)
                .HasDefaultValueSql("GETDATE()");

            modelBuilder.Entity<ParkingSpot>()
                .Property(ps => ps.CreatedAt)
                .HasDefaultValueSql("GETDATE()");

            modelBuilder.Entity<PaymentVerificationCode>()
                .Property(pvc => pvc.CreatedAt)
                .HasDefaultValueSql("GETDATE()");

            modelBuilder.Entity<ParkingTransaction>()
                .Property(pt => pt.CreatedAt)
                .HasDefaultValueSql("GETDATE()");

            modelBuilder.Entity<LogVerification>()
                .Property(lv => lv.CreatedAt)
                .HasDefaultValueSql("GETDATE()");

            
            modelBuilder.Entity<PaymentVerificationCode>()
                .HasOne(pvc => pvc.ParkingTransaction) 
                .WithOne() 
                .HasForeignKey<PaymentVerificationCode>(pvc => pvc.ParkingTransactionId) 
                .IsRequired(false); 

            
        }
    }
}
