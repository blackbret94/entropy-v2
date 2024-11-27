using UnityEngine;
using UnityEngine.UI;

namespace Vashta.Entropy.UI
{
    public class ButtonEntropy : Button
    {
        public Image rootImage; // Parent image (optional)

        private Image[] _allImages;

        protected override void Start()
        {
            base.Start();
            // Get all child images of the root image
            if (rootImage != null)
            {
                _allImages = rootImage.GetComponentsInChildren<Image>();
            }
        }

        protected override void DoStateTransition(SelectionState state, bool instant)
        {
            base.DoStateTransition(state, instant);

            Color tintColor;
            switch (state)
            {
                case SelectionState.Normal:
                    tintColor = colors.normalColor;
                    break;
                case SelectionState.Highlighted:
                    tintColor = colors.highlightedColor;
                    break;
                case SelectionState.Pressed:
                    tintColor = colors.pressedColor;
                    break;
                case SelectionState.Disabled:
                    tintColor = colors.disabledColor;
                    break;
                default:
                    tintColor = Color.white;
                    break;
            }

            if (_allImages != null)
            {
                foreach (var image in _allImages)
                {
                    if (image != null)
                    {
                        image.color = tintColor;
                    }
                }
            }
        }
    }
}