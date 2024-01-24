using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Neo4jClient;
using NuGet.Protocol.Plugins;
using RedisNeo2.Models.DTOs;
using RedisNeo2.Models.Entities;
using RedisNeo2.Services.Implementation;
using StackExchange.Redis;
using System.Security.Claims;
using static ServiceStack.Diagnostics.Events;

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
            var k = await this._client.Cypher
                  .Match("(korisnik:Korisnik)")
                  .Where((Korisnik korisnik) =>
                   korisnik.Email == username &&
                   korisnik.Lozinka == lozinka)
                  .Return(korisnik => korisnik.As<Korisnik>())
                  .ResultsAsync;

            if (!k.Any())
            {
                return RedirectToAction("NePostojeciKorisnik", "Korisnik");
            }

            var claims = new List<Claim> {
                    new(ClaimTypes.Email, username),
                    new(ClaimTypes.Role, "Korisnik")
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

        public IActionResult NePostojeciKorisnik()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> UclaniSeUBibl(string mail)
        {
            var userMail = this.httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Email);
            var potMatch = await this._client.Cypher
                .OptionalMatch("(korisnik:Korisnik)-[r:UclanjenU]->(biblioteka:Biblioteka)")
                .Where((Korisnik korisnik, Biblioteka biblioteka) =>
                korisnik.Email == userMail &&
                biblioteka.Email == mail)
                .Return(korisnik => korisnik.As<Korisnik>())
                .ResultsAsync;

            if (potMatch.First() == null)
            {
                await this._client.Cypher
                                .Match("(bib:Biblioteka), (k:Korisnik)")
                                .Where((Biblioteka bib, Korisnik k) =>
                                     bib.Email == mail &&
                                     k.Email == userMail
                                     )
                                .Create("(k)-[r:UclanjenU]->(bib)")
                                .ExecuteWithoutResultsAsync();

                return RedirectToAction("UspesnaPrijava", "Korisnik");
            }


            return RedirectToAction("NeuspesnaPrijava", "Korisnik");
        }

        [HttpPost]
        public IActionResult Add(Korisnik model)
        {

            var biblioteka = this.korisnikService.AddKorisnik(model);
            if (biblioteka)
            {
                return RedirectToAction("LoginPage", "Login");
            }
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Korisnik")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteKorisnik()
        {
            System.Diagnostics.Debug.WriteLine($"Delete korisnik");
            var signedInMail = httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Email);
            var result = await _client.Cypher.Match("(k:Korisnik)")
                                            .Where((Korisnik k) => k.Email == signedInMail)
                                            .Return(k => k.As<Korisnik>()).ResultsAsync;

            System.Diagnostics.Debug.WriteLine($"p ={result.FirstOrDefault()?.Email}");

            if (result == null || !result.Any()) return RedirectToAction("UnsuccessfullyDeletedKorisnik");

            await this._client.Cypher.OptionalMatch("(k: Korisnik)")
                              .Where((Korisnik k) => k.Email == signedInMail)
                              .DetachDelete("k")
                              .ExecuteWithoutResultsAsync();

            return RedirectToAction("SuccessfullyDeletedKorisnik");
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("AddKorisnikPage");
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

        public IActionResult UspesnaPrijava() {

            return View();
        }

        public IActionResult NeuspesnaPrijava() {

            return View();
        }

        [Authorize]
        public IActionResult SuccessfullyDeletedKorisnik()
        {
            return View();
        }

        [Authorize]
        public IActionResult UnsuccessfullyDeletedKorisnik()
        {
            return View();
        }
    }
}
