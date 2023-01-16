using System;
using System.Threading;
using CoreData.Infrastructure;
using CoreSvc.Common;
using Microsoft.Extensions.Hosting;
using Serilog;
using CoreDataHelper = CoreData.Common.Helper;

namespace IdentityServer
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Console.Title = "IdentityServer4";

            //For Redis to work.    
            ThreadPool.SetMinThreads(50, 50);

            LogManager.Initialize();

            var host = CreateHostBuilder(args).Build();

            CoreDataHelper.RegisterDelegates();

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