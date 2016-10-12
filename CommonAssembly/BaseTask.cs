using System;

namespace Redmanmale.ChildProcessExecutor.Common
{
    public abstract class BaseTask : MarshalByRefObject
    {
        public abstract void Init(string initParam);

        public abstract string Run(string runParam);
    }
}
