using Archpack.Training.ArchUnits.Contracts.V1;
using Archpack.Training.ArchUnits.Collections.V1;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections;
using System.Text;

namespace Archpack.Training.ArchUnits.Data.Sql.V1
{
    public class QueryClausePart
    {
        private List<QueryClausePartItem> items = new List<QueryClausePartItem>();

        public QueryClausePart(ISqlDefinitionFactory factory)
        {
            Contract.NotNull(factory, "factory");
            this.Factory = factory;
        }

        /// <summary>
        /// Where句およびFrom句を追加するための <see cref="QueryClausePartItem"/> を取得します。
        /// </summary>
        /// <returns>Where句およびFrom句を追加するための <see cref="QueryClausePartItem"/> </returns>
        public QueryClausePartItem CreateItem()
        {
            var item = new QueryClausePartItem(this.Factory);
            items.Add(item);
            return item;
        }

        protected IEnumerable<QueryClausePartItem> Items
        {
            get { return items; }
        }

        /// <summary>
        /// 名前づけSQLを取得するための <see cref="ISqlDefinitionFactory"/> を取得します。
        /// </summary>
        public ISqlDefinitionFactory Factory { get; private set; }

        /// <summary>
        /// 全ての <see cref="QueryClausePartItem"/>に設定されたFrom句を結合した文字列を取得します。
        /// </summary>
        public string FromClause
        {
            get
            {
                var enableItems = this.items.Where(i => i.FromClauses.Any());
                var namedFroms = enableItems
                    .SelectMany(i => i.FromClauses.Where(f => f is NamedClause))
                    .Cast<NamedClause>()
                    .GroupBy(c => c.QueryName)
                    .Select(g => g.First());
                var plainFroms = enableItems.SelectMany(i => i.FromClauses.Where(f => f is PlainTextClause));
                var namedWheres = this.items.SelectMany(i => i.WhereClauses);

                return GetFromQueries(namedFroms, plainFroms, namedWheres).Concat(plainFroms.Select(p => p.Query)).ConcatWith(" ");
            }
        }

        private IEnumerable<string> GetFromQueries(IEnumerable<NamedClause> namedFroms,
            IEnumerable<IClause> plainFroms,
            IEnumerable<IClause> wheres)
        {
            var graph = new DependencyGraph<string>();
            var loadedClauses = namedFroms.ToDictionary(f => f.QueryName, f => f.Query);
            foreach (var from in namedFroms)
            {
                if (!graph.HasNode(from.QueryName))
                {
                    graph.AddNode(from.QueryName);
                }
                BuildDepGraph(graph, loadedClauses, from, from.QueryName, true);
            }
            foreach (var from in plainFroms)
            {
                BuildDepGraph(graph, loadedClauses, from, string.Empty, true, true);
            }
            foreach (var where in wheres)
            {
                BuildDepGraph(graph, loadedClauses, where, string.Empty, true, true);
            }
            return graph.TopologicalSort().Select(queryName => loadedClauses[queryName]);
        }

        private void BuildDepGraph(DependencyGraph<string> graph, Dictionary<string, string> loadedClauses,
            IClause clause, string queryName, bool unCheckHasNode, bool notSetToNode = false)
        {
            if (unCheckHasNode || !graph.HasNode(queryName))
            {
                foreach (var depend in this.Factory.GetDependNames(clause.Query))
                {
                    if (!loadedClauses.ContainsKey(depend))
                    {
                        var newClause = new NamedClause(this.Factory, depend);
                        loadedClauses.Add(newClause.QueryName, newClause.Query);
                        BuildDepGraph(graph, loadedClauses, newClause, newClause.QueryName, false);
                    }
                    if (notSetToNode)
                    {
                        graph.AddNode(depend);
                    }
                    else
                    {
                        graph.AddEdge(depend, queryName);
                    }
                }
            }
        }
        /// <summary>
        /// 全ての <see cref="QueryClausePartItem"/>に設定されたWhere句を結合した文字列を取得します。
        /// </summary>
        public string WhereClause
        {
            get
            {
                return
                    this.items.Where(i => i.WhereClauses.Any())
                        .Select(item => string.Format("({0})", item.WhereClauses.Select(w => string.Format("({0})", w.Query)).ConcatWith(" AND ")))
                        .ConcatWith(" AND ");
            }
        }
        /// <summary>
        /// 全ての <see cref="QueryClausePartItem"/>に設定されたパラメーターを取得します。
        /// </summary>
        public IEnumerable<KeyValuePair<string, object>> Parameters
        {
            get
            {
                return this.Items.SelectMany(item => item.Parameters);
            }
        }

