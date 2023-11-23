using System.Collections.Generic;
using Meta;

namespace Core
{
    public class SwipeData
    {
        public CardMeta CurrentCardMeta;
        public CardVO CurrentCardVO;

        public List<ConditionMeta> Conditions;

        public ChoiceData Left = new ChoiceData();
        public ChoiceData Right = new ChoiceData();

        public int Choice;

        public CardMeta PrevLayer;

        public ChoiceData GetChoiceData(int choice) => choice == Swipe.LEFT_CHOICE ? Left : Right;

    }

}

public class ChoiceData
{
    public CardMeta NextCard;

    //visible
    public string Text;
    public string Icon;
    public List<RewardMeta> Reward;
}