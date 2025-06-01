using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Http;
using MVC_DB_.Services;
using MVC_DB_.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.AspNetCore.Diagnostics;

namespace MVC_DB_
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // 1) 注册 EF Core DbContext
            //builder.Services.AddDbContext<ApplicationDbContext>(options =>
            //    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            // 2) 注册 Identity（如有需要）
            // builder.Services.AddDefaultIdentity<IdentityUser>(options => { /* ... */ })
            //                 .AddEntityFrameworkStores<ApplicationDbContext>();

            // 3) 注册你的 CampaignService
            builder.Services.AddScoped<ICampaignService, CampaignService>();
            builder.Services.AddScoped<DBmanager>();
            builder.Services.AddSession();

            builder.Services.AddControllersWithViews(options =>
            {
                options.SuppressImplicitRequiredAttributeForNonNullableReferenceTypes = true;
            })
            .AddRazorRuntimeCompilation();

            var app = builder.Build();

            // Configure the HTTP request pipeline
            if (app.Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();
            app.UseSession();
            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=index_R}/{id?}");

            app.Run();
        }
    }
}
