namespace LASU 
{
	public partial class LASUPlayer 
	{
		public override void FrameSimulate( IClient cl )
		{
			base.FrameSimulate( cl );

			Camera.Rotation = ViewAngles.ToRotation();

			Camera.FieldOfView = Screen.CreateVerticalFieldOfView( 70 );
			Camera.FirstPersonViewer = null;

			Vector3 targetPos;
			var center = Position + Vector3.Up * 64;

			var pos = center;
			var rot = Camera.Rotation * Rotation.FromAxis( Vector3.Up, -16 );

			float distance = 130.0f * Scale;
			targetPos = pos + rot.Right * ((CollisionBounds.Mins.x + 48) * Scale);
			targetPos += rot.Forward * -distance;

			var tr = Trace.Ray( pos, targetPos )
				.WithAnyTags( "solid" )
				.Ignore( this )
				.Radius( 8 )
				.Run();

			Camera.Position = tr.EndPosition;

			if ( LifeState != LifeState.Alive && Corpse.IsValid() )
			{
				Corpse.EnableDrawing = true;

				var dpos = Corpse.GetBoneTransform( 0 ).Position + Vector3.Up * 10;
				var dtargetPos = dpos + Camera.Rotation.Backward * 100;

				var dtr = Trace.Ray( dpos, dtargetPos )
					.WithAnyTags( "solid" )
					.Ignore( this )
					.Radius( 8 )
					.Run();

				Camera.Position = dtr.EndPosition;
				Camera.FirstPersonViewer = null;
			}
		}
	}
}
