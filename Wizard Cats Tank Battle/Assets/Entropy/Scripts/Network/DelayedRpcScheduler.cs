using System;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

namespace Vashta.Entropy.Network
{
    public class DelayedRpcScheduler : NetworkBehaviour
    {
        private struct ScheduledAction
        {
            public float ExecuteAt;
            public Action Action;
        }

        private readonly List<ScheduledAction> _scheduled = new List<ScheduledAction>();

        public static DelayedRpcScheduler Instance { get; private set; }

        private void Awake()
        {
            if (Instance == null) Instance = this;
        }

        public void Schedule(Action action, float delay)
        {
            float now = Runner.SimulationTime;
            _scheduled.Add(new ScheduledAction
            {
                ExecuteAt = now + delay,
                Action = action
            });
        }

        public override void FixedUpdateNetwork()
        {
            float now = Runner.SimulationTime;

            for (int i = _scheduled.Count - 1; i >= 0; i--)
            {
                if (now >= _scheduled[i].ExecuteAt)
                {
                    _scheduled[i].Action?.Invoke();
                    _scheduled.RemoveAt(i);
                }
            }
        }

        // Overload to handle parameters
        public void Schedule<T1, T2>(Action<T1, T2> action, T1 arg1, T2 arg2, float delay)
        {
            Schedule(() => action(arg1, arg2), delay);
        }
    }
}