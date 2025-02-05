using System;
using System.Collections;
using Fusion;
using Lovatto.MiniMap;
using TanksMP;
using UnityEngine;
using Vashta.Entropy.Player;
using Vashta.Entropy.ScriptableObject;

namespace Entropy.Scripts.Player
{
    /// <summary>
    /// Extends functionality from Lovatto.Minimap, integrating it with PUN and other systems
    /// </summary>
    public class MinimapController : NetworkBehaviour
    {
        private PlayerController _localPlayerController;
        public bl_MiniMap MiniMap;
        private float _refreshRate = .5f;
        private float _lastRefreshTime;

        public GameObject PointerPrefab;
        public TeamDefinitionDictionary TeamDefinitionDictionary;

        
        private int _teamIndex = -1;
        private GameObject _spawnedPointer;
        
        private void Start()
        {
            StartCoroutine(AttachToPlayerCoroutine());
        }

        private void Update()
        {
            if (!_localPlayerController)
                return;
            
            if (Time.time >= _lastRefreshTime + _refreshRate)
            {
                UpdateImage();
                _lastRefreshTime = Time.time;
            }
        }

        private IEnumerator AttachToPlayerCoroutine()
        {
            while (_localPlayerController == null)
            {
                _localPlayerController = PlayerList.GetLocalPlayer();
                yield return null;
            }

            MiniMap.Target = _localPlayerController.transform;
            Debug.Log("Player attached!");
        }

        private void UpdateImage()
        {
            if (MiniMap == null)
                return;
            
            int teamIndex = _localPlayerController.TeamIndex;

            if (teamIndex != _teamIndex)
            {
                _teamIndex = teamIndex;
                TeamInstance teamInstance = GameManager.GetInstance().TeamController.GetTeamByIndex(_teamIndex + 1);

                try
                {
                    if (teamInstance != null && teamInstance.teamDefinition != null)
                    {
                        MiniMap.SetPointerColor(teamInstance.teamDefinition.TeamColorPrim);
                    }
                }
                catch (NullReferenceException e)
                {
                    Debug.LogWarning(e);
                }
            }
        }

        /// <summary>
        /// Spawn a ping on all clients
        /// </summary>
        /// <param name="teamIndex"></param>
        public void CmdSpawnPing(Vector3 position, int teamIndex)
        {
            short[] pos = { (short)(position.x * 10), (short)(position.y * 10), (short)(position.z * 10) };
            RpcSpawnPing(pos, (short) teamIndex);
        }

        [Rpc(RpcSources.All, RpcTargets.All)]
        public void RpcSpawnPing(short[] position, short teamIndex)
        {
            PlayerController localPlayerController = PlayerList.GetLocalPlayer();

            // Only spawn if for this player's team
            if (localPlayerController != null && localPlayerController.TeamIndex == teamIndex)
            {
                if (_spawnedPointer)
                {
                    PoolManager.Despawn(_spawnedPointer);
                }
                
                Vector3 pos = new Vector3(position[0]/10f, position[1]/10f, position[2]/10f);
                _spawnedPointer = PoolManager.Spawn(PointerPrefab, pos, transform.rotation);
                bl_MapPointer mapPointer = _spawnedPointer.GetComponent<bl_MapPointer>();
                if (mapPointer)
                {
                    TeamInstance teamInstance = GameManager.GetInstance().TeamController.teams[teamIndex];

                    if (teamInstance != null)
                    {
                        mapPointer.SetColor(teamInstance.teamDefinition.TeamColorPrim);
                    }
                }
            }
        }
    }
}