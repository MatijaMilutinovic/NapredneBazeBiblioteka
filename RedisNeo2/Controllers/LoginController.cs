using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Neo4jClient;
using RedisNeo2.Models.Entities;
using System.Security.Claims;

namespace RedisNeo2.Controllers
{
    public class LoginController : Controller
    {
        private readonly IGraphClient client;

        public LoginController(IGraphClient client) {
            this.client = client;
        }
        public IActionResult LoginPage()
        {
            return View();
        }
        public IActionResult LoginKorisnik()
        {
            return View();
        }

        public IActionResult LoginBiblioteka()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(Login login)
        {
            if (login.Login_Kao == "Korisnik") {
                var k = await this.client.Cypher
              .Match("(korisnik:Korisnik)")
              .Where((Korisnik korisnik) =>
               korisnik.Email == login.Email &&
               korisnik.Lozinka == login.Lozinka)
              .Return(korisnik => korisnik.As<Korisnik>())
              .ResultsAsync;

                if (k.First() == null)
                {
                    return RedirectToAction("VecPostoji", "Korisnik");
                }
            }

            if (login.Login_Kao == "Biblioteka")
            {
                var n = await this.client.Cypher
                   .Match("(biblioteka:Biblioteka)")
                   .Where((Biblioteka biblioteka) =>
                    biblioteka.Email == login.Email &&
                    biblioteka.Lozinka == login.Lozinka)
                   .Return(biblioteka => biblioteka.As<Korisnik>())
                   .ResultsAsync;



                if (n.First() == null)
                {
                    return RedirectToAction("PostojiBiblioteka", "Biblioteka");
                }
            }
            var claims = new List<Claim> {
                    new Claim(ClaimTypes.Email, login.Email),
                    //new Claim(ClaimTypes.Name, login_kao),
                    new Claim(ClaimTypes.Role, login.Login_Kao)
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

            if (login.Login_Kao.Equals("Korisnik"))
               //return RedirectToAction("LoggedInKorisnik", "Korisnik");
               return RedirectToAction("LoggedInKorisnik", "Korisnik");
            else if (login.Login_Kao.Equals("Biblioteka"))
                return RedirectToAction("LoggedInBiblioteka", "Biblioteka");
            else
                return RedirectToAction("LoginPage", "Login");
        }
    }
}
