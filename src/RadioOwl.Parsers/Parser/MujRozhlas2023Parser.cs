using HtmlAgilityPack;
using RadioOwl.Parsers.Parser.Base;
using RadioOwl.Parsers.Parser.Data;
using RadioOwl.Parsers.Parser.Helpers;
using System;

namespace RadioOwl.Parsers.Parser
{
    internal class MujRozhlas2023Parser : MujRozhlasParserBase
    {
        //private readonly  MujRozhlasHelper _mujRozhlasHelper = new MujRozhlasHelper();

        public MujRozhlas2023Parser(Action<string> logAction) : base(logAction)        
        {            
        }

        public override int Version => 1;

        public override string[] ParseUrls { get { return new string[] { "mujrozhlas.cz" }; } }

        public override ParserResult TryParse(string url, Action<string> log)
        {
            var result = new ParserResult(url);

            try
            {
                var (html, httpStatusCode, isSucces) = StaticHttpClient.HttpDownload(url);
                result.Html = html;
                if (!isSucces) throw new ParserException("Download error");
                //{
                //    result.ParserResultState = ParserResultState.DownloadError;
                //    return result;
                //}
                Log($"Download html status {httpStatusCode}/{isSucces} {result.Html?.Length}");




                var mujRozhlasData = new MujRozhlasData();


                // RID nelze zjistit pouze z url poradu, napr 'https://www.mujrozhlas.cz/lide/martin-c-putna' zadne RID nevraci!
                mujRozhlasData.RId = GetRID(result.Html);
                if (string.IsNullOrEmpty(mujRozhlasData.RId)) throw new ParserException("RID not found");
                Log($"RID: {mujRozhlasData.RId}");

                // html nemusi byt validni xml, takze je potreba pro parsovani pouzit Html Agility Pack
                var htmlDoc = new HtmlDocument();
                htmlDoc.LoadHtml(result.Html);

                // načtení hlavního popisu k pořadu
                var detailDescriptionInnerText = htmlDoc.DocumentNode.SelectSingleNode(@".//div[@class='b-detail__description']//p").InnerText;
                var detailDescription = HtmlEntity.DeEntitize(detailDescriptionInnerText);

                // zde asi jedine misto, kde zjistim pocet epizod?
                // dohledat <script> pod <div> kde je to jako kus JS zdrojaku s definovanou JSON promennou, kterou z toho zkusim vykousnou
                mujRozhlasData.MujRozhlas2020SiteInfo = GetContentSerialAllParts(htmlDoc);

                if (string.IsNullOrEmpty(mujRozhlasData.MujRozhlas2020SiteInfo.SiteEntityBundle))
                {
                    Log($"Nedohledáno SiteEntityBundle:'{mujRozhlasData.MujRozhlas2020SiteInfo.ContentId}");
                    result.ParserResultState = ParserResultState.SiteEntityBundleNotFound;
                }
                else
                {
                    Log($"Dohledáno SiteEntityBundle:'{mujRozhlasData.MujRozhlas2020SiteInfo.SiteEntityBundle}', Parts:{mujRozhlasData.MujRozhlas2020SiteInfo.ContentSerialAllParts}, Label:{mujRozhlasData.MujRozhlas2020SiteInfo.SiteEntityLabel}");
                    switch (mujRozhlasData.MujRozhlas2020SiteInfo.SiteEntityBundle)
                    {
                        case "episode":
                            throw new NotImplementedException();
                            // neni serial, jen jednodilny porad, ContentSerialAllParts je null
                            // ParseEpisode(mujRozhlasData.MujRozhlas2020SiteInfo);
                            break;
                        case "show":
                            throw new NotImplementedException();
                            // napr Spirituala https://www.mujrozhlas.cz/spirituala - stranka obsahuje jednotlive dily 
                            //await ParseShowBundleAsync(radioData, mujRozhlas2020SiteInfo, rid);
                            break;
                        case "serial":
                            // napr cetba s hvezdickou: https://www.mujrozhlas.cz/cetba-s-hvezdickou/zenska-na-1000deg-drsna-i-humorna-zpoved-prezidentske-vnucky-z-islandu
                            ParseSerial(mujRozhlasData); //, mujRozhlasData.MujRozhlas2020SiteInfo);
                            break;
                        default:
                            throw new ParserException($"Unknown SiteEntityBundle:'{mujRozhlasData.MujRozhlas2020SiteInfo.SiteEntityBundle}'");
                    }
                    result.MujRozhlasData = mujRozhlasData;
                    result.ParserResultState = ParserResultState.Success;
                }
            }
            catch(ParserException parserException)
            {
                Log($"Parser error: {parserException?.Message}");
                result.ParserResultState = ParserResultState.ParserError;

            }
            catch (Exception ex)
            {
                Log($"Unexpected error: {ex?.Message}");
                result.ParserResultState = ParserResultState.UnexpectedError;
            }
            return result;
        }

    }
}
