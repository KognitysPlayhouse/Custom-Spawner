using Exiled.API.Interfaces;
using System.Collections.Generic;
using System.ComponentModel;

namespace CustomSpawner
{
	public sealed class Config : IConfig
	{
		[Description("If the plugin is enabled or not.")]
		public bool IsEnabled { get; set; } = true;

		[Description("Upper text shown to the user")]
		public string UpperText { get; set; } = "Welcome to the Server!";

		[Description("Bottom text shown to the user")]
		public string BottomText { get; set; } = "Go stand next to the team you want to play as!";

		[Description("Your discord group invite")]
		public string DiscordInvite { get; set; } = "discord.gg/kognity";

		[Description("If you want a Broadcast to show the class that the player will vote as when they're standing on the circle")]
		public bool VotingBroadcast { get; set; } = true;

		public string StartingSoonText { get; set; } = "The game will be starting soon";
		public string PausedServer { get; set; } = "Server is paused";
		public string RoundStarted{ get; set; } = "Round is being started";
		public string SecondRemain { get; set; } = "second remain";
		public string SecondsRemain { get; set; } = "seconds remain";
		public string PlayerHasConnected { get; set; } = "player has connected";
		public string PlayersHaveConnected { get; set; } = "players have connected";

		public string SCPTeam { get; set; } = "You are voting for SCP team!";
		public string ClassDTeam { get; set; } = "You are voting for Class D team!";
		public string ScientistTeam { get; set; } = "You are voting for Scientist team!";
		public string GuardTeam { get; set; } = "You are voting for Guard team!";
		public string RandomTeam{ get; set; } = "You are voting for random team!";

		public string RandomTeamDummy { get; set; } = "Random Team";
		public string ClassDTeamDummy { get; set; } = "Class D Team";
		public string SCPTeamDummy { get; set; } = "SCP Team";
		public string ScientistTeamDummy { get; set; } = "Scientist Team";
		public string MTFTeamDummy { get; set; } = "MTF Team";
	}	
}
