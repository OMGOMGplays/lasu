namespace LASU 
{
	public partial class LASUGame 
	{
		[Net]
		public string NextMap {get; set;}

		[Net]
		public IDictionary<long, string> MapVotes {get; set;}

		[Net]
		public IList<string> MapOptions {get; set;}

		private async void InitMapCycle() 
		{
			Game.AssertServer();

			NextMap = Game.Server.MapIdent;

			var packages = await Package.FindAsync("type: map game:oop.lasu", 16);
			var maps = packages.Packages.Select(x => x.FullIdent).ToList();

			var pkg = await Package.Fetch(Game.Server.GameIdent, true);
			if (pkg != null) 
			{
				maps.AddRange(pkg.GetMeta<List<string>>("MapList", new()));
			}

			MapOptions = maps.OrderBy(x => Game.Random.Int(9999))
				.Distinct()
				.Where(x => x != Game.Server.MapIdent)
				.Take(5)
				.ToList();
			NextMap = Game.Random.FromList(MapOptions.ToList());
		}

		private async void ChangeMapWithDelay(string mapIdent, float delay) 
		{
			Game.AssertServer();

			delay *= 1000f;
			var timer = 0f;
			while (timer < delay) 
			{
				await Task.Delay(100);

				timer += 100;

				if (timer % 1000 == 0) 
				{
					var timeLeft = (delay - timer) / 1000f;
					// Meddelande
				}
			}

			// Meddelande

			await Task.Delay(3000);

			LASUCmd_ChangeMap(mapIdent);
		}

		[ConCmd.Server]
		public static void LASUCmd_ChangeMap(string mapIdent) 
		{
			Game.ChangeLevel(mapIdent);
		}

		[ConCmd.Server]
		public static void LASUCmd_SetMapVote(string mapIdent) 
		{
			if (!ConsoleSystem.Caller.IsValid()) return;

			if (Instance.MapVotes.TryGetValue(ConsoleSystem.Caller.SteamId, out var vote) && vote == mapIdent) return;

			Instance.MapVotes[ConsoleSystem.Caller.SteamId] = mapIdent;

			var votemap = new Dictionary<string, int>();
			Instance.MapVotes.Values.ToList().ForEach(x => votemap.Add(x, 0));
			foreach (var kvp in Instance.MapVotes) 
			{
				votemap[kvp.Value]++;
			}
			Instance.NextMap = votemap.OrderByDescending(x => x.Value).First().Key;

			// Meddelande
		}
	}
}
