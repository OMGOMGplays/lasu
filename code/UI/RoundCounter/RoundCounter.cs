using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;

namespace LASU 
{
	public partial class RoundCounter : Panel 
	{
		public RoundCounter() 
		{
			StyleSheet.Load("ui/roundcounter/RoundCounter.scss");

			Add.Label("Round " + LASUGame.Instance.CurrentRound, "counter");
		}
	}
}
