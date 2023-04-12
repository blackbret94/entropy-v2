using System;
using Entropy.Scripts.Player;
using TanksMP;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Vashta.Entropy.UI
{
    public class CharacterClassCard : MonoBehaviour
    {
        public ClassDefinition ClassDefinition; // Deprecated
        public Image ClassIcon; // Deprecated
        public TextMeshProUGUI TitleText; // Deprecated
        public Slider HealthSlider;
        public Slider FireRateSlider;
        public Slider SpeedSlider;
        public Slider DamageOnCollisionSlider;
        public Slider ArmorSlider;

        private static readonly Tuple<float, float> healthRange = new(0f,22f);
        private static readonly Tuple<float, float> fireRateRange = new(.2f, 1.2f);
        private static readonly Tuple<float, float> moveSpeedRange = new(2f, 12f);
        private static readonly Tuple<float, float> damageOnCollisionRange = new(0f, 17f);
        private static readonly Tuple<float, float> armorRange = new(0f, 7f);

        public void Start()
        {
            if (ClassDefinition == null)
                return;
            
            ClassIcon.sprite = ClassDefinition.classIcon;
            TitleText.text = ClassDefinition.className;
            SetSlider(HealthSlider, ClassDefinition.maxHealth, healthRange.Item1, healthRange.Item2);
            SetSlider(FireRateSlider, fireRateRange.Item2-ClassDefinition.fireRate, fireRateRange.Item1, fireRateRange.Item2);
            SetSlider(SpeedSlider, ClassDefinition.moveSpeed, moveSpeedRange.Item1, moveSpeedRange.Item2);
            SetSlider(DamageOnCollisionSlider, ClassDefinition.damageAmtOnCollision, damageOnCollisionRange.Item1, damageOnCollisionRange.Item2);
            SetSlider(ArmorSlider, ClassDefinition.armor, armorRange.Item1, armorRange.Item2);
        }
        
        public void SetClassForCard(ClassDefinition classDefinition){
            SetSlider(HealthSlider, classDefinition.maxHealth, healthRange.Item1, healthRange.Item2);
            SetSlider(FireRateSlider, fireRateRange.Item2-classDefinition.fireRate, fireRateRange.Item1, fireRateRange.Item2);
            SetSlider(SpeedSlider, classDefinition.moveSpeed, moveSpeedRange.Item1, moveSpeedRange.Item2);
            SetSlider(DamageOnCollisionSlider, classDefinition.damageAmtOnCollision, damageOnCollisionRange.Item1, damageOnCollisionRange.Item2);
            SetSlider(ArmorSlider, classDefinition.armor, armorRange.Item1, armorRange.Item2);
        }

        private void SetSlider(Slider slider, float value, float min, float max)
        {
            float valAdjusted = value - min;
            float range = max - min;
            float normalized = valAdjusted / range;

            slider.value = normalized;
        }

        public void SetClass()
        {
            Player player = GameManager.GetInstance().localPlayer;
            player.SetClass(ClassDefinition);
        }
    }
}