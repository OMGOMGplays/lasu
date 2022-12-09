using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;

namespace LASU 
{
	public partial class RoundCounter : Panel 
	{
		LASUGame game;
		int Round;

		public RoundCounter() 
		{
			StyleSheet.Load("ui/roundcounter/RoundCounter.scss");

			UpdateRoundCounter();
		}

		public void UpdateRoundCounter()
	    {
    	    game.GetCurrentRound(Round); // Set the round number here
        	Add.Label("Round " + Round, "counter");
	    }
	}
}
