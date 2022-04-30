using System.Collections.Generic;

public class SwipeData
{
    public CardData CardData;
    //public CardVO cardVO;
    public ChoiceData Left;
    public ChoiceData Right;
    //public List<SkillVO> skills;
    public List<RewardData> Drop;

    public void SetCardData(CardData cardData)
    {
        this.Left = new ChoiceData();
        this.Right = new ChoiceData();
        this.CardData = cardData;
    }
}

public class ChoiceData
{
    public bool Available;
    public int Chance;

    public CardData NextCard;

    private ActionData _action;
    public ActionData action
    {
        get { return _action; }
        set { _action = value; }
    }
}