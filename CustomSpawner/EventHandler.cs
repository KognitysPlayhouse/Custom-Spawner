using Exiled.API.Extensions;
using Exiled.API.Features;
using Exiled.Events.EventArgs;
using MEC;
using Mirror;
using RemoteAdmin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;

namespace CustomSpawner
{
	public class EventHandler
	{
		public CustomSpawner plugin;
		public EventHandler(CustomSpawner plugin) => this.plugin = plugin;

		private readonly Config Config = CustomSpawner.Singleton.Config;

		private static Vector3 SpawnPoint = new Vector3(240, 978, 96); // Spawn point for all players when they get set to tutorial

		// Spawn points for the different teams
		private static Vector3 ClassDPoint = new Vector3(237, 980, 86);
		private static Vector3 SCPPoint = new Vector3(251, 980, 98);
		private static Vector3 ScientistPoint = new Vector3(245, 980, 107);
		private static Vector3 GuardPoint = new Vector3(230, 980, 95);
		private static Vector3 Tutorial = new Vector3(241, 980, 96);

		private CoroutineHandle lobbyTimer;

		private int SCPsToSpawn = 0;
		private int ClassDsToSpawn = 0;
		private int ScientistsToSpawn = 0;
		private int GuardsToSpawn = 0;

		private List<Pickup> boll = new List<Pickup> { }; // boll :flushed:

		private List<GameObject> Dummies = new List<GameObject> { };

		private static Dictionary<RoleType, KeyValuePair<Vector3, Quaternion>> dummySpawnPointsAndRotations = new Dictionary<RoleType, KeyValuePair<Vector3, Quaternion>>
		{
			{ RoleType.Tutorial, new KeyValuePair<Vector3, Quaternion>(Tutorial, new Quaternion(0, 0, 0, 0) ) },
			{ RoleType.ClassD, new KeyValuePair<Vector3, Quaternion>(ClassDPoint, new Quaternion(0, 0.1f, 0, -1) ) },
			{ RoleType.Scp173, new KeyValuePair<Vector3, Quaternion>(SCPPoint, new Quaternion(0, 0.8f, 0, -0.6f) ) },
			{ RoleType.Scientist, new KeyValuePair<Vector3, Quaternion>(ScientistPoint, new Quaternion(0, 1, 0, -0.2f) ) },
			{ RoleType.FacilityGuard, new KeyValuePair<Vector3, Quaternion>(GuardPoint, new Quaternion(0, 0.9f, 0, 0.4f) ) },
		};

		public void OnPickingUp(PickingUpItemEventArgs ev)
		{
			if (boll.Contains(ev.Pickup))
				ev.IsAllowed = false;
		}

		public void OnVerified(VerifiedEventArgs ev)
		{
			if (!Round.IsStarted && (GameCore.RoundStart.singleton.NetworkTimer > 1 || GameCore.RoundStart.singleton.NetworkTimer == -2))
			{
				Timing.CallDelayed(0.5f, () =>
				{
					ev.Player.IsOverwatchEnabled = false;
					ev.Player.Role = RoleType.Tutorial;
					Scp096.TurnedPlayers.Add(ev.Player);
					Scp173.TurnedPlayers.Add(ev.Player);
				});

				Timing.CallDelayed(1f, () =>
				{
					ev.Player.Position = SpawnPoint;
				});
			}
		}

