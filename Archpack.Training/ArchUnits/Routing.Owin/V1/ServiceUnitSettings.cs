using Archpack.Training.ArchUnits.Contracts.V1;
using Archpack.Training.ArchUnits.Routing.V1;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Archpack.Training.ArchUnits.Routing.Owin.V1
{
    /// <summary>
    /// <see cref="ServiceUnit"/> の設定を定義します。
    /// </summary>
    public sealed class ServiceUnitSettings
    {
        private ServiceUnitSettings()
        {

        }

        private ServiceUnitSettings(ServiceUnitSettings source)
        {
            this.ContextCreatedHandler = source.ContextCreatedHandler;
        }
        /// <summary>
        /// <see cref="ServiceUnitSettings"/> のインスタンスを作成します。
        /// </summary>
        /// <returns><see cref="ServiceUnitSettings"/> のインスタンス</returns>
        public static ServiceUnitSettings Create()
        {
            return new ServiceUnitSettings();
        }

        /// <summary>
        /// <see cref="ServiceUnitContext"/> が作成された際に実行されるアクションを設定します。
        /// </summary>
        /// <param name="handler"><see cref="ServiceUnitContext"/> が作成された際に実行されるアクション</param>
        /// <returns>指定されたアクションが設定された <see cref="ServiceUnitSettings"/> の新しいインスタンス</returns>
        public ServiceUnitSettings SetContextCreatedHandler(Action<ServiceUnitContext> handler)
        {
            this.ContextCreatedHandler = handler;
            return new ServiceUnitSettings(this);
        }

        /// <summary>
        /// <see cref="ServiceUnitContext"/> が作成された際に実行されるアクションを取得します。
        /// </summary>
        public Action<ServiceUnitContext> ContextCreatedHandler { get; private set; }

    }
}