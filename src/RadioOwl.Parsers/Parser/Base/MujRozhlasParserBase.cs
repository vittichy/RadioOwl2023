using Dtc.Html.Html;
using HtmlAgilityPack;
using Newtonsoft.Json.Linq;
using RadioOwl.Parsers.Data.Factory;
using RadioOwl.Parsers.Data;
using RadioOwl.Parsers.Parser.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dtc.Common.Extensions;
using RadioOwl.Parsers.Parser.Helpers;

namespace RadioOwl.Parsers.Parser.Base
{
    public abstract class MujRozhlasParserBase : PageParserBase2
    {
       // private StaticHttpClient staticHttpClient = new StaticHttpClient();


        protected MujRozhlasParserBase(Action<string> logAction) : base(logAction)
        {
        }








        /// <summary>
        /// Potrebuju zjistit RID identifaktor poradu 
        /// - bohuzel nejde vykousnout z nejakeho jsonu atd, musim dohledat v parametru ajax volani z cele html stranky
        /// 
        /// napr:
        /// <input type="checkbox" class="checkbox__control" id="" name="" checked="checked" data-ajax="/ajax/ajax_list_redraw/serial?size=9&amp;id=serial-1239859&amp;rid=1239859">
        /// <a href="https://www.mujrozhlas.cz/ajax/ajax_list/serial?page=1&amp;size=9&amp;id=serial-1239859&amp;rid=1239859" class="more-link__link ajax">
        /// </summary>
        /// <param name="html"></param>
        /// <returns></returns>
        public string GetRID(string html)
        {
            if (string.IsNullOrEmpty(html))
                return null;
            // někde je problém s crlf a pod
            html = html.Replace("\n", "").Replace("\r", "");
            var htmlParts = html.Split(new string[] { "rid=" }, StringSplitOptions.RemoveEmptyEntries);
            if (htmlParts.Length > 1)
            {
                // prvni token je zacatek html, ten mne nezajima
                for (int i = 1; i < htmlParts.Length; i++)
                {
                    var rid = ReadNumber(htmlParts[i]);
                    if (!string.IsNullOrEmpty(rid))
                        return rid;
                }
            }
            return null;
        }





        /// <summary>
        /// Vrací začátek stringu, který obsahuje 'IsDigit' znaky
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        private string ReadNumber(string s)
        {
            if (string.IsNullOrEmpty(s))
                return null;
            var result = string.Empty;
            for (var i = 0; i < s.Length; i++)
            {
                if (!char.IsDigit(s[i]))
                    break;
                result += s[i];
            }
            return result;
        }








        /// <summary>
        /// Dohleda detaily o poradu + zde asi jedine misto, kde zjistim pocet epizod?
        /// dohledat <script> pod <div> kde je to jako kus JS zdrojaku s definovanou JSON promennou, kterou z toho zkusim vykousnou
        /// </summary>
        public MujRozhlas2020SiteInfo GetContentSerialAllParts(HtmlDocument htmlDoc)
        {
            var divPageMain = htmlDoc.DocumentNode.SelectNodes(@".//div[@class='page__main']//script");
            if (divPageMain == null)
                throw new Exception("Chyba při parsování html - nepodařilo se dohledat seznam 'div[@class='page__main']'.");
            if (!divPageMain.Any())
                throw new Exception("Chyba při parsování html - podařilo se dohledat seznam 'article[@class='b-episode']' - seznam je však prázdný.");

            var pageMainScript = divPageMain.FirstOrDefault().InnerHtml?.SubstrFromToChar('{', '}');

            var mainJson = System.Net.WebUtility.HtmlDecode(pageMainScript);
            var jMainJson = JObject.Parse(mainJson);

            var mujRozhlas2020SiteInfo = new MujRozhlas2020SiteInfo
            {
                ContentSerialAllParts = jMainJson.SelectToken("contentSerialAllParts")?.Value<int>(),
                SiteEntityBundle = jMainJson.SelectToken("siteEntityBundle")?.Value<string>(),
                SiteEntityLabel = jMainJson.SelectToken("siteEntityLabel")?.Value<string>(),
                SiteDocumentPath = jMainJson.SelectToken("siteDocumentPath")?.Value<string>(),
                ContentId = jMainJson.SelectToken("contentId")?.Value<string>()
            };

            return mujRozhlas2020SiteInfo;
        }









