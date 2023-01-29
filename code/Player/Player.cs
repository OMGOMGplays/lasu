using Sandbox;
using System.Linq;

namespace LASU 
{
	public partial class LASUPlayer : Player 
	{
		public float TimeSinceSpawned;

		public bool IsSpectating;
		public bool CanMove;

		private DamageInfo LastDamage;

		private ClothingContainer Clothing = new();

		public LASUPlayer() 
		{
		}
		
		public LASUPlayer(IClient cl) : base() 
		{
			Clothing.LoadFromClient(cl);
		}

		public override void Respawn()
		{
			base.Respawn();

			SetModel("models/citizen/citizen.vmdl");

			Clothing.DressEntity(this);

			CanMove = true;

			if (!IsSpectating) 
			{
				EnableAllCollisions = true;
				EnableDrawing = true;

				Controller = new WalkController();

				LASUGame.Instance.PlayersLeft++;

				TimeSinceSpawned = 0.0f;
			}

			if (IsSpectating) 
			{
				RenderColor = new Color(255, 255, 255, 0.5f);

				EnableAllCollisions = false;
				EnableDrawing = true;

				Controller = new NoclipController();
			}
		}

		public override void OnKilled()
		{
			base.OnKilled();

			LASUGame.Instance.PlayersLeft--;

			BecomeRagdollOnClient( Velocity, LastDamage.Position, LastDamage.Force, LastDamage.BoneIndex, LastDamage.HasTag( "bullet" ), LastDamage.HasTag( "blast" ) );

			Controller = null;

			EnableAllCollisions = false;
			EnableDrawing = false;

			var currGameState = LASUGame.Instance.CurrGameState;

			if (currGameState == LASUGame.GameStates.Ongoing) 
			{
				IsSpectating = true;
			}
			else 
			{
				IsSpectating = false;
			}
		}

		public override void Simulate( IClient cl )
		{
			base.Simulate( cl );

			if (LifeState == LifeState.Alive)
			{
				TimeSinceSpawned += Time.Delta;
			}

			var controller = GetActiveController();
			if (controller != null) 
			{
				EnableSolidCollisions = !controller.HasTag("noclip");

				SimulateAnimation(controller);
			}	

			// if (LASUGame.Instance.CurrGameState == LASUGame.GameStates.Done) 
			// {
			// 	if (!IsSpectating) 
			// 	{
			// 		IsSpectating = true;
			// 		Respawn();
			// 	}
			// }	
		}
	}
}
