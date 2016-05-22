using Archpack.Training.ArchUnits.Routing.V1;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Resources;
using System.Web;

namespace Archpack.Training.ArchUnits.Routing.Resource.V1
{
    public class ResourceProcessResolver : IUriProcessResolver
    {

        public ResourceProcessResolver()
        {
            BuildHandleErrorPipeLine();
        }

        private void BuildHandleErrorPipeLine()
        {
            HandleErrorPipeline = new List<Func<ServiceUnitContext, ServiceUnitResponse, ServiceUnitResponse>>();
        }

        public List<Func<ServiceUnitContext, ServiceUnitResponse, ServiceUnitResponse>> HandleErrorPipeline { get; private set; }

        private static readonly string CultureParameterName = "c";

        public Type GetExecutionType(ServiceUnitContext suContext)
        {
            return typeof(ResourceHolder);
        }

        public object CreateInstance(ServiceUnitContext suContext, Type instanceType)
        {
            var classAbstractPath = string.Format("/{0}/{1}/Resources/{2}",
                suContext.ServiceUnitName, suContext.Version, suContext.Request.ProcessPath.Split(new[] { '/' })[1]);
            var typePath = suContext.ServiceContainer.GetService<ITypePath>();
            var type = typePath.GetType(classAbstractPath);
            var resourceManager = new ResourceManager(type.FullName, type.Assembly);
            return new ResourceHolder(resourceManager);
        }

        public ActionDefinition GetAction(ServiceUnitContext suContext, object instance)
        {
            return new ActionDefinition() { Instance = instance };
        }

        public ServiceUnitResponse InvokeAction(ServiceUnitContext suContext, ActionDefinition action)
        {
            var holder = action.Instance as ResourceHolder;
            var resourceSetWizCul = RetrieveResourceSetWithCulture(suContext, holder);
            if (resourceSetWizCul == null)
            {
                return new ServiceUnitResponse(HttpStatusCode.NotFound);
            }
            var result = AsEnumerable(resourceSetWizCul.Item1)
                .Where(e => e.Key is string && e.Value is string)
                .Aggregate(new Dictionary<string, string>(), (dic, de) =>
                {
                    dic.Add(de.Key.ToString(), de.Value.ToString());
                    return dic;
                });
            return new ServiceUnitResponse(HttpStatusCode.OK)
            {
                Data = new ResourceResponseData(result, resourceSetWizCul.Item2)
            };
        }

        private IEnumerable<DictionaryEntry> AsEnumerable(ResourceSet rs)
        {
            var dic = rs.GetEnumerator();
            while (dic.MoveNext())
            {
                yield return dic.Entry;
            }
            yield break;
        }

        private Tuple<ResourceSet, CultureInfo> RetrieveResourceSetWithCulture(ServiceUnitContext suContext, ResourceHolder holder)
        {
            var resourceManager = holder.Manager;
            var culture = CultureInfo.InvariantCulture;
            CultureInfo targetCulture = CultureInfo.InvariantCulture;
            ResourceSet resourceSet = null;
            if (suContext.Request.Query.ContainsKey(CultureParameterName))
            {
                var paramCultures = suContext.Request.Query[CultureParameterName].Split(',');
                foreach (var paramCulture in paramCultures)
                {
                    culture = targetCulture = new CultureInfo(paramCulture);
                    while (true)
                    {
                        if (culture.Name == CultureInfo.InvariantCulture.Name)
                        {
                            break;
                        }
                        resourceSet = resourceManager.GetResourceSet(culture, true, false);
                        if (resourceSet != null)
                        {
                            break;
                        }
                        culture = culture.Parent;
                    }
                    if (resourceSet != null)
                    {
                        break;
                    }
                }
            }
            if (resourceSet == null)
            {
                resourceSet = resourceManager.GetResourceSet(CultureInfo.InvariantCulture, true, false);
            }
            return new Tuple<ResourceSet, CultureInfo>(resourceSet, targetCulture);
        }

        private class ResourceHolder
        {
            public ResourceHolder(ResourceManager manager)
            {
                this.Manager = manager;
            }

            public ResourceManager Manager { get; private set; }
        }
    }
}