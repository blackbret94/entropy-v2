using UnityEngine;

namespace Vashta.Entropy.UI.GameLog
{
    public class GameLogRow
    {
        private float _fadeOutAnimationLength = 1f;
        
        public GameLogRow(float spawnTime, float timeToFade, string text)
        {
            SpawnTime = spawnTime;
            TimeToFade = timeToFade;
            Text = text;
            IsFresh = true;
        }

        public bool IsExpired()
        {
            return SpawnTime + TimeToFade < Time.time;
        }

        public bool ShouldDelete()
        {
            return SpawnTime + TimeToFade + _fadeOutAnimationLength < Time.time;
        }

        public string Text { get; }
        public float SpawnTime { get; }
        public float TimeToFade { get; }
        public bool IsFresh { get; set; }
    }
}