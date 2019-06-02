using System;
using System.Runtime.CompilerServices;

namespace NSMainLoop
{
    public class MAwaiter : ICriticalNotifyCompletion
    {
        private Action next;
        public bool IsCompleted { get; private set; } = false;

        public void Complete()
        {
            if (!IsCompleted)
            {
                IsCompleted = true;
                next?.Invoke();
            }
        }

        public void GetResult() { }

        public void OnCompleted(Action continuation)
        {
            UnsafeOnCompleted(continuation);
        }

        public void UnsafeOnCompleted(Action continuation)
        {
            next = continuation;
        }
    }

    public class MAwaiter<TResult> : ICriticalNotifyCompletion
    {
        private Action next;
        private TResult result;
        public bool IsCompleted { get; private set; } = false;

        public void Complete(TResult result)
        {
            if (!IsCompleted)
            {
                IsCompleted = true;
                this.result = result;
                next?.Invoke();
            }
        }

        public TResult GetResult() => result;

        public void OnCompleted(Action continuation)
        {
            UnsafeOnCompleted(continuation);
        }

        public void UnsafeOnCompleted(Action continuation)
        {
            next = continuation;
        }
    }
    
}
