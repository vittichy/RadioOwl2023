using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RadioOwl.Parsers.Parser.Data
{
    public class ParserResult
    {
        /// <summary>
        /// Zdrojové url
        /// </summary>
        public readonly string Url;


        /// <summary>
        /// Html stažené ze zdrojového <see cref="Url">Url</see>
        /// </summary>
        public string Html;


        /// <summary>
        /// 
        /// </summary>
        public ParserResultState ParserResultState;



        public MujRozhlasData MujRozhlasData;


        /// <summary>
        /// Konstruktor
        /// </summary>
        public ParserResult(string url)
        {
            Url = url;
        }
    }
}
