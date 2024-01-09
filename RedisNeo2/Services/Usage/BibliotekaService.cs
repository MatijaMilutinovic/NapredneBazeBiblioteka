using Neo4jClient;
using RedisNeo2.Models.Entities;
using RedisNeo2.Services.Implementation;

namespace RedisNeo2.Services.Usage
{

    public class BibliotekaService : IBibliotekaService
    {
        
        private readonly IGraphClient client;

        public BibliotekaService(IGraphClient client)
        {
            this.client = client;
        }

        public bool AddBiblioteka(Biblioteka novaBibl)
        {
            this.client.Cypher.Create("(n:Biblioteka $novaBibl)")
                              .WithParam("novaBibl", novaBibl)
                              .ExecuteWithoutResultsAsync();

            return true;
        }

        public bool Delete(int Pib)
        {
            var trazeni = this.client.Cypher.Match("(bibliteka: Biblioteka)")
                                            .Where((Biblioteka biblioteka) => biblioteka.Pib == Pib)
                                            .Return(biblioteka => biblioteka.As<Biblioteka>())
                                            .ResultsAsync;
            if (trazeni == null)
                return false;

            this.client.Cypher.Match("(biblioteka: Biblioteka)<-r-()")
                              .Where((Biblioteka biblioteka) => biblioteka.Pib == Pib)
                              .Delete("biblioteka, d")
                              .ExecuteWithoutResultsAsync();


            return true;
        }

        public IEnumerable<Biblioteka> FindByEmail(string email)
        {
            var trazeni = this.client.Cypher.Match("(biblioteka: Biblioteka)")
                                            .Where((Biblioteka biblioteka) => biblioteka.Email == email)
                                            .Return(biblioteka => biblioteka.As<Biblioteka>())
                                            .ResultsAsync;

            return trazeni.Result;

        }

        public IEnumerable<Biblioteka> GetAll()
        {
            var organizacije = this.client.Cypher
                            .Match("(x: Biblioteka)")
                            .Return(x => x.As<Biblioteka>())
                            .ResultsAsync;


            return organizacije.Result;
        }
    }
}