        protected void ParseEpisode( MujRozhlas2020SiteInfo mujRozhlas2020SiteInfo)
        {
            if (mujRozhlas2020SiteInfo is null) throw new ArgumentNullException(nameof(mujRozhlas2020SiteInfo));

            if (mujRozhlas2020SiteInfo.SiteEntityBundle != "episode")
                throw new NotSupportedException("Pouze pro typ pořadu bundle=episode!");

            var episodeJsonUrl = $@"https://api.mujrozhlas.cz/episodes/{mujRozhlas2020SiteInfo.ContentId}";


            var httpDownload = StaticHttpClient.HttpDownload(episodeJsonUrl);
            //            var episodeJsonCode = HttpDownload( await DownloadHtmlAsync(episodeJsonUrl);

            // viz https://stackoverflow.com/questions/9303257/how-to-decode-a-unicode-character-in-a-string
            // var urlPartInfoUnescaped = System.Text.RegularExpressions.Regex.Unescape(urlPartInfo);

            var episodeJson = JObject.Parse(httpDownload.content); // e pisodeJsonCode);

            if (episodeJson != null)
            {
                //  var episodeMp3 = episodeJson.SelectToken("$.data.audioLinks[0].url")?.Value<string>();

                var partNo = 0; // jen jedn epizoda  
                // 
                var partTitle = episodeJson.SelectToken("$.data.attributes.title")?.Value<string>();
                //
                var partShortTitle = episodeJson.SelectToken("$.data.attributes.shortTitle")?.Value<string>();
                // 
                var partDescriptionHtml = episodeJson.SelectToken("$.data.attributes.description")?.Value<string>();
                var partDescription = new HtmlHelper().StripHtmlTags(partDescriptionHtml);
                // samotne url k mp3
                var partAudioLink = episodeJson.SelectToken("$.data.attributes.audioLinks[0].url")?.Value<string>();

                var radioData = new RadioData();
                new RadioDataPartFactory().Create(radioData, partNo, partTitle, partDescription, partAudioLink);
            }
        }





        protected void ParseSerial(MujRozhlasData mujRozhlasData) //, MujRozhlas2020SiteInfo mujRozhlas2020SiteInfo)
        {
            if (!mujRozhlasData.MujRozhlas2020SiteInfo.ContentSerialAllParts.HasValue)
                throw new Exception($"Nepodařilo se dohledat ContentSerialAllParts, pro SiteEntityBundle:'{mujRozhlasData.MujRozhlas2020SiteInfo.SiteEntityBundle}'.");

            var allPartsHtml = GetShowPartsAll(mujRozhlasData.RId);
            if (allPartsHtml == null)
                throw new Exception($"Nepodařilo se dohledat přehratelné díly pořadu. RID={mujRozhlasData.RId},SiteEntityBundle:'{mujRozhlasData.MujRozhlas2020SiteInfo.SiteEntityBundle}'.");
            if (!allPartsHtml.Any())
                throw new Exception($"Neexistují díly pořadu. RID={mujRozhlasData.RId},SiteEntityBundle:'{mujRozhlasData.MujRozhlas2020SiteInfo.SiteEntityBundle}'.");

            // ID jednotlivých částí
            var partUuidSet = GetPartIdAll(allPartsHtml);

            // z ID jednotlivých částí už mohu přes API stáhnout JSON s finalními daty pro konkrétní část
            foreach (var partUuid in partUuidSet)
            {
                var partData = GetAudioLink(mujRozhlasData, partUuid);
                mujRozhlasData.PartSet.Add(partData);
            }
        }



        /// <summary>
        /// Stahne vsechny (prvnich 99) dilu poradu
        /// </summary>
        private HtmlNodeCollection GetShowPartsAll(string rid)
        {
            return GetShowParts(rid, 0, 99);
        }



        /// <summary>
        /// Momentalne to vypada, ze vsechny dily poradu pujdou stahnout pres 1 dotaz, tak jako to dela web stranka pri kliknuti
        /// na "Dalsi dily" - tam se ajax dotaz vola s parametry page=1 size=9 (na strance je zobrazeno prvnich 9 poradu), timhle
        /// se donacte zbytek.
        /// Pokusne zjistuju, ze pri zaslani page=0, size=99 api neprotestuje a zasle vsechny dily najednou - bohuzel jako html stranku.
        /// </summary>
        internal HtmlNodeCollection GetShowParts(string rid, int page, int size)
        {
            var ajaxListUrl = $@"https://www.mujrozhlas.cz/ajax/ajax_list/serial?page={page}&size={size}&id=serial-{rid}&rid={rid}";
            StaticHttpClient.HttpDownload(ajaxListUrl, out string ajaxList);

            var ajaxListDecoded = System.Net.WebUtility.HtmlDecode(ajaxList);
            var ajaxListJson = JObject.Parse(ajaxListDecoded);
            var contentTag = ajaxListJson.SelectToken($"$.snippets.serial-{rid}.content")?.Value<string>();
            var contentTagDecodede = System.Net.WebUtility.HtmlDecode(contentTag);

            // je možné, že pořad už není publikovaný a nemá žádné veřejné díly
            if (string.IsNullOrEmpty(contentTagDecodede?.Trim()))
                return null;

            // selector pro vyber tagu jednotlivych dilu
            var articleTagSelector = @".//article[@class='b-episode']";

            var html = new HtmlDocument();
            html.LoadHtml(contentTagDecodede);
            var htmlArticleSet = html.DocumentNode.SelectNodes(articleTagSelector);
            return htmlArticleSet;
        }



