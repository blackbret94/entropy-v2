using System.Collections.Generic;

namespace Vashta.Entropy.TanksExtensions
{
    [System.Serializable]
    public class ScoreboardRowDataSerializableList
    {
        public List<ScoreboardRowDataSerializable> list;

        public ScoreboardRowDataSerializableList()
        {
            list = new List<ScoreboardRowDataSerializable>();
        }

        public void Add(ScoreboardRowDataSerializable newRow)
        {
            list.Add(newRow);
        }
    }
}