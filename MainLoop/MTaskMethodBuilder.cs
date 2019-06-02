using System;
using System.Runtime.CompilerServices;

namespace NSMainLoop
{
    public sealed class MTaskMethodBuilder<TResult>
    {

        private MTaskMethodBuilder()
        {
            Task = new MTask<TResult>();
        }

        public static MTaskMethodBuilder<TResult> Create()
        {
            return new MTaskMethodBuilder<TResult>();
        }

        public void SetStateMachine(IAsyncStateMachine stateMachine)
        {
        }

        public void SetException(Exception exception)
        {
            //TODO: Exception
        }

        public void SetResult(TResult result)
        {
            Task.Complete(result);
        }

        public void Start<TStateMachine>(ref TStateMachine stateMachine)
        where TStateMachine : IAsyncStateMachine
        {
            stateMachine.MoveNext();
        }

        public void AwaitOnCompleted<TAwaiter, TStateMachine>(ref TAwaiter awaiter, ref TStateMachine stateMachine)
            where TAwaiter : ICriticalNotifyCompletion where TStateMachine : IAsyncStateMachine
        {
            AwaitUnsafeOnCompleted(ref awaiter, ref stateMachine);
        }

        public void AwaitUnsafeOnCompleted<TAwaiter, TStateMachine>(ref TAwaiter awaiter, ref TStateMachine stateMachine)
            where TAwaiter : ICriticalNotifyCompletion where TStateMachine : IAsyncStateMachine
        {
            awaiter.UnsafeOnCompleted(stateMachine.MoveNext);
        }

        public MTask<TResult> Task { get; private set; }

    }


    public sealed class MTaskMethodBuilder
    {
        private MTaskMethodBuilder()
        {
            Task = new MTask();
        }

        public static MTaskMethodBuilder Create()
        {
            return new MTaskMethodBuilder();
        }

        public void SetStateMachine(IAsyncStateMachine stateMachine)
        {
        }

        public void SetException(Exception exception)
        {
            //TODO: Exception
        }

        public void SetResult()
        {
            Task.Complete();
        }

        public void Start<TStateMachine>(ref TStateMachine stateMachine) 
        where TStateMachine: IAsyncStateMachine
        {
            stateMachine.MoveNext();
        }

        public void AwaitOnCompleted<TAwaiter, TStateMachine>(ref TAwaiter awaiter, ref TStateMachine stateMachine)
            where TAwaiter: ICriticalNotifyCompletion
            where TStateMachine : IAsyncStateMachine
        {
            AwaitUnsafeOnCompleted(ref awaiter, ref stateMachine);
        }

        public void AwaitUnsafeOnCompleted<TAwaiter, TStateMachine>(ref TAwaiter awaiter, ref TStateMachine stateMachine)
            where TAwaiter : ICriticalNotifyCompletion
            where TStateMachine : IAsyncStateMachine
        {
            awaiter.UnsafeOnCompleted(stateMachine.MoveNext);
        }

        public MTask Task { get; private set; }
    }
}
