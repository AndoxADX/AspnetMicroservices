using AspnetRunBasics.HttpHandlers;
using AspnetRunBasics.Services;
using AspnetRunBasics.Utils;
using IdentityModel;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Net.Http.Headers;
using System;

namespace AspnetRunBasics
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
            RegisterServices(services);

            ConfigureAuthentication(services);

            ConfigureHttpClient(services);

            services.AddRazorPages();
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
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
            });
        }

        #region Private Methods
        private void RegisterServices(IServiceCollection services)
        {
            services.AddScoped<ICatalogService, CatalogService>();
            services.AddScoped<IBasketService, BasketService>();
            services.AddScoped<IOrderService, OrderService>();

            //services.AddHttpClient<ICatalogService, CatalogService>(c =>
            //                c.BaseAddress = new Uri(Configuration["ApiSettings:GatewayAddress"]));

            //services.AddHttpClient<IBasketService, BasketService>(c =>
            //    c.BaseAddress = new Uri(Configuration["ApiSettings:GatewayAddress"]));

            //services.AddHttpClient<IOrderService, OrderService>(c =>
            //    c.BaseAddress = new Uri(Configuration["ApiSettings:GatewayAddress"]));
        }

        private void ConfigureAuthentication(IServiceCollection services)
        {
            services.AddAuthentication(options =>
            {
                options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
            })
            .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme)
            .AddOpenIdConnect(OpenIdConnectDefaults.AuthenticationScheme, options =>
            {
                options.Authority = Configuration["IdentityServer:BaseUrl"];
                options.ClientId = "shop_mvc_client";
                options.ClientSecret = "secret";
                options.ResponseType = "code id_token";
                //options.Scope.Add("profile");
                //options.Scope.Add("openid");
                options.Scope.Add("roles");
                options.Scope.Add("email");
                options.Scope.Add("address");
                options.Scope.Add("catalogAPI");
                options.Scope.Add("basketAPI");
                options.Scope.Add("discountAPI");
                options.Scope.Add("orderingAPI");

                options.ClaimActions.MapUniqueJsonKey("role", "role");

                options.SaveTokens = true;
                options.GetClaimsFromUserInfoEndpoint = true;

                options.TokenValidationParameters = new TokenValidationParameters
                {
                    NameClaimType = JwtClaimTypes.GivenName,
                    RoleClaimType = JwtClaimTypes.Role
                };
            });
        } 

        private void ConfigureHttpClient(IServiceCollection services)
        {
            services.AddTransient<AuthenticationDelegatingHandler>();

            services.AddHttpClient(IdentityClient.ShopAPIClient, client =>
            {
                client.BaseAddress = new Uri(Configuration["ApiSettings:GatewayAddress"]);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Add(HeaderNames.Accept, "application/json");
            }).AddHttpMessageHandler<AuthenticationDelegatingHandler>();

            services.AddHttpClient(IdentityClient.IDPClient, client =>
            {
                client.BaseAddress = new Uri(Configuration["IdentityServer:BaseUrl"]);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Add(HeaderNames.Accept, "application/json");
            });

            services.AddHttpContextAccessor();
        }
        #endregion
    }
}
