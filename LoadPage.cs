using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using System.Net;
using System.IO;
using System;

namespace webcrawler
{
    public class LoadPage
    {
        public async Task<string> load(string link)
        {
            var url = link;

            WebClient client = new WebClient();

            // Add a user agent header in case the
            // requested URI contains a query.

            client.Headers.Add("user-agent", "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.2; .NET CLR 1.0.3705;)");

            Stream data = await client.OpenReadTaskAsync(new Uri(link));
            StreamReader reader = new StreamReader(data);
            string s = reader.ReadToEnd();
            // Console.WriteLine(s);
            data.Close();
            reader.Close();

            // var http = new HttpClient();
            // http.DefaultRequestHeaders.Add("Accept-Language", "fa-IR");

            // var content = new FormUrlEncodedContent(new[]
            // {
            //     new KeyValuePair<string, string>("Status","true"),
            //     new KeyValuePair<string, string>("page", pageNum.ToString())
            // });
            // var page = await http.GetAsync(url);
            return s;
        }
    }
}