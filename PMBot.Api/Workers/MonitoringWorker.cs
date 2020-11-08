using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PMBot.Core;
using PMBot.Core.Entities;
using PMBot.Grabbers.Interfaces;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;

namespace PMBot.Api.Workers
{
    public class MonitoringWorker : IHostedService
    {
        private Timer _timer;
        private readonly IList<IGrabber> _grabbers;
        private readonly ITelegramBotClient _client;
        private readonly IServiceScopeFactory _scopeFactory;

        public MonitoringWorker(
            ITelegramBotClient client,
            IEnumerable<IGrabber> grabbers,
            IServiceScopeFactory scopeFactory)
        {
            _client = client;
            _scopeFactory = scopeFactory;
            _grabbers = grabbers.ToList();
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            _timer = new Timer(async state =>
            {
                using var scope = _scopeFactory.CreateScope();
                var context = scope.ServiceProvider.GetRequiredService<ApplicationContext>();

                var tasks = new List<Task>();
                var semaphore = new SemaphoreSlim(5, 5);

                context.Adverts
                    .Include(x => x.User)
                    .Where(x => x.LastCheckDate >= DateTime.Now.AddHours(-1))
                    .ToList()
                    .ForEach(async advert =>
                    {
                        await semaphore.WaitAsync();
                        tasks.Add(CheckPriceAsync(advert, context));
                        semaphore.Release();
                    });

                await Task.WhenAll(tasks);

            }, null, TimeSpan.Zero, TimeSpan.FromMinutes(10));
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
        
        private async Task CheckPriceAsync(Advert advert, ApplicationContext context)
        {
            var grabber = _grabbers.First(x => advert.Url.Contains(x.Url));

            var freshAdvert = await grabber.ParseAdvert(advert.Url);
            if (freshAdvert.LowPrice != default && freshAdvert.LowPrice != advert.LowPrice)
            {
                await NotifyUserOfPriceCut(advert, freshAdvert, DateTime.Now);

                advert.LowPrice = freshAdvert.LowPrice;
                advert.LastCheckDate = DateTime.Now;

                context.Attach(advert);
                context.Entry(advert).State = EntityState.Modified;

                var priceHistory = new AdvertPriceHistory
                {
                    AdvertId = advert.Id,
                    Price = advert.LowPrice,
                    ChangeDate = advert.LastCheckDate
                };

                await context.AdvertPriceHistories.AddAsync(priceHistory);
                await context.SaveChangesAsync();
            }
        }

        private async Task NotifyUserOfPriceCut(Advert advert, Advert freshAdvert, DateTime changeDate)
        {
            var sb = new StringBuilder()
                .AppendLine($"Цена на объявление снизилась: <a href='{advert.Url}'>{advert.Name}</a>")
                .AppendLine($"Было: {advert.LowPrice}р")
                .AppendLine($"Стало: {freshAdvert.LowPrice}р")
                .AppendLine($"Дата изменения: {changeDate}");
            
            await _client.SendTextMessageAsync(advert.User.ChatId, sb.ToString(), ParseMode.Html);
        }
    }
}