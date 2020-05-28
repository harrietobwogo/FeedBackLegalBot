using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FeedBackLegalBot.Models.Interfaces
{
    /// <summary>
    /// Provides a contract for reading from and writing to file sources.
    /// </summary>
    public interface IFileUtility
    {
        Task<string> ReadFromFile(string filePathSource);

        Task WriteToFile(string fileContents, string filePathSource);
    }
}
