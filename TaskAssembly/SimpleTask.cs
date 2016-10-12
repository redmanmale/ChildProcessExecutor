using Redmanmale.ChildProcessExecutor.Common;

namespace Redmanmale.ChildProcessExecutor.Task
{
    public class SimpleTask : BaseTask
    {
        private string _initParam;

        public override void Init(string initParam)
        {
            _initParam = $"Init={initParam}";
        }

        public override string Run(string runParam)
        {
            return _initParam + $" RunParam={runParam}";
        }
    }
}
