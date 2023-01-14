using System;
using System.Collections;
using Entropy.Scripts.Audio;
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
        public SfxController SfxController;

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
        public Turret Turret;
        public Meow Meow;
        public Material DefaultTeamMaterial;

        [HideInInspector] public Team Team;
        
        // Inventory Indexes
        private int
            _hatIndex = 0,
            _bodyIndex = 0,
            _skinIndex = 0,
            _cartIndex = 0,
            _turretIndex = 0,
            _meowIndex = 0;

        public int HatIndex => _hatIndex;
        public int BodyIndex => _bodyIndex;
        public int SkinIndex => _skinIndex;
        public int CartIndex => _cartIndex;
        public int TurretIndex => _turretIndex;
        public int MeowIndex => _meowIndex;
        
        public void Start()
        {
            InitSfxController();

            if (SaveLoad == null)
                return;
            
            if (Player == null)
                // Wardrobe
                StartCoroutine(LoadAppearanceWhenInventoryIsLoaded());
            else if (SaveLoad && (Player != null && Player.IsLocal))
                // Gameplay, local player vs. other players
                StartCoroutine(LoadAppearanceWhenInventoryIsLoaded());
        }

        private IEnumerator LoadAppearanceWhenInventoryIsLoaded()
        {
            while (PlayerInventory.PlayerInventorySaveLoad == null)
                yield return null;

            PlayerInventorySaveLoad playerInventorySaveLoad = PlayerInventory.PlayerInventorySaveLoad;

            while (!playerInventorySaveLoad.IsLoaded())
                yield return null;
            
            SaveLoad.Load();
        }

        public void RefreshIndexes()
        {
            _hatIndex = PlayerInventory.GetHatIndexById(Hat.Id);
            _bodyIndex = PlayerInventory.GetBodyTypeIndexById(Body.Id);
            _skinIndex = PlayerInventory.GetSkinIndexById(_bodyIndex, Skin.Id);
            _cartIndex = PlayerInventory.GetCartIndexById(Cart.Id);
            _turretIndex = PlayerInventory.GetTurretIndexById(Turret.Id);
            _meowIndex = PlayerInventory.GetMeowIndexById(Meow.Id);
        }

        private void InitSfxController()
        {
            if (SfxController == null)
            {
                SfxController = Camera.main.gameObject.GetComponentInChildren<SfxController>();
            }
        }
        
        public void LoadAppearanceCallback(CharacterAppearanceSerializable appearance)
        {
            LoadFromSerialized(appearance);
        }

        public void SaveAppearance()
        {
            CharacterAppearanceSerializable appearanceSerializable = Serialize();
            SaveLoad.Save(appearanceSerializable);
        }

        public void ApplyOutfit()
        {
            ReplaceMeshes();
        }

        private void ReplaceMeshes()
        {
            ReplaceHat();
            ReplaceCatBody();
            ReplaceCatFur();
            ReplaceCart();
            ReplaceTurret();
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
            int hatCount = PlayerInventory.Hats.Count;

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

            _skinIndex = --_skinIndex < 0 ? count-1 : _skinIndex;
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

            _cartIndex = --_cartIndex < 0 ? count-1 : _cartIndex;
            Cart = PlayerInventory.GetCartByIndex(_cartIndex);
            ReplaceCart();
            
            return new CharacterWardrobeSelectorData(_cartIndex+1, count);
        }

        public CharacterWardrobeSelectorData NextTurret()
        {
            int count = PlayerInventory.Turrets.Count;

            _turretIndex = ++_turretIndex >= count ? 0 : _turretIndex;
            Turret = PlayerInventory.GetTurretByIndex(_turretIndex);
            ReplaceTurret();

            return new CharacterWardrobeSelectorData(_turretIndex+1, count); 
        }

        public CharacterWardrobeSelectorData PrevTurret()
        {
            int count = PlayerInventory.Turrets.Count;

            _turretIndex = --_turretIndex < 0 ? count-1 : _turretIndex;
            Turret = PlayerInventory.GetTurretByIndex(_turretIndex);
            ReplaceTurret();
            
            return new CharacterWardrobeSelectorData(_turretIndex+1, count);
        }

        public CharacterWardrobeSelectorData NextMeow()
        {
            int count = PlayerInventory.Meows.Count;
            
            _meowIndex = ++_meowIndex >= count ? 0 : _meowIndex;
            Meow = PlayerInventory.GetMeowByIndex(_meowIndex);

            return new CharacterWardrobeSelectorData(_meowIndex+1, count); 
        }
        
        public CharacterWardrobeSelectorData PrevMeow()
        {
            int count = PlayerInventory.Meows.Count;

            _meowIndex = --_meowIndex < 0 ? count-1 : _meowIndex;
            Meow = PlayerInventory.GetMeowByIndex(_meowIndex);
            
            return new CharacterWardrobeSelectorData(_meowIndex+1, count);
        }
        
        public CharacterAppearanceSerializable Serialize()
        {
            return new CharacterAppearanceSerializable(Hat.Id, Body.Id, Skin.Id, Cart.Id, Turret.Id, Meow.Id);
        }

        public void LoadFromSerialized(CharacterAppearanceSerializable characterAppearanceSerializable)
        {
            PlayerInventory.Init();
            
            string hatId = characterAppearanceSerializable.HatId;
            Hat = PlayerCharacterWardrobe.GetHatById(hatId);

            string bodyId = characterAppearanceSerializable.BodyId;
            Body = PlayerCharacterWardrobe.GetBodyTypeById(bodyId);

            string skinId = characterAppearanceSerializable.SkinId;
            Skin = PlayerCharacterWardrobe.GetSkinById(skinId);

            string cartId = characterAppearanceSerializable.CartId;
            Cart = PlayerCharacterWardrobe.GetCartById(cartId);

            string turretId = characterAppearanceSerializable.TurretId;
            Turret = PlayerCharacterWardrobe.GetTurretById(turretId);

            string meowId = characterAppearanceSerializable.MeowId;
            Meow = PlayerCharacterWardrobe.GetMeowById(meowId);

            try
            {
                _hatIndex = PlayerInventory.GetHatIndexById(hatId);
                _bodyIndex = PlayerInventory.GetBodyTypeIndexById(bodyId);
                _skinIndex = PlayerInventory.GetSkinIndexById(_bodyIndex, skinId);
                _cartIndex = PlayerInventory.GetCartIndexById(cartId);
                _turretIndex = PlayerInventory.GetTurretIndexById(turretId);
                _meowIndex = PlayerInventory.GetMeowIndexById(meowId);
            }
            catch (Exception e) {}

            ApplyOutfit();
        }

        /****************************/

        private void ReplaceHat()
        {
            DestroyAllChildren(HatNode.transform);
            Instantiate(Hat.ItemObject, HatNode);
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
            GameObject cart = Instantiate(Cart.ItemObject, CartNode);
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
            DestroyAllChildren(TurretNode.transform);
            Instantiate(Turret.ItemObject, TurretNode);
        }

        private void DestroyAllChildren(Transform parent)
        {
            foreach (Transform child in parent)
            {
                GameObject.Destroy(child.gameObject);
            }
        }

        public void PlayMeow()
        {
            Debug.Log("Meowing!");
            
            SfxController.PlaySound(Meow.AudioClip);
        }
    }
}
