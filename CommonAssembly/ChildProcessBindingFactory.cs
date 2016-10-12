using System;
using System.ServiceModel;
using System.ServiceModel.Channels;

namespace Redmanmale.ChildProcessExecutor.Common
{
    public static class ChildProcessBindingFactory
    {
        private const int MAX_MESSAGE_SIZE = 2147483647;

        public static Binding GetBinding(bool isRemote)
        {
            Binding binding;
            if (isRemote)
            {
                binding = new BasicHttpBinding
                {
                    OpenTimeout = TimeSpan.MaxValue,
                    CloseTimeout = TimeSpan.MaxValue,
                    SendTimeout = TimeSpan.MaxValue,
                    ReceiveTimeout = TimeSpan.MaxValue,
                    ReaderQuotas = {MaxStringContentLength = MAX_MESSAGE_SIZE, MaxBytesPerRead = MAX_MESSAGE_SIZE, MaxArrayLength = MAX_MESSAGE_SIZE},
                    MaxBufferSize = MAX_MESSAGE_SIZE,
                    MaxReceivedMessageSize = MAX_MESSAGE_SIZE,
                    Security = new BasicHttpSecurity
                    {
                        Mode = BasicHttpSecurityMode.None
                    }
                };
            }
            else
            {
                binding = new NetNamedPipeBinding
                {
                    OpenTimeout = TimeSpan.MaxValue,
                    CloseTimeout = TimeSpan.MaxValue,
                    SendTimeout = TimeSpan.MaxValue,
                    ReceiveTimeout = TimeSpan.MaxValue,
                    ReaderQuotas = {MaxStringContentLength = MAX_MESSAGE_SIZE, MaxBytesPerRead = MAX_MESSAGE_SIZE, MaxArrayLength = MAX_MESSAGE_SIZE},
                    MaxBufferSize = MAX_MESSAGE_SIZE,
                    MaxReceivedMessageSize = MAX_MESSAGE_SIZE
                };
            }
            return binding;
        }
    }
}
