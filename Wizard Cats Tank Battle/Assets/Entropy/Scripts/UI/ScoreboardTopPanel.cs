using System;
using System.Collections.Generic;
using TanksMP;
using UnityEngine;

namespace Vashta.Entropy.UI
{
    public class ScoreboardTopPanel : GamePanel
    {
        [Tooltip("Scale down to this if more than 2 teams are present")]
        public float SmallScale = .75f;
        
        public List<GameObject> Team3GameObjects;
        public List<GameObject> Team4GameObjects;

        private void Start()
        {
            Init();
        }

        private void Init()
        {
            // Adjust for number of teams
            int numOfTeams = GameManager.GetInstance().TeamController.teams.Length;
            
            ToggleAllInList(Team3GameObjects, numOfTeams >= 3);
            ToggleAllInList(Team4GameObjects, numOfTeams >= 4);
            
            // Adjust scale based on teams and screen size
            if (numOfTeams > 2)
            {
                GetComponent<RectTransform>().localScale = new Vector3(.75f, .75f, .75f);
            }
        }

        private void ToggleAllInList(List<GameObject> list, bool setActive)
        {
            foreach (var gameObject in list)
            {
                gameObject.SetActive(setActive);
            }
        }
    }
}