using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RedisNeo2.Models.DTOs;
using RedisNeo2.Services.Implementation;

namespace RedisNeo2.Controllers
{
    public class ChatController : Controller
    {
        public readonly IChatServices chatService;

        public ChatController(IChatServices chatService)
        {

            this.chatService = chatService;
        }

        [Authorize]
        public IActionResult ChatPage()
        {
            //var currentLogedInUser = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            //ViewBag.CurrentLogedIn = currentLogedInUser;
            return View();
        }



        [Authorize(Roles = "Korisnik")]
        public IActionResult ChatVjuKorisnik()
        {
            //string D = this.chatService.Receive().ToString();
            //string[] slice = D.Split("^");
            //ViewBag.PORUKA = slice[1];
            //ViewBag.KORISNIK = slice[0];
            return View();
        }

        [Authorize(Roles = "Biblioteka")]
        public IActionResult ChatVjuBiblioteka()
        {
            return View();
        }


        [HttpPost]
        public async Task<NoContentResult> Send(PorukaDTO porukaZaSlati)
        {

            await this.chatService.SendMessage(porukaZaSlati);
            return NoContent();

        }

        [HttpGet]
        public async Task<string> Receive()
        {

            string D = await this.chatService.Receive();
            //string[] slice = D.Split("^");
            ////ViewBag.PORUKA = D;
            //PorukaDTO PP = new()
            //{
            //    Sender = slice[0],
            //    Poruka = slice[1],
            //};

            return D;
        }
    }
}
