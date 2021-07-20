using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using myproject.Models;
using myproject.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace myproject
{
    public class Startup
    {
        private IConfiguration _config;

        public Startup(IConfiguration config)
        {
            _config = config;
        }
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContextPool<AppDbContext>(options => options.UseSqlServer(_config.GetConnectionString("EmployeeDbConnection")));
            // services.AddMvcCore(options => options.EnableEndpointRouting = false);
            services.AddIdentity<ApplicationUser, IdentityRole>(options=> 
            {
                options.Password.RequiredLength = 10;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequiredUniqueChars = 3;

                options.SignIn.RequireConfirmedEmail = true;
                options.Tokens.EmailConfirmationTokenProvider = "CustomEmailConfirmation";
                options.Lockout.MaxFailedAccessAttempts = 5;
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
            })
                .AddEntityFrameworkStores<AppDbContext>()
                .AddDefaultTokenProviders()
                .AddTokenProvider<CustomEmailConfirmationTokenProvider<ApplicationUser>>("CustomEmailConfirmation");
            services.Configure<DataProtectionTokenProviderOptions>(o => o.TokenLifespan = TimeSpan.FromHours(5));
            services.Configure<CustomEmailConfirmationTokenProviderOptions>(o => o.TokenLifespan = TimeSpan.FromDays(3));

            services.AddMvc(options => 
            {
                var policy = new AuthorizationPolicyBuilder()
                                 .RequireAuthenticatedUser()
                                 .Build();
                options.Filters.Add(new AuthorizeFilter(policy));
                options.EnableEndpointRouting = false;
            });
            services.AddAuthentication()
                .AddGoogle(options =>
                {
                    options.ClientId = "858926484916-l02pc8i1vpn54gn23p5mdkp34p6rcnmo.apps.googleusercontent.com";
                    options.ClientSecret = "l7fDMy5FAn0ElvTKRG2WPnI-";
                })
                .AddFacebook(options =>
                {
                    options.AppId = "111715667673972";
                    options.AppSecret = "1102f395fc437eb8a01b6d8365fc08fd";
                });
            services.ConfigureApplicationCookie(options =>
            {
                options.AccessDeniedPath = new PathString("/Administration/AccessDenied");
            });
            services.AddAuthorization(options =>
            {
                options.AddPolicy("DeleteOptionPolicy", policy => policy.RequireClaim("Delete Role"));
                //options.AddPolicy("EditRolePolicy", policy => policy.RequireAssertion(context =>
                //                                    context.User.IsInRole("Admin") &&
                //                                    context.User.HasClaim(claim => claim.Type == "Edit Role" && claim.Value == "true") ||
                //                                    context.User.IsInRole("Super Admin")));
                options.AddPolicy("EditRolePolicy", policy => policy.AddRequirements(new ManageAdminRoleAndClaimRequirement()));
                options.AddPolicy("AdminRolePolicy", policy => policy.RequireRole("Admin"));
            });
           // services.AddControllersWithViews();
            services.AddScoped<IEmployeeRepository, SQLEmployeeRepository>();
            services.AddSingleton<IAuthorizationHandler, CanEditOtherAdminRoleAndClaimsHandler>();
            services.AddSingleton<IAuthorizationHandler, SuperAdminHandler>();
            services.AddSingleton<DataProtectionPurposestring>();
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
                app.UseStatusCodePagesWithReExecute("/Error/{0}");
            }
            app.UseStaticFiles();
            //app.UseMvcWithDefaultRoute();
            app.UseAuthentication();
            app.UseRouting();
            app.UseMvc(routes =>
            {
                routes.MapRoute("default", "{controller=Home}/{action=Index}/{id?}");
            });
            //app.Run(async context =>
            //{
            //    await context.Response.WriteAsync("Hello World!");
            //});
            //app.UseEndpoints(endpoints =>
            //{
            //    endpoints.MapGet("/", async (context) =>
            //    {

            //        await context.Response.WriteAsync("Hello world!");

            //    });

            //});


        }
    }
}
