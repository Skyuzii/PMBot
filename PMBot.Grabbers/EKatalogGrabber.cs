using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using HtmlAgilityPack;
using PMBot.Core.Entities;
using PMBot.Grabbers.Abstracts;
using PMBot.Grabbers.Interfaces;

namespace PMBot.Grabbers
{
    public class EKatalogGrabber : BaseGrabber, IGrabber, IPageGrabber
    {
        public string Url => "https://www.e-katalog.ru/";

        public async Task<Advert> ParseAdvert(string url)
        {
            var doc = await GetHtmlDocumentByUrl(url);
           
            var advert = new Advert
            {
                Url = url
            };
            
            Name(advert, doc);
            LowPrice(advert, doc);

            return advert;
        }

        public void LowPrice(Advert advert, HtmlDocument doc)
        {
            var node = doc.DocumentNode
               .Descendants("span")
               .FirstOrDefault(x => x.Attributes["itemprop"] != null && x.Attributes["itemprop"].Value == "lowPrice");

            if (node == null)
            {
                OnlyOnePrice(advert, doc);
            }
            else
            {
                var text = node.InnerText.Replace(" ", string.Empty);
                advert.LowPrice = int.Parse(text);
            }
        }

        private void OnlyOnePrice(Advert advert, HtmlDocument doc)
        {
            var node = doc.DocumentNode
                .Descendants("span")
                .FirstOrDefault(x => x.Attributes["price_marker"] != null);

            if (node != null)
            {
                var text = node.InnerText.Replace(" ", string.Empty);
                advert.LowPrice = int.Parse(text);
            }
        }

        public void Name(Advert advert, HtmlDocument doc)
        {
            var node = doc.DocumentNode
               .Descendants("h1")
               .FirstOrDefault(x => x.Attributes["itemprop"] != null && x.Attributes["itemprop"].Value == "name");

            if (node != null)
            {
                advert.Name = node.InnerText;
            }
        }
    }
}