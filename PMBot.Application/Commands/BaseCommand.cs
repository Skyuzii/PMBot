using System;
using System.Threading.Tasks;
using PMBot.Application.Services;
using PMBot.Core;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace PMBot.Application.Commands
{
    public abstract class BaseCommand
    {
        protected readonly ChatService _chatService;
        
        protected readonly ITelegramBotClient _client;
        
        protected readonly ApplicationContext _context;

        public BaseCommand(ApplicationContext context, ChatService chatService, ITelegramBotClient client)
        {
            _client = client;
            _context = context;
            _chatService = chatService;
        }
        
        public abstract string Name { get; }
        
        public abstract int State { get; }

        public abstract Task ExecuteAsync(Message msg);
        
        protected ReplyKeyboardMarkup GetDefaultReplyButtons()
        {
            return new ReplyKeyboardMarkup(new []
            {
                new []
                {
                    new KeyboardButton { Text = "Мои объявления" },
                }
            });
        }
    }
}