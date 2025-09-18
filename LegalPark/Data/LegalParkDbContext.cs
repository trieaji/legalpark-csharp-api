using LegalPark.Models.Entities;
using Microsoft.AspNetCore.Identity; // Untuk IdentityUser
using Microsoft.AspNetCore.Identity.EntityFrameworkCore; // Untuk IdentityDbContext
using Microsoft.EntityFrameworkCore;

namespace LegalPark.Data
{
    // Mewarisi dari IdentityDbContext<User, IdentityRole<Guid>, Guid>
    // User: Kelas entitas pengguna kustom Anda
    // IdentityRole<Guid>: Kelas role bawaan Identity dengan Guid sebagai PK
    // Guid: Tipe data untuk Primary Key dari User dan Role
    public class LegalParkDbContext : IdentityDbContext<User, IdentityRole<Guid>, Guid>
    {
        public LegalParkDbContext(DbContextOptions<LegalParkDbContext> options) : base(options)
        {
        }

        // --- DbSet untuk Entitas Anda ---
        public DbSet<Merchant> Merchants { get; set; }
        // DbSet untuk User sudah ditangani oleh IdentityDbContext<User, ...>
        public DbSet<Vehicle> Vehicles { get; set; }
        public DbSet<ParkingSpot> ParkingSpots { get; set; }
        public DbSet<PaymentVerificationCode> PaymentVerificationCodes { get; set; }
        public DbSet<ParkingTransaction> ParkingTransactions { get; set; }
        public DbSet<LogVerification> LogVerifications { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // PENTING: Selalu panggil base.OnModelCreating(modelBuilder) terlebih dahulu
            // saat menggunakan IdentityDbContext. Ini mengkonfigurasi tabel Identity.
            base.OnModelCreating(modelBuilder);

            // --- Konfigurasi Indeks Unik ---

            // MerchantCode harus unik
            modelBuilder.Entity<Merchant>()
                .HasIndex(m => m.MerchantCode)
                .IsUnique();

            // LicensePlate harus unik
            modelBuilder.Entity<Vehicle>()
                .HasIndex(v => v.LicensePlate)
                .IsUnique();

            // Code di LogVerification harus unik
            modelBuilder.Entity<LogVerification>()
                .HasIndex(lv => lv.Code)
                .IsUnique();

            // Email di User sudah unik secara default oleh IdentityUser
            // AccountName di User juga bisa dibuat unik jika diinginkan,
            // tetapi UserName dari IdentityUser biasanya sudah unik.
            // Jika AccountName Anda ingin unik terpisah dari UserName, tambahkan:
            // modelBuilder.Entity<User>()
            //     .HasIndex(u => u.AccountName)
            //     .IsUnique();


            // --- Konfigurasi Tipe Data Presisi (Decimal) ---

            // Balance di User
            modelBuilder.Entity<User>()
                .Property(u => u.Balance)
                .HasColumnType("decimal(10,2)"); // Presisi 10, skala 2

            // TotalCost di ParkingTransaction
            modelBuilder.Entity<ParkingTransaction>()
                .Property(pt => pt.TotalCost)
                .HasColumnType("decimal(10,2)"); // Presisi 10, skala 2


            // --- Konfigurasi Timestamp Default (CreatedAt) ---
            // Kita akan mengatur default value untuk CreatedAt menggunakan fungsi SQL GETDATE()
            // Ini akan otomatis mengisi kolom CreatedAt saat baris baru ditambahkan.
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

            // --- Konfigurasi Relasi (jika tidak terdeteksi otomatis oleh konvensi) ---

            // Relasi One-to-One antara PaymentVerificationCode dan ParkingTransaction
            // PaymentVerificationCode memiliki satu ParkingTransaction
            // ParkingTransaction memiliki satu PaymentVerificationCode (opsional, karena ParkingTransactionId di PVC nullable)
            modelBuilder.Entity<PaymentVerificationCode>()
                .HasOne(pvc => pvc.ParkingTransaction) // PVC memiliki satu ParkingTransaction
                .WithOne() // ParkingTransaction tidak memiliki properti navigasi kembali ke PVC secara eksplisit di model ini
                .HasForeignKey<PaymentVerificationCode>(pvc => pvc.ParkingTransactionId) // Kunci asing di PVC
                .IsRequired(false); // Relasi ini opsional dari sisi PVC, karena ParkingTransactionId bisa null

            // Konfigurasi relasi Many-to-One (seperti Vehicle -> User, ParkingSpot -> Merchant)
            // biasanya sudah terdeteksi otomatis oleh EF Core jika properti navigasi
            // dan foreign key sudah ada di entitas.
            // Contoh eksplisit (biasanya tidak perlu jika konvensi diikuti):
            // modelBuilder.Entity<Vehicle>()
            //     .HasOne(v => v.Owner)
            //     .WithMany(u => u.Vehicles) // Ini memerlukan properti ICollection<Vehicle> di User
            //     .HasForeignKey(v => v.OwnerId);

            // Menentukan nama tabel untuk Identity (opsional, jika Anda ingin mengubah nama default)
            // Ini akan mengubah nama tabel Identity dari AspNetUsers, AspNetRoles, dll.
            // menjadi nama yang lebih sesuai jika Anda tidak ingin prefix "AspNet".
            // Namun, untuk konsistensi dengan IdentityDbContext, seringkali dibiarkan default.
            // Jika Anda ingin mengubahnya, ini contohnya:
            // modelBuilder.Entity<User>().ToTable("Users");
            // modelBuilder.Entity<IdentityRole<Guid>>().ToTable("Roles");
            // modelBuilder.Entity<IdentityUserRole<Guid>>().ToTable("UserRoles");
            // modelBuilder.Entity<IdentityUserClaim<Guid>>().ToTable("UserClaims");
            // modelBuilder.Entity<IdentityUserLogin<Guid>>().ToTable("UserLogins");
            // modelBuilder.Entity<IdentityRoleClaim<Guid>>().ToTable("RoleClaims");
            // modelBuilder.Entity<IdentityUserToken<Guid>>().ToTable("UserTokens");
        }
    }
}
