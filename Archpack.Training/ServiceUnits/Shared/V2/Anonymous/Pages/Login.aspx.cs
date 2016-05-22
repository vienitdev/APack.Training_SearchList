using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Archpack.Training.ArchUnits.Authentications.V1;
//using Archpack.Training.ArchUnits.Authentications.V2;
using Archpack.Training.ArchUnits.Routing.V1;
using System.Diagnostics;

namespace Archpack.Training.ServiceUnits.Shared.V2.Anonymous.Pages
{
    public partial class Login : System.Web.UI.Page
    {
        /// <summary>
        /// 現在の HTTP 要求の言語を設定します。
        /// </summary>
        protected readonly string lang = System.Threading.Thread.CurrentThread.CurrentUICulture.TwoLetterISOLanguageName;

        /// <summary>
        /// ログインエラーのメッセージを取得します。
        /// </summary>
        /// <returns>ログインエラーメッセージ</returns>
        /// <remarks>このメソッドは aspx 側から呼び出すために定義されています。</remarks>
        public string LoginErrorMessage { get; set; }

        protected void Page_Load(object sender, EventArgs e)
        {
            this.LoginErrorMessage = null;
            if (this.Page.IsPostBack)
            {
                this.LogIn(this.userName.Value, this.password.Value);
            }

            this.userName.Attributes["placeholder"] = Resources.StringContents.UserName;
            this.password.Attributes["placeholder"] = Resources.StringContents.Password;
        }

        /// <summary>
        /// ユーザID、パスワードを指定してログイン処理を実行します
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="password"></param>
        private void LogIn(string userName, string password)
        {
            // ユーザー名およびパスワードで認証
            Authentication auth = new Authentication(Context.GetOwinContext());
            var result = auth.SignIn(new AuthenticationRequest(userName, password));

            if (result.IsAuthenticate)
            {
                System.Web.Security.FormsAuthentication.RedirectFromLoginPage(userName, false);
            }
            else if (result.Status == AuthenticationStatus.AccountLockedOut)
            {
                this.LoginErrorMessage = Resources.Messages.AccountLockedOut;
            }
            else 
            {
                this.LoginErrorMessage = Resources.Messages.InvalidUserNameOrPassword;
            }
        }
    }
}