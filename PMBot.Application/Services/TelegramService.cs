using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PMBot.Application.Commands;
using PMBot.Application.Exceptions;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace PMBot.Application.Services
{
    public class TelegramService
    {
        private readonly ChatService _chatService;
        private readonly ITelegramBotClient _client;
        private readonly List<BaseCommand> _commands;

        public TelegramService(
            ChatService chatService,
            ITelegramBotClient client,
            IEnumerable<BaseCommand> commands)
        {
            _client = client;
            _chatService = chatService;
            _commands = commands.ToList();
        }

        public async Task Handle(Update update)
        {
            var msg = update.Message;
            if (msg == null) return;

            if (!_chatService.Chats.ContainsKey(msg.Chat.Id))
            {
                await _commands.First(x => x is StartCommand).ExecuteAsync(msg);
            }
            else
            {
                var cmd = _commands.FirstOrDefault(x => msg.Text.Trim().Contains(x.Name))
                          ?? _commands.FirstOrDefault(x => x.State == _chatService.Chats[msg.Chat.Id].States);

                if (cmd == null)
                {
                    throw new TelegramException("Команда не найдена", msg.Chat.Id);
                }

                await cmd.ExecuteAsync(msg);
            }
        }
    }
}