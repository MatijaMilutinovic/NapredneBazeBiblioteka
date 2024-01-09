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

        [Authorize(Roles="Biblioteka")]
        [HttpPost]
        public IActionResult Add(string o, string d, Dogadjaj model) {

            var objavljenDogadjajResultSuccess = this.authService.Add(o, d, model);

            if (objavljenDogadjajResultSuccess)
            {
                return RedirectToAction("GetAll", "Dogadjaj");
            }
            return View();
        }

        public IActionResult Index1()
        {
            return View();
        }

        public IActionResult ObjaviDogadjajPage() {
            return View();
        }

        public IActionResult PrijaviSe() {
            return View();
        }

       
    }
}
