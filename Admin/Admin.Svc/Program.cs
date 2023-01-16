using System;
using System.Threading;
using System.Threading.Tasks;
using CoreData.Common;
using CoreData.Infrastructure;
using CoreData.Infrastructure.Consumers;
using CoreSvc.Common;
using Microsoft.Extensions.Hosting;
using Serilog;
using CoreSvcHelper = CoreSvc.Common.Helper;
using CoreDataHelper = CoreData.Common.Helper;
using CoreSvcConstants = CoreSvc.Constants;

namespace Admin.Svc
{
    public class Program
    {
        public static void Main(string[] args)
        {
            //For Redis to work.    
            ThreadPool.SetMinThreads(50, 50);

            LogManager.Initialize();

            var host = CreateHostBuilder(args).Build();

            #region Consumers

            Task.Run(new LogConsumer().Listen);
            Task.Run(new CommMailConsumer().Listen);
            Task.Run(new CommSMSConsumer().Listen);

            #endregion Consumers

            CoreDataHelper.RegisterDelegates();
            CoreSvcHelper.CacheServiceDefinitions(includedAssemblies: CoreSvcConstants.PROJECT_NAME);

            try
            {
                host.Run();
            }
            catch (Exception e)
            {
                Log.Fatal(e, "Host terminated unexpectedly");
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureLogging()
                .ConfigureWebHost<Startup>();
    }
}