using Exiled.API.Interfaces;
using System.Collections.Generic;

namespace CustomSpawner
{
	public sealed class Config : IConfig
	{
		public bool IsEnabled { get; set; } = true;

	}
}
