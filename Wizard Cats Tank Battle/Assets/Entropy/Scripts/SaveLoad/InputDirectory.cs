using UnityEngine;

namespace Vashta.Entropy.SaveLoad
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
            UI_Right,
            ClosePanel;

    }
}