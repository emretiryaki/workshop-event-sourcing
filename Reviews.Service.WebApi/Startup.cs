using System;
using System.Threading.Tasks;
using EventStore.ClientAPI;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Reviews.Core.EventStore;
using Reviews.Core;
using Reviews.Service.WebApi.Modules.Reviews;
using Swashbuckle.AspNetCore.Swagger;


namespace Reviews.Service.WebApi
{
    public class Startup
    {
        public Startup(IHostingEnvironment environment, IConfiguration configuration)
        {
            Environment = environment;
            Configuration = configuration;
        }

        private IConfiguration Configuration { get; }
        private IHostingEnvironment Environment { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            ConfigureServicesAsync(services).GetAwaiter().GetResult();
            var a = 1;
        }
        
        
        private async Task ConfigureServicesAsync(IServiceCollection services)
        {
            //Building Event store components
            BuildEventStore(services);
            
            services
                .AddMvc()
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
            
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc(
                    $"v{Configuration["Swagger:Version"]}", 
                    new Info {
                        Title   = Configuration["Swagger:Title"], 
                        Version = $"v{Configuration["Swagger:Version"]}"
                    });
            });
        }

        private void RegisterDependecies(IServiceCollection service)
        {
        }

        private async Task BuildEventStore(IServiceCollection services)
        {
            //Create EventStore Connection
            var gesConnection = EventStoreConnection.Create(
                Configuration["EventStore:ConnectionString"],
                ConnectionSettings.Create()
                    .KeepReconnecting()
                    .EnableVerboseLogging()
                    .SetHeartbeatInterval(TimeSpan.FromMilliseconds(5 * 1000))
                    .UseDebugLogger(),
                Environment.ApplicationName
            );
            
            gesConnection.Connected += (sender, args) 
                => Console.WriteLine($"Connection to {args.RemoteEndPoint} event store established.");
            
            gesConnection.ErrorOccurred += (sender, args) 
                => Console.WriteLine($"Connection error : {args.Exception}");
            
            await gesConnection.ConnectAsync();
            
            
            var serializer = new JsonNetSerializer();

            var eventMapper = new EventTypeMapper()
                .Map<Domain.Events.V1.ReviewCreated>("reviewCreated")
                .Map<Domain.Events.V1.CaptionAndContentChanged>("reviewUpdated")
                .Map<Domain.Events.V1.ReviewPublished>("reviewPublished")
                .Map<Domain.Events.V1.ReviewApproved>("reviewApproved");

            var aggregateStore = new GesAggrigateStore(
                gesConnection, 
                serializer, 
                eventMapper,
                (type, id) => $"{type.Name}-{id}", 
                null);
            
            services.AddSingleton(new ApplicationService(aggregateStore));   
        }
        
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            
            app.UseMvcWithDefaultRoute();
            app.UseSwagger();
            app.UseSwaggerUI(options => options.SwaggerEndpoint(
                Configuration["Swagger:Endpoint:Url"], 
                Configuration["Swagger:Endpoint:Name"]));
            
            app.UseMvc();
        }
    }
}
