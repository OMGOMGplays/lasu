using Sandbox;

namespace LASU
{
	public partial class LASUGame 
	{
		[ConCmd.Client("lasu_restartround")]
		public static void RestartRound() 
		{
			Instance.SetGameState(GameStates.WaitingForPlayers);
			Instance.ResetPlayers();
		}

		[ConCmd.Client("lasu_forcestartround")]
		public static void ForceStart() 
		{
			if (Instance.CurrGameState == GameStates.WaitingForPlayers)
			{
				Instance.SetGameState(GameStates.Starting);
			}
		}

		[ConCmd.Client("lasu_resetprops")]
		public static void ResetPropsCommand() 
		{
			Instance.ResetProps();
		}
	}
}
