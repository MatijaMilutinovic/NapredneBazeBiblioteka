using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Neo4jClient;
using RedisNeo2.Models.Entities;
using RedisNeo2.Services.Implementation;
using System.Security.Claims;

namespace RedisNeo2.Services.Usage
{
    public class AuthService : IAuthService
    {
        private readonly IGraphClient client;

        public AuthService(IGraphClient client)
        {
            this.client = client;
        }

        private void DodajToSpisak(Biblioteka n, Dogadjaj d) {
            n.SpisakDogadjaja.Add(d);
        }

        public bool Add(string organiz, string dog, Dogadjaj noviDogadjaj)
        {

            var D = noviDogadjaj;
            D.Organizator = organiz;
            this.client.Cypher.Match("biblioteka:Biblioteka").Where((Biblioteka biblioteka) => biblioteka.Naziv == organiz)
                              .Create("(biblioteka)-[r:Organizuje]->(d:Dogadjaj {D})")
                              .WithParam("D", D)
                              .ExecuteWithoutResultsAsync(); ;

            return true;

        }

        
    }
}

