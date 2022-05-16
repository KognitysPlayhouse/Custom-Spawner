using System;
using System.Collections.Generic;

using Exiled.API.Enums;
using Exiled.API.Features;
using UnityEngine;
using HarmonyLib;

namespace CustomSpawner
{
	public class CustomSpawner : Plugin<Config>
	{
		public EventHandler Handler;
		private Harmony harmony;

		public static CustomSpawner Singleton;

		public override string Name { get; } = "CustomSpawner";
		public override string Author { get; } = "Kognity";
		public override string Prefix { get; } = "CustomSpawner";
		public override Version RequiredExiledVersion { get; } = new Version(5, 2, 0);
		public override Version Version { get; } = new Version(2, 0, 0);

		public override void OnEnabled()
		{
			Singleton = this;
			Handler = new EventHandler(this);
			harmony = new Harmony($"CustomSpawner, {DateTime.UtcNow.Ticks}");
			harmony.PatchAll();

			Exiled.Events.Handlers.Player.Verified += Handler.OnVerified;
			Exiled.Events.Handlers.Server.RoundStarted += Handler.OnRoundStart;
			Exiled.Events.Handlers.Server.RoundEnded += Handler.OnRoundEnd;
			Exiled.Events.Handlers.Server.WaitingForPlayers += Handler.OnWaitingForPlayers;
			Exiled.Events.Handlers.Player.PickingUpItem += Handler.OnPickingUp;

			base.OnEnabled();
		}

		public override void OnDisabled()
		{
			Exiled.Events.Handlers.Player.Verified -= Handler.OnVerified;
			Exiled.Events.Handlers.Server.RoundStarted -= Handler.OnRoundStart;
			Exiled.Events.Handlers.Server.RoundEnded -= Handler.OnRoundEnd;
			Exiled.Events.Handlers.Server.WaitingForPlayers -= Handler.OnWaitingForPlayers;
			Exiled.Events.Handlers.Player.PickingUpItem -= Handler.OnPickingUp;

			harmony?.UnpatchAll(harmony.Id);
			harmony = null;
			Handler = null;
			base.OnDisabled();
		}
	}

	public static class DummiesManager
	{
		public static Dictionary<GameObject, ReferenceHub> dummies = new Dictionary<GameObject, ReferenceHub>();

		public static bool IsDummy(this ReferenceHub hub) => hub.gameObject.IsDummy();
		public static bool IsDummy(this GameObject obj) => dummies.ContainsKey(obj);
	}
}
