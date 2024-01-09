using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using RedisNeo2.Models.DTOs;
using RedisNeo2.Models.Entities;
using RedisNeo2.Services.Implementation;
using RedisNeo2.Services.Usage;
using StackExchange.Redis;
using System.Security.Claims;
using System.Text.Json;

namespace RedisNeo2.Hubs
{
    public class ChatHub : Hub
    {
        private readonly IBibliotekaService _bibliotekaService;
        private readonly IKorisnikService _korService;
        private readonly IChatServices _chatService;

        //private readonly ILogger<Consumer> logger;
        private readonly IConnectionMultiplexer _cmux;
      //  private readonly IDatabase _redisDb;

        private readonly  string Chanell = "NBP";
        public ChatHub(IConnectionMultiplexer cmux, IBibliotekaService bibliotekaService, IKorisnikService k, IChatServices chatService)
        {
            _bibliotekaService = bibliotekaService;
            _chatService = chatService;
            _korService = k;
            _cmux = cmux;
           // _redisDb = redisDb;
        }

        public async Task Pub(string user, string messageString) {

            var subscriber =  _cmux.GetSubscriber();
            string A = string.Concat(user, "^");
            string B  = string.Concat(A, messageString);
            
            await subscriber.PublishAsync(Chanell, B);
         

        }

        public async Task<PorukaDTO> Sub() {
            var subscriber = _cmux.GetSubscriber();
            string korisnik = string.Empty;
            string poruka = string.Empty;
            List<string> listaPoruka = new List<string>();
            await subscriber.SubscribeAsync(Chanell, (channel, porukaPLUScovek) => {
                //listaPoruka.Add(message);
                string[] subs = porukaPLUScovek.ToString().Split("^");
                korisnik = subs[0];
                poruka = subs[1];
                Console.WriteLine("Poruku sa teksotm: " + poruka + " salje korisnik: " + korisnik);
            });
            Console.ReadLine();

            PorukaDTO vratiPoruku = new PorukaDTO();
            vratiPoruku.korisnik = korisnik;
            vratiPoruku.poruka = poruka;

            return vratiPoruku;
        }

        public async Task SendMessageNovo(string user, string messageString)
        {
            //var message = JsonSerializer.Deserialize<Message>(messageString);

            await Clients.All.SendAsync("NBP_Chat", user, messageString);
        }

        //public async Task GetMessage1() {
        //    //await _chatService.GetMessage();    
        //}

        //public async Task SendMessage1(string user, string message) {
        //    await Clients.All.SendAsync("NBP_Chat", user, message);
        //}

        //public async Task JoinRoom(RoomConnection rc) {
        //    await Groups.AddToGroupAsync(Context.ConnectionId, rc.Room);
        //    await Clients.Group(rc.Room).SendAsync("chatapp", _userBot, $"{rc.User} has joined {rc.Room}");
        //}

        //public async Task SendMessage(string user, string message) =>
        //    await Clients.All.SendAsync("chatapp", user, message);

        //public string GetConnectionID() => Context.ConnectionId;

        public string korisnik() => Context.User.FindFirstValue(ClaimTypes.Email);

        public string re() => Context.User.FindFirstValue(ClaimTypes.Email);

        public string ren() => Context.User.FindFirstValue(ClaimTypes.Email);
        public string rek() => Context.User.FindFirstValue(ClaimTypes.Email);
        //public async Task SendToUser(string user, string receiverConnectonID, string message) =>
        //    await Clients.Client(receiverConnectonID).SendAsync("chatapp", user, message);

        //[Authorize(Roles = "Korisnik")]
        //public async Task KDopisivanje(string user, string receiverConnectonID, string message) =>
        //   await Clients.Client(receiverConnectonID).SendAsync("chatapp", user, message);
        //private static Dictionary<string, string> Users = new Dictionary<string, string>();

        //public override async Task OnConnectedAsync()
        //{
        //    string username = Context.GetHttpContext().Request.Query["username"];
        //    Users.Add(Context.ConnectionId, username);
        //    await AddMessageToChat(string.Empty, $"{username} joined the party!");
        //    await base.OnConnectedAsync();
        //}

        //public override async Task OnDisconnectedAsync(Exception? exception)
        //{
        //    string username = Users.FirstOrDefault(u => u.Key == Context.ConnectionId).Value;
        //    await AddMessageToChat(string.Empty, $"{username} left!");
        //}

        //public async Task AddMessageToChat(string user, string message)
        //{
        //    await Clients.All.SendAsync("GetThatMessageDude", user, message);
        //}
    }
}
