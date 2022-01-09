using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Net.Http.Headers;

namespace MyWeb.API
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

            //Tüm istekleri kabul et
            //services.AddCors(opt =>
            //{
            //    opt.AddDefaultPolicy(builder =>
            //    {
            //        builder.AllowAnyOrigin()
            //            .AllowAnyHeader()
            //            .AllowAnyMethod();
            //    });
            //});


            ////Ýzin Verilen domainler gelebilir
            //services.AddCors(opt =>
            //{
            //    opt.AddPolicy("AllowSites", builder =>
            //    {
            //        builder.WithOrigins("https://localhost:44320", "https://www.aykutgursel.com")
            //            .AllowAnyHeader()
            //            .AllowAnyMethod();
            //    });

            //    opt.AddPolicy("AllowSites2", builder =>
            //    {
            //        builder.WithOrigins("https://www.blog.aykutgursel.com")
            //            .WithHeaders(HeaderNames.ContentType, "x-custom-header");
            //    });
            //});


            ////Ýzin Verilen domainler gelebilir
            services.AddCors(opt =>
            {
                opt.AddPolicy("AllowSites1", builder =>
                {
                    builder.WithOrigins("https://*.aykutgursel.com")
                        .SetIsOriginAllowedToAllowWildcardSubdomains()
                        .AllowAnyHeader()
                        .AllowAnyMethod();
                    
                });

                //Ýzin Verilen domain  izin verilen method tiplerine gelebilir
                opt.AddPolicy("AllowSites2", builder =>
                {
                    builder.WithOrigins("https://localhost:44320")
                        .WithMethods("POST", "GET")
                        .AllowAnyHeader();
                });
            });

            services.AddControllers();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();
            //app.UseCors();
            //app.UseCors("AllowSites");
            //app.UseCors("AllowSites2");
            app.UseCors();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
