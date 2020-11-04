using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PMBot.Application.Constants;
using PMBot.Application.Services;
using PMBot.Core;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace PMBot.Application.Commands
{
    public class MyAdvertsCommand : BaseCommand
    {
        public MyAdvertsCommand(
            ApplicationContext context,
            ChatService chatService,
            ITelegramBotClient client) : base(context, chatService, client)
        {
        }

        public override string Name => "Мои объявления";
        public override int State => UserDialogStates.MyAdverts;
        
        public override async Task ExecuteAsync(Message msg)
        {
            var adverts = _context.Adverts
                .Where(x => x.UserId == _chatService.Chats[msg.Chat.Id].Id)
                .Take(20)
                .ToList();

            if (adverts.Count > 0)
            {
                adverts.ForEach(async advert =>
                {
                    var textSb = new StringBuilder()
                        .AppendLine($"Название: <a href='{advert.Url}'>{advert.Name}</a>")
                        .AppendLine($"Дата добавления: {advert.CreateDate}")
                        .AppendLine($"Дата последней провери: {advert.LastCheckDate}")
                        .AppendLine($"Актуальная цена на момент последнего чека: {advert.LowPrice}р");
                    
                    await _client.SendTextMessageAsync(msg.Chat.Id, textSb.ToString(), ParseMode.Html);
                });
            }
            else
            {
                await _client.SendTextMessageAsync(msg.Chat.Id, "У вас нет объявлений");
            }
        }
    }
}