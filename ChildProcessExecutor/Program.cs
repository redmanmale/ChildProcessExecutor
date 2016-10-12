using System;
using System.IO;
using System.Reflection;
using NLog;
using Redmanmale.ChildProcessExecutor.Common;

namespace Redmanmale.ChildProcessExecutor
{
    public static class Program
    {
        private static readonly ILogger _logger = LogManager.GetCurrentClassLogger();

        private static string _taskFolder;

        public static void Main(string[] args)
        {
            _logger.Info("Child starting");

            if (args.Length != 2)
            {
                _logger.Info("Usage: serviceUrl isRemote");
                return;
            }

            ManagerProxyWrapper service = null;
            try
            {
                _logger.Debug("CMD params: {0} {1}", args[0], args[1]);
                _logger.Debug("Parse bool params: {0}", bool.Parse(args[1]));

                service = new ManagerProxyWrapper(args[0], bool.Parse(args[1]));

                _logger.Debug("Requesting config");
                var config = service.GetConfiguration();

                _logger.Debug("Config loaded");
                _taskFolder = config.TaskFolder;

                _logger.Debug("Task folder {0}", _taskFolder);

                AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;

                _logger.Debug("Creating child instance");
                var taskInstance = AppDomain.CurrentDomain.CreateInstanceAndUnwrap(config.AssemblyFullName, config.ClassType) as BaseTask;
                if (taskInstance == null)
                {
                    throw new Exception($"Assembly has no derived classes from {typeof(BaseTask)}");
                }

                _logger.Debug("Initing child");
                taskInstance.Init(config.InitParam);

                _logger.Debug("Running child");
                var result = taskInstance.Run(config.RunParam);

                _logger.Debug("Sending results");
                service.SendResult(result);
            }
            catch (Exception exc)
            {
                _logger.Error(exc);

                if (service != null)
                {
                    try
                    {
                        service.SendResult(exc.ToString());

                        Environment.ExitCode = 1;
                        _logger.Error("Error child process execution, exit code = {0}", Environment.ExitCode);
                    }
                    catch (TimeoutException ex)
                    {
                        Environment.ExitCode = 3;
                        _logger.Error(ex, "Timeout during sending error to parent process service, exit code = {0}", Environment.ExitCode);
                    }
                    catch (Exception ex)
                    {
                        Environment.ExitCode = 4;
                        _logger.Error(ex, "Unknown exception during sending error to parent process service, exit code = {0}", Environment.ExitCode);
                    }
                }
                else
                {
                    Environment.ExitCode = 2;
                    _logger.Error("Could not communicate with parent process service, exit code = {0}", Environment.ExitCode);
                }
            }
        }

        private static Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            var assemblyNameParts = args.Name.Split(',');
            if (assemblyNameParts.Length < 0)
            {
                throw new Exception($"Could not find assembly: {args.Name} in folder: {_taskFolder}");
            }
            var assemblyPath = Path.Combine(_taskFolder, assemblyNameParts[0] + ".dll");
            var assembly = Assembly.LoadFile(assemblyPath);
            return assembly;
        }
    }
}
