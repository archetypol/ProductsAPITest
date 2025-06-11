using System.Text;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using ProductsApi.Auth.Models;
using ProductsApi.Auth.Service;
using ProductsApi.Data;
using ProductsApi.Products.Service;
using ProductsApi.Products.Repository;
using ProductsApi.Auth.Repository;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;

// DB Init
builder.Services.AddOpenApi();
builder.Services.AddDbContext<AppDbContext>(opt =>
    opt.UseSqlServer(configuration.GetConnectionString("DefaultConnection"))
);

// Identity models
builder
    .Services.AddIdentity<ProductsApiUser, IdentityRole>(options =>
    {
        options.Password.RequiredLength = 6;
        options.User.RequireUniqueEmail = true;
    })
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();

// JWT init
var jwtSettingsSection = configuration.GetSection("JwtSettings");
var jwtSettings = jwtSettingsSection.Get<JwtFields>();
builder.Services.Configure<JwtFields>(jwtSettingsSection);

builder
    .Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.RequireHttpsMetadata = !builder.Environment.IsDevelopment();
        options.SaveToken = true;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,

            ValidIssuer = jwtSettings!.Issuer,
            ValidAudience = jwtSettings.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Key)),

            ClockSkew = TimeSpan.Zero,
        };
    });

// REgister the auth and Products Service
builder.Services.AddScoped<UserAuthService>();
builder.Services.AddScoped<ProductsService>();
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();

// Admin Policy
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy => policy.RequireClaim("IsAdmin", "true"));
});

builder.Services.AddControllers();

// Health checks
builder.Services.AddHealthChecks()
    .AddDbContextCheck<AppDbContext>(
        failureStatus: HealthStatus.Unhealthy,
        tags: ["db", "ef"]
    );

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.MapHealthChecks("/health");

app.Run();
