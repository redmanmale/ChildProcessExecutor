using System;
using System.Runtime.Serialization;

namespace Redmanmale.ChildProcessExecutor.Common
{
    [DataContract]
    public class ChildProcessArgs
    {
        public ChildProcessArgs() {}

        public ChildProcessArgs(Type type, RemoteProcessConfig remoteConfig = null)
        {
            AssemblyFullName = type.Assembly.FullName;
            ClassType = type.FullName;

            if (remoteConfig != null)
            {
                IP = remoteConfig.MasterIP;
                Port = remoteConfig.MasterPort;

                // empty space or domain name WITH '\'
                var domain = string.IsNullOrWhiteSpace(remoteConfig.RemoteDomain)
                    ? string.Empty
                    : remoteConfig.RemoteDomain.EndsWith("\\")
                        ? remoteConfig.RemoteDomain
                        : remoteConfig.RemoteDomain + '\\';

                // \\<MACHINENAME> -u [<DOMAIN>\]<LOGIN> -p <PASSWORD>
                RemoteExecuteParam = $"\\\\{remoteConfig.RemoteIP} -u {domain}{remoteConfig.RemoteUser} -p {remoteConfig.RemotePassword}";
            }
        }

        [DataMember]
        public string TaskFolder { get; set; }

        [DataMember]
        public string AssemblyFullName { get; private set; }

        [DataMember]
        public string ClassType { get; private set; }

        [DataMember]
        public string InitParam { get; set; }

        [DataMember]
        public string RunParam { get; set; }

        [IgnoreDataMember]
        public string ExecutorFolder { get; set; }

        [IgnoreDataMember]
        public string IP { get; }

        [IgnoreDataMember]
        public int Port { get; }

        /// <summary>
        /// \\[MACHINENAME] -u[DOMAIN]\[LOGIN] -p[PASSWORD]
        /// </summary>
        [IgnoreDataMember]
        public string RemoteExecuteParam { get; }
    }
}
