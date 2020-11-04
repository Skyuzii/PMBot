using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace PMBot.Core.Entities
{
    [Table(nameof(Advert))]
    public class Advert
    {
        public int Id { get; set; }

        public int UserId { get; set; }
        
        [ForeignKey(nameof(UserId))]
        public TelegramUser User { get; set; }

        public string Url { get; set; }
        
        public string Name { get; set; }

        public int LowPrice { get; set; }
        
        public int MaxPrice { get; set; }
        
        public bool Complete { get; set; }
        
        public bool IsActual { get; set; }

        public DateTime CreateDate { get; set; }
        
        public DateTime LastCheckDate { get; set; }
        
        public IList<AdvertPriceHistory> PriceHistory { get; set; }
    }
}