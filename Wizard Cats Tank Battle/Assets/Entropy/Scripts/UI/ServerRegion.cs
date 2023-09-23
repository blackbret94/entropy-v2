namespace Vashta.Entropy.UI
{
    [System.Serializable]
    public class ServerRegion
    {
        public string Region;
        public string HostedIn;
        public string Token;

        public string Stringify()
        {
            return Region + "(" + HostedIn + ")";
        }
    }
}