namespace LASU
{
	public partial class LASUGame 
	{
		// Server Settings

		[ConVar.Replicated("lasu_setstartorigin")]
		public static float SetStartingOrigin {get; set;} = TimeUntilStartOrigin;

		[ConVar.Replicated("lasu_roundsuntilover")]
		public static int SetRoundsTillOver {get; set;} = MaxRoundOrigin;
	}
}
