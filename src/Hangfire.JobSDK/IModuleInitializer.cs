using Microsoft.Extensions.DependencyInjection;

namespace Hangfire.JobSDK
{
    public interface IModuleInitializer
    {
        void Init(IServiceCollection serviceCollection,string environmentName);
    }
}
