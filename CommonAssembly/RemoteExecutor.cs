using System;
using System.Diagnostics;
using System.IO;
using System.ServiceModel;

namespace Redmanmale.ChildProcessExecutor.Common
{
    public sealed class RemoteExecutor
    {
        private const string PS_EXEC_EXE = "PsExec.exe";
        private const string CHILD_EXEC_EXE = "ChildProcessExecutor.exe";

        private readonly string _executorPath;
        private readonly string _psExecPath;

        public RemoteExecutor(string executorFolder, string psExecPath = null)
        {
            _executorPath = Path.Combine(executorFolder, CHILD_EXEC_EXE);
            _psExecPath = string.IsNullOrWhiteSpace(psExecPath) ? PS_EXEC_EXE : Path.Combine(psExecPath, PS_EXEC_EXE);
        }

        public string ExecuteInSeparateProcess(ChildProcessArgs procArgs)
        {
            var isRemoteExec = !string.IsNullOrEmpty(procArgs.RemoteExecuteParam);

            var pid = Guid.NewGuid();
            var serviceUri = isRemoteExec
                ? $"http://{procArgs.IP}:{procArgs.Port}/ChildProcess_{pid}"
                : $"net.pipe://localhost/ChildProcess_{pid}";

            var childProcessService = new ChildProcessService(procArgs);
            var host = new ServiceHost(childProcessService, new Uri(serviceUri));
            var binding = ChildProcessBindingFactory.GetBinding(isRemoteExec);

            host.AddServiceEndpoint(typeof(IChildProcessService), binding, string.Empty);
            host.Open();

            var procInfo = new ProcessStartInfo
            {
                CreateNoWindow = true,
                UseShellExecute = false
            };

            var procArguments = $"{serviceUri} {isRemoteExec}";
            if (isRemoteExec)
            {
                procInfo.FileName = _psExecPath;
                procInfo.Arguments = $"{procArgs.RemoteExecuteParam} {_executorPath} {procArguments}";
            }
            else
            {
                procInfo.FileName = _executorPath;
                procInfo.Arguments = procArguments;
            }

            var process = new Process {StartInfo = procInfo};
            process.Start();

            process.WaitForExit();
            host.Close();

            switch (process.ExitCode)
            {
                case 0: // OK
                    return childProcessService.Result;
                case 1:
                    throw new Exception($"Child process error: {childProcessService.Result}");
                case 2:
                    throw new Exception("Child process error. For details see child executor log. " +
                                        $"Could not communicate with parent process service: {childProcessService.Result}");
                case 3:
                    throw new Exception("Child process error. For details see child executor log. " +
                                        $"Timeout during sending exception to parent process service: {childProcessService.Result}");
                default:
                    throw new Exception("Child process error. For details see child executor log. " +
                                        $"Unknown exception during sending error to parent process service: {childProcessService.Result}");
            }
        }
    }
}
