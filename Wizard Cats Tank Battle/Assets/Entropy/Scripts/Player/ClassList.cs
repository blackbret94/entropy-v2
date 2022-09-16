using System.Collections.Generic;
using UnityEngine;

namespace Entropy.Scripts.Player
{
    [CreateAssetMenu(fileName = "Class List", menuName = "Entropy/ClassList", order = 1)]
    public class ClassList: ScriptableObject
    {
        public List<ClassDefinition> Classes;

        public ClassDefinition RandomClass()
        {
            return Classes[Random.Range(0, Classes.Count)];
        }

        public ClassDefinition GetClassById(int id)
        {
            int index = GetIndexFromId(id);

            if (index >= Classes.Count)
                return Classes[0];

            return Classes[index];
        }
        
        private int GetIndexFromId(int id)
        {
            return id - 1;
        }

        private int GetIdFromIndex(int index)
        {
            return index + 1;
        }
    }
}