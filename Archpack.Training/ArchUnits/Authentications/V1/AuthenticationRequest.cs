using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Archpack.Training.ArchUnits.Authentications.V1
{
    /// <summary>
    /// 認証要求を保持します。
    /// </summary>
    public class AuthenticationRequest
    {
        /// <summary>
        /// 指定された識別子とパスワードを指定してインスタンスを初期化します。
        /// </summary>
        /// <param name="identifier">識別子</param>
        /// <param name="password">パスワード</param>
        public AuthenticationRequest(string identifier, string password)
        {
            this.Identifier = identifier;
            this.Password = password;
        }
        
        /// <summary>
        /// 識別子を取得します。
        /// </summary>
        public string Identifier { get; private set; }
        /// <summary>
        /// パスワードを取得します。
        /// </summary>
        public string Password { get; private set; }

    }
}