        public string CreateQueryText(bool appendWhere = true)
        {
            var sb = new StringBuilder();
            var from = this.FromClause;
            if (!string.IsNullOrEmpty(from))
            {
                sb.Append(from);
            }
            var where = this.WhereClause;
            if (!string.IsNullOrEmpty(where))
            {
                if (appendWhere)
                {
                    sb.Append(" WHERE 1 = 1");
                }
                sb.Append(" AND");
                sb.Append(" (" + this.WhereClause + ")");
            }
            return sb.ToString();
        }
    }

    /// <summary>
    /// SQLのWhere句とFrom句を個別に組み立てる機能を提供します。
    /// </summary>
    public sealed class DataQueryClausePart: QueryClausePart
    {
        private DataQuery dataQuery;
        
        /// <summary>
        /// <see cref="DataQuery"/> を指定して、インスタンスを初期化します。
        /// </summary>
        /// <param name="dataQuery">組み立て結果を設定する <see cref="DataQuery"/> </param>
        public DataQueryClausePart(DataQuery dataQuery):base(dataQuery == null ? null : dataQuery.Factory)
        {
            Contract.NotNull(dataQuery, "dataQuery");

            this.dataQuery = dataQuery;
        }
        
        /// <summary>
        /// <see cref="DataQuery"/>に対して、設定されているWhere句およびFrom句を組みたて、末尾に追加します。
        /// </summary>
        /// <param name="appendWhere"><see cref="QueryClausePartItem"/>の結合結果で、先頭に Where を追加するかどうかを指定します。</param>
        /// <returns>Where句およびFrom句が末尾に追加された <see cref="DataQuery"/> </returns>
        public DataQuery Build(bool appendWhere = true)
        {
            var queryText = this.CreateQueryText(appendWhere);
            if (!string.IsNullOrEmpty(queryText))
            {
                this.dataQuery.AppendQuery(queryText);
            }

            foreach (var param in this.Items.SelectMany(item => item.Parameters))
            {
                this.dataQuery.SetParameter(param.Key, param.Value);
            }
            return dataQuery;
        }
        
        
    }
    /// <summary>
    /// SQLのWhere句とFrom句を追加する機能を提供します。
    /// </summary>
    public sealed class QueryClausePartItem
    {
        private Dictionary<string, object> parameters = new Dictionary<string, object>();
        private List<IClause> fromClauses = new List<IClause>();
        private List<IClause> whereClauses = new List<IClause>();

        private List<string> appendedFromClauses = new List<string>();
        /// <summary>
        /// <see cref="ISqlDefinitionFactory"/> を指定して、インスタンスを初期化します。
        /// </summary>
        /// <param name="definitionFactory">名前付けSQLを取得するための <see cref="ISqlDefinitionFactory"/> </param>
        public QueryClausePartItem(ISqlDefinitionFactory definitionFactory)
        {
            this.Factory = definitionFactory;
        }
        /// <summary>
        /// 名前づけSQLを取得するための <see cref="ISqlDefinitionFactory"/> を取得します。
        /// </summary>
        public ISqlDefinitionFactory Factory { get; private set; }

