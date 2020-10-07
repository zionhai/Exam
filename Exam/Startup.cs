using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Exam.DataGenerators;
using Exam.Hubs;
using Exam.Jobs;
using Hangfire;
using Hangfire.MemoryStorage;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Exam
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddDbContext<ApplicationDbContext>(options =>
                            options.UseInMemoryDatabase(databaseName: "Schedule"));

            services.AddSignalR(options =>
            {
                options.KeepAliveInterval = TimeSpan.FromMinutes(1);
                options.ClientTimeoutInterval = TimeSpan.FromMinutes(1);
                options.EnableDetailedErrors = true;

            });

            services.AddHangfire(config =>
            config.SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
                  .UseSimpleAssemblyNameTypeSerializer()
                  .UseDefaultTypeSerializer()
                  .UseMemoryStorage());
         

            services.AddHangfireServer();

            services.AddScoped<IAgentJob, HandleScheduleItemsJob>();

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, 
                              IWebHostEnvironment env, 
                              IRecurringJobManager agent,
                              IServiceProvider provider)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseAuthorization();

            app.UseHangfireDashboard();
            ConfigureJobs(agent, provider);

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHub<ScheduleHub>("schedule");
            });

            
            
        }

        private void ConfigureJobs(IRecurringJobManager agent, IServiceProvider provider)
        {
            var ser = provider.GetService<IAgentJob>();
            var jobs = provider.GetServices<IAgentJob>();
            foreach (var job in jobs)
            {
                agent.AddOrUpdate($"{job.GetType()}", () => job.Run(), job.Time);
            }

        }
    }
}
