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

			Exiled.Events.Handlers.Player.Verified += Handler.OnVerified;
			Exiled.Events.Handlers.Server.RoundStarted += Handler.OnRoundStart;
			Exiled.Events.Handlers.Server.WaitingForPlayers += Handler.OnWaitingForPlayers;
			Exiled.Events.Handlers.Player.PickingUpItem += Handler.OnPickingUp;

			base.OnEnabled();
		}

		public override void OnDisabled()
		{
			Exiled.Events.Handlers.Player.Verified -= Handler.OnVerified;
			Exiled.Events.Handlers.Server.RoundStarted -= Handler.OnRoundStart;
			Exiled.Events.Handlers.Server.WaitingForPlayers -= Handler.OnWaitingForPlayers;
			Exiled.Events.Handlers.Player.PickingUpItem -= Handler.OnPickingUp;

			Handler = null;
			base.OnDisabled();
		}
	}
}
