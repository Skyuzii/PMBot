using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using PMBot.Application.Constants;
using PMBot.Application.Services;
using PMBot.Core;
using PMBot.Core.Entities;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace PMBot.Application.Commands
{
    public class StartCommand : BaseCommand
    {
        public StartCommand(
            ApplicationContext context,
            ChatService chatService,
            ITelegramBotClient client) : base(context, chatService, client)
        {
        }

        public override string Name => "/start";
        
        public override int State => UserDialogStates.Start;
        
        public override async Task ExecuteAsync(Message msg)
        {
            var user = new TelegramUser
            {
                ChatId = msg.Chat.Id,
                UserName = msg.Chat.Username,
                FirstName = msg.Chat.FirstName,
                LastName = msg.Chat.LastName,
                States = UserDialogStates.AddAdvert
            };

            await _context.TelegramUsers.AddAsync(user);
            await _context.SaveChangesAsync();
            
            _chatService.Chats.TryAdd(msg.Chat.Id, user);
            
            await _client.SendTextMessageAsync(msg.Chat.Id, $"Добро пожаловать, {user.UserName}\nВведите ссылку", replyMarkup: GetDefaultReplyButtons());
        }
    }
}