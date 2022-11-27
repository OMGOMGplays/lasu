using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;

namespace LASU 
{
	public partial class LASUHud : HudEntity<RootPanel> 
	{
		private LASUHud Instance {get; set;}

		public LASUHud() 
		{
			if (Instance != null) 
			{
				Instance.Delete();
				Instance = null;
			}

			Instance = this;
		}

		public override void CreateRootPanel()
		{
			RootPanel?.Delete();

			base.CreateRootPanel();
			CreateHudElements();
		}

		public void CreateHudElements() 
		{
			RootPanel.AddChild<PlayerAmount>();
			RootPanel.AddChild<RoundCounter>();
		}
	}
}
