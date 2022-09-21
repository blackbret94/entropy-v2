using System.Collections.Generic;

namespace Vashta.Entropy.ScriptableObject
{
    /// <summary>
    /// Scriptable Object with ID Dictionary
    /// </summary>
    public class SOIDDictionary
    {
        private Dictionary<string, ScriptableObjectWithID> _dict;

        public SOIDDictionary()
        {
            _dict = new Dictionary<string, ScriptableObjectWithID>();
        }

        public ScriptableObjectWithID this[string id]
        {
            get => _dict[id];
            set => _dict[id] = value;
        }

    }
}