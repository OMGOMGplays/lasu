using Sandbox;
using Editor;
using System.ComponentModel.DataAnnotations;

namespace LASU
{
	[Display(Name = "Prop Launcher"), Icon("eject")]
	[HammerEntity]
	[DrawAngles]
	[Model]
	[Library("lasu_proplauncher")]
	public partial class PropLauncher : ModelEntity
	{
		[Property, Display(Name = "Launch Speed", GroupName = "General", Description = "The speed of which the launcher should shoot its models Recommended speed is the default speed")] // Beskrivningen ser lite konstigt ut utan punkter, men ser bättre ut i Hammer.
		public float LaunchSpeed {get; set;} = 4000.0f;

		// En jälvar lång lista, men jag tror nog inte att jag kan göra det något bättre.

		[Property, Display(Name = "Launchable Models 1", GroupName = "Models", Description = "One of the models that this launcher can launch."), ResourceType("vmdl")]
		public string Model1 {get; set;} = "models/sbox_props/concrete_barrier/concrete_barrier.vmdl";

		[Property, Display(Name = "Launchable Models 2", GroupName = "Models", Description = "One of the models that this launcher can launch."), ResourceType("vmdl")]
		public string Model2 {get; set;} = "models/sbox_props/concrete_barrier/concrete_barrier.vmdl";

		[Property, Display(Name = "Launchable Models 3", GroupName = "Models", Description = "One of the models that this launcher can launch."), ResourceType("vmdl")]
		public string Model3 {get; set;} = "models/sbox_props/concrete_barrier/concrete_barrier.vmdl";

		[Property, Display(Name = "Launchable Models 4", GroupName = "Models", Description = "One of the models that this launcher can launch."), ResourceType("vmdl")]
		public string Model4 {get; set;} = "models/sbox_props/concrete_barrier/concrete_barrier.vmdl";

		[Property, Display(Name = "Launchable Models 5", GroupName = "Models", Description = "One of the models that this launcher can launch."), ResourceType("vmdl")]
		public string Model5 {get; set;} = "models/sbox_props/concrete_barrier/concrete_barrier.vmdl";

		[Property, Display(Name = "Launchable Models 6", GroupName = "Models", Description = "One of the models that this launcher can launch."), ResourceType("vmdl")]
		public string Model6 {get; set;} = "models/sbox_props/concrete_barrier/concrete_barrier.vmdl";

		public TimeSince TimeSinceLastLaunch;

		public int ObjectsLaunched;

		public override void Spawn()
		{
			base.Spawn();
		}

		[Event.Tick.Server]
		public void LauncherSimulate()
		{
			var randomTime = Game.Random.Float(7.0f, 15.0f);

			if (LASUGame.Instance.CurrGameState == LASUGame.GameStates.Ongoing)
			{
				if (TimeSinceLastLaunch >= randomTime) 
				{
					TimeSinceLastLaunch = 0.0f;
					// randomTime = Rand.Float(5.0f, 15.0f);

					LaunchModel();
				}
			}

			if (LASUGame.Instance.CurrGameState != LASUGame.GameStates.Ongoing) 
			{
				TimeSinceLastLaunch = 0;
			}

			// Log.Info($"Entity: {Name}'s TimeSinceLastLaunch is: {TimeSinceLastLaunch}.");
		}

		public void LaunchModel() 
		{
			var modelToLaunchRand = Game.Random.Int(1, 6);
			string modelToLaunch = modelToLaunchRand switch
			{
				1 => Model1,
				2 => Model2,
				3 => Model3,
				4 => Model4,
				5 => Model5,
				6 => Model6,
				_ => Model1,
			};

			Log.Info($"Launching {modelToLaunch}!");

			var model = new ModelEntity();
			model.SetModel(modelToLaunch);
			model.Position = Position + Rotation.Forward * 15;
			model.Rotation = Rotation.LookAt(Vector3.Random.Normal);
			model.SetupPhysicsFromModel(PhysicsMotionType.Dynamic, false);
			model.PhysicsGroup.Velocity = Rotation.Forward * LaunchSpeed;

			if (ObjectsLaunched < 2) 
			{
				ObjectsLaunched++;
			}
			else if (ObjectsLaunched >= 2) 
			{
				var lastModel = model;
				lastModel.Delete();

				ObjectsLaunched = 0;
			}
		}
	}
}
