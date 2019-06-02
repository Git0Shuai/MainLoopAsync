using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;

namespace NSMainLoop
{
    /// <summary>
    /// NOT thread safe
    /// </summary>
    public class MainLoop
    {
        private static ConcurrentDictionary<int, MainLoop> threadId2MainLoop = new ConcurrentDictionary<int, MainLoop>();

        public static MainLoop Instance => threadLocalInstance.Value;
        private static ThreadLocal<MainLoop> threadLocalInstance = new ThreadLocal<MainLoop>(() => new MainLoop());

        public readonly int BelongThreadId;

        public uint Frequency { get; private set; }

        public ulong CurrentFrame { get; private set; }

        public State LoopState { get; private set; }

        public bool IsTerminating { get; private set; }

        private ulong lastUid;
        private Dictionary<ulong, MTimerAction> timers = new Dictionary<ulong, MTimerAction>();
        // TODO: need FIFO
        private Dictionary<ulong, Action> asap = new Dictionary<ulong, Action>();

        private bool hasPending => timers.Count != 0 || asap.Count != 0;

        /// <summary>
        /// run loop untill terminated
        /// </summary>
        public void Start(Action startAction, uint driveFrequency = 5)
        {
            if (LoopState != State.STOP)
            {
                throw new Exception();
            }
            LoopState = State.WAITING;
            Frequency = driveFrequency;
            long nextFrameTicks = 0;
            var ticksPerMilliSec = Stopwatch.Frequency / 1000;
            var ticksPerFrame = Frequency * ticksPerMilliSec;

            startAction?.Invoke();

            var sw = new Stopwatch();
            sw.Start();
            while (!IsTerminating && hasPending)
            {
                ++CurrentFrame;

                Drive();

                nextFrameTicks += ticksPerFrame;
                var leftTicks = nextFrameTicks - sw.ElapsedTicks;
                if (leftTicks < 0)
                {
                    continue;
                }
                Thread.Sleep((int)(leftTicks / ticksPerMilliSec));
            }
            sw.Stop();
            IsTerminating = false;
            LoopState = State.TERMINATE;
        }

        /// <summary>
        /// run once
        /// </summary>
        public void Drive()
        {
            if (LoopState != State.WAITING)
            {
                throw new Exception();
            }
            LoopState = State.PROCESSING;

            CheckTimer();
            CheckAsap();

            LoopState = State.WAITING;
        }

        private void CheckTimer()
        {
            var keys = timers.Keys.ToList();
            foreach (var key in keys)
            {
                if (timers.TryGetValue(key, out var timer) && timer.TimeOutFrame <= CurrentFrame)
                {
                    toRemoveTimers.Add(key);
                    // TODO: exception ?
                    timer.OnTimeOut?.Invoke();
                }
            }
            foreach(var key in toRemoveTimers)
            {
                timers.Remove(key);
            }
            toRemoveTimers.Clear();
        }
        private List<ulong> toRemoveTimers = new List<ulong>();

        private void CheckAsap()
        {
            var keys = asap.Keys.ToList();
            foreach(var key in keys)
            {
                if (asap.TryGetValue(key, out var action))
                {
                    toRemoveAsap.Add(key);
                    // TODO: exception ?
                    action?.Invoke();
                }
            }
            foreach(var key in toRemoveAsap)
            {
                asap.Remove(key);
            }
            asap.Clear();
        }
        private List<ulong> toRemoveAsap = new List<ulong>();

        /// <summary>
        /// loop terminate
        /// </summary>
        public void Terminate()
        {
            if (LoopState == State.STOP)
            {
                throw new Exception();
            }
            if (IsTerminating || LoopState == State.TERMINATE) {
                return;
            }
            IsTerminating = true;
        }

        /// <summary>
        /// run action as soon as possible
        /// </summary>
        /// <returns></returns>
        public ulong CallAsSoonAsPossible(Action action)
        {
            var uid = NextUid;
            asap.Add(uid, action);
            return uid;
        }

        /// <summary>
        /// run action after a delay
        /// </summary>
        /// <param name="milliSec"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        public ulong CallAfter(ulong milliSec, Action action)
        {
            if (milliSec < (ulong)Frequency)
            {
                return CallAsSoonAsPossible(action);
            }

            var timeoutFrame = CurrentFrame + milliSec / Frequency;
            var timer = new MTimerAction(NextUid, timeoutFrame, action);
            timers.Add(timer.Uid, timer);

            return timer.Uid;
        }

        /// <summary>
        /// run action at intervals
        /// </summary>
        /// <param name="milliSec"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        public ulong CallInterval(ulong milliSec, Action action)
        {
            return 0;
        }

        private ulong NextUid => ++lastUid;

        /// <summary>
        /// cancel an action
        /// </summary>
        /// <param name="CallBack"></param>
        /// <returns></returns>
        public bool Cancel(ulong actionId)
        {
            // TODO:
            throw new NotImplementedException();
        }

        private MainLoop()
        {
            BelongThreadId = Thread.CurrentThread.ManagedThreadId;
        }

        public enum State
        {
            STOP = 0,   // before start
            WAITING,       
            PROCESSING,
            TERMINATE,  // after terminated
        }
    }
}
 