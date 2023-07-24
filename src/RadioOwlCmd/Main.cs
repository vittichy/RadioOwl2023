using RadioOwl.Parsers;
using RadioOwl.Parsers.Data;
using RadioOwl.Parsers.Data.Factory;
using RadioOwl.Parsers.Parser;
using RadioOwl.Parsers.Parser.Data;
using RadioOwl.Parsers.Parser.Interfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace RadioOwlCmd
{
    internal class Main
    {
        /// <summary>
        /// Seznam dostupných parserů
        /// </summary>
        private readonly ParserCollection _parsers = new ParserCollection();


        //public Main()
        //{
            
        //}



        public void Run(string[] args)
        {
            // var url = Console.ReadLine();

            // https://www.mujrozhlas.cz/cetba-na-pokracovani/vrazda-pro-zlateho-muze-kapitan-exner-vysetruje-zlocin-z-vasne
            //var url = @"https://www.mujrozhlas.cz/cetba-na-pokracovani/vrazda-pro-zlateho-muze-kapitan-exner-vysetruje-zlocin-z-vasne";

            // uz nema stahnutelne dily
            //var url = @"https://www.mujrozhlas.cz/cetba-na-pokracovani/karin-lednicka-sikmy-kostel-i-kdyz-je-zivot-tvrdy-jako-kamen-laska-je";


            var url = @"https://www.mujrozhlas.cz/cetba-s-hvezdickou/anthony-burgess-mechanicky-pomeranc-parta-frendiku-pacha-brutalni-nasili-v";

            if (!string.IsNullOrEmpty(url)) ProcessUrl(url);
        }


        //   /// <summary>
        ///// Zahaji zpracovani url
        ///// </summary>
        //private void ProcessUrl(string url)
        //{
        //    if (string.IsNullOrWhiteSpace(url)) return;

        //    // založení nového stahování
        // // TODO?   var radioData = new RadioDataFactory().Create(url);
        //    //RadioDataSet.Insert(0, radioData);
        //    //RadioDataSelected = radioData;
        //    //DataGridMainGotFocus();

        //    ProcessUrl(url);
        //}




        /// <summary>
        /// 
        /// </summary>
        private void ProcessUrl(string url)
        {
            var parserSet = new ParserCollectionFactory().Build(Log);


            // dohledání vhodného parseru stránky
            //var parserSet = _parsers.FindParser2(url);

            if (!parserSet.Any())
            {
                //radioData.AddLogError($"Nepodařilo se dohledat parser pro url: {radioData.Url}.");
                return;
            }

            // zkusím použít parser a rozpasovat
            foreach (var parser in parserSet)
            {
                if (parser.CanParse(url))
                {
                    var parserResult = parser.TryParse(url, Log);
                    //// beru prvni parser kteremu se povedlo parsovani
                    //if (parseOk)
                    //{
                    //    return true;
                    //}


                    if(parserResult.ParserResultState == ParserResultState.Success)
                    {
                        DonwloadMp3(parserResult);
                    }

                }
            }
            //return false;

            //var parseOk = TryParser(radioData, parserSet).Result;
            //if (parseOk)
            //{
            //    radioData.AddLog("Parser ok.");
            //    await DownloadRadioDataAsync(radioData);
            //}
            //else
            //{
            //    radioData.AddLogError("Parser error.");
            //}
        }


        private void Log(string message)
        {
            Console.WriteLine($"{DateTime.Now:s} {message}" );
        }

        //private async Task<bool> TryParser(RadioData radioData, List<IPageParser2> parserSet)
        //{
        //    foreach (var parser in parserSet)
        //    {
        //        var parseOk = await parser.ParseAsync(radioData);
        //        // beru prvni parser kteremu se povedlo parsovani
        //        if (parseOk)
        //        {
        //            return true;
        //        }
        //    }
        //    return false;
        //}







        /// <summary>
        /// Zpracovani rozparsovanych dat - tj zde bych jiz mel znat odkazy na finalni mp3 a ty stahnu, ulozim, porezim filename, mp3id tagy atd
        /// </summary>
        private async Task DownloadRadioDataAsync(RadioData radioData)
        {
            //radioData.PartSet
            //                .Where(p => p.UrlMp3Exists)
            //                    .ToList()
            //                        .ForEach(async p => await ProcessDataPartAsync(p));

            // ted tedy synchronne po jednom? v cmd
            foreach(var part in radioData.PartSet)
            {
                await ProcessDataPartAsync(part);
            }



            //TODO       await SaveImageAsync(radioData);
            //TODO       SaveReadMe(radioData);
        }


        /// <summary>
        /// Zpracování jedné části pořadu - zde už musím mít rozparsovanou url
        /// </summary>
        private async Task ProcessDataPartAsync(RadioDataPart radioDataPart)
        {
            //radioDataPart.State = RadioDataPartState.Started;
            //radioDataPart.FileName = new FileHelper().GenerateMp3FileName(radioDataPart);

            //// soubor jeste neexistuje?
            //if (!File.Exists(radioDataPart.FileName))
            //{
            //    radioDataPart.State = RadioDataPartState.Started;
            //    await DownloadPartAsync(radioDataPart);
            //}
            //else
            //{
            //    radioDataPart.State = RadioDataPartState.FileAlreadyExists;
            //    radioDataPart.AddLogWarning($"Soubor již existuje: {radioDataPart.FileName}.");
            //}
        }












        private void DonwloadMp3(ParserResult parserResult)
        {
            foreach(var part in parserResult.MujRozhlasData.PartSet)
            {
                DonwloadMp3Part(parserResult.MujRozhlasData.MujRozhlas2020SiteInfo.ContentId, part);

                Console.WriteLine($"Cmd result: {part.CmdProcessExitCode}, mp4a:{File.Exists(part.Mp4a)}, mp3:{File.Exists(part.Mp3)}");


            }


        }

        private void DonwloadMp3Part(string contentId, MujRozhlasPartData part)
        {
            var exePath = Path.GetDirectoryName(Process.GetCurrentProcess()?.MainModule?.FileName);
            if (string.IsNullOrEmpty(exePath)) throw new ArgumentNullException(nameof(exePath));

            // cesta pod exe, kde jsou vyexportovane BAT a EXE pro stazeni a prekonverovani mp4 a mp3
            var batPath = Path.Combine(exePath, "ConverterFiles");

            // zakladni TMP cesta
            var tmpRoot = Path.Combine(Path.GetTempPath(), "RADIOOWL_TMP");
            Directory.CreateDirectory(tmpRoot);

            // root tmp cesta pro porad/content
            var tmpContentRoot = Path.Combine(tmpRoot, $"CONTENT_{contentId}");
            Directory.CreateDirectory(tmpContentRoot);         
            // pod root pro konkretne resenou cast poradu
            var tmpPartRoot = Path.Combine(tmpContentRoot, $"PART_{part.No:0000}_{part.Uuid}");
            Directory.CreateDirectory(tmpPartRoot);

            part.Mp4a = $"{tmpPartRoot}\\{part.Uuid}.mp4a"; // cesta kam bude stazeno mp4
            part.Mp3 = $"{tmpPartRoot}\\{part.Uuid}.mp3";   // cesta kam bude prekonvertovano mp3
            var batCommand = $"{batPath}\\converter2.bat";  // cesta ke konvertujicimu BATu

            var arguments = new List<string>()              // parametry BATu
            {
                part.AudioLink,
                part.Mp4a,
                part.Mp3 
            };

            var process = Process.Start(batCommand, arguments);
            process.WaitForExit();
            part.CmdProcessExitCode = process.ExitCode;

            Console.WriteLine($"*** Cmd exit code:{part.CmdProcessExitCode}");
        }
    }
}
