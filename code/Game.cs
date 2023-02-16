using LASU.UI;

namespace LASU
{
	public partial class LASUGame : GameManager
	{
		// GameStates
		[Net] public GameStates CurrGameState {get; set;} = GameStates.WaitingForPlayers;

		// TimeUntil
		private const float TimeUntilStartOrigin = 15.0f; // Kommer detta vara användbart? Kanske ifall jag lyckas få inställningarna fungera.
		[Net] public float TimeUntilStart {get; set;} = TimeUntilStartOrigin;
		[Net] public float TimeUntilSwitchToMapVote {get; set;} = 45.0f;
		[Net] public float TimeUntilSwitchMap {get; set;} = 50.0f;

		// TimeSince
		public TimeSince TimeSinceAddedRound;
		public TimeSince TimeSinceResetPlayers;
		public TimeSince TimeSinceRoundEnded;

		// Rounds
		private const int MaxRoundOrigin = 4;
		public int MaxRound = MaxRoundOrigin;
		[Net] public int CurrentRound {get; set;} = 0;

		// Players
		private int MinAmountOfPlayers = 2;
		[Net] public int AmountOfPlayers {get; set;}
		[Net] public int PlayersLeft {get; set;}

		// Misc.
		public static LASUGame Instance => Current as LASUGame;

		public string CurrMap {get; set;}
		public string MapIdent {get; set;}

		public LASUGame() 
		{
			if (Game.IsClient) 
			{
				new LASUHud();
			}
			if (Game.IsServer) 
			{
				InitMapCycle();
			}
		}

		public override void ClientJoined( IClient cl )
		{
			base.ClientJoined( cl );

			var pawn = new LASUPlayer(cl);
			pawn.Respawn();

			cl.Pawn = pawn;

			if (CurrGameState != GameStates.WaitingForPlayers || CurrGameState != GameStates.Starting) 
			{
				pawn.IsSpectating = true;
			}

			AmountOfPlayers++;

			Log.Info($"Player {cl.Name} has joined! Current amount of players is now: {AmountOfPlayers}!");
		}

		public override void ClientDisconnect( IClient cl, NetworkDisconnectionReason reason )
		{
			base.ClientDisconnect( cl, reason );

			AmountOfPlayers--;

			Log.Info($"Player {cl.Name} has disconnected! Reason for disconnect: {reason}. Current amount of players is now: {AmountOfPlayers}!");
		}

		public override void Simulate( IClient cl )
		{
			base.Simulate( cl );

			// En jävlar massa if's, men det borde ändå fungera!!

			if (CurrGameState == GameStates.WaitingForPlayers) 
			{
				CheckMinPlayerReached();
			}

			if (CheckMinPlayerReached() == true) 
			{
				if (CurrGameState == GameStates.WaitingForPlayers)
				{
					SetGameState(GameStates.Starting);
				}
			}

			if (CurrGameState == GameStates.Starting) 
			{
				// Log.Info($"Starting the game in {TimeUntilStart} seconds!");

				TimeUntilStart -= Time.Delta; // Fungerar detta??? - Ja!!!

				if (TimeUntilStart <= 0.0f) 
				{
					SetGameState(GameStates.Ongoing);
					StartRound();
				}
			}

			if (CurrGameState == GameStates.Ongoing) 
			{
				if (PlayersLeft <= 1) 
				{
					if (CurrentRound < MaxRound) 
					{
						StartRound();
					}
					else if (CurrentRound >= MaxRound) 
					{
						SetGameState(GameStates.Done);
					}
				}
			}

			if (CurrGameState == GameStates.Done && CurrentRound >= MaxRound) 
			{
				TimeUntilSwitchToMapVote -= Time.Delta;

				foreach (var player in All.OfType<LASUPlayer>()) 
				{
					player.CanMove = false;
				}

				// Log.Info($"Switching to map vote in {TimeUntilSwitchToMapVote} seconds!");

				if (TimeUntilSwitchToMapVote <= 0.0f) 
				{
					SetGameState(GameStates.MapVoting);
				}
			}

			if (CurrGameState == GameStates.MapVoting) 
			{
				if (TimeUntilSwitchMap > 0.0f)
				{
					TimeUntilSwitchMap -= Time.Delta;

					// Log.Info($"In {TimeUntilSwitchMap} seconds the map will switch!");
				}

				if (TimeUntilSwitchMap <= 0.0f) 
				{
					if (NextMap != null)
					{
						ChangeMap(NextMap);
						return;
					}
					else 
					{
						Log.Error("Vote on a map!");
						TimeUntilSwitchMap = 50.0f;
					}
				}
			}

			if (!CheckMinPlayerReached()) 
			{
				if (CurrGameState == GameStates.Ongoing || CurrGameState == GameStates.Starting)
				{
					Log.Error("Not enough players to start / continue round! Canceling / restarting!");
					Log.Info($"Time until start has been reset to: {TimeUntilStartOrigin}");
					TimeUntilStart = TimeUntilStartOrigin;
					SetGameState(GameStates.WaitingForPlayers);
				}
			} 
		}

		public void SetGameState(GameStates nextGS) 
		{
			if (CurrGameState == nextGS) 
			{
				Log.Error($"The current game state is already {nextGS}!");
				return;
			}

			Log.Info($"The game state will now switch to: {nextGS}.");
			CurrGameState = nextGS;
			return;
		}

		public bool CheckMinPlayerReached() 
		{
			if (AmountOfPlayers >= MinAmountOfPlayers) return true;

			return false;
		}

		public void StartRound() 
		{
			ResetPlayers();

			ResetProps();

			if (TimeSinceAddedRound >= 4.0f)
			{
				AddRound();
				Log.Info($"Round Started! The Current Round Is: {CurrentRound}!");
			}
		}

		public void AddRound() 
		{
			TimeSinceAddedRound = 0;

			CurrentRound++;

			return;
		}

		public void ResetProps() 
		{
			foreach (var physicsProp in All.OfType<LASUPhysicsEntity>()) 
			{
				physicsProp.Reset();
			}
		}

		public void ResetPlayers() 
		{
			foreach (var player in All.OfType<LASUPlayer>()) 
			{
				var spawnPoint = All
					.OfType<SpawnPoint>()
					.OrderBy(x => Guid.NewGuid())
					.FirstOrDefault();

				player.Transform = spawnPoint.Transform;
				player.IsSpectating = false;
				player.Velocity = 0;
				
				TimeSinceResetPlayers = 0;
			}
		}

		public enum GameStates 
		{
			WaitingForPlayers = 0,
			Starting,
			Ongoing,
			Done,
			MapVoting
		}
	}
}
