using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.Chat;
using Terraria;
using Terraria.ModLoader;
using Terraria.Localization;
using Terraria.Net;
using Microsoft.Xna.Framework;
using Microsoft.VisualBasic;
using Terraria.GameContent.NetModules;
using System.ComponentModel.Design;
using Ionic.Zlib;
using Terraria.Chat.Commands;
using static System.Net.Mime.MediaTypeNames;
using Terraria.UI.Chat;

namespace ProximityTextChat
{
	// Please read https://github.com/tModLoader/tModLoader/wiki/Basic-tModLoader-Modding-Guide#mod-skeleton-contents for more information about the various files in a mod.
	public class ProximityTextChat : Mod
	{
		public static ProximityTextChat PTCMod { get; set; }

		public override void Load()
		{
			base.Load();
			PTCMod = this;
			On_SayChatCommand.ProcessIncomingMessage += On_SayChatCommand_ProcessIncomingMessage;
			On_NetMessage.greetPlayer += NetMessage_greetPlayer;
		}

		public override void Unload()
		{
			base.Unload();
			On_SayChatCommand.ProcessIncomingMessage -= On_SayChatCommand_ProcessIncomingMessage;
			On_NetMessage.greetPlayer -= NetMessage_greetPlayer;
		}

		private void On_SayChatCommand_ProcessIncomingMessage(On_SayChatCommand.orig_ProcessIncomingMessage orig, SayChatCommand self, string text, byte clientId)
		{
			SendProximityChat(text, 500, 3000, clientId);

			if (Main.dedServ)
				Console.WriteLine("<{0}> {1}", Main.player[clientId].name, text);
		}

		private void NetMessage_greetPlayer(On_NetMessage.orig_greetPlayer orig, int plr)
		{
			NetPacket packet = NetTextModule.SerializeServerMessage(NetworkText.FromLiteral("[ProximityTextChat]\n  /shout /whisper"), Color.SkyBlue, byte.MaxValue);
			NetManager.Instance.SendToClient(packet, plr);
			orig(plr);
		}

		/// <summary>
		/// Sends a chat message to players within the designated proximity.
		/// </summary>
		/// <param name="message">The text sent by the player.</param>
		/// <param name="innerRadius">The initial distance to send the message at full strength (brightest color).</param>
		/// <param name="outerRadius">The maximum distance the chat message can be heard.</param>
		/// <param name="playerIdWhoSent">The id of the player who sent the message.</param>
		/// <param name="deadzone">The radius from the player in which to not broadcast the message.</param>
		public void SendProximityChat(string message, float innerRadius, float outerRadius, int playerIdWhoSent, Color? messageColor = null, float deadzone = 0)
		{
			if (innerRadius > outerRadius)
				throw new ArgumentException("innerRadius can not be larger than outerRadius!");

			Color color;
			if (messageColor == null)
				color = Color.White;
			else
				color = messageColor.Value;

			for (int i = 0; i < 256; i++)
			{
				// Send to all players except the player who sent
				if (i != playerIdWhoSent && Main.player[i].name != "")
				{
					var distance = Vector2.Distance(Main.player[i].position, Main.player[playerIdWhoSent].position);
					
					if(innerRadius == outerRadius) // If the inner and outer radius are the same, it's just a radius.
					{
						if (innerRadius < distance)
							ChatHelper.SendChatMessageToClientAs((byte)playerIdWhoSent, NetworkText.FromLiteral(message), color, i);
					}
					else if (innerRadius < distance && distance < outerRadius)
					{
						var distanceAsPercent = (distance - innerRadius) / (outerRadius - innerRadius);
						// Reuse distance as a distanceMultiplier to dim the text as it gets closer to outerRadius
						// Don't dim too much though! 75% dim is enough.
						distance = 1 - (float)Math.Min(distanceAsPercent, 0.8);

						ChatHelper.SendChatMessageToClientAs((byte)playerIdWhoSent, NetworkText.FromLiteral(message), color * distance, i);
					}
					else if (distance <= innerRadius)
					{
						ChatHelper.SendChatMessageToClientAs((byte)playerIdWhoSent, NetworkText.FromLiteral(message), color, i);
					}
				}
				else if (i == playerIdWhoSent) // Player who sent will always be 0 distance, so just send the message
				{
					ChatHelper.SendChatMessageToClientAs((byte)playerIdWhoSent, NetworkText.FromLiteral(message), color, playerIdWhoSent);
				}
			}
		}
	}
}
