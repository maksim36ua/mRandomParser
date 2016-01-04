using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using HtmlAgilityPack;

namespace MrandomParser
{
    class Program
    {
        private static async void Parsing(string website)
        {
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
                    var tmp = (userLink.Remove(0,14)).Insert(0, "@");
                    Console.WriteLine(tmp);
                }
            }
            catch (Exception)
            {
                Console.WriteLine("Error occured");
            }
        }

        static void Main(string[] args)
        {
            Console.WriteLine("Enter MRandom URL");
            var url = Console.ReadLine();
            Parsing(url);
            Console.Read();
        }
    }
}
