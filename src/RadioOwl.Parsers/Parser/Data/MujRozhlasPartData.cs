using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RadioOwl.Parsers.Parser.Data
{
    public class MujRozhlasPartData
    {
        public MujRozhlasData MujRozhlasData { get; init; }

        /// <summary>
        /// Zdrojova JSON data 
        /// </summary>
        public JObject Json { get; init; }

        /// <summary>
        /// Id části
        /// </summary>
        public string Uuid { get; init; }

        public string DataId { get; internal set; }
        public string DataType { get; internal set; }
        public int? No { get; internal set; }
        public string Title { get; internal set; }
        public object ShortTitle { get; internal set; }
        public string DescriptionHtml { get; internal set; }
        public string Description { get; internal set; }
        public string AudioLink { get; internal set; }

        /// <summary>
        /// Cesta kam bude stazeno mp4
        /// </summary>
        public string Mp4a { get; set; }

        /// <summary>
        /// Cesta kam bude prekonvertovano mp3 z mp4
        /// </summary>
        public string Mp3 { get; set; }


        /// <summary>
        /// Exit code od processu, kterym poustim BAT soubor stahujici a konverujici stream
        /// </summary>
        public int CmdProcessExitCode { get; set; }

    }
}
