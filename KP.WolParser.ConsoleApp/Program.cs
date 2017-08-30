using System;
using System.Linq;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;
using Newtonsoft.Json;
using KP.WolParser.Models;

namespace KP.WolParser.ConsoleApp
{
    class Program
    {
        static async Task Main(string[] args)
        {
            CultureInfo.DefaultThreadCurrentCulture = new CultureInfo("el-GR");
            var utf8 = System.Text.Encoding.UTF8;
            Console.OutputEncoding = utf8;
            string filePath = AppContext.BaseDirectory + "\\InsightEntries.json";
            string filePath2 = AppContext.BaseDirectory + "\\InsightEntriesFull.json";

            List<InsightEntry> insightEntries = new List<InsightEntry>();

            if (File.Exists(filePath2))
            {
                var insightEntriesJson = File.ReadAllText(filePath2, utf8);
                insightEntries = JsonConvert.DeserializeObject<List<InsightEntry>>(insightEntriesJson);
            }
            else
            {
                var letters = await WolParser.GetInsightIndexForLetter(new Uri("https://wol.jw.org/el/wol/lv/r11/lp-g/0/2"));

                foreach (var letter in letters)
                {
                    insightEntries.AddRange(await WolParser.GetInsightIndexForLetter(letter.Uri));
                }
                File.WriteAllText(filePath, JsonConvert.SerializeObject(insightEntries), utf8);
            }


            Console.WriteLine($"Sum of pages {insightEntries.Sum(p => p.SizeInPages)}");

            Console.WriteLine($"Total {insightEntries.Count}");
            Console.WriteLine($"Total of one pagers {insightEntries.Count(p => p.SizeInPages == 1)}");
            Console.WriteLine($"Total of > 2 pagers {insightEntries.Count(p => p.SizeInPages > 2)}");
            Console.WriteLine($"Total of > 9 pagers {insightEntries.Count(p => p.SizeInPages > 9)}");
            Console.WriteLine($"Total of > 9 pagers \n{string.Join("\n ", insightEntries.Where(p => p.SizeInPages > 9).Select(pp => pp.Name))}");
            Console.WriteLine($"Total of < 10 pagers >5 {insightEntries.Count(p => p.SizeInPages > 5 && p.SizeInPages < 10)}");
            Console.WriteLine($"Total of < 10 pagers >5 \n{string.Join("\n ", insightEntries.Where(p => p.SizeInPages > 5 && p.SizeInPages < 10).Select(pp => pp.Name))}");


            Console.WriteLine("Read All Insight?");
            var read = Console.ReadLine();

            if (read != "Y") return;

            List<Task> listOfTasks = new List<Task>();
            try
            {
                foreach (var entry in insightEntries)
                {
                    listOfTasks.Add(WolParser.PopulateEntry(entry));
                }
                await Task.WhenAll(listOfTasks);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                //
            }
            File.WriteAllText(filePath2, JsonConvert.SerializeObject(insightEntries), utf8);
            Console.ReadLine();
        }
    }
}