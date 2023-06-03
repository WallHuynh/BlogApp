using BlogApp.Areas.Identity.Data;
using Microsoft.AspNetCore.Identity;

namespace BlogApp.Data
{
    public class SeedUser
    {
        public static async Task InitializeAsync(IServiceProvider serviceProvider)
        {
            var userManager = serviceProvider.GetRequiredService<UserManager<BlogAppUser>>();

            string adminEmail = Environment.GetEnvironmentVariable("adminEmail");
            string adminPassword = Environment.GetEnvironmentVariable("adminPassword");
            string managerEmail = Environment.GetEnvironmentVariable("managerEmail");
            string managerPassword = Environment.GetEnvironmentVariable("managerPassword");
            string readerEmail = Environment.GetEnvironmentVariable("readerEmail");
            string readerPassword = Environment.GetEnvironmentVariable("readerPassword");

            if (adminEmail != null && await userManager.FindByEmailAsync(adminEmail) == null)
            {
                var admin = new BlogAppUser();
                admin.FirstName = "Tuong";
                admin.LastName = "Huynh";
                admin.UserName = adminEmail;
                admin.Email = adminEmail;
                await userManager.CreateAsync(admin, adminPassword);
                await userManager.AddToRoleAsync(admin, "Admin");
            }

            if (managerEmail != null && await userManager.FindByEmailAsync(managerEmail) == null)
            {
                var manager = new BlogAppUser();
                manager.FirstName = "Thu";
                manager.LastName = "Huynh";
                manager.UserName = managerEmail;
                manager.Email = managerEmail;
                await userManager.CreateAsync(manager, managerPassword);
                await userManager.AddToRoleAsync(manager, "Manager");
            }
            if (readerEmail != null && await userManager.FindByEmailAsync(readerEmail) == null)
            {
                var reader = new BlogAppUser();
                reader.FirstName = "Reader";
                reader.LastName = "Test";
                reader.UserName = readerEmail;
                reader.Email = readerEmail;
                await userManager.CreateAsync(reader, readerPassword);
                await userManager.AddToRoleAsync(reader, "Reader");
            }
        }
    }
}
