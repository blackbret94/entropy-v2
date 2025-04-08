/*  This file is part of the "Tanks Multiplayer" project by FLOBUK.
 *  You are only allowed to use these resources if you've bought them from the Unity Asset Store.
 * 	You shall not license, sublicense, sell, resell, transfer, assign, distribute or
 * 	otherwise make available to any third party the Service or the Content. */

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Fusion;
using Vashta.Entropy.Player;
using Random = UnityEngine.Random;

namespace TanksMP
{
    /// <summary>
    /// Manages network-synced spawning of prefabs, in this case collectibles and powerups.
    /// With the respawn time synced on all clients it supports host migration too.
    /// </summary>
    public class ObjectSpawner : NetworkBehaviour, IStateAuthorityChanged
    {
        [Networked]
        public NetworkBool IsSpawned { get; set; }
        
        /// <summary>
        /// Prefab to sync the instantiation for over the network.
        /// </summary>
        public List<GameObject> prefabList;

        /// <summary>
        /// Checkbox whether the object should be respawned after being despawned.
        /// </summary>
        public bool respawn;

        /// <summary>
        /// Delay until respawning the object again after it got despawned.
        /// </summary>
        public int respawnTime;

        /// <summary>
        /// Reference to the spawned prefab gameobject instance in the scene.
        /// </summary>
        [HideInInspector]
        public GameObject obj;

        /// <summary>
        /// Type of Collectible this spawner should utilize. This is set automatically,
        /// thus hidden in the inspector. Fires network messages depending on type.
        /// </summary>
        [HideInInspector]
        public CollectionType colType = CollectionType.Use;

        //time value when the next respawn should happen measured in game time
        [Networked]
        public float nextSpawn { get; set; }
        [Networked]
        public int lastInflatedObjectIndex { get; set; }
        [Networked]
        public int nextInflatedObjectIndex { get; set; } // calculate on state authority
        [Networked] public PlayerRef playerHeldBy { get; set; } // Make sure fusion supports Nullable
        [Networked] public NetworkBool isHeldByPlayer { get; set; }

        
        public override void Spawned()
        {
            if (!isActiveAndEnabled)
                return;
            
            if (HasStateAuthority)
            {
                // State authority: Init
                // Pick index
                PickNextSpawnIndex();
                
                // Spawn object
                Instantiate(nextInflatedObjectIndex);
                
                lastInflatedObjectIndex = nextInflatedObjectIndex;
                PickNextSpawnIndex();
            }
            else
            {
                // Client: "catch up" to state
                if (IsSpawned)
                {
                    Instantiate(lastInflatedObjectIndex);
                    
                    if (isHeldByPlayer)
                    {
                        PlayerController playerController = PlayerController.GetPlayerGameObject(playerHeldBy);
                        obj.transform.parent = playerController.transform;
                        obj.transform.localPosition = Vector3.zero + new Vector3(0, 2, 0);
                    }
                }
            }
            
            // switch (colType)
            // {
            //     case CollectionType.Use:
            //         if (obj == null || !obj.activeInHierarchy)
            //         {
            //             SetRespawn(nextSpawn);
            //         }
            //         break;
            //
            //     case CollectionType.Pickup:
            //         //in addition to the check above, here we check for the current state too
            //         //if the item got dropped, the master should send an updated respawn time as well
            //         if (obj == null || !obj.activeInHierarchy ||
            //           (obj.transform.parent != PoolManager.GetPool(obj).transform && obj.transform.position != transform.position))
            //         {
            //             SetRespawn(nextSpawn);
            //         }
            //         break;
            // }
        }

        private void PickNextSpawnIndex()
        {
            nextInflatedObjectIndex = Random.Range(0, prefabList.Count);
        }

        //calculates the remaining time until the next respawn,
        //waits for the delay to have passed and then instantiates the object
        IEnumerator SpawnRoutine()
		{
            if (HasStateAuthority)
            {
                PickNextSpawnIndex();
            }
            
            yield return new WaitForEndOfFrame();
            float delay = Mathf.Clamp(nextSpawn - (float)Runner.SimulationTime, 0, respawnTime);
			yield return new WaitForSeconds(delay);

            if (Runner.IsRunning)
            {
                //differ between CollectionType
                if(colType == CollectionType.Pickup && obj != null)
                {
                    //if the item is of type Pickup, it should not be destroyed after
                    //the routine is over but returned to its original position again
                    Return();
                }
                else
                {
                    //instantiate a new copy on all clients
                    SpawnObject(false);
                }
            }
        }

        public void StateAuthorityChanged()
        {
            if (HasStateAuthority)
            {
                if (!IsSpawned)
                {
                    StartCoroutine(SpawnRoutine());
                }
            }
        }
        
