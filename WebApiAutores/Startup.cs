using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using WebApiAutores.Filtros;
using WebApiAutores.Middlewares;
using WebApiAutores.Servicios;
using WebApiAutores.Utilidades;

namespace WebApiAutores
{
    public class Startup
    {

        public Startup(IConfiguration configuration)
        {
            //Se usa para limpiar los mapeos de los claims
            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            services.AddControllers(opciones =>
            {
                opciones.Filters.Add(typeof(FiltroDeExcepcion));
                opciones.Conventions.Add(new SwaggerAgrupaPorVersion());
            }).AddNewtonsoftJson();

            services.AddDbContext<ApplicationDbContext>(options => 
                options.UseSqlServer(Configuration.GetConnectionString("defaultConnection")));

            //Cuando no utiliza estado
            //Siempre crea una instancia por cada peticion
            services.AddTransient<IServicio, ServicioA>();


            //Siempre usa la misma instancia por usuario
            //services.AddSingleton<IServicio, ServicioA>();

            //Crea una instancia por peticion http
            //Para el dbContext(Esto lo hace automaticamente) crea una instacia para cada usuario
            //services.AddScoped<IServicio, ServicioA>();


            services.AddTransient<ServicioTransient>();
            services.AddScoped<ServicioScoped>();
            services.AddSingleton<ServicioSinglenton>();

            services.AddTransient<MiFiltroDeAccion>();

            //services.AddHostedService<EscribirEnArchivo>();

            services.AddResponseCaching();


            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(opciones => opciones.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(Configuration["llavejwt"])),
                    ClockSkew = TimeSpan.Zero
                });


            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "WebApiAutores",
                    Version = "v1",
                    Description = "Este es un Web Api para trabajar con Autores y Libro",
                    Contact = new OpenApiContact
                    {
                        Name = "Piero Aldaves",
                        Email = "piero@gmail.com",
                        
                    },
                    License = new OpenApiLicense
                    {
                        Name = "MIT"
                    }
                });
                c.SwaggerDoc("v2", new OpenApiInfo { Title = "WebApiAutores", Version = "v2" });

                c.OperationFilter<AgregarParemetroXVersion>();
                c.OperationFilter<AgregarParametroHATEOAS>();

                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header
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
                        new string[]{}
                    }
                });

                var archivoXML = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var rutaXML = Path.Combine(AppContext.BaseDirectory, archivoXML);
                c.IncludeXmlComments(rutaXML);

            });

            services.AddAutoMapper(typeof(Startup));

            services.AddIdentity<IdentityUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();


            services.AddAuthorization(opciones =>
            {
                opciones.AddPolicy("EsAdmin", politica => politica.RequireClaim("esAdmin"));
            });

            services.AddCors(opciones => opciones.AddDefaultPolicy(builder =>
            {
                builder.WithOrigins("https://www.apirequest.io").AllowAnyMethod().AllowAnyHeader()
                .WithExposedHeaders(new string[] { "cantidadTotalRegistros" });

            }));

            services.AddDataProtection();

            services.AddTransient<HashService>();

            services.AddTransient<GeneradorEnlaces>();

            services.AddTransient<HATEOSAutorFilterAttribute>();

            services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();

            services.AddApplicationInsightsTelemetry(Configuration["ApplicationInsights:ConnectionString"]);

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILogger<Startup> logger)
        {

            //app.UseLoguearRespuestaHTTP();

            //app.Map("/ruta1", app =>
            //{
            //    app.Run(async contexto =>
            //    {
            //        await contexto.Response.WriteAsync("Estoy intersectando la tuberia");
            //    });
            //});

           

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
               
            }

            app.UseSwagger();
            app.UseSwaggerUI(c => {

                c.SwaggerEndpoint("/swagger/v1/swagger.json", "WebApiAutores v1");
                c.SwaggerEndpoint("/swagger/v2/swagger.json", "WebApiAutores v2");


            });

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseCors();

            app.UseResponseCaching();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
