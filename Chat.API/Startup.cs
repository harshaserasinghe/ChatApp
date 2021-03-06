using Chat.Common.Models;
using Chat.Service.Interfaces;
using Chat.Service.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;

namespace Chat.API
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

            services.Configure<CosmoDBConfig>(Configuration.GetSection("AzureCosmosDB"));
            services.Configure<AzureServiceBusConfig>(Configuration.GetSection("AzureServiceBus"));

            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Chat.API", Version = "v1" });
            });

            services.AddScoped<ISupportService, SupportService>();
            services.AddScoped<IAgentService, AgentService>();
            services.AddScoped<IAzureServiceBusService, AzureServiceBusService>();
            services.AddSingleton<IAzureRedisService, AzureRedisService>();
            services.AddScoped<ICosmosDBService, CosmosDBService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Chat.API v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
