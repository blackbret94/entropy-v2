using UnityEngine;

namespace Vashta.Entropy.UI
{
    public class CatNameGenerator
    {
        private static readonly string[] wizardType =
        {
            "Fire", "Water", "Earth", "Wind", "Chrono", "Necro"
        };

        private static readonly string[] catType =
        {
            "Calico", "Tortie", "Tabby", "Persian", "Maine Coon", "Siamese", "Himalayan", "Longhair", "Shorthair",
            "Sphynx", "Kitty", "Cat", "Pussy"
        };

        public string GetRandomName()
        {
            return GetRandomWizardType() + "-" + GetRandomCatType();
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