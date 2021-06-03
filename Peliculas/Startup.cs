using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Peliculas.Datos;
using Peliculas.Repository;
using Peliculas.Repository.IRepository;
using Peliculas.PeliculasMapper;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Reflection;
using System.IO;
using System;
using Microsoft.OpenApi.Models;
using System.Collections.Generic;
using System.Net;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Peliculas.Helpers;

namespace Peliculas
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
            services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(Configuration.GetConnectionString("DevConnection")));

            services.AddScoped<ICategoriaRepository, CategoriaRepository>();
            services.AddScoped<IPeliculaRepository, PeliculaRepository>();
            services.AddScoped<IUsuarioRepository, UsuarioRepository>();

            //Agregar dependencias del token 
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(Configuration.GetSection("AppSettings:Token").Value)),
                        ValidateIssuer = false,
                        ValidateAudience = false
                    };
                }
                );

            services.AddAutoMapper(typeof(PeliculasMappers));

            //Configuracion de documentacion 
            //Categoria 
            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("ApiPeliculasCategorias", new Microsoft.OpenApi.Models.OpenApiInfo()
                {
                    Title = "API Categoria Películas",
                    Version = "1",
                    Description = "Backend Películas",
                    Contact = new Microsoft.OpenApi.Models.OpenApiContact()
                    {
                        Email = "pujolsjosue5@gmail.com",
                        Name = "Josue Pujols",
                        Url = new Uri("https://github.com/josuepujols")
                    },
                    License = new Microsoft.OpenApi.Models.OpenApiLicense()
                    {
                        Name = "MIT License",
                        Url = new Uri("https://en.wikipedia.org/wiki/MIT_License")
                    }
                });

                //Peliculas
                options.SwaggerDoc("ApiPeliculas", new Microsoft.OpenApi.Models.OpenApiInfo()
                {
                    Title = "API Películas",
                    Version = "1",
                    Description = "Backend Películas",
                    Contact = new Microsoft.OpenApi.Models.OpenApiContact()
                    {
                        Email = "pujolsjosue5@gmail.com",
                        Name = "Josue Pujols",
                        Url = new Uri("https://github.com/josuepujols")
                    },
                    License = new Microsoft.OpenApi.Models.OpenApiLicense()
                    {
                        Name = "MIT License",
                        Url = new Uri("https://en.wikipedia.org/wiki/MIT_License")
                    }
                });

                //Usuarios
                options.SwaggerDoc("ApiUsuariosPeliculas", new Microsoft.OpenApi.Models.OpenApiInfo()
                {
                    Title = "API Usuarios Películas",
                    Version = "1",
                    Description = "Backend Películas",
                    Contact = new Microsoft.OpenApi.Models.OpenApiContact()
                    {
                        Email = "pujolsjosue5@gmail.com",
                        Name = "Josue Pujols",
                        Url = new Uri("https://github.com/josuepujols")
                    },
                    License = new Microsoft.OpenApi.Models.OpenApiLicense()
                    {
                        Name = "MIT License",
                        Url = new Uri("https://en.wikipedia.org/wiki/MIT_License")
                    }
                });

                var archivo_xml_comentarios = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var ruta_api_comentarios = Path.Combine(AppContext.BaseDirectory, archivo_xml_comentarios);
                options.IncludeXmlComments(ruta_api_comentarios);


                //Esquema de seguridad 
                options.AddSecurityDefinition("Bearer",
                    new OpenApiSecurityScheme
                    {
                        Description = "Autenticacion JWT (Bearer)",
                        Type = SecuritySchemeType.Http,
                        Scheme = "bearer"
                    }); 

                options.AddSecurityRequirement(new OpenApiSecurityRequirement {
                        {
                            new OpenApiSecurityScheme
                            {
                                Reference = new OpenApiReference
                                {
                                    Id = "Bearer",
                                    Type = ReferenceType.SecurityScheme
                                }
                            }, new List<string>()
                        }
                });
            });

            services.AddControllers();

            //Habilitar los cors 
            services.AddCors();

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler(builder => {
                    builder.Run(async context => {
                        context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                        var error = context.Features.Get<IExceptionHandlerFeature>();
                        if (error != null)
                        {
                            context.Response.AddApplicationError(error.Error.Message);
                            await context.Response.WriteAsync(error.Error.Message);
                        }
                    });
                });
            }

            app.UseHttpsRedirection();

            //Documentacion 
            app.UseSwagger();
            app.UseSwaggerUI(options => 
            {
                options.SwaggerEndpoint("/swagger/ApiPeliculasCategorias/swagger.json", "API categorias Películas");
                options.SwaggerEndpoint("/swagger/ApiPeliculas/swagger.json", "API Películas");
                options.SwaggerEndpoint("/swagger/ApiUsuariosPeliculas/swagger.json", "API Usuarios Películas");
                options.RoutePrefix = "";

            });

            app.UseRouting();

            //Autenticacion y Autorizacion 
            app.UseAuthorization();
            app.UseAuthentication();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            //Habilitar cors en este metodo 
            app.UseCors(x => x.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());

        }
    }
}
