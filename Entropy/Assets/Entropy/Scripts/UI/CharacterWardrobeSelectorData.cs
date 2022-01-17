namespace Vashta.Entropy.UI
{
    public class CharacterWardrobeSelectorData
    {
        public CharacterWardrobeSelectorData(int itemIndex, int itemCount)
        {
            ItemIndex = itemIndex;
            ItemCount = itemCount;
        }

        public int ItemIndex { get; }
        public int ItemCount { get; }
    }
}