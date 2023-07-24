using RadioOwl.Parsers.Data;
using RadioOwl.Parsers.Parser.Data;
using System;
using System.Threading.Tasks;

namespace RadioOwl.Parsers.Parser.Interfaces
{
    /// <summary>
    /// Interface pro parsery 
    /// </summary>
    public interface IPageParser2
    {
        /// <summary>
        /// Verze parseru
        /// </summary>
        int Version { get; }

        /// <summary>
        /// Url ke zpracování
        /// </summary>
        string[] ParseUrls { get; }

        /// <summary>
        /// Umim parsovat zaslané url?
        /// </summary>
        bool CanParse(string url);

        /// <summary>
        /// Parsování dat z url odkazu
        /// </summary>
        ParserResult TryParse(string url, Action<string> log);
    }
}
