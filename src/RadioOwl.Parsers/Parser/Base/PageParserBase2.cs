using RadioOwl.Parsers.Parser.Data;
using RadioOwl.Parsers.Parser.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace RadioOwl.Parsers.Parser.Base
{
    /// <summary>
    /// 
    /// </summary>
    public abstract class PageParserBase2 : IPageParser2
    {
        /// <inheritdoc/>
        public abstract int Version { get; }

        /// <inheritdoc/>
        public abstract string[] ParseUrls { get; }



        public readonly List<string> LogMessages = new List<string>();


        protected readonly Action<string> LogAction;
        protected void Log(string message)
        {
            var fullMessage = $"{DateTime.Now:u} {message}";
            LogMessages.Add(fullMessage);
            LogAction?.Invoke(fullMessage);
            
        }


        public PageParserBase2(Action<string> logAction)
        {
            LogAction = logAction ?? throw new ArgumentNullException(nameof(logAction));
        }









        /// <inheritdoc/>
        public bool CanParse(string url)
        {
            if (string.IsNullOrEmpty(url)) return false;
            return ParseUrls.Any(p => url.Contains(p, StringComparison.InvariantCultureIgnoreCase));
        }



        public abstract ParserResult TryParse(string url, Action<string> log);











        ///// <summary>
        ///// Http klient
        ///// <para>Staticky viz: <see href="https://www.aspnetmonsters.com/2016/08/2016-08-27-httpclientwrong/">You're using HttpClient wrong and it is destabilizing your software</see></para>
        ///// </summary>
        //private static readonly HttpClient Client = new HttpClient();

        //protected (string content, HttpStatusCode? httpStatusCode, bool isSuccess) HttpDownload(string url)
        //{
        //    //var asyncDownloader = new AsyncDownloader();
        //    //var downloaderOutput = await asyncDownloader.GetString(url);
        //    //return downloaderOutput.DownloadOk ? downloaderOutput.Output : null;
        //    //using var client = new HttpClient();
        //    //string value = client.DownloadString(args[0]);
        //    //// Append url.
        //    //File.AppendAllText(args[1],
        //    //    string.Format("--- {0} ---\n", DateTime.Now) +
        //    //                value);


        //    var httpResponseMessage = Client.GetAsync(url).Result;
        //    if (httpResponseMessage == null)
        //        return (null, null, false);
        //    var content = httpResponseMessage.Content.ReadAsStringAsync().Result;
        //    return (content, httpResponseMessage.StatusCode, httpResponseMessage.IsSuccessStatusCode);
        //}




    }
}
