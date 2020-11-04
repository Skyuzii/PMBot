using System.Threading.Tasks;
using PMBot.Core.Entities;

namespace PMBot.Grabbers.Interfaces
{
    public interface IGrabber
    {
        /// <summary>
        /// Ссылка сайта
        /// </summary>
        public string Url { get; }

        /// <summary>
        /// Парсинг объявления
        /// </summary>
        /// <param name="url"></param>
        Task<Advert> ParseAdvert(string url);
    }
}