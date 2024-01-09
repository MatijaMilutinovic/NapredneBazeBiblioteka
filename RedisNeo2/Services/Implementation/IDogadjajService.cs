using Neo4jClient;
using RedisNeo2.Models.Entities;

namespace RedisNeo2.Services.Implementation
{
    public interface IDogadjajService
    {
        public IEnumerable<Dogadjaj> GetAll();
        public bool AddEvent(Dogadjaj noviDogadjaj);
       
        public bool Delete(string Naziv);
        public Dogadjaj FindByName();
        public  bool PrijaviSe(string dog, string BibliotekaName, string KorisnikEmail);
    }
}
