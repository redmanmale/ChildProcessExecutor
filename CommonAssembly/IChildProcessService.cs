using System.ServiceModel;

namespace Redmanmale.ChildProcessExecutor.Common
{
    [ServiceContract]
    public interface IChildProcessService
    {
        [OperationContract]
        ChildProcessArgs GetConfig();

        [OperationContract]
        void SetResult(string result);
    }
}
