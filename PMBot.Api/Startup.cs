using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PMBot.Api.Workers;
using PMBot.Application;
using PMBot.Application.Exceptions;
using Telegram.Bot;

namespace PMBot.Api
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }
        
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers()
                .AddNewtonsoftJson();
            
            services.AddApplication(Configuration);
            services.AddHostedService<MonitoringWorker>();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ITelegramBotClient client, ILogger<Startup> logger)
        {
            client.SetWebhookAsync($"{Configuration["AppUrl"]}/api/bot");
            
            app.UseRouting();
            app.UseExceptionHandler(x => x.Run(context => ExceptionHandler(context, client, logger)));
            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }

        private async Task ExceptionHandler(HttpContext context, ITelegramBotClient client, ILogger<Startup> logger)
        {
            var exception = context.Features.Get<IExceptionHandlerPathFeature>().Error;

            if (exception is TelegramException telegramException)
            {
                await client.SendTextMessageAsync(telegramException.ChatId, telegramException.Message);
            }
            else
            {
                logger.LogError(exception.Message);
            }
        }
    }
}