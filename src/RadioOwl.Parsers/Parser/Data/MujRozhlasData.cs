using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RadioOwl.Parsers.Parser.Data
{
    public class MujRozhlasData
    {
        public readonly List<MujRozhlasPartData> PartSet = new List<MujRozhlasPartData>();

        public string RId { get; internal set; }
        public MujRozhlas2020SiteInfo MujRozhlas2020SiteInfo { get; internal set; }
    }
}
