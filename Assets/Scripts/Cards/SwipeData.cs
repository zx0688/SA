using System.Collections.Generic;
using Data;

namespace Cards
{
    public class SwipeData
    {
        public CardMeta Data;
        //public CardVO cardVO;
        public ChoiceData Left;
        public ChoiceData Right;
        //public List<SkillVO> skills;
        public List<RewardData> Drop;

        public void SetCardData(CardMeta cardData)
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

        public CardMeta NextCard;
        public ActionMeta Action;
    }
}