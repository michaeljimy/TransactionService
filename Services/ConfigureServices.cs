using Confluent.Kafka;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using Transaction_Service.Data.Contexts;
using Transaction_Service.Interfaces;
using Transaction_Service.Repositories.Interfaces;
using Transaction_Service.Services;

public static class ConfigureService
{
    public static void AddCustomServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddControllers();
        services.AddHttpContextAccessor();
        services.AddHttpClient<IServiceWallet, WalletService>((sp, client) =>
        {
            var configuration = sp.GetRequiredService<IConfiguration>();
            var walletApiUrl = configuration["WalletServiceUrl"] ?? "http://localhost:5086/api/v1/wallet";
            client.BaseAddress = new Uri(walletApiUrl);
        });

        services.AddEndpointsApiExplorer();
        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            var jwtS = configuration["JWTSecretKey"] ?? "JWTSecretKey";
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(jwtS))
            };
        });
        services.AddScoped<IServiceTransaction, TransactionService>(); // Change to Scoped
        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo { Title = "Transaction Service API", Version = "v1" });
            c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                In = ParameterLocation.Header,
                Description = "Please enter into field the word 'Bearer' followed by a space and the JWT value",
                Name = "Authorization",
                Type = SecuritySchemeType.ApiKey
            });
            c.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    new string[] {}
                }
            });
        });

    }

    public static void AddProducerConfig(this IServiceCollection services, ProducerConfig producerConfig)
    {
        services.AddSingleton(producerConfig);
        services.AddSingleton<IServiceKafka, KafkaServices>(); // Register IKafkaService
    }

    public static void AddServiceDbContext(this IServiceCollection services, string DbConnectionString)
    {
        services.AddDbContext<ServiceDbContext>((opt) =>
        {
            opt.UseSqlServer(DbConnectionString)
                .EnableSensitiveDataLogging()
                .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
        });
    }
}
