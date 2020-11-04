using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PMBot.Application.Commands;
using PMBot.Application.Services;
using PMBot.Core;
using PMBot.Grabbers;
using PMBot.Grabbers.Interfaces;
using Telegram.Bot;

namespace PMBot.Application
{
    public static class Configure
    {
        public static void AddApplication(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<ApplicationContext>(options => 
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"), x => x.MigrationsAssembly("PMBot.Core")));
            
            services.AddTelegramBot(configuration);
            services.AddServices();
            services.AddCommands();
            services.AddGrabbers();
        }

        private static void AddTelegramBot(this IServiceCollection services, IConfiguration configuration)
        {
            var client = new TelegramBotClient(configuration["TelegramToken"]);
            services.AddSingleton<ITelegramBotClient>(client);
        }

        private static void AddServices(this IServiceCollection services)
        {
            services.AddSingleton(new ChatService(services.BuildServiceProvider().GetRequiredService<ApplicationContext>()));
            services.AddTransient<TelegramService>();
        }

        private static void AddCommands(this IServiceCollection services)
        {
            services.AddTransient<BaseCommand, StartCommand>();
            services.AddTransient<BaseCommand, AddAdvertCommand>();
            services.AddTransient<BaseCommand, MyAdvertsCommand>();
        }

        private static void AddGrabbers(this IServiceCollection services)
        {
            services.AddTransient<IGrabber, EKatalogGrabber>();
        }
    }
}