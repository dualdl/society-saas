using System.Text;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using SocietySaaS.Application.Common.Interfaces;
using SocietySaaS.Domain.Entities;
using SocietySaaS.Infrastructure.Persistence;
using SocietySaaS.Infrastructure.Services;

var builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .CreateLogger();

builder.Host.UseSerilog();

builder.Services.AddControllers().AddJsonOptions(o =>
{
    o.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
});
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
if (builder.Environment.IsProduction())
{
    connectionString = builder.Configuration["AZURE_SQL_CONNECTIONSTRING"];
}

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddScoped<JwtTokenService>();
builder.Services.AddScoped<IApplicationDbContext>(provider => provider.GetRequiredService<ApplicationDbContext>());
builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();
builder.Services.AddHttpContextAccessor();

var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var secretKey = jwtSettings["SecretKey"] ?? "YourSuperSecretKeyHereMustBe32Characters!!";

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSettings["Issuer"],
            ValidAudience = jwtSettings["Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
            ClockSkew = TimeSpan.Zero
        };
    });

builder.Services.AddAuthorization();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp", policy =>
    {
        policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseCors("AllowReactApp");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    await db.Database.EnsureCreatedAsync();

    // Seed super admin
    if (!await db.Users.AnyAsync(u => u.IsSuperAdmin))
    {
        var superAdmin = new User
        {
            Id = Guid.NewGuid(),
            Email = "superadmin@societypro.com",
            FirstName = "Super",
            LastName = "Admin",
            PasswordHash = JwtTokenService.HashPassword("SuperAdmin@123"),
            IsActive = true,
            IsSuperAdmin = true,
            CreatedAt = DateTime.UtcNow
        };
        db.Users.Add(superAdmin);

        // Seed demo society with data
        var demoTenant = new Tenant
        {
            Id = Guid.NewGuid(),
            Name = "Sunshine Residency",
            Address = "123 MG Road",
            City = "Mumbai",
            State = "Maharashtra",
            PinCode = "400001",
            Phone = "9876543210",
            Email = "admin@sunshineresidency.com",
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };
        db.Tenants.Add(demoTenant);

        var societyAdmin = new User
        {
            Id = Guid.NewGuid(),
            Email = "admin@sunshineresidency.com",
            FirstName = "Rajesh",
            LastName = "Kumar",
            PasswordHash = JwtTokenService.HashPassword("Admin@123"),
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };
        db.Users.Add(societyAdmin);

        db.UserTenants.Add(new UserTenant
        {
            Id = Guid.NewGuid(),
            UserId = societyAdmin.Id,
            TenantId = demoTenant.Id,
            Role = "Admin",
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        });

        // Default charges
        var charges = new[]
        {
            new Charge { Id = Guid.NewGuid(), TenantId = demoTenant.Id, Name = "Maintenance", CalculationType = "Fixed", Amount = 3000, IsRecurring = true, IsActive = true, CreatedAt = DateTime.UtcNow },
            new Charge { Id = Guid.NewGuid(), TenantId = demoTenant.Id, Name = "Sinking Fund", CalculationType = "Fixed", Amount = 500, IsRecurring = true, IsActive = true, CreatedAt = DateTime.UtcNow },
            new Charge { Id = Guid.NewGuid(), TenantId = demoTenant.Id, Name = "Water Charges", CalculationType = "Fixed", Amount = 400, IsRecurring = true, IsActive = true, CreatedAt = DateTime.UtcNow },
            new Charge { Id = Guid.NewGuid(), TenantId = demoTenant.Id, Name = "Parking", CalculationType = "Fixed", Amount = 1000, IsRecurring = true, IsActive = true, CreatedAt = DateTime.UtcNow },
            new Charge { Id = Guid.NewGuid(), TenantId = demoTenant.Id, Name = "Electricity Common Area", CalculationType = "Fixed", Amount = 600, IsRecurring = true, IsActive = true, CreatedAt = DateTime.UtcNow },
        };
        db.Charges.AddRange(charges);

        // Wings
        var wingA = new Wing { Id = Guid.NewGuid(), TenantId = demoTenant.Id, Name = "Wing A", TotalFloors = 10, FlatsPerFloor = 4, IsActive = true, CreatedAt = DateTime.UtcNow };
        var wingB = new Wing { Id = Guid.NewGuid(), TenantId = demoTenant.Id, Name = "Wing B", TotalFloors = 10, FlatsPerFloor = 4, IsActive = true, CreatedAt = DateTime.UtcNow };
        db.Wings.AddRange(wingA, wingB);

        // Flats with members
        var flatNumbers = new[] { "101", "102", "103", "104", "201", "202", "203", "204", "301", "302", "303", "304", "401", "402", "403", "404", "501", "502", "503", "504" };
        var names = new[] { "Amit Sharma", "Priya Patel", "Vikram Singh", "Neha Gupta", "Rahul Verma", "Anjali Desai", "Sanjay Mehta", "Pooja Reddy", "Arun Nair", "Deepa Iyer", "Suresh Pillai", "Kavita Joshi", "Manoj Tiwari", "Sunita Rao", "Vivek Choudhary", "Meena Bhat", "Ravi Shankar", "Lakshmi Menon", "Kiran Bhatt", "Geeta Pandey" };
        var mobiles = new[] { "9876543210", "9876543211", "9876543212", "9876543213", "9876543214", "9876543215", "9876543216", "9876543217", "9876543218", "9876543219", "9876543220", "9876543221", "9876543222", "9876543223", "9876543224", "9876543225", "9876543226", "9876543227", "9876543228", "9876543229" };

        for (int i = 0; i < flatNumbers.Length; i++)
        {
            var wing = i < 10 ? wingA : wingB;
            var floor = int.Parse(flatNumbers[i][0].ToString());
            var flatId = Guid.NewGuid();

            var flat = new Flat
            {
                Id = flatId,
                TenantId = demoTenant.Id,
                FlatNumber = flatNumbers[i],
                Floor = floor,
                CarpetArea = 800 + (i % 5) * 100,
                BuiltUpArea = 1000 + (i % 5) * 120,
                FlatType = i % 3 == 0 ? "2BHK" : "3BHK",
                OccupancyStatus = "Owner",
                WingId = wing.Id,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };
            db.Flats.Add(flat);

            db.Members.Add(new Member
            {
                Id = Guid.NewGuid(),
                TenantId = demoTenant.Id,
                FlatId = flatId,
                FirstName = names[i].Split(' ')[0],
                LastName = names[i].Split(' ')[1],
                Mobile = mobiles[i],
                Email = $"{names[i].Split(' ')[0].ToLower()}@email.com",
                MemberType = "Owner",
                IsPrimary = true,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            });

            // Generate bills for last 3 months
            for (int m = 0; m < 3; m++)
            {
                var billingPeriod = DateTime.UtcNow.AddMonths(-m - 1).ToString("yyyy-MM");
                var billId = Guid.NewGuid();
                var billAmount = charges.Sum(c => c.Amount);

                db.Bills.Add(new Bill
                {
                    Id = billId,
                    TenantId = demoTenant.Id,
                    FlatId = flatId,
                    BillNumber = $"BILL-{billingPeriod}-{flatNumbers[i]}",
                    BillingPeriod = billingPeriod,
                    BillDate = new DateTime(int.Parse(billingPeriod.Split('-')[0]), int.Parse(billingPeriod.Split('-')[1]), 1),
                    DueDate = new DateTime(int.Parse(billingPeriod.Split('-')[0]), int.Parse(billingPeriod.Split('-')[1]), 15).AddMonths(1),
                    PreviousOutstanding = 0,
                    CurrentCharges = billAmount,
                    GrandTotal = billAmount,
                    BalanceOutstanding = m == 0 ? billAmount : billAmount * 0.3m,
                    AmountPaid = m == 0 ? 0 : billAmount * 0.7m,
                    Status = m == 0 ? "Pending" : "Partial",
                    CreatedAt = DateTime.UtcNow
                });
            }
        }

        await db.SaveChangesAsync();
        Log.Information("Database seeded successfully. SuperAdmin: superadmin@societypro.com / SuperAdmin@123");
    }
}

app.Run();
