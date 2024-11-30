using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Localization;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using NoventiqApplication;
using NoventiqApplication.Interface;
using NoventiqApplication.Repositories;
using NoventiqApplication.Services;
using System.Globalization;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Database connection string setup
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? "Data Source=app.sqlite";
var setupScriptPath = "Setup.sql";

// Test the SQLite connection
try
{
    using var connection = new SqliteConnection(connectionString);
    connection.Open();
    Console.WriteLine("SQLite connection successful.");
}
catch (Exception ex)
{
    Console.WriteLine($"SQLite connection failed: {ex.Message}");
    throw; // Re-throw exception to halt execution if the database connection is not working
}

// Check if the database file exists and initialize schema if required
if (!File.Exists("app.sqlite"))
{
    try
    {
        using var connection = new SqliteConnection(connectionString);
        connection.Open();

        if (File.Exists(setupScriptPath))
        {
            var setupScript = File.ReadAllText(setupScriptPath);
            using var command = new SqliteCommand(setupScript, connection);
            command.ExecuteNonQuery();
            Console.WriteLine("Database initialized with schema from Setup.sql.");
        }
        else
        {
            Console.WriteLine("Setup.sql file not found. Ensure it exists in the root directory.");
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error initializing database: {ex.Message}");
        throw; // Re-throw exception to prevent the app from running with a broken database
    }
}
else
{
    Console.WriteLine("Database already exists. Skipping setup.");
}

// Register services
builder.Services.AddAutoMapper(typeof(MappingProfile));
builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(connectionString));
builder.Services.AddSingleton<DapperRepository>(sp =>
    new DapperRepository(connectionString));
builder.Services.AddSingleton<PasswordService>();
builder.Services.AddSingleton<TokenService>(sp =>
    new TokenService(builder.Configuration["JwtSettings:SecretKey"]));
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IRoleRepository, RoleRepository>();
builder.Services.AddScoped<IRoleService, RoleService>();
builder.Services.AddSingleton<IPasswordService, PasswordService>();
builder.Services.AddScoped(typeof(IUnitOfWork), typeof(UnitOfWork));
builder.Services.AddSingleton<ITokenService>(sp =>
    new TokenService(builder.Configuration["JwtSettings:SecretKey"]));
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.AddScoped<ILocalizationService, LocalizationService>();
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(builder.Configuration["JwtSettings:SecretKey"])),
        ValidateIssuer = false,
        ValidateAudience = false,
        ClockSkew = TimeSpan.Zero
    };
});

builder.Services.AddControllers();
var supportedCultures = new[] { "en", "hi" };
builder.Services.Configure<RequestLocalizationOptions>(options =>
{
    options.DefaultRequestCulture = new RequestCulture("en"); // Default to English
    options.SupportedCultures = supportedCultures.Select(c => new CultureInfo(c)).ToList();
    options.SupportedUICultures = supportedCultures.Select(c => new CultureInfo(c)).ToList();

    // Add providers for culture detection
    options.RequestCultureProviders.Insert(0, new QueryStringRequestCultureProvider()); // Query string takes precedence
    options.RequestCultureProviders.Insert(1, new AcceptLanguageHeaderRequestCultureProvider()); // Header-based
});
var app = builder.Build();

// Ensure EF Core initializes the database schema and applies migrations
using (var scope = app.Services.CreateScope())
{
    try
    {
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        // Apply migrations if any
        dbContext.Database.Migrate();
        Console.WriteLine("Database schema ensured and migrations applied.");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error applying migrations: {ex.Message}");
        throw; // Prevent the app from running with incomplete migrations
    }
}


app.UseSwagger();
app.UseSwaggerUI();
// Middleware configuration
app.UseRouting();
app.UseRequestLocalization();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
