using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Neo4jClient;
using RedisNeo2.Models.DTOs;
using RedisNeo2.Models.Entities;
using RedisNeo2.Services.Implementation;
using StackExchange.Redis;
using System.Security.Claims;

namespace RedisNeo2.Controllers
{
    public class KorisnikController : Controller
    {
        private readonly IKorisnikService korisnikService;
        private readonly IGraphClient _client;
        private readonly IConnectionMultiplexer _redis;
        private readonly IHttpContextAccessor httpContextAccessor;
        public KorisnikController(IHttpContextAccessor httpContextAccessor, IConnectionMultiplexer _redis, IKorisnikService korisnikService, IGraphClient _client) {
            this.korisnikService = korisnikService;
            this._redis = _redis;
            this._client = _client;
            this.httpContextAccessor = httpContextAccessor;
        }

        public IActionResult Korisnik()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string username, string lozinka)
        {

            var claims = new List<Claim> {
                    new Claim(ClaimTypes.Email, username),
                    new Claim(ClaimTypes.Role, "Korisnik")
                };

            ClaimsIdentity identitycl = new ClaimsIdentity(
                claims,
                CookieAuthenticationDefaults.AuthenticationScheme
            );

            ClaimsPrincipal principalcl = new ClaimsPrincipal(identitycl);

            var properties = new AuthenticationProperties();

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                principalcl,
                properties
            );

            return RedirectToAction("LoggedInKorisnik", "Korisnik");
        }

        public IActionResult AddKorisnikPage() {
            return View();
        }

        public IActionResult LoggedInKorisnik()
        {
            return View();
        }

        public IActionResult LoginKorisnik()
        {
            return View();
        }

        public IActionResult Postoji()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Add(Korisnik model)
        {

            var biblioteka = this.korisnikService.AddKorisnik(model);
            if (biblioteka)
            {
                return RedirectToAction("AddKorisnikPage", "Korisnik");
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("AddKorisnikPage");
        }

        public async Task<IActionResult> SviPrijavljeni(Dogadjaj d1) {
            var b = await this._client.Cypher
                    .OptionalMatch("(korisnik:Korisnik)-[r:PrijavljenNa]->(dogadjaj:Dogadjaj)")
                    .Where((Korisnik korisnik, Dogadjaj dogadjaj) =>
                    dogadjaj.Naziv == d1.Naziv)
                    .Return(korisnik => korisnik.As<Korisnik>())
                    .ResultsAsync;

            //var A = this.korisnikService.SviPrijavljeni(imeDogadjaja);
            return View(b);
            //return View(b);
        }

        [HttpPost]
        [Authorize(Roles="Korisnik")]
        public async Task<IActionResult> UpdateKor(UpdateKorisnikBiblioDTO k) {
            var a = this.httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Email);

            var result = await _client.Cypher.Match("(kk:Korisnik)")
                                            .Where((Korisnik kk) => kk.Email == a)
                                            .Return(kk => kk.As<Korisnik>()).ResultsAsync;

            Korisnik stariKor = result.First();
            stariKor.Lozinka = k.NovaLozinka;

            await this._client.Cypher
            .Match("(k1:Korisnik)")
            .Where((Korisnik k1) => k1.Email == a)
            .Set("k1 = $korisnik")
            .WithParam("korisnik", stariKor)
            .ExecuteWithoutResultsAsync();

            return NoContent();
        }

        public IActionResult UpdateKorisnik() {

            return View();
        }
    }
}
