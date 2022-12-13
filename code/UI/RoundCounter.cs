using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;

namespace LASU 
{
	public partial class RoundCounter : Panel 
	{
		LASUGame game = (LASUGame)GameManager.Current;
		
		int Round;

		public RoundCounter() 
		{
			StyleSheet.Load("ui/RoundCounter.scss");

			UpdateRoundCounter();
		}

		public void UpdateRoundCounter()
	    {
    	    game.GetCurrentRound(Round); // Set the round number here
        	Add.Label("Round " + Round, "counter");
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
