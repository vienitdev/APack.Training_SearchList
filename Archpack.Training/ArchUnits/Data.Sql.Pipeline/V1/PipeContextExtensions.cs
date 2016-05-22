using Archpack.Training.ArchUnits.Pipeline.V1;
using System.Collections.Generic;
using System.Data.Entity;

namespace Archpack.Training.ArchUnits.Data.Sql.Pipeline.V1
{
    public static class PipeContextExtensions
    {

        public static DataQueryParameters GetDataQueryParameters(this PipeContext context, string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                return null;
            }

            if (!context.Items.ContainsKey(name))
            {
                return null;
            }

            var paramter = context.Items[name] as DataQueryParameters;
            return paramter;
        }

        public static void SetDataQueryParameters(this PipeContext context, string name, DataQueryParameters parameters)
        {
            context.Items[name] = parameters;
        }

    }

    
    public class DataQueryParameters
    {
        private List<KeyValuePair<string, object>> parameters = new List<KeyValuePair<string, object>>();
        public DbContext DbContext { get; set; }

        public void Add(string name, object value)
        {
            this.parameters.Add(new KeyValuePair<string, object>(name, value));
        }

        public IEnumerable<KeyValuePair<string, object>> Parameters 
        { 
            get { return this.parameters.AsReadOnly();  } 
        }

    }
}