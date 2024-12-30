namespace TanksMP
{
    /// <summary>
    /// Available game modes selected per scene.
    /// Used in the AddScore() method for filtering.
    /// </summary>
    public enum GameMode
    {
        /// <summary>
        /// Team Deathmatch
        /// </summary>
        TDM,

        /// <summary>
        /// Capture The Flag
        /// </summary>
        CTF,
        
        /// <summary>
        /// Free For All
        /// </summary>
        FFA,
        
        /// <summary>
        /// King of the Hill
        /// </summary>
        KOTH,
        
        /// <summary>
        /// Mouse Hunt
        /// </summary>
        MH,
        
        /// <summary>
        /// Capture The Flags
        /// </summary>
        CTFS,
        
        /// <summary>
        /// King of the Hills
        /// </summary>
        KOTHS
    }
}