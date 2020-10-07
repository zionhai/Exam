using Exam.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Exam.DataGenerators
{
    public class ScheduleItemGenerator 
    {
        private const int Amount = 10;
        private const int MaxMinutes = 2; //For the sake of the demonstration
        private static Random Rand = new Random(DateTime.Now.Millisecond);
        private readonly ApplicationDbContext Context;

        public ScheduleItemGenerator(ApplicationDbContext context)
        {
            Context = context;
        }

        public static void Generate(IServiceProvider serviceProvider, int amount = Amount)
        {
            using (var context = new ApplicationDbContext(
                       serviceProvider.GetRequiredService<DbContextOptions<ApplicationDbContext>>()))
            {
                if (context.ScheduleItems.Any())
                {
                    return;
                }
                List<ScheduleItem> items = new List<ScheduleItem>();
                for (int i = 0; i < amount; i++)
                {
                    int randNum = Rand.Next(0, MaxMinutes);
                    items.Add(new ScheduleItem
                    {
                        Name = $"My Event {i + 1}",
                        StartTime = DateTime.UtcNow.AddMinutes(randNum), //Utc because we want to support universal date
                        EndTime = DateTime.UtcNow.AddMinutes(randNum + 5) //For the sake of this demo, an event lasts 5 minutes
                    }); ;
                }
                context.ScheduleItems.AddRange(items);
                context.SaveChanges();
            }
                
        }
    }
}
