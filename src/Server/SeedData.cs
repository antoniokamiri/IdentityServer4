using IdentityModel;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Mappers;
using IdentityServer4.EntityFramework.Storage;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Server.Data;
using System.Linq;
using System.Security.Claims;

namespace Server
{
    public class SeedData
    {
        public static void EnsureSeedData(string mConnectionString)
        {
            var service = new ServiceCollection();
            service.AddLogging();
            service.AddDbContext<AspNetIdentityDbContext> (options => options.UseSqlServer(mConnectionString));

            service.AddIdentity<IdentityUser, IdentityRole>().AddEntityFrameworkStores<AspNetIdentityDbContext>().AddDefaultTokenProviders();

            service.AddOperationalDbContext(options =>
            {
                options.ConfigureDbContext = db => db.UseSqlServer(mConnectionString, sql => sql.MigrationsAssembly(typeof(SeedData).Assembly.FullName));
            });

            service.AddConfigurationDbContext(options =>
            {
                options.ConfigureDbContext = db => db.UseSqlServer(mConnectionString, sql => sql.MigrationsAssembly(typeof(SeedData).Assembly.FullName));
            });

            var serviceProvider = service.BuildServiceProvider();

            using var scope = serviceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope();

            var context = scope.ServiceProvider.GetService<ConfigurationDbContext>();
            context.Database.Migrate();

            EnsureSeedData(context);

            var ctx = scope.ServiceProvider.GetService<AspNetIdentityDbContext>();
            ctx.Database.Migrate();

            EnsureUsers(scope);

        }

        public static void EnsureUsers (IServiceScope scope)
        {
            var userMgnt = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();

            var user = userMgnt.FindByNameAsync("antony").Result;

            if (user == null)
            {
                user = new IdentityUser
                {
                    UserName = "Antony",
                    Email = "antonicamwangi@gmail.com",
                    EmailConfirmed = true,
                };

                var result = userMgnt.CreateAsync(user, "P@55w0rd").Result;

                if(!result.Succeeded)
                {
                    throw new System.Exception(result.Errors.First().Description);
                }

                result = userMgnt.AddClaimsAsync(user, new Claim[]
                {
                    new Claim(JwtClaimTypes.Name, "Antony kamiri"),
                    new Claim(JwtClaimTypes.GivenName, "Antony"),
                    new Claim(JwtClaimTypes.FamilyName, "kamiri"),
                    new Claim(JwtClaimTypes.WebSite, "http://Antonykamiri.com"),
                    new Claim("Location", "Nairobi")
                }).Result;

                if(!result.Succeeded)
                {
                    throw new System.Exception(result.Errors.First().Description);
                }
            }
        }

        public static void EnsureSeedData(ConfigurationDbContext context)
        {

            if (!context.Clients.Any())
            {
                foreach (var client in Config.Clients.ToList())
                {
                    context.Clients.Add(client.ToEntity());
                }
                context.SaveChanges();
            }

            if (!context.IdentityResources.Any())
            {
                foreach (var resource in Config.IdentityResources.ToList())
                {
                    context.IdentityResources.Add(resource.ToEntity());
                }
                context.SaveChanges();
            }

            if (!context.ApiResources.Any())
            {
                foreach (var resource in Config.ApiResources)
                {
                    context.ApiResources.Add(resource.ToEntity());
                }
                context.SaveChanges();
            }

            if (!context.ApiScopes.Any())
            {
                foreach (var resource in Config.ApiScopes)
                {
                    context.ApiScopes.Add(resource.ToEntity());
                }
                context.SaveChanges();
            }
        }
    }
}
