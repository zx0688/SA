using System.Collections.Generic;
using Meta;

namespace Cards
{
    public class SwipeData
    {
        public CardData Data;
        //public CardVO cardVO;
        public ChoiceData Left;
        public ChoiceData Right;
        //public List<SkillVO> skills;
        public List<RewardData> Drop;

        public void SetCardData(CardData cardData)
        {
            this.Left = new ChoiceData();
            this.Right = new ChoiceData();
            this.Data = cardData;
        }
    }

    public class ChoiceData
    {
        public bool Available;
        public int Chance;

        public CardData NextCard;
        public ActionData Action;
    }
}