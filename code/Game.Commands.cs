using Sandbox;

namespace LASU
{
	public partial class LASUGame 
	{
		[ConCmd.Client("lasu_restartround")]
		public static void RestartRound() 
		{
			Instance.SetGameState(GameStates.WaitingForPlayers);
			Instance.ResetPlayerPositions();
		}

		[ConCmd.Client("lasu_forcestartround")]
		public static void ForceStart() 
		{
			if (Instance.CurrGameState == GameStates.WaitingForPlayers)
			{
				Instance.SetGameState(GameStates.Starting);
			}
		}
	}
}
