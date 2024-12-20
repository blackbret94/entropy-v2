using CBS;
using UnityEngine;

namespace Vashta.Entropy.Scripts.CBSIntegration
{
    public class ProfileState : MonoBehaviour
    {
        private IProfile ProfileModule { get; set; }
        public string CachedDisplayName { get; private set; }

        void Start()
        {
            ProfileModule = CBSModule.Get<CBSProfile>();
            CachedDisplayName = "";
            ProfileModule.OnAcountInfoGetted += OnAccountInfoGetted;
            ProfileModule.OnDisplayNameUpdated += OnDisplayNameUpdated;
        }

        public void GetActiveUser()
        {
            ProfileModule.GetAccountInfo(OnAccountInfoGetted);
        }
        
        public void GetProfileById(string profileID)
        {
            var getProfileRequest = new CBSGetProfileRequest {
                ProfileID = profileID,
                LoadClan = true,
                LoadEntityId = true,
                LoadLevel = true
            };

            ProfileModule.GetPlayerProfile(getProfileRequest, OnGetPlayerProfile);
        }
        
        private void OnGetPlayerProfile(CBSGetProfileResult result)
        {
            if (result.IsSuccess)
            {
                Debug.Log("Player id = " + result.ProfileID);
                Debug.Log("Entity id = " + result.EntityID);
                Debug.Log("Avatar URL = " + result.AvatarURL);
                Debug.Log("Nickname = " + result.DisplayName);
                Debug.Log("Level = " + result.LevelData.CurrentLevel);
                Debug.Log("Exist in clan ? " + result.ClanData.ExistInClan);
            }
        }
        
        private void OnAccountInfoGetted(CBSGetAccountInfoResult result)
        {
            if (result.IsSuccess)
            {
                CachedDisplayName = result.DisplayName;
                // Debug.Log("Display name = " + result.DisplayName);
                // Debug.Log("Avatar URL = " + result.AvatarUrl);
            }
        }
        
        private void OnDisplayNameUpdated(CBSUpdateDisplayNameResult result)
        {
            if (result.IsSuccess)
            {
                CachedDisplayName = result.DisplayName;
                Debug.Log("User has successfully updated their nickname");
            }
        }
    }
}