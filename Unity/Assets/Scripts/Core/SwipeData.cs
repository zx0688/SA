using System.Collections.Generic;

namespace Core
{
    public class SwipeData
    {
        public CardMeta Card;

        public List<CardMeta> Choices;
        public ItemMeta Hero;
        public bool LastCard;
        public int FollowPrompt;
        public DeckItem Item;


        public CardData Data;
        public List<ItemTypeData> Conditions;

        public int CurrentChoice;
    }

}