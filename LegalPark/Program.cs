using LegalPark.Data;
using LegalPark.Helpers;
using LegalPark.Models.Entities;
using LegalPark.Repositories.Generic;
using LegalPark.Repositories.LogVerification;
using LegalPark.Repositories.Merchant;
using LegalPark.Repositories.ParkingSpot;
using LegalPark.Repositories.ParkingTransaction;
using LegalPark.Repositories.PaymentVerificationCode;
using LegalPark.Repositories.User;
using LegalPark.Repositories.Vehicle;
using LegalPark.Security.Jwt;
using LegalPark.Services.Auth;
using LegalPark.Services.Balance;
using LegalPark.Services.Merchant;
using LegalPark.Services.Notification;
using LegalPark.Services.ParkingSpot.Admin;
using LegalPark.Services.ParkingSpot.User;
using LegalPark.Services.ParkingTransaction.Admin;
using LegalPark.Services.ParkingTransaction.User;
using LegalPark.Services.Payment;
using LegalPark.Services.Report.Admin;
using LegalPark.Services.Report.User;
using LegalPark.Services.Template;
using LegalPark.Services.User;
using LegalPark.Services.Vehicle.Admin;
using LegalPark.Services.Vehicle.User;
using LegalPark.Services.VerificationCode;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// === Add Services ===
builder.Services.AddDbContext<LegalParkDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// --- Register Repositories ---
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<ILogVerificationRepository, LogVerificationRepository>();
builder.Services.AddScoped<IMerchantRepository, MerchantRepository>();
builder.Services.AddScoped<IParkingSpotRepository, ParkingSpotRepository>();
builder.Services.AddScoped<IPaymentVerificationCodeRepository, PaymentVerificationCodeRepository>();
builder.Services.AddScoped<IVehicleRepository, VehicleRepository>();
builder.Services.AddScoped<IParkingTransactionRepository, ParkingTransactionRepository>();

// --- Register Services ---
builder.Services.AddScoped<ITemplateService, TemplateService>();
builder.Services.AddScoped<INotificationService, NotificationService>();
builder.Services.AddScoped<IAuthService,  AuthService>();
builder.Services.AddScoped<IBalanceService, BalanceService>();
builder.Services.AddScoped<IMerchantService, MerchantService>();
builder.Services.AddScoped<IAdminParkingSpotService,  AdminParkingSpotService>();
builder.Services.AddScoped<IUserParkingSpotService,  UserParkingSpotService>();
builder.Services.AddScoped<IAdminParkingTransactionService, AdminParkingTransactionService>();
builder.Services.AddScoped<IUserParkingTransactionService,  UserParkingTransactionService>();
builder.Services.AddScoped<IPaymentService, PaymentService>();
builder.Services.AddScoped<IAdminReportService, AdminReportService>();
builder.Services.AddScoped<IUserReportService, UserReportService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IAdminVehicleService, AdminVehicleService>();
builder.Services.AddScoped<IUserVehicleService, UserVehicleService>();
builder.Services.AddScoped<IVerificationCodeService, VerificationCodeService>();

// Mendaftarkan SmtpSettings dari konfigurasi
builder.Services.Configure<SmtpSettings>(builder.Configuration.GetSection("SmtpSettings"));

// Mendaftarkan Helpers sebagai Singleton atau Scoped
builder.Services.AddSingleton<MailService>();
builder.Services.AddScoped<CodeGeneratorUtil>();
builder.Services.AddScoped<InfoAccount>();


// --- Pendaftaran AutoMapper dan Service Mapper Anda ---
// Tambahkan AutoMapper. AutoMapper akan mencari semua profile di assembly saat ini.
builder.Services.AddAutoMapper(typeof(Program).Assembly);

// Daftarkan ParkingSpotResponseMapper sebagai Scoped
builder.Services.AddScoped<ParkingSpotResponseMapper>();
builder.Services.AddScoped<ParkingTransactionResponseMapper>();
builder.Services.AddScoped<ReportResponseMapper>();
builder.Services.AddScoped<VehicleResponseMapper>();

// === JWT Service ===
// Perbaikan: Daftarkan IJwtService dan JwtService untuk Dependency Injection yang benar.
// JwtService masih dibutuhkan untuk MENGHASILKAN token.
builder.Services.AddScoped<IJwtService, JwtService>();

// === Identity Configuration === //UserDetailsService (open)
builder.Services.AddIdentity<User, IdentityRole<Guid>>(options =>
{
    options.Password.RequireDigit = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequiredLength = 6;
    options.User.RequireUniqueEmail = true;
})
.AddEntityFrameworkStores<LegalParkDbContext>()
.AddDefaultTokenProviders();
//UserDetailsService (close)



// === Authentication (JWT) === //WebSecurityConfig.java (open)
builder.Services.AddAuthentication(options => //AuthenticationProvider
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    var key = Encoding.UTF8.GetBytes(builder.Configuration["Jwt:SecretKey"]);
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"], 
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(key),
        // Tambahkan konfigurasi ClockSkew agar sesuai dengan JwtService
        ClockSkew = TimeSpan.Zero
    };
});

// === Authorization Policies ===
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("UserPolicy", policy => policy.RequireRole("USER", "ADMIN"));
    options.AddPolicy("AdminPolicy", policy => policy.RequireRole("ADMIN"));
});
//WebSecurityConfig.java (close)



// === CORS ===
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowLocalhost3000", policy =>
    {
        policy.WithOrigins("http://localhost:3000")
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials();
    });
});

// === Controller & Swagger ===
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    // Opsi untuk mendefinisikan skema keamanan JWT. Ini akan menampilkan "Authorize" button di Swagger UI.
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "Masukkan 'Bearer' diikuti dengan spasi dan JWT token. Contoh: 'Bearer asdfghjkl12345'",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            //new string[] {}
            Array.Empty<string>()
        }
    });
});

var app = builder.Build();

// === Middleware Pipeline ===
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("AllowLocalhost3000");

app.UseAuthentication(); // <== WAJIB sebelum Authorization
app.UseAuthorization();

// --- Inisialisasi Peran & Admin Pertama ---
using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole<Guid>>>();
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();

    // Seeding Peran
    await SeedRolesAsync(roleManager);

    // Seeding Admin Pertama
    await SeedAdminUserAsync(userManager, roleManager);
}

//app.UseMiddleware<JwtAuthenticationMiddleware>();

app.MapControllers();

app.Run();

// --- Metode untuk seeding Peran & Admin ---
async Task SeedRolesAsync(RoleManager<IdentityRole<Guid>> roleManager)
{
    if (!await roleManager.RoleExistsAsync("Admin"))
    {
        await roleManager.CreateAsync(new IdentityRole<Guid>("Admin"));
    }
    if (!await roleManager.RoleExistsAsync("User"))
    {
        await roleManager.CreateAsync(new IdentityRole<Guid>("User"));
    }
}

async Task SeedAdminUserAsync(UserManager<User> userManager, RoleManager<IdentityRole<Guid>> roleManager)
{
    var adminUser = await userManager.FindByEmailAsync("admin@legalpark.com");
    if (adminUser == null)
    {
        adminUser = new User
        {
            UserName = "adminlegalpark",
            Email = "admin@legalpark.com",
            AccountName = "Admin LegalPark",
            AccountStatus = AccountStatus.ACTIVE,
            Balance = 0m
        };

        await userManager.CreateAsync(adminUser, "P@ssword123"); // Ganti kata sandi ini di produksi!
        await userManager.AddToRoleAsync(adminUser, "Admin");
    }
}