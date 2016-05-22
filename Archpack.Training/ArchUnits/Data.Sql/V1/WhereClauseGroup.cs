using System;
using System.Collections.Generic;
using System.Linq;
using Archpack.Training.ArchUnits.Collections.V1;

namespace Archpack.Training.ArchUnits.Data.Sql.V1
{
    public class WhereClauseGroup
    {
        private List<string> clauses = new List<string>();
        private Dictionary<string, object> parameters = new Dictionary<string, object>();

        public WhereClauseGroup(ISqlDefinitionFactory factory)
        {
            this.Factory = factory;
        }

        public ISqlDefinitionFactory Factory { get; private set; }

        public WhereClauseGroup QueryText(string query)
        {
            this.clauses.Add(query);
            return this;
        }

        public WhereClauseGroup Named(string queryName, params object[] replaceValues)
        {
            var plain = this.Factory.GetPlainText(queryName);
            var query = this.Factory.ReplacePlaceholder(plain, replaceValues);
            this.QueryText(query);
            return this;
        }

        public WhereClauseGroup NamedIf(bool condition, string queryName, params object[] replaceValues)
        {
            if (condition)
            {
                return Named(queryName, replaceValues);
            }

            return this;
        }

        public WhereClauseGroup QueryTextIf(bool condition, string query)
        {
            if (condition)
            {
                return QueryText(query);
            }

            return this;
        }

        public WhereClauseGroup Parameter(string name, object value)
        {
            if (this.parameters.ContainsKey(name))
            {
                this.parameters[name] = value;
            }
            else
            {
                this.parameters.Add(name, value);
            }
            return this;
        }

        public WhereClauseGroup Parameter(IEnumerable<KeyValuePair<string, object>> parameters)
        {
            foreach (var keyValue in parameters)
            {
                this.Parameter(keyValue.Key, keyValue.Value);
            }
            return this;
        }

        public WhereClauseGroup ParameterIf(bool condition, string name, object value)
        {
            if (condition)
            {
                return Parameter(name, value);
            }

            return this;
        }

        public WhereClauseGroup ParameterIf(bool condition, string name, Func<object> func)
        {
            if (condition)
            {
                return Parameter(name, func());
            }

            return this;
        }

        public WhereClauseGroup ParameterIf(bool condition, IEnumerable<KeyValuePair<string, object>> parameters)
        {
            if (condition)
            {
                return Parameter(parameters);
            }

            return this;
        }

        public IEnumerable<string> Clause
        {
            get { return this.clauses; }
        }

        public void Apply(QueryClausePartItem item, bool useAnd = false)
        {
            if (clauses.Count > 0)
            {
                item.Where(clauses.ConcatWith(useAnd ? " AND " : " OR " , "({0})"));
            }
            if (parameters.Count > 0)
            {
                item.Parameter(parameters);
            }
            clauses.Clear();
            parameters.Clear();
        }
    }
}