using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Neo4jClient;
using RedisNeo2.Models.Entities;
using RedisNeo2.Services.Implementation;
using System.Security.Claims;

namespace RedisNeo2.Services.Usage
{
    public class AuthService : IAuthService
    {
        private readonly IGraphClient client;

        public AuthService(IGraphClient client)
        {
            this.client = client;
        }


        
    }
}

