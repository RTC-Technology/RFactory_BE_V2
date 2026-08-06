using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using RFactory.API.Infrastructure;
using RFactory.Shared.Abstractions;
using RFactory.Shared.Constants;
using RFactory.Shared.Security;

namespace RFactory.API.Extensions;

public static class DependencyInjection
{
    public static IServiceCollection AddApi(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddControllers();
        services.AddEndpointsApiExplorer();

        services.AddHttpContextAccessor();
        services.AddScoped<IUser, CurrentUser>();

        services.AddSingleton(TimeProvider.System);
        services.AddSingleton<IPasswordHasher, BCryptPasswordHasher>();

        // The refresh-token cookie only travels on credentialed requests, and the spec
        // forbids pairing AllowCredentials with a wildcard origin — browsers reject
        // `Access-Control-Allow-Origin: *` outright once credentials are involved. So the
        // origins have to be named. Production must set Cors:AllowedOrigins; the fallback
        // covers a local `ng serve` and nothing else.
        var allowedOrigins = configuration.GetSection("Cors:AllowedOrigins").Get<string[]>();
        if (allowedOrigins is null || allowedOrigins.Length == 0)
        {
            allowedOrigins = new[] { "http://localhost:4200", "https://localhost:4200" };
        }

        services.AddCors(opt =>
            opt.AddPolicy(AppConstants.CorsPolicy, policy => policy
                .WithOrigins(allowedOrigins)
                .AllowAnyMethod()
                .AllowAnyHeader()
                .AllowCredentials()));

        var jwtOptions = configuration.GetSection("Jwt").Get<JwtOptions>()
            ?? throw new InvalidOperationException("JWT configuration section 'Jwt' is missing.");
        services.Configure<JwtOptions>(configuration.GetSection("Jwt"));

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(opt =>
            {
                opt.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtOptions.Issuer,
                    ValidAudience = jwtOptions.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(jwtOptions.SecretKey))
                };
            });

        services.AddAuthorization();

        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo { Title = "RFactory MES API", Version = "v1" });
            c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Description = "JWT Authorization header: 'Bearer {token}'",
                Name = "Authorization",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.ApiKey,
                Scheme = "Bearer"
            });
            c.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
                    },
                    Array.Empty<string>()
                }
            });
        });

        return services;
    }
}