        /// <summary>
        /// Z html casti pro dil poradu se pokusi vykousnout ID jednotlive casti
        /// </summary>
        internal List<string> GetPartIdAll(HtmlNodeCollection htmlCollection)
        {
            return htmlCollection.Select(p => GetPartId(p)).ToList();
        }

        /// <summary>
        /// Z html casti pro dil poradu se pokusi vykousnout ID jednotlive casti
        /// </summary>
        internal string GetPartId(HtmlNode articleTag)
        {
            var dataEntryAttributeHtml = articleTag.GetAttributeValue("data-entry", string.Empty);
            if (string.IsNullOrWhiteSpace(dataEntryAttributeHtml))
                throw new Exception($"Chyba při parsování html - 'data-entry' is empty.");

            var dataEntryAttributeValue = System.Net.WebUtility.HtmlDecode(dataEntryAttributeHtml);

            var jsonDataEntry = JObject.Parse(dataEntryAttributeValue);

            // hlavni ID dílu seriálu
            var partUuid = jsonDataEntry.SelectToken("$.uuid")?.Value<string>();
            return partUuid;
        }






        private MujRozhlasPartData GetAudioLink(MujRozhlasData mujRozhlasData, string partUuid)
        {
            var episodeJson = ApiMujRozhlasGetEpisodesJson(partUuid);

            // guid
            var partDataId = episodeJson.SelectToken("$.data.id")?.Value<string>();
            // melo by byt episode?
            var partDataType = episodeJson.SelectToken("$.data.type")?.Value<string>();
            // 
            var partNo = episodeJson.SelectToken("$.data.attributes.part")?.Value<int>();
            // 
            var partTitle = episodeJson.SelectToken("$.data.attributes.title")?.Value<string>();
            //
            var partShortTitle = episodeJson.SelectToken("$.data.attributes.shortTitle")?.Value<string>();
            // 
            var partDescriptionHtml = episodeJson.SelectToken("$.data.attributes.description")?.Value<string>();
            var partDescription = new HtmlHelper().StripHtmlTags(partDescriptionHtml);

            // samotne url k mp3
            var partAudioLink = episodeJson.SelectToken("$.data.attributes.audioLinks[0].url")?.Value<string>();

            var data = new MujRozhlasPartData()
            {
                MujRozhlasData = mujRozhlasData,
                Uuid = partUuid,
                Json = episodeJson,
                DataId = partDataId,
                DataType = partDataType,
                No = partNo,
                Title = partTitle,
                ShortTitle = partShortTitle,
                DescriptionHtml = partDescriptionHtml,
                Description = partDescription,
                AudioLink = partAudioLink
            };

            return data;

            //var radioData = new RadioData();
            //new RadioDataPartFactory().Create(radioData, partNo, partTitle, partDescription, partAudioLink);
        }


        /// <summary>
        /// Stáhne pře API json data ke konkrétnímu pořadu
        /// </summary>
        private JObject ApiMujRozhlasGetEpisodesJson(string partUuid)
        {
            //priklad: https://api.mujrozhlas.cz/episodes/6d34cb00-fb37-3aaf-8037-6c2c5ba0deb0

            var urlPart = $@"https://api.mujrozhlas.cz/episodes/{partUuid}";
            StaticHttpClient.HttpDownload(urlPart, out string urlPartInfo);

            // unescapovat unicode znaky typu "\u003Cp\u003E\u010cetbu na pokra\u010dov\u00e1n\u00ed ze..."
            // viz https://stackoverflow.com/questions/9303257/how-to-decode-a-unicode-character-in-a-string
            var urlPartInfoUnescaped = System.Text.RegularExpressions.Regex.Unescape(urlPartInfo);

            // faq dotazy na json: https://www.newtonsoft.com/json/help/html/QueryJsonSelectTokenJsonPath.htm
            var partJson = JObject.Parse(urlPartInfoUnescaped);
            return partJson;
        }

    }
}
