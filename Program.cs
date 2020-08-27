using System;
using System.Linq;
using System.Net.Http;
using HtmlAgilityPack;
using static System.Console;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Newtonsoft;
using Newtonsoft.Json;
using web_crawler_using_cSharp.Model;
using webcrawler;

namespace web_crawler_using_cSharp
{
    class Program
    {
        static string TypesPage;
        static List<string> typePage = new List<string>();
        static Random random = new Random();
        LoadNextPage page = new LoadNextPage();
        static void Main(string[] args)
        {
            Program obj = new Program();
            obj.StartCrawling();
        }

        public async Task LoadTypePaqe(string link)
        {
            try
            {
                LoadPage loadPage = new LoadPage();
                await Task.Delay(random.Next(0, 5) * 5000 + 1);
                var pageHtml = await loadPage.load(link);
                if (pageHtml.Contains("Server Error"))
                {
                    Console.WriteLine("server error {0}", link);
                    await LoadTypePaqe(link);
                }
                else
                {

                    typePage.Add(pageHtml);
                    Console.WriteLine("success in link {0}", link);
                }
            }
            catch (System.Exception e)
            {
                Console.WriteLine("problem in link {0} message: {1}", link, e.InnerException != null ? e.InnerException.Message : e.Message);
                await LoadTypePaqe(link);
            }

        }

        // public async Task LoadEmailPaqe(string link)
        // {
        //     try
        //     {
        //         LoadPage loadPage = new LoadPage();
        //         var pageHtml = await loadPage.load(link);
        //         Console.WriteLine(link);
        //         Console.WriteLine(pageHtml);
        //         Console.WriteLine("\n\n\n\n\n\n\n\n");

        //         typePage.Add(pageHtml);
        //     }
        //     catch (System.Exception)
        //     {

        //     }

        // }

        public static async Task GelTypes()
        {
            LoadPage loadPage = new LoadPage();
            var typesPage = await loadPage.load("https://ecomotive.ir/startups-list");
            Program.TypesPage = typesPage;
        }

