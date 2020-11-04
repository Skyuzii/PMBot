using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PMBot.Application.Constants;
using PMBot.Application.Services;
using PMBot.Core;
using PMBot.Grabbers.Interfaces;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace PMBot.Application.Commands
{
    public class AddAdvertCommand : BaseCommand
    {
        private readonly IList<IGrabber> _grabbers;

        public AddAdvertCommand(
            ApplicationContext context,
            ChatService chatService,
            ITelegramBotClient client,
            IEnumerable<IGrabber> grabbers) : base(context, chatService, client)
        {
            _grabbers = grabbers.ToList();
        }
        
        public override string Name => "Добавить ссылку";
        public override int State => UserDialogStates.AddAdvert;

        public override async Task ExecuteAsync(Message msg)
        {
            var url = msg.Text.Trim();
            if (await _context.Adverts.AnyAsync(x => x.UserId == _chatService.Chats[msg.Chat.Id].Id && x.Url == url))
            {
                await _client.SendTextMessageAsync(msg.Chat.Id, "У вас уже есть данное объявление", replyMarkup: GetDefaultReplyButtons());
            }
            else
            {
                var grabber = _grabbers.FirstOrDefault(x => msg.Text.Contains(x.Url))
                    ?? throw new Exception("Пожалуйста, введите коректную ссылку");
                
                var advert = await grabber.ParseAdvert(msg.Text);
                advert.UserId = _chatService.Chats[msg.Chat.Id].Id;
                advert.IsActual = true;
                advert.Complete = false;
                advert.CreateDate = DateTime.Now;
                advert.LastCheckDate = DateTime.Now;

                await _context.Adverts.AddAsync(advert);
                await _context.SaveChangesAsync();

                var message = $"Название: {advert.Name}\nМинимальная цена: {advert.LowPrice}\nДобавлено: {advert.CreateDate}";

                await _client.SendTextMessageAsync(msg.Chat.Id, message, replyMarkup: GetDefaultReplyButtons());
            }
        }
    }
}