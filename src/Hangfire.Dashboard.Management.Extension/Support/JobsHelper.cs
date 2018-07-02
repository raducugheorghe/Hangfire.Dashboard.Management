using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using Hangfire.Dashboard.Management.Extension.Metadata;
using Hangfire.JobSDK;

namespace Hangfire.Dashboard.Management.Extension.Support
{
    public static class JobsHelper
    {
        public static List<JobMetadata> Metadata { get; private set; }
        internal static List<ManagementPageAttribute> Pages { get; set; }
        static JobsHelper()
        {
            Metadata = new List<JobMetadata>();
            Pages = new List<ManagementPageAttribute>();
        }

        internal static void GetAllJobs(Assembly assembly)
        {
            
            foreach (Type ti in  assembly.GetTypes().Where(x => !x.IsInterface && typeof(IJob).IsAssignableFrom(x) && x.Name != (typeof(IJob).Name)))
            {

                if (ti.GetCustomAttributes(true).OfType<ManagementPageAttribute>().Any())
                {
                    var attr = ti.GetCustomAttribute<ManagementPageAttribute>();
                    if (!Pages.Any(p => p.Category == attr.Category))
                    {
                        Pages.Add(attr);
                    }
                    
                    foreach (var methodInfo in ti.GetMethods().Where(m => m.DeclaringType == ti && m.GetCustomAttributes(true).OfType<DisplayNameAttribute>().Any()))
                    {
                        var meta = new JobMetadata { Type = ti, Queue = attr.Queue, Category = attr.Category};

                        meta.MethodInfo = methodInfo;
                        meta.DisplayName = methodInfo.GetCustomAttribute<DisplayNameAttribute>().DisplayName;

                        if (methodInfo.GetCustomAttributes(true).OfType<DescriptionAttribute>().Any())
                        {
                            meta.Description = methodInfo.GetCustomAttribute<DescriptionAttribute>().Description;
                        }

                        Metadata.Add(meta);
                    }
                }
            }
        }
    }
}
