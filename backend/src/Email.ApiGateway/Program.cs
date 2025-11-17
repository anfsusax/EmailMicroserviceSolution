using System.Text;
using Email.ApiGateway.Configuration;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration
    .AddJsonFile("ocelot.json", optional: false, reloadOnChange: true)
    .AddEnvironmentVariables();

builder.Host.UseSerilog((context, services, config) =>
    config.ReadFrom.Configuration(context.Configuration)
          .ReadFrom.Services(services)
          .Enrich.FromLogContext()
          .WriteTo.Console());

builder.Services.Configure<JwtOptions>(builder.Configuration.GetSection(JwtOptions.SectionName));

var jwtOptions = builder.Configuration.GetSection(JwtOptions.SectionName).Get<JwtOptions>() ?? new();
var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.Secret.PadRight(32, '0')));

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtOptions.Issuer,
            ValidAudience = jwtOptions.Audience,
            IssuerSigningKey = signingKey
        };
    });

builder.Services.AddAuthorization();
builder.Services.AddOcelot(builder.Configuration);

var app = builder.Build();

// Middleware de autenticação e autorização
app.UseAuthentication();
app.UseAuthorization();

// Middleware para rotas diretas (antes do Ocelot)
app.Use(async (context, next) =>
{
    var path = context.Request.Path.Value;
    
    if (path == "/")
    {
        context.Response.ContentType = "text/plain; charset=utf-8";
        await context.Response.WriteAsync("Email API Gateway em execução.");
        return;
    }
    
    if (path == "/health")
    {
        context.Response.ContentType = "application/json";
        await context.Response.WriteAsync("""{"status":"healthy","service":"Email.ApiGateway"}""");
        return;
    }
    
    await next();
});

// Ocelot processa apenas rotas que não foram tratadas pelo middleware acima
await app.UseOcelot();
await app.RunAsync();
