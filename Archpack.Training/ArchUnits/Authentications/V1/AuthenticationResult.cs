using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web;

namespace Archpack.Training.ArchUnits.Authentications.V1
{
    /// <summary>
    /// 認証結果を表します。
    /// </summary>
    public enum AuthenticationStatus
    {
        /// <summary>
        /// 認証された状態を表します。
        /// </summary>
        Authenticated,
        /// <summary>
        /// 認証が拒否された状態を表します。
        /// </summary>
        Denied,
        /// <summary>
        /// アカウントがロックされている状態を表します。
        /// </summary>
        AccountLockedOut
    }

    /// <summary>
    /// 認証結果を保持します。
    /// </summary>
    public class AuthenticationResult
    {
        /// <summary>
        /// 指定された認証結果を利用してインスタンスを初期化します。
        /// </summary>
        /// <param name="status">認証状態</param>
        public AuthenticationResult(AuthenticationStatus status)
        {
            this.Status = status;
            this.Claims = new List<Claim>();
        }
        /// <summary>
        /// 認証されているかどうかを取得します。
        /// </summary>
        public bool IsAuthenticate
        {
            get
            {
                return (this.Status == AuthenticationStatus.Authenticated);
            }
        }
        /// <summary>
        /// 認証結果を取得します。
        /// </summary>
        public AuthenticationStatus Status { get; private set; }
        /// <summary>
        /// 認証が成功している場合の <see cref="Claim"/> を取得します。
        /// </summary>
        public List<Claim> Claims { get; private set; }

    }

}