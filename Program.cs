using BlogApp.Areas.Identity.Data;
using BlogApp.Data;
using BlogApp.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;

namespace BlogApp
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            var connectionString =
                builder.Configuration.GetConnectionString("defaultconnection")
                ?? throw new InvalidOperationException("Connection string not found.");
            if (connectionString == null)
            {
                System.Diagnostics.Trace.TraceError(
                    "If you're seeing this, connectiontring is not exits"
                );
            }

            builder.Services.AddDbContext<BlogAppContext>(
                options => options.UseSqlite(connectionString)
            );

            builder.Services
                .AddDefaultIdentity<BlogAppUser>(
                    options => options.SignIn.RequireConfirmedAccount = false
                )
                .AddRoles<IdentityRole>()
                .AddEntityFrameworkStores<BlogAppContext>();

            // Add services to the container.
            builder.Services.AddLogging(logging =>
            {
                logging.ClearProviders();
                logging.AddConsole();
                logging.AddDebug();
                logging.SetMinimumLevel(LogLevel.Debug);
            });

            builder.Services.AddTransient<IEmailSender, EmailSender>();

            builder.Services.AddControllersWithViews();

            var app = builder.Build();

            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                await SeedRole.InitializeAsync(services);
                await SeedUser.InitializeAsync(services);
            }

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}"
            );
            app.MapRazorPages();

            app.Run();
        }
    }
}
