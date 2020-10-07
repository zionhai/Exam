using Exam.Hubs;
using Exam.Models;
using Hangfire;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Exam.Jobs
{
    public class HandleScheduleItemsJob : IAgentJob
    {
        public string Time { get; set; }
        private readonly ApplicationDbContext Context;
        private readonly IHubContext<ScheduleHub> HubContext;
        public HandleScheduleItemsJob(ApplicationDbContext context, IHubContext<ScheduleHub> hubContext)
        {
            Time = "*/10 * * * * *";
            Context = context;
            HubContext = hubContext;
        }
        [DisableConcurrentExecution(10 * 60)]
        public async Task Run()
        {
            try
            {
                var items = await Context.ScheduleItems
                                     .Where(i => i.StartTime <= DateTime.UtcNow)
                                     .Where(i => i.WasPosted == false)
                                     .ToListAsync();
                foreach (var item in items)
                {
                    //Need to implement concurring execution, current Hangfire attribute is buggy.
                    await PostToClient(item);

                    item.WasPosted = true;
                    Context.Entry<ScheduleItem>(item).State = EntityState.Modified;
                    await Context.SaveChangesAsync();
           
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
            
            
        }

        private async Task PostToClient(ScheduleItem item)
        {

            await HubContext.Clients.All.SendAsync("receiveScheduleItem", item);
            Console.WriteLine(item.Id);
            Console.WriteLine(item.Name);
            Console.WriteLine(item.StartTime);
            Console.WriteLine(item.EndTime);
        }
    }
}
