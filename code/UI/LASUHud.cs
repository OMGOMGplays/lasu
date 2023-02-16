namespace LASU.UI 
{
    public partial class LASUHud : HudEntity<RootPanel> 
    {
        public static LASUHud Current;

        public LASUHud() 
        {
            Current = this;

            RootPanel.AddChild<MapVoteScreen>();
            RootPanel.AddChild<PlayerAmount>();
            RootPanel.AddChild<RoundCounter>();
            RootPanel.AddChild<LASUScoreboard<LASUScoreboardEntry>>();
        }
    }
}