using Hangfire.Dashboard.Management.Extension.Pages;
using Hangfire.Dashboard.Management.Extension.Support;
using Microsoft.AspNetCore.Builder;
using System;
using System.Linq;
using System.Reflection;

namespace Hangfire.Dashboard.Management.Extension
{
    public static class GlobalConfigurationExtension
    {
        public static void UseManagementPages(this IApplicationBuilder config)
        {
            CreateManagement();
        }

        private static void CreateManagement()
        {
            foreach (var pageInfo in JobsHelper.Pages)
            {
                ManagementBasePage.AddCommands(pageInfo.Category);

                ManagementSidebarMenu.Items.Add(p => new MenuItem(pageInfo.MenuName, p.Url.To($"{ManagementPage.UrlRoute}/{pageInfo.Category}"))
                {
                    Active = p.RequestPath.StartsWith($"{ManagementPage.UrlRoute}/{pageInfo.Category}")
                });

                DashboardRoutes.Routes.AddRazorPage($"{ManagementPage.UrlRoute}/{pageInfo.Category}", x => new ManagementBasePage(pageInfo.MenuName, pageInfo.Title, pageInfo.Category, pageInfo.Queue));
            }

            //note: have to use new here as the pages are dispatched and created each time. If we use an instance, the page gets duplicated on each call
            DashboardRoutes.Routes.AddRazorPage(ManagementPage.UrlRoute, x => new ManagementPage());

            // can't use the method of Hangfire.Console as it's usage overrides any similar usage here. Thus
            // we have to add our own endpoint to load it and call it from our code. Actually is a lot less work
            DashboardRoutes.Routes.Add("/jsm", new EmbeddedResourceDispatcher(Assembly.GetExecutingAssembly(), "Hangfire.Dashboard.Management.Extension.Content.management.js"));

            NavigationMenu.Items.Add(page => new MenuItem(ManagementPage.Title, page.Url.To(ManagementPage.UrlRoute))
            {
                Active = page.RequestPath.StartsWith(ManagementPage.UrlRoute)
            });

        }

    }
}
