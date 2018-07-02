using System.Collections.Generic;

namespace Hangfire.Dashboard.Management.Infrastructure
{
    public static class GlobalLoaderConfiguration
    {
        static GlobalLoaderConfiguration()
        {
            Modules = new List<ModuleInfo>();
        }

        public static IList<ModuleInfo> Modules { get; set; }

        public static string WebRootPath { get; set; }

        public static string ContentRootPath { get; set; }
    }
}
