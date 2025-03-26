using UnityEngine;

namespace Vashta.Entropy.Util
{
    public class Timer
    {
        public float LastTime { get; private set; }
        public float TimeGap { get; private set; }
        public bool HasRun { get; private set; }
        public bool IsActive { get; private set; }

        public Timer(float timeGap, bool runAtStart)
        {
            TimeGap = timeGap;
            IsActive = true;

            if (!runAtStart)
            {
                LastTime = Time.time;
            }
        }

        public bool Run()
        {
            if (!IsActive)
                return false;
            
            if (Time.time > LastTime + TimeGap)
            {
                HasRun = true;
                LastTime = Time.time;
                return true;
            }

            return false;
        }

        public float GetTimeToRun()
        {
            return Mathf.Max(0, (LastTime + TimeGap) - Time.time);
        }

        public void Set(bool active)
        {
            IsActive = active;
            Reset();
        }

        public void SetActive(bool active)
        {
            IsActive = active;
        }

        public void Reset()
        {
            LastTime = Time.time;
        }

        public void SetTimeGap(float newTimeGap)
        {
            TimeGap = newTimeGap;
        }
    }
}