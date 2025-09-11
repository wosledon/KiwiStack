using System.Diagnostics;
using KiwiStack.Api.Data;
using KiwiStack.Api.Services.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Proxies; // added - requires Microsoft.EntityFrameworkCore.Proxies NuGet package
using Scalar.AspNetCore;
using Soda.AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

builder.Services.AddControllers();

    ConfigureServices(builder.Services);
    // JWT
    var key = builder.Configuration["Jwt:Key"] ?? "replace-this-secret-in-production";
    var issuer = builder.Configuration["Jwt:Issuer"] ?? "kiwistack";

    builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    }).AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = false,
            ValidateIssuerSigningKey = true,
            ValidIssuer = issuer,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key))
        };
    });

    builder.Services.AddScoped<KiwiStack.Api.Services.Auth.ITokenService, KiwiStack.Api.Services.Auth.TokenService>();
builder.Services.AddDbContext<KiwiDbContext>(opts =>
{
    opts.UseSqlite("Data Source=kiwiStack.db");

    opts.AddInterceptors(
        new SqlTraceInterceptor(
            (msg) => Debug.WriteLine(msg)
            , 200));

    opts.UseLazyLoadingProxies();
});


var app = builder.Build();

SodaMapperExtension.InitSodaMapper(app);

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.Run();


void ConfigureServices(IServiceCollection services)
{
    var scopedServiceTypes = AppDomain.CurrentDomain.GetAssemblies();

    foreach (var type in scopedServiceTypes.SelectMany(a => a.GetTypes())
                 .Where(t => t.IsClass && !t.IsAbstract && typeof(KiwiStack.Shared.Contracts.IScopedService).IsAssignableFrom(t)))
    {
        services.AddScoped(type);
    }

    foreach (var type in scopedServiceTypes.SelectMany(a => a.GetTypes())
                 .Where(t => t.IsClass && !t.IsAbstract && typeof(KiwiStack.Shared.Contracts.ISingletonService).IsAssignableFrom(t)))
    {
        services.AddSingleton(type);
    }
}
