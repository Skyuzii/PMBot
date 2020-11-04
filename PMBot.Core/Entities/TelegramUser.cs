using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace PMBot.Core.Entities
{
    [Table(nameof(TelegramUser))]
    public class TelegramUser
    {
        public int Id { get; set; }
        
        public string UserName { get; set; }
        
        public string FirstName { get; set; }
        
        public string LastName { get; set; }

        public int States { get; set; }

        public long ChatId { get; set; }
        
        public IList<Advert> Adverts { get; set; }
    }

}