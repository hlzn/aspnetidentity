using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.SqlServer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using mvc.Models;
using mvc.Security;
using ScottBrady91.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication;
using mvc.Security.Keys;

namespace mvc
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
            var connectionString = @"Data Source=.;database=dummy.MvcUser;trusted_connection=yes";
            var migrationAssembly = typeof(Startup).GetTypeInfo().Assembly.GetName().Name;
            services.AddControllersWithViews();
            services.AddDbContext<MvcUserDbContext>(opt => opt.UseSqlServer(connectionString, sql => sql.MigrationsAssembly(migrationAssembly)));
            services.AddIdentity<MvcUser, IdentityRole>(options => {
                        //options.SignIn.RequireConfirmedEmail = true;
                        options.Tokens.EmailConfirmationTokenProvider = "emailConfirmation";
                        options.Password.RequiredLength = 11;
                        options.User.RequireUniqueEmail = true;

                        options.Lockout.AllowedForNewUsers = true;
                        options.Lockout.MaxFailedAccessAttempts = 3;
                        options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(10);
                    })
                    .AddEntityFrameworkStores<MvcUserDbContext>()
                    .AddDefaultTokenProviders()
                    .AddTokenProvider<EmailConfirmationTokenProvider<MvcUser>>("emailConfirmation")
                    .AddPasswordValidator<DoesNotContainPasswordValidator<MvcUser>>();

            services.AddScoped<IPasswordHasher<MvcUser>, BCryptPasswordHasher<MvcUser>>();

            services.AddScoped<IUserStore<MvcUser>, UserOnlyStore<MvcUser, MvcUserDbContext>>();
            //services.AddAuthentication("cookies").AddCookie("cookies", options => options.LoginPath = "/Home/Login");
            services.AddScoped<IUserClaimsPrincipalFactory<MvcUser>, MvcUserClaimsPrincipalFactory>();

            services.Configure<DataProtectionTokenProviderOptions>(options => options.TokenLifespan = TimeSpan.FromHours(3));
            services.Configure<EmailConfirmationTokenProviderOptions>(options => options.TokenLifespan = TimeSpan.FromDays(2));

            services.Configure<BCryptPasswordHasherOptions>(options => {
                options.WorkFactor = 10;
                options.EnhancedEntropy = false;
            });
            services.ConfigureApplicationCookie(options => options.LoginPath = "/Home/Login");

            services.AddAuthentication()
                    .AddGoogle("google", options => {
                        options.ClientId = ConfigurationManager.AppSetting["GoogleKeys:ClientID"];
                        options.ClientSecret = ConfigurationManager.AppSetting["GoogleKeys:ClientSecret"];
                        options.SignInScheme = IdentityConstants.ExternalScheme;
                    });

            // todo: configure security stamp cookie
            // services.AddCookie(IdentityConstants.ApplicationScheme, o => {
            //     o.LoginPath = new PathString("/Account/Login");
            //     o.Events = new CookieAuthenticationEvents {
            //         OnValidatePrincipal => SecurityStampValidator.ValidatePrincipalAsync
            //     };
            // });
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
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseAuthentication();

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
                // endpoints.MapControllerRoute(
                //     name: "api",
                //     pattern: "api/{controller}/{action}");
            });
        }
    }
}
