using System.Collections.Generic;
using TanksMP;
using UnityEngine;
using Vashta.Entropy.ScriptableObject;

namespace Vashta.Entropy.UI
{
    public class TeammateFinderPanel : GamePanel
    {
        public GameObject TeammateFinderIconPrefab;
        public GameModeDictionary GameModeDictionary;
        
        private List<TeammateFinderIcon> _teammateFinderIcons;
        private const int _numOfIcons = 8;

        private void Start()
        {
            // init icons
            _teammateFinderIcons = new List<TeammateFinderIcon>();
            
            for (int i = 0; i < _numOfIcons; i++)
            {
                GameObject newTeammateFinder = Instantiate(TeammateFinderIconPrefab, transform);
                TeammateFinderIcon newTeammateFinderIcon = newTeammateFinder.GetComponent<TeammateFinderIcon>();

                if (!newTeammateFinderIcon)
                {
                    Debug.LogError("Instantiated a new TeammateFinderIcon gameobject that was missing a TeammateFinderIcon component!");
                    continue;
                }
                
                _teammateFinderIcons.Add(newTeammateFinderIcon);
            }
        }

        private void Update()
        {
            RefreshIcons();
        }

        private void RefreshIcons()
        {
            // reset icons
            foreach (TeammateFinderIcon icon in _teammateFinderIcons)
            {
                icon.Hide();
            }
            
            // save our place while adding icons to the list
            int iconListIndex = 0;
            
            Player localPlayer = Player.GetLocalPlayer();
            int localPlayerTeamIndex = localPlayer.GetView().GetTeam();
            
            List<Player> players = Player.GetAllPlayers;
            
            // iterate over full list of players and fill out icons
            for (int i = 0; i < players.Count; i++)
            {
                if (iconListIndex > _teammateFinderIcons.Count) break;

                Player thisPlayer = players[i];
                
                // ignore local player
                if(thisPlayer.IsLocal)
                    continue;

                if (thisPlayer.GetView().GetTeam() == localPlayerTeamIndex)
                {
                    // If the player is on the local player's team, update the icon
                    _teammateFinderIcons[iconListIndex].SetPlayer(thisPlayer);
                    iconListIndex++;
                }
            }
            
            // Show game mode specific icons
            TanksMP.GameMode gameMode = GameManager.GetInstance().gameMode;

            if (gameMode == TanksMP.GameMode.CTF)
            {
                List<CollectibleCaptureTheFlag> flags = CollectibleCaptureTheFlag.GetAllFlags();

                for (int i = 0; i < flags.Count; i++)
                {
                    if (iconListIndex > _teammateFinderIcons.Count) break;

                    CollectibleCaptureTheFlag flag = flags[i];

                    if (!flag.gameObject.activeInHierarchy)
                        continue;

                    GameModeDefinition gameModeDefinition = GameModeDictionary[TanksMP.GameMode.CTF];

                    if (!gameModeDefinition)
                    {
                        Debug.LogError("Missing game mode for CTF!");
                        break;
                    }
                    
                    Sprite sprite = gameModeDefinition.IconSmall;
                    _teammateFinderIcons[iconListIndex].SetFlag(flag, sprite);
                    iconListIndex++;
                }
            }
        }
    }
}