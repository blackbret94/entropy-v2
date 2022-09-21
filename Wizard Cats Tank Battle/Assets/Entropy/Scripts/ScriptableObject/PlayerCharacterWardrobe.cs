using System.Collections.Generic;
using System.Linq;
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
        private Dictionary<string, Cart> _indexedCarts;
        private Dictionary<string, Turret> _indexedTurrets;

        private bool _hasInit = false;

        private void Awake()
        {
            Init();
        }

        private void Init()
        {
            if (_hasInit)
                return;
            
            IndexWardrobe();

            _hasInit = true;
        }

        private void IndexWardrobe()
        {
            _indexedHats = new Dictionary<string, Hat>();
            foreach (var hat in Hats)
                _indexedHats[hat.Id] = hat;
            
            _indexedBodyTypes = new Dictionary<string, BodyType>();
            foreach (var bt in BodyTypes)
                _indexedBodyTypes[bt.Id] = bt;
            
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

        public Skin GetSkinById(string bodyTypeId, int skinId)
        {
            Init();
            BodyType body = GetBodyTypeById(bodyTypeId);
            // LEFT OFF HERE
            int skinIndex = skinId + 1;

            if (skinIndex >= body.SkinOptions.Count)
                return body.SkinOptions[0];

            return body.SkinOptions[skinIndex];
        }

        public Cart GetRandomCart()
        {
            return Carts[Random.Range(0, Carts.Count)];
        }

        public Cart GetCartById(string id)
        {
            Init();
            if (_indexedBodyTypes.ContainsKey(id))
                return _indexedCarts[id];

            return Carts[0];
        }

        public Turret GetRandomTurret()
        {
            return Turrets[Random.Range(0, Turrets.Count)];
        }
    }
}
