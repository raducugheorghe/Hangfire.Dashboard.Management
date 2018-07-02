﻿using Hangfire.Dashboard.Pages;

namespace Hangfire.Dashboard.Management.Extension.Pages
{
    internal class ManagementPage : RazorPage
    {
        public const string Title = "Management";
        public const string UrlRoute = "/management";
      
        public override void Execute()
        {
            WriteLiteral("\r\n");
            Layout = new LayoutPage(Title);

            WriteLiteral("<div class=\"row\">\r\n");
            WriteLiteral("<div class=\"col-md-3\">\r\n");

            Write(Html.RenderPartial(new CustomSidebarMenu(ManagementSidebarMenu.Items)));

            WriteLiteral("</div>\r\n");
            WriteLiteral("<div class=\"col-md-9\">\r\n");
            WriteLiteral("<h1 class=\"page-header\">\r\n");
            Write(Title);
            WriteLiteral("</h1>\r\n");

            Write("Select the job type you would like enqueue from the menu.");

            WriteLiteral("\r\n</div>\r\n");
            WriteLiteral("\r\n</div>\r\n");
           
        }
    }
}