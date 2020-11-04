using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using HtmlAgilityPack;

namespace PMBot.Grabbers.Abstracts
{
    public abstract class BaseGrabber
    {
        protected async Task<HtmlDocument> GetHtmlDocumentByUrl(string url)
        {
            using var httpClient = new HttpClient();

            var html = await httpClient.GetStringAsync(url);

            var doc = new HtmlDocument();
            doc.LoadHtml(HttpUtility.HtmlDecode(html));

            return doc;
        }
    }
}