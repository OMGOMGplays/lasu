using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;

namespace LASU 
{
	public partial class RoundCounter : Panel 
	{	
		private int Round;

		public Label RoundText;

		public RoundCounter() 
		{
			StyleSheet.Load("ui/RoundCounter.scss");

			RoundText = Add.Label("Round 0", "rounds");
		}

		public override void Tick()
		{
			base.Tick();

			var game = LASUGame.Instance;

			if (game.CurrGameState != LASUGame.GameStates.Ongoing) 
			{
				SetClass("hidden", true);
			}
			else 
			{
				SetClass("hidden", false);
			}

			Round = game.CurrentRound;
			RoundText.Text = $"Round {Round}";
		}
	}
}
