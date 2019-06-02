using System;

namespace NSMainLoop
{
    internal class MTimerAction
    {
        public MTimerAction(ulong uid, ulong timeOutFrame, Action onTimeOut)
        {
            Uid = uid;
            TimeOutFrame = timeOutFrame;
            OnTimeOut = onTimeOut;
        }

        public ulong Uid { get; private set; }
        public Action OnTimeOut { get; private set; }
        public ulong TimeOutFrame { get; private set; }
    }
}
