using System.Collections.Generic;
using UnityEngine;

namespace Vashta.Entropy.ScriptableObject
{
    [CreateAssetMenu(fileName = "Cat Name list", menuName = "Entropy/Cat Name List", order = 1)]
    public class CatNameList: UnityEngine.ScriptableObject
    {
        public List<string> CatNames;

        public string GetRandomName()
        {
            int index = Random.Range(0, CatNames.Count);
            return CatNames[index];
        }
    }
}