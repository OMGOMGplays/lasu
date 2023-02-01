using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;
using System.Linq;
using System.Collections.Generic;

namespace LASU.UI
{
	public partial class LASUScoreboard<T> : Panel where T : LASUScoreboardEntry, new()
	{
		public Panel Canvas {get; protected set;}
		Dictionary<IClient, T> Rows = new ();

		public Panel Header {get; protected set;}

		public LASUScoreboard() 
		{
			StyleSheet.Load("/ui/scoreboard/LASUScoreboard.scss");
			AddClass("scoreboard");

			AddHeader();

			Canvas = Add.Panel("canvas");
		}

		public override void Tick()
		{
			base.Tick();

			SetClass("open", ShouldBeOpen());

			if (!IsVisible)
				return;

			foreach (var client in Game.Clients.Except(Rows.Keys)) 
			{
				var entry = AddClient(client);
				Rows[client] = entry;
			}

			foreach (var client in Rows.Keys.Except(Game.Clients)) 
			{
				if (Rows.TryGetValue(client, out var row)) 
				{
					row?.Delete();
					Rows.Remove(client);
				}
			}
		}

		public virtual bool ShouldBeOpen() 
		{
			if (LASUGame.Instance.CurrGameState == LASUGame.GameStates.Done) 
			{
				return true;
			}

			if (Input.Down(InputButton.Score)) return true;

			return false;
		}

		protected virtual void AddHeader() 
		{
			Header = Add.Panel("header");
			Header.Add.Label("Name", "name");
			Header.Add.Label("Deaths", "deaths");
			Header.Add.Label("Ping", "ping");
		}

		protected virtual T AddClient(IClient entry) 
		{
			var p = Canvas.AddChild<T>();
			p.Client = entry;
			return p;
		}
	}
}
