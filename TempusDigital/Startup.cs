using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using BLL.Helper;
using BLL.Services;
using BOL.Interfaces.Repositories;
using BOL.Interfaces.Services;
using DAL;
using DAL.Repositories;





namespace TempusDigital
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
            services.AddControllersWithViews();

            services.AddScoped<DbContext, TempusDigitalContext>();

            services.AddScoped<IClienteRepository, ClienteRepository>();

            services.AddScoped<IClienteService, ClienteService>();

            services.AddDbContext<TempusDigitalContext>(options => 
                options.UseSqlServer(Configuration.GetConnectionString("Default"))
            );

            
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IServiceProvider serviceProvider)
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
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });


            // New Class

            serviceProvider.GetService<TempusDigitalContext>()
                .Database.EnsureCreated();

            var clientList = FileJson.FileJsonToClienteDb();

            var clienteRepo = serviceProvider.GetService<IClienteRepository>();
                if (!clienteRepo.FindIfCpfIsUse(clientList[0].CPF))
                {
                    clienteRepo.AddRange(clientList.ToListClienteEntity());
                }




        }
    }
}
