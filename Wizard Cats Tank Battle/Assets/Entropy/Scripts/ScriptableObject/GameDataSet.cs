using UnityEngine;
using Vashta.Entropy.StatusEffects;

namespace Vashta.Entropy.ScriptableObject
{
    public class GameDataSet : MonoBehaviour
    {
        private static GameDataSet _instance;
        public static GameDataSet Get() => _instance;
        
        public StatusEffectDirectory StatusEffectDirectory;
        public GameModeDictionary GameModeDictionary;
        public BulletDictionary BulletDictionary;
        public PowerupDirectory PowerupDirectory;
        public ProjectileDirectory ProjectileDirectory;
        public RarityDictionary RarityDictionary;
        public TeamDefinitionDictionary TeamDefinitionDictionary;
        public VisualEffectDirectory VisualEffectDirectory;
        
        private void Awake()
        {
            if(_instance != null)
                Destroy(this);
            else
            {
                DontDestroyOnLoad(this);
                _instance = this;
            }
        }
        
    }
}