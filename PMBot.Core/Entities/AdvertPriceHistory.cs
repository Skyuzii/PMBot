using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace PMBot.Core.Entities
{
    [Table(nameof(AdvertPriceHistory))]
    public class AdvertPriceHistory
    {
        public int Id { get; set; }
        
        public int AdvertId { get; set; }
        
        public int Price { get; set; }

        public string Type { get; set; }

        public DateTime ChangeDate { get; set; }
    }

}