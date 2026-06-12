using LibraryService.WebAPI.Data;
using LibraryService.WebAPI.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;

namespace LibraryService.WebAPI
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // CORS: origenes permitidos configurables (appsettings o variables
            // de entorno Cors__AllowedOrigins__0, ...) para el FE local y el publicado.
            var allowedOrigins = Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>()
                                 ?? new[] { "http://localhost:5173", "https://examfinallab.netlify.app" };

            services.AddCors(o => o.AddPolicy("Frontend", p => p
                .WithOrigins(allowedOrigins)
                .AllowAnyHeader()
                .AllowAnyMethod()));

            // Servicio del modulo publico de reportes de fraude.
            services.AddTransient<IFraudService, FraudService>();

            services.AddDbContextPool<AppDbContext>(options =>
                options.UseNpgsql(Configuration.GetConnectionString("DefaultConnection"), npgsqlOptions =>
                {
                    npgsqlOptions.EnableRetryOnFailure(
                        maxRetryCount: 1,
                        maxRetryDelay: TimeSpan.FromSeconds(5),
                        errorCodesToAdd: null);
                }),
                poolSize: 20);

            services.AddControllers();

            // Add Swagger generation
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "Fraud Reports API",
                    Version = "v1",
                    Description = "API publica para registrar y consultar reportes de fraude (LABCIBE-UNA)"
                });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
          //  if (env.IsDevelopment())
          //  {
                app.UseDeveloperExceptionPage();
           // }

            // Swagger habilitado tambien en produccion para facilitar la
            // verificacion del API publicada.
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Fraud Reports API v1");
            });

            // Aplica las migraciones pendientes al iniciar (crea la tabla Frauds).
            using (var scope = app.ApplicationServices.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                db.Database.Migrate();
            }

            app.UseRouting();

            app.UseCors("Frontend");

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
