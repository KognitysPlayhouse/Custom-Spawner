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
	}	
}
