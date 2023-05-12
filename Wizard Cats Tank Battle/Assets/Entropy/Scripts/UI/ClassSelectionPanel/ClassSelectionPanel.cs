using TanksMP;
using TMPro;
using UnityEngine;

namespace Vashta.Entropy.UI.ClassSelectionPanel
{
    public class ClassSelectionPanel : GamePanel
    {
        public ClassSelectionSelector ClassSelectionSelector;
        public ClassSelectionTeamSelector ClassSelectionTeamSelector;

        public float TimerLength = 30f;
        public TextMeshProUGUI Counter;

        public GameObject ApplyButton;
        public GameObject RespawnButton;
        public GameObject CountdownSelectionButton;
        
        private float _counterEndTime;
        private CanvasGroup _canvasGroup;

        private static ClassSelectionPanel _instance;

        public static ClassSelectionPanel Instance => _instance;
        
        private void Start()
        {
            _instance = this;
            _canvasGroup = GetComponent<CanvasGroup>();
            _canvasGroup.alpha = 1;
            
            ShowCountdownButtons();
            _counterEndTime = Time.time + TimerLength;
        }

        private void Update()
        {
            
            if (Time.time < _counterEndTime + 1)
            {
                Counter.text = Mathf.CeilToInt(_counterEndTime - Time.time).ToString();
            }
            else
            {
                if (Counter.IsActive())
                {
                    Counter.enabled = false;
                    ShowPostCountdownButtons();
                }
            }
        }

        private void ShowCountdownButtons()
        {
            CountdownSelectionButton.SetActive(true);
            RespawnButton.SetActive(false);
            ApplyButton.SetActive(false);
        }

        private void ShowPostCountdownButtons()
        {
            CountdownSelectionButton.SetActive(false);
            RespawnButton.SetActive(true);
            ApplyButton.SetActive(true);
        }

        public void ApplyChanges(bool respawnPlayer = false)
        {
            Player player = GameManager.GetInstance().localPlayer;
            player.SetClass(ClassSelectionSelector.SelectedClassDefinition(), respawnPlayer);
            player.PreferredTeamIndex = ClassSelectionTeamSelector.SelectedTeamIndex();
            
            ClosePanel();
        }

        public void RespawnPlayer()
        {
            ApplyChanges(true);
        }

        public bool CountdownIsActive()
        {
            return _counterEndTime >= Time.time;
        }
    }
}