using Entropy.Scripts.Audio;
using Entropy.Scripts.Player;
using UnityEngine;

namespace Vashta.Entropy.Character
{
    public class ClickToAnimate : MonoBehaviour
    {
        public PlayerAnimator Animator;
        public SfxController SfxController;
        public CharacterAppearance CharacterAppearance;

        private void Update()
        {
            // Check for left mouse button click
            if (Input.GetMouseButtonUp(0))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition); // Create a ray from the camera to the mouse position
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit))
                {
                    // Check if the clicked object is this GameObject
                    if (hit.transform.gameObject == gameObject)
                    {
                        OnClick();
                    }
                }
            }
        }
        
        public void OnClick()
        {
            Animator.PlayRandomAnimation();
            SfxController.PlaySound(CharacterAppearance.Meow.AudioClip);
        }
    }
}