using AssetManager;
using Microsoft.EntityFrameworkCore;
using AssetManager.Data;
using AssetManager.Interfaces;
using AssetManager.Repository;
using Microsoft.AspNetCore.Identity;
using AssetManager.Helper;
using AutoMapper;
using AssetManager.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.OpenApi.Models;
using ActionFilters.Filters;
using AssetManager.ActionFilers.Filters;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddTransient<Seed>();
builder.Services.AddControllers(options =>
{
    options.Filters.Add<ActionFilterSetUserId>();
});
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c => {
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "JWTToken_Auth_API",
        Version = "v1"
    });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "JWT Authorization header using the Bearer scheme. \r\n\r\n Enter 'Bearer' [space] and then your token in the text input below.\r\n\r\nExample: \"Bearer 1safsfsdfdfd\"",
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement {
        {
            new OpenApiSecurityScheme {
                Reference = new OpenApiReference {
                    Type = ReferenceType.SecurityScheme,
                        Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});
builder.Services.AddSession();
builder.Services.AddAutoMapper(typeof(MappingProfiles));
builder.Services.AddScoped<IAssetRepository, AssetRepository>();
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<IBrokerRepository, BrokerRepository>();
builder.Services.AddScoped<IAssetService, AssetService>();
builder.Services.AddScoped<IPriceService, PriceService>();
builder.Services.AddScoped<ICurrentPriceProvider, CurrentPriceProvider>();
builder.Services.AddScoped<IHistoricalPriceProvider, HistoricalPriceProvider>();
builder.Services.AddScoped<IAssetClassificationsService, AssetClassificationsService>();
builder.Services.AddHttpClient<IHistoricalPriceProvider, HistoricalPriceProvider>();
builder.Services.AddScoped<ExceptionFilter>();

builder.Services.AddCors(options =>
    {
        options.AddPolicy("AllowAllOrigins",
            builder =>
            {
                builder
                    .AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader();
            });
    });


builder.Services.AddDbContext<DataContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});
builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<DataContext>();
var tokenOptions = builder.Configuration.GetSection("TokenOptions").Get<TokenOptions>();
var key = Encoding.UTF8.GetBytes(tokenOptions.Secret);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
})
    .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidIssuer = tokenOptions.Issuer,
            ValidAudience = tokenOptions.Audience,
            ValidateAudience = true,
            ValidateIssuer = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(key),
        };
    });
builder.Services.AddLogging(configure =>
{
    configure.AddConsole(); // You can choose a different logging provider here if needed.
});

var app = builder.Build();



//
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var seed = services.GetRequiredService<Seed>();
         seed.SeedDataContext();
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while seeding the database.");
    }
}
var config = new MapperConfiguration(cfg =>
{
    cfg.AddProfile<MappingProfiles>();
});

try
{
    config.AssertConfigurationIsValid();
}
catch (Exception e)
{
    Console.WriteLine(e);
    throw;
}
app.UseCors("AllowAllOrigins");

// Add a middleware to log token information for debugging purposes
app.Use(async (context, next) =>
{
    var logger = app.Services.GetRequiredService<ILogger<Program>>();
    var token = context.Request.Headers["Authorization"].ToString();
    logger.LogInformation($"Received token a: {token}");

    // Call the next middleware in the pipeline
    await next.Invoke();
});
app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.UseSession();
app.Run();

