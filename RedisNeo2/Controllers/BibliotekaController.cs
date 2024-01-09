using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Neo4jClient;
using RedisNeo2.Models;
using RedisNeo2.Models.DTOs;
using RedisNeo2.Models.Entities;
using RedisNeo2.Services.Implementation;
using System.Security.Claims;

namespace RedisNeo2.Controllers
{
   
    public class BibliotekaController : Controller
    {
        private readonly ILogger<BibliotekaController> _logger;
        private readonly IBibliotekaService service;
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly IGraphClient client;

        public BibliotekaController(IBibliotekaService service, IGraphClient client, ILogger<BibliotekaController> logger, IHttpContextAccessor httpContextAccessor)
        {
            this.httpContextAccessor = httpContextAccessor;
            this.service = service;
            this.client = client;
            _logger = logger;
        }

        public IActionResult Biblioteka()
        {
            return View();
        }

        
        public IActionResult LoginBibliotekaPage()
        {
            return View();
        }

        
        [HttpPost]
        public async Task<IActionResult> Login(string email, string lozinka) {

            var claims = new List<Claim> {
                    new(ClaimTypes.Email, email),
                    new(ClaimTypes.Role, "Biblioteka")
                };

            ClaimsIdentity identitycl = new(
                claims,
                CookieAuthenticationDefaults.AuthenticationScheme
            );

            ClaimsPrincipal principalcl = new(identitycl);

            var properties = new AuthenticationProperties();

            await HttpContext.SignInAsync(
                 CookieAuthenticationDefaults.AuthenticationScheme,
                 principalcl,
                 properties
             );

            return RedirectToAction("LoggedInBiblioteka", "Biblioteka");
        }

        [HttpPost]
        public async Task<IActionResult> Logout() {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("AddBibliotekaPage");
        }

        [HttpPost]
        public IActionResult Add(Biblioteka model) {

            var biblioteka = this.service.AddBiblioteka(model);
            if (biblioteka) {
                return RedirectToAction("AddBibliotekaPage", "Biblioteka");
            }
            return View();
        }

        public IActionResult AddBibliotekaPage() {
            return View();
        }

        public IActionResult GetAll() {

            var sveOrganizacije = this.service.GetAll();
            return View(sveOrganizacije);
        }

        public IActionResult PostojiBiblioteka() {
            return View();
        }

        [Authorize]
        public IActionResult LoggedInBiblioteka()
        {
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Biblioteka")]
        public async Task<IActionResult> UpdateN(UpdateKorisnikBiblioDTO k)
        {
            var a = httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Email);

            var result = await client.Cypher.Match("(nn:Biblioteka)")
                                            .Where((Biblioteka nn) => nn.Email == a)
                                            .Return(nn => nn.As<Biblioteka>()).ResultsAsync;

            Biblioteka staraBibl = result.First();
            staraBibl.Lozinka = k.NovaLozinka;

            await this.client.Cypher
            .Match("(n1:Biblioteka)")
            .Where((Biblioteka n1) => n1.Email == a)
            .Set("n1 = $bibliotekaa")
            .WithParam("bibliotekaa", staraBibl)
            .ExecuteWithoutResultsAsync();

            return NoContent();
        }

        public IActionResult UpdateBiblioteka()
        {

            return View();
        }

    }
}
