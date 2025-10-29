namespace Vashta.Entropy.UI.ReportPlayerPanel
{
    public class PlayerReportInfo
    {
        public string PlayerName { get; private set; }
        public string PlayfabId { get; private set; }

        public PlayerReportInfo(string playerName, string playfabId)
        {
            PlayerName = playerName;
            PlayfabId = playfabId;
        }
    }
}