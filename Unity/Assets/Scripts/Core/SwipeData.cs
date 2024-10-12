using System.Collections.Generic;

namespace Core
{
    public class SwipeData
    {
        public bool ChoiceMode;
        public CardMeta Card;

        public List<CardMeta> Choices;
        public ItemMeta Hero;
        public bool LastCard;
        public int FollowPrompt;


        public CardData Data;
        public List<ItemTypeData> Conditions;

        public int CurrentChoice;
    }

}