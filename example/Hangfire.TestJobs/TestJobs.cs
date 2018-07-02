using Hangfire;
using Hangfire.JobSDK;
using Hangfire.Server;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.ComponentModel;
using System.Threading;

namespace Hangfire.TestJobs
{
    [ManagementPage("Test Jobs", "Test Jobs")]
    public class TestJobs : IJob,IModuleInitializer
    {
        public void Init(IServiceCollection serviceCollection,string environmentName)
        {
            //Add services required by job
        }

        [DisplayName("Test")]
        [Description("Test that jobs are running with simple console output.")]
        [AutomaticRetry(Attempts = 0)]
        [DisableConcurrentExecution(90)]
        public void Test(PerformContext context, IJobCancellationToken token,
                [DisplayData("Output Text", "Enter text to output.")] string outputText,
                [DisplayData("Repeat When Completed", "Repeat")] bool repeat,
                [DisplayData("Test Date", "Enter date")] DateTime testDate)

        {
           // context.WriteLine(outputText);
            Thread.Sleep(15000);

            token.ThrowIfCancellationRequested();

            if (repeat)
            {
             //   context.WriteLine("Enquing the job again from the job.");
                BackgroundJob.Enqueue<TestJobs>(m => m.Test(context, token, outputText, repeat,testDate));
            }
        }

        [DisplayName("Test2")]
        [Description("Test that jobs are running with simple console output.")]
        [AutomaticRetry(Attempts = 0)]
        [DisableConcurrentExecution(90)]
        public void Test2(PerformContext context, IJobCancellationToken token)
        {
            // context.WriteLine(outputText);
            
            Thread.Sleep(15000);

            token.ThrowIfCancellationRequested();

        }
    }
}
