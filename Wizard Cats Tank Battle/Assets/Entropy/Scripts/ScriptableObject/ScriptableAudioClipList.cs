using UnityEngine;

namespace Vashta.Entropy.ScriptableObject
{
    [CreateAssetMenu(fileName = "AudioClipList", menuName = "Entropy/AudioClipList", order = 1)]
    public class ScriptableAudioClipList: UnityEngine.ScriptableObject
    {
        public AudioClip[] AudioClips;

        public AudioClip GetRandomClip()
        {
            int count = AudioClips.Length;
            int index = Random.Range(0, count);
            return AudioClips[index];
        }
    }
}