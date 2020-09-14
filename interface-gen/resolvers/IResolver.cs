using Microsoft.Azure.DigitalTwins.Parser;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace IoTModels.Resolvers
{
    /// <summary>
    /// IResolver interface for resolver strategy
    /// </summary>
    public interface IResolver
    {
        /// <summary>
        /// DMTI resolver can be used as delegate in dt parser
        /// </summary>
        /// <param name="dtmis"></param>
        /// <returns></returns>
        Task<IEnumerable<string>> DtmiResolver(IReadOnlyCollection<Dtmi> dtmis);
    }
}