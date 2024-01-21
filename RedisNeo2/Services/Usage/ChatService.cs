
using Microsoft.AspNetCore.Mvc;
using RedisNeo2.Hubs;
using RedisNeo2.Models.DTOs;
using RedisNeo2.Models.Entities;
using RedisNeo2.Services.Implementation;
using StackExchange.Redis;
using System.Security.Claims;
using System.Text.Json;

namespace RedisNeo2.Services.Usage
{
    public class ChatService : IChatServices
    {
        private readonly IConnectionMultiplexer _cmux;
        private readonly IHttpContextAccessor httpContextAccessor;

        private readonly string Channel = "Kanal";
        public ChatService(IConnectionMultiplexer cmux, IHttpContextAccessor httpContextAccessor)
        {
            _cmux = cmux;
            this.httpContextAccessor = httpContextAccessor;
        }

        public async Task SendMessage(PorukaDTO porukaZaSlati)
        {

            var sender = httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Email);
            var subscriber = _cmux.GetSubscriber();
            string msgToSend = $"{sender}: {porukaZaSlati.Poruka} to {porukaZaSlati.Receiver}";
            System.Diagnostics.Debug.WriteLine($"Poruka za slati: {msgToSend}");
            IDatabase redis = _cmux.GetDatabase();
            await subscriber.PublishAsync(Channel, msgToSend);
            redis.StringSet(porukaZaSlati.Receiver, msgToSend);
        }

        private Task<string> GetMessageAsync()
        {
            var subscriber = _cmux.GetSubscriber();
            var tcs = new TaskCompletionSource<string>();
            subscriber.Subscribe(Channel, (channel, message) => tcs.TrySetResult(message));
            return tcs.Task;
        }

        private void RenderComplete(string json)
        {
            _tcs.TrySetResult(json);
        }

        private TaskCompletionSource<string> _tcs = new TaskCompletionSource<string>();

        public async Task<string> Receive()
        {
            var receiver = httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Email);
            IDatabase redis = _cmux.GetDatabase();
            var subscriber = _cmux.GetSubscriber();
            string A = await redis.StringGetAsync(receiver);

            System.Diagnostics.Debug.WriteLine($"Receiver: {receiver}");
            return A ?? "Nemate privatnih poruka!";


        }
    }
}
