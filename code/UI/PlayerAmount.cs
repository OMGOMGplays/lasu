using Sandbox.UI;
using Sandbox.UI.Construct;

namespace LASU
{
	public class PlayerAmount : Panel 
	{
		private int PL;
		private int AOP;

		public Label Players;

		public PlayerAmount() 
		{
			StyleSheet.Load("ui/PlayerAmount.scss");

			Players = Add.Label("0/0", "playersleft");
		}

		public override void Tick() 
		{
			base.Tick();

			var game = LASUGame.Instance;

			PL = game.PlayersLeft;
			AOP = game.AmountOfPlayers;

			Players.Text = $"{PL}/{AOP}";

			if (game.CurrGameState == LASUGame.GameStates.Done) 
			{
				SetClass("hidden", true);
			}
		}
	}
}
