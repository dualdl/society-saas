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

Log.Information("Environment: {Env}", builder.Environment.EnvironmentName);
Log.Information("Connection string source: {Source}", builder.Environment.IsProduction() ? "AZURE_SQL_CONNECTIONSTRING" : "DefaultConnection");

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
else
{
    app.UseExceptionHandler(error =>
    {
        error.Run(async context =>
        {
            var exception = context.Features.Get<Microsoft.AspNetCore.Diagnostics.IExceptionHandlerFeature>()?.Error;
            Log.Error(exception, "Unhandled exception");
            context.Response.StatusCode = 500;
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsync(new { message = "Internal server error", detail = exception?.Message }?.ToString() ?? "{}");
        });
    });
}

app.UseSwagger();
app.UseSwaggerUI();

app.UseStaticFiles();
app.UseRouting();
app.UseCors("AllowReactApp");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

try
{
    using (var scope = app.Services.CreateScope())
    {
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        var hasMigrationsHistory = await db.Database.SqlQueryRaw<int>(
            "SELECT COUNT(*) FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = '__EFMigrationsHistory'").ToListAsync();

        if (hasMigrationsHistory.FirstOrDefault() == 0)
        {
            Log.Information("No migration history found. Recreating database...");
            await db.Database.EnsureDeletedAsync();
            await db.Database.MigrateAsync();
            Log.Information("Database recreated and migrated.");
        }
        else
        {
            Log.Information("Applying pending migrations...");
            await db.Database.MigrateAsync();
            Log.Information("Migrations applied successfully.");
        }

        if (!await db.Users.AnyAsync(u => u.IsSuperAdmin))
        {
            Log.Information("Seeding database...");
            await SeedData.SeedAsync(db);
            Log.Information("Database seeded successfully.");
        }
        else
        {
            Log.Information("Database already seeded.");
        }
    }
}
catch (Exception ex)
{
    Log.Error(ex, "Failed to apply migrations or seed database");
}

Log.Information("Starting API...");
app.Run();
