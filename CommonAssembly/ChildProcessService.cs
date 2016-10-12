using System.ServiceModel;

namespace Redmanmale.ChildProcessExecutor.Common
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Multiple, IncludeExceptionDetailInFaults = true)]
    public class ChildProcessService : IChildProcessService
    {
        private readonly ChildProcessArgs _config;

        public string Result { get; private set; }

        public ChildProcessService(ChildProcessArgs config)
        {
            _config = config;
        }

        public ChildProcessArgs GetConfig()
        {
            return _config;
        }

        public void SetResult(string result)
        {
            Result = result;
        }
    }
}
