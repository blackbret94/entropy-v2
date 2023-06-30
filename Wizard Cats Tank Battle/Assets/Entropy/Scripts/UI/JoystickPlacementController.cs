using System;
using TanksMP;
using UnityEngine;

namespace Vashta.Entropy.UI
{
    public class JoystickPlacementController : MonoBehaviour
    {
        public RectTransform LeftSlot,
            RightSlot,
            MovementTrigger,
            FireTrigger;

        private void Start()
        {
            Load();
        }

        private void Load()
        {
            bool leftHandedModeEnabled = Convert.ToBoolean(PlayerPrefs.GetInt(PrefsKeys.lefthandedMode, 0));
            
            if(leftHandedModeEnabled)
                SetLeftHandedMode();
            else
                SetRightHandedMode();
        }

        public void ApplyChanges(bool leftHandedModeEnabled)
        {
            if(leftHandedModeEnabled)
                SetLeftHandedMode();
            else
                SetRightHandedMode();
        }

        public void SetRightHandedMode()
        {
            MovementTrigger.SetParent(LeftSlot);
            MovementTrigger.anchoredPosition = Vector3.zero;
            
            FireTrigger.SetParent(RightSlot);
            FireTrigger.anchoredPosition = Vector3.zero;
        }

        public void SetLeftHandedMode()
        {
            MovementTrigger.SetParent(RightSlot);
            MovementTrigger.anchoredPosition = Vector3.zero;
            
            FireTrigger.SetParent(LeftSlot);
            FireTrigger.anchoredPosition = Vector3.zero;
        }
    }
}