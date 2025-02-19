using UnityEngine;

namespace Vashta.Entropy.StatusEffects
{
    /// <summary>
    /// A view to pass status effect information onto the UI.  Contains references
    /// to the Network struct and several other useful pieces of information.
    ///
    /// This is largely a wrapper for a struct.
    /// </summary>
    public class StatusEffectView
    {
        private StatusEffectNetwork _statusEffect;
        private bool _isFresh = false;
        
        public ref StatusEffectNetwork StatusEffect=> ref _statusEffect;
        
        public StatusEffectView(StatusEffectNetwork statusEffect)
        {
            _statusEffect = statusEffect;
        }

        public bool IsFresh()
        {
            return _isFresh;
        }

        public void SetFresh( bool fresh)
        {
            _isFresh = fresh;
        }
    }
}