        /// <summary>
        /// Instantiates the object in the scene using PoolManager functionality.
        /// </summary>
		public void Instantiate(int index)
		{
            //sanity check in case there already is an object active
            if (obj != null)
                return;

            lastInflatedObjectIndex = index;

            if (lastInflatedObjectIndex >= prefabList.Count)
            {
                lastInflatedObjectIndex = 0;
                Debug.LogError("Tried to instantiate object with index larger than the size of PrefabList: " + lastInflatedObjectIndex);
            }
            GameObject prefab = prefabList[lastInflatedObjectIndex];
			obj = PoolManager.Spawn(prefab, transform.position, transform.rotation);
            //set the reference on the instantiated object for cross-referencing
            Collectible colItem = obj.GetComponent<Collectible>();
            if(colItem != null)
            {
                //set cross-reference
                colItem.spawner = this;
                //set internal item type automatically
                if (colItem is CollectibleTeam) colType = CollectionType.Pickup;
                else colType = CollectionType.Use;
            }
            
            IsSpawned = true;
		}
        
        private void SpawnObject(bool spawnInstantly)
        {
            lastInflatedObjectIndex = Random.Range(0, prefabList.Count);

            if (spawnInstantly)
            {
                Instantiate(lastInflatedObjectIndex);
            }
            else
            {
                Instantiate(lastInflatedObjectIndex);
            }
        }

        /// <summary>
        /// Collects the object and assigns it to the player with the corresponding view.
        /// </summary>
        public void Pickup(PlayerController playerController)
        {
            //in case this method call is received over the network earlier than the
            //spawner instantiation, here we make sure to catch up and instantiate it directly
            if (obj == null)
                SpawnObject(true);

            playerHeldBy = playerController.PlayerId;
            isHeldByPlayer = true;

            //get target view transform to parent to
            obj.transform.parent = playerController.transform;
            obj.transform.localPosition = Vector3.zero + new Vector3(0, 2, 0);
            
            //assign carrier to Collectible
            Collectible colItem = obj.GetComponent<Collectible>();
            if (colItem != null)
            {
                colItem.carrier = playerController;
                colItem.OnPickup();
            }

            // Notify
            if (colItem is CollectibleCaptureTheFlag)
            {
                GameManager gameManager = GameManager.GetInstance();
                gameManager.ui.GameLogPanel.EventSpoonPickedUp(playerController.PlayerName, gameManager.TeamController.GetTeamByIndex(playerController.TeamIndex).teamDefinition);
                
                if (playerController.IsLocal)
                {
                    GameManager.GetInstance().ui.DropCollectiblesButton.gameObject.SetActive(true);
                }
            }
            else
            {
                // This should register as being despawned if not an item carried by the player
                IsSpawned = false;
            }
            
            //cancel return timer as this object is now being carried around
            StopAllCoroutines();
        }


        /// <summary>
        /// Unparents the object from any carrier and drops it at the targeted position.
        /// </summary>
        public void Drop(Vector3 position)
        {
            //in case this method call is received over the network earlier than the
            //spawner instantiation, here we make sure to catch up and instantiate it directly
            if (obj == null)
                SpawnObject(true);

            //re-parent object to this spawner
            obj.transform.parent = PoolManager.GetPool(obj).transform;
            obj.transform.position = position;

            //reset carrier
            isHeldByPlayer = false;
            Collectible colItem = obj.GetComponent<Collectible>();
            if (colItem != null)
            {
                colItem.carrier = null;
                colItem.OnDrop();
            }

            //update respawn counter for a future point in time
            SetRespawn();
            //if the respawn mechanic is selected, trigger a new coroutine
            if (respawn)
            {
                StopAllCoroutines();
                StartCoroutine(SpawnRoutine());
            }
        }


        /// <summary>
        /// Returns the object back to this spawner's position. E.g. in Capture The Flag mode this
        /// can occur if a team collects its own flag, or a flag timed out after being dropped. 
        /// </summary>
        public void Return()
        {
            //re-parent object to this spawner
            obj.transform.parent = PoolManager.GetPool(obj).transform;
            obj.transform.position = transform.position;

            //reset carrier
            isHeldByPlayer = false;
            Collectible colItem = obj.GetComponent<Collectible>();
            if (colItem != null)
            {
                colItem.carrier = null;
                colItem.OnReturn();
            }

            //cancel return timer as the object is now back at its base position
            StopAllCoroutines();
        }


        /// <summary>
        /// Called by the spawned object to destroy itself on this managing component.
        /// This could be the case when it has been collected by players.
        /// </summary>
		public void Destroy()
		{
            //despawn object and clear references
			PoolManager.Despawn(obj);
            obj = null;

            IsSpawned = false;
			
            //if it should respawn again, trigger a new coroutine
			if(respawn)
                StartCoroutine(SpawnRoutine());
		}
        
        
        /// <summary>
        /// Called by the spawned object to reset its respawn counter when it is despawned
        /// in the scene. Also called on all clients with the current counter on host migration.
        /// </summary>
        public void SetRespawn(float init = 0f)
        {
            if(init > 0f)
                nextSpawn = init;
            else
                nextSpawn = (float)Runner.SimulationTime + respawnTime;
        }


        /// -- EDITOR --
#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            Color gizmoColor = Color.green;
            gizmoColor.a = .5f;
            Gizmos.color = Color.green;
            Gizmos.DrawSphere(transform.position, 1);
        }
#endif
	}


    /// <summary>
    /// Collectible type used on the ObjectSpawner, to define whether the item is consumed or picked up.
    /// </summary>
    public enum CollectionType
    {
        Use,
        Pickup
    }
}