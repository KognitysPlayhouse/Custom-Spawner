using Exiled.API.Extensions;
using Exiled.API.Features;
using Exiled.Events.EventArgs;
using MEC;
using Mirror;
using RemoteAdmin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;

namespace CustomSpawner
{
	public class EventHandler
	{
		public CustomSpawner plugin;
		public EventHandler(CustomSpawner plugin) => this.plugin = plugin;

		private static Vector3 SpawnPoint = new Vector3(240, 978, 96);

		private static Vector3 ClassDPoint = new Vector3(237, 980, 86);
		private static Vector3 SCPPoint = new Vector3(251, 980, 98);
		private static Vector3 ScientistPoint = new Vector3(245, 980, 107);
		private static Vector3 GuardPoint = new Vector3(230, 980, 95);
		private static Vector3 Tutorial = new Vector3(241, 980, 96);

		private List<GameObject> Dummies = new List<GameObject> { };
		private Dictionary<RoleType, string> dummiesToSpawn = new Dictionary<RoleType, string>
		{
			{ RoleType.Tutorial, "Random Team" },
			{ RoleType.ClassD, "Class D Team" },
			{ RoleType.Scp173, "SCP Team" },
			{ RoleType.Scientist, "Scientist Team" },
			{ RoleType.FacilityGuard, "MTF Team" },
		};

		private Dictionary<RoleType, KeyValuePair<Vector3, Quaternion>> dummySpawnPointsAndRotations = new Dictionary<RoleType, KeyValuePair<Vector3, Quaternion>>
		{
			{ RoleType.Tutorial, new KeyValuePair<Vector3, Quaternion>(Tutorial, new Quaternion(0, 0, 0, 0) ) },
			{ RoleType.ClassD, new KeyValuePair<Vector3, Quaternion>(ClassDPoint, new Quaternion(0, 0.1f, 0, -1) ) },
			{ RoleType.Scp173, new KeyValuePair<Vector3, Quaternion>(SCPPoint, new Quaternion(0, 0.8f, 0, -0.6f) ) },
			{ RoleType.Scientist, new KeyValuePair<Vector3, Quaternion>(ScientistPoint, new Quaternion(0, 1, 0, -0.2f) ) },
			{ RoleType.FacilityGuard, new KeyValuePair<Vector3, Quaternion>(GuardPoint, new Quaternion(0, 0.9f, 0, 0.4f) ) },
		};

		public void OnVerified(VerifiedEventArgs ev)
		{

		}

		public void OnRoundStart()
		{

		}

		public void OnWaitingForPlayers()
		{
			GameObject.Find("StartRound").transform.localScale = Vector3.zero;
			//Timing.RunCoroutine(UtilityMethods.LobbyTimer());

			foreach (var Role in dummiesToSpawn)
			{
				GameObject obj = UnityEngine.Object.Instantiate(
										NetworkManager.singleton.spawnPrefabs.FirstOrDefault(p => p.gameObject.name == "Player"));
				CharacterClassManager ccm = obj.GetComponent<CharacterClassManager>();
				if (ccm == null)
					Log.Error("CCM is null, this can cause problems!");
				ccm.CurClass = Role.Key;
				ccm.GodMode = true;
				//ccm.OldRefreshPlyModel(PlayerManager.localPlayer);
				obj.GetComponent<NicknameSync>().Network_myNickSync = Role.Value;
				obj.GetComponent<QueryProcessor>().PlayerId = 9999;
				obj.GetComponent<QueryProcessor>().NetworkPlayerId = 9999;
				obj.transform.localScale = new Vector3(2.3f, 2.3f, 2.3f);

				obj.transform.position = dummySpawnPointsAndRotations[Role.Key].Key;
				obj.transform.rotation = dummySpawnPointsAndRotations[Role.Key].Value;
				NetworkServer.Spawn(obj);
				Dummies.Add(obj);

				Pickup pickup = Exiled.API.Extensions.Item.Spawn(ItemType.SCP018, 0, dummySpawnPointsAndRotations[Role.Key].Key);
				GameObject gameObject = pickup.gameObject;
				gameObject.transform.localScale = new Vector3(30f, 0.1f, 30f);
				NetworkServer.UnSpawn(gameObject);
				NetworkServer.Spawn(pickup.gameObject);
				Dummies.Add(pickup.gameObject);

				// Don't think I need this but just incase
				//Markers.Add(pickup);

				Rigidbody rigidBody = pickup.gameObject.GetComponent<Rigidbody>();
				Collider[] collider = pickup.gameObject.GetComponents<Collider>();
				foreach (Collider thing in collider)
				{
					thing.enabled = false;
				}
				if (rigidBody != null)
				{
					rigidBody.useGravity = false;
					rigidBody.detectCollisions = false;
				}
				pickup.transform.localPosition = dummySpawnPointsAndRotations[Role.Key].Key + Vector3.down * 3.3f;
			}
		}
	}
}