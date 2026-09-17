using System.Text;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using SocietySaaS.Application.Common.Interfaces;
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

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString, sql =>
    {
        sql.CommandTimeout(120);
        sql.EnableRetryOnFailure(3);
    }));

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
            await context.Response.WriteAsync(System.Text.Json.JsonSerializer.Serialize(new
            {
                message = "Internal server error",
                detail = exception?.Message
            }));
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

app.MapPost("/api/v1/admin/seed", async (ApplicationDbContext db) =>
{
    try
    {
        Log.Information("Seed endpoint called. Deleting and recreating DB...");
        await db.Database.EnsureDeletedAsync();
        Log.Information("Database deleted.");

        Log.Information("Creating database...");
        await db.Database.EnsureCreatedAsync();
        Log.Information("Database created.");

        if (!await db.Users.AnyAsync(u => u.IsSuperAdmin))
        {
            Log.Information("Seeding data...");
            await SeedData.SeedAsync(db);
            Log.Information("Data seeded successfully.");
            return Results.Ok(new { message = "Database created and seeded successfully" });
        }
        return Results.Ok(new { message = "Database already seeded" });
    }
    catch (Exception ex)
    {
        Log.Error(ex, "Seed failed");
        return Results.BadRequest(new { message = ex.Message, innerException = ex.InnerException?.Message });
    }
});

try
{
    using (var scope = app.Services.CreateScope())
    {
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        Log.Information("Checking database connectivity...");

        var canConnect = await db.Database.CanConnectAsync();
        if (!canConnect)
        {
            Log.Error("Cannot connect to database.");
        }
        else
        {
            Log.Information("Database connection OK.");
        }
    }
}
catch (Exception ex)
{
    Log.Error(ex, "Database connectivity check failed");
}

Log.Information("Starting API...");
app.Run();
