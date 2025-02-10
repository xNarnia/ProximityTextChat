using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.Chat;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria;
using Microsoft.Xna.Framework;
using static ProximityTextChat.ProximityTextChat;

namespace ProximityTextChat.Commands
{
	// Alias
	public class WCommand : WhisperCommand { public override string Command => "w"; }
	public class WhisperCommand : ModCommand
	{
		// CommandType.World means that command can be used in Chat in SP and MP, but executes on the Server in MP
		public override CommandType Type
			=> CommandType.World;

		// The desired text to trigger this command
		public override string Command
			=> "whisper";

		// A short usage explanation for this command
		public override string Usage
			=> "/whisper Message\n/w Message";

		// A short description of this command
		public override string Description
			=> "Whisper to players very close by.";
		public override void Action(CommandCaller caller, string input, string[] args)
		{
			string message = input.Split(" ", 2)[1];
			PTCMod.SendProximityChat(message, 30, 60, (byte)caller.Player.whoAmI, Color.SkyBlue);

			if (Main.dedServ)
				Console.WriteLine("<{0}> {1}", Main.player[caller.Player.whoAmI].name, message);
		}
	}
}
