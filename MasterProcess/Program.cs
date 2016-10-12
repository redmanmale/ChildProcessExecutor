using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Redmanmale.ChildProcessExecutor.Common;
using Redmanmale.ChildProcessExecutor.Task;

namespace Redmanmale.ChildProcessExecutor.MasterProcess
{
    internal static class Program
    {
        internal static void Main(string[] args)
        {
            try
            {
                var argsDict = ParseArgs(args);

                if (!argsDict.Any())
                {
                    argsDict.Add("-taskfolder", Path.GetFullPath(@"..\..\..\TaskAssembly\bin\Debug"));
                    argsDict.Add("-executorfolder", Path.GetFullPath(@"..\..\..\ChildProcessExecutor\bin\Debug"));
                    argsDict.Add("-runparam", "world!");
                    argsDict.Add("-intiparam", "hello");
                }

                var childParams = createChildProcessArgs(typeof(SimpleTask), argsDict);
                var exec = new RemoteExecutor(childParams.ExecutorFolder);

                var result = exec.ExecuteInSeparateProcess(childParams);

                Console.WriteLine("===================");
                Console.Write("Result: ");
                Console.WriteLine(result);
            }
            catch (Exception exc)
            {
                Console.WriteLine(exc);
            }
            Console.ReadKey();
        }

        private static Dictionary<string, string> ParseArgs(IEnumerable<string> args)
        {
            var argDict = new Dictionary<string, string>();

            foreach (var arg in args)
            {
                var argKeyValue = arg.Split('=');
                if (argKeyValue.Length != 2)
                {
                    throw new KeyNotFoundException($"Wrong param: {arg}");
                }

                argDict[argKeyValue[0].ToLowerInvariant()] = argKeyValue[1];
            }

            return argDict;
        }

        private static ChildProcessArgs createChildProcessArgs(Type taskType, IReadOnlyDictionary<string, string> args)
        {
            var remoteConfig = new RemoteProcessConfig();

            var isRemote = false;
            var initParam = string.Empty;
            var runParam = string.Empty;
            var taskFolder = string.Empty;
            var executorFolder = string.Empty;

            foreach (var arg in args)
            {
                switch (arg.Key)
                {
                    case "-initparam":
                        initParam = arg.Value;
                        break;
                    case "-runparam":
                        runParam = arg.Value;
                        break;
                    case "-taskfolder":
                        taskFolder = Path.GetFullPath(arg.Value);
                        break;
                    case "-masterip":
                        remoteConfig.MasterIP = arg.Value;
                        break;
                    case "-masterport":
                        remoteConfig.MasterPort = int.Parse(arg.Value);
                        break;
                    case "-executorfolder":
                        executorFolder = arg.Value;
                        break;
                    case "-remotedomain":
                        remoteConfig.RemoteDomain = arg.Value;
                        break;
                    case "-remotemachine":
                        isRemote = true;
                        remoteConfig.RemoteIP = arg.Value;
                        break;
                    case "-remoteuser":
                        remoteConfig.RemoteUser = arg.Value;
                        break;
                    case "-remotepassword":
                        remoteConfig.RemotePassword = arg.Value;
                        break;
                }
            }

            var childParams = isRemote ? new ChildProcessArgs(taskType, remoteConfig) : new ChildProcessArgs(taskType);
            childParams.InitParam = initParam;
            childParams.RunParam = runParam;
            childParams.TaskFolder = taskFolder;
            childParams.ExecutorFolder = executorFolder;

            return childParams;
        }
    }
}
