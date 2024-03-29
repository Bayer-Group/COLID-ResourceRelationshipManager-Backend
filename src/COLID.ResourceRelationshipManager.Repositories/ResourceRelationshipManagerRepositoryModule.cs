﻿using COLID.ResourceRelationshipManager.Repositories.Implementation;
using COLID.ResourceRelationshipManager.Repositories.Interface;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;

namespace COLID.ResourceRelationshipManager.Repositories
{
    public static class ResourceRelationshipManagerRepositoryModule
    {
        public static IServiceCollection AddResourceRelationshipManagerRepositoryModule(this IServiceCollection services, IConfiguration configuration)
        {

            services.AddScoped<IGraphMapRepository, GraphMapRepository>();
            services.AddTransient<IUserInfoRepository, UserInfoRepository>();

            var connectionString = BuildConnectionString(configuration);
            services.AddDbContext<ResourceRelationshipManagerContext>(options =>
            {
                options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString), mysqlOptions =>
                {
                    mysqlOptions.CommandTimeout(5);
                    mysqlOptions.EnableRetryOnFailure(3);
                    mysqlOptions.EnableStringComparisonTranslations();
                });
            });


            return services;
        }

        public static void UseSqlDatabaseMigration(this IApplicationBuilder app)
        {
            using var scope = app.ApplicationServices.CreateScope();
            var services = scope.ServiceProvider;

            try
            {
                var context = services.GetRequiredService<ResourceRelationshipManagerContext>();
                context.Database.Migrate();
            } catch(System.Exception ex)
            {
                var logger = app.ApplicationServices.GetRequiredService<ILogger<DbContext>>();
                logger.LogError(ex, "An error occured and the DB migration failed");
                throw;
            }
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
