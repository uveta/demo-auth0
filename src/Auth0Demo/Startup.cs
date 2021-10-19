using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.SpaServices.ReactDevelopmentServer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;

namespace Auth0Demo
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

            services.AddControllersWithViews();

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultSignInScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
                options.Authority = "https://uveta-demo-auth0.eu.auth0.com/";
                options.Audience = "https://demo/api";
                options.RequireHttpsMetadata = false;
            });

            services.AddAuthorization(authorization =>
            {
                authorization.AddPolicy(Policies.OrdersFull, policy =>
                {
                    policy.RequireClaim("scope", "orders:full");
                });
                authorization.AddPolicy(Policies.OrdersRead, policy =>
                {
                    policy.RequireAssertion(context =>
                    {
                        var user = context.User;
                        var claims = user?.Claims;
                        if (claims is null) return false;
                        return claims
                            .Where(c => c.Type == "scope")
                            .Any(c => c.Value == "orders:full" || c.Value == "orders:read");
                    });
                });
            });

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Auth0 Demo API", Version = "v1" });
                c.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
                {
                    Type = SecuritySchemeType.OAuth2,
                    Flows = new OpenApiOAuthFlows()
                    {
                        Implicit = new OpenApiOAuthFlow()
                        {
                            AuthorizationUrl = new Uri("https://uveta-demo-auth0.eu.auth0.com/authorize"),
                            Scopes = new Dictionary<string, string>
                            {
                                ["orders:read"] = "Read orders",
                                ["orders:full"] = "Full access to orders"
                            }
                        }
                    }
                });
                c.OperationFilter<SecurityRequirementsOperationFilter>();
            });

            // In production, the React files will be served from this directory
            services.AddSpaStaticFiles(configuration =>
            {
                configuration.RootPath = "ClientApp/build";
            });
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
                app.UseExceptionHandler("/Error");
            }

            app.UseStaticFiles();
            app.UseSpaStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("v1/swagger.json", "Auth0 Demo API V1");
                c.OAuthClientId("HvZbHkgYg17ZyU4bZ8KgYVC4i7tChnP9");
                c.OAuth2RedirectUrl("http://localhost:5000/oauth2-redirect.html");
            });

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapDefaultControllerRoute();
            });

            app.UseSpa(spa =>
            {
                spa.Options.SourcePath = "ClientApp";

                if (env.IsDevelopment())
                {
                    spa.UseReactDevelopmentServer(npmScript: "start");
                }
            });
        }
    }
}
