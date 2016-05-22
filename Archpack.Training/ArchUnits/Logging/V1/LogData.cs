using System;
using System.Collections.Generic;

namespace Archpack.Training.ArchUnits.Logging.V1
{
    /// <summary>
    /// ログの出力内容を保持します。
    /// </summary>
    public class LogData
    {
        private IDictionary<string, object> items = new Dictionary<string, object>();

        /// <summary>
        /// インスタンスを初期化します。
        /// </summary>
        public LogData()
        {
            this.LogId = Guid.Empty;
        }

        /// <summary>
        /// ログ名を取得または設定します。
        /// </summary>
        public virtual string LogName { get; set; }

        /// <summary>
        /// アクセス先URLを取得または設定します。
        /// </summary>
        public string Uri
        {
            get { return this.Get<string>("Uri"); }
            set { this.Set("Uri", value); }
        }
        /// <summary>
        /// ログに出力する項目の一覧を取得します。
        /// </summary>
        public IDictionary<string, object> Items
        {
            get { return items; }
        }

        /// <summary>
        /// ログIDを取得または設定します。
        /// </summary>
        public Guid LogId
        {
            get { return this.Get<Guid>("LogId"); }
            set { this.Set("LogId", value); }
        }

        /// <summary>
        /// メッセージを取得または設定します。
        /// </summary>
        public string Message
        {
            get { return this.Get<string>("Message"); }
            set { this.Set("Message", value); }
        }

        /// <summary>
        /// 例外を取得または設定します。
        /// </summary>
        public Exception Exception
        {
            get { return this.Get<Exception>("Exception"); }
            set { this.Set("Exception", value); }
        }

        /// <summary>
        /// ユーザー名を取得または設定します。
        /// </summary>
        public string User
        {
            get { return this.Get<string>("User"); }
            set { this.Set("User", value); }
        }
        /// <summary>
        /// 指定されたキーに一致するログ項目の値を取得します。
        /// </summary>
        /// <typeparam name="T">ログ項目の型</typeparam>
        /// <param name="key">取得するキー名</param>
        /// <returns>ログ項目の値</returns>
        public T Get<T>(string key)
        {
            if (items.ContainsKey(key))
            {
                return (T)items[key];
            }
            return default(T);
        }
        /// <summary>
        /// 指定されたキーと値でログ項目を設定します。
        /// </summary>
        /// <typeparam name="T">ログ項目の型</typeparam>
        /// <param name="key">設定するキー名</param>
        /// <param name="value">ログ項目の値</param>
        public void Set<T>(string key, T value)
        {
            items[key] = value;
        }
        /// <summary>
        /// 指定されたキーに一致するログ項目を削除します。
        /// </summary>
        /// <param name="key">削除するログ項目のキー名</param>
        protected void Remove(string key)
        {
            if (items.ContainsKey(key))
            {
                items.Remove(key);
            }
        }
        /// <summary>
        /// 指定されたキーのログ項目があるかどうかを取得します。
        /// </summary>
        /// <param name="key">キー名</param>
        /// <returns>指定されたキーのログ項目がある場合は true 、そうでない場合は false</returns>
        protected bool Has(string key)
        {
            return items.ContainsKey(key);
        }
    }
}