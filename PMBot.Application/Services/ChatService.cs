using System;
using System.Collections.Concurrent;
using System.Linq;
using PMBot.Core;
using PMBot.Core.Entities;

namespace PMBot.Application.Services
{
    public class ChatService
    {
        public ConcurrentDictionary<long, TelegramUser> Chats { get; }

        public ChatService(ApplicationContext context)
        {
            Chats = new ConcurrentDictionary<long, TelegramUser>(
                context.TelegramUsers.ToDictionary(x => x.ChatId, y => y));
        }
    }
}