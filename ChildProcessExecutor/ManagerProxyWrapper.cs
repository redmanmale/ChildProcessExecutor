using System;
using System.ServiceModel;
using Redmanmale.ChildProcessExecutor.Common;

namespace Redmanmale.ChildProcessExecutor
{
    internal class ManagerProxyWrapper : IDisposable
    {
        private readonly ChannelFactory<IChildProcessService> _channelFactory;

        public ManagerProxyWrapper(string uri, bool isRemote)
        {
            var binding = ChildProcessBindingFactory.GetBinding(isRemote);
            _channelFactory = new ChannelFactory<IChildProcessService>(binding, uri);
        }

        public ChildProcessArgs GetConfiguration()
        {
            return ExecRemoteMethod(x => x.GetConfig());
        }

        public void SendResult(string result)
        {
            ExecRemoteMethod(x => x.SetResult(result));
        }

        public void Dispose()
        {
            ((IDisposable) _channelFactory)?.Dispose();
        }

        private T ExecRemoteMethod<T>(Func<IChildProcessService, T> action)
        {
            T remoteMethodResult;
            var proxy = _channelFactory.CreateChannel();
            var communicationProxy = (ICommunicationObject) proxy;
            try
            {
                communicationProxy.Open();
                remoteMethodResult = action(proxy);
            }
            catch (Exception e)
            {
                throw new Exception("Operation failed. Unknown error.", e);
            }
            finally
            {
                if (communicationProxy.State == CommunicationState.Opened)
                {
                    communicationProxy.Close();
                }
                else
                {
                    communicationProxy.Abort();
                }
            }
            return remoteMethodResult;
        }

        private void ExecRemoteMethod(Action<IChildProcessService> action)
        {
            var proxy = _channelFactory.CreateChannel();
            var communicationProxy = (ICommunicationObject) proxy;
            try
            {
                communicationProxy.Open();
                action(proxy);
            }
            catch (Exception e)
            {
                throw new Exception("Operation failed. Unknown error.", e);
            }
            finally
            {
                if (communicationProxy.State == CommunicationState.Opened)
                {
                    communicationProxy.Close();
                }
                else
                {
                    communicationProxy.Abort();
                }
            }
        }
    }
}
