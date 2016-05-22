using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Archpack.Training.ArchUnits.Authentications.V1
{
    /// <summary>
    /// <see cref="IAuthenticationProvider"/> の実装クラスに名称を指定します・
    /// </summary>
    public class AuthenticationProviderAttribute : Attribute
    {
        /// <summary>
        /// 名称を取得または設定します。
        /// </summary>
        public string Name { get; set; }
    }
}