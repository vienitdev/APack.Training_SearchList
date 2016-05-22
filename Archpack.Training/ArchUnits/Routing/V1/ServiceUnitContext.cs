using Archpack.Training.ArchUnits.Configuration.V1;
using Archpack.Training.ArchUnits.Container.V1;
using Archpack.Training.ArchUnits.Contracts.V1;
using Archpack.Training.ArchUnits.Logging.V1;
using Archpack.Training.ArchUnits.Path.V1;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Web;

namespace Archpack.Training.ArchUnits.Routing.V1
{
    /// <summary>
    /// サービスユニットの実行コンテキストを保持します。
    /// </summary>
    public class ServiceUnitContext
    {
        /// <summary>
        /// サービスユニットのPathを指定してインスタンスを初期化します。
        /// </summary>
        /// <param name="path">パス</param>
        public ServiceUnitContext(string path): this(path, null)
        {
        }
        /// <summary>
        /// サービスのPathとユーザー情報を指定してインスタンスを初期化します。
        /// </summary>
        /// <param name="path">パス</param>
        /// <param name="identity">ユーザー情報</param>
        public ServiceUnitContext(string path, IPrincipal user) {
            Contract.NotEmpty(path, "path");
            this.Properties = new Dictionary<string, Object>();
            this.Initialize(path, user);
        }


        /// <summary>
        /// コンテキストの一意なIDを取得します。
        /// </summary>
        public Guid Id { get; private set; }
        /// <summary>
        /// 対象のサービスユニット名を取得します。
        /// </summary>
        public string ServiceUnitName { get; private set; }
        /// <summary>
        /// 対象のバージョンを取得します。
        /// </summary>
        public string Version { get; private set; }
        /// <summary>
        /// 対象のロールを取得します。
        /// </summary>
        public string Role { get; private set; }
        /// <summary>
        /// リクエスト情報を取得します。
        /// </summary>
        public ServiceUnitRequest Request { get; private set; }
        /// <summary>
        /// 対象の構成情報を取得します。
        /// </summary>
        public ServiceConfiguration Configuration { get; private set; }
        /// <summary>
        /// コンテキストの追加情報を設定するためのオブジェクトを取得します。
        /// </summary>
        public IDictionary<string, object> Properties { get; private set; }

        /// <summary>
        /// ログインスタンスを格納する <see cref="Archpack.Training.ArchUnits.Logging.V1.LogContext"/> のインスタンスを取得します。
        /// </summary>
        public LogContext LogContext { get; private set; }

        public IPrincipal User { get; private set; }

        /// <summary>
        /// サービスを保持するコンテナを取得します。
        /// </summary>
        public Container.V1.IServiceContainer ServiceContainer { get; private set; }

        private void Initialize(string path, IPrincipal user)
        {
            ServiceContainer = GlobalContainer.CreateChild();
            
            this.Id = Guid.NewGuid();
            this.User = user;

            var info = PathInfoBuilder.Build(path);
            if (info == null)
            {
                return;
            }
            this.ServiceUnitName = info.ServiceUnitName;
            this.Version = info.Version;
            this.Role = info.Role;
            this.Request = new ServiceUnitRequest(this, info);
            this.Configuration = ServiceConfigurationLoader.Load(info.ServiceUnitName, info.Version, info.Role);

            var logConfig = LogConfiguration.CreateLogAdapterSetting(this.Configuration.Raw);
            
            var logger = new Logger(info.ProcessPath, logConfig);
            IIdentity identity = (this.User != null) ? this.User.Identity : new GenericIdentity("Anonymous");

            this.LogContext = new LogContext(logger, this.Id, identity);

            ServiceContainer.AddInstance(this);
            ServiceContainer.AddInstance(this.LogContext);
            ServiceContainer.AddInstance(this.Configuration);

        }
    }
}