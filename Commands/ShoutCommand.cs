using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.Chat;
using Terraria.Localization;
using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;

namespace ProximityTextChat.Commands
{
	// Alias
	public class YellCommand : ShoutCommand { public override string Command => "yell"; }
	public class ShoutCommand : ModCommand
	{
		// CommandType.World means that command can be used in Chat in SP and MP, but executes on the Server in MP
		public override CommandType Type
			=> CommandType.World;

		// The desired text to trigger this command
		public override string Command
			=> "shout";

		// A short usage explanation for this command
		public override string Usage
			=> "/shout Message\n/yell Message";

		// A short description of this command
		public override string Description
			=> "Shout to the entire server.";
		public override void Action(CommandCaller caller, string input, string[] args)
		{
			string message = input.Split(" ", 2)[1];

			ChatHelper.BroadcastChatMessageAs((byte)caller.Player.whoAmI, NetworkText.FromLiteral(message), Color.Orange);
			if (Main.dedServ)
				Console.WriteLine("<{0}> {1}", Main.player[caller.Player.whoAmI].name, message);
		}
	}
}
