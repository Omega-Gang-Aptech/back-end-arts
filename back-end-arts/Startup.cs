
using back_end_arts.Interface;
using back_end_arts.Models;
using back_end_arts.Repository;
using back_end_arts.Service;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace back_end_arts
{
    public class Startup
    {
        readonly string AllowSpecificOrigins = "AllowSpecificOrigins";
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            #region Connect DB
            services.AddDbContext<ArtsDbWithKeyContext>(options => options.UseSqlServer(Configuration.GetConnectionString("artscon")));
            #endregion

            #region Add Repo
            services.AddScoped(typeof(IArtsRepository<>), typeof(ArtsRepository<>));
            services.AddScoped<IJwtService, JwtService>();
            #endregion

            #region Allow CORS
            services.AddCors(options =>
            {
                options.AddPolicy(name: AllowSpecificOrigins,
                                  builder =>
                                  {
                                      builder.AllowAnyHeader()
            .AllowAnyMethod()
            .WithOrigins("http://localhost:3000")
            .AllowCredentials();
                                  });
            });
            #endregion

            #region Add Swagger
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "StationaryServer2", Version = "v1" });
                c.EnableAnnotations();
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    In = ParameterLocation.Header,
                    Description = "Please insert JWT with Bearer into field",
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey
                });
                c.AddSecurityRequirement(new OpenApiSecurityRequirement {
                                                                           {
                                                                             new OpenApiSecurityScheme
                                                                             {
                                                                               Reference = new OpenApiReference
                                                                               {
                                                                                 Type = ReferenceType.SecurityScheme,
                                                                                 Id = "Bearer"
                                                                               }
                                                                              },
                                                                              new string[] { }
                                                                            }
                                                                          });

            });

            #endregion

            #region JWT Authentication
            services.Configure<JwtConfig>(Configuration.GetSection("JwtConfig"));//ket noi,
            var key = Encoding.ASCII.GetBytes(Configuration["JwtConfig:Secret"]);//ma hoa va sinh ra JWT, ma hoa ve byte

            var tokenValidationParams = new TokenValidationParameters
            {
                //tu cap token
                ValidateIssuer = false,
                ValidateAudience = false,//sai dich vu o ngoai thi true va chi duong dan cua dich vu

                //ky len token cua minh
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),

                //thoi gian hieu luc
                ValidateLifetime = true,
                RequireExpirationTime = true,
                ClockSkew = TimeSpan.Zero
            };
            services.AddSingleton(tokenValidationParams);
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;//them thu vien
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;//them thu vien

            })
            .AddJwtBearer(jwt =>
            {
                jwt.SaveToken = true;
                jwt.TokenValidationParameters = tokenValidationParams;
            });
            #endregion
            services.AddCors();
            services.AddControllers();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "back_end_arts v1"));
            }

            app.UseHttpsRedirection();
            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}