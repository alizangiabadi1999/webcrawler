using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace web_crawler_using_cSharp
{
    class LoadNextPage
    {
        public async Task<string> Next(int pageNum)
        {
            var url = "https://www.iranassistance.com/CareCenter/Search";
            var http = new HttpClient();
            var content = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("Status","true"),
                new KeyValuePair<string, string>("page", pageNum.ToString())
            });
            var page = await http.PostAsync(url, content);
            return await page.Content.ReadAsStringAsync();
        } 
    }
}
