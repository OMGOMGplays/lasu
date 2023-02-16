namespace LASU.UI
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

			Round = game.CurrentRound;
			RoundText.Text = $"Round {Round}";
		}
	}
}
