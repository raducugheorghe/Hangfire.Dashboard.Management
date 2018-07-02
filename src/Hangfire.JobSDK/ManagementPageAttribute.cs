using System;
using System.Net;

namespace Hangfire.JobSDK
{
    public class ManagementPageAttribute : Attribute
    {
        public string Title { get; }
        public string MenuName { get; }
        public string Queue { get; }

        public string Category => WebUtility.UrlEncode(MenuName.ToLower().Replace(" ",""));

        public ManagementPageAttribute(string menuName, string title, string queue="default")
        {
            Title = title;
            MenuName = menuName;
            Queue = queue;
        }
    }
}