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

		public override string Name { get; } = "CustomSpawner";
		public override string Author { get; } = "Kognity";
		public override string Prefix { get; } = "CustomSpawner";
		public override Version RequiredExiledVersion { get; } = new Version(2, 8, 0);
		public override Version Version { get; } = new Version(1, 1, 0);

		public override void OnEnabled()
		{
			Handler = new EventHandler(this);

			Exiled.Events.Handlers.Player.Verified += Handler.OnVerified;
			Exiled.Events.Handlers.Server.RoundStarted += Handler.OnRoundStart;
			Exiled.Events.Handlers.Server.WaitingForPlayers += Handler.OnWaitingForPlayers;
			base.OnEnabled();
		}

		public override void OnDisabled()
		{
			Exiled.Events.Handlers.Player.Verified -= Handler.OnVerified;
			Exiled.Events.Handlers.Server.RoundStarted -= Handler.OnRoundStart;
			Exiled.Events.Handlers.Server.WaitingForPlayers -= Handler.OnWaitingForPlayers;

			Handler = null;
			base.OnDisabled();
		}
	}
}