		public void OnRoundStart()
		{
			System.Random random = new System.Random();
			foreach (var thing in Dummies)
			{
				UnityEngine.Object.Destroy(thing); // Deleting the dummies and SCP-018 circles
			}
			if (lobbyTimer.IsRunning)
			{
				Timing.KillCoroutines(lobbyTimer);
			}

			for (int x = 0; x < Player.List.ToList().Count; x++)
			{
				if (x >= Config.SpawnQueue.Count())
				{
					ClassDsToSpawn += 1;
					continue;
				}
				switch (Config.SpawnQueue[x])
				{
					case '4':
						ClassDsToSpawn += 1;
						break;
					case '0':
						SCPsToSpawn += 1;
						break;
					case '1':
						GuardsToSpawn += 1;
						break;
					case '3':
						ScientistsToSpawn += 1;
						break;
				}
			}

			List<Player> BulkList = Player.List.ToList();
			List<Player> SCPPlayers = new List<Player> { };
			List<Player> ScientistPlayers = new List<Player> { };
			List<Player> GuardPlayers = new List<Player> { };
			List<Player> ClassDPlayers = new List<Player> { };

			List<Player> PlayersToSpawnAsSCP = new List<Player> { };
			List<Player> PlayersToSpawnAsScientist = new List<Player> { };
			List<Player> PlayersToSpawnAsGuard = new List<Player> { };
			List<Player> PlayersToSpawnAsClassD = new List<Player> { };

			foreach (var player in Player.List)
			{
				if (Vector3.Distance(player.Position, SCPPoint) <= 3)
				{
					SCPPlayers.Add(player);
				}
				else if (Vector3.Distance(player.Position, ClassDPoint) <= 3)
				{
					ClassDPlayers.Add(player);
				}
				else if (Vector3.Distance(player.Position, ScientistPoint) <= 3)
				{
					ScientistPlayers.Add(player);
				}
				else if (Vector3.Distance(player.Position, GuardPoint) <= 3)
				{
					GuardPlayers.Add(player);
				}
			}
			// ---------------------------------------------------------------------------------------\\
			// ClassD
			if (ClassDsToSpawn != 0)
			{
				if (ClassDPlayers.Count <= ClassDsToSpawn) // Less people (or equal) voted than what is required in the game.
				{
					foreach (Player ply in ClassDPlayers)
					{
						PlayersToSpawnAsClassD.Add(ply);
						ClassDsToSpawn -= 1;
						BulkList.Remove(ply);
					}
				}
				else // More people voted than what is required, time to play the game of chance.
				{
					for (int x = 0; x < ClassDsToSpawn; x++)
					{
						Player Ply = ClassDPlayers[random.Next(ClassDPlayers.Count)];
						PlayersToSpawnAsClassD.Add(Ply);
						ClassDPlayers.Remove(Ply); // Removing winner from the list
						BulkList.Remove(Ply); // Removing the winners from the bulk list
					}
					ClassDsToSpawn = 0;
				}
			}
			// ---------------------------------------------------------------------------------------\\
			// Scientists
			if (ScientistsToSpawn != 0)
			{
				if (ScientistPlayers.Count <= ScientistsToSpawn) // Less people (or equal) voted than what is required in the game.
				{
					foreach (Player ply in ScientistPlayers)
					{
						PlayersToSpawnAsScientist.Add(ply);
						ScientistsToSpawn -= 1;
						BulkList.Remove(ply);
					}
				}
				else // More people voted than what is required, time to play the game of chance.
				{
					for (int x = 0; x < ScientistsToSpawn; x++)
					{
						Player Ply = ScientistPlayers[random.Next(ScientistPlayers.Count)];
						PlayersToSpawnAsScientist.Add(Ply);
						ScientistPlayers.Remove(Ply); // Removing winner from the list
						BulkList.Remove(Ply); // Removing the winners from the bulk list
					}
					ScientistsToSpawn = 0;
				}
			}
			// ---------------------------------------------------------------------------------------\\
			// Guards
			if (GuardsToSpawn != 0)
			{
				if (GuardPlayers.Count <= GuardsToSpawn) // Less people (or equal) voted than what is required in the game.
				{
					foreach (Player ply in GuardPlayers)
					{
						PlayersToSpawnAsGuard.Add(ply);
						GuardsToSpawn -= 1;
						BulkList.Remove(ply);
					}
				}
				else // More people voted than what is required, time to play the game of chance.
				{
					for (int x = 0; x < GuardsToSpawn; x++)
					{
						Player Ply = GuardPlayers[random.Next(GuardPlayers.Count)];
						PlayersToSpawnAsGuard.Add(Ply);
						GuardPlayers.Remove(Ply); // Removing winner from the list
						BulkList.Remove(Ply); // Removing the winners from the bulk list
					}
					GuardsToSpawn = 0;
				}
			}
			// ---------------------------------------------------------------------------------------\\
			// SCPs
			if (SCPsToSpawn != 0)
			{
				if (SCPPlayers.Count <= SCPsToSpawn) // Less people (or equal) voted than what is required in the game.
				{
					foreach (Player ply in SCPPlayers)
					{
						PlayersToSpawnAsSCP.Add(ply);
						SCPsToSpawn -= 1;
						BulkList.Remove(ply);
					}
				}
				else // More people voted than what is required, time to play the game of chance.
				{
					for (int x = 0; x < SCPsToSpawn; x++)
					{
						Player Ply = SCPPlayers[random.Next(SCPPlayers.Count)];
						SCPPlayers.Remove(Ply);
						PlayersToSpawnAsSCP.Add(Ply); // Removing winner from the list
						BulkList.Remove(Ply); // Removing the winners from the bulk list
					}
					SCPsToSpawn = 0;
				}
			}
			// ---------------------------------------------------------------------------------------\\
			// ---------------------------------------------------------------------------------------\\
			// ---------------------------------------------------------------------------------------\\
			// ---------------------------------------------------------------------------------------\\

			// At this point we need to check for any blanks and fill them in via the bulk list guys
			if (ClassDsToSpawn != 0)
			{
				for (int x = 0; x < ClassDsToSpawn; x++)
				{
					Player Ply = BulkList[random.Next(BulkList.Count)];
					PlayersToSpawnAsClassD.Add(Ply);
					BulkList.Remove(Ply); // Removing the winners from the bulk list
				}
			}
			if (SCPsToSpawn != 0)
			{
				for (int x = 0; x < SCPsToSpawn; x++)
				{
					Player Ply = BulkList[random.Next(BulkList.Count)];
					PlayersToSpawnAsSCP.Add(Ply);
					BulkList.Remove(Ply); // Removing the winners from the bulk list
				}
			}
			if (ScientistsToSpawn != 0)
			{
				for (int x = 0; x < ScientistsToSpawn; x++)
				{
					Player Ply = BulkList[random.Next(BulkList.Count)];
					PlayersToSpawnAsScientist.Add(Ply);
					BulkList.Remove(Ply); // Removing the winners from the bulk list
				}
			}
			if (GuardsToSpawn != 0)
			{
				for (int x = 0; x < GuardsToSpawn; x++)
				{
					Player Ply = BulkList[random.Next(BulkList.Count)];
					PlayersToSpawnAsGuard.Add(Ply);
					BulkList.Remove(Ply); // Removing the winners from the bulk list
				}
			}
			// ---------------------------------------------------------------------------------------\\

			// Okay we have the list! Time to spawn everyone in, we'll leave SCP for last as it has a bit of logic.
			foreach (Player ply in PlayersToSpawnAsClassD)
			{
				ply.Role = RoleType.ClassD;
			}
			foreach (Player ply in PlayersToSpawnAsScientist)
			{
				ply.Role = RoleType.Scientist;
			}
			foreach (Player ply in PlayersToSpawnAsGuard)
			{
				ply.Role = RoleType.FacilityGuard;
			}

			// ---------------------------------------------------------------------------------------\\

			// SCP Logic, preventing SCP-079 from spawning if there isn't at least 2 other SCPs
			List<RoleType> Roles = new List<RoleType>
				{ RoleType.Scp049, RoleType.Scp096, RoleType.Scp106, RoleType.Scp173, RoleType.Scp93953, RoleType.Scp93989 };

			if (PlayersToSpawnAsSCP.Count > 2)
				Roles.Add(RoleType.Scp079);

			foreach (Player ply in PlayersToSpawnAsSCP)
			{
				RoleType role = Roles[random.Next(Roles.Count)];
				Roles.Remove(role);

				ply.Role = role;
			}

			Timing.CallDelayed(1f, () =>
			{
				Round.IsLocked = false;
			});

			// I will come back to this later
			/*
			var test = new RoundSummary.SumInfo_ClassList // Still don't know if this does anything
			{
				class_ds = ClassDsToSpawn,
				scientists = ScientistsToSpawn,
				scps_except_zombies = SCPsToSpawn,
				mtf_and_guards = GuardsToSpawn
			};
			RoundSummary.singleton.SetStartClassList(test);*/

			Timing.CallDelayed(3, () =>
			{
				Scp096.TurnedPlayers.Clear();
				Scp173.TurnedPlayers.Clear();
			});
		}

