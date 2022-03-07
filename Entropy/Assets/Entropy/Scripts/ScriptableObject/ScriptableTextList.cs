using System.Collections.Generic;
using UnityEngine;

namespace Vashta.Entropy.ScriptableObject
{
    [CreateAssetMenu(fileName = "TextList", menuName = "Entropy/TextList", order = 1)]
    public class ScriptableTextList : UnityEngine.ScriptableObject
    {
        public List<string> textList;

        public string this[int index]
        {
            get
            {
                if (textList.Count < index)
                    return textList[index];

                return getRandomString();
            }
        }

        public string getRandomString()
        {
            int index = Random.Range(0, textList.Count);
            return textList[index];
        }
    }
}