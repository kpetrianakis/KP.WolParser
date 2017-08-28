using System;
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
            Console.WriteLine("Read All Insight?");
            var read = Console.ReadLine();
            if (read != "Y") return;
            return;
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