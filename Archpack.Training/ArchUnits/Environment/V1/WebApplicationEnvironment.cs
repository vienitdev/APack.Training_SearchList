using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Hosting;

namespace Archpack.Training.ArchUnits.Environment.V1
{
    public class WebApplicationEnvironment: IApplicationEnvironment
    {
        public WebApplicationEnvironment(string environmentName)
        {
            this.EnvironmentName = environmentName;
            this.RootDir = this.GetRootDir();
            this.ApplicationRoot = HostingEnvironment.ApplicationVirtualPath;
        }

        public WebApplicationEnvironment()
        {
            this.RootDir = this.GetRootDir();
            this.EnvironmentName = (new EnvironmentSelector()).GetEnvironment(this.RootDir);
            this.ApplicationRoot = HostingEnvironment.ApplicationVirtualPath;
        }

        private string GetRootDir()
        {
            return HostingEnvironment.MapPath("~/");
        }

        public string EnvironmentName { get; private set; }

        public string RootDir { get; private set; }

        public string MapPath(string virtualPath)
        {
            return HostingEnvironment.MapPath(virtualPath);
        }

        public string ApplicationRoot { get; private set; }
    }
}