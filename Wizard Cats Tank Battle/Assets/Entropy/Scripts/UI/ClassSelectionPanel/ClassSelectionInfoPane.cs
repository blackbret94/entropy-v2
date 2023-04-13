using System.Collections.Generic;
using Entropy.Scripts.Player;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Vashta.Entropy.UI.ClassSelectionPanel
{
    public class ClassSelectionInfoPane : MonoBehaviour
    {
        public TextMeshProUGUI ClassName;
        public TextMeshProUGUI ClassRole;
        
        public Image ClassPortrait;
        public Image ClassIcon;
        public Image ClassCounter;
        public Image ClassCounters;
        public CharacterClassCard AttributeSliders;

        public void SetClass(ClassDefinition definition)
        {
            // Update text
            ClassName.text = definition.className;
            
            // Update portrait
            ClassPortrait.sprite = definition.classPortrait;

            // Update Icon
            ClassIcon.sprite = definition.classIcon;

            // Update counters
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

            // Update attributes
            AttributeSliders.SetClassForCard(definition);
            
            // Role
            ClassRole.text = "Role: " + definition.role;

            // TODO: Change colors
        }
    }
}