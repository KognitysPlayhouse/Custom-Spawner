using Exiled.API.Interfaces;
using System.Collections.Generic;
using System.ComponentModel;

namespace CustomSpawner
{
	public sealed class Config : IConfig
	{
		[Description("If the plugin is enabled or not.")]
		public bool IsEnabled { get; set; } = true;

		[Description("The Spawn Queue (4: Class D, 0: SCP, 1: Guard, 3: Scientist) IMPORTANT THE LENGTH MATCHES YOUR MAX PLAYER COUNT!")]
		public List<char> SpawnQueue { get; set; } = new List<char>{ '4', '0', '1', '4', '3', '1', '4', '0', '3', '1', '4', '4', '1', '4', '0', '4', '1', '3', '4', '0', '3', '1', '4', '4', '1', '4', '0', '4', '1', '3', '0', '4', '4', '1', '0', '1', '4', '3', '3', '1' };

		[Description("Upper text shown to the user")]
		public string UpperText { get; set; } = "Welcome to the Server!";

		[Description("Bottom text shown to the user")]
		public string BottomText { get; set; } = "Go stand next to the team you want to play as!";

		[Description("Your discord group invite")]
		public string DiscordInvite { get; set; } = "discord.gg/kognity";
	}	
}
