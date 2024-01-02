using System.Collections.Generic;
using Entropy.Scripts.Player;
using TanksMP;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Vashta.Entropy.UI.ClassSelectionPanel
{
    public class ClassSelectionInfoPane : GamePanel
    {
        public TextMeshProUGUI ClassName;
        [FormerlySerializedAs("ClassRole")] public TextMeshProUGUI ClassDescription;
        
        public Image ClassPortrait;
        public Image ClassIcon;

        public GameObject ClassCounteredByGO;
        [FormerlySerializedAs("ClassCounter")] public Image ClassCounteredBy;

        public GameObject ClassCountersGO;
        public Image ClassCounters;
        
        public ClassSkillComponent Skill1;
        public ClassSkillComponent Skill2;
        public ClassSkillComponent UltimateSkill;

        public Slider HealthSlider;
        public Slider DamageSlider;
        public Slider MovementSlider;

        public TextMeshProUGUI Role;

        public void SetClass(ClassDefinition definition)
        {
            // Title
            ClassName.text = definition.className;
            ClassName.color = definition.colorPrimary;
            
            // Description
            ClassDescription.text = definition.description;
            ClassDescription.color = definition.colorSecondary;

            // Portrait
            ClassPortrait.sprite = definition.classPortrait;

            // Icon
            ClassIcon.sprite = definition.classIcon;

            // Role
            if (Role)
            {
                Role.text = definition.role;
            }
            
            // Counters
            if (definition.classCounters.Count > 0)
            {
                ClassCountersGO.SetActive(true);
                ClassCounters.sprite = definition.classCounters[0].classIcon;
            }
            else
            {
                // Hide if no counter
                ClassCountersGO.SetActive(false);
            }
            
            // Update countered by
            List<ClassDefinition> counteredByList = definition.GetClassesCounteredBy();
            if (counteredByList.Count > 0)
            {
                ClassCounteredByGO.SetActive(true);
                ClassCounteredBy.sprite = counteredByList[0].classIcon;
            }
            else
            {
                ClassCounteredByGO.SetActive(false);
            }
            
            Bullet bullet = definition.Missile.GetComponent<Bullet>();
            
            // SKILLS
            // On enemy hit
            if (bullet.StatusEffectOnEnemy)
            {
                Skill1.OpenPanel();
                Skill1.Set(bullet.StatusEffectOnEnemy, definition.colorPrimary, definition.colorSecondary);
            }
            else
            {
                Skill1.ClosePanel();
            }

            // On ally hit
            if (bullet.StatusEffectOnAlly)
            {
                Skill2.OpenPanel();
                Skill2.Set(bullet.StatusEffectOnAlly, definition.colorPrimary, definition.colorSecondary);
            }
            else
            {
                Skill2.ClosePanel();
            }
            
            // Ultimate
            if (definition.ultimateSpell != null)
            {
                UltimateSkill.OpenPanel();
                UltimateSkill.Set(definition.ultimateSpell, definition.colorPrimary, definition.colorSecondary);
            }
            else
            {
                UltimateSkill.ClosePanel();
            }
            
            // SLIDERS
            if (HealthSlider)
            {
                HealthSlider.value = definition.healthDisplay;
                HealthSlider.fillRect.GetComponent<Image>().color = definition.colorPrimary;
            }

            if (DamageSlider)
            {
                DamageSlider.value = definition.damageDisplay;
                DamageSlider.fillRect.GetComponent<Image>().color = definition.colorPrimary;
            }

            if (MovementSlider)
            {
                MovementSlider.value = definition.speedDisplay;
                MovementSlider.fillRect.GetComponent<Image>().color = definition.colorPrimary;
            }
        }
    }
}