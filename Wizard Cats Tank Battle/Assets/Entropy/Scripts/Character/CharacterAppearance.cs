using Entropy.Scripts.Player.Inventory;
using TanksMP;
using UnityEngine;
using Vashta.Entropy.Character.Prop;
using Vashta.Entropy.SaveLoad;
using Vashta.Entropy.ScriptableObject;
using Vashta.Entropy.UI;

namespace Vashta.Entropy.Character
{
    public class CharacterAppearance : MonoBehaviour
    {
        [Header("Dependencies")] 
        public Player Player;
        public PlayerCharacterWardrobe PlayerCharacterWardrobe;
        public PlayerInventory PlayerInventory;
        public CharacterAppearanceSaveLoad SaveLoad;

        [Header("Nodes")] 
        public Transform HatNode;
        public Transform CatBodyNode;
        public Transform CartNode;
        public Transform TurretNode;
        
        [Header("Config")]
        public Hat Hat;
        public BodyType Body;
        public Skin Skin;
        public Cart Cart;
        public GameObject Turret;
        public Material DefaultTeamMaterial;

        [HideInInspector] public Team Team;
        
        // Inventory Indexes
        private int
            _hatIndex = 0,
            _bodyIndex = 0,
            _skinIndex = 0,
            _cartIndex = 0,
            _turretIndex = 0;

        public int HatIndex => _hatIndex;
        public int BodyIndex => _bodyIndex;
        public int SkinIndex => _skinIndex;
        public int CartIndex => _cartIndex;
        public int TurretIndex => _turretIndex;

        public void Start()
        {
            if(Player == null)
                // Wardrobe
                LoadAppearance();
            else if(SaveLoad && (Player != null && Player.IsLocal))
                // Gameplay, local player vs. other players
                LoadAppearance();
        }

        public void LoadAppearance()
        {
            CharacterAppearanceSerializable appearanceSerializable = SaveLoad.Load();
            LoadFromSerialized(appearanceSerializable);
        }

        public void SaveAppearance()
        {
            CharacterAppearanceSerializable appearanceSerializable = Serialize();
            SaveLoad.Save(appearanceSerializable);
        }

        public void ApplyOutfit()
        {
            ReplaceMeshes();
            UpdatePlayer();
        }

        private void ReplaceMeshes()
        {
            ReplaceHat();
            ReplaceCatBody();
            ReplaceCatFur();
            ReplaceCart();
            //ReplaceTurret();
        }

        private void UpdatePlayer()
        {
            //UpdatePlayerTurret();
        }
        
        public CharacterWardrobeSelectorData NextHat()
        {
            int hatCount = PlayerInventory.Hats.Count;
            
            _hatIndex = ++_hatIndex >= hatCount ? 0 : _hatIndex;
            Hat = PlayerInventory.GetHatByIndex(_hatIndex);
            ReplaceHat();

            return new CharacterWardrobeSelectorData(_hatIndex+1, hatCount);
        }
        
        public CharacterWardrobeSelectorData PrevHat()
        {
            int hatCount = PlayerCharacterWardrobe.Hats.Count;

            _hatIndex = --_hatIndex < 0 ? hatCount-1 : _hatIndex;
            Hat = PlayerInventory.GetHatByIndex(_hatIndex);
            ReplaceHat();
            
            return new CharacterWardrobeSelectorData(_hatIndex+1, hatCount);
        }

        // NEED TO UPDATE
        public CharacterWardrobeSelectorData NextBodyType()
        {
            int count = PlayerInventory.BodyTypes.Count;

            _bodyIndex = ++_bodyIndex >= count ? 0 : _bodyIndex;
            Body = PlayerInventory.GetBodyTypeByIndex(_bodyIndex);
            ReplaceCatBody();
            
            return new CharacterWardrobeSelectorData(_bodyIndex+1, count);
        }

        // NEED TO UPDATE
        public CharacterWardrobeSelectorData PrevBodyType()
        {
            int count = PlayerInventory.BodyTypes.Count;

            _bodyIndex = --_bodyIndex < 0 ? count-1 : _bodyIndex;
            Body = PlayerInventory.GetBodyTypeByIndex(_bodyIndex);
            ReplaceCatBody();
            
            return new CharacterWardrobeSelectorData(_bodyIndex+1, count);
        }

