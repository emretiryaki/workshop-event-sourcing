using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Raven.Client.Documents;
using Swashbuckle.AspNetCore.Swagger;

namespace Reviews.Service.QueryApi
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
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1); services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc(
                    $"v{Configuration["Swagger:Version"]}", 
                    new Info {
                        Title   = Configuration["Swagger:Title"], 
                        Version = $"v{Configuration["Swagger:Version"]}"
                    });
            });
            
            
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseSwagger();
            app.UseSwaggerUI(options => options.SwaggerEndpoint(
                Configuration["Swagger:Endpoint:Url"], 
                Configuration["Swagger:Endpoint:Name"]));
            app.UseMvc();

            BuildRevenDb(env);
        }
        
        
         private IDocumentStore BuildRevenDb(IHostingEnvironment env)
        {
            var store = new DocumentStore {
                Urls     = new[] {Configuration["RavenDb:Url"]},
                Database = Configuration["RavenDb:Database"]
            };
            
            if (env.IsDevelopment()) store.OnBeforeQuery += (_, args) 
                => args.QueryCustomization.WaitForNonStaleResults();

            try 
            {
                store.Initialize();                
                Console.WriteLine($"Connection to {store.Urls[0]} document store established.");
            }
            catch (Exception ex)
            {
                throw new ApplicationException(
                    $"Failed to establish connection to \"{store.Urls[0]}\" document store!" +
                    $"Please check if https is properly configured in order to use the certificate.", ex);
            }
            
            return store;
            
        }
    }
}
