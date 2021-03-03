using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Quartz;
using Quartz.Impl;

namespace MyCard.Web
{
    public class JobScheduler
    {
        public static void Start()
        {
            IJobDetail emailJob = JobBuilder.Create<EmailJob>().WithIdentity("job1").Build();

            ITrigger trigger = TriggerBuilder.Create().WithDailyTimeIntervalSchedule
                (s =>
                    s.OnEveryDay()
                    .StartingDailyAt(TimeOfDay.HourAndMinuteOfDay(18, 05))
                    .EndingDailyAfterCount(1)
                )
                //(s =>
                //    s.WithIntervalInSeconds(30)
                //    .OnEveryDay()
                //)
                .ForJob(emailJob)
                .WithIdentity("trigger1")
                .StartNow()
                //.WithCronSchedule("0 0/1 * * * ?")
                .WithCronSchedule("0 05 1 * * ?")
                .Build();

            ISchedulerFactory sf = new StdSchedulerFactory();
            IScheduler sc = sf.GetScheduler();

            sc.ScheduleJob(emailJob, trigger);
            sc.Start();
        }
    }
}