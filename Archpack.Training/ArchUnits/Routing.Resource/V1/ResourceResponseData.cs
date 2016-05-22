using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;

namespace Archpack.Training.ArchUnits.Routing.Resource.V1
{
    public sealed class ResourceResponseData
    {
        public ResourceResponseData(IDictionary<string, string> resources, CultureInfo culture)
        {
            this.Resources = resources;
            this.Culture = culture;
        }

        public IDictionary<string, string> Resources { get; private set; }

        public CultureInfo Culture { get; private set; }
    }
}