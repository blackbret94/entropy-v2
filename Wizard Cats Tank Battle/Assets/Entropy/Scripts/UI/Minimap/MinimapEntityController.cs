using UnityEngine;

namespace Vashta.Entropy.UI.Minimap
{
    [RequireComponent(typeof(bl_MiniMapEntity))]
    public class MinimapEntityController : MonoBehaviour
    {
        protected bl_MiniMapEntity _entity;
        private bool _hasInit;
        
        private void Start()
        {
            Init();
            PostInit();
        }

        // Called to make sure everything is set up correctly.
        // Safety goes here, other logic should go in PostInit.
        protected virtual void Init()
        {
            if (_hasInit)
                return;

            _entity = GetComponent<bl_MiniMapEntity>();

            if (!_entity)
            {
                Debug.LogError("Missing connection to entity!");
            }
            
            _hasInit = true;
        }

        // Can be safely called during Init without risk of recursion. 
        // This is a good place to call setup code
        protected virtual void PostInit()
        {
            
        }

        protected void SetEntitySprite(Sprite newIcon)
        {
            Init();

            _entity.Icon = newIcon;
        }

        public void SetEntityColor(Color color)
        {
            Init();
            
            _entity.SetIconColor(color);
        }

        protected void SetOffscreenVisible(bool visibleOffscreen)
        {
            Init();

            _entity.OffScreen = visibleOffscreen;
        }

        public void ShowEntity()
        {
            Init();
            
            _entity.ShowItem();
        }

        public void HideEntity()
        {
            Init();
            
            _entity.HideItem();
        }
    }
}