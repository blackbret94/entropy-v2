using UnityEngine;
using Vashta.Entropy.ScriptableObject;

namespace Vashta.Entropy.UI
{
    public class CatNameGenerator: MonoBehaviour
    {
        public CatNameList CatNameList;
        public float RealNamePercent = 80;
        
        private static readonly string[] wizardType =
        {
            "Fire", "Water", "Earth", "Wind", "Chrono", "Necro"
        };

        private static readonly string[] catType =
        {
            "Calico", "Tortie", "Tabby", "Persian", "Maine Coon", "Siamese", "Himalayan", "Longhair", "Shorthair",
            "Sphynx", "Kitty", "Cat"
        };

        public string GetRandomName()
        {
            float r = Random.Range(0f, 100f);

            if (r < RealNamePercent)
            {
                return CatNameList.GetRandomName();
            }
            else
            {
                return GetRandomWizardType() + "-" + GetRandomCatType();
            }
        }

        private string GetRandomWizardType()
        {
            return wizardType[Random.Range(0, wizardType.Length)];
        }

        private string GetRandomCatType()
        {
            return catType[Random.Range(0, catType.Length)];
        }
    }
}