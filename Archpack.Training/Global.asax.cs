using System;
using System.Data.Entity;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Routing;
using System.Web.Security;
using System.Web.SessionState;
using System.Web.Http;
using System.Net;
using Archpack.Training.ArchUnits.Configuration.V1;
using Archpack.Training.ArchUnits.Logging.V1;
using Archpack.Training.ArchUnits.Routing.WebForm.V1;

namespace Archpack.Training
{
    public class Global : HttpApplication
    {
        void Application_Start(object sender, EventArgs e)
        {
            // アプリケーションのスタートアップで実行するコードです
        }

        /// <summary>
        /// エラー発生時に呼び出される処理を実装します
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Application_Error(object sender, EventArgs e)
        {
            Exception exception = Server.GetLastError();
            //HttpException httpException = exception as HttpException;

            ////404 など 500 以外の Http エラーの場合はロギングを実施しない
            //if (httpException != null)
            //{
            //    if (httpException.GetHttpCode() != (int)HttpStatusCode.InternalServerError)
            //    {
            //        return;
            //    }
            //}

            var logger = GetLogger();
            if (logger == null)
            {
                var logData = new LogData();
                logData.LogName = "error";
                logData.User = HttpContext.Current.User.Identity.Name;
                logData.Message = exception.Message;
                logData.Exception = exception;
                logger.Error(logData); 
            }
            
            Context.Items["Error"] = exception;
        }

        private Logger GetLogger()
        {
            var serviceUnitContext = this.Context.GetServiceUnitContext();
            if (serviceUnitContext == null)
            {
                return GetDefaultLogger();
            }
            var logContext = serviceUnitContext.LogContext;
            if (logContext == null)
            {
                return GetDefaultLogger();
            }
            return logContext.Logger;
        }

        private Logger GetDefaultLogger()
        {
            var serviceConfig = ServiceConfigurationLoader.Load();
            var adapterSettings = LogConfiguration.CreateLogAdapterSetting(serviceConfig.Raw);
            return new Logger(Context.Request.RawUrl, adapterSettings);
        }
    }
}