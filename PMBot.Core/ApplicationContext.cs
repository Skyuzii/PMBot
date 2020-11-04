using Microsoft.EntityFrameworkCore;
using PMBot.Core.Entities;

namespace PMBot.Core
{
    public class ApplicationContext : DbContext
    {
        public ApplicationContext(DbContextOptions<ApplicationContext> options) : base(options)
        {
        }
        
        public DbSet<Advert> Adverts { get; set; }
        
        public DbSet<TelegramUser> TelegramUsers { get; set; }
        
        public DbSet<AdvertPriceHistory> AdvertPriceHistories { get; set; }
    }
}