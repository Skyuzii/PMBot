using HtmlAgilityPack;
using PMBot.Core.Entities;

namespace PMBot.Grabbers.Interfaces
{
    public interface IPageGrabber
    {
        /// <summary>
        /// Парсинг названия объявления
        /// </summary>
        /// <param name="advert"></param>
        void Name(Advert advert, HtmlDocument doc);

        /// <summary>
        /// Парсинг низкой цены
        /// </summary>
        /// <param name="advert"></param>
        void LowPrice(Advert advert, HtmlDocument doc);
    }
}