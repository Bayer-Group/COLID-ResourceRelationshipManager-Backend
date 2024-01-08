using COLID.Exception;
using COLID.Identity;
using COLID.ResourceRelationshipManager.Repositories;
using COLID.ResourceRelationshipManager.Services;
using COLID.Swagger;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Serialization;
using System.Net.Http;

var builder = WebApplication.CreateBuilder(args);
ConfigurationManager configuration = builder.Configuration;

builder.Services.AddCors();
builder.Services.AddControllers().AddNewtonsoftJson(
                opt =>
                {
                    opt.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
                    opt.SerializerSettings.NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore;
                    opt.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
                }
            );
builder.Services.AddHttpClient("NoProxy").ConfigurePrimaryHttpMessageHandler(() =>
{
    return new HttpClientHandler
    {
        UseProxy = false,
        Proxy = null
    };
});
builder.Services.AddHttpContextAccessor();
builder.Services.AddHealthChecks();
builder.Services.AddIdentityModule(configuration);
builder.Services.AddColidSwaggerGeneration(configuration);
builder.Services.AddServicesModule(configuration);
builder.Services.AddResourceRelationshipManagerRepositoryModule(configuration);

var app = builder.Build();

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

app.UseColidSwaggerUI(configuration);

app.Run();
