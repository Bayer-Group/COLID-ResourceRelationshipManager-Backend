using COLID.ResourceRelationshipManager.Services.Interface;
using Microsoft.AspNetCore.Http;
using System;
using System.Linq;
using System.Security.Claims;

namespace COLID.ResourceRelationshipManager.Services.Implementation
{
    public class UserInfoService : IUserInfoService
    {

        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly string _email;

        public UserInfoService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
            var user = _httpContextAccessor.HttpContext.User;
            var claims = ((ClaimsIdentity)user.Identity).Claims.ToList();

            _email = claims.FirstOrDefault(c => c.Type == ClaimTypes.Upn)?.Value;

        }
        public string GetEmail()
        {
            string currentEnvironment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            if (currentEnvironment != null && (currentEnvironment == "Local" || currentEnvironment == "Docker"))
            {
                return "superadmin@bayer.com";
            }
            return _email;
        }
    }
}
