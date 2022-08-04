using ApiManagerStudent.EF;
using ApiManagerStudent.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiManagerStudent
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

            services.AddControllers();
          
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "ApiManagerStudent", Version = "v1" });
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "This site uses Bearer token and you have to pass" +
                   "it as Bearer<<space>>Token",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer"
                });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement()
                {
                    {
                    new OpenApiSecurityScheme
                    {
                        Reference=new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id="Bearer"
                        },
                        Scheme="oauth2",
                        Name="Bearer",
                        In = ParameterLocation.Header
                    },
                    new List<string>()
                    }
                });
            });
            var jwtKey = Configuration.GetValue<string>("JwtSettings:Key");
            var keyBytes = Encoding.ASCII.GetBytes(jwtKey);

            TokenValidationParameters tokenValidation = new TokenValidationParameters
            {
                IssuerSigningKey = new SymmetricSecurityKey(keyBytes),
                ValidateLifetime = true,
                ValidateAudience = false,
                ValidateIssuer = false,
                ClockSkew = TimeSpan.Zero
            };

            services.AddSingleton(tokenValidation);

            services.AddAuthentication(authOptions =>
            {
                authOptions.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                authOptions.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
                .AddJwtBearer(jwtOptions =>
                {
                    jwtOptions.TokenValidationParameters = tokenValidation;
                    jwtOptions.Events = new JwtBearerEvents();
                    jwtOptions.Events.OnTokenValidated = async (context) =>
                    {
                        var ipAddress = context.Request.HttpContext.Connection.RemoteIpAddress.ToString();
                        var jwtService = context.Request.HttpContext.RequestServices.GetService<IJwtService>();
                        var jwtToken = context.SecurityToken as JwtSecurityToken;
                        if (!await jwtService.IsTokenValid(jwtToken.RawData, ipAddress))
                            context.Fail("Invalid Token Details");


                    };
                });

            services.AddTransient<IJwtService, JwtService>();
            services.AddCors(options =>
            {
                options.AddPolicy("MyPolicy", builder =>
                {
                    builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
                });
            });
            services.AddDbContext<ManagerStudentContext>(options =>
            options.UseSqlServer(Configuration.GetConnectionString("ManagerStudentDB")));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment()||env.IsProduction())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    c.RoutePrefix = string.Empty;
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "ApiManagerStudent v1");
                });
            }
            app.UseStaticFiles(new StaticFileOptions { 
                FileProvider=new PhysicalFileProvider(Path.Combine(env.ContentRootPath,"Images")),
                RequestPath="/Images"
            });
            app.UseHttpsRedirection();


            app.UseRouting();
            app.UseCors();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
