# Hangfire.Core.Dashboard.Management

![MIT License](https://img.shields.io/badge/license-MIT-orange.svg)

Hangfire.Core.Dashboard.Management provides a Management page in the default dashboard. It allows for manually creating jobs.

**Based on**: [Hangfire.Core.Dashboard.Management](https://github.com/timohayes/Hangfire.Core.Dashboard.Management)


![management](management.PNG)

## Features
 - **.NET Standard 2.0**
 - **Dependency Injection**: supports Microsoft.Extensions.DependencyInjection. Jobs can register and use services.
 - **Load jobs from folder**: You can load jobs from a folder (without referencing them in host). Extremely fragile without System.Addin :)
 - **Automatic page and menu generation**: Simple attributes on your job classes define management pages. 
 - **Automatic input generation**: Simple attributes on your properties allows for auto generation of input fields. (bool, int, text, DateTime)
 - **Support for IJobCancellationToken and PerformContext **: These job properties are automatically ignored and set null on job creation.
 - **Simple Fire-and-Forget**: Directly from your Management dashboard you can fire any Job.
 - **Set a Cron Job**: Define your cron and set it for any Job.
 - **Schedule a Job**: Schedule your job to run in the future. (Currently 5, 10, 15, 30 and 60 min intervals)
 - **Extensable**: Use the framework to add your own additional pages. (takes so digging to figure this out)

## Setup

```c#
public void ConfigureServices(IServiceCollection services)
{
...
	services.AddManagementPages(_configuration,_hostingEnvironment, Path.Combine(_hostingEnvironment.ContentRootPath, "Jobs"));  
	
	OR

	services.AddManagementPages(_configuration, _hostingEnvironment, typeof(Hangfire.TestJobs.TestJobs).Assembly);
...
}


public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
{
...
	app.UseHangfireServer();

 	app.UseHangfireDashboard();

	app.UseManagementPages();
...
}
```


## Defining Pages

Pages are defined and based on your Job classes. A Job class needs to implement IJob. The class should also have the attribute 
ManagementPage defined. Everything within this class will be on it's own page and have a navigation item in the side menu.

Each function within the class is defined as a specific job. The function should be decorated with DisplayName and Description. 
Displayname will be in the header of the panel and description is part of the panel body.

Each input property, other than IJobCancellationToken and PerformContext, should be decorated with the DisplayData attribute. This
defines the input label and placeholder text for better readability. 

```c#
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
		context.WriteLine(outputText);
		Thread.Sleep(15000);
				
		token.ThrowIfCancellationRequested();

		if (repeat)
		{
			context.WriteLine("Enquing the job again from the job.");
			BackgroundJob.Enqueue<MiscJobs>(m => m.Test(context, token, outputText, repeat));
		}
	}
}
```

## Caution
Things might not work as expected and could just not work. There has only been manual testing so far. If attributes are missing I'm not
sure what will happen.

## License

Copyright (c) 2017

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
