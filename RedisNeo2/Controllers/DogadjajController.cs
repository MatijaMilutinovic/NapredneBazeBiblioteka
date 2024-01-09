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
    public class DogadjajController : Controller
    {
        private readonly ILogger<DogadjajController> _logger;
        private readonly IGraphClient _client;
        private readonly IDogadjajService dogadjaj_service;
        private readonly IKorisnikService korisnik_service;
        private readonly IConnectionMultiplexer _redis;
        private readonly IHttpContextAccessor httpContextAccessor;
        public DogadjajController(IHttpContextAccessor httpContextAccessor, IConnectionMultiplexer _redis, IKorisnikService korisnik_service, IDogadjajService dogadjaj_service, IGraphClient client, ILogger<DogadjajController> logger)
        {
            _client = client;
            _logger = logger;
            this.dogadjaj_service = dogadjaj_service;
            this.korisnik_service = korisnik_service;
            this._redis = _redis;
            this.httpContextAccessor = httpContextAccessor;
        }

        public IActionResult Dogadjaj()
        {
            return View();
        }

        public IActionResult Brisanje()
        {
            return View();
        }


        [Authorize(Roles = "Biblioteka")]
        [HttpPost]
        public IActionResult AddEvent(Dogadjaj model) {

            var addEventResultSuccess = this.dogadjaj_service.AddEvent(model);

            if (addEventResultSuccess) {
                TempData["msg"] = "Added Successfully";
                return RedirectToAction("GetAll", "Dogadjaj");
                //return RedirectToAction(nameof(AddEvent));
            }

            TempData["msg"] = "Error has occured on server side";

            return View(TempData);
        }

        public async Task<IActionResult> UbaciUSortedSet(UbaciUSort u) {
            var DB = _redis.GetDatabase();

            var k = await this._client.Cypher
              .Match("(dogadjaj:Dogadjaj)")
              .Where((Dogadjaj dogadjaj) =>
               dogadjaj.Naziv == u.imeDogadjaj)
              .Return(dogadjaj => dogadjaj.As<Dogadjaj>())
              .ResultsAsync;
            
            await Task.WhenAll(
                    DB.SortedSetAddAsync(u.sifraseta, k.First().Naziv, k.First().BrojUcesnika)
                );

            return NoContent();
        }

        [HttpPost]
        public async Task<string> UredjeniDogadjaji(UbaciUSort u)
        {
            var DB = _redis.GetDatabase();


            var primiSortedSet = await DB.SortedSetRangeByRankWithScoresAsync(u.sifraseta, 0, -1);
            var vratiPodatke = (string.Join("\n", primiSortedSet));

            return vratiPodatke;
            //return RedirectToAction("UredjeniDogadjajiPage", "Dogadjaj");

        }

        public IActionResult UredjeniDogadjajiPage()
        {
            return View();
        }


        [HttpPost]
        public async Task<IActionResult> PrijaviSe(PrijaviSeDTO p)
        {
            var a = this.httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Email);
            //var a = 98;
            var b = await this._client.Cypher
                .OptionalMatch("(korisnik:Korisnik)-[r:PrijavljenNa]->(dogadjaj:Dogadjaj)")
                .Where((Korisnik korisnik, Dogadjaj dogadjaj) =>
                korisnik.Email == a &&
                dogadjaj.Naziv == p.ImeDogadjaja)
                .Return(korisnik => korisnik.As<Korisnik>())
                .ResultsAsync;

            if (b.First() == null) {
            await this._client.Cypher
                            .Match("(d:Dogadjaj), (k:Korisnik)")
                            // .Match("(k)-[r:PrijavljenNa]->(d)")
                            .Where((Dogadjaj d, Korisnik k) =>
                                 d.Naziv == p.ImeDogadjaja &&
                                 k.Email == a
                                 )
                            
                            .Create("(k)-[r:PrijavljenNa]->(d)")
                            //.Return(d => d.As<Dogadjaj>()).ResultsAsync;
                            .ExecuteWithoutResultsAsync();

                return RedirectToAction("UspesnaPrijava", "Dogadjaj");
            }

            
            return RedirectToAction("NeuspesnaPrijava", "Dogadjaj");
           // return RedirectToAction("/Dogadjaj/GetAll");
        }

        public async Task<IActionResult> Delete(Dogadjaj d1)
        {
            await this._client.Cypher.OptionalMatch("(d: Dogadjaj)<-[r]-()")
                              .Where((Dogadjaj d) => d.Naziv == d1.Naziv)
                              .Delete("r, d")
                              .ExecuteWithoutResultsAsync();
            //var result = this.dogadjaj_service.Delete(ime);
            return RedirectToAction("GetAll");
        }

        public IActionResult PrijaviSePage()
        {
            return View();
        }

        public IActionResult UspesnaPrijava() {
            return View();
        }

        public IActionResult NeuspesnaPrijava()
        {
            return View();
        }

        public IActionResult GetAll() {

            var podaci = this.dogadjaj_service.GetAll();
            return View(podaci);
        }

       

        public IActionResult AddEventPage()
        {

            return View();
        }

        public IActionResult Prijavi() {
            return View();
        }

        //public async Task<IActionResult> UredjeniDogadjaji() {
        //    var dogadjaji = this._client.Cypher
        //                        .Match("(x: Dogadjaj)")
        //                        .Return(x => x.As<Dogadjaj>())
        //                        .ResultsAsync;
        //    var DB = _redis.GetDatabase();
            
        //    foreach (Dogadjaj d in dogadjaji.Result) {
        //        await Task.WhenAll(
        //            DB.SortedSetAddAsync("uredjeniDog", d.Naziv, d.BrojUcesnika)
        //        ) ;
        //    }

        //    var primiSortedSet = await DB.SortedSetRangeByRankWithScoresAsync("uredjeniDog", 0, -1);
        //    var vratiPodatke = (string.Join("\n", primiSortedSet));

        //    return View(vratiPodatke);
        //}


        [HttpPost]
        public async Task<NoContentResult> UrediPoBrojuUcesnika()
        {

            var dogadjaji = this._client.Cypher
                                .Match("(x: Dogadjaj)")
                                .Return(x => x.As<Dogadjaj>())
                                .ResultsAsync;
            var DB = _redis.GetDatabase();

            foreach (Dogadjaj d in dogadjaji.Result)
            {
                await Task.WhenAll(
                    DB.SortedSetAddAsync("K10", d.Naziv, d.BrojUcesnika)
                );
            }

            return NoContent();

        }

        //[HttpGet]
        //public async Task<IActionResult> UredjeniDogadjaji()
        //{
        //    var DB = _redis.GetDatabase();

        //    var primiSortedSet = await DB.SortedSetRangeByRankWithScoresAsync("K10", 0, -1);
        //    var vratiPodatke = (string.Join("\n", primiSortedSet));

        //    return View(vratiPodatke);

        //}
        //[HttpGet]
        //public async Task<IActionResult> GetAll() {

        //    var dogadjaji = await _client.Cypher
        //                    .Match("(x: Dogadjaj)")
        //                     .Return(x => x.As<Dogadjaj>()).ResultsAsync;
        //    return Ok(dogadjaji);
        //}

    }
}
