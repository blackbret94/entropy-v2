using CBS;
using CBS.Scriptable;
using CBS.UI;
using TMPro;
using UnityEngine;

namespace Vashta.Entropy.Scripts.CBSIntegration
{
    public class ProfileIcon : MonoBehaviour
    {
        [SerializeField]
        private TextMeshProUGUI NickNameLabel;
        [SerializeField]
        private AvatarDrawer Avatar;

        private IProfile CBSProfile { get; set; }
        private ProfilePrefabs Prefabs { get; set; }

        private void Start()
        {
            Prefabs = CBSScriptable.Get<ProfilePrefabs>();
            CBSProfile = CBSModule.Get<CBSProfile>();
            // subscribe to events
            CBSProfile.OnDisplayNameUpdated += OnDisplayNameUpdated;
            CBSProfile.OnAvatarImageUpdated += OnAvatarImageUpdated;
            // try display cache value
            DisplayName();
            // get actual data from DB
            CBSProfile.GetAccountInfo(OnAccountInfoGetted);

            DrawAvatar();
        }

        private void OnDestroy()
        {
            CBSProfile.OnDisplayNameUpdated -= OnDisplayNameUpdated;
            CBSProfile.OnAvatarImageUpdated -= OnAvatarImageUpdated;
        }

        private void DisplayName()
        {
            NickNameLabel.text = CBSProfile.DisplayName;
        }

        private void DrawAvatar()
        {
            // draw avatar
            var avatarUrl = CBSProfile.AvatarUrl;
            var profileID = CBSProfile.PlayerID;
            Avatar.LoadAvatarFromUrl(avatarUrl, profileID);
        }

        // button click
        public void ShowAccountInfo()
        {
            var windowsPrefab = Prefabs.AccountForm;
            UIView.ShowWindow(windowsPrefab);
        }

        private void OnDisplayNameUpdated(CBSUpdateDisplayNameResult result)
        {
            DisplayName();
        }

        private void OnAccountInfoGetted(CBSGetAccountInfoResult result)
        {
            DisplayName();
        }

        private void OnAvatarImageUpdated(CBSUpdateAvatarUrlResult obj)
        {
            DrawAvatar();
        }
    }
}