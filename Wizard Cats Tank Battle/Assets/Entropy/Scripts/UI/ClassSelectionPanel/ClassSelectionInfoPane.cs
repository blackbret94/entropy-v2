using System.Collections.Generic;
using Entropy.Scripts.Player;
using TanksMP;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Vashta.Entropy.UI.ClassSelectionPanel
{
    public class ClassSelectionInfoPane : MonoBehaviour
    {
        public TextMeshProUGUI ClassName;
        [FormerlySerializedAs("ClassRole")] public TextMeshProUGUI ClassDescription;
        
        
        public Image ClassPortrait;
        public Image ClassIcon;
        public Image ClassCounter;
        public Image ClassCounters;
        public ClassSkillComponent Skill1;
        public ClassSkillComponent Skill2;

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

            // Counters
            if (definition.classCounters.Count > 0)
            {
                ClassCounters.sprite = definition.classCounters[0].classIcon;
            }
            
            // Update countered by
            List<ClassDefinition> counteredByList = definition.GetClassesCounteredBy();
            if (counteredByList.Count > 0)
            {
                ClassCounter.sprite = counteredByList[0].classIcon;
            }
            
            Bullet bullet = definition.Missile.GetComponent<Bullet>();
            
            // Skills
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
        }
    }
}