using System;

namespace PMBot.Application.Exceptions
{
    public class TelegramException : Exception
    {
        public long ChatId { get; }

        public TelegramException(string message, long chatId) : base(message)
        {
            ChatId = chatId;
        }
    }
}