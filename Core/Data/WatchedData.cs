using System;
using System.Collections.Generic;

namespace FargowiltasCrossmod.Core.Data
{
    public class WatchedData(Dictionary<string, object> data, Type intendedType)
    {
        /// <summary>
        /// The data to watch.
        /// </summary>
        public Dictionary<string, object> Data = data;

        /// <summary>
        /// The intended type of the data.
        /// </summary>
        public readonly Type IntendedType = intendedType;
    }
}
