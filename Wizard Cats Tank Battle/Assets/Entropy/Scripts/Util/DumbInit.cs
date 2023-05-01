using System.Collections;
using TanksMP;
using UnityEngine;

namespace Vashta.Entropy.Util
{
    public class DumbInit : MonoBehaviour
    {
        private void Awake()
        {
            StartCoroutine(Init());
        }

        private IEnumerator Init()
        {
            yield return new WaitForSeconds(.1f);
            if (FirstSceneLoaded.Get().IsFirstScene())
            {
                NetworkManagerCustom.StartMatch((NetworkMode)PlayerPrefs.GetInt(PrefsKeys.networkMode));
            }
            else
            {
                Destroy(gameObject);
            }
        }
    }
}