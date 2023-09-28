using flights.application.Services;
using Quartz;
using Quartz.Impl;

namespace flights.application.Schedules
{
    public class Schedules
    {
        public static void Start()
        {
            ISchedulerFactory schedFact = new StdSchedulerFactory();

            IScheduler scheduler = schedFact.GetScheduler().GetAwaiter().GetResult();
            scheduler.Start();

            IJobDetail consulta = JobBuilder.Create<JobAvailabilityService>().Build();


            ITrigger dayle = TriggerBuilder.Create()
              //  .WithDailyTimeIntervalSchedule
              //(s =>
              //   s.WithIntervalInHours(1)
              //  .OnEveryDay()
              //  .StartingDailyAt(TimeOfDay.HourAndMinuteOfDay(10, 35))
              //)
              .WithSchedule(CronScheduleBuilder.CronSchedule("0 0/1 * * * ?"))
            .Build();

            scheduler.ScheduleJob(consulta, dayle);
        }
    }
}
