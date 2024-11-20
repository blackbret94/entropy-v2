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
        }

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

        protected void SetEntitySprite(Sprite newIcon)
        {
            Init();

            _entity.Icon = newIcon;
        }

        protected void SetEntityColor(Color color)
        {
            Init();
            
            _entity.SetIconColor(color);
        }

        protected void SetOffscreenVisible(bool visibleOffscreen)
        {
            Init();

            _entity.OffScreen = visibleOffscreen;
        }
    }
}