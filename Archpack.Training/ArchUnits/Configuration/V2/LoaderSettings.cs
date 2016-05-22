using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Archpack.Training.ArchUnits.Configuration.V2
{
    /// <summary>
    /// <see cref="ServiceConfigurationLoader"/> に対する設定を保持します。
    /// </summary>
    public sealed class LoaderSettings
    {
        private static LoaderSettings defaultSettings = new LoaderSettings();

        private LoaderSettings()
        {

        }

        private LoaderSettings(LoaderSettings source)
        {

        }
        /// <summary>
        /// 既定の設定を取得します。
        /// </summary>
        public static LoaderSettings Default {
            get { return defaultSettings; }
        }
        /// <summary>
        /// 環境の名前を取得します。
        /// </summary>
        public string EnvironmentName
        {
            get; private set;
        }
        /// <summary>
        /// 指定された環境の名前が設定された、新しい <see cref="LoaderSettings"/> のインスタンスを返します。
        /// </summary>
        /// <param name="environmentName">環境の名前</param>
        /// <returns>指定された環境の名前が設定された、新しい <see cref="LoaderSettings"/> のインスタンス</returns>
        public LoaderSettings SetEnvironmentName(string environmentName)
        {
            var result = new LoaderSettings(this);
            result.EnvironmentName = environmentName;
            return result;
        }
    }
}