using RedisNeo2.Models.DTOs;
using RedisNeo2.Models.Entities;

namespace RedisNeo2.Services.Implementation
{
    public interface IChatServices
    {
        public Task SendMessage(string message);
        public Task<string> Receive();
    }
}
