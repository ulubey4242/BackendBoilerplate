using Business.Hangfire.Managers.RecurringJobs;
using Hangfire;
using System;

namespace HangfireJobs
{
    public static class RecurringJobs
    {
        public static void FetchUserOperation()
        {
            RecurringJob.RemoveIfExists(nameof(FetchUserScheduleJobManager));

            RecurringJob.AddOrUpdate<FetchUserScheduleJobManager>(nameof(FetchUserScheduleJobManager),
                job => job.Process(), "59 23 * * *", TimeZoneInfo.Local);
        }
    }
}
