using Sandbox;
using System;
using System.Linq;

namespace LASU
{
	public partial class LASUGame : Game 
	{
		public TimeSince TimeSinceRoundEnded;

		public GameStates CurrGameState {get; set;} = GameStates.WaitingForPlayers;

		public static float TimeUntilStartOrigin = 15.0f; // Kommer detta vara användbart? Kanske ifall jag lyckas få inställningarna fungera.

		public float TimeUntilStart = 15.0f;
		public float TimeUntilSwitchToMapVote = 45.0f;
		public float TimeUntilSwitchMap = 50.0f;

		public TimeSince TimeSinceAddedRound;

		public int CurrentRound = 0;
		public int MaxRound = 3;

		public int AmountOfPlayers;
		public int MinAmountOfPlayers = 2;
		public int PlayersLeft;

		public static LASUGame Instance => Current as LASUGame;

		public LASUGame() 
		{
			if (IsClient) 
			{
				_ = new LASUHud();
			}
		}

		public override void ClientJoined( Client cl )
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

		public override void ClientDisconnect( Client cl, NetworkDisconnectionReason reason )
		{
			base.ClientDisconnect( cl, reason );

			AmountOfPlayers--;

			Log.Info($"Player {cl.Name} has disconnected! Reason for disconnect: {reason}. Current amount of players is now: {AmountOfPlayers}!");
		}

		public override void Simulate( Client cl )
		{
			base.Simulate( cl );

			// En jävlar massa if's, men det borde ändå fungera!!

			if (CurrGameState == GameStates.WaitingForPlayers) 
			{
				CheckMinPlayerReached();
			}

			if (CheckMinPlayerReached() == true && CurrGameState == GameStates.WaitingForPlayers) 
			{
				SetGameState(GameStates.Starting);
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
				if (PlayersLeft == 0 || PlayersLeft == 1) 
				{
					if (CurrentRound < MaxRound) 
					{
						StartRound();

						Log.Info($"Current round is now: {CurrentRound}. Match is not done.");
					}
					else if (CurrentRound >= MaxRound) 
					{
						SetGameState(GameStates.Done);

						Log.Info($"Current round is now: {CurrentRound}. Match is done.");
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

				ShowScoreboard();

				if (TimeUntilSwitchToMapVote <= 0.0f) 
				{
					CloseScoreboard();
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
					// Ändra banan
					return;
				}

				OpenMapSwitchMenu();
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
				Log.Error($"The current game state is already {nextGS}! Stopping change!");
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
			ResetPlayerPositions();

			foreach (var physicsProp in All.OfType<LASUPhysicsEntity>()) 
			{
				physicsProp.Reset();
			}

			if (TimeSinceAddedRound >= 4.0f)
			{
				AddRound();
			}
		}

		public void AddRound() 
		{
			TimeSinceAddedRound = 0;

			CurrentRound++;

			return;
		}

		public void ResetPlayerPositions() 
		{
			foreach (var player in All.OfType<LASUPlayer>()) 
			{
				var spawnpoint = All.OfType<SpawnPoint>();
				var randomSpawnPoint = spawnpoint.OrderBy(x => Guid.NewGuid()).FirstOrDefault();

				player.IsSpectating = false;
				player.Transform = randomSpawnPoint.Transform;

				player.Velocity = 0;
			}
		}

		public void ShowScoreboard() 
		{
			// Gör saker här när det behövs.

			return;
		}

		public void CloseScoreboard() 
		{
			// Samma här.

			return;
		}

		public void OpenMapSwitchMenu() 
		{
			// Du vet vad som gäller här också.

			return;
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
