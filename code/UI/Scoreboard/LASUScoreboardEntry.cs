namespace LASU.UI
{
	public partial class LASUScoreboardEntry : Panel 
	{
		public IClient Client;

		public Label PlayerName;
		public Label Map;
		public Label Deaths;
		public Label Ping;

		public LASUScoreboardEntry() 
		{
			AddClass("entry");

			PlayerName = Add.Label("PlayerName", "name");
			Deaths = Add.Label("", "deaths");
			Ping = Add.Label("", "ping");
		}

		RealTimeSince TimeSinceUpdate = 0;

		public override void Tick() 
		{
			base.Tick();

			if (!IsVisible) return;

			if (!Client.IsValid()) return;

			if (TimeSinceUpdate < 0.1f) return;

			TimeSinceUpdate = 0;
			UpdateData();
		}

		public virtual void UpdateData() 
		{
			PlayerName.Text = Client.Name;
			Deaths.Text = Client.GetInt("deaths").ToString();
			Ping.Text = Client.Ping.ToString();
		}

		public virtual void UpdateFrom(IClient client) 
		{
			Client = client;
			UpdateData();
		}
	}
}
