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

		private DamageInfo LastDamage;

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

			SetModel("models/citizen/citizen.vmdl");

			Clothing.DressEntity(this);

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

			if (LASUGame.Instance.CurrGameState != LASUGame.GameStates.Done || LASUGame.Instance.CurrGameState != LASUGame.GameStates.MapVoting) 
			{
				CanMove = true;
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

			var controller = GetActiveController();
			if (controller != null) 
			{
				EnableSolidCollisions = !controller.HasTag("noclip");

				SimulateAnimation(controller);
			}			

			// Log.Info($"Player {Client.Name}'s Time Since Spawned is: {TimeSinceSpawned}.");
		}

		public override void FrameSimulate( Client cl )
		{
			base.FrameSimulate( cl );

			Camera.Rotation = ViewAngles.ToRotation();

			Camera.FieldOfView = Screen.CreateVerticalFieldOfView( 70 );
			Camera.FirstPersonViewer = null;

			Vector3 targetPos;
			var center = Position + Vector3.Up * 64;

			var pos = center;
			var rot = Rotation.FromAxis( Vector3.Up, -16 ) * Camera.Rotation;

			float distance = 130.0f * Scale;
			targetPos = pos + rot.Right * ((CollisionBounds.Mins.x + 32) * Scale);
			targetPos += rot.Forward * -distance;

			var tr = Trace.Ray( pos, targetPos )
				.WithAnyTags( "solid" )
				.Ignore( this )
				.Radius( 8 )
				.Run();

			Camera.Position = tr.EndPosition;
		}

		void SimulateAnimation( PawnController controller )
		{
			if ( controller == null )
				return;

			// where should we be rotated to
			var turnSpeed = 0.02f;

			Rotation rotation;

			// If we're a bot, spin us around 180 degrees.
			if ( Client.IsBot )
				rotation = ViewAngles.WithYaw( ViewAngles.yaw + 180f ).ToRotation();
			else
				rotation = ViewAngles.ToRotation();

			var idealRotation = Rotation.LookAt( rotation.Forward.WithZ( 0 ), Vector3.Up );
			Rotation = Rotation.Slerp( Rotation, idealRotation, controller.WishVelocity.Length * Time.Delta * turnSpeed );
			Rotation = Rotation.Clamp( idealRotation, 45.0f, out var shuffle ); // lock facing to within 45 degrees of look direction

			CitizenAnimationHelper animHelper = new CitizenAnimationHelper( this );

			animHelper.WithWishVelocity( controller.WishVelocity );
			animHelper.WithVelocity( controller.Velocity );
			animHelper.WithLookAt( EyePosition + EyeRotation.Forward * 100.0f, 1.0f, 1.0f, 0.5f );
			animHelper.AimAngle = rotation;
			animHelper.FootShuffle = shuffle;
			animHelper.DuckLevel = MathX.Lerp( animHelper.DuckLevel, controller.HasTag( "ducked" ) ? 1 : 0, Time.Delta * 10.0f );
			animHelper.VoiceLevel = ( Host.IsClient && Client.IsValid() ) ? Client.TimeSinceLastVoice < 0.5f ? Client.VoiceLevel : 0.0f : 0.0f;
			animHelper.IsGrounded = GroundEntity != null;
			animHelper.IsSitting = controller.HasTag( "sitting" );
			animHelper.IsNoclipping = controller.HasTag( "noclip" );
			animHelper.IsClimbing = controller.HasTag( "climbing" );
			animHelper.IsSwimming = this.GetWaterLevel() >= 0.5f;
			animHelper.IsWeaponLowered = false;

			if ( controller.HasEvent( "jump" ) ) animHelper.TriggerJump();

			if ( ActiveChild is BaseCarriable carry )
			{
				carry.SimulateAnimator( animHelper );
			}
			else
			{
				animHelper.HoldType = CitizenAnimationHelper.HoldTypes.None;
				animHelper.AimBodyWeight = 0.5f;
			}
		}
	}
}
