using Sandbox;
using SandboxEditor;
using System.ComponentModel.DataAnnotations;

namespace LASU
{
	[HammerEntity]
	[Model]
	[Display(Name = "LASU Physics Entity")]
	[Library("lasu_physics_entity")]
	public partial class LASUPhysicsEntity : ModelEntity 
	{
		public Vector3 SpawnPosition {get; set;}
		public Rotation SpawnRotation {get; set;}

		public TimeSince TimeSinceLastReset;

		public override void Spawn()
		{
			base.Spawn();

			PhysicsEnabled = true;
			UsePhysicsCollision = true;
			EnableHideInFirstPerson = true;
			EnableShadowInFirstPerson = true;
			Tags.Add( "prop", "solid" );

			SetupPhysics();

			SpawnPosition = Position;
			SpawnRotation = Rotation;
		}

		private void SetupPhysics()
		{
			var physics = SetupPhysicsFromModel( PhysicsMotionType.Dynamic );
			if ( !physics.IsValid() )
				return;
		}

		public void Reset() 
		{
			PhysicsEnabled = false;

			if (TimeSinceLastReset >= 2.5f) 
			{
				PhysicsEnabled = true;
			}

			TimeSinceLastReset = 0;

			Position = SpawnPosition;
			Rotation = SpawnRotation;
		}
	}
}
