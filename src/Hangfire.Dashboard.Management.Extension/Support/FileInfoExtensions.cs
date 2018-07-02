using System;
using System.IO;

namespace Hangfire.Dashboard.Management.Extension.Support
{
    public static class FileInfoExtensions
    {
        public static string GetNameWithoutExtension(this FileInfo fileinfo)
        {
            return fileinfo.Name.Replace(fileinfo.Extension, "");
        }
    }
}
