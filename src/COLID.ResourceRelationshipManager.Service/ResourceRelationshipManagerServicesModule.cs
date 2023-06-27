using System;
using COLID.ResourceRelationshipManager.Services.Configuration;
using COLID.ResourceRelationshipManager.Services.Implementation;
using COLID.ResourceRelationshipManager.Services.Interface;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using COLID.ResourceRelationshipManager.Repositories;

namespace COLID.ResourceRelationshipManager.Services
{
    public static class ResourceRelationshipManagerServicesModule
    {
        /// <summary>
        /// This will register all the supported functionality by Services module.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/> object for registration</param>
        public static IServiceCollection AddServicesModule(this IServiceCollection services, IConfiguration configuration)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }
            services.Configure<ColidRegistrationServiceTokenOptions>(configuration.GetSection("ColidRegistrationServiceTokenOptions"));
            services.Configure<ColidSearchServiceTokenOptions>(configuration.GetSection("ColidSearchServiceTokenOptions"));
            services.AddTransient<IRemoteRegistrationService, RemoteRegistrationService>();
            services.AddTransient<IRemoteSearchService, RemoteSearchService>();
            services.AddTransient<IUserInfoService, UserInfoService>();
            services.AddTransient<IGraphMapService, GraphMapService>();
            
            return services;
        }

        private static string BuildConnectionString(IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("MySQLConnection");
            var dbUser = configuration.GetValue<string>("Database:User");
            var dbPassword = configuration.GetValue<string>("Database:Password");

            return connectionString
                .Replace("{DB_USER}", dbUser, StringComparison.Ordinal)
                .Replace("{DB_PASSWORD}", dbPassword, StringComparison.Ordinal);
        }
    }
}
