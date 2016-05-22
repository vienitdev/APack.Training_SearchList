using Archpack.Training.ArchUnits.Configuration.V1;
using Archpack.Training.ArchUnits.Contracts.V1;
using Archpack.Training.ArchUnits.Logging.V1;
using Archpack.Training.Properties;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Services;

namespace Archpack.Training.ArchUnits.Arcs.PdfDocuments.V1
{
    /// <summary>
    /// PdfDocument
    /// </summary>
    public class PdfDocument
    {
        public IPdfService Service { get; private set; }
        public Parameter[] Parameters { get; private set; }

        private string endPoint = "";
        private int maxSize = 0;

        /// <summary>
        /// PdfDocumentのコンストラクタ
        /// </summary>
        public PdfDocument()
        {
            var config = ServiceConfigurationLoader.Load();
            this.endPoint = config.AppSettings["pdfServiceUrl"].ToString();
            this.maxSize = int.Parse(config.AppSettings["pdfMaxSize"].ToString());
            this.Service = new PdfServiceEx() { Url = this.endPoint };
        }

        /// <summary>
        /// PdfDocumentのコンストラクタ
        /// </summary>
        /// <param name="service"></param>
        public PdfDocument(IPdfService service)
        {
            var config = ServiceConfigurationLoader.Load();            
            this.maxSize = int.Parse(config.AppSettings["pdfMaxSize"].ToString());
            this.Service = service;
        }

        /// <summary>
        /// 「PDF帳票出力サービス」 を実行し PDF ファイルコンテンツを取得します。
        /// </summary>
        /// <param name="documentDatas">帳票ID ごとのデータの配列</param>
        /// <returns>PDFのバイトデータ配列</returns>
        public byte[] CreatePdf(params DocumentData[] documentDatas)
        {
            Contract.NotNull(documentDatas, "documentDatas");
            Contract.Assert((documentDatas.Length > 0), "documentDatas");

            var parameters = new List<Parameter>();

            foreach (var documentData in documentDatas)
            {
                List<DataGroupMap> maps = new List<DataGroupMap>();

                foreach (var dataGroup in documentData.DataGroups)
                {
                    DataGroupMap map = new DataGroupMap();
                    map.GroupName = dataGroup.GroupName;

                    List<ItemData> items = new List<ItemData>();

                    foreach (var item in dataGroup.Items)
                    {
                        ItemData data = new ItemData { ItemName = item.ItemName, Value = item.Value };
                        items.Add(data);
                    }

                    map.ItemDataList = items.ToArray();
                    maps.Add(map);
                }

                parameters.Add(new Parameter() { DocumentId = documentData.DocumentId, DataGroupMaps = maps.ToArray() });
            }

            var param = parameters.ToArray();

            try
            {
                byte[] result = this.Service.createPDF(param);

                if (result.Length == 0)
                {
                    throw new ApplicationException(Resources.ZeroLengthDownloadFile);
                }

                if (result.Length > maxSize)
                {
                    throw new ApplicationException(string.Format(Resources.MaxSizeDownloadFile, "10MB"));
                }

                return result;
            }
            catch (Exception ex)
            {
                OutputLog(ex);
                throw new PDFDocumentException(ex.Message, ex);
            }
        }

        /// <summary>
        /// OutputLog
        /// </summary>
        /// <param name="exception"></param>
        private void OutputLog(Exception exception)
        {
            var serviceConfig = ServiceConfigurationLoader.Load();
            var adapterSettings = LogConfiguration.CreateLogAdapterSetting(serviceConfig.Raw);
            var target = new Logger(HttpContext.Current.Request.RawUrl, adapterSettings);
            var logData = new LogData();
            logData.LogName = "trace";
            logData.User = HttpContext.Current.User.Identity.Name;
            logData.Message = exception.Message;
            logData.Exception = exception;
            target.Error(logData);
        }
    }
}