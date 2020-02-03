using HtmlAgilityPack;
using System;
using System.Linq;
using System.Net.Http;
using static System.Console;
using System.IO;
using System.Collections.Generic;
using web_crawler_using_cSharp.Model;
using Newtonsoft;
using Newtonsoft.Json;

namespace web_crawler_using_cSharp
{
    class Program
    {
        LoadNextPage page = new LoadNextPage();
        static void Main(string[] args)
        {
            Program obj = new Program();
            obj.StartCrawling();
        }

        public void StartCrawling()
        {
            List<RowModel> rows = new List<RowModel>();
            int i = 1;
            var stringHtml =  File.ReadAllText("D:/مراکز درمانی طرف قرارداد - کمک رسان ایران.html");
            var htmlParser = new HtmlDocument();
            htmlParser.LoadHtml(stringHtml);
            var section = htmlParser
                .DocumentNode
                .Descendants("section")
                .FirstOrDefault(e => e.Id == "sos-medical-centers-table");
            var divs = section.Descendants(3).Where(e => e.HasClass("sos-table-row")).ToList();
            divs.ForEach((v) =>
            {
                //WriteLine("\ndiv\n");
                var childDivs = v.Descendants("div").Where(e => e.HasClass("sos-col")).ToList();
                RowModel row = new RowModel();
                for (int i = 0; i < childDivs.Count; i++)
                {
                    if (i == 0)
                    {
                        row.DoctorName = childDivs[0].InnerText.Trim();
                    }
                    else if (i == 1)
                    {
                        row.Job = childDivs[1].InnerText.Trim();
                    }
                    else if (i == 2)
                    {
                        row.Address = childDivs[2].InnerText.Trim();
                    }
                    else if (i == 4)
                    {
                        var div2 = childDivs[4].Descendants("div").FirstOrDefault(e => e.HasClass("sos-td"));
                        //var ii = div2.Descendants("i").FirstOrDefault(e => e.HasClass("fa fa-check-circle"));
                        row.CanIntroduce = div2.FirstChild.NextSibling.Attributes.FirstOrDefault(e => e.Name == "title").Value.Trim();
                    }
                    else if (i == 5)
                    {
                        var div2 = childDivs[5].Descendants("div").FirstOrDefault(e => e.HasClass("sos-td"));
                        //var ii = div2.Descendants("i").FirstOrDefault(e => e.HasClass("fa fa-check-circle"));
                        row.CanIntroduceOnline = div2.FirstChild.NextSibling.Attributes.FirstOrDefault(e => e.Name == "title").Value.Trim();
                    }
                    else if (i == 6)
                    {
                        var div2 = childDivs[6].Descendants("div").FirstOrDefault(e => e.HasClass("sos-td"));
                        //var ii = div2.Descendants("a").FirstOrDefault(e => e.HasClass("fancybx"));
                        row.MoreInfoUrl = div2.FirstChild.Attributes.FirstOrDefault(e => e.Name == "href").Value.Trim();
                    }
                }
                WriteLine(row.ToString());
                rows.Add(row);

                string json = JsonConvert.SerializeObject(rows);
                WriteLine(json);
            });
        }
    }
}
