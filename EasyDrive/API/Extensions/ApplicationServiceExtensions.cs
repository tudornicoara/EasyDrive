using System.Text;
using API.AppUsers;
using API.Data;
using API.Helpers;
using API.Interfaces;
using API.Services;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace API.Extensions;

public static class ApplicationServiceExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration config)
    {
        services.AddScoped<ITokenService, TokenService>();
        services.AddMediatR(typeof(Create.Handler).Assembly);
        services.AddAutoMapper(typeof(AutoMapperProfiles).Assembly);
        services.AddDbContext<DataContext>(opt =>
        {
            opt.UseSqlite(config.GetConnectionString("DefaultConnection"));
        });

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["TokenKey"] ?? throw new InvalidOperationException())),
                    ValidateIssuer = false,
                    ValidateAudience = false
                };
            });
        
        services.AddCors(opt =>
        {
            opt.AddPolicy("CorsPolicy", policy =>
            {
                policy.AllowAnyMethod().AllowAnyHeader().AllowCredentials().WithOrigins("http://127.0.0.1:5173");
            });
        });

        return services;
    }
}