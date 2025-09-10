
using Microsoft.IdentityModel.Tokens;
using Supabase;
using SupaBaseIntegration.Services;
using System.Text;


namespace SupaBaseIntegration
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            var supabaseUrl = builder.Configuration["Supabase:Url"];
            var supabaseKey = builder.Configuration["Supabase:Key"];
            //var supabaseJwtSecret = builder.Configuration["Jwt:Key"];

            // Validate configuration
            if (string.IsNullOrEmpty(supabaseUrl))
                throw new ArgumentNullException(nameof(supabaseUrl), "Supabase URL is not configured");
            if (string.IsNullOrEmpty(supabaseKey))
                throw new ArgumentNullException(nameof(supabaseKey), "Supabase Key is not configured");
            //if (string.IsNullOrEmpty(supabaseJwtSecret))
            //    throw new ArgumentNullException(nameof(supabaseJwtSecret), "Supabase JWT Secret is not configured. Please add it to appsettings.json");

            builder.Services.AddSingleton<Client>(_ =>
            {
                var client = new Client(supabaseUrl, supabaseKey);
                client.InitializeAsync().Wait();
                return client;
            });
            builder.Services.AddScoped<IAuthService, AuthService>();
            builder.Services.AddScoped<IProviderService, ProviderService>();

            builder.Services.AddControllers();

            builder.Services.AddOpenApi();
            builder.Services.AddLogging(config =>
            {
                config.AddConsole();
                config.AddDebug();
            });

            // Add CORS for frontend demo
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowFrontend", policy =>
                {
                    policy.AllowAnyOrigin()
                          .AllowAnyMethod()
                          .AllowAnyHeader();
                });
            });

            //builder.Services.AddAuthentication().AddJwtBearer(options =>
            //{
            //    options.Authority = $"{supabaseUrl}/auth/v1";
            //    options.RequireHttpsMetadata = false; // keep true in prod, false in local dev
            //    options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
            //    {
            //        ValidateIssuerSigningKey = true,
            //        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(supabaseJwtSecret)),
            //        ValidateIssuer = true,
            //        ValidIssuer = $"{supabaseUrl}/auth/v1",
            //        ValidateAudience = true,
            //        ValidAudience = "authenticated", 
            //        ValidateLifetime = true,
            //    };
            //    options.Events = new Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerEvents
            //    {
            //        OnAuthenticationFailed = context =>
            //        {
            //            Console.WriteLine($"Authentication failed: {context.Exception.Message}");
            //            return Task.CompletedTask;
            //        },
            //        OnTokenValidated = context =>
            //        {
            //            Console.WriteLine("Token validated successfully");
            //            return Task.CompletedTask;
            //        }
            //    };
            //});
            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
            }

            app.UseHttpsRedirection();
            app.UseCors("AllowFrontend");

            app.UseAuthentication();
            app.UseAuthorization();


            app.MapControllers();

            var logger = app.Services.GetRequiredService<ILogger<Program>>();
            logger.LogInformation("Supabase Integration API started successfully");
            logger.LogInformation("Supabase URL: {Url}", supabaseUrl);

            app.Run();
        }

    }

}
