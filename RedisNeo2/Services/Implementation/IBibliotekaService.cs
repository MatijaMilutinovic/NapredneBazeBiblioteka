using RedisNeo2.Models.Entities;

namespace RedisNeo2.Services.Implementation
{
    public interface IBibliotekaService
    {
        public IEnumerable<Biblioteka> GetAll();
        public bool AddBiblioteka(Biblioteka novaBiblioteka);
        public bool Delete(int Pib);
        public IEnumerable<Biblioteka> FindByEmail(string email);
    }
}
