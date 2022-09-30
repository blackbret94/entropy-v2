using System.Collections.Generic;
using UnityEngine;

namespace Vashta.Entropy.ScriptableObject
{
    [CreateAssetMenu(fileName = "WardrobeData", menuName = "Entropy/PlayerCharacterWardrobe", order = 1)]
    public class PlayerCharacterWardrobe : UnityEngine.ScriptableObject
    {
        public List<Hat> Hats;
        public List<BodyType> BodyTypes;
        public List<Cart> Carts;
        public List<Turret> Turrets;

        private Dictionary<string, Hat> _indexedHats;
        private Dictionary<string, BodyType> _indexedBodyTypes;
        private Dictionary<string, Skin> _indexedSkins; // This will need to be updated if unique body type+skin combos are ever added
        private Dictionary<string, Cart> _indexedCarts;
        private Dictionary<string, Turret> _indexedTurrets;

        private void Awake()
        {
            Init();
        }

        // The issue here is that this is a scriptable object
        private void Init()
        {
            if (_indexedHats != null)
                return;

            IndexWardrobe();
        }

        private void IndexWardrobe()
        {
            Debug.Log("Indexing wardrobe");
            _indexedHats = new Dictionary<string, Hat>();
            foreach (var hat in Hats)
                _indexedHats[hat.Id] = hat;
            
            _indexedBodyTypes = new Dictionary<string, BodyType>();
            foreach (var bt in BodyTypes)
                _indexedBodyTypes[bt.Id] = bt;

            _indexedSkins = new Dictionary<string, Skin>();
            foreach (var skin in BodyTypes[0].SkinOptions)
            {
                _indexedSkins[skin.Id] = skin;
            }
            
            _indexedCarts = new Dictionary<string, Cart>();
            foreach (var cart in Carts)
                _indexedCarts[cart.Id] = cart;
            
            _indexedTurrets = new Dictionary<string, Turret>();
            foreach (var turret in Turrets)
                _indexedTurrets[turret.Id] = turret;
        }

        public Hat GetRandomHat()
        {
            return Hats[Random.Range(0, Hats.Count)];
        }

        public Hat GetHatById(string id)
        {
            Init();
            if (_indexedHats.ContainsKey(id))
                return _indexedHats[id];

            return Hats[0];
        }

        public BodyType GetRandomBodyType()
        {
            return BodyTypes[Random.Range(0, BodyTypes.Count)];
        }

        public BodyType GetBodyTypeById(string id)
        {
            Init();
            if (_indexedBodyTypes.ContainsKey(id))
                return _indexedBodyTypes[id];

            return BodyTypes[0];
        }

        // Will need to be updated if we ever add unique mesh + skin combos
        public Skin GetSkinById(string skinId)
        {
            Init();

            if (_indexedSkins.ContainsKey(skinId))
                return _indexedSkins[skinId];

            return BodyTypes[0].SkinOptions[0];
        }

        public Cart GetRandomCart()
        {
            return Carts[Random.Range(0, Carts.Count)];
        }

        public Cart GetCartById(string id)
        {
            Init();
            if (_indexedCarts.ContainsKey(id))
                return _indexedCarts[id];

            Debug.Log("Could not find cart with ID: " + id);
            return Carts[0];
        }

        public Turret GetRandomTurret()
        {
            return Turrets[Random.Range(0, Turrets.Count)];
        }
    }
}
