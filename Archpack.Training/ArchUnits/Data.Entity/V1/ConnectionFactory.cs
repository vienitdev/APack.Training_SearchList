using Archpack.Training.ArchUnits.Configuration.V1;
using Archpack.Training.ArchUnits.Contracts.V1;
using System;
using System.Data.Common;
using System.Data.Entity;
using System.Data.Entity.Infrastructure.DependencyResolution;
using System.Data.Entity.Infrastructure;


namespace Archpack.Training.ArchUnits.Data.Entity.V1
{
    /// <summary>
    /// EntityFramework で利用する <see cref="DbConnection"/> を取得します。
    /// </summary>
    public static class ConnectionFactory
    {

        private static DbConnection CreateFromDbProviderFactory(string providerName, string connectionString)
        {
            DbProviderFactory providerFactory = DbProviderFactories.GetFactory(providerName);
            
            DbConnection connection = providerFactory.CreateConnection();
            connection.ConnectionString = connectionString;

            return connection;
        }

        private static DbConnection CreateFromDefaultDbConnectionFactory(string connectionString)
        {
            IDbConnectionFactory connectionFactory = DbConfiguration.DependencyResolver.GetService<IDbConnectionFactory>();

            if (connectionFactory == null)
            {
                throw new InvalidOperationException("The entityFramework/defaultConnectionFactory is not defined in the configuration file");
            }

            DbConnection connection = connectionFactory.CreateConnection(connectionString);

            return connection;
        }

        /// <summary>
        /// 接続定義を利用して<see cref="DbConnection"/>のインスタンスを作成します。
        /// </summary>
        /// <remarks>
        /// プロバイダーが指定されていない場合は EntityFramework　が利用する <see cref="IDbConnectionFactory"/> をもとにインスタンスを作成します。
        /// </remarks>
        /// <param name="item">接続定義</param>
        /// <returns><see cref="DbConnection"/></returns>
        public static DbConnection Create(ConnectionStringItem item)
        {
            Contract.NotNull(item, "item");

            if (!string.IsNullOrEmpty(item.ProviderName))
            {
                return CreateFromDbProviderFactory(item.ProviderName, item.ConnectionString);
            }
            else
            {
                return CreateFromDefaultDbConnectionFactory(item.ConnectionString);
            }
        }
    }
}