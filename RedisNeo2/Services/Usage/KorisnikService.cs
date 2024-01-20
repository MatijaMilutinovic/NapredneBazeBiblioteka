using Neo4jClient;
using RedisNeo2.Models.Entities;
using RedisNeo2.Services.Implementation;

namespace RedisNeo2.Services.Usage
{
    public class KorisnikService : IKorisnikService
    {
        private readonly IGraphClient client;

        public KorisnikService(IGraphClient client) { 
            this.client = client;
        }

        public bool AddKorisnik(Korisnik noviKorisnik)
        {
            this.client.Cypher.Create("(k:Korisnik $noviKorisnik)")
                                 .WithParam("noviKorisnik", noviKorisnik)
                                 .ExecuteWithoutResultsAsync(); ;

            string str = $"(k:Korisnik {noviKorisnik})";

            return true;
        }
    }
}
