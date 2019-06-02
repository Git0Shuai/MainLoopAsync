using System.Runtime.CompilerServices;

namespace NSMainLoop
{
    [AsyncMethodBuilder(typeof(MTaskMethodBuilder))]
    public class MTask
    {
        public bool IsComplete { get; protected set; } = false;
        private MAwaiter awaiter;

        public MAwaiter GetAwaiter()
        {
            if (awaiter == null)
            {
                awaiter = new MAwaiter();
            }
            return awaiter;
        }

        public MTask Complete()
        {
            if (!IsComplete)
            {
                IsComplete = true;
                GetAwaiter().Complete();
            }
            return this;
        }
    }

    [AsyncMethodBuilder(typeof(MTaskMethodBuilder))]
    public class MDelay : MTask
    {
        public MDelay(uint delayMilliSec)
        {
            MainLoop.Instance.CallAfter(delayMilliSec, OnTimeout);
        }

        private void OnTimeout()
        {
            Complete();
        }
    }

    [AsyncMethodBuilder(typeof(MTaskMethodBuilder<>))]
    public class MTask<TResult> : MTask
    {
        private MAwaiter<TResult> awaiter;

        new public MAwaiter<TResult> GetAwaiter()
        {
            if (awaiter == null)
            {
                awaiter = new MAwaiter<TResult>();
            }
            return awaiter;
        }

        // hide base impl
        new private void Complete() { }

        public MTask<TResult> Complete(TResult result)
        {
            if (!IsComplete)
            {
                IsComplete = true;
                awaiter?.Complete(result);
            }
            return this;
        }
    }
}
