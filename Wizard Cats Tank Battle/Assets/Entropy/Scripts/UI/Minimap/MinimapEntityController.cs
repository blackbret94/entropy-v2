using UnityEngine;

namespace Vashta.Entropy.UI.Minimap
{
    [RequireComponent(typeof(bl_MiniMapEntity))]
    public class MinimapEntityController : MonoBehaviour
    {
        private bl_MiniMapEntity _entity;
        private bool _hasInit;
        
        private void Start()
        {
            Init();
        }

        private void Init()
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
            
            _entity.IconColor = color;
        }

        protected void SetDeathIcon(Sprite newIcon)
        {
            Init();

            _entity.DeathIcon = newIcon;
        }
    }
}