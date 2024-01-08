using COLID.Exception;
using COLID.Identity;
using COLID.StatisticsLog;
using COLID.Swagger;
using COLID.ResourceRelationshipManager.Services;
using COLID.ResourceRelationshipManager.Repositories;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Serialization;
using System.Net.Http;

namespace COLID.ResourceRelationshipManager
{
    /// <summary>
    /// 
    /// </summary>
    public class Startup
    {
        /// <summary>
        /// 
        /// </summary>
        public IConfiguration Configuration { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public IWebHostEnvironment Environment { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="environment"></param>
        public Startup(IConfiguration configuration, IWebHostEnvironment environment)
        {
            Configuration = configuration;
            Environment = environment;
        }
        /// <summary>
        /// This method gets called by the runtime. Use this method to add services to the container.
        /// For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        /// </summary>
        /// <param name="services"></param>
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCors();
            services.AddControllers().AddNewtonsoftJson(
                opt =>
                {
                    opt.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
                    opt.SerializerSettings.NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore;
                    opt.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
                }
            );
            services.AddHttpContextAccessor();
            services.AddHttpClient("NoProxy").ConfigurePrimaryHttpMessageHandler(() =>
            {
                return new HttpClientHandler
                {
                    UseProxy = false,
                    Proxy = null
                };
            });
            services.AddHealthChecks();

            services.AddIdentityModule(Configuration);
            services.AddColidSwaggerGeneration(Configuration);
            services.AddSingleton(Configuration);
            services.AddServicesModule(Configuration);
            services.AddResourceRelationshipManagerRepositoryModule(Configuration);
        }
        /// <summary>
        /// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        /// </summary>
        /// <param name="app"></param>
        /// <param name="env"></param>
        // 
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            //Add AzureAD Authentication
            app.UseSqlDatabaseMigration();
            app.UseDeveloperExceptionPage();
            app.UseExceptionMiddleware();
            app.UseHttpsRedirection();
            
            app.UseRouting();
            app.UseCors(
                options => options.SetIsOriginAllowed(x => _ = true)
                .AllowAnyOrigin()
                .AllowAnyHeader()
                .AllowAnyMethod()
            );

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapHealthChecks("/health");
                endpoints.MapControllers();
            });

            app.UseColidSwaggerUI(Configuration);

            //app.UseMessageQueueModule(Configuration);
        }
    }
}
