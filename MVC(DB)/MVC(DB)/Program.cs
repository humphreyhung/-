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
            builder.Services.AddControllersWithViews(options =>
            {
                options.SuppressImplicitRequiredAttributeForNonNullableReferenceTypes = true;
            });

            var app = builder.Build();

            // Configure the HTTP request pipeline
            if (app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler(errorApp =>
                {
                    errorApp.Run(async context =>
                    {
                        var exceptionHandlerPathFeature = context.Features.Get<IExceptionHandlerPathFeature>();
                        var exception = exceptionHandlerPathFeature?.Error;

                        context.Response.StatusCode = 500;
                        context.Response.ContentType = "text/html";
                        await context.Response.WriteAsync("<html><body style='padding: 20px; font-family: Arial, sans-serif;'>\r\n");
                        await context.Response.WriteAsync($"<h2 style='color: #d32f2f;'>Error: {exception?.Message}</h2><br>\r\n");
                        
                        if (exception is SqlException sqlException)
                        {
                            await context.Response.WriteAsync($"<div style='background-color: #fff3f3; padding: 15px; border-radius: 4px; margin: 10px 0;'>\r\n");
                            await context.Response.WriteAsync($"<h3 style='color: #d32f2f;'>Database Error Details:</h3>\r\n");
                            await context.Response.WriteAsync($"<p><strong>Error Number:</strong> {sqlException.Number}</p>\r\n");
                            await context.Response.WriteAsync($"<p><strong>Error State:</strong> {sqlException.State}</p>\r\n");
                            await context.Response.WriteAsync($"<p><strong>Error Class:</strong> {sqlException.Class}</p>\r\n");
                            await context.Response.WriteAsync($"</div>\r\n");
                        }

                        if (exception != null)
                        {
                            await context.Response.WriteAsync("<div style='background-color: #f5f5f5; padding: 15px; border-radius: 4px; margin: 10px 0;'>\r\n");
                            await context.Response.WriteAsync("<h3>Stack Trace:</h3>\r\n");
                            await context.Response.WriteAsync($"<pre style='overflow-x: auto; background-color: #fff; padding: 10px; border-radius: 4px;'>{exception.StackTrace}</pre>\r\n");
                            await context.Response.WriteAsync("</div>\r\n");
                        }
                        
                        await context.Response.WriteAsync("<br><a href='/' style='display: inline-block; padding: 10px 20px; background-color: #1976d2; color: white; text-decoration: none; border-radius: 4px;'>Back to Home</a><br>\r\n");
                        await context.Response.WriteAsync("</body></html>\r\n");
                    });
                });
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();
            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}
