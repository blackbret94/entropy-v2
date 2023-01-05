using CBS;
using UnityEngine;

namespace Vashta.Entropy.UI
{
    public class ProfileBadgePanel : GamePanel
    {
        private IProfile ProfileModule { get; set; }
        
        void Start()
        {
            ProfileModule = CBSModule.Get<CBSProfile>();
            ProfileModule.OnAcountInfoGetted += OnAcountInfoGetted;
        }
        
        private void OnAcountInfoGetted(CBSGetAccountInfoResult result)
        {
            if (result.IsSuccess)
            {
                Debug.Log("User information has been updated from the server");
            }
        }
    }
}