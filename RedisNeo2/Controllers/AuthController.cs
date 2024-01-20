using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Neo4jClient;
using RedisNeo2.Models.Entities;
using RedisNeo2.Services.Implementation;
using System.Security.Claims;

namespace RedisNeo2.Controllers
{
    [Authorize(Roles="Biblioteka")]
    [Authorize(Roles = "Korisnik")]
    public class AuthController : Controller
    {
        private readonly ILogger<AuthController> _logger;
        private readonly IAuthService authService;
        public AuthController(ILogger<AuthController> _logger, IAuthService authService)
        {
            this._logger = _logger;
            this.authService = authService;
        }

        public IActionResult Index1()
        {
            return View();
        }

        public IActionResult PrijaviSe() {
            return View();
        }

       
    }
}