        /// <summary>
        /// 指定された名前付けSQLをFrom句に追加します。
        /// </summary>
        /// <param name="queryName">名前付けSQL名</param>
        /// <returns><see cref="QueryClausePartItem"/>のインスタンス</returns>
        public QueryClausePartItem NamedFrom(string queryName)
        {
            if (!FromClauses.Any(c => c is NamedClause && ((NamedClause)c).QueryName == queryName))
            {
                this.fromClauses.Add(new NamedClause(this.Factory, queryName));
            }
            return this;
        }
        /// <summary>
        /// コンディションが真の場合だけ、指定された名前付けSQLをFrom句に追加します。
        /// </summary>
        /// <param name="condition">名前付けSQLを追加するかしないか</param>
        /// <param name="queryName">名前付けSQL名</param>
        /// <returns><see cref="QueryClausePartItem"/>のインスタンス</returns>
        public QueryClausePartItem NamedFromIf(bool condition, string queryName)
        {
            if (condition)
            {
                return NamedFrom(queryName);
            }
            return this;
        }
        /// <summary>
        /// SQL文字列をFrom句に追加します。
        /// </summary>
        /// <param name="query">SQL文字列</param>
        /// <returns><see cref="QueryClausePartItem"/>のインスタンス</returns>
        public QueryClausePartItem From(string query)
        {
            this.fromClauses.Add(new PlainTextClause(query));
            return this;
        }
        /// <summary>
        /// 指定された名前付けSQLをWhere句に追加します。
        /// </summary>
        /// <param name="queryName">名前付けSQL名</param>
        /// <param name="replaceValues">置換文字列の値</param>
        /// <returns><see cref="QueryClausePartItem"/>のインスタンス</returns>
        public QueryClausePartItem NamedWhere(string queryName, params object[] replaceValues)
        {
            this.whereClauses.Add(new NamedClause(this.Factory, queryName, replaceValues));
            return this;
        }
        /// <summary>
        /// SQL文字列をWhere句に追加します。
        /// </summary>
        /// <param name="query">SQL文字列</param>
        /// <returns><see cref="QueryClausePartItem"/>のインスタンス</returns>
        public QueryClausePartItem Where(string query)
        {
            this.whereClauses.Add(new PlainTextClause(query));
            return this;
        }
        /// <summary>
        /// コンディションが真の場合だけ、指定された名前付けSQLをWhere句に追加します。
        /// </summary>
        /// <param name="condition">名前付けSQLを追加するかしないか</param>
        /// <param name="queryName">名前付けSQL名</param>
        /// <returns><see cref="QueryClausePartItem"/>のインスタンス</returns>
        public QueryClausePartItem NamedWhereIf(bool condition, string queryName, params object[] replaceValues)
        {
            if (condition)
            {
                return NamedWhere(queryName, replaceValues);
            }

            return this;
        }
        /// <summary>
        /// コンディションが真の場合だけ、SQL文字列をWhere句に追加します。
        /// </summary>
        /// <param name="condition">SQLを追加するかしないか</param>
        /// <param name="query">SQL文字列</param>
        /// <returns><see cref="QueryClausePartItem"/>のインスタンス</returns>
        public QueryClausePartItem WhereIf(bool condition, string query)
        {
            if (condition)
            {
                return Where(query);
            }

            return this;
        }

