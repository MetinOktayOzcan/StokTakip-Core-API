using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using StokTakip_Core_API.Data;
using StokTakip_Core_API.Interfaces;
using StokTakip_Core_API.Repository;
using StokTakip_Core_API.Services;
using StokTakip_Core_API.Validators;
using System.Text;
using System.Threading.RateLimiting;

var homePath = Environment.GetEnvironmentVariable("HOME");
var logDirectory = string.IsNullOrEmpty(homePath) ? "Logs" : Path.Combine(homePath, "LogFiles");
var bootstrapLogPath = Path.Combine(logDirectory, "stoktakip-bootstrap-.txt");
var appLogPath = Path.Combine(logDirectory, "stoktakip-sec-log-.txt");

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File(bootstrapLogPath, rollingInterval: RollingInterval.Day)
    .CreateBootstrapLogger();

try
{
    Log.Information("Uygulama baslatiliyor...");
    var builder = WebApplication.CreateBuilder(args);

    builder.Host.UseSerilog((context, services, configuration) => configuration
     .ReadFrom.Configuration(context.Configuration)
     .ReadFrom.Services(services)
     .Enrich.FromLogContext()
     .WriteTo.Console()
     .WriteTo.File(
         appLogPath,
         rollingInterval: RollingInterval.Day,
         fileSizeLimitBytes: 10 * 1024 * 1024,
         retainedFileCountLimit: 31,
         rollOnFileSizeLimit: true
     ));

    builder.Services.AddDbContext<stokTakipContext>(options =>
        options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

    builder.Services.AddHealthChecks()
        .AddSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")!);

    builder.Services.AddCors(options =>
    {
        options.AddPolicy("GuvenliCORS", policy =>
        {
            policy.WithOrigins("http://localhost:5173", "https://stoktakip.metinoktayozcan.dev")
                  .AllowAnyHeader()
                  .AllowAnyMethod()
                  .AllowCredentials();
        });
    });

    builder.Services.AddControllers();
    builder.Services.AddDistributedMemoryCache();
    builder.Services.AddHttpContextAccessor();

    builder.Services.AddFluentValidationAutoValidation()
                    .AddFluentValidationClientsideAdapters();
    builder.Services.AddValidatorsFromAssemblyContaining<UrunEkleDTOValidator>();

    builder.Services.AddRateLimiter(options =>
    {
        options.AddPolicy("LoginPolicy", httpContext =>
            RateLimitPartition.GetFixedWindowLimiter(
                partitionKey: httpContext.Connection.RemoteIpAddress?.ToString() ?? "BilinmeyenIP",
                factory: partition => new FixedWindowRateLimiterOptions
                {
                    AutoReplenishment = true,
                    PermitLimit = 5,
                    QueueLimit = 0,
                    Window = TimeSpan.FromMinutes(1)
                }));

        options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(httpContext =>
            RateLimitPartition.GetFixedWindowLimiter(
                partitionKey: httpContext.User.Identity?.Name ?? httpContext.Connection.RemoteIpAddress?.ToString() ?? "BilinmeyenIP",
                factory: partition => new FixedWindowRateLimiterOptions
                {
                    AutoReplenishment = true,
                    PermitLimit = 100,
                    QueueLimit = 2,
                    Window = TimeSpan.FromMinutes(1)
                }));

        options.OnRejected = async (context, token) =>
        {
            context.HttpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;
            context.HttpContext.Response.ContentType = "application/json";
            await context.HttpContext.Response.WriteAsync("{\"Mesaj\": \"Çok fazla istek gönderdiniz. Lütfen daha sonra tekrar deneyin.\"}", token);
        };
    });

    var jwtSettings = builder.Configuration.GetSection("Jwt");
    var secretKey = builder.Configuration["JWT_SECRET_KEY"] ?? throw new InvalidOperationException("JWT_SECRET_KEY yapılandırması bulunamadı!");

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
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSettings["Issuer"],
            ValidAudience = jwtSettings["Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
            ClockSkew = TimeSpan.Zero
        };

        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                if (context.Request.Cookies.ContainsKey("X-Access-Token"))
                {
                    context.Token = context.Request.Cookies["X-Access-Token"];
                }
                return Task.CompletedTask;
            },
            OnTokenValidated = async context =>
            {
                var cache = context.HttpContext.RequestServices.GetRequiredService<Microsoft.Extensions.Caching.Distributed.IDistributedCache>();
                var jti = context.Principal?.FindFirst(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Jti)?.Value;

                if (!string.IsNullOrEmpty(jti))
                {
                    var isBlacklisted = await cache.GetStringAsync($"blacklist_{jti}");
                    if (!string.IsNullOrEmpty(isBlacklisted))
                    {
                        context.Fail("Bu token oturum kapatma işlemi ile geçersiz kılınmıştır.");
                    }
                }
            }
        };
    });

    builder.Services.AddScoped<IAuditLogService, AuditLogService>();
    builder.Services.AddScoped<IStokHareketService, StokHareketService>();
    builder.Services.AddScoped<IKategoriRepository, KategoriRepository>();
    builder.Services.AddScoped<IUrunRepository, UrunRepository>();
    builder.Services.AddScoped<IStokHareketleriRepository, StokHareketleriRepository>();
    builder.Services.AddScoped<IIslemGecmisiRepository, IslemGecmisiRepository>();
    builder.Services.AddScoped<IKullaniciRepository, KullaniciRepository>();
    builder.Services.AddSingleton<IDateTimeProvider, TurkeyDateTimeProvider>();

    builder.WebHost.ConfigureKestrel(options =>
    {
        options.AddServerHeader = false;
    });

    var app = builder.Build();

    var forwardedHeadersOptions = new ForwardedHeadersOptions
    {
        ForwardedHeaders = Microsoft.AspNetCore.HttpOverrides.ForwardedHeaders.XForwardedFor |
                           Microsoft.AspNetCore.HttpOverrides.ForwardedHeaders.XForwardedProto
    };

    var trustedProxyIp = app.Configuration["Security:TrustedProxy"];
    if (!string.IsNullOrEmpty(trustedProxyIp) && System.Net.IPAddress.TryParse(trustedProxyIp, out var proxyIp))
    {
        forwardedHeadersOptions.KnownProxies.Add(proxyIp);
    }

    app.UseForwardedHeaders(forwardedHeadersOptions);

    app.UseMiddleware<StokTakip_Core_API.Middlewares.ExceptionMiddleware>();
    app.UseMiddleware<StokTakip_Core_API.Middlewares.SecurityHeadersMiddleware>();

    if (!app.Environment.IsDevelopment())
    {
        app.UseHsts();
    }

    app.UseHttpsRedirection();
    app.UseRouting();
    app.UseCors("GuvenliCORS");
    app.UseRateLimiter();
    app.UseAuthentication();
    app.UseAuthorization();

    app.MapHealthChecks("/api/health").RequireAuthorization(new AuthorizeAttribute { Roles = "Admin" });
    app.MapControllers();

    try
    {
        DataSeeder.Seed(app);
        Log.Information("DataSeeder calisti: Veritabani hazir ve seed islemi tamamlandi.");
    }
    catch (Exception dbEx)
    {
        Log.Error(dbEx, "DIKKAT: Veritabanina baglanirken veya tablo olusturulurken hata meydana geldi.");
    }

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "API baslatilirken kritik bir hata olustu.");
}
finally
{
    Log.Information("Uygulama sonlandiriliyor.");
    Log.CloseAndFlush();
}