		public void OnWaitingForPlayers()
		{
			Round.IsLocked = true;

			SCPsToSpawn = 0;
			ClassDsToSpawn = 0;
			ScientistsToSpawn = 0;
			GuardsToSpawn = 0;


			Dictionary<RoleType, string> dummiesToSpawn = new Dictionary<RoleType, string>
			{
				{ RoleType.Tutorial, Config.RandomTeamDummy },
				{ RoleType.ClassD, Config.ClassDTeamDummy },
				{ RoleType.Scp173, Config.SCPTeamDummy },
				{ RoleType.Scientist, Config.ScientistTeamDummy },
				{ RoleType.FacilityGuard, Config.MTFTeamDummy },
			};


			GameObject.Find("StartRound").transform.localScale = Vector3.zero;

			if (lobbyTimer.IsRunning)
			{
				Timing.KillCoroutines(lobbyTimer);
			}
			lobbyTimer = Timing.RunCoroutine(LobbyTimer());

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
				boll.Add(pickup);
				GameObject gameObject = pickup.gameObject;
				gameObject.transform.localScale = new Vector3(30f, 0.1f, 30f);
				NetworkServer.UnSpawn(gameObject);
				NetworkServer.Spawn(pickup.gameObject);

				Dummies.Add(pickup.gameObject);

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

		private IEnumerator<float> LobbyTimer()
		{
			StringBuilder message = new StringBuilder();
			var text = $"\n\n\n\n\n\n\n\n\n<b>{Config.DiscordInvite}</b>\n<color=%rainbow%><b>{Config.UpperText}\n{Config.BottomText}</b></color>";
			int x = 0;
			string[] colors = { "#f54242", "#f56042", "#f57e42", "#f59c42", "#f5b942", "#f5d742", "#f5f542", "#d7f542", "#b9f542", "#9cf542", "#7ef542", "#60f542", "#42f542", "#42f560", "#42f57b", "#42f599", "#42f5b6", "#42f5d4", "#42f5f2", "#42ddf5", "#42bcf5", "#429ef5", "#4281f5", "#4263f5", "#4245f5", "#5a42f5", "#7842f5", "#9642f5", "#b342f5", "#d142f5", "#ef42f5", "#f542dd", "#f542c2", "#f542aa", "#f5428d", "#f5426f", "#f54251" };
			while (!Round.IsStarted)
			{
				message.Clear();
				for (int i = 0; i < 0; i++)
				{
					message.Append("\n");
				}

				message.Append($"<size=40><color=yellow><b>{Config.StartingSoonText}, %seconds</b></color></size>");

				short NetworkTimer = GameCore.RoundStart.singleton.NetworkTimer;

				switch (NetworkTimer)
				{
					case -2: message.Replace("%seconds", Config.PausedServer); break;

					case -1: message.Replace("%seconds", Config.RoundStarted); break;

					case 1: message.Replace("%seconds", $"{NetworkTimer} {Config.SecondRemain}"); break;

					case 0: message.Replace("%seconds", Config.RoundStarted); break;

					default: message.Replace("%seconds", $"{NetworkTimer} {Config.SecondsRemain}"); break;
				}

				message.Append($"\n<size=30><i>%players</i></size>");

				if (Player.List.Count() == 1) message.Replace("%players", $"{Player.List.Count()} {Config.PlayerHasConnected}");
				else message.Replace("%players", $"{Player.List.Count()} {Config.PlayersHaveConnected}");

				message.Append(text.Replace("%rainbow%", colors[x++ % colors.Length]));

				foreach (Player ply in Player.List)
				{
					ply.ShowHint(message.ToString(), 1f);

					if (!Config.VotingBroadcast)
						continue;

					if (Vector3.Distance(ply.Position, SCPPoint) <= 3)
					{
						ply.Broadcast(1, $"<i>{Config.SCPTeam}</i>");
					}
					else if (Vector3.Distance(ply.Position, ClassDPoint) <= 3)
					{
						ply.Broadcast(1, $"<i>{Config.ClassDTeam}</i>");
					}
					else if (Vector3.Distance(ply.Position, ScientistPoint) <= 3)
					{
						ply.Broadcast(1, $"<i>{Config.ScientistTeam}</i>");
					}
					else if (Vector3.Distance(ply.Position, GuardPoint) <= 3)
					{
						ply.Broadcast(1, $"<i>{Config.GuardTeam}</i>");
					}
					else
					{
						ply.Broadcast(1, $"<i>{Config.RandomTeam}</i>");
					}
				}
				x++;
				yield return Timing.WaitForSeconds(1f);
			}
		}
	}
}