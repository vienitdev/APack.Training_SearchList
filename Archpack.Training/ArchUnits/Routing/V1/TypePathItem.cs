using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Archpack.Training.ArchUnits.Routing.V1
{
    public class TypePathItem
    {
        public TypePathItem(string path, Type targetType)
        {
            this.Path = path;
            this.TargetType = targetType;
        }

        public string Path { get; private set; }

        public Type TargetType { get; private set; }
    }
}