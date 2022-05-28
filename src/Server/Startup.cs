using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Server.Data;

namespace Server
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }
        public void ConfigureServices(IServiceCollection services)
        {
            var assembly = typeof(Program).Assembly.GetName().Name;
            var mDefaultServerConnection = Configuration.GetConnectionString("DefaultServerConnection");

            SeedData.EnsureSeedData(mDefaultServerConnection);            

            services.AddDbContext<AspNetIdentityDbContext>(options => options.UseSqlServer(mDefaultServerConnection, opt => opt.MigrationsAssembly(assembly)));

            services.AddIdentity<IdentityUser, IdentityRole>().AddEntityFrameworkStores<AspNetIdentityDbContext>();

            services.AddIdentityServer()
                .AddAspNetIdentity<IdentityUser>()
                .AddConfigurationStore(option =>
                {
                    option.ConfigureDbContext = b => b.UseSqlServer(mDefaultServerConnection, opt => opt.MigrationsAssembly(assembly));
                })
                .AddOperationalStore( option =>
                {
                    option.ConfigureDbContext = b => b.UseSqlServer(mDefaultServerConnection, opt => opt.MigrationsAssembly(assembly));
                })
                //.AddInMemoryClients(IdentityConfiguration.Clients)
                //.AddInMemoryIdentityResources(IdentityConfiguration.IdentityResources)
                //.AddInMemoryApiResources(IdentityConfiguration.ApiResources)
                //.AddInMemoryApiScopes(IdentityConfiguration.ApiScopes)
                //.AddTestUsers(IdentityConfiguration.TestUsers)
                .AddDeveloperSigningCredential();

            services.AddControllersWithViews();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseStaticFiles();

            app.UseRouting();

            app.UseIdentityServer();

            app.UseAuthorization();

            app.UseAuthentication();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapDefaultControllerRoute();

            });
        }
    }
}
