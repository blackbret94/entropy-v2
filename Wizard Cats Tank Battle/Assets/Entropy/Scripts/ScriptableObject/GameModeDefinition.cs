using UnityEngine;

namespace Vashta.Entropy.ScriptableObject
{
    [CreateAssetMenu(fileName = "Game Mode", menuName = "Entropy/Game Mode", order = 1)]
    public class GameModeDefinition : ScriptableObjectWithID
    {
        public string Title;
        public string Description;
        public Sprite Icon;
        public Sprite IconSmall;
        public TanksMP.GameMode GameMode;
        public int ScoreToWin = 20;

        [Header("Scoring")] 
        public int KillPoints = 1;
        public int CapturePoints = 10;
        public int HoldPointPoints = 1;
    }
}