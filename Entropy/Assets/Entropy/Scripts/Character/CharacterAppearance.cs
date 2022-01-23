using System;
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
            CharacterAppearanceSerializable serializable = SaveLoad.Load();
            LoadFromSerialized(serializable);
        }

        public void SaveAppearance()
        {
            CharacterAppearanceSerializable serializable = Serialize();
            SaveLoad.Save(serializable);
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
            int id = Hat.HatId;
            int hatCount = PlayerCharacterWardrobe.Hats.Count;

            id = ++id > hatCount ? 1 : id;
            Hat = PlayerCharacterWardrobe.GetHatById(id);
            ReplaceHat();

            return new CharacterWardrobeSelectorData(id, hatCount);
        }

        public CharacterWardrobeSelectorData PrevHat()
        {
            int id = Hat.HatId;
            int hatCount = PlayerCharacterWardrobe.Hats.Count;

            id = --id < 1 ? hatCount : 1;
            Hat = PlayerCharacterWardrobe.GetHatById(id);
            ReplaceHat();
            
            return new CharacterWardrobeSelectorData(id, hatCount);
        }

        public CharacterWardrobeSelectorData NextBodyType()
        {
            int id = Body.BodyTypeId;
            int count = PlayerCharacterWardrobe.BodyTypes.Count;

            id = ++id > count ? 1 : id;
            Body = PlayerCharacterWardrobe.GetBodyTypeById(id);
            ReplaceCatBody();
            
            return new CharacterWardrobeSelectorData(id, count);
        }

        public CharacterWardrobeSelectorData PrevBodyType()
        {
            int id = Body.BodyTypeId;
            int count = PlayerCharacterWardrobe.BodyTypes.Count;

            id = --id < 1 ? count : 1;
            Body = PlayerCharacterWardrobe.GetBodyTypeById(id);
            ReplaceCatBody();
            
            return new CharacterWardrobeSelectorData(id, count);
        }

        public CharacterWardrobeSelectorData NextSkin()
        {
            int id = Skin.SkinId;
            int count = PlayerCharacterWardrobe.GetBodyTypeById(Body.BodyTypeId).SkinOptions.Count;

            id = ++id > count ? 1 : id;
            Skin = PlayerCharacterWardrobe.GetSkinById(Body.BodyTypeId, id);
            ReplaceCatFur();
            
            return new CharacterWardrobeSelectorData(id, count);
        }

        public CharacterWardrobeSelectorData PrevSkin()
        {
            int id = Skin.SkinId;
            int count = PlayerCharacterWardrobe.GetBodyTypeById(Body.BodyTypeId).SkinOptions.Count;

            id = --id < 1 ? count : id;
            Skin = PlayerCharacterWardrobe.GetSkinById(Body.BodyTypeId, id);
            ReplaceCatFur();
            
            return new CharacterWardrobeSelectorData(id, count);
        }

        public CharacterWardrobeSelectorData NextCart()
        {
            int id = Cart.CartId;
            int count = PlayerCharacterWardrobe.Carts.Count;

            id = ++id > count ? 1 : id;
            Cart = PlayerCharacterWardrobe.GetCartById(id);
            ReplaceCart();

            return new CharacterWardrobeSelectorData(id, count); 
        }

        public CharacterWardrobeSelectorData PrevCart()
        {
            int id = Cart.CartId;
            int count = PlayerCharacterWardrobe.Carts.Count;

            id = --id < 1 ? count : 1;
            Cart = PlayerCharacterWardrobe.GetCartById(id);
            ReplaceCart();
            
            return new CharacterWardrobeSelectorData(id, count);
        }

        public CharacterAppearanceSerializable Serialize()
        {
            return new CharacterAppearanceSerializable(Hat.HatId, Body.BodyTypeId, Skin.SkinId, Cart.CartId);
        }

        public void LoadFromSerialized(CharacterAppearanceSerializable characterAppearanceSerializable)
        {
            Hat = PlayerCharacterWardrobe.GetHatById(characterAppearanceSerializable.HatId);
            Body = PlayerCharacterWardrobe.GetBodyTypeById(characterAppearanceSerializable.BodyId);
            Skin = PlayerCharacterWardrobe.GetSkinById(characterAppearanceSerializable.BodyId, characterAppearanceSerializable.SkinId);
            Cart = PlayerCharacterWardrobe.GetCartById(characterAppearanceSerializable.CartId);
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
