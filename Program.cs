using System.Reflection;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.RateLimiting;
using APICatalogo.Context;
using APICatalogo.DTOs.Mappings;
using APICatalogo.Filters;
using APICatalogo.Logging;
using APICatalogo.Models;
using APICatalogo.RateLimitOptions;
using APICatalogo.Repositores;
using APICatalogo.Services;
using Asp.Versioning;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args: args);

// Add services to the container.
builder.Services.AddControllers(configure: options =>
{
    options.Filters.Add(filterType: typeof(ApiExceptionFilter));
})
.AddJsonOptions(configure: options =>
{
    options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
});

builder.Services.AddControllers(configure: options =>
{
    options.Filters.Add(filterType: typeof(ApiExceptionFilter));
})
.AddNewtonsoftJson(setupAction: options =>
{
    options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
});

var OrigensComAcessoPermitido = "_origensComAcessoPermitido";

builder.Services.AddCors(setupAction: options =>
{
    options.AddPolicy(name: "OrigensComAcessoPermitido",
        configurePolicy: policy =>
        {
            policy.WithOrigins(origins: "https://localhost:7223")
                .WithMethods(methods: ["GET", "POST"])
                .AllowAnyHeader();
        });
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(setupAction: c =>
{
    //c.SwaggerDoc("v1", new OpenApiInfo { Title = "apicatalogo", Version = "v1" });

    c.SwaggerDoc(name: "v1", info: new OpenApiInfo
    {
        Version = "v1",
        Title = "APICatalogo",
        Description = "Catálogo de Produtos e Categorias",
        TermsOfService = new Uri(uriString: "https://master.com/terms"),
        Contact = new OpenApiContact
        {
            Name = "Master",
            Email = "master@gmail.com",
            Url = new Uri(uriString: "https://www.master.net"),
        },
        License = new OpenApiLicense
        {
            Name = "User sobre LICX",
            Url = new Uri(uriString: "https://master.net/license"),
        }
    });

    var xmlFileName = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    c.IncludeXmlComments(filePath: Path.Combine(path1: AppContext.BaseDirectory, path2: xmlFileName));
    
    c.AddSecurityDefinition(name: "Bearer", securityScheme: new OpenApiSecurityScheme()
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "JWT Authorization header using the Bearer scheme.",
    });

    c.AddSecurityRequirement(securityRequirement: new OpenApiSecurityRequirement()
    {
        {
            new OpenApiSecurityScheme()
            {
                Reference = new OpenApiReference()
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] { }
        }
    });
});

builder.Services.AddIdentity<ApplicationUser, IdentityRole>().
    AddEntityFrameworkStores<AppDbContext>()
        .AddDefaultTokenProviders();

string? npsqlConnection = builder.Configuration.GetConnectionString(name: "DefaultConnection");

builder.Services.AddDbContext<AppDbContext>(optionsAction: options =>
                    options.UseNpgsql(connectionString: npsqlConnection));

builder.Services.AddScoped<ApiLoggingFilter>();

builder.Services.AddApiVersioning(setupAction: o =>
{
    o.DefaultApiVersion = new ApiVersion(majorVersion: 1, minorVersion: 0);
    o.AssumeDefaultVersionWhenUnspecified = true;
    o.ReportApiVersions = true;
    o.ApiVersionReader = ApiVersionReader.Combine(
        apiVersionReader: new QueryStringApiVersionReader(),
        otherApiVersionReaders: new UrlSegmentApiVersionReader());
}).AddApiExplorer(setupAction: options =>
{
    options.GroupNameFormat = "'v'VVV";
    options.SubstituteApiVersionInUrl = true;
});

builder.Services.AddScoped<ICategoriaRepository,CategoriaRepository>();
builder.Services.AddScoped<IProdutoRepository,ProdutoRepository>();
builder.Services.AddScoped(serviceType: typeof(IRepository<>), implementationType: typeof(Repository<>));
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<ITokenService, TokenService>();

var secretKey = builder.Configuration[key: "JWT:SecretKey"]
                ?? throw new ArgumentNullException(paramName: "Invalid secret key");

builder.Services.AddAuthentication(configureOptions: options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(configureOptions: options =>
{
    options.SaveToken = true;
    options.RequireHttpsMetadata = true;
    options.TokenValidationParameters = new TokenValidationParameters()
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ClockSkew = TimeSpan.Zero,
        ValidAudience = builder.Configuration[key: "JWT:ValidAudience"],
        ValidIssuer = builder.Configuration[key: "JWT:ValidIssuer"],
        IssuerSigningKey = new SymmetricSecurityKey(
            key: Encoding.UTF8.GetBytes(s: secretKey))
    };
});

builder.Services.AddAuthorization(configure: options =>
{
    options.AddPolicy(name: "AdminOnly", configurePolicy: policy => policy.RequireRole(roles: "Admin"));
    
    options.AddPolicy(name: "SuperAdminOnly", configurePolicy: policy => policy.RequireRole(roles: "Admin").
        RequireRole(roles: ["id", "master"]));

    options.AddPolicy(name: "UserOnly", configurePolicy: policy => policy.RequireRole(roles: "User"));
    
    options.AddPolicy(name: "ExclusivePolicyOnly", configurePolicy: policy =>
                             policy.RequireAssertion(handler: context => 
                                 context.User.HasClaim(match: claim => 
                                     claim.Type == "id" && 
                                     claim.Value == "master")
                                 || context.User.IsInRole(role: "SuperAdmin")));
});

var myOptions = new MyRateLimitOptions();

builder.Configuration.GetSection(key: MyRateLimitOptions.MyRateLimit).Bind(instance: myOptions);
builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

    options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(
        httpContext =>
            RateLimitPartition.GetFixedWindowLimiter(
                partitionKey: httpContext.User.Identity?.Name
                              ?? httpContext.Request.Host.ToString(),
                factory: partition => new FixedWindowRateLimiterOptions
                {
                    AutoReplenishment = true,
                    PermitLimit = 20,
                    QueueLimit = 0,
                    Window = TimeSpan.FromSeconds(10)
                }));
});

builder.Logging.AddProvider(provider: new CustomLoggerProvider(config: new CustomLoggerProviderConfiguration
{
    LogLevel = LogLevel.Information,
}));
builder.Services.AddAutoMapper(profileAssemblyMarkerTypes: typeof(ProdutoDTOMappingProfile));

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseRateLimiter();

app.UseCors();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.Run();
    