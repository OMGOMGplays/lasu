using Sandbox.UI;
using Sandbox.UI.Construct;

namespace LASU
{
	public class PlayerAmount : Panel 
	{
		private int PL;
		private int AOP;

		public PlayerAmount() 
		{
			StyleSheet.Load("ui/PlayerAmount.scss");

			Add.Label(PL + " / " + AOP, "playersleft");
		}

		public override void Tick() 
		{
			base.Tick();
		}
	}
}
