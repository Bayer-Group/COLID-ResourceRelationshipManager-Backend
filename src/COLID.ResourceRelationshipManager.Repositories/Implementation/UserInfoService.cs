
using COLID.ResourceRelationshipManager.Repositories.Interface;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;

namespace COLID.ResourceRelationshipManager.Repositories.Implementation
{
    public class UserInfoRepository : IUserInfoRepository
    {

        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly string _email;

        public UserInfoRepository(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
            var user = _httpContextAccessor.HttpContext.User;
            var claims = ((ClaimsIdentity)user.Identity).Claims.ToList();

            _email = claims.FirstOrDefault(c => c.Type == ClaimTypes.Upn)?.Value;

        }
        public string GetEmail()
        {
            return _email;
        }
    }
}
