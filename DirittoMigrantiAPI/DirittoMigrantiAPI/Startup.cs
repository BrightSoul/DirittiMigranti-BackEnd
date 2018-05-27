﻿using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using DirittoMigrantiAPI.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;

namespace DirittoMigrantiAPI
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            //Usa un db interno
            services.AddDbContext<UserContext>(options =>
                options.UseInMemoryDatabase("UserList"));
            //options.ValidateScopes = false



            //Configurare il middleware di autenticazione di ASP.NET Core per supportare i token JWT
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateIssuerSigningKey = true,

                    //Importante: indicare lo stesso Issuer, Audience e chiave segreta
                    //usati anche nel JwtTokenMiddleware
                    ValidIssuer = "Issuer",
                    ValidAudience = "Audience",
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(Program.key)
                  ),
                    //Tolleranza sulla data di scadenza del token
                    ClockSkew = TimeSpan.Zero
                };
            });

            services.AddMvc();
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, IServiceProvider serviceProvider)
        {
            app.UseMiddleware<JwtTokenMiddleware>();
            app.UseAuthentication();


            //DEBUG
            var context = serviceProvider.GetRequiredService<UserContext>();
            AddTestData(context);

            app.UseMvc();
        }

        private static void AddTestData(UserContext context)
        {
            var testUser1 = new User
            {
                Username = "utente1",
                Password = "utente1"
            };
            context.Users.Add(testUser1);

            var testUser2 = new User
            {
                Username = "utente2",
                Password = "utente2"
            };
            context.Users.Add(testUser2);

            context.SaveChanges();
        }
    }
}