        /// <summary>
        /// パラメーターを追加します。
        /// </summary>
        /// <param name="name">パラメーター名</param>
        /// <param name="value">パラメーターの値</param>
        /// <returns><see cref="QueryClausePartItem"/>のインスタンス</returns>
        public QueryClausePartItem Parameter(string name, object value)
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
        /// <summary>
        /// 複数のパラメーターを追加します。
        /// </summary>
        /// <param name="parameters">パラメーター名と値の組</param>
        /// <returns><see cref="QueryClausePartItem"/>のインスタンス</returns>
        public QueryClausePartItem Parameter(IEnumerable<KeyValuePair<string, object>> parameters)
        {
            foreach (var keyValue in parameters)
            {
                this.Parameter(keyValue.Key, keyValue.Value);
            }
            return this;
        }
        /// <summary>
        /// コンディションが真の場合だけ、パラメーターを追加します。
        /// </summary>
        /// <param name="condition">パラメーターを追加するかしないか</param>
        /// <param name="name">パラメーター名</param>
        /// <param name="value">パラメーターの値</param>
        /// <returns><see cref="QueryClausePartItem"/>のインスタンス</returns>
        public QueryClausePartItem ParameterIf(bool condition, string name, object value)
        {
            if (condition)
            {
                return Parameter(name, value);
            }

            return this;
        }
        /// <summary>
        /// コンディションが真の場合だけ、複数のパラメーターを追加します。
        /// </summary>
        /// <param name="condition">パラメーターを追加するかしないか</param>
        /// <param name="parameters">パラメーター名と値の組</param>
        /// <returns><see cref="QueryClausePartItem"/>のインスタンス</returns>
        public QueryClausePartItem ParameterIf(bool condition, IEnumerable<KeyValuePair<string, object>> parameters)
        {
            if (!condition)
            {
                return this;
            }
            foreach (var keyValue in parameters)
            {
                this.Parameter(keyValue.Key, keyValue.Value);
            }
            return this;
        }
        /// <summary>
        /// コンディションが真の場合だけ、パラメーターを追加します。
        /// </summary>
        /// <param name="condition">パラメーターを追加するかしないか</param>
        /// <param name="name">パラメーター名</param>
        /// <param name="func">パラメーターの値を取得するメソッド</param>
        /// <returns><see cref="QueryClausePartItem"/>のインスタンス</returns>
        public QueryClausePartItem ParameterIf(bool condition, string name, Func<object> func)
        {
            if (condition)
            {
                return Parameter(name, func());
            }

            return this;
        }
        /// <summary>
        /// コンディションが真の場合だけ、複数のパラメーターを追加します。
        /// </summary>
        /// <param name="condition">パラメーターを追加するかしないか</param>
        /// <param name="func">パラメーターの値を取得するメソッド</param>
        /// <returns><see cref="QueryClausePartItem"/>のインスタンス</returns>
        public QueryClausePartItem ParameterIf(bool condition, Func<IEnumerable<KeyValuePair<string, object>>> func)
        {
            if (condition)
            {
                return Parameter(func());
            }

            return this;
        }
        /// <summary>
        /// パラメーターの一覧を取得します。
        /// </summary>
        public IEnumerable<KeyValuePair<string, object>> Parameters
        {
            get { return this.parameters; }
        }
        /// <summary>
        /// 追加されているFrom句を連結した文字列を取得します。
        /// </summary>
        public IEnumerable<IClause> FromClauses
        {
            get { return fromClauses; }
        }
        /// <summary>
        /// 追加されているWhere句を連結した文字列を取得します。
        /// </summary>
        public IEnumerable<IClause> WhereClauses
        {
            get { return whereClauses; }
        }
    }
    /// <summary>
    /// SQLの句を表すインターフェイスを定義します。
    /// </summary>
    public interface IClause : IStructuralEquatable
    {
        /// <summary>
        /// SQL文字列を取得します。
        /// </summary>
        string Query { get; }
    }
    /// <summary>
    /// 名前付きSQLで得られた句を保持します。
    /// </summary>
    public sealed class NamedClause : IClause
    {
        private ISqlDefinitionFactory factory;
        /// <summary>
        /// 指定された引数を利用してインスタンスを初期化します。
        /// </summary>
        /// <param name="factory">名前づけSQLを取得するための <see cref="ISqlDefinitionFactory"/> </param>
        /// <param name="queryName">名前付けSQL名</param>
        /// <param name="replaceValues">置換する値</param>
        public NamedClause(ISqlDefinitionFactory factory, string queryName, params object[] replaceValues)
        {
            this.factory = factory;
            this.QueryName = queryName;
            var plain = factory.GetPlainText(queryName);
            this.Query = factory.ReplacePlaceholder(plain, replaceValues);
        }
        /// <summary>
        /// SQL文字列を取得します。
        /// </summary>
        public string Query { get; private set; }
        /// <summary>
        /// 名前付けSQL名を取得します。
        /// </summary>
        public string QueryName { get; private set; }

        bool IStructuralEquatable.Equals(object other, IEqualityComparer comparer)
        {
            if (other == null)
            {
                return false;
            }
            var value = other as NamedClause;
            if (value == null)
            {
                return false;
            }
            return comparer.Equals(this.QueryName, value.QueryName) && comparer.Equals(this.Query, value.Query);
        }

        int IStructuralEquatable.GetHashCode(IEqualityComparer comparer)
        {
            return CombineHashCodes(CombineHashCodes(comparer.GetHashCode(this.QueryName), comparer.GetHashCode(this.Query)), typeof(NamedClause).GetHashCode());
        }

        private int CombineHashCodes(int h1, int h2)
        {
            return (((h1 << 5) + h1) ^ h2);
        }
    }
    /// <summary>
    /// SQL文で得られた句を富士します。
    /// </summary>
    public sealed class PlainTextClause : IClause
    {
        /// <summary>
        /// 指定されたSQL文を利用して、インスタンスを初期化します。
        /// </summary>
        /// <param name="query">SQL文</param>
        public PlainTextClause(string query)
        {
            this.Query = query;
        }
        /// <summary>
        /// SQL文字列を取得します。
        /// </summary>
        public string Query { get; private set; }

        bool IStructuralEquatable.Equals(object other, IEqualityComparer comparer)
        {
            if (other == null)
            {
                return false;
            }
            var value = other as PlainTextClause;
            if (value == null)
            {
                return false;
            }
            return comparer.Equals(this.Query, value.Query);
        }

        int IStructuralEquatable.GetHashCode(IEqualityComparer comparer)
        {
            return CombineHashCodes(comparer.GetHashCode(this.Query), typeof(PlainTextClause).GetHashCode());
        }
        private int CombineHashCodes(int h1, int h2)
        {
            return (((h1 << 5) + h1) ^ h2);
        }
    }


}