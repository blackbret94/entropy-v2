using System.Collections.Generic;
using UnityEngine;
using Vashta.Entropy.StatusEffects;

namespace Vashta.Entropy.Character
{
    [System.Serializable]
    public class StatusEffectVisualizerUnit
    {
        public GameObject EffectRoot;
        public GameObject EffectPrefab;
        public List<StatusEffectData> TriggeredBy;

        public void Toggle(bool enable)
        {
            EffectRoot.SetActive(enable);
        }

        public void Toggle(SortedSet<ushort> indexedStatusEffectIds)
        {
            foreach (var statusEffectData in TriggeredBy)
            {
                if (indexedStatusEffectIds.Contains(statusEffectData.SessionId))
                {
                    Toggle(true);
                    return;
                }
            }
            
            Toggle(false);
        }
    }
}