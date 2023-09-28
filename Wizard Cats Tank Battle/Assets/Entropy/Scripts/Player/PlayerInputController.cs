using System.Collections.Generic;

namespace Entropy.Scripts.Player
{
    public class PlayerInputController
    {
        private Dictionary<PlayerInputType, IPlayerInputAdapter> _inputAdapterDict;
        private PlayerInputType _currentInputType = PlayerInputType.MKb;

        public PlayerInputController()
        {
            _inputAdapterDict = new();
            _inputAdapterDict.Add(PlayerInputType.MKb, new PlayerInputAdapterMKB());
            _inputAdapterDict.Add(PlayerInputType.Touch, new PlayerInputAdapterTouch());
            _inputAdapterDict.Add(PlayerInputType.Gamepad, new PlayerInputAdapterGamepad());
            
            // SetInputType(PlayerInputType.Gamepad);
        }

        public IPlayerInputAdapter GetAdapter()
        {
            return _inputAdapterDict[_currentInputType];
        }

        public void SetInputType(PlayerInputType type)
        {
            _currentInputType = type;
        }
    }
}