using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace RadioOwl.Parsers.Parser.Helpers
{
    public  class StaticHttpClient
    {

        /// <summary>
        /// Http klient
        /// <para>Staticky viz: <see href="https://www.aspnetmonsters.com/2016/08/2016-08-27-httpclientwrong/">You're using HttpClient wrong and it is destabilizing your software</see></para>
        /// </summary>
        private static readonly HttpClient Client = new HttpClient();




        public static (string content, HttpStatusCode? httpStatusCode, bool isSuccess) HttpDownload(string url)
        {
            //var asyncDownloader = new AsyncDownloader();
            //var downloaderOutput = await asyncDownloader.GetString(url);
            //return downloaderOutput.DownloadOk ? downloaderOutput.Output : null;
            //using var client = new HttpClient();
            //string value = client.DownloadString(args[0]);
            //// Append url.
            //File.AppendAllText(args[1],
            //    string.Format("--- {0} ---\n", DateTime.Now) +
            //                value);


            var httpResponseMessage = Client.GetAsync(url).Result;
            if (httpResponseMessage == null)
                return (null, null, false);
            var content = httpResponseMessage.Content.ReadAsStringAsync().Result;
            return (content, httpResponseMessage.StatusCode, httpResponseMessage.IsSuccessStatusCode);
        }



        public static void HttpDownload(string url, out string content)
        {
            var httpContent = HttpDownload(url);
            content = httpContent.content;
        }
    }
}
