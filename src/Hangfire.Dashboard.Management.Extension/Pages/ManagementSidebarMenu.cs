﻿using System;
using System.Collections.Generic;

namespace Hangfire.Dashboard.Management.Extension.Pages
{
    public static class ManagementSidebarMenu
    {
        public static List<Func<RazorPage, MenuItem>> Items = new List<Func<RazorPage, MenuItem>>();
    }
}