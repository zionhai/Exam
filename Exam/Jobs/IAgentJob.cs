using Hangfire;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Exam.Jobs
{
    public interface IAgentJob
    {
        string Time { get; set; }

        [DisableConcurrentExecution(10 * 60)]
   
        Task Run();
    }
}
