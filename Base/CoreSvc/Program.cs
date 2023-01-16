using System.Threading;
using CoreData.Infrastructure;
using CoreSvc.Common;
using Microsoft.Extensions.Hosting;

namespace CoreSvc
{
    public class Program
    {
        public static void Main(string[] args)
        {
            //For Redis to work.    
            ThreadPool.SetMinThreads(50, 50);

            LogManager.Initialize();

            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureLogging()
                .ConfigureWebHost<Startup>();
    }
}