using Autofac;
using Autofac.Extensions.DependencyInjection;
using Hangfire.Dashboard.Management.Extension.Support;
using Hangfire.Dashboard.Management.Infrastructure;
using Hangfire.JobSDK;
using MediatR;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;

namespace Hangfire.Dashboard.Management.Extension
{
    public static class IServiceCollectionExtensions
    {

        public static void AddManagementPages(this IServiceCollection services, IConfiguration config, IHostingEnvironment hostingEnvironment, string jobsRootFolder)
        {
            GlobalLoaderConfiguration.WebRootPath = hostingEnvironment.WebRootPath;
            GlobalLoaderConfiguration.ContentRootPath = hostingEnvironment.ContentRootPath;

            services.LoadInstalledModules(jobsRootFolder, hostingEnvironment);

            foreach (var module in GlobalLoaderConfiguration.Modules)
            {
                JobsHelper.GetAllJobs(module.Assembly);
            }


            services.Build(config, hostingEnvironment);
        }

        public static void AddManagementPages(this IServiceCollection services, IConfiguration config, IHostingEnvironment hostingEnvironment, params Assembly[] assemblies)
        {
            GlobalLoaderConfiguration.WebRootPath = hostingEnvironment.WebRootPath;
            GlobalLoaderConfiguration.ContentRootPath = hostingEnvironment.ContentRootPath;

            services.LoadInstalledModules(hostingEnvironment, assemblies);

            foreach (var module in GlobalLoaderConfiguration.Modules)
            {
                JobsHelper.GetAllJobs(module.Assembly);
            }


            services.Build(config, hostingEnvironment);
        }

        private static IServiceCollection LoadInstalledModules(this IServiceCollection services, IHostingEnvironment hostingEnvironment, Assembly[] assemblies)
        {
            var modules = new List<ModuleInfo>();

            foreach (var assembly in assemblies)
            {
                var fileInfo = new FileInfo(typeof(string).Assembly.Location);
                var module = new ModuleInfo
                {
                    Name = fileInfo.GetNameWithoutExtension(),
                    Assembly = assembly,
                    Path = fileInfo.FullName
                };

                // Register dependency in modules
                var moduleInitializerType =
                    module.Assembly.GetTypes().FirstOrDefault(x => typeof(IModuleInitializer).IsAssignableFrom(x));
                if ((moduleInitializerType != null) && (moduleInitializerType != typeof(IModuleInitializer)))
                {
                    var moduleInitializer = (IModuleInitializer)Activator.CreateInstance(moduleInitializerType);
                    moduleInitializer.Init(services, hostingEnvironment.EnvironmentName);
                }

                modules.Add(module);

            }
            GlobalLoaderConfiguration.Modules = modules;
            return services;
        }

        private static IServiceCollection LoadInstalledModules(this IServiceCollection services, string modulesRootPath, IHostingEnvironment hostingEnvironment)
        {
            var modules = new List<ModuleInfo>();
            var moduleRootFolder = new DirectoryInfo(modulesRootPath);
            var moduleFolders = moduleRootFolder.GetDirectories();

            foreach (var moduleFolder in moduleFolders)
            {
                var binFolder = new DirectoryInfo(Path.Combine(moduleFolder.FullName, "bin"));
                if (!binFolder.Exists)
                {
                    continue;
                }

                var assemblies = Directory
                           .GetFiles(binFolder.FullName, "*.dll", SearchOption.AllDirectories)
                           .Select(AssemblyLoadContext.Default.LoadFromAssemblyPath)
                           .ToList();

                foreach (var assembly in assemblies)
                {
                    if (assembly.FullName.Contains(moduleFolder.Name))
                    {
                        var module = new ModuleInfo
                        {
                            Name = moduleFolder.Name,
                            Assembly = assembly,
                            Path = moduleFolder.FullName
                        };

                        // Register dependency in modules
                        var moduleInitializerType =
                            module.Assembly.GetTypes().FirstOrDefault(x => typeof(IModuleInitializer).IsAssignableFrom(x));
                        if ((moduleInitializerType != null) && (moduleInitializerType != typeof(IModuleInitializer)))
                        {
                            var moduleInitializer = (IModuleInitializer)Activator.CreateInstance(moduleInitializerType);
                            moduleInitializer.Init(services, hostingEnvironment.EnvironmentName);
                        }

                        modules.Add(module);
                    }
                }

            }

            GlobalLoaderConfiguration.Modules = modules;
            return services;
        }
        private static IServiceProvider Build(this IServiceCollection services, IConfiguration configuration, IHostingEnvironment hostingEnvironment)
        {
            var builder = new ContainerBuilder();


            builder.RegisterAssemblyTypes(typeof(IMediator).GetTypeInfo().Assembly).AsImplementedInterfaces();
            builder.RegisterType<SequentialMediator>().As<IMediator>();
            builder.Register<SingleInstanceFactory>(ctx =>
            {
                var c = ctx.Resolve<IComponentContext>();
                return t => c.Resolve(t);
            });

            builder.Register<MultiInstanceFactory>(ctx =>
            {
                var c = ctx.Resolve<IComponentContext>();
                return t => (IEnumerable<object>)c.Resolve(typeof(IEnumerable<>).MakeGenericType(t));
            });

            foreach (var module in GlobalLoaderConfiguration.Modules)
            {
                builder.RegisterAssemblyTypes(module.Assembly).AsImplementedInterfaces();
            }

            builder.RegisterInstance(configuration);
            builder.RegisterInstance(hostingEnvironment);
            builder.Populate(services);
            var container = builder.Build();
            return container.Resolve<IServiceProvider>();
        }
    }
}
