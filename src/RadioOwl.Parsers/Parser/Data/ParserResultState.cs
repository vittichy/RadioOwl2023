using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RadioOwl.Parsers.Parser.Data
{
    public enum ParserResultState
    {
        [Description("Success")]
        Success = 1,






        [Description("Download error")]
        DownloadError = 1100,

        [Description("RID number not found")]
        RidNotFound = 1101,

        [Description("SiteEntityBundleNotFound not found")]
        SiteEntityBundleNotFound = 1102,



        [Description("Parser error")]
        ParserError = 100,
        [Description("Unexpected error")]
        UnexpectedError = 101,
        
    }
}
