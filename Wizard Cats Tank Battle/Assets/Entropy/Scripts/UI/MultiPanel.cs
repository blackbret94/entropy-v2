using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Vashta.Entropy.Util;

namespace Vashta.Entropy.UI
{
    public class MultiPanel : GamePanel
    {
        public GamePanel highResPanel;
        public GamePanel lowResPanel;

        public bool startEnabled;
        
        private bool _isHighRes = true;
        private bool _hasInit = false;

        private void Start()
        {
            Init();
            
            if(highResPanel == null)
                Debug.LogWarning("Missing Large Res Panel!");
            
            if(lowResPanel == null)
                Debug.LogWarning("Missing Low Res Panel!");

            if (startEnabled)
            {
                if (_isHighRes)
                {
                    highResPanel.SetActive(true);
                    lowResPanel.SetActive(false);
                }
                else
                {
                    highResPanel.SetActive(false);
                    lowResPanel.SetActive(true);
                }
            }
        }

        private void Init()
        {
            if (_hasInit)
                return;

            _isHighRes = CrossPlatformUIScaler.IsLargeScreenStatic();
            _hasInit = true;
        }

        public override void OpenPanel()
        {
            GetPanel().OpenPanel();
        }

        public override void ClosePanel()
        {
            GetPanel().ClosePanel();
        }

        public override void CloseByHotkey()
        {
            GetPanel().CloseByHotkey();
        }

        public override void TogglePanel()
        {
            GetPanel().TogglePanel();
        }

        public override void SetActive(bool b)
        {
            GetPanel().SetActive(b);
        }

        protected GamePanel GetPanel()
        {
            Init();
            
            if (_isHighRes)
            {
                return highResPanel;
            }
            else
            {
                return lowResPanel;
            }
        }
    }
}