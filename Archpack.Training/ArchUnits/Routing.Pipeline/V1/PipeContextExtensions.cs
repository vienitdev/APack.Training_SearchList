using Archpack.Training.ArchUnits.Contracts.V1;
using Archpack.Training.ArchUnits.Logging.V1;
using Archpack.Training.ArchUnits.Pipeline.V1;
using Archpack.Training.ArchUnits.Routing.V1;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Archpack.Training.ArchUnits.Routing.Pipeline.V1
{
    public static class PipeContextExtensions
    {
        private static class PipeContextKeys 
        {
            public const string LogContext = "LogContext";
            public const string VersionFisicalDirectory = "VersionFisicalDirectory";
        }

        /// <summary>
        /// パイプラインで利用するコンテキスト <see cref="PipeContext"/> を生成します。
        /// </summary>
        /// <param name="serviceContext"></param>
        /// <returns></returns>
        public static PipeContext CreatePipeContext(this ServiceUnitContext serviceContext, object requestData)
        {
            PipeContext result = new PipeContext(serviceContext.ServiceContainer);
            result.RequestData = requestData;

            result.Items.Add(PipeContextKeys.LogContext, serviceContext.LogContext);
            result.Items.Add(PipeContextKeys.VersionFisicalDirectory, serviceContext.GetVersionFisicalDirectory());

            return result;
        }

        public static LogContext GetLogContext(this PipeContext pipeContext) 
        {
            Contract.NotNull(pipeContext, "pipeContext");

            if (!pipeContext.Items.ContainsKey(PipeContextKeys.LogContext))
            {
                return null;
            }

            return pipeContext.Items[PipeContextKeys.LogContext] as LogContext;
        }

        public static string GetVersionFisicalDirectory(this PipeContext pipeContext)
        {
            Contract.NotNull(pipeContext, "pipeContext");

            if (!pipeContext.Items.ContainsKey(PipeContextKeys.VersionFisicalDirectory))
            {
                return null;
            }

            return pipeContext.Items[PipeContextKeys.VersionFisicalDirectory] as string;
        }

    }
}