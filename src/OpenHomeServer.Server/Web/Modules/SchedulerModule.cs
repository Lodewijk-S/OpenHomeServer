using Nancy;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenHomeServer.Server.Web.Modules
{
    public class SchedulerModule : NancyModule
    {
        public SchedulerModule(IScheduler scheduler)
        {
            Get["/"] = x => View["index.cshtml", new
            {
                IsStarted = scheduler.IsStarted,
                Jobs = from j in scheduler.GetCurrentlyExecutingJobs()
                       select new {
                           Name = j.JobDetail.Description,
                           Key = j.JobDetail.Key,
                           State = scheduler.GetTriggerState(j.Trigger.Key)
                       }
            }];
        }        
    }
}
