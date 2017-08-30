using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using HtmlAgilityPack;
using KP.WolParser.Models;

namespace KP.WolParser
{
    public class WolParser
    {
        public static async Task PopulateEntry(InsightEntry entry)
        {
            var html = new HtmlDocument();
            var doc = await new HttpClient().GetStringAsync(entry.Uri);
            html.LoadHtml(doc);
            var root = html.DocumentNode;
            var article = root.Descendants("article").FirstOrDefault();

            entry.Content = article.InnerText;

            Console.WriteLine($"{entry.Name}: {entry.SizeInChars}");
        }

        public static async Task<List<InsightEntry>> GetInsightIndexForLetter(Uri uri)
        {
            var html = new HtmlDocument();
            var doc = await new HttpClient().GetStringAsync(uri);
            html.LoadHtml(doc);
            var root = html.DocumentNode;
            var lis = root.Descendants("li")
                .Where(n => n.GetAttributeValue("class", "").Equals("row card navCard"));

            var index = new List<InsightEntry>();

            foreach (var item in lis)
            {
                var a = item.ChildNodes.First();
                var div = a.ChildNodes.First(s => s.Name == "div");
                var letter = div.FirstChild.InnerText;
                var page = div.ChildNodes[1]?.InnerText;

                var url = new Uri("https://wol.jw.org" + a.Attributes["href"].Value);

                index.Add(new InsightEntry { Name = letter, Uri = url, Page = page });
                //Console.WriteLine($"{letter} : {url}");
            }

            return index;
        }
    }
}
