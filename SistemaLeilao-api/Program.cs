using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore; 
using Microsoft.IdentityModel.Tokens;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure; // Add this for ServerVersion
using SistemaLeilao_api.Data; 
using SistemaLeilao_api.Hubs; 
using SistemaLeilao_api.Interfaces; // Add this
using SistemaLeilao_api.Services; // Add this
using System.Text;

namespace SistemaLeilao_api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            // Configure DbContext for MySQL
            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") 
                                   ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
            
            var serverVersion = new MySqlServerVersion(new Version(8, 0, 34)); 

            builder.Services.AddDbContext<LeilaoDbContext>(options =>
                options.UseMySql(connectionString, serverVersion, 
                    mySqlOptions => mySqlOptions.EnableRetryOnFailure(
                        maxRetryCount: 5,
                        maxRetryDelay: TimeSpan.FromSeconds(30),
                        errorNumbersToAdd: null)
                ));

            // Register Services 
            builder.Services.AddScoped<IAuthService, AuthService>();
            builder.Services.AddScoped<IUserService, UserService>(); 
            builder.Services.AddScoped<ILeilaoService, LeilaoService>(); // Register LeilaoService
            // Register other services here as they are created (e.g., ILanceService)

            // Configure JWT Authentication
            var jwtSettings = builder.Configuration.GetSection("Jwt");
            var key = Encoding.ASCII.GetBytes(jwtSettings["Key"] ?? throw new ArgumentNullException("Jwt:Key", "JWT Key not configured in appsettings.json"));

            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = false; 
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = true,
                    ValidIssuer = jwtSettings["Issuer"],
                    ValidateAudience = true,
                    ValidAudience = jwtSettings["Audience"],
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero 
                };
            });

            builder.Services.AddAuthorization();

            // Add SignalR service
            builder.Services.AddSignalR();

            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAll", policy =>
                {
                    policy.AllowAnyOrigin()
                          .AllowAnyMethod()
                          .AllowAnyHeader();
                });
            });


            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();
            app.UseCors("AllowAll");
            app.UseAuthentication();
            app.UseAuthorization();
            app.MapControllers();
            app.MapHub<AuctionHub>("/auctionHub"); 

            app.Run();
        }
    }
}

