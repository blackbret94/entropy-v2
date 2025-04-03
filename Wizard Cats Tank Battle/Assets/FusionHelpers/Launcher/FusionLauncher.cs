using System;
using System.Collections.Generic;
using Fusion;
using Fusion.Photon.Realtime;
using Fusion.Sockets;
using UnityEngine;
using UnityEngine.SceneManagement;
using Vashta.Entropy.PhotonExtensions;
using Vashta.Entropy.ScriptableObject;
using Vashta.Entropy.UI.MapSelection;

namespace FusionHelpers
{
	/// <summary>
	/// <p>Small helper that provides a simple session/player pattern for launching Fusion in either hosted/server or shared mode.
	/// </p>
	/// <p><br/>Usage:</p>
	/// <ul>
	/// <li>Extend FusionPlayer to create your custom player data object - the game scene will have one such instance per player.</li>
	/// <li>Extend FusionSession to create your session manager - the game will have one such instance per game session.</li>
	/// <li>Create prefabs of both and provide the player prefab in the sessions PlayerPrefab field.</li>
	/// <li>Pass a reference to the session prefab to the Launch() method on all peers.</li>
	/// <li>Use Runner.GetSingleton&lt;YourFusionSessionDerivedType&gt;() to access the session in your code.</li>
	/// </ul>
	/// <p>Note: You may need to adjust the capacity of the FusionSession _players member to match your max target player count.
	/// </p>
	/// <p><br/>How it works:</p>
	/// <ul>
	/// <li>The FusionLauncher will spawn an instance of the session prefab if called on host or master client.</li>
	/// <li>When a new player joins, that players ref is added to a networked dictionary on the session</li>
	/// <li>In hosted mode, an instance of the FusionPlayer prefab is spawned immediately</li>
	/// <li>In shared mode, the FusionSession will detect the added player ref and spawn the FusionPlayer on the relevant peer.</li>
	/// <li>When the FusionPlayer spawns on each peer, it registers itself with the session allowing you to access each players data via the session.</li>
	/// </ul>
	/// </summary>

	public class FusionLauncher : MonoBehaviour, INetworkRunnerCallbacks
	{
		private Action<NetworkRunner, ConnectionStatus, string> _connectionCallback;
		private FusionSession _sessionPrefab;

		public enum ConnectionStatus
		{
			Disconnected,
			Connecting,
			Failed,
			Connected,
			Loading,
			Loaded
		}
		
		public static FusionLauncher Launch(StartGameArgs startGameArgs, string region,FusionSession sessionPrefab,
			INetworkSceneManager sceneLoader,
			Action<NetworkRunner, ConnectionStatus, string> onConnect)
		{
			FusionLauncher launcher = new GameObject("Launcher").AddComponent<FusionLauncher>();

			launcher.InternalLaunch(startGameArgs, region,sessionPrefab, sceneLoader, onConnect);
			return launcher;
		}
		
		private async void InternalLaunch(StartGameArgs startGameArgs, string region, FusionSession sessionPrefab,
			INetworkSceneManager sceneManager,
			Action<NetworkRunner, ConnectionStatus, string> onConnect)
		{
			_sessionPrefab = sessionPrefab;
			_connectionCallback = onConnect;

			DontDestroyOnLoad(gameObject);
			
			NetworkRunner runner = gameObject.AddComponent<NetworkRunner>();
			runner.name = name;
			runner.ProvideInput = startGameArgs.GameMode != GameMode.Server;
			
			// An empty region will use the best region.
			PhotonAppSettings.Global.AppSettings.FixedRegion = region;

			SetConnectionStatus(runner, ConnectionStatus.Connecting, "");
			
			startGameArgs.ObjectProvider = gameObject.AddComponent<PooledNetworkObjectProvider>();
			
			NetworkSceneInfo scene = new NetworkSceneInfo();
			int sceneIndex = GetSceneIndex(startGameArgs);

			if (sceneIndex != -1)
			{
				scene.AddSceneRef(SceneRef.FromIndex(sceneIndex));
				startGameArgs.Scene = scene;
				startGameArgs.SceneManager = sceneManager;
			}

			await runner.StartGame(startGameArgs);
		}

