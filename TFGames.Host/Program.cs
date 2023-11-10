using System.Reflection;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using SendGrid.Extensions.DependencyInjection;
using TFGames.API.Controllers;
using TFGames.API.Helpers;
using TFGames.API.Services;
using TFGames.API.Services.Interfaces;
using TFGames.Common.Configurations;
using TFGames.Common.Constants;
using TFGames.DAL.Entities;
using TFGames.DAL.Data;
using TFGames.DAL.Data.Repository;
using TFGames.Host.Middleware;
using TFGames.API.Mappers;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddDbContext<DataContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("SqlServer"));
});

builder.Services.AddSendGrid(options => {
    options.ApiKey = builder.Configuration.GetValue<string>("SendGridApiKey");
});

builder.Services.AddSwaggerGen(options =>
{
    var xmlFilename = $"{Assembly.GetAssembly(typeof(UserController)).GetName().Name}";
    options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, $"{xmlFilename}.xml"));

    options.EnableAnnotations();
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "first-version",
        Title = "TFGame API",
        Description = "ASPNER Web-service",
        TermsOfService = new Uri("https://example.com/terms"),
        Contact = new OpenApiContact
        {
            Name = " ",
            Url = new Uri("https://example.com/contact")
        },
        License = new OpenApiLicense
        {
            Name = "Example License",
            Url = new Uri("https://example.com/license")
        }
    });
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
    {
        In = ParameterLocation.Header,
        Description = "Access token",
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                },
                Scheme = "oauth2",
                Name = "Bearer",
                In = ParameterLocation.Header
            },
            new List<string>()
        }
    });
});

builder.Logging.AddConsole();

builder.Services.AddMemoryCache(options =>
{
    options.CompactionPercentage = 0.02;
    options.ExpirationScanFrequency = TimeSpan.FromMinutes(30);
    options.SizeLimit = 500;
});

builder.Services.Configure<JwtTokenSettings>(builder.Configuration.GetSection("JWTSettings"));
builder.Services.Configure<Domains>(builder.Configuration.GetSection(nameof(Domains)));
builder.Services.Configure<ValidImageProperties>(builder.Configuration.GetSection("ValidImageProperties"));

var environment = builder.Configuration.GetSection("Environment").Value;

builder.Configuration.AddEnvironmentVariables();

builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
builder.Configuration.AddJsonFile($"appsettings.{environment}.json", optional: false, reloadOnChange: true);

var jwtSettings = builder.Configuration.GetSection("JWTSettings").Get<JwtTokenSettings>();

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = jwtSettings.ValidIssuer,
            ValidateAudience = true,
            ValidAudience = jwtSettings.ValidAudience,
            ValidateIssuerSigningKey = true,

            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.ASCII.GetBytes(jwtSettings.SymmetricSecurityKey)
            ),
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };
    });

builder.Services.AddIdentityCore<User>(options => {
    options.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-_+=!";
})
    .AddRoles<IdentityRole>()
    .AddSignInManager<SignInManager<User>>()
    .AddEntityFrameworkStores<DataContext>()
    .AddDefaultTokenProviders();

builder.Services.AddControllers();

builder.Services.AddAutoMapper(typeof(ArticleAutoMapper));

builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IArticleRepository, ArticleRepository>();
builder.Services.AddScoped<ICommentRepository, CommentRepository>();
builder.Services.AddScoped<IArticleTagsRepository, ArticleTagsRepository>();
builder.Services.AddScoped<IApplicationSettingsRepository, ApplicationSettingsRepository>();
builder.Services.AddScoped<IApplicationSettingsService, ApplicationSettingsService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IAccountService, AccountService>();
builder.Services.AddScoped<IArticleService, ArticleService>();
builder.Services.AddScoped<IJwtService, JwtService>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<IImageService, ImageService>();
builder.Services.AddScoped<ICommentService, CommentService>();

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(builder =>
    {
        builder.AllowAnyOrigin()
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

var app = builder.Build();

var scopeFactory = app.Services.GetRequiredService<IServiceScopeFactory>();

using (var scope = scopeFactory.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<DataContext>();

    var isConnectable = context.Database.CanConnect();

    context.Database.Migrate();

    if (!isConnectable)
    {
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

        await RoleInitializer.Initialize(roleManager);

        var userService = scope.ServiceProvider.GetRequiredService<IUserService>();

        await SuperAdminInitializer.Initialize(userService);
    }
}

// Configure the HTTP request pipeline.

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<ErrorHandlerMiddleware>();

app.UseHttpsRedirection();


app.UseRouting();

app.UseAuthorization();
app.UseAuthentication();
app.UseCors();

app.MapControllers();

app.Run();