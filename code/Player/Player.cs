using Sandbox;
using System.Linq;

namespace LASU 
{
	public partial class LASUPlayer : Player 
	{
		public ClothingContainer Clothing = new();

		public float TimeSinceSpawned;

		public bool IsSpectating;
		public bool CanMove;

		public LASUPlayer() 
		{
		}

		public LASUPlayer(Client cl) : base() 
		{
			Clothing.LoadFromClient(cl);
		}

		public override void Respawn()
		{
			base.Respawn();

			if (!IsSpectating) 
			{
				SetModel("models/citizen/citizen.vmdl");

				Clothing.DressEntity(this);

				EnableAllCollisions = true;

				Animator = new StandardPlayerAnimator();
				Controller = new WalkController();
				CameraMode = new ThirdPersonCamera();

				LASUGame.Instance.PlayersLeft++;

				TimeSinceSpawned = 0.0f;
			}

			if (IsSpectating) 
			{
				SetModel("models/citizen/citizen.vmdl");

				Clothing.DressEntity(this);

				RenderColor = new Color(255, 255, 255, 0.5f);

				EnableAllCollisions = false;

				Animator = new StandardPlayerAnimator();
				Controller = new NoclipController();
				CameraMode = new ThirdPersonCamera();
			}

			CanMove = true;
		}

		public override void OnKilled()
		{
			base.OnKilled();

			LASUGame.Instance.PlayersLeft--;

			var currGameState = LASUGame.Instance.CurrGameState;

			switch (currGameState) // Det finns säkert en bättre väg att göra detta, jag gillar bara att använda switch för de ser coola ut.
			{
				case LASUGame.GameStates.Done: IsSpectating = false; break;
				case LASUGame.GameStates.MapVoting: IsSpectating = false; break;
				case LASUGame.GameStates.Ongoing: IsSpectating = true; break;
				case LASUGame.GameStates.Starting: IsSpectating = false; break;
				case LASUGame.GameStates.WaitingForPlayers: IsSpectating = false; break;
			}
		}

		public override void Simulate( Client cl )
		{
			base.Simulate( cl );

			if (LifeState == LifeState.Alive)
			{
				TimeSinceSpawned += Time.Delta;
			}

			if (CanMove == false) 
			{
				Velocity = 0;
			}

			// Log.Info($"Player {Client.Name}'s Time Since Spawned is: {TimeSinceSpawned}.");
		}
	}
}
