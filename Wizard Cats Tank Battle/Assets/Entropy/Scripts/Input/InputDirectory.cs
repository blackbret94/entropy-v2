using UnityEngine;

namespace Vashta.Entropy.GameInput
{
    [CreateAssetMenu(fileName = "Input Code Directory", menuName = "Entropy/Input Code Directory", order = 1)]
    public class InputDirectory : UnityEngine.ScriptableObject
    {
        public InputCode Fire,
            DropCollectible,
            UsePowerup,
            UseUltimate,
            OpenSettings,
            OpenClassSelection,
            OpenScoreboard,
            UI_Up,
            UI_Down,
            UI_Left,
            UI_Right;

        [Tooltip("Bottom button (A)")]
        public InputCode UI_Primary;
        [Tooltip("Right button (B)")]
        public InputCode UI_Secondary;
        [Tooltip("Left button (X)")]
        public InputCode UI_Tertiary;
        [Tooltip("Top button (Y)")]
        public InputCode UI_Quatrary;
        
        public InputCode ClosePanel;

    }
}