        // NEED TO UPDATE
        public CharacterWardrobeSelectorData NextSkin()
        {
            int count = PlayerInventory.GetBodyTypeByIndex(_bodyIndex).SkinOptions.Count;

            _skinIndex = ++_skinIndex >= count ? 0 : _skinIndex;
            Skin = PlayerInventory.GetSkinByIndex(_bodyIndex, _skinIndex);
            ReplaceCatFur();
            
            return new CharacterWardrobeSelectorData(_skinIndex+1, count);
        }

        // NEED TO UPDATE
        public CharacterWardrobeSelectorData PrevSkin()
        {
            int count = PlayerInventory.GetBodyTypeByIndex(_bodyIndex).SkinOptions.Count;

            _skinIndex = --_skinIndex <= 0 ? count-1 : _skinIndex;
            Skin = PlayerInventory.GetSkinByIndex(_bodyIndex, _skinIndex);
            ReplaceCatFur();
            
            return new CharacterWardrobeSelectorData(_skinIndex+1, count);
        }

        public CharacterWardrobeSelectorData NextCart()
        {
            int count = PlayerInventory.Carts.Count;

            _cartIndex = ++_cartIndex >= count ? 0 : _cartIndex;
            Cart = PlayerInventory.GetCartByIndex(_cartIndex);
            ReplaceCart();

            return new CharacterWardrobeSelectorData(_cartIndex+1, count); 
        }

        public CharacterWardrobeSelectorData PrevCart()
        {
            int count = PlayerInventory.Carts.Count;

            _cartIndex = --_cartIndex <= 0 ? count-1 : _cartIndex;
            Cart = PlayerInventory.GetCartByIndex(_cartIndex);
            ReplaceCart();
            
            return new CharacterWardrobeSelectorData(_cartIndex+1, count);
        }

        public CharacterAppearanceSerializable Serialize()
        {
            return new CharacterAppearanceSerializable(Hat.Id, Body.Id, Skin.Id, Cart.Id);
        }

        public void LoadFromSerialized(CharacterAppearanceSerializable characterAppearanceSerializable)
        {
            PlayerInventory.Init();
            PlayerInventory.Load();
            
            string hatId = characterAppearanceSerializable.HatId;
            Hat = PlayerCharacterWardrobe.GetHatById(hatId);
            _hatIndex = PlayerInventory.GetHatIndexById(hatId);

            string bodyId = characterAppearanceSerializable.BodyId;
            Body = PlayerCharacterWardrobe.GetBodyTypeById(bodyId);
            _bodyIndex = PlayerInventory.GetBodyTypeIndexById(bodyId);

            string skinId = characterAppearanceSerializable.SkinId;
            Skin = PlayerCharacterWardrobe.GetSkinById(characterAppearanceSerializable.SkinId);
            _skinIndex = PlayerInventory.GetSkinIndexById(_bodyIndex, skinId);

            string cartId = characterAppearanceSerializable.CartId;
            Cart = PlayerCharacterWardrobe.GetCartById(cartId);
            _cartIndex = PlayerInventory.GetCartIndexById(cartId);

            ApplyOutfit();
        }

        /****************************/

        private void UpdatePlayerTurret()
        {
            // Player.turret = TurretNode.GetChild(0).transform;
        }

        private void ReplaceHat()
        {
            DestroyAllChildren(HatNode.transform);
            Instantiate(Hat.HatObject, HatNode);
        }

        private void ReplaceCatBody()
        {
            SkinnedMeshRenderer catBodyMeshFilter = CatBodyNode.GetComponent<SkinnedMeshRenderer>();
            catBodyMeshFilter.sharedMesh = Body.BodyMesh;
        }

        private void ReplaceCatFur()
        {
            SkinnedMeshRenderer catBodyMeshRenderer = CatBodyNode.GetComponent<SkinnedMeshRenderer>();
            catBodyMeshRenderer.materials = new[] { Skin.SkinMaterial };
        }

        private void ReplaceCart()
        {
            DestroyAllChildren(CartNode.transform);
            GameObject cart = Instantiate(Cart.CartObject, CartNode);
            CartProp cartProp = cart.GetComponentInChildren<CartProp>();

            if (cartProp == null)
            {
                Debug.LogError("Cart is missing a 'CartProp' component");
                return;
            }

            Material material;
            if (Team.material != null)
                material = Team.material;
            else
                material = DefaultTeamMaterial;
            
            cartProp.ColorizeForTeam(material);
        }

        private void ReplaceTurret()
        {
            DestroyAllChildren(Turret.transform);
            Instantiate(Turret, TurretNode);
        }

        private void DestroyAllChildren(Transform parent)
        {
            foreach (Transform child in parent)
            {
                GameObject.Destroy(child.gameObject);
            }
        }
    }
}
