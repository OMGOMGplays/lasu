using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;

namespace LASU 
{
	public partial class RoundCounter : Panel 
	{
		private LASUGame game = (LASUGame)GameManager.Current;
		
		private int Round;

		public RoundCounter() 
		{
			StyleSheet.Load("ui/RoundCounter.scss");

			Round = game.CurrentRound;
			Add.Label("Round " + Round, "rounds"); // MaxRound is 4, but I want the UI to say the game ends at round 3.
		}

		public override void Tick()
		{
			base.Tick();

			if (LASUGame.Instance.CurrGameState != LASUGame.GameStates.Ongoing) 
			{
				SetClass("hidden", true);
			}
			else 
			{
				SetClass("hidden", false);
			}

			// Round = game.GetCurrentRound(Round);
		}
	}
}
