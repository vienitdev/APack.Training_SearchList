using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;

namespace Archpack.Training.ArchUnits.Routing.V1
{
    public class AutoDetectTypePath: ITypePath
    {
        private readonly static Object syncRoot = new Object();
        private IEnumerable<TypePathItem> items;

        public AutoDetectTypePath(string rootNamespace, params Assembly[] targetAssemblies)
        {
            this.Initialize(rootNamespace, targetAssemblies);
        }

        public void Initialize(string rootNamespace, params Assembly[] targetAssemblies)
        {
            var targetNamespacePre = string.Format("{0}.{1}", rootNamespace, "ServiceUnits");
            lock (syncRoot)
            {
                items = targetAssemblies.SelectMany(a => a.GetTypes())
                    .Where(t => IsTargetType(t, targetNamespacePre))
                    .Select(t => new TypePathItem(t.FullName.Substring(targetNamespacePre.Length).Replace(".", "/"), t)).ToList();
            }
        }
        
        public Type GetType(string path)
        {
            if (items == null)
            {
                return null;
            }
            var item = items.FirstOrDefault(i => i.Path.Equals(path, StringComparison.InvariantCultureIgnoreCase));
            if (item != null)
            {
                return item.TargetType;
            }
            return null;
        }

        public string GetPath(Type type)
        {
            if (items == null)
            {
                return null;
            }
            var item = items.FirstOrDefault(i => i.TargetType == type);
            if (item != null)
            {
                return item.Path;
            }
            return null;
        }

        public IEnumerable<TypePathItem> Items
        {
            get { return items == null ? Enumerable.Empty<TypePathItem>() : items; }
        }

        private bool IsTargetType(Type target, string targetNamespacePre)
        {
            if (target.Namespace == null)
            {
                return false;
            }
            if (target.IsGenericType)
            {
                return false;
            }
            if (target.IsNested)
            {
                return false;
            }
            return target.Namespace.StartsWith(targetNamespacePre);
        }
    }
}