		private int GetSceneIndex(StartGameArgs startGameArgs)
		{
			MapDefinitionDictionary mapDefinitionDictionary = GameDataSet.Get().MapDefinitionDictionary;
			MapDefinition mapDefinition;
			
			if (startGameArgs.SessionProperties.TryGetValue(RoomKeys.mapKey, out var sceneName))
			{
				Debug.Log("Map Scene Name: " + sceneName);
				
				if (sceneName == "random")
				{
					// Get random map
					mapDefinition = mapDefinitionDictionary.GetRandom();
				}
				else
				{
					// Get specific map
					mapDefinition = mapDefinitionDictionary.GetByName(sceneName);
				}
			}
			else
			{
				// Get random map
				mapDefinition = mapDefinitionDictionary.GetRandom();
			}
			
			if (mapDefinition != null)
			{
				// Get map index
				Debug.Log("Scene Index: " + mapDefinition.SceneIndex());
				return mapDefinition.SceneIndex();
			}
			else
			{
				Debug.LogError("Failed to get random map!");
				return -1;
			}
		}

		public void SetConnectionStatus(NetworkRunner runner, ConnectionStatus status, string message)
		{
			if (_connectionCallback != null)
				_connectionCallback(runner, status, message);
		}

		public void OnConnectedToServer(NetworkRunner runner)
		{
			Debug.Log("Connected to server");
			SetConnectionStatus(runner, ConnectionStatus.Connected, "");
		}

		public void OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason)
		{
			Debug.Log("Disconnected from server");
			SetConnectionStatus(runner, ConnectionStatus.Disconnected, "");
		}

		public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token)
		{
			request.Accept();
		}

		public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason)
		{
			Debug.Log($"Connect failed {reason}");
			SetConnectionStatus(runner, ConnectionStatus.Failed, reason.ToString());
		}

		public void OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player)
		{
		}

		public void OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player)
		{
		}

		public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
		{
			Debug.Log($"Player {player} Joined");
			if (runner.IsServer || runner.IsSharedModeMasterClient) {
		        if(!runner.TryGetSingleton(out FusionSession session) && _sessionPrefab!=null)
		        {
		          Debug.Log($"I am {(runner.IsServer ? "Server":"Master")} and I do not have a session - Spawning Session");
		          session = runner.Spawn(_sessionPrefab);
		        }
		        session.PlayerJoined(player);
			}
		}

		public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
		{
			if(runner.TryGetSingleton(out FusionSession session))
				session.PlayerLeft(player);
		}

		public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason)
		{
			Debug.Log("OnShutdown");
			string message = "";
			switch (shutdownReason)
			{
				case ShutdownReason.IncompatibleConfiguration:
					message = "This room already exist in a different game mode!";
					break;
				case ShutdownReason.Ok:
					message = "User terminated network session!"; 
					break;
				case ShutdownReason.Error:
					message = "Unknown network error!";
					break;
				case ShutdownReason.ServerInRoom:
					message = "There is already a server/host in this room";
					break;
				case ShutdownReason.DisconnectedByPluginLogic:
					message = "The Photon server plugin terminated the network session!";
					break;
				default:
					message = shutdownReason.ToString();
					break;
			}
			SetConnectionStatus(runner, ConnectionStatus.Disconnected, message);
			runner.ClearRunnerSingletons();
			Destroy(gameObject);
		}

		public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message) { }
		public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList) { }
		public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data) { }
		public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken) { }
		public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey key, ArraySegment<byte> data) { }

		public void OnReliableDataProgress(NetworkRunner runner, PlayerRef player, ReliableKey key, float progress) { }
		
		public void OnSceneLoadStart(NetworkRunner runner) { }
		public void OnSceneLoadDone(NetworkRunner runner) { }
		public void OnInput(NetworkRunner runner, NetworkInput input) { }
		public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input) { }
	}
}