using System.Collections.Generic;

namespace Core
{
    public class SwipeData
    {
        public CardMeta Card;

        public CardMeta Left;
        public CardMeta Right;
        public ItemMeta Hero;
        public bool LastCard;
        public int FollowPrompt;


        public CardData Data;
        public List<ItemTypeData> Conditions;

        public int Choice;
    }

}