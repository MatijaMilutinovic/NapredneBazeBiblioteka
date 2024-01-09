using RedisNeo2.Models.Entities;

namespace RedisNeo2.Services.Implementation
{
    public interface IAuthService
    {
        public bool Add(string organiz,string dog, Dogadjaj noviDogadjaj);
    }
}
