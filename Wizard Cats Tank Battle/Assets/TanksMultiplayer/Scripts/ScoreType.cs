namespace TanksMP
{
    /// <summary>
    /// Defines the types that could grant points to players or teams.
    /// Used in the AddScore() method for filtering.
    /// </summary>
    public enum ScoreType
    {
        Kill,
        Capture,
        HoldPoint,
        TargetDestroyed
    }
}