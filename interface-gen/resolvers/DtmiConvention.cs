using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IoTModels.Resolvers
{
    /// <summary>
    /// Dtmi convention class
    /// </summary>
    public static class DtmiConvention
    {
        /// <summary>
        /// Map dtmi to a path based on convention
        /// </summary>
        /// <param name="dtmi"></param>
        /// <returns></returns>
        public static string Dtmi2Path(string dtmi)
        {
            var idAndVersion = dtmi.ToLowerInvariant().Split(';');
            var segments = idAndVersion[0].Split(':');
            var fileName = $"{segments.ElementAt(segments.Length - 1)}-{idAndVersion[1]}.json";
            Array.Resize(ref segments, segments.Length - 1);
            return string.Join("/", segments) + "/" + fileName;
        }
    }
}
