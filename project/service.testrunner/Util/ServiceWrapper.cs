using ThoughtWorks.CruiseControl.Service;

namespace service.testrunner.Util
{
    public class ServiceWrapper
    {
        ServiceSkeleton service = new ServiceSkeleton();
        public void StartService()
        {
            service.OnStart();
        }

        public void StopService()
        {
            service.OnStop();
        }

        public void PauseService()
        {
            service.OnPause();
        }

        public void ResumeService()
        {
            service.OnContinue();
        }

        public bool CanStop => true;
    }
}
