using System;
using System.Collections.Generic;
using System.Linq;
using CoreData;
using CoreData.CacheManager;
using CoreSvc.Common;
using CoreSvc.Filters;
using CoreType.Types;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StackExchange.Redis;

namespace Scheduler.API.Controllers
{
    // Samples
    // Fire-and-Forget
    // BackgroundJob.Enqueue(() => Console.WriteLine("Fire-and-forget"));

    // Delayed
    // BackgroundJob.Schedule(() => Console.WriteLine("2 mins Delayed Task"), TimeSpan.FromMinutes(2));

    // Recurring
    // RecurringJob.AddOrUpdate(() => Console.WriteLine("Minutely Job"), Cron.Minutely);

    [Authorize]
    [DefaultRoute]
    public class HangfireTasksController : BaseController
    {
        [HttpPost]
        public ResponseWrapper<bool> IsJobSucceeded([FromBody] string jobName)
        {
            var genericResponse = new ResponseWrapper<bool>();

            var jobId = DistributedCache.Database.HashGet($"{{hangfire}}:recurring-job:{jobName}", "LastJobId");

            if (jobId.IsNullOrEmpty)
                genericResponse.Data = false;
            else
            {
                var isFailed = DistributedCache.Database.SortedSetScan("{hangfire}:failed")
                    .Any(x => x.Element.CompareTo(jobId) == 0);

                genericResponse.Data = !isFailed;
            }

            genericResponse.Message = LocalizedMessages.PROCESS_SUCCESSFUL;
            genericResponse.Success = true;

            return genericResponse;
        }

        [HttpPost]
        public ResponseWrapper<List<JobStatus>> ListJobStatuses()
        {
            var genericResponse = new ResponseWrapper<List<JobStatus>>();
            List<JobStatus> jobList = new List<JobStatus>();

            var jobNames = DistributedCache.Database.SortedSetScan("{hangfire}:recurring-jobs")
                .Select(x => x.Element.ToString())
                .ToList();

            var failedJobs = DistributedCache.Database.SortedSetScan("{hangfire}:failed")
                .ToList();

            foreach (var jobName in jobNames)
            {
                var jobDict = DistributedCache.Database.HashGetAll($"{{hangfire}}:recurring-job:{jobName}").ToStringDictionary();
                var isFailed = failedJobs.Any(x => x.Element.ToString().Equals(jobDict["LastJobId"]));

                jobList.Add(new JobStatus
                {
                    JobId = jobDict["LastJobId"],
                    JobName = jobName,
                    LastExecution = !string.IsNullOrEmpty(jobDict["LastExecution"]) ? Convert.ToDateTime(jobDict["LastExecution"]) : default(DateTime?),
                    NextExecution = !string.IsNullOrEmpty(jobDict["NextExecution"]) ? Convert.ToDateTime(jobDict["NextExecution"]) : default(DateTime?),
                    Status = !isFailed
                });
            }

            genericResponse.Data = jobList;
            genericResponse.Message = LocalizedMessages.PROCESS_SUCCESSFUL;
            genericResponse.Success = true;

            return genericResponse;
        }

        /*[HttpPost]
        public void HangFireAddRecurring3(string param3)
        {
            Console.WriteLine("HangFireTask3");

            var result = HangfireJobClient.AddRecurringJob("https://localhost:5001/hangfire/httpjob", new RecurringJob()
            {
                JobName = param3,

                Method = "Post",
                Data = new { param1 = param3 },
                Url = "https://localhost:5001/HangfireTasks/HangFireAddRecurring1",
                Mail = new List<string> { "info@netcoregenesis.com" },
                SendSucMail = true,
                Cron = "0/60 0 0 ? * * *"
            }, new HangfireServerPostOption
            {
                BasicUserName = "admin",
                BasicPassword = "test"
            });
        }*/
    }
}