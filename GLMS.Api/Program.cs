/*Author: Zenwyn Naicker*/
/*Code Attribution: */
/**************************************************************************************************************
 * Author: DenzylSGovender
 * Date: 2026
 * Title: PROG7311-PART3_Support
 * Type: [Source Code]
 * Available at: <https://github.com/DenzylSGovender/PROG7311-PART3_Support.git> [Accessed 18 May 2026]
 **************************************************************************************************************/
/**************************************************************************************************************
 * Author: Microsoft
 * Date: 2026
 * Title: Create web APIs with ASP.NET Core
 * Type: [Source Code]
 * Available at: <https://learn.microsoft.com/en-us/aspnet/core/web-api/?view=aspnetcore-10.0> [Accessed 18 May 2026]
 **************************************************************************************************************/
/**************************************************************************************************************
 * Author: Microsoft
 * Date: 2026
 * Title: Configuration in ASP.NET Core
 * Type: [Source Code]
 * Available at: <https://learn.microsoft.com/en-us/aspnet/core/fundamentals/configuration/?view=aspnetcore-10.0> [Accessed 6 April 2026]
 **************************************************************************************************************/
/**************************************************************************************************************
 * Author: Microsoft
 * Date: 2026
 * Title: Dependency injection in ASP.NET Core
 * Type: [Source Code]
 * Available at: <https://learn.microsoft.com/en-us/aspnet/core/fundamentals/dependency-injection?view=aspnetcore-10.0> [Accessed 6 April 2026]
 **************************************************************************************************************/
/**************************************************************************************************************
 * Author: Microsoft
 * Date: 2025
 * Title: Configure JWT bearer authentication in ASP.NET Core
 * Type: [Source Code]
 * Available at: <https://learn.microsoft.com/en-us/aspnet/core/security/authentication/configure-jwt-bearer-authentication?view=aspnetcore-10.0> [Accessed 18 May 2026]
 **************************************************************************************************************/
/**************************************************************************************************************
 * Author: Microsoft
 * Date: 2026
 * Title: Generate OpenAPI documents
 * Type: [Source Code]
 * Available at: <https://learn.microsoft.com/en-us/aspnet/core/fundamentals/openapi/aspnetcore-openapi?view=aspnetcore-10.0> [Accessed 18 May 2026]
 **************************************************************************************************************/
/**************************************************************************************************************
 * Author: Microsoft
 * Date: 2026
 * Title: Microsoft SQL Server Database Provider - EF Core
 * Type: [Source Code]
 * Available at: <https://learn.microsoft.com/en-us/ef/core/providers/sql-server/> [Accessed 6 April 2026]
 **************************************************************************************************************/
/**************************************************************************************************************
 * Author: Microsoft
 * Date: 2026
 * Title: Applying Migrations
 * Type: [Source Code]
 * Available at: <https://learn.microsoft.com/en-us/ef/core/managing-schemas/migrations/applying> [Accessed 18 May 2026]
 **************************************************************************************************************/

using System.Text;
using GLMS.Api.Data;
using GLMS.Api.Repositories;
using GLMS.Api.Services;
using GLMS.Api.Services.Factories;
using GLMS.Api.Services.Observers;
using GLMS.Api.Services.Strategies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;

var builder = WebApplication.CreateBuilder(args);
            builder.Configuration.AddJsonFile("appsettings.Local.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables()
                .AddCommandLine(args);

builder.Services.AddDbContext<GlmsDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("GlmsDb")));

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "GLMS API",
        Version = "v1",
        Description = "Backend API for the Global Logistics Management System"
    });

    options.AddSecurityDefinition(JwtBearerDefaults.AuthenticationScheme, new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Description = "Enter a JWT bearer token.",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT"
    });

    options.AddSecurityRequirement(document => new OpenApiSecurityRequirement
    {
        [new OpenApiSecuritySchemeReference(JwtBearerDefaults.AuthenticationScheme, document)] = new List<string>()
    });
});

var jwtKey = builder.Configuration["Jwt:Key"];
if (string.IsNullOrWhiteSpace(jwtKey) || Encoding.UTF8.GetByteCount(jwtKey) < 32)
    throw new InvalidOperationException("Configure Jwt:Key with at least 32 bytes of random secret material.");

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateIssuerSigningKey = true,
        ValidateLifetime = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
        ClockSkew = TimeSpan.FromMinutes(2)
    };
});

builder.Services.AddAuthorization();

builder.Services.AddScoped<IClientRepository, ClientRepository>();
builder.Services.AddScoped<IContractRepository, ContractRepository>();
builder.Services.AddScoped<IServiceRequestRepository, ServiceRequestRepository>();

builder.Services.AddScoped<IClientService, ClientService>();
builder.Services.AddScoped<IContractService, ContractService>();
builder.Services.AddScoped<IServiceRequestService, ServiceRequestService>();
builder.Services.AddScoped<ICurrencyService, CurrencyService>();
builder.Services.AddScoped<IAgreementService, AgreementService>();
builder.Services.AddScoped<IJwtTokenService, JwtTokenService>();

builder.Services.AddScoped<DraftContractFactory>();
builder.Services.AddScoped<ActiveContractFactory>();
builder.Services.AddScoped<ExpiredContractFactory>();
builder.Services.AddScoped<OnHoldContractFactory>();
builder.Services.AddScoped<DeclinedContractFactory>();
builder.Services.AddScoped<IContractFactory, DraftContractFactory>();
builder.Services.AddScoped<IContractFactory, ActiveContractFactory>();
builder.Services.AddScoped<IContractFactory, ExpiredContractFactory>();
builder.Services.AddScoped<IContractFactory, OnHoldContractFactory>();
builder.Services.AddScoped<IContractFactory, DeclinedContractFactory>();
builder.Services.AddScoped<ContractFactoryResolver>();

builder.Services.AddScoped<ServiceRequestBlockerObserver>();
builder.Services.AddScoped<NotificationObserver>();
builder.Services.AddScoped<AuditLogObserver>();
builder.Services.AddScoped<ContractStatusSubject>(serviceProvider =>
{
    var subject = new ContractStatusSubject();
    subject.Register(serviceProvider.GetRequiredService<ServiceRequestBlockerObserver>());
    subject.Register(serviceProvider.GetRequiredService<NotificationObserver>());
    subject.Register(serviceProvider.GetRequiredService<AuditLogObserver>());
    return subject;
});

builder.Services.AddHttpClient<LiveApiConversionStrategy>();
builder.Services.AddScoped<CachedRateConversionStrategy>();
builder.Services.AddScoped<FixedRateConversionStrategy>();
builder.Services.AddScoped<CurrencyConversionService>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<GlmsDbContext>();
    dbContext.Database.Migrate();
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
