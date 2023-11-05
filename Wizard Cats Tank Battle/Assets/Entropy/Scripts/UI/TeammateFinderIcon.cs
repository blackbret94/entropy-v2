using TanksMP;
using UnityEngine;
using UnityEngine.UI;

namespace Vashta.Entropy.UI
{
    public class TeammateFinderIcon : GamePanel
    {
        public GameObject IconRoot;
        public Image IconImage;

        private int _screenEdgeOffset = 35;
        private Player _player;
        private CollectibleCaptureTheFlag _flag;

        private bool _showsPlayer;

        public void Hide()
        {
            _player = null;
            _flag = null;
        }

        public void SetPlayer(Player player)
        {
            _showsPlayer = true;
            _player = player;
            IconImage.sprite = _player.classIcon.sprite;
        }

        public void SetFlag(CollectibleCaptureTheFlag collectibleCaptureTheFlag, Sprite sprite)
        {
            _showsPlayer = false;
            _flag = collectibleCaptureTheFlag;
            IconImage.sprite = sprite;
        }

        private void Update()
        {
            if (!IsValid())
            {
                IconRoot.SetActive(false);
                return;
            }

            Camera mainCamera = Camera.main;
            
            if (!mainCamera) return;
            
            Vector3 positionOnScreen = mainCamera.WorldToScreenPoint(TargetWorldPosition());
            bool isVisible = PanelIsVisible(positionOnScreen);
            
            // update visibility
            IconRoot.SetActive(isVisible);
            
            // hide if player is dead
            if (_showsPlayer && _player.IsDead)
            {
                IconRoot.SetActive(false);
                return;
            }
            
            // Set position
            if (isVisible)
            {
                // update icon
                if(_showsPlayer)
                    IconImage.sprite = _player.classIcon.sprite;
                
                ClampPosition(positionOnScreen);
            }
        }

        private void ClampPosition(Vector3 positionOnScreen)
        {
            // clamp
            // x
            if (positionOnScreen.x < _screenEdgeOffset)
                positionOnScreen.x = _screenEdgeOffset;

            int rightEdgeBound = Screen.width - _screenEdgeOffset;
            if (positionOnScreen.x > rightEdgeBound)
                positionOnScreen.x = rightEdgeBound;

            // y
            if (positionOnScreen.y < _screenEdgeOffset)
                positionOnScreen.y = _screenEdgeOffset;

            int bottomBound = Screen.height - _screenEdgeOffset;
            if (positionOnScreen.y > bottomBound)
                positionOnScreen.y = bottomBound;

            transform.position = positionOnScreen;
        }

        private bool PanelIsVisible(Vector3 positionOnScreen)
        {
            return positionOnScreen.x < 0 ||
                   positionOnScreen.y < 0 ||
                   positionOnScreen.x > Screen.width ||
                   positionOnScreen.y > Screen.height;
        }

        private bool IsValid()
        {
            if (_showsPlayer)
                return _player != null;

            return _flag != null;
        }

        private Vector3 TargetWorldPosition()
        {
            if (_showsPlayer)
                return _player.transform.position;

            return _flag.transform.position;
        }
    }
}