using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GDS_SERVER_WPF.DataCLasses
{
    public class SemaphoresForTask
    {
        public Semaphore semaphoreSynFlag = new Semaphore(1, 1);
        public Semaphore semaphoreSynFlagWinpe = new Semaphore(1, 1);
        public Semaphore semaphoreForTask = new Semaphore(1, 1);
        public List<Semaphore> semaphores = new List<Semaphore>();

        public SemaphoresForTask()
        {
            semaphoreSynFlag.WaitOne();
            semaphoreSynFlagWinpe.WaitOne();
            semaphoreForTask.WaitOne();            
            semaphores.Add(semaphoreSynFlag);
            semaphores.Add(semaphoreSynFlagWinpe);
            semaphores.Add(semaphoreForTask);
        }
    }
}
