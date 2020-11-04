using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PMBot.Application.Services;
using Telegram.Bot.Types;

namespace PMBot.Api.Controllers
{
    [ApiController]
    [Route("/api/[controller]")]
    public class BotController : ControllerBase
    {
        private readonly TelegramService _telegramService;

        public BotController(TelegramService telegramService)
        {
            _telegramService = telegramService;
        }
        
        [HttpGet]
        public string Check() => "OK";

        [HttpPost]
        public async Task Handle([FromBody] Update update)
        {
            await _telegramService.Handle(update);
        }
    }
}