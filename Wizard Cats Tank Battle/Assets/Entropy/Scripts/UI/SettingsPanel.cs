using System;
using Entropy.Scripts.Audio;
using TanksMP;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Vashta.Entropy.IO;
using Vashta.Entropy.ScriptableObject;
using Vashta.Entropy.UI.MapSelection;

namespace Vashta.Entropy.UI
{
    public class SettingsPanel : GamePanel
    {
        /// <summary>
        /// Settings: checkbox for playing background music.
        /// </summary>
        public Toggle musicToggle;

        /// <summary>
        /// Settings: slider for adjusting game sound volume.
        /// </summary>
        public Slider volumeSlider;

        public Toggle leftHandedModeToggle;

        public JoystickPlacementController joystickPlacementController;

        public MusicController MusicController;

        public Toggle FullscreenToggle;
        public Toggle AimArrowToggle;
        
        public TextMeshProUGUI MapNameText;
        public TextMeshProUGUI GameModeText;
        public TextMeshProUGUI GameModeDescription;
        
        public override void OpenPanel()
        {
            base.OpenPanel();
            ReadSettings();
            UpdateGameModeText();
            UpdateMapNameText();
            HUDPanel.Get().ClosePanel();
        }
        
        public override void ClosePanel()
        {
            base.ClosePanel();
            ApplySettings();
            HUDPanel.Get().OpenPanel();
        }

        private void UpdateGameModeText()
        {
            if (GameModeText == null)
                return;

            GameModeDefinition gameModeDefinition = GameManager.GetInstance().GetGameModeDefinition();
            GameModeText.text = gameModeDefinition.Title;

            if (GameModeDescription == null)
                return;

            GameModeDescription.text = gameModeDefinition.Description;
        }

        private void UpdateMapNameText()
        {
            if (MapNameText == null)
                return;
            
            MapDefinition gameModeDefinition = GameManager.GetInstance().GetMap();
            MapNameText.text = gameModeDefinition.Title;
        }

        private void ReadSettings()
        {
            musicToggle.isOn = SettingsReader.GetMusicIsOn();
            volumeSlider.value = SettingsReader.GetVolume();
            leftHandedModeToggle.isOn = SettingsReader.GetLeftHandedMode();
            AimArrowToggle.isOn = SettingsReader.GetAimArrow();
        }

        public void ApplySettings()
        {
            PlayerPrefs.SetString(PrefsKeys.playMusic, musicToggle.isOn.ToString());
            PlayerPrefs.SetFloat(PrefsKeys.appVolume, volumeSlider.value);
            PlayerPrefs.SetInt(PrefsKeys.lefthandedMode, leftHandedModeToggle.isOn ? 1 : 0);
            PlayerPrefs.SetInt(PrefsKeys.aimArrow, AimArrowToggle.isOn ? 1 : 0);
            
            UIGame.GetInstance().RefreshAimArrow();
            
            PlayerPrefs.Save();
        }

        public void OnLeftHandedModeChanged(bool leftHandedModeEnabled)
        {
            joystickPlacementController.ApplyChanges(leftHandedModeEnabled);
        }
        
        public void OnMusicChanged(bool value)
        {
            MusicController.AudioSource.enabled = musicToggle.isOn;
            MusicController.PlayMusic();
        }

        public void OnAimArrowChanged(bool value)
        {
        }
        
        /// <summary>
        /// Modify global game volume based on player selection.
        /// Called by Slider onValueChanged event.
        /// </summary>
        public void OnVolumeChanged(float value)
        {
            volumeSlider.value = value;
            AudioListener.volume = value;
        }
    }
}