        public void StartCrawling()
        {
            // load types page
            Program.GelTypes().Wait();

            var htmlParser = new HtmlDocument();
            htmlParser.LoadHtml(Program.TypesPage);

            var links = htmlParser
                .DocumentNode
                .SelectNodes("//a[@class='" + "link1" + "']").ToList();

            List<string> typeLinks = new List<string>();

            foreach (var link in links)
            {
                Console.WriteLine(link.Attributes["href"].Value);
                typeLinks.Add(link.Attributes["href"].Value);
            }

            List<Task> tasks = new List<Task>();
            foreach (var link in typeLinks)
            {
                tasks.Add(LoadTypePaqe(link));
            }

            Task.WaitAll(tasks.ToArray());
            List<string> emailPages = new List<string>();

            typePage.Where(e => e != null).ToList().ForEach(
                page =>
                {
                    Console.WriteLine("page {0}", page == null);
                    var htmlParser2 = new HtmlDocument();
                    htmlParser2.LoadHtml(page);
                    var nodes = htmlParser2
                        .DocumentNode
                        .SelectNodes("//a[@class='" + "test" + "']");
                    var links = nodes != null ? nodes.ToList() : new List<HtmlNode>();

                    links.ForEach(
                        link =>
                        {
                            if (link.HasAttributes && link.Attributes["href"] != null)
                            {
                                emailPages.Add(link.Attributes["href"].Value);
                            }

                        }
                    );

                }
            );

            List<Task> tasks2 = new List<Task>();
            emailPages.Remove("https://list.ecomotive.ir/company/%D9%85%D8%B1%DA%A9%D8%B2%D8%A7%D8%B7%D9%84%D8%A7%D8%B9%D8%A7%D8%AA_%D8%AA%D8%B5%D9%88%DB%8C%D8%B1%DB%8C_%D8%A7%D9%85%D9%84%D8%A7%DA%A9_%28%D9%85%D8%A7%D8%AA%D8%A7%29");
            emailPages.Remove("https://list.ecomotive.ir/company/%D8%A8%D8%A7%D9%86%DA%A9_%D8%A7%D8%B7%D9%84%D8%A7%D8%B9%D8%A7%D8%AA_%D9%85%D8%B3%DA%A9%D9%86_%D8%A7%DB%8C%D8%B1%D8%A7%D9%86_%28%D8%A8%D8%A7%D9%85%D8%A7%29");
            foreach (var link in emailPages)
            {
                tasks2.Add(LoadTypePaqe(link.Contains("https") || link.Contains("http") ? link : ("https:" + link)));
                Console.WriteLine("processing {0} ", link);

            }

            typePage = new List<string>();
            Task.WaitAll(tasks2.ToArray());

            Console.WriteLine("all pages downloaded {0}", typePage.Count);

            List<RowModel> rows = new List<RowModel>();

            typePage.ForEach(
                page =>
                {
                    Console.WriteLine("page {0}", page);
                    var htmlParser2 = new HtmlDocument();
                    htmlParser2.LoadHtml(page);
                    var nodes = htmlParser2
                        .DocumentNode
                        .SelectNodes("//a[@class='" + "link_information mgh_ltr" + "']");

                    var nodes2 = htmlParser2
                        .DocumentNode
                        .SelectNodes("//a[@class='" + "text_description col-12 col-sm-auto" + "']");

                    var links = nodes != null ? nodes.ToList() : new List<HtmlNode>();
                    var links2 = nodes2 != null ? nodes2.ToList() : new List<HtmlNode>();
                    links.AddRange(links2);
                    Console.WriteLine("nodes found {0}", links.Count);

                    links.ForEach(
                        link =>
                        {
                            if (link.Attributes["href"] != null)
                            {
                                var row = new RowModel();
                                var href = link.Attributes["href"].Value;
                                var innertext = link.InnerText;
                                if (href.Contains("mailto"))
                                {
                                    row.email = href;
                                }
                                if (href.Contains("http"))
                                {
                                    row.site = href;
                                }
                                if (innertext.Contains("98"))
                                {
                                    row.phone = innertext;
                                }

                                rows.Add(row);
                            }
                            Console.WriteLine("page link {0} ", link.Attributes["href"] != null ? link.Attributes["href"].Value : "no link");
                        }
                    );
                }
            );

            string json = JsonConvert.SerializeObject(rows);
            WriteLine(json);

            //write string to file
            System.IO.File.WriteAllText(@"./ecomotive.txt", json);

            // List<RowModel> rows = new List<RowModel>();
            // int i = 1;
            // var stringHtml =  File.ReadAllText("D:/مراکز درمانی طرف قرارداد - کمک رسان ایران.html");
            // var divs = section.Descendants(3).Where(e => e.HasClass("sos-table-row")).ToList();
            // divs.ForEach((v) =>
            // {
            //     //WriteLine("\ndiv\n");
            //     var childDivs = v.Descendants("div").Where(e => e.HasClass("sos-col")).ToList();
            //     RowModel row = new RowModel();
            //     for (int i = 0; i < childDivs.Count; i++)
            //     {
            //         if (i == 0)
            //         {
            //             row.DoctorName = childDivs[0].InnerText.Trim();
            //         }
            //         else if (i == 1)
            //         {
            //             row.Job = childDivs[1].InnerText.Trim();
            //         }
            //         else if (i == 2)
            //         {
            //             row.Address = childDivs[2].InnerText.Trim();
            //         }
            //         else if (i == 4)
            //         {
            //             var div2 = childDivs[4].Descendants("div").FirstOrDefault(e => e.HasClass("sos-td"));
            //             //var ii = div2.Descendants("i").FirstOrDefault(e => e.HasClass("fa fa-check-circle"));
            //             row.CanIntroduce = div2.FirstChild.NextSibling.Attributes.FirstOrDefault(e => e.Name == "title").Value.Trim();
            //         }
            //         else if (i == 5)
            //         {
            //             var div2 = childDivs[5].Descendants("div").FirstOrDefault(e => e.HasClass("sos-td"));
            //             //var ii = div2.Descendants("i").FirstOrDefault(e => e.HasClass("fa fa-check-circle"));
            //             row.CanIntroduceOnline = div2.FirstChild.NextSibling.Attributes.FirstOrDefault(e => e.Name == "title").Value.Trim();
            //         }
            //         else if (i == 6)
            //         {
            //             var div2 = childDivs[6].Descendants("div").FirstOrDefault(e => e.HasClass("sos-td"));
            //             //var ii = div2.Descendants("a").FirstOrDefault(e => e.HasClass("fancybx"));
            //             row.MoreInfoUrl = div2.FirstChild.Attributes.FirstOrDefault(e => e.Name == "href").Value.Trim();
            //         }
            //     }
            //     WriteLine(row.ToString());
            //     rows.Add(row);

            // string json = JsonConvert.SerializeObject(rows);
            // WriteLine(json);
            // });
        }
    }
}