using System.Threading.Tasks;

namespace Business.Hangfire.Managers.FireJobs
{
    public class SampleFireJobManager : IScheduleJobManager
    {
        public SampleFireJobManager()
        {

        }

        public Task Process(int id)
        {
            return Task.FromResult(id);
        }
    }
}
