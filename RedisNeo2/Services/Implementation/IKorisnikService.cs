using RedisNeo2.Models.Entities;

namespace RedisNeo2.Services.Implementation
{
    public interface IKorisnikService
    {
        public bool AddKorisnik(Korisnik noviKorisnik);
        public IEnumerable<Korisnik> SviPrijavljeni(string imeDogadjaja);
    } 
}
