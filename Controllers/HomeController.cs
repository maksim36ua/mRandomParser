using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;
using HtmlAgilityPack;
using System.Net.Http;
using System.Text;
using System.Net;

namespace mRandomRaffleParser.Controllers
{
    public class HomeController : Controller
    {
        private List<string> winnersList = new List<string>() { "Paste your link in the textbox higher" };
        public IActionResult Index()
        {
            return View(winnersList);            
        }

        [HttpPost]
        public IActionResult Index(string website)
        {
            winnersList = Parsing(website).Result;
            return View(winnersList);
        }

        public IActionResult Error()
        {
            return View();
        }

        private async Task<List<string>> Parsing(string website)
        {
            List<string> winnersList = new List<string>();
            try
            {
                HttpClient http = new HttpClient();
                var response = await http.GetByteArrayAsync(website);
                String source = Encoding.GetEncoding("utf-8").GetString(response, 0, response.Length - 1);
                source = WebUtility.HtmlDecode(source);

                HtmlDocument result = new HtmlDocument();
                result.LoadHtml(source);

                List<HtmlNode> toftitle = result.DocumentNode.Descendants().Where
                    (x => (x.Name == "div" && x.Attributes["class"] != null && x.Attributes["class"].Value.Contains("col-md-6"))).ToList();

                List<string> id = new List<string>();

                for (int i = 0; i < toftitle.Count; i++)
                {
                    var span = toftitle[i].Descendants("h4").ToList();
                    var link = span[0].Descendants("a").ToList()[0].GetAttributeValue("href", null);
                    id.Add(link);
                }

                foreach (var userLink in id)
                {
                    var tmp = (userLink.Remove(0, 14)).Insert(0, "@");
                    winnersList.Add(tmp);
                }
                return winnersList;

            }
            catch (Exception)
            {
                winnersList.Add("Wrong link or bad Internet connection");
                return winnersList;
            }
        }
    }
}
