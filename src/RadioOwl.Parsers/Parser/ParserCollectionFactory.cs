using RadioOwl.Parsers.Parser.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RadioOwl.Parsers.Parser
{
    public class ParserCollectionFactory
    {

        /// <summary>
        /// Seznam dostupných parserů
        /// </summary>
        public List<IPageParser2> Build(Action<string> logAction)
        {
            if (logAction is null) throw new ArgumentNullException(nameof(logAction));

            return new List<IPageParser2>()
                                {
                                    new MujRozhlas2023Parser(logAction),
                                };
        }
            

    }
}
