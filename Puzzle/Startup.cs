using Hubs;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Models;
using Puzzle.Data;
using Puzzle.Helper;
using Puzzle.Models;
using Puzzle.Repository;
using Repository;
using System;
using System.Linq;

namespace Puzzle
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
            //services.AddDbContext<PuzzleDbContext>(options =>
            //    options.UseSqlServer(
            //        Configuration.GetConnectionString("DefaultConnection")));

            //services.AddDatabaseDeveloperPageExceptionFilter();

            //services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
            //    .AddEntityFrameworkStores<PuzzleDbContext>();

            services.AddDbContext<PuzzleDbContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

            services.AddIdentity<User, IdentityRole>()
                .AddEntityFrameworkStores<PuzzleDbContext>()
                .AddDefaultTokenProviders();

            services.AddTransient<MediaRepository>();

            services.AddHostedService<TimedHostedService>();

            services.Configure<IdentityOptions>(options =>
            {
                options.Password.RequireDigit = false;
                options.Password.RequiredLength = 6;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireLowercase = false;
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
                options.Lockout.MaxFailedAccessAttempts = 5;
                options.Lockout.AllowedForNewUsers = true;
            });

            services.ConfigureApplicationCookie(options =>
            {
                options.SlidingExpiration = true;
                options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
            });

            services.AddSignalR();

            services.AddControllersWithViews().AddRazorRuntimeCompilation()
                .AddNewtonsoftJson(options => options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseMigrationsEndPoint();

            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();

                Setting.IsDevelopment = false;
            }

            //app.UseHttpsRedirection();

            app.UseStaticFiles(new StaticFileOptions
            {
                ServeUnknownFileTypes = true,
                OnPrepareResponse = ctx =>
                {
                    // Cache static files for 30 days
                    ctx.Context.Response.Headers.Append("Cache-Control", "public,max-age=2592000");
                }
            });

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            //var db = new PuzzleDbContext();

            //var project = db.Project
            //    .Where(x => x.Media.Any())
            //    .Include(x => x.Media)
            //    .ToList();

            //foreach (var item in project)
            //{
            //    foreach (var group in item.Media.GroupBy(x => x.Description))
            //    {
            //        if (string.IsNullOrWhiteSpace(group.Key) == false)
            //        {
            //            db.Conversation.Add(new Conversation
            //            {
            //                Accepted = true,
            //                CreatedAt = group.OrderBy(x => x.CreatedAt).First().CreatedAt.AddSeconds(-1),
            //                Message = group.Key,
            //                ProjectId = item.Id,
            //                Role = Enums.Enum.Role.Designer,
            //                UserId = item.DesignerId
            //            });
            //        }

            //        foreach (var m in group.OrderBy(x => x.CreatedAt))
            //        {
            //            db.Conversation.Add(new Conversation
            //            {
            //                Accepted = true,
            //                CreatedAt = m.CreatedAt,
            //                ProjectId = item.Id,
            //                Role = Enums.Enum.Role.Designer,
            //                UserId = item.DesignerId,
            //                Type = m.Type,
            //                Attachment = m.Url,
            //                Message = group.Key
            //            });
            //        }
            //    }
            //}

            //db.SaveChanges();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");

                endpoints.MapHub<SignalRHub>("/notif");
                endpoints.MapHub<ChatHub>("/conversation");
            });
        }
    }
}
