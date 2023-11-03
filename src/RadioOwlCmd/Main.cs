using RadioOwl.Parsers;
using RadioOwl.Parsers.Data;
using RadioOwl.Parsers.Data.Factory;
using RadioOwl.Parsers.Parser;
using RadioOwl.Parsers.Parser.Data;
using RadioOwl.Parsers.Parser.Interfaces;
using RadioOwlCmd.Helpers;
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




        public void Run(string[] args)
        {
            // var url = Console.ReadLine();

            // https://www.mujrozhlas.cz/cetba-na-pokracovani/vrazda-pro-zlateho-muze-kapitan-exner-vysetruje-zlocin-z-vasne
            //var url = @"https://www.mujrozhlas.cz/cetba-na-pokracovani/vrazda-pro-zlateho-muze-kapitan-exner-vysetruje-zlocin-z-vasne";

            // uz nema stahnutelne dily
            //var url = @"https://www.mujrozhlas.cz/cetba-na-pokracovani/karin-lednicka-sikmy-kostel-i-kdyz-je-zivot-tvrdy-jako-kamen-laska-je";


            var url = @"https://www.mujrozhlas.cz/cetba-s-hvezdickou/anthony-burgess-mechanicky-pomeranc-parta-frendiku-pacha-brutalni-nasili-v";
            url = @"https://www.mujrozhlas.cz/cetba-na-pokracovani/josef-jedlicka-kde-zivot-nas-je-v-puli-se-svou-pouti-o-hledani-smyslu";
            url = @"https://www.mujrozhlas.cz/cetba-s-hvezdickou/elsa-aids-pripravy-na-vsechno-truchlivy-pribeh-z-ceskeho-zivota-ve-vyprodeji";
            url = @"https://www.mujrozhlas.cz/cetba-na-pokracovani/howard-phillips-lovecraft-v-horach-silenstvi-vyprava-objevi-stopy-desive";
            url = @"https://www.mujrozhlas.cz/milan-kundera-nesmrtelnost/milan-kundera-nesmrtelnost-posledni-cesky-psany-roman-uvadime-v";
            //url = @"https://www.mujrozhlas.cz/hra-na-nedeli/kovboj-jiri-vyoralek-v-letni-komedii-o-muzi-ktereho-zena-vyhodila-z-domu"; // episode
            //url = @"https://www.mujrozhlas.cz/vecerni-drama"; // show
            url = @"https://www.mujrozhlas.cz/cetba-na-pokracovani/howard-phillips-lovecraft-v-horach-silenstvi-vyprava-objevi-stopy-desive";
            url = @"https://www.mujrozhlas.cz/cetba-s-hvezdickou/pribeh-kriminalniho-rady-vrazdy-deti-i-tezkosti-dospivani-v-detektivnim-hororu";
            //url = @"https://www.mujrozhlas.cz/cetba-s-hvezdickou/zemrel-spisovatel-vaclav-kahuda-spodni-proudy-jeho-zivota-si-pripomenme-v-cetbe";
            //     url = @"https://www.mujrozhlas.cz/cetba-na-pokracovani/josef-jedlicka-kde-zivot-nas-je-v-puli-se-svou-pouti-o-hledani-smyslu";
            url = @"https://www.mujrozhlas.cz/povidka/svet-bohatstvi-zahalky-jazzovych-vecirku-poslechnete-si-povidky-francise-scotta-fitzgeralda";
            url = @"https://www.mujrozhlas.cz/radiokniha/jiri-fried-leto-v-altamire-roman-o-klukovskem-pratelstvi-v-dobe-tesne-pred-valkou";
            url = @"https://www.mujrozhlas.cz/podvecerni-cteni/milostny-dopis-klinovym-pismem-strhujici-pribeh-jedne-rodiny-na-pozadi-udalosti";
            url = @"https://www.mujrozhlas.cz/cetba-na-pokracovani/grandhotel-premierova-cetba-z-romanu-jaroslava-rudise-v-podani-petra-stacha"; ;
            //url = @"https://www.mujrozhlas.cz/cetba-na-pokracovani/thomas-harris-mlceni-jehnatek-svetoznamy-thriller-o-kanibalskem-zlocinci";
            //  url = @"https://www.mujrozhlas.cz/radiokniha/erazim-kohak-zelena-svatozar-kapitoly-z-ekologicke-etiky";
            //   url = @"https://www.mujrozhlas.cz/poctenicko/petr-sagitarius-trujkunt-kruhy-drsna-detektivka-z-jablunkovskeho-pohranici";

            url = @"https://www.mujrozhlas.cz/cetba-s-hvezdickou/lucie-faulerova-smrtholka-tema-rodinne-tragedie-zabalene-do-jemneho-humoru"; ;


            url = @"https://www.mujrozhlas.cz/poctenicko/petr-sagitarius-trujkunt-kruhy-drsna-detektivka-z-jablunkovskeho-pohranici";
            url = @"https://www.mujrozhlas.cz/podvecerni-cteni/milos-urban-boletus-arcanus-thriller-o-tajemnem-hribu-ktery-splni-kazde-prani";

            if (!string.IsNullOrEmpty(url)) ProcessUrl(url);
        }


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


            // TODO co kdyz se parser nevybere?


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
            // zakladni TMP cesta
            var tmpRoot = Path.Combine(Path.GetTempPath(), "RADIOOWL_TMP");
            Directory.CreateDirectory(tmpRoot);
            // root tmp cesta pro porad/content
            var tmpContentRoot = Path.Combine(tmpRoot, $"CONTENT_{parserResult.MujRozhlasData.MujRozhlas2020SiteInfo.ContentId}");
            Directory.CreateDirectory(tmpContentRoot);

            foreach (var part in parserResult.MujRozhlasData.PartSet)
            {
                DonwloadMp3Part(tmpContentRoot, parserResult.MujRozhlasData.MujRozhlas2020SiteInfo.ContentId, part);

                Console.WriteLine($"Cmd result: {part.CmdProcessExitCode}, mp4a:{File.Exists(part.TmpMp4aFileName)}, mp3:{File.Exists(part.TmpMp3Filename)}");

                if(part.CmdProcessExitCode == 0)
                {
                    SetId3Tags(part);


                    var finalMp3Filename = new FileHelper().GeneratePartFilename(part.ShortTitle, part.No , parserResult.MujRozhlasData.MujRozhlas2020SiteInfo.ContentSerialAllParts , "mp3");

                    part.FinalMp3Filename = Path.Combine(tmpContentRoot, finalMp3Filename);
                    File.Copy(part.TmpMp3Filename, part.FinalMp3Filename, overwrite:true );
                }
            }


        }

        private void SetId3Tags(MujRozhlasPartData part)
        {
            var tagliFile = TagLib.File.Create(part.TmpMp3Filename);
            tagliFile.Tag.Track = (part.No ?? 0) < 0 ? 0 : (uint)(part.No.Value);
            tagliFile.Tag.Title = part.ShortTitle;
            tagliFile.Tag.Description = part.Description;
            // ?? tagliFile.Tag.Performers = new string[] {part.}
            tagliFile.Save();
        }

        private void DonwloadMp3Part(string tmpContentRoot, string contentId, MujRozhlasPartData part)
        {
            var exePath = Path.GetDirectoryName(Process.GetCurrentProcess()?.MainModule?.FileName);
            if (string.IsNullOrEmpty(exePath)) throw new ArgumentNullException(nameof(exePath));

            // cesta pod exe, kde jsou vyexportovane BAT a EXE pro stazeni a prekonverovani mp4 a mp3
            var batPath = Path.Combine(exePath, "ConverterFiles");
          
            // pod root pro konkretne resenou cast poradu
            var tmpPartRoot = Path.Combine(tmpContentRoot, $"PART_{part.No:0000}_{part.Uuid}");
            Directory.CreateDirectory(tmpPartRoot);

            part.TmpMp4aFileName = $"{tmpPartRoot}\\{part.Uuid}.mp4a"; // cesta kam bude stazeno mp4
            part.TmpMp3Filename = $"{tmpPartRoot}\\{part.Uuid}.mp3";   // cesta kam bude prekonvertovano mp3
            var batCommand = $"{batPath}\\converter2.bat";  // cesta ke konvertujicimu BATu

            var arguments = new List<string>()              // parametry BATu
            {
                part.AudioLink,
                part.TmpMp4aFileName,
                part.TmpMp3Filename 
            };


            //var process = Process.Start(batCommand, arguments);

            // pokud chci poustet BATy v samostatnem okne
            var processStartInfo = new ProcessStartInfo()
            {
                CreateNoWindow = false,
                UseShellExecute = true,
                FileName = batCommand,
                Arguments = arguments.Aggregate((a, b) => a + " " + b),
            };
            var process = Process.Start(processStartInfo); 
            process.WaitForExit();
            part.CmdProcessExitCode = process.ExitCode;

            Console.WriteLine($"*** Cmd exit code:{part.CmdProcessExitCode}");

        }